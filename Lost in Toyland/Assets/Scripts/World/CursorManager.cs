using UnityEngine;
using UnityEngine.InputSystem;


public class CursorManager : MonoBehaviour
{
    private bool isCursorLocked = false;

    void Start()
    {
        LockCursor();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            LockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorLocked = true;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorLocked = false;
    }
}
