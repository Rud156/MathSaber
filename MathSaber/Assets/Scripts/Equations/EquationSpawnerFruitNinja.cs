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

        #region Overridden Parent

        protected override void SpawnBonusBlocks()
        {
            int randomNumber = Random.Range(1, 10);
            string answer = randomNumber.ToString();

            Transform spawnPoint = bonusSpawnPoints[Mathf.FloorToInt(Random.value * bonusSpawnPoints.Count)];
            GameObject numberObject = equationAndBlockGenerator.GetRandomNumberGameObject(answer, TagManager.BonusAnswer, BlockType.FruitNinjaBlock);

            EquationBlockController cubeController = numberObject.GetComponent<EquationBlockController>();
            cubeController.SetEquationStatus(null, answer, false);

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

            // Allocations in Update is very bad...
            List<Transform> spawnPointsCopy = spawnPoints.ToList();
            List<Transform> selectedSpawnPoints = spawnPointsCopy.Shuffle().Take(totalSpawnPointsToSelect).ToList();
            HashSet<string> usedNumbers = new HashSet<string>();
            List<FruitNinjaEquationAndBlockController> fruitNinjaBlockControllers = new List<FruitNinjaEquationAndBlockController>();

            GameObject parentGameObject = new GameObject();
            parentGameObject.transform.SetParent(blockHolder);
            ParentBlockController parentBlockController = parentGameObject.AddComponent<ParentBlockController>();

            while (selectedSpawnPoints.Count > 0)
            {
                int randomIndex = Mathf.FloorToInt(Random.value * selectedSpawnPoints.Count);
                Transform spawnTransform = selectedSpawnPoints[randomIndex];

                if (!correctShown)
                {
                    GameObject correctGameObject = equationAndBlockGenerator.CreateBasicEquation(BlockType.FruitNinjaBlock);
                    correctGameObject.transform.position = spawnTransform.position;
                    correctGameObject.transform.SetParent(parentGameObject.transform);

                    string equation = equationAndBlockGenerator.LastEquation;
                    string answer = equationAndBlockGenerator.LastAnswer;

                    // Display equation the text to the UI
                    textDisplay.text = equation;
                    usedNumbers.Add(answer);

                    FruitNinjaEquationAndBlockController cubeController = correctGameObject.GetComponent<FruitNinjaEquationAndBlockController>();
                    cubeController.SetEquationStatus(equation, answer, true);

                    cubeController.SetParent(parentBlockController);
                    parentBlockController.AddEquationBlock(cubeController);

                    fruitNinjaBlockControllers.Add(cubeController);
                    correctShown = true;
                }
                else
                {
                    string answerString = equationAndBlockGenerator.LastAnswer;
                    int answerDigits = $"{Mathf.Abs(int.Parse(answerString))}".Length; // Very Bad. But OK for Prototype

                    var incorrectObject = equationAndBlockGenerator
                        .GetRandomDigitCountNumber(answerDigits, TagManager.InCorrectAnswer, usedNumbers, BlockType.FruitNinjaBlock);
                    usedNumbers.Add(incorrectObject.Item2);

                    GameObject incorrectGameObject = incorrectObject.Item1;
                    incorrectGameObject.transform.position = spawnTransform.position;
                    incorrectGameObject.transform.SetParent(parentGameObject.transform);

                    string equation = equationAndBlockGenerator.LastEquation;
                    string answer = equationAndBlockGenerator.LastAnswer;

                    FruitNinjaEquationAndBlockController cubeController = incorrectGameObject.GetComponent<FruitNinjaEquationAndBlockController>();
                    cubeController.SetEquationStatus(equation, answer, false);

                    cubeController.SetParent(parentBlockController);
                    parentBlockController.AddEquationBlock(cubeController);

                    fruitNinjaBlockControllers.Add(cubeController);
                }

                selectedSpawnPoints.RemoveAt(randomIndex);
            }

            _currentEquationsSpawnedCount += 1;

            fruitNinjaBlockControllers = fruitNinjaBlockControllers.Shuffle().ToList();
            for (int i = 0; i < fruitNinjaBlockControllers.Count; i++)
            {
                StartCoroutine(SpawnEquationBlock((i + 1) * delayBetweenBlockSpawn, fruitNinjaBlockControllers[i]));
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