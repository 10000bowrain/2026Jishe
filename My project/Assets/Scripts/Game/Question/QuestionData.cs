using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "QuestionData", menuName = "Quiz/QuestionData")]
public class QuestionData : ScriptableObject
{
    public Question[] questions;
}
