using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices.WindowsRuntime;

public class BossController : MonoBehaviour
{
    [Header("Vida del Boss")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    [Header("Disparo")]
    public GameObject bossBulletPrefab;
    public Transform firePoint;
    public float fireRate = 4f;
    private float fireCooldown;

    public Animator shootAnimator;

    public GameObject keys;

    void Start()
    {
        keys.gameObject.SetActive(false);
        currentHealth = maxHealth;

        shootAnimator = GetComponentInChildren<Animator>();

        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }

        fireCooldown = fireRate;
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = fireRate;
        }
    }

    void Shoot()
    {
        if (shootAnimator != null)
        {
            SoundManager.Instance.PlaySound2D("BossAttack");
            shootAnimator.SetTrigger("AttackTrigger");            
        }
        else
        {
            SoundManager.Instance.PlaySound2D("BossIdle");
        }
    }

    public void FireBullet()
    {
        SoundManager.Instance.PlaySound2D("BossBullet");
        Instantiate(bossBulletPrefab, firePoint.position, firePoint.rotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (healthText != null)
            healthText.text = currentHealth.ToString();
    }

    public void Die()
    {
        StartCoroutine(WaitDie());
    }

    IEnumerator WaitDie()
    {
        SoundManager.Instance.PlaySound2D("BossDie");
        shootAnimator.SetTrigger("Die");
        keys.gameObject.SetActive(true);

        yield return new WaitForSeconds(3);
        BossController bossController = new BossController();
        bossController.enabled = false;
        shootAnimator.enabled = false;
    }
}
