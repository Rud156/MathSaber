using System;
using System.Collections.Generic;
using Structs;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Equations
{
    public class EquationAndBlockGenerator : MonoBehaviour
    {
        [Header("Equations Controller")] public GradeEnum gradeEnum;

        [Header("Prefabs")] public GameObject[] numbers;
        public GameObject minusOperatorPrefab;
        public GameObject blockPrefab;
        public GameObject jumpingBlockPrefab;
        public GameObject fruitNinjaBlockPrefab;

        [Header("Offsets")] public float singleCharOffset;
        public float defaultScaleValue;
        public float scaleValDouble;

        [Header("Fractions")] public float fractionNumeratorScale;
        public float fractionDenominatorScale;
        public float fractionMixedPartScale;
        public float fractionPartYOffset;
        public float fractionCharOffset;
        public Vector3 fractionLineDivScale;

        [Header("Holders")] public Transform blockHolder;
        public Transform spawnTransform;

        // This is added as it is not possible to completely 
        // Send multiple values from functions like Python
        // Well it is possible
        private string _lastEquation;
        private string _lastAnswer;

        #region External Functions

        public GameObject CreateBasicEquation(BlockType blockType)
        {
            (string, string) questionData;

            switch (gradeEnum)
            {
                case GradeEnum.Grade1:
                    questionData = GetGrade1Equation();
                    break;

                case GradeEnum.Grade2:
                    questionData = GetGrade2Equation();
                    break;

                case GradeEnum.Grade3:
                    questionData = GetGrade3Equation();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            string equation = questionData.Item1;
            string answer = questionData.Item2;

            _lastEquation = equation;
            _lastAnswer = answer;

            return GetCombinedNumberGameObject(answer, TagManager.CorrectAnswer, blockType);
        }

        public GameObject ReCreatePreviousEquation(BlockType blockType)
        {
            return GetCombinedNumberGameObject(_lastAnswer, TagManager.CorrectAnswer, blockType);
        }

        public (GameObject, string) GetRandomDigitCountNumber(int digitCount, string tagName, HashSet<string> answersAlreadyUsed, BlockType blockType)
        {
            int currentDigitCount = 0;
            string randomNumberString = string.Empty;

            while (true)
            {
                while (currentDigitCount != digitCount)
                {
                    var randomNumber = GetRandomNumber(currentDigitCount != 0 ? 0 : 1, 9);
                    randomNumberString += randomNumber;

                    currentDigitCount += 1;
                }

                if (!answersAlreadyUsed.Contains(randomNumberString))
                {
                    break;
                }

                currentDigitCount = 0;
                randomNumberString = string.Empty;
            }

            return (GetCombinedNumberGameObject(randomNumberString, tagName, blockType), randomNumberString);
        }

        public GameObject GetRandomNumberGameObject(string randomNumber, string tagName, BlockType blockType) =>
            GetCombinedNumberGameObject(randomNumber, tagName, blockType);

        public GameObject GetCustomEquationNumberGameObject(CustomEquationNumber equationNumber, string tagName, BlockType blockType) =>
            GetCombinedFractionNumberGameObject(equationNumber, tagName, blockType);

        public string LastEquation => _lastEquation;

        public string LastAnswer => _lastAnswer;

        #endregion

        #region Utility Functions

        private GameObject GetCombinedFractionNumberGameObject(CustomEquationNumber customEquationNumber, string tagName, BlockType blockType)
        {
            GameObject blockFinalPrefab;
            switch (blockType)
            {
                case BlockType.ForwardMovementBlock:
                    blockFinalPrefab = blockPrefab;
                    break;

                case BlockType.JumpingBlock:
                    blockFinalPrefab = jumpingBlockPrefab;
                    break;

                case BlockType.FruitNinjaBlock:
                    blockFinalPrefab = fruitNinjaBlockPrefab;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(blockType), blockType, null);
            }

            // TODO: All of this is probably highly hacky. Go through it later on...
            if (customEquationNumber.numberType == 1 || customEquationNumber.numberType == 2)
            {
                GameObject blockObjectInstance = Instantiate(blockFinalPrefab, Vector3.zero, Quaternion.identity);
                blockObjectInstance.tag = tagName;
                Vector3 spawnPositionCenter = blockObjectInstance.transform.GetChild(0).position;
                Vector3 spawnPositionLeft = blockObjectInstance.transform.GetChild(1).position;
                Vector3 spawnPositionRight = blockObjectInstance.transform.GetChild(2).position;

                if (customEquationNumber.numberType == 1) // This means it is a simple fraction and the center can be directly used
                {
                    GameObject subtractInstance = Instantiate(minusOperatorPrefab, spawnPositionCenter, Quaternion.identity);
                    subtractInstance.transform.localScale = fractionLineDivScale;
                    subtractInstance.transform.SetParent(blockObjectInstance.transform);

                    Vector3 numeratorPosition = spawnPositionCenter + Vector3.up * fractionPartYOffset;
                    Vector3 denominatorPosition = spawnPositionCenter + Vector3.down * fractionPartYOffset;

                    GameObject numeratorGameObject = GetNumberInEmptyGameObject(numeratorPosition, fractionNumeratorScale, fractionCharOffset,
                        customEquationNumber.numerator.ToString());
                    GameObject denominatorGameObject = GetNumberInEmptyGameObject(denominatorPosition, fractionDenominatorScale, fractionCharOffset,
                        customEquationNumber.denominator.ToString());

                    numeratorGameObject.transform.SetParent(blockObjectInstance.transform);
                    denominatorGameObject.transform.SetParent(blockObjectInstance.transform);
                }
                else if (customEquationNumber.numberType == 2) // This means it is a mixed fraction and the left and right parts must be used 
                {
                    GameObject subtractInstance = Instantiate(minusOperatorPrefab, spawnPositionRight, Quaternion.identity);
                    subtractInstance.transform.localScale = fractionLineDivScale;
                    subtractInstance.transform.SetParent(blockObjectInstance.transform);

                    Vector3 numeratorPosition = spawnPositionRight + Vector3.up * fractionPartYOffset;
                    Vector3 denominatorPosition = spawnPositionRight + Vector3.down * fractionPartYOffset;

                    GameObject numeratorGameObject = GetNumberInEmptyGameObject(numeratorPosition, fractionNumeratorScale, fractionCharOffset,
                        customEquationNumber.numerator.ToString());
                    GameObject denominatorGameObject = GetNumberInEmptyGameObject(denominatorPosition, fractionDenominatorScale, fractionCharOffset,
                        customEquationNumber.denominator.ToString());

                    numeratorGameObject.transform.SetParent(blockObjectInstance.transform);
                    denominatorGameObject.transform.SetParent(blockObjectInstance.transform);

                    GameObject mixedGameObject = GetNumberInEmptyGameObject(spawnPositionLeft, fractionMixedPartScale, fractionCharOffset,
                        customEquationNumber.mixedPart.ToString());
                    mixedGameObject.transform.SetParent(blockObjectInstance.transform);
                }

                return blockObjectInstance;
            }
            else
            {
                return GetCombinedNumberGameObject(customEquationNumber.baseAnswer, tagName, blockType);
            }
        }

        private GameObject GetNumberInEmptyGameObject(Vector3 initialPosition, float objectScale, float charOffset, string number)
        {
            GameObject holderGameObject = new GameObject();
            holderGameObject.transform.position = initialPosition;

            float offsetLeft = number.Length / 2;
            if (number.Length % 2 == 0)
            {
                // This is used for the case when the scale of the object needs to be divide by 2
                offsetLeft -= objectScale / 2;
            }

            Vector3 startPosition = offsetLeft * charOffset * Vector3.right;

            for (int i = 0; i < number.Length; i++)
            {
                string value = number[i].ToString();
                Vector3 position = startPosition + (i) * charOffset * Vector3.left;
                position += initialPosition;

                if (value.Equals("-"))
                {
                    GameObject subtractInstance = Instantiate(minusOperatorPrefab, position, Quaternion.identity);
                    subtractInstance.transform.localScale = Vector3.one * objectScale;
                    subtractInstance.transform.SetParent(holderGameObject.transform);
                }
                else
                {
                    GameObject objectInstance = Instantiate(numbers[int.Parse(value)], position, Quaternion.identity);
                    objectInstance.transform.localScale = Vector3.one * objectScale;
                    objectInstance.transform.SetParent(holderGameObject.transform);
                }
            }

            return holderGameObject;
        }

        private GameObject GetCombinedNumberGameObject(string answer, string tagName, BlockType blockType)
        {
            float offsetLeft = answer.Length / 2;
            if (answer.Length % 2 == 0)
            {
                // This is used for the case when the scale of the object needs to be divide by 2
                offsetLeft -= scaleValDouble / 2;
            }

            Vector3 startPosition = offsetLeft * singleCharOffset * Vector3.right;

            GameObject blockFinalPrefab;
            switch (blockType)
            {
                case BlockType.ForwardMovementBlock:
                    blockFinalPrefab = blockPrefab;
                    break;

                case BlockType.JumpingBlock:
                    blockFinalPrefab = jumpingBlockPrefab;
                    break;

                case BlockType.FruitNinjaBlock:
                    blockFinalPrefab = fruitNinjaBlockPrefab;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(blockType), blockType, null);
            }

            GameObject blockObjectInstance = Instantiate(blockFinalPrefab, Vector3.zero, Quaternion.identity);
            blockObjectInstance.tag = tagName;
            Vector3 spawnPosition = blockObjectInstance.transform.GetChild(0).position;

            for (int i = 0; i < answer.Length; i++)
            {
                string value = answer[i].ToString();
                Vector3 position = startPosition + (i) * singleCharOffset * Vector3.left;
                position += spawnPosition;

                if (value.Equals("-"))
                {
                    GameObject subtractInstance = Instantiate(minusOperatorPrefab, position, Quaternion.identity);
                    if (answer.Length % 2 == 0)
                    {
                        subtractInstance.transform.localScale = Vector3.one * scaleValDouble;
                    }

                    subtractInstance.transform.SetParent(blockObjectInstance.transform);
                }
                else
                {
                    GameObject objectInstance = Instantiate(numbers[int.Parse(value)], position, Quaternion.identity);
                    if (answer.Length % 2 == 0)
                    {
                        objectInstance.transform.localScale = Vector3.one * scaleValDouble;
                    }

                    objectInstance.transform.SetParent(blockObjectInstance.transform);
                }
            }

            return blockObjectInstance;
        }

        // Returns (Equation, Answer)
        private (string, string) GetGrade1Equation()
        {
            int leftNumber = GetRandomNumber(0, 9);
            int rightNumber = GetRandomNumber(0, 9);

            bool randomBoolean = Random.value > 0.5f;

            if (randomBoolean)
            {
                // Plus Operator

                int totalValue = leftNumber + rightNumber;
                string equation = $"{leftNumber} + {rightNumber} =";

                return (equation, $"{totalValue}");
            }
            else
            {
                // Minus Operator

                int totalValue;
                string equation;

                if (leftNumber < rightNumber)
                {
                    totalValue = rightNumber - leftNumber;
                    equation = $"{rightNumber} - {leftNumber} =";
                }
                else
                {
                    totalValue = leftNumber - rightNumber;
                    equation = $"{leftNumber} - {rightNumber} =";
                }

                return (equation, $"{totalValue}");
            }
        }

        // Returns (Equation, Answer)
        private (string, string) GetGrade2Equation()
        {
            int leftNumber = GetRandomNumber(0, 20);
            int rightNumber = GetRandomNumber(0, 20);

            bool randomBoolean = Random.value > 0.5f;

            if (randomBoolean)
            {
                // Plus Ultra Operator

                int totalValue = leftNumber + rightNumber;
                string equation = $"{leftNumber} + {rightNumber}";

                bool addExtraDigit = Random.value > 0.5f;

                if (addExtraDigit)
                {
                    int randomNumber = GetRandomNumber(0, 9);
                    totalValue += randomNumber;
                    equation += $" + {randomNumber}";
                }

                return (equation, $"{totalValue}");
            }
            else
            {
                // Minus Operator

                int totalValue = leftNumber - rightNumber;
                string equation = $"{leftNumber} - {rightNumber} =";

                return (equation, $"{totalValue}");
            }
        }

        private (string, string) GetGrade3Equation()
        {
            int maxDigits = 3;
            float randomValue = Random.value;

            if (randomValue > 0 && randomValue <= 0.34f)
            {
                // Plus Operator
                int totalDigitCount = GetRandomNumber(2, maxDigits);

                int totalValue = 0;
                string equation = string.Empty;

                for (int i = 0; i < totalDigitCount; i++)
                {
                    int randomNumber = GetRandomNumber(0, 20);
                    if (i != totalDigitCount - 1)
                    {
                        equation += $"{randomNumber} + ";
                    }
                    else
                    {
                        equation += $"{randomNumber} =";
                    }

                    totalValue += randomNumber;
                }

                return (equation, $"{totalValue}");
            }
            else if (randomValue > 0.34f && randomValue <= 0.67f)
            {
                // Minus Operator

                int totalDigitCount = GetRandomNumber(2, maxDigits);

                int totalValue = 0;
                string equation = string.Empty;

                for (int i = 0; i < totalDigitCount; i++)
                {
                    int randomNumber = GetRandomNumber(0, 20);
                    if (i != totalDigitCount - 1)
                    {
                        equation += $"{randomNumber} - ";
                    }
                    else
                    {
                        equation += $"{randomNumber} =";
                    }

                    if (i == 0)
                    {
                        totalValue += randomNumber;
                    }
                    else
                    {
                        totalValue -= randomNumber;
                    }
                }

                return (equation, $"{totalValue}");
            }
            else
            {
                // Multiply Operator

                int leftNumber = GetRandomNumber(0, 10);
                int rightNumber = GetRandomNumber(0, 10);

                int totalValue = leftNumber * rightNumber;
                string equation = $"{leftNumber} * {rightNumber} =";

                return (equation, $"{totalValue}");
            }
        }

        private int GetRandomNumber(int start, int end) => Random.Range(start, end);

        private OperatorEnum GetRandomOperator()
        {
            Array values = Enum.GetValues(typeof(OperatorEnum));
            return (OperatorEnum) values.GetValue(Random.Range(0, values.Length));
        }

        #endregion
    }
}