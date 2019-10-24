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
        [Header("Prefabs")] public GameObject[] numbers;
        public GameObject plusOperatorPrefab;
        public GameObject minusOperatorPrefab;
        public GameObject multiplyOperatorPrefab;
        public GameObject divideOperatorPrefab;

        [Header("Offsets")] public float singleCharOffset;
        public int wordSpacingCount;
        public float objectScale = 1;

        [Header("Holders")] public Transform blockHolder;
        public Transform spawnTransform;

        [Header("UI")] public TextMeshPro numberDisplay;

        // This is added as it is not possible to completely 
        // Send multiple values from functions like Python
        private string _lastEquation;
        private string _lastAnswer;

        #region External Functions

        public GameObject CreateBasicEquationAndAddToUI()
        {
            int leftValue = GetRandomNumber(0, 9);
            int rightValue = GetRandomNumber(0, 9);
            OperatorEnum randomOperator = GetRandomOperator();

            if (randomOperator == OperatorEnum.Divide && rightValue == 0)
            {
                rightValue = 1;
            }

            int totalValue;
            string operatorString;

            switch (randomOperator)
            {
                case OperatorEnum.Plus:
                    totalValue = leftValue + rightValue;
                    operatorString = "+";
                    break;

                case OperatorEnum.Minus:
                    totalValue = leftValue - rightValue;
                    operatorString = "-";
                    break;

                case OperatorEnum.Multiply:
                    totalValue = leftValue * rightValue;
                    operatorString = "*";
                    break;

                case OperatorEnum.Divide:
                    totalValue = leftValue / rightValue;
                    operatorString = "/";
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            string questionEquation = $"{leftValue} {operatorString} {rightValue} = ";
            string answer = $"{totalValue}";

            _lastEquation = questionEquation;
            _lastAnswer = answer;
            numberDisplay.text = questionEquation;

            Debug.Log($"Question: {questionEquation}, Answer: {answer}");

            return GetCombinedNumberGameObject(answer, TagManager.CorrectAnswer);
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

        private int GetRandomNumber(int start, int end) => Random.Range(start, end);

        private OperatorEnum GetRandomOperator()
        {
            Array values = Enum.GetValues(typeof(OperatorEnum));
            return (OperatorEnum) values.GetValue(Random.Range(0, values.Length));
        }

        #endregion
    }
}