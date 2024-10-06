using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager instance;

    public TextMeshProUGUI scoreText;
    private int score = 0;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void IncreaseScore(int amount) {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI() {
        scoreText.text = "Score: " + score;
    }

    public void ResetScore() {
        score = 0;
        UpdateScoreUI();
    }
}