using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public GameObject pauseMenuUI; // ������ ������ �����
    public GameObject overlayPanel; // ������, ������������� ������� ���������
    public GameObject pauseButton; // ������ �����

    private bool isPaused = false;

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;
        pauseMenuUI?.SetActive(true);
        overlayPanel?.SetActive(true);
        pauseButton?.SetActive(false);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuUI?.SetActive(false);
        overlayPanel?.SetActive(false);
        pauseButton?.SetActive(true);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneController.Instance.MainMenuScene();
    }
}
