using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;




public class QuizManager : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI ScoreText;
    public Button[] answerButtons;

    public QuestionData questionData;

    private int currentQuestionIndex = 0;
    private int score = 0;

    public GameObject StartCanvas;
    public GameObject EndCanvas;
    public GameObject ReStart;



    void Start()
    {
        ShowQuestion();
    }



    void ShowQuestion()
    {
        Question q = questionData.questions[currentQuestionIndex];

        questionText.text = q.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.answers[i];

            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    void OnAnswerSelected(int index)
    {
        Question q = questionData.questions[currentQuestionIndex];

        if (index == q.correctIndex)
        {
            score++;
        }

        currentQuestionIndex++;

        if (currentQuestionIndex < questionData.questions.Length)
        {
            ShowQuestion();
        }

        else//答题结束
        {
            EndCanvas.SetActive(true);
            StartCanvas.SetActive(false);

            if(score==questionData.questions.Length)
            {
                string Score = score.ToString();
                ScoreText.text = ("您的得分为" + Score+"已通过测试！");
            }

            if(score!= questionData.questions.Length)
            {
                string Score = score.ToString();
                ScoreText.text = ("您的得分为" + Score + "未能通过测试！");
                ReStart.SetActive(true);
            }

        }
    }


}
