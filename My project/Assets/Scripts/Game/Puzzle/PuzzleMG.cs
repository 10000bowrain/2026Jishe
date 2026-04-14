using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PuzzleMG : MonoBehaviour
{
    public static PuzzleMG instance;
    public List<PuzzlePiece> puzzles = new List<PuzzlePiece>();
    public GameObject gameOverPanel;

    void Start()
    {
        instance = this;
        puzzles.Clear();
        puzzles.AddRange(GetComponentsInChildren<PuzzlePiece>());
    }

    public void Check()
    {
        foreach (PuzzlePiece puzzle in puzzles)
        {
            if (!puzzle.isComplete)
            {
                return;
            }
        }

        gameOverPanel.SetActive(true);
    }
}
