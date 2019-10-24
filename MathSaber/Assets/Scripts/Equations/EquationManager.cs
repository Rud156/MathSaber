using System;
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

        [Header("Offsets")] public float singleCharOffset;
        public int wordSpacingCount;
        public float objectScale = 1;

        [Header("Holders")] public Transform blockHolder;
        public Transform spawnTransform;

        [Header("UI")] public TextMeshPro numberDisplay;
        
        private string _lastEquation;
        private string _lastAnswer;

        #region External Functions

        public GameObject CreateBasicEquationAndAddToUI()
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

            numberDisplay.text = equation;

            // TODO: Remove this later on...
            return new GameObject();
        }

        public GameObject ReCreatePreviousEquation()
        {
            numberDisplay.text = _lastEquation;

            return new GameObject();
        }

        public GameObject GetRandomDigitNumber(int digitCount, string tagName)
        {
            int currentDigitCount = 0;
            string randomNumberString = string.Empty;

            while (currentDigitCount != digitCount)
            {
                var randomNumber = GetRandomNumber(currentDigitCount != 0 ? 0 : 1, 9);
                randomNumberString += randomNumber;

                currentDigitCount += 1;
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
                offsetLeft -= (objectScale / 2.0f);
            }

            Vector3 startPosition = -offsetLeft * singleCharOffset * Vector3.right;
            GameObject holderObject = new GameObject("Answer Object")
            {
                tag = tagName
            };
            Bounds holderObjectBounds = new Bounds();

            for (int i = 0; i < answer.Length; i++)
            {
                string value = answer[i].ToString();
                Vector3 position = startPosition + i * singleCharOffset * Vector3.right;

                if (value.Equals("-"))
                {
                    GameObject subtractInstance = Instantiate(minusOperatorPrefab, position, Quaternion.identity);
                    subtractInstance.transform.SetParent(holderObject.transform);

                    holderObjectBounds.Encapsulate(subtractInstance.GetComponent<MeshRenderer>().bounds);
                }
                else
                {
                    GameObject objectInstance = Instantiate(numbers[int.Parse(value)], position, Quaternion.identity);
                    objectInstance.transform.SetParent(holderObject.transform);

                    holderObjectBounds.Encapsulate(objectInstance.GetComponent<MeshRenderer>().bounds);
                }
            }

            BoxCollider holderCollider = holderObject.AddComponent<BoxCollider>();
            holderCollider.size = holderObjectBounds.size;
            holderCollider.center = Vector3.zero;
            holderCollider.isTrigger = true;

            holderObject.transform.position = spawnTransform.position;

            return holderObject;
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