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

        [Header("Bonus Mode")] public float secondsBetweenBonusObjects;
        public float timeDelayBeforeBonusMode = 4;
        public int minBonusAmount = 30;
        public int maxBonusAmount = 51;

        [Header("Exit Information")] public float timeDelayBeforeExit = 3;

        [Header("Managers")] public EquationManager equationManager;

        [Header("Holders")] public Transform blockHolder;
        public TextMeshPro textDisplay;

        private float _currentTime;
        private int _currentCounter;
        private List<EquationBlockController> _cubes;

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
            _cubes = new List<EquationBlockController>();
            _currentTime = secondsBetweenEachObject;

            SetSpawnerState(SpawnerState.EquationMode);
        }

        private void Update()
        {
            _currentTime -= Time.deltaTime;
            switch (_spawnerState)
            {
                case SpawnerState.EquationMode:
                {
                    if (_currentTime <= 0)
                    {
                        SpawnEquation();
                        _currentTime = secondsBetweenEachObject;
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

        public void NotifyParentCollision()
        {
            foreach (EquationBlockController cubeMovementAndDestruction in _cubes)
            {
                cubeMovementAndDestruction.SetParentCollided();
            }
        }

        #endregion

        #region External Functions

        public void SpawnNextEquation()
        {
            if (_spawnerState != SpawnerState.EquationMode)
            {
                return;
            }

            SpawnEquation();
            _currentTime = secondsBetweenEachObject;
        }

        public bool ReduceBonusValueCheckAndActivateEnd(int amount)
        {
            if (amount > _currentCounter)
            {
                return false;
            }

            _currentCounter -= amount;
            textDisplay.text = _currentCounter.ToString();

            return true;
        }

        #endregion

        #region Utility Functions

        private void EndGame()
        {
            textDisplay.text = "Bonus Complete. Game Over...";
            _currentTime = timeDelayBeforeExit;
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

            Transform spawnPoint = spawnPoints[Mathf.FloorToInt(Random.value * spawnPoints.Count)];
            GameObject numberObject = equationManager.GetRandomNumberGameObject(answer, TagManager.BonusAnswer);

            EquationBlockController cubeController = numberObject.GetComponent<EquationBlockController>();
            cubeController.SetEquationStatus(null, answer, false);

            numberObject.transform.position = spawnPoint.position;
            numberObject.transform.SetParent(blockHolder);
        }

        private void SpawnEquation()
        {
            _cubes.Clear();

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

            while (selectedSpawnPoints.Count > 0)
            {
                int randomIndex = Mathf.FloorToInt(Random.value * selectedSpawnPoints.Count);
                Transform spawnTransform = selectedSpawnPoints[randomIndex];

                if (!correctShown)
                {
                    GameObject correctGameObject = equationManager.CreateBasicEquation();
                    correctGameObject.transform.position = spawnTransform.position;
                    correctGameObject.transform.SetParent(blockHolder);

                    string equation = equationManager.LastEquation;
                    string answer = equationManager.LastAnswer;

                    // Display equation the text to the UI
                    textDisplay.text = equation;
                    usedNumbers.Add(answer);

                    EquationBlockController cubeController = correctGameObject.GetComponent<EquationBlockController>();
                    cubeController.SetEquationStatus(equation, answer, true);
                    cubeController.SetParent(this);
                    _cubes.Add(cubeController);

                    correctShown = true;
                }
                else
                {
                    string answerString = equationManager.LastAnswer;
                    int answerDigits = $"{Mathf.Abs(int.Parse(answerString))}".Length; // Very Bad. But OK for Prototype

                    var incorrectObject = equationManager.GetRandomDigitCountNumber(answerDigits, TagManager.InCorrectAnswer, usedNumbers);
                    usedNumbers.Add(incorrectObject.Item2);

                    GameObject incorrectGameObject = incorrectObject.Item1;
                    incorrectGameObject.transform.position = spawnTransform.position;
                    incorrectGameObject.transform.SetParent(blockHolder);

                    string equation = equationManager.LastEquation;
                    string answer = equationManager.LastAnswer;

                    EquationBlockController cubeController = incorrectGameObject.GetComponent<EquationBlockController>();
                    cubeController.SetEquationStatus(equation, answer, false);
                    cubeController.SetParent(this);
                    _cubes.Add(cubeController);
                }

                selectedSpawnPoints.RemoveAt(randomIndex);
            }

            _currentCounter += 1;
        }

        private void SetSpawnerState(SpawnerState spawnerState) => _spawnerState = spawnerState;

        #endregion
    }
}