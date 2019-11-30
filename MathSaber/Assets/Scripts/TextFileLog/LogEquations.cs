using System;
using System.Collections.Generic;
using UnityEngine;
using Equations;
using System.IO;

namespace TextFileLog
{
    public class LogEquations : MonoBehaviour
    {
        #region Unity Functions

        private void Start() => WriteAnalyticsDataToFile();

        #endregion

        #region External Functions

        static void WriteAnalyticsDataToFile()
        {
            List<EquationsAnalyticsManager.EquationsData> equationsData = EquationsAnalyticsManager.Instance.GetEquationsData();
            string path = $"{Application.persistentDataPath}/EquationsSolvedLog/Equations_{DateTime.UtcNow.Ticks.ToString()}.csv";

            using (StreamWriter streamWriter = new StreamWriter(path))
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