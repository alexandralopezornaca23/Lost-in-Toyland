using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider staminaSlider;

    public float maxStamina = 100f;
    private float currentStamina;

    private float regenerateStaminaTime = 0.1f;
    private float regenerateAmount = 2f;

    private float losingStaminaTime = 0.1f;

    private Coroutine myCoroutineLosing;
    private Coroutine myCoroutineRegenerating;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = maxStamina;
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
            FindFirstObjectByType<PlayerMovement>().isSprinting = false;
            Debug.Log("No tenemos Stamina");
        }
    }

    private IEnumerator LosingStaminaCoroutine(float amount)
    {
        while (currentStamina >= 0)
        {
            currentStamina -= amount;

            staminaSlider.value = currentStamina;

            yield return new WaitForSeconds(losingStaminaTime);
        }

        myCoroutineLosing = null;

        FindFirstObjectByType<PlayerMovement>().isSprinting = false;
    }

    private IEnumerator RegenerateStaminaCoroutine()
    {
        yield return new WaitForSeconds(1);

        while (currentStamina < maxStamina)
        {
            currentStamina += regenerateAmount;
            staminaSlider.value = currentStamina;

            yield return new WaitForSeconds(regenerateStaminaTime);
        }

        myCoroutineRegenerating = null;
    }
}
