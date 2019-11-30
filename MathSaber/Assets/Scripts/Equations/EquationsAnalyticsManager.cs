using System.Collections.Generic;
using UnityEngine;

namespace Equations
{
    public class EquationsAnalyticsManager : MonoBehaviour
    {
        public struct EquationsData
        {
            public int questionNumber;
            public string equation;
            public string answer;
            public string answerChosenByUser;
            public bool gotCorrect;
            public float timeTakenToAnswer;
        }

        private List<EquationsData> _equationsData;

        #region Unity Functions

        private void Start()
        {
            if (_equationsData == null)
            {
                _equationsData = new List<EquationsData>();
            }
        }

        #endregion

        #region External Functions

        public void AddEquationToList(int questionNumber, string equation, string answer, string answerChosenByUser, bool isCorrect, float timeTakenToAnswer)
        {
            EquationsData equationsData = new EquationsData()
            {
                questionNumber = questionNumber,
                equation = equation,
                answer = answer,
                answerChosenByUser = answerChosenByUser,
                gotCorrect = isCorrect,
                timeTakenToAnswer = timeTakenToAnswer
            };
            _equationsData.Add(equationsData);
        }

        public List<EquationsData> GetEquationsData() => _equationsData;

        public void ClearAnalyticsData() => _equationsData.Clear();

        #endregion

        #region Singleton

        private static EquationsAnalyticsManager _instance;

        public static EquationsAnalyticsManager Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }

            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        #endregion
    }
}