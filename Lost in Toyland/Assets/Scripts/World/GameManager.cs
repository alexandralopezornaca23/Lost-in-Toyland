using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI healthText;


    public static GameManager Instance { get; private set; }

    public int gunAmmo = 10;
    public int health = 100;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        ammoText.text = gunAmmo.ToString();
        healthText.text = health.ToString();
    }
}
