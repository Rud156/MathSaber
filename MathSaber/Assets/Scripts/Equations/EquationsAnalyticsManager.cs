﻿using System.Collections.Generic;
using UnityEngine;

namespace Equations
{
    public class EquationsAnalyticsManager : MonoBehaviour
    {
        public struct EquationsData
        {
            public string equation;
            public string answer;
            public bool gotCorrect;
            public float timeBeforeAnswer;
        }

        private List<EquationsData> _equationsData;

        #region Unity Functions

        private void Start()
        {
            _equationsData = new List<EquationsData>();
        }

        #endregion

        #region External Functions

        public void AddEquationToList(string equation, string answer, bool gotCorrect, float timeBeforeAnswer)
        {
            EquationsData equationsData = new EquationsData()
            {
                equation = equation,
                answer = answer,
                gotCorrect = gotCorrect,
                timeBeforeAnswer = timeBeforeAnswer
            };
            _equationsData.Add(equationsData);
        }

        public List<EquationsData> GetEquationsData() => _equationsData;

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