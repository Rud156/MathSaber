using Structs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blocks;
using Extensions;
using TMPro;
using UnityEngine;
using Utils;

namespace Equations
{
    public class EquationSpawnerCustomInput : EquationSpawnerBase
    {
        [Header("File Data")] public string fileName = "Questions.txt";

        [Header("Text Display")] public TextMeshPro farTextDisplay;

        private const string NumberLessThan = "NLT";
        private const string NumberGreaterThan = "NGT";
        private const string Algebra = "ALG";
        private const string GreatestNumber = "GSTN";
        private const string LeastNumber = "LSTN";

        private string _filePath;
        private List<CustomEquationData> _customEquationsData;

        #region Overridden Parent

        protected override void Start()
        {
            _filePath = $"./EquationsQuestions/{fileName}";
            _customEquationsData = new List<CustomEquationData>();

            ReadQuestionsFromFile();
            base.Start();
        }

        protected override void SpawnEquation()
        {
            if (_currentEquationsSpawnedCount >= _customEquationsData.Count)
            {
                SetSpawnerState(SpawnerState.BonusModeCountDown);
                _currentTime = timeDelayBeforeBonusMode;
                farTextDisplay.text = "";
                return;
            }

            bool correctShown = false;

            _selectedSpawnPoints.Clear();

            _selectedSpawnPoints = spawnPoints.ToList();
            _selectedSpawnPoints = _selectedSpawnPoints.Shuffle().Take(totalEquationsToSpawn).ToList();

            GameObject parentGameObject = new GameObject();
            parentGameObject.transform.SetParent(blockHolder);
            ParentBlockController parentBlockController = parentGameObject.AddComponent<ParentBlockController>();

            CustomEquationData customEquationData = _customEquationsData[_currentEquationsSpawnedCount];
            int currentAnswerIndex = 0;

            while (_selectedSpawnPoints.Count > 0)
            {
                int randomIndex = Mathf.FloorToInt(Random.value * _selectedSpawnPoints.Count);
                Transform spawnTransform = _selectedSpawnPoints[randomIndex];

                if (!correctShown)
                {
                    GameObject correctGameObject = equationAndBlockGenerator.GetCustomEquationNumberGameObject(
                        customEquationData.answers[currentAnswerIndex], TagManager.CorrectAnswer, BlockType.JumpingBlock
                    );
                    correctGameObject.transform.position = spawnTransform.position;
                    correctGameObject.transform.SetParent(parentGameObject.transform);

                    string equation = customEquationData.question;
                    string answer = customEquationData.answers[currentAnswerIndex].baseAnswer;

                    // Display equation the text to the UI
                    farTextDisplay.text = customEquationData.question;

                    EquationBlockController cubeController = correctGameObject.GetComponent<EquationBlockController>();
                    cubeController.SetEquationStatus(_currentEquationsSpawnedCount, equation, answer, answer, true);

                    cubeController.SetMovementSpeed(_currentObjectMovementSpeed);
                    cubeController.SetParent(parentBlockController);
                    parentBlockController.AddEquationBlock(cubeController);

                    correctShown = true;
                }
                else
                {
                    GameObject incorrectGameObject = equationAndBlockGenerator.GetCustomEquationNumberGameObject(
                        customEquationData.answers[currentAnswerIndex], TagManager.InCorrectAnswer, BlockType.JumpingBlock
                    );
                    incorrectGameObject.transform.position = spawnTransform.position;
                    incorrectGameObject.transform.SetParent(parentGameObject.transform);

                    string equation = equationAndBlockGenerator.LastEquation;
                    string answer = equationAndBlockGenerator.LastAnswer;

                    EquationBlockController cubeController = incorrectGameObject.GetComponent<EquationBlockController>();
                    cubeController.SetEquationStatus(_currentEquationsSpawnedCount, equation, answer,
                        customEquationData.answers[currentAnswerIndex].baseAnswer, false);

                    cubeController.SetMovementSpeed(_currentObjectMovementSpeed);
                    cubeController.SetParent(parentBlockController);
                    parentBlockController.AddEquationBlock(cubeController);
                }

                currentAnswerIndex += 1;
                _selectedSpawnPoints.RemoveAt(randomIndex);
            }

            _currentEquationsSpawnedCount += 1;
        }

        #endregion Overridden Parent

        #region Utility Functions

        private void ReadQuestionsFromFile()
        {
            string[] lines = File.ReadAllLines(_filePath);
            string lastQuestionType = string.Empty;

            foreach (string line in lines)
            {
                string cleanedLine = line.Trim();
                if (cleanedLine == NumberLessThan || cleanedLine == NumberGreaterThan || cleanedLine == Algebra || cleanedLine == GreatestNumber ||
                    cleanedLine == LeastNumber)
                {
                    lastQuestionType = cleanedLine;

                    Debug.Log(lastQuestionType);

                    continue;
                }

                string[] dataStrings = line.Split(',');

                switch (lastQuestionType)
                {
                    case NumberLessThan:
                        CreateLTNQA(dataStrings);
                        break;

                    case NumberGreaterThan:
                        CreateGTNQA(dataStrings);
                        break;

                    case Algebra:
                        CreateALGQA(dataStrings);
                        break;

                    case GreatestNumber:
                        CreateGSTNQA(dataStrings);
                        break;

                    case LeastNumber:
                        CreateLSTNQA(dataStrings);
                        break;

                    default:
                        // Do Nothing In This Case
                        break;
                }
            }
        }

        #region Questions Data Creator

        private void CreateLTNQA(string[] data)
        {
            CustomEquationData customEquationData = new CustomEquationData
            {
                question = $"Which of the numbers is less than {data[0].Trim()}?",
                answers = new List<CustomEquationNumber>()
            };

            for (int i = 1; i < data.Length; i++)
            {
                string cleanedData = data[i].Trim();
                int fractionResult = IsFraction(cleanedData);
                if (fractionResult == 0)
                {
                    CustomEquationNumber customEquationNumber = new CustomEquationNumber()
                    {
                        baseAnswer = cleanedData,
                        numberType = 0
                    };
                    customEquationData.answers.Add(customEquationNumber);
                }
                else
                {
                    var fraction = SplitFraction(cleanedData);

                    int.TryParse(fraction.Item2, out var mixedPart);
                    int.TryParse(fraction.Item3, out var numerator);
                    int.TryParse(fraction.Item4, out var denominator);

                    CustomEquationNumber customEquationNumber = new CustomEquationNumber()
                    {
                        baseAnswer = cleanedData,
                        numberType = fraction.Item1,
                        mixedPart = mixedPart,
                        numerator = numerator,
                        denominator = denominator
                    };
                    customEquationData.answers.Add(customEquationNumber);
                }
            }

            _customEquationsData.Add(customEquationData);
        }

        private void CreateGTNQA(string[] data)
        {
            CustomEquationData customEquationData = new CustomEquationData
            {
                question = $"Which of the numbers is greater than {data[0].Trim()}?",
                answers = new List<CustomEquationNumber>()
            };

            for (int i = 1; i < data.Length; i++)
            {
                string cleanedData = data[i].Trim();
                int fractionResult = IsFraction(cleanedData);
                if (fractionResult == 0)
                {
                    CustomEquationNumber customEquationNumber = new CustomEquationNumber()
                    {
                        baseAnswer = cleanedData,
                        numberType = 0
                    };
                    customEquationData.answers.Add(customEquationNumber);
                }
                else
                {
                    var fraction = SplitFraction(cleanedData);

                    int.TryParse(fraction.Item2, out var mixedPart);
                    int.TryParse(fraction.Item3, out var numerator);
                    int.TryParse(fraction.Item4, out var denominator);

                    CustomEquationNumber customEquationNumber = new CustomEquationNumber()
                    {
                        baseAnswer = cleanedData,
                        numberType = fraction.Item1,
                        mixedPart = mixedPart,
                        numerator = numerator,
                        denominator = denominator
                    };
                    customEquationData.answers.Add(customEquationNumber);
                }
            }

            _customEquationsData.Add(customEquationData);
        }

        private void CreateALGQA(string[] data)
        {
            CustomEquationData customEquationData = new CustomEquationData
            {
                question = $"{data[0].Trim()}",
                answers = new List<CustomEquationNumber>()
            };

            for (int i = 1; i < data.Length; i++)
            {
                string cleanedData = data[i].Trim();
                int fractionResult = IsFraction(cleanedData);
                if (fractionResult == 0)
                {
                    CustomEquationNumber customEquationNumber = new CustomEquationNumber()
                    {
                        baseAnswer = cleanedData,
                        numberType = 0
                    };
                    customEquationData.answers.Add(customEquationNumber);
                }
                else
                {
                    var fraction = SplitFraction(cleanedData);

                    int.TryParse(fraction.Item2, out var mixedPart);
                    int.TryParse(fraction.Item3, out var numerator);
                    int.TryParse(fraction.Item4, out var denominator);

                    CustomEquationNumber customEquationNumber = new CustomEquationNumber()
                    {
                        baseAnswer = cleanedData,
                        numberType = fraction.Item1,
                        mixedPart = mixedPart,
                        numerator = numerator,
                        denominator = denominator
                    };
                    customEquationData.answers.Add(customEquationNumber);
                }
            }

            _customEquationsData.Add(customEquationData);
        }

        private void CreateGSTNQA(string[] data)
        {
            CustomEquationData customEquationData = new CustomEquationData
            {
                question = "Which is the greatest number?",
                answers = new List<CustomEquationNumber>()
            };

            for (int i = 0; i < data.Length; i++)
            {
                string cleanedData = data[i].Trim();
                int fractionResult = IsFraction(cleanedData);
                if (fractionResult == 0)
                {
                    CustomEquationNumber customEquationNumber = new CustomEquationNumber()
                    {
                        baseAnswer = cleanedData,
                        numberType = 0
                    };
                    customEquationData.answers.Add(customEquationNumber);
                }
                else
                {
                    var fraction = SplitFraction(cleanedData);

                    int.TryParse(fraction.Item2, out var mixedPart);
                    int.TryParse(fraction.Item3, out var numerator);
                    int.TryParse(fraction.Item4, out var denominator);

                    CustomEquationNumber customEquationNumber = new CustomEquationNumber()
                    {
                        baseAnswer = cleanedData,
                        numberType = fraction.Item1,
                        mixedPart = mixedPart,
                        numerator = numerator,
                        denominator = denominator
                    };
                    customEquationData.answers.Add(customEquationNumber);
                }
            }

            _customEquationsData.Add(customEquationData);
        }

        private void CreateLSTNQA(string[] data)
        {
            CustomEquationData customEquationData = new CustomEquationData
            {
                question = "Which is the least number?",
                answers = new List<CustomEquationNumber>()
            };

            for (int i = 0; i < data.Length; i++)
            {
                string cleanedData = data[i].Trim();
                int fractionResult = IsFraction(cleanedData);
                if (fractionResult == 0)
                {
                    CustomEquationNumber customEquationNumber = new CustomEquationNumber()
                    {
                        baseAnswer = cleanedData,
                        numberType = 0
                    };
                    customEquationData.answers.Add(customEquationNumber);
                }
                else
                {
                    var fraction = SplitFraction(cleanedData);

                    int.TryParse(fraction.Item2, out var mixedPart);
                    int.TryParse(fraction.Item3, out var numerator);
                    int.TryParse(fraction.Item4, out var denominator);

                    CustomEquationNumber customEquationNumber = new CustomEquationNumber()
                    {
                        baseAnswer = cleanedData,
                        numberType = fraction.Item1,
                        mixedPart = mixedPart,
                        numerator = numerator,
                        denominator = denominator
                    };
                    customEquationData.answers.Add(customEquationNumber);
                }
            }

            _customEquationsData.Add(customEquationData);
        }

        #endregion Questions Data Creator

        #region Fractions Checker

        private int IsFraction(string number)
        {
            // This is a very simple check for fractions based on the division symbol
            // If questions also need to be parsed, then this cannot be used.
            // Only works for single input number strings

            // TODO: Handle Negative Fractions (Very Important) (Test This)
            if (number.Contains(" "))
            {
                return 2;
            }
            else if (number.Contains("/"))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private (int, string, string, string) SplitFraction(string number)
        {
            int fractionResult = IsFraction(number);
            if (fractionResult == 0)
            {
                return (fractionResult, null, null, null);
            }

            if (fractionResult == 1) // This is a simple fraction
            {
                string[] data = number.Split('/');
                return (fractionResult, null, data[0].Trim(), data[1].Trim());
            }
            else // This is a mixed fraction
            {
                string[] mixedData = number.Split(' ');
                string[] data = mixedData[1].Split('/');
                return (fractionResult, mixedData[0].Trim(), data[0].Trim(), data[1].Trim());
            }
        }

        #endregion Fractions Checker

        #endregion Utility Functions
    }
}