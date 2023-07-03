using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManagerScript : MonoBehaviour
{
   public int currentScore;
   public Text scoreText;

    [ContextMenu("Increase current score")]
   public void addScore(int scoreToAdd)
    {
        currentScore = currentScore + scoreToAdd;
        scoreText.text = "Score: ";
        scoreText.text += currentScore.ToString();
    }

    
}
