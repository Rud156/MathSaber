using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Equations;
using TMPro;

public class DisplayScore : MonoBehaviour
{
    // Start is called before the first frame update

    private int correctAnswers = 0;
    private int incorrectAnswers = 0;
   
    public TextMeshPro correctAnswerText;
    public TextMeshPro incorrectAnswerText;
    void Start()
    {
        List<EquationsAnalyticsManager.EquationsData> equationsDatas = EquationsAnalyticsManager.Instance.GetEquationsData();
        foreach(var equation in equationsDatas)
        {
            if(equation.gotCorrect==true)
            {
                correctAnswers++;
            }
            else
            {
                incorrectAnswers++;
            }
        }
        Debug.Log(correctAnswers);
        Debug.Log(incorrectAnswers);
        correctAnswerText.text = correctAnswers.ToString();
        incorrectAnswerText.text = incorrectAnswers.ToString();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
