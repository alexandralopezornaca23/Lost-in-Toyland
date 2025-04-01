using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public PlayerInput playerInput;
    public CinemachineBrain cinemachineBrain;

    private bool isPaused = false;

    // Update is called once per frame
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
        }
        else
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
            playerInput.enabled = true;
            cinemachineBrain.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        playerInput.enabled = true;
        cinemachineBrain.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
