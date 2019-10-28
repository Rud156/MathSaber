using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Equations
{
    public class EquationManager : MonoBehaviour
    {
        [Header("Equations Controller")] public GradeEnum gradeEnum;

        [Header("Prefabs")] public GameObject[] numbers;
        public GameObject minusOperatorPrefab;
        public GameObject blockPrefab;

        [Header("Offsets")] public float singleCharOffset;
        public float defaultScaleValue;
        public float scaleValDouble;

        [Header("Holders")] public Transform blockHolder;
        public Transform spawnTransform;

        [Header("UI")] public TextMeshPro numberDisplay;

        // This is added as it is not possible to completely 
        // Send multiple values from functions like Python
        private string _lastEquation;
        private string _lastAnswer;

        private HashSet<string> _answersBeforeLastReset;

        #region Unity Functions

        private void Start() => _answersBeforeLastReset = new HashSet<string>();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                GetCombinedNumberGameObject("2", TagManager.CorrectAnswer);
            }
        }

        #endregion

        #region External Functions

        public GameObject CreateBasicEquationAndAddToUI()
        {
            _answersBeforeLastReset.Clear();

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

            numberDisplay.text = equation;

            _answersBeforeLastReset.Add(answer);
            return GetCombinedNumberGameObject(answer, TagManager.CorrectAnswer);
        }

        public GameObject ReCreatePreviousEquation()
        {
            numberDisplay.text = _lastEquation;

            return GetCombinedNumberGameObject(_lastAnswer, TagManager.CorrectAnswer);
        }

        public GameObject GetRandomDigitNumber(int digitCount, string tagName)
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

                if (!_answersBeforeLastReset.Contains(randomNumberString))
                {
                    break;
                }

                currentDigitCount = 0;
                randomNumberString = string.Empty;
            }

            return GetCombinedNumberGameObject(randomNumberString, tagName);
        }

        public string LastEquation => _lastEquation;

        public string LastAnswer => _lastAnswer;

        #endregion

        #region Utility Functions

        private GameObject GetCombinedNumberGameObject(string answer, string tagName)
        {
            float offsetLeft = answer.Length / 2;
            if (answer.Length % 2 == 0)
            {
                // This is used for the case when the scale of the object needs to be divide by 2
                offsetLeft -= scaleValDouble / 2;
            }

            Vector3 startPosition = offsetLeft * singleCharOffset * Vector3.right;

            GameObject blockObjectInstance = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity);
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
                // PLus Operator

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

                    totalValue -= randomNumber;
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