using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Equations;
using System.IO;

public class LogEquations : MonoBehaviour
{
    #region Unity Functions
    void Start()
    {
        WriteString();
    }

    
    void Update()
    {
        
    }
    #endregion
    #region External Functions
    static void WriteString()
    {
        List<EquationsAnalyticsManager.EquationsData> equationsDatas = EquationsAnalyticsManager.Instance.GetEquationsData();
        string path = Application.persistentDataPath + "/EqationsSolvedLog/equations.csv";
        StreamWriter streamWriter = new StreamWriter(path, true);
        streamWriter.WriteLine("Equation,Answer,GotCorrect,TimeBeforeAnswer");
        foreach (var equations in equationsDatas)
        {
            streamWriter.WriteLine($"{equations.equation},{equations.answer},{equations.gotCorrect},{equations.timeBeforeAnswer}");
        }
        streamWriter.Close();

    }
    #endregion
}
