using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitch : MonoBehaviour
{
    public GameObject[] weapons;

    public int selectedWeapon = 0;

    public PlayerController playerController;

    void Start()
    {
        SelectWeapon();
    }

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

        if (Keyboard.current.digit1Key.wasPressedThisFrame && playerController.hasNonGun)
        {
            playerController.jumpAnimation = Animator.StringToHash("PlayerJump");
            playerController.animator.SetBool("EquipNonGun", true);
            playerController.animator.SetBool("EquipPistol", false);
            playerController.animator.SetBool("EquipRifle", false);
            selectedWeapon = 0;
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame && playerController.hasPistol && weapons.Length >= 2)
        {
            playerController.jumpAnimation = Animator.StringToHash("PlayerJumpPistol");
            playerController.animator.SetBool("EquipNonGun", false);            
            playerController.animator.SetBool("EquipPistol", true);
            playerController.animator.SetBool("EquipRifle", false);
            selectedWeapon = 1;
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame && playerController.hasRifle && weapons.Length >= 3)
        {
            playerController.jumpAnimation = Animator.StringToHash("PlayerJumpRifle");
            playerController.animator.SetBool("EquipNonGun", false);
            playerController.animator.SetBool("EquipPistol", false);
            playerController.animator.SetBool("EquipRifle", true);
            selectedWeapon = 2;
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame && playerController.hasFrozenGun && weapons.Length >= 4)
        {
            playerController.jumpAnimation = Animator.StringToHash("PlayerJumpPistol");
            playerController.animator.SetBool("EquipNonGun", false);
            playerController.animator.SetBool("EquipPistol", true);
            playerController.animator.SetBool("EquipRifle", false);
            selectedWeapon = 3;
        }

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
