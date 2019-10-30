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

        private int _correctAnswers;
        private int _incorrectAnswers;

        void Start()
        {
            List<EquationsAnalyticsManager.EquationsData> equationsData = EquationsAnalyticsManager.Instance.GetEquationsData();
            foreach (var equation in equationsData)
            {
                if (equation.gotCorrect)
                {
                    _correctAnswers++;
                }
                else
                {
                    _incorrectAnswers++;
                }
            }

            correctAnswerText.text = _correctAnswers.ToString();
            incorrectAnswerText.text = _incorrectAnswers.ToString();
        }
    }
}