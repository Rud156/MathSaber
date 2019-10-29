using System.Collections.Generic;
using System.Linq;
using Extensions;
using General;
using UnityEngine;
using Utils;

namespace Equations
{
    public class EquationSpawner : MonoBehaviour
    {
        [Header("Spawn Data")] public List<Transform> spawnPoints;
        public int totalSpawnPointsToSelect = 2;
        public float secondsBetweenEachObject;

        [Header("Managers")] public EquationManager equationManager;

        [Header("Holders")] public Transform blockHolder;

        private float _currentTime;
        private List<EquationBlockController> _cubes;

        #region Unity Functions

        private void Start() => _cubes = new List<EquationBlockController>();

        private void Update()
        {
            _currentTime -= Time.deltaTime;
            if (_currentTime <= 0)
            {
                SpawnEquation();
                _currentTime = secondsBetweenEachObject;
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

        #region Utility Functions

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

                    EquationBlockController cubeMovement = correctGameObject.GetComponent<EquationBlockController>();
                    cubeMovement.SetParent(this);
                    _cubes.Add(cubeMovement);

                    correctShown = true;
                }
                else
                {
                    string answerString = equationManager.LastAnswer;
                    int answerDigits = $"{Mathf.Abs(int.Parse(answerString))}".Length; // Very Bad. But OK for Prototype

                    GameObject incorrectGameObject = equationManager.GetRandomDigitNumber(answerDigits, TagManager.InCorrectAnswer);
                    incorrectGameObject.transform.position = spawnTransform.position;
                    incorrectGameObject.transform.SetParent(blockHolder);

                    EquationBlockController cubeMovement = incorrectGameObject.GetComponent<EquationBlockController>();
                    cubeMovement.SetParent(this);
                    _cubes.Add(cubeMovement);
                }

                selectedSpawnPoints.RemoveAt(randomIndex);
            }
        }

        #endregion
    }
}