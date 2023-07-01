using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManagerScript : MonoBehaviour
{
   public int currentScore;
   public Text scoreText;

    [ContextMenu("Increase current score")]
   public void addScore()
    {
        currentScore = currentScore + 1;
        scoreText.text = "Score: ";
        scoreText.text += currentScore.ToString();
    }
}
