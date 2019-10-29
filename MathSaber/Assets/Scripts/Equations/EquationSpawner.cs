using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using General;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Equations
{
    public class EquationSpawner : MonoBehaviour
    {
        [Header("Spawn Positions")] public List<Transform> spawnPoints;

        [Header("Spawn Data")] public int totalSpawnPointsToSelect = 4;
        public float secondsBetweenEachObject;
        public float secondsBetweenBonusObjects;
        public int totalEquationsToSpawn;

        [Header("Managers")] public EquationManager equationManager;

        [Header("Holders")] public Transform blockHolder;

        private float _currentTime;
        private List<EquationBlockController> _cubes;

        private int _currentEquationsCount;

        private enum SpawnerState
        {
            EquationMode,
            BonusMode
        }

        private SpawnerState _spawnerState;

        #region Unity Functions

        private void Start()
        {
            _cubes = new List<EquationBlockController>();
            SetSpawnerState(SpawnerState.EquationMode);
        }

        private void Update()
        {
            _currentTime -= Time.deltaTime;
            if (_currentTime <= 0)
            {
                switch (_spawnerState)
                {
                    case SpawnerState.EquationMode:
                        SpawnEquation();
                        _currentTime = secondsBetweenEachObject;
                        break;

                    case SpawnerState.BonusMode:
                        SpawnBonusBlocks();
                        _currentTime = secondsBetweenBonusObjects;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
            if (_spawnerState == SpawnerState.BonusMode)
            {
                return;
            }

            SpawnEquation();
            _currentTime = secondsBetweenEachObject;
        }

        #endregion

        #region Utility Functions

        private void SpawnBonusBlocks()
        {
            // TODO: Implement this...
        }

        private void SpawnEquation()
        {
            _cubes.Clear();

            bool correctShown = false;
            List<Transform> spawnPointsCopy = spawnPoints.ToList();
            List<Transform> selectedSpawnPoints = spawnPointsCopy.Shuffle().Take(totalSpawnPointsToSelect).ToList();

            while (selectedSpawnPoints.Count > 0)
            {
                int randomIndex = Mathf.FloorToInt(Random.value * selectedSpawnPoints.Count);
                Transform spawnTransform = selectedSpawnPoints[randomIndex];

                if (!correctShown)
                {
                    GameObject correctGameObject = equationManager.CreateBasicEquationAndAddToUI();
                    correctGameObject.transform.position = spawnTransform.position;
                    correctGameObject.transform.SetParent(blockHolder);

                    string equation = equationManager.LastEquation;
                    string answer = equationManager.LastAnswer;

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

                    GameObject incorrectGameObject = equationManager.GetRandomDigitNumber(answerDigits, TagManager.InCorrectAnswer);
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

            IncrementEquationsAndSetState();
        }

        private void IncrementEquationsAndSetState()
        {
            _currentEquationsCount += 1;

            if (_currentEquationsCount >= totalEquationsToSpawn)
            {
                Debug.Log("All Equations Spawned");
                SetSpawnerState(SpawnerState.BonusMode);
            }
        }

        private void SetSpawnerState(SpawnerState spawnerState) => _spawnerState = spawnerState;

        #endregion
    }
}