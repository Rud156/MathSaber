using System.Collections.Generic;
using UnityEngine;
using Equations;
using TMPro;

namespace UI
{
    public class DisplayScore : MonoBehaviour
    {
        public TextMeshPro correctAnswerText;
        public TextMeshPro incorrectAnswerText;

        private void Start()
        {
            List<EquationsAnalyticsManager.EquationsData> equationsData = EquationsAnalyticsManager.Instance.GetEquationsData();
            int correctAnswers = 0;
            int incorrectAnswers = 0;

            foreach (var equation in equationsData)
            {
                if (equation.gotCorrect)
                {
                    correctAnswers += 1;
                }
                else
                {
                    incorrectAnswers += 1;
                }
            }

            correctAnswerText.text = correctAnswers.ToString();
            incorrectAnswerText.text = incorrectAnswers.ToString();
        }
    }
}