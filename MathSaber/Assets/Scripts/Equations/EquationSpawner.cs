using System.Collections.Generic;
using System.Linq;
using General;
using UnityEngine;
using Utils;

namespace Equations
{
    public class EquationSpawner : MonoBehaviour
    {
        public List<Transform> spawnPoints;
        public float secondsBetweenEachObject;
        public EquationManager equationManager;
        public Transform blockHolder;

        private float _currentTime;
        private List<CubeMovementAndDestruction> _cubes;

        #region Unity Functions

        private void Start()
        {
            _currentTime = secondsBetweenEachObject;
            _cubes = new List<CubeMovementAndDestruction>();
        }

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
            foreach (CubeMovementAndDestruction cubeMovementAndDestruction in _cubes)
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
            List<Transform> copiedList = spawnPoints.ToList();

            while (copiedList.Count > 0)
            {
                int randomIndex = Mathf.FloorToInt(Random.value * copiedList.Count);
                Transform spawnTransform = copiedList[randomIndex];

                if (!correctShown)
                {
                    GameObject correctGameObject = equationManager.CreateBasicEquationAndAddToUI();
                    correctGameObject.transform.position = spawnTransform.position;
                    correctGameObject.transform.SetParent(blockHolder);

                    CubeMovementAndDestruction cubeMovement = correctGameObject.AddComponent<CubeMovementAndDestruction>();
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

                    CubeMovementAndDestruction cubeMovement = incorrectGameObject.AddComponent<CubeMovementAndDestruction>();
                    cubeMovement.SetParent(this);
                    _cubes.Add(cubeMovement);
                }

                copiedList.RemoveAt(randomIndex);
            }
        }

        #endregion
    }
}