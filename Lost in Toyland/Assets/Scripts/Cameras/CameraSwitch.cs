using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitch : MonoBehaviour
{
    public Camera fpsCam;
    public Camera tpsCam;

    private bool fpsEnabled = true;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            fpsEnabled = !fpsEnabled;
            ChangeCamera();
        }
    }

    public void ChangeCamera()
    {
        if (fpsEnabled)
        {
            fpsCam.enabled = true;
            tpsCam.enabled = false;
        }
        else
        {
            fpsCam.enabled = false;
            tpsCam.enabled = true;
        }
    }
}


