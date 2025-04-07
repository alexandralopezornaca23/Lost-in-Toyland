using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public PlayerInput playerInput;
    public CinemachineBrain cinemachineBrain;

    private bool isPaused = false;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            isPaused = !isPaused;
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            playerInput.enabled = false;
            cinemachineBrain.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            FindFirstObjectByType<PlayerController>().isPaused = true;
        }
        else
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
            playerInput.enabled = true;
            cinemachineBrain.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;

            FindFirstObjectByType<PlayerController>().isPaused = false;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        playerInput.enabled = true;
        cinemachineBrain.enabled = true;

        FindFirstObjectByType<PlayerController>().isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
