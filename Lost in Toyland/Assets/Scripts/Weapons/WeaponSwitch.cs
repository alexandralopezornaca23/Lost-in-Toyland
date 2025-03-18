using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitch : MonoBehaviour
{
    public GameObject[] weapons;

    public int selectedWeapon = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        int previousWeapon = selectedWeapon;

        float scrollValue = Mouse.current.scroll.ReadValue().y;

        if (scrollValue > 0)
        {
            selectedWeapon = (selectedWeapon >= weapons.Length - 1) ? 0 : selectedWeapon + 1;
        }
        else if (scrollValue < 0)
        {
            selectedWeapon = (selectedWeapon <= 0) ? weapons.Length - 1 : selectedWeapon - 1;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame) selectedWeapon = 0;
        if (Keyboard.current.digit2Key.wasPressedThisFrame && weapons.Length >= 2) selectedWeapon = 1;
        if (Keyboard.current.digit3Key.wasPressedThisFrame && weapons.Length >= 3) selectedWeapon = 2;

        if (previousWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    void SelectWeapon()
    {
        int i = 0;

        foreach (Transform weapon in transform)
        {
            if (weapon.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {
                weapon.gameObject.SetActive(i == selectedWeapon);
                i++;
            }
        }
    }
}
