using System;
using System.Collections.Generic;
using System.Linq;
using Blocks;
using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Random = UnityEngine.Random;

namespace Equations
{
    public class EquationSpawnerBase : MonoBehaviour
    {
        [Header("Spawn Positions")] public List<Transform> spawnPoints;
        public List<Transform> bonusSpawnPoints;

        [Header("Spawn Data")] public int totalSpawnPointsToSelect = 4;
        public float secondsBetweenEachObject;
        public int totalEquationsToSpawn;
        public float initialSpawnDelay = 3;
        public bool initialSingleSpawn;

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

        protected float _currentTime;
        protected int _currentEquationsSpawnedCount;

        private float _currentSpawnBetweenTime;
        protected float _currentObjectMovementSpeed;

        private bool _initialEquationSpawned;

        protected List<Transform> _selectedSpawnPoints;
        protected HashSet<string> _usedNumbers;

        protected enum SpawnerState
        {
            EquationMode,
            BonusModeCountDown,
            BonusMode,
            EndMode
        }

        private SpawnerState _spawnerState;

        #region Unity Functions

        protected virtual void Start()
        {
            _currentSpawnBetweenTime = secondsBetweenEachObject;
            _currentObjectMovementSpeed = initialMovementSpeed;

            _currentTime = initialSpawnDelay;
            _initialEquationSpawned = false;

            _selectedSpawnPoints = new List<Transform>();
            _usedNumbers = new HashSet<string>();

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
                        if (!initialSingleSpawn || !_initialEquationSpawned)
                        {
                            SpawnEquation();
                        }

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

                        _currentEquationsSpawnedCount = Random.Range(minBonusAmount, maxBonusAmount);
                        textDisplay.text = _currentEquationsSpawnedCount.ToString();
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

        public virtual void SpawnNextEquation()
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
            if (amount > _currentEquationsSpawnedCount)
            {
                return false;
            }

            _currentEquationsSpawnedCount -= amount;
            textDisplay.text = _currentEquationsSpawnedCount.ToString();

            if (_currentEquationsSpawnedCount == 0)
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
                SceneManager.LoadScene(4);
            }
        }

        protected virtual void SpawnBonusBlocks()
        {
            int randomNumber = Random.Range(1, 10);
            string answer = randomNumber.ToString();

            Transform spawnPoint = bonusSpawnPoints[Mathf.FloorToInt(Random.value * bonusSpawnPoints.Count)];
            GameObject numberObject = equationAndBlockGenerator.GetRandomNumberGameObject(answer, TagManager.BonusAnswer, BlockType.ForwardMovementBlock);

            EquationBlockController cubeController = numberObject.GetComponent<EquationBlockController>();
            cubeController.SetEquationStatus(-1, null, answer, answer, true);
            cubeController.SetMovementSpeed(_currentObjectMovementSpeed);

            numberObject.transform.position = spawnPoint.position;
            numberObject.transform.SetParent(blockHolder);
        }

        protected virtual void SpawnEquation()
        {
            if (_currentEquationsSpawnedCount >= totalEquationsToSpawn)
            {
                SetSpawnerState(SpawnerState.BonusModeCountDown);
                _currentTime = timeDelayBeforeBonusMode;
                return;
            }

            bool correctShown = false;

            _selectedSpawnPoints.Clear();
            _usedNumbers.Clear();

            _selectedSpawnPoints = spawnPoints.ToList();
            _selectedSpawnPoints = _selectedSpawnPoints.Shuffle().Take(totalEquationsToSpawn).ToList();

            GameObject parentGameObject = new GameObject();
            parentGameObject.transform.SetParent(blockHolder);
            ParentBlockController parentBlockController = parentGameObject.AddComponent<ParentBlockController>();

            while (_selectedSpawnPoints.Count > 0)
            {
                int randomIndex = Mathf.FloorToInt(Random.value * _selectedSpawnPoints.Count);
                Transform spawnTransform = _selectedSpawnPoints[randomIndex];

                if (!correctShown)
                {
                    GameObject correctGameObject = equationAndBlockGenerator.CreateBasicEquation(BlockType.JumpingBlock);
                    correctGameObject.transform.position = spawnTransform.position;
                    correctGameObject.transform.SetParent(parentGameObject.transform);

                    string equation = equationAndBlockGenerator.LastEquation;
                    string answer = equationAndBlockGenerator.LastAnswer;

                    // Display equation the text to the UI
                    textDisplay.text = equation;
                    _usedNumbers.Add(answer);

                    EquationBlockController cubeController = correctGameObject.GetComponent<EquationBlockController>();
                    cubeController.SetEquationStatus(_currentEquationsSpawnedCount, equation, answer, answer, true);

                    cubeController.SetMovementSpeed(_currentObjectMovementSpeed);
                    cubeController.SetParent(parentBlockController);
                    parentBlockController.AddEquationBlock(cubeController);

                    correctShown = true;
                }
                else
                {
                    string answerString = equationAndBlockGenerator.LastAnswer;
                    int answerDigits = $"{Mathf.Abs(int.Parse(answerString))}".Length; // Very Bad. But OK for Prototype

                    var incorrectObject = equationAndBlockGenerator
                        .GetRandomDigitCountNumber(answerDigits, TagManager.InCorrectAnswer, _usedNumbers, BlockType.JumpingBlock);
                    _usedNumbers.Add(incorrectObject.Item2);

                    GameObject incorrectGameObject = incorrectObject.Item1;
                    incorrectGameObject.transform.position = spawnTransform.position;
                    incorrectGameObject.transform.SetParent(parentGameObject.transform);

                    string equation = equationAndBlockGenerator.LastEquation;
                    string answer = equationAndBlockGenerator.LastAnswer;

                    EquationBlockController cubeController = incorrectGameObject.GetComponent<EquationBlockController>();
                    cubeController.SetEquationStatus(_currentEquationsSpawnedCount, equation, answer, incorrectObject.Item2, false);

                    cubeController.SetMovementSpeed(_currentObjectMovementSpeed);
                    cubeController.SetParent(parentBlockController);
                    parentBlockController.AddEquationBlock(cubeController);
                }

                _selectedSpawnPoints.RemoveAt(randomIndex);
            }

            _currentEquationsSpawnedCount += 1;
        }

        protected void SetSpawnerState(SpawnerState spawnerState) => _spawnerState = spawnerState;

        #endregion
    }
}