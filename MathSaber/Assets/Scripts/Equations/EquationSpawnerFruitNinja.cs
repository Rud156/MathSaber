using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blocks;
using Extensions;
using UnityEngine;
using Utils;

namespace Equations
{
    public class EquationSpawnerFruitNinja : EquationSpawnerBase
    {
        [Header("Fruit Ninja Data")] public float delayBetweenBlockSpawn;

        private List<FruitNinjaEquationAndBlockController> _fruitNinjaBlockControllers;

        #region Overridden Parent

        protected override void Start()
        {
            _fruitNinjaBlockControllers = new List<FruitNinjaEquationAndBlockController>();

            base.Start();
        }

        public override void SpawnNextEquation()
        {
            // Don't do anything as spawning on hit feels bad
        }

        protected override void SpawnBonusBlocks()
        {
            int randomNumber = Random.Range(1, 10);
            string answer = randomNumber.ToString();

            Transform spawnPoint = bonusSpawnPoints[Mathf.FloorToInt(Random.value * bonusSpawnPoints.Count)];
            GameObject numberObject = equationAndBlockGenerator.GetRandomNumberGameObject(answer, TagManager.BonusAnswer, BlockType.FruitNinjaBlock);

            FruitNinjaEquationAndBlockController cubeController = numberObject.GetComponent<FruitNinjaEquationAndBlockController>();
            cubeController.SetEquationStatus(-1, null, answer, answer, true);
            cubeController.LaunchBlock();

            numberObject.transform.position = spawnPoint.position;
            numberObject.transform.SetParent(blockHolder);
        }

        protected override void SpawnEquation()
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
            _fruitNinjaBlockControllers.Clear();

            _selectedSpawnPoints = spawnPoints.ToList();
            _selectedSpawnPoints = _selectedSpawnPoints.Shuffle().Take(totalSpawnPointsToSelect).ToList();

            GameObject parentGameObject = new GameObject();
            parentGameObject.transform.SetParent(blockHolder);
            ParentBlockController parentBlockController = parentGameObject.AddComponent<ParentBlockController>();

            while (_selectedSpawnPoints.Count > 0)
            {
                int randomIndex = Mathf.FloorToInt(Random.value * _selectedSpawnPoints.Count);
                Transform spawnTransform = _selectedSpawnPoints[randomIndex];

                if (!correctShown)
                {
                    GameObject correctGameObject = equationAndBlockGenerator.CreateBasicEquation(BlockType.FruitNinjaBlock);
                    correctGameObject.transform.position = spawnTransform.position;
                    correctGameObject.transform.SetParent(parentGameObject.transform);

                    string equation = equationAndBlockGenerator.LastEquation;
                    string answer = equationAndBlockGenerator.LastAnswer;

                    // Display equation the text to the UI
                    textDisplay.text = equation;
                    _usedNumbers.Add(answer);

                    FruitNinjaEquationAndBlockController cubeController = correctGameObject.GetComponent<FruitNinjaEquationAndBlockController>();
                    cubeController.SetEquationStatus(_currentEquationsSpawnedCount, equation, answer, answer, true);

                    cubeController.SetParent(parentBlockController);
                    parentBlockController.AddEquationBlock(cubeController);

                    _fruitNinjaBlockControllers.Add(cubeController);
                    correctShown = true;
                }
                else
                {
                    string answerString = equationAndBlockGenerator.LastAnswer;
                    int answerDigits = $"{Mathf.Abs(int.Parse(answerString))}".Length; // Very Bad. But OK for Prototype. Well now it's in the game

                    var incorrectObject = equationAndBlockGenerator
                        .GetRandomDigitCountNumber(answerDigits, TagManager.InCorrectAnswer, _usedNumbers, BlockType.FruitNinjaBlock);
                    _usedNumbers.Add(incorrectObject.Item2);

                    GameObject incorrectGameObject = incorrectObject.Item1;
                    incorrectGameObject.transform.position = spawnTransform.position;
                    incorrectGameObject.transform.SetParent(parentGameObject.transform);

                    string equation = equationAndBlockGenerator.LastEquation;
                    string answer = equationAndBlockGenerator.LastAnswer;

                    FruitNinjaEquationAndBlockController cubeController = incorrectGameObject.GetComponent<FruitNinjaEquationAndBlockController>();
                    cubeController.SetEquationStatus(_currentEquationsSpawnedCount, equation, answer, incorrectObject.Item2, false);

                    cubeController.SetParent(parentBlockController);
                    parentBlockController.AddEquationBlock(cubeController);

                    _fruitNinjaBlockControllers.Add(cubeController);
                }

                _selectedSpawnPoints.RemoveAt(randomIndex);
            }

            _currentEquationsSpawnedCount += 1;

            _fruitNinjaBlockControllers = _fruitNinjaBlockControllers.Shuffle().ToList();
            for (int i = 0; i < _fruitNinjaBlockControllers.Count; i++)
            {
                StartCoroutine(SpawnEquationBlock((i + 1) * delayBetweenBlockSpawn, _fruitNinjaBlockControllers[i]));
            }
        }

        #endregion

        #region Utility Functions

        private IEnumerator SpawnEquationBlock(float waitDelay, FruitNinjaEquationAndBlockController fruitNinjaEquationAndBlockController)
        {
            fruitNinjaEquationAndBlockController.gameObject.SetActive(false);

            yield return new WaitForSeconds(waitDelay);

            fruitNinjaEquationAndBlockController.gameObject.SetActive(true);
            fruitNinjaEquationAndBlockController.LaunchBlock();
        }

        #endregion
    }
}