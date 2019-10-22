using System;
using TMPro;
using UnityEngine;
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

        [Header("UI")] public TextMeshProUGUI numberDisplay;

        #region Unityu Functions

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                SpawnBasicEquation();
            }
        }

        #endregion

        #region Utility Functions

        private void SpawnBasicEquation()
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

            Debug.Log($"Question: {questionEquation}, Answer: {answer}");
            
            float offsetLeft = answer.Length / 2.0f;
            Vector3 startPosition = offsetLeft * singleCharOffset * Vector3.right;

            for (int i = 0; i < answer.Length; i++)
            {
                string value = answer[0].ToString();
                Vector3 position = startPosition + i * singleCharOffset * Vector3.right;

                if (value.Equals("-"))
                {
                    GameObject subtractInstance = Instantiate(minusOperatorPrefab, position, Quaternion.identity);
                }
                else
                {
                    GameObject objectInstance = Instantiate(numbers[int.Parse(value)], position, Quaternion.identity);
                }
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