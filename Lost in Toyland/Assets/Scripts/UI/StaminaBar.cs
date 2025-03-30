using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider staminaBar;

    public float maxStamina = 100f;
    public float currentStamina;

    private float regenerateStaminaTime = 0.1f;
    private float regenerateAmount = 2f;

    private float losingStaminaTime = 0.1f;

    private Coroutine myCoroutineLosing;
    private Coroutine myCoroutineRegenerating;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
    }

    public void UseStamina(float amount)
    {
        if (currentStamina - amount > 0)
        {
            if (myCoroutineLosing != null)
            {
                StopCoroutine(myCoroutineLosing);
            }
            myCoroutineLosing = StartCoroutine(LosingStaminaCoroutine(amount));

            if (myCoroutineRegenerating != null)
            {
                StopCoroutine(myCoroutineRegenerating);
            }
            myCoroutineRegenerating = StartCoroutine(RegenerateStaminaCoroutine());
        }
        else
        {
            currentStamina = 0;
            staminaBar.value = currentStamina;
            FindFirstObjectByType<PlayerController>().isSprinting = false;
            FindFirstObjectByType<PlayerController>().animator.SetBool("isSprinting", false); // Asegurar que la animación se desactive
        }
    }

    public void RecoverStamina(float amount)
    {
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
        staminaBar.value = currentStamina; // Asegurar que la UI refleje el cambio
    }

    private IEnumerator LosingStaminaCoroutine(float amount)
    {
        while (currentStamina >= 0)
        {
            currentStamina -= amount;

            staminaBar.value = currentStamina;

            yield return new WaitForSeconds(losingStaminaTime);
        }

        myCoroutineLosing = null;

        FindFirstObjectByType<PlayerController>().isSprinting = false;
    }

    private IEnumerator RegenerateStaminaCoroutine()
    {
        yield return new WaitForSeconds(1);

        while (currentStamina < maxStamina)
        {
            currentStamina += regenerateAmount;
            staminaBar.value = currentStamina;

            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
                staminaBar.value = currentStamina;
                yield break; // Salir del bucle si la estamina ya está al máximo
            }

            yield return new WaitForSeconds(regenerateStaminaTime);
        }

        myCoroutineRegenerating = null;
    }
}
