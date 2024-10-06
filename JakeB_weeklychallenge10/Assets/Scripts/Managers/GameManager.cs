using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public GameObject gameOverCanvas;
    public GameObject mainCanvas;
    public TextMeshProUGUI timerText;
    public float gameDuration = 300f;

    private float timeRemaining;
    private bool isGameActive = true;

    private void Start() {
        gameOverCanvas.SetActive(false);
        timeRemaining = gameDuration;
        UpdateTimerUI();
    }

    private void Update() {
        if (isGameActive) {
            if (timeRemaining > 0) {
                timeRemaining -= Time.deltaTime;
                UpdateTimerUI();
            } else {
                timeRemaining = 0;
                EndGame();
            }
        }
    }

    private void UpdateTimerUI() {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void EndGame() {
        isGameActive = false;
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0f;
        mainCanvas.SetActive(false);
    }

    public void ResetGame() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}