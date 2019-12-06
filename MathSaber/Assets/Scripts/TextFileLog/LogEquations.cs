using System;
using System.Collections.Generic;
using UnityEngine;
using Equations;
using System.IO;

namespace TextFileLog
{
    public class LogEquations : MonoBehaviour
    {
        private string _directoryPath;
        private string _filePathBase;

        #region Unity Functions

        private void Start()
        {
            _directoryPath = "./EquationsSolvedLog";
            _filePathBase = $"{_directoryPath}/Equations_";

            WriteAnalyticsDataToFile();
        }

        #endregion

        #region Utility Functions

        private void WriteAnalyticsDataToFile()
        {
            Directory.CreateDirectory(_directoryPath);

            List<EquationsAnalyticsManager.EquationsData> equationsData =
                EquationsAnalyticsManager.Instance.GetEquationsData();
            string filePath = $"{_filePathBase}{DateTime.UtcNow.Ticks.ToString()}.csv";

            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                streamWriter.WriteLine("S. No.,Equation,Answer,Answer Chosen,Is Correct,Time Taken To Answer");
                foreach (var equation in equationsData)
                {
                    streamWriter
                        .WriteLine(
                            $"{equation.questionNumber},{equation.equation},{equation.answer},{equation.answerChosenByUser},{equation.gotCorrect},{equation.timeTakenToAnswer}");
                }
            }
        }

        #endregion
    }
}