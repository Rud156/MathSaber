using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using General;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Random = UnityEngine.Random;

namespace Equations
{
    public class EquationSpawner : MonoBehaviour
    {
        [Header("Spawn Positions")] public List<Transform> spawnPoints;
        public List<Transform> bonusSpawnPoints;

        [Header("Spawn Data")] public int totalSpawnPointsToSelect = 4;
        public float secondsBetweenEachObject;
        public int totalEquationsToSpawn;
        public float initialSpawnDelay = 3;

        [Header("Speed Run")] public bool enableSpeedRunMode;
        public float timeChangePerBlockHit = 0.15f;
        public float minSpawnTime = 1.5f;
        public float initialMovementSpeed = 4;
        public float movementSpeedChangeAmount = 0.5f;

        [Header("Bonus Mode")] public float secondsBetweenBonusObjects;
        public float timeDelayBeforeBonusMode = 4;
        public int minBonusAmount = 30;
        public int maxBonusAmount = 51;

        [Header("Exit Information")] public float timeDelayBeforeExit = 3;

        [Header("Managers")] public EquationAndBlockGenerator equationAndBlockGenerator;

        [Header("Holders")] public Transform blockHolder;
        public TextMeshPro textDisplay;

        private float _currentTime;
        private int _currentCounter;

        private float _currentSpawnBetweenTime;
        private float _currentObjectMovementSpeed;

        private bool _initialEquationSpawned;

        private enum SpawnerState
        {
            EquationMode,
            BonusModeCountDown,
            BonusMode,
            EndMode
        }

        private SpawnerState _spawnerState;

        #region Unity Functions

        private void Start()
        {
            _currentSpawnBetweenTime = secondsBetweenEachObject;
            _currentObjectMovementSpeed = initialMovementSpeed;

            _currentTime = initialSpawnDelay;
            _initialEquationSpawned = false;

            SetSpawnerState(SpawnerState.EquationMode);
        }

        private void Update()
        {
            _currentTime -= Time.deltaTime;
            switch (_spawnerState)
            {
                case SpawnerState.EquationMode:
                {
                    if (_currentTime <= 0 && !_initialEquationSpawned)
                    {
                        SpawnEquation();

                        _currentTime = _currentSpawnBetweenTime;
                        _initialEquationSpawned = true;
                    }
                }
                    break;

                case SpawnerState.BonusModeCountDown:
                {
                    UpdateCountDownBonusTimer();

                    if (_currentTime <= 0)
                    {
                        SetSpawnerState(SpawnerState.BonusMode);
                        _currentTime = secondsBetweenBonusObjects;

                        _currentCounter = Random.Range(minBonusAmount, maxBonusAmount);
                        textDisplay.text = _currentCounter.ToString();
                    }
                }
                    break;

                case SpawnerState.BonusMode:
                {
                    if (_currentTime <= 0)
                    {
                        SpawnBonusBlocks();
                        _currentTime = secondsBetweenBonusObjects;
                    }
                }
                    break;

                case SpawnerState.EndMode:
                    UpdateExitMode();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region External Functions

        public void IncrementSpeed()
        {
            if (!enableSpeedRunMode)
            {
                return;
            }

            if (_currentSpawnBetweenTime - timeChangePerBlockHit < minSpawnTime)
            {
                _currentSpawnBetweenTime = minSpawnTime;
            }
            else
            {
                _currentSpawnBetweenTime -= timeChangePerBlockHit;
            }

            _currentObjectMovementSpeed += movementSpeedChangeAmount;
        }

        public void DecrementSpeed()
        {
            if (!enableSpeedRunMode)
            {
                return;
            }

            if (_currentSpawnBetweenTime + timeChangePerBlockHit > secondsBetweenEachObject)
            {
                _currentSpawnBetweenTime = secondsBetweenEachObject;
            }
            else
            {
                _currentSpawnBetweenTime += timeChangePerBlockHit;
            }

            if (_currentObjectMovementSpeed - movementSpeedChangeAmount < initialMovementSpeed)
            {
                _currentObjectMovementSpeed = initialMovementSpeed;
            }
            else
            {
                _currentObjectMovementSpeed -= movementSpeedChangeAmount;
            }
        }

        public void SpawnNextEquation()
        {
            if (_spawnerState != SpawnerState.EquationMode)
            {
                return;
            }

            SpawnEquation();
            _currentTime = _currentSpawnBetweenTime;
        }

        public bool ReduceBonusValueCheckAndActivateEnd(int amount)
        {
            if (amount > _currentCounter)
            {
                return false;
            }

            _currentCounter -= amount;
            textDisplay.text = _currentCounter.ToString();

            if (_currentCounter == 0)
            {
                EndGame();
            }

            return true;
        }

        #endregion

        #region Utility Functions

        private void EndGame()
        {
            textDisplay.text = "Bonus Complete. Game Over...";
            _currentTime = timeDelayBeforeExit;

            SetSpawnerState(SpawnerState.EndMode);
        }

        private void UpdateCountDownBonusTimer()
        {
            int timeLeft = Mathf.FloorToInt(_currentTime);
            textDisplay.text = $"Bonus Mode Starts In: {timeLeft}";
        }

        private void UpdateExitMode()
        {
            if (_currentTime <= 0)
            {
                SceneManager.LoadScene(2);
            }
        }

        private void SpawnBonusBlocks()
        {
            int randomNumber = Random.Range(1, 10);
            string answer = randomNumber.ToString();

            Transform spawnPoint = bonusSpawnPoints[Mathf.FloorToInt(Random.value * bonusSpawnPoints.Count)];
            GameObject numberObject = equationAndBlockGenerator.GetRandomNumberGameObject(answer, TagManager.BonusAnswer);

            EquationBlockController cubeController = numberObject.GetComponent<EquationBlockController>();
            cubeController.SetEquationStatus(null, answer, false);
            cubeController.SetMovementSpeed(_currentObjectMovementSpeed);

            numberObject.transform.position = spawnPoint.position;
            numberObject.transform.SetParent(blockHolder);
        }

        private void SpawnEquation()
        {
            if (_currentCounter >= totalEquationsToSpawn)
            {
                SetSpawnerState(SpawnerState.BonusModeCountDown);
                _currentTime = timeDelayBeforeBonusMode;
                return;
            }

            bool correctShown = false;

            // Allocations in Update is very bad...
            List<Transform> spawnPointsCopy = spawnPoints.ToList();
            List<Transform> selectedSpawnPoints = spawnPointsCopy.Shuffle().Take(totalSpawnPointsToSelect).ToList();
            HashSet<string> usedNumbers = new HashSet<string>();

            GameObject parentGameObject = new GameObject();
            parentGameObject.transform.SetParent(blockHolder);
            ParentBlockController parentBlockController = parentGameObject.AddComponent<ParentBlockController>();

            while (selectedSpawnPoints.Count > 0)
            {
                int randomIndex = Mathf.FloorToInt(Random.value * selectedSpawnPoints.Count);
                Transform spawnTransform = selectedSpawnPoints[randomIndex];

                if (!correctShown)
                {
                    GameObject correctGameObject = equationAndBlockGenerator.CreateBasicEquation();
                    correctGameObject.transform.position = spawnTransform.position;
                    correctGameObject.transform.SetParent(parentGameObject.transform);

                    string equation = equationAndBlockGenerator.LastEquation;
                    string answer = equationAndBlockGenerator.LastAnswer;

                    // Display equation the text to the UI
                    textDisplay.text = equation;
                    usedNumbers.Add(answer);

                    EquationBlockController cubeController = correctGameObject.GetComponent<EquationBlockController>();
                    cubeController.SetEquationStatus(equation, answer, true);

                    cubeController.SetMovementSpeed(_currentObjectMovementSpeed);
                    cubeController.SetParent(parentBlockController);
                    parentBlockController.AddEquationBlock(cubeController);

                    correctShown = true;
                }
                else
                {
                    string answerString = equationAndBlockGenerator.LastAnswer;
                    int answerDigits = $"{Mathf.Abs(int.Parse(answerString))}".Length; // Very Bad. But OK for Prototype

                    var incorrectObject = equationAndBlockGenerator.GetRandomDigitCountNumber(answerDigits, TagManager.InCorrectAnswer, usedNumbers);
                    usedNumbers.Add(incorrectObject.Item2);

                    GameObject incorrectGameObject = incorrectObject.Item1;
                    incorrectGameObject.transform.position = spawnTransform.position;
                    incorrectGameObject.transform.SetParent(parentGameObject.transform);

                    string equation = equationAndBlockGenerator.LastEquation;
                    string answer = equationAndBlockGenerator.LastAnswer;

                    EquationBlockController cubeController = incorrectGameObject.GetComponent<EquationBlockController>();
                    cubeController.SetEquationStatus(equation, answer, false);

                    cubeController.SetMovementSpeed(_currentObjectMovementSpeed);
                    cubeController.SetParent(parentBlockController);
                    parentBlockController.AddEquationBlock(cubeController);
                }

                selectedSpawnPoints.RemoveAt(randomIndex);
            }

            _currentCounter += 1;
        }

        private void SetSpawnerState(SpawnerState spawnerState) => _spawnerState = spawnerState;

        #endregion
    }
}