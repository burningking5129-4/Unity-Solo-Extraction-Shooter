using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    public PlayerController Player;

    public Image healthbar;

    public TextMeshProUGUI AmmoText;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        healthbar = GameObject.Find("healthbar").GetComponent<Image>();

        AmmoText = GameObject.Find("AmmoText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        healthbar.fillAmount = (float)Player.hp / (float)Player.maxHp;

        if (Player.currentWeapon)
        {
            AmmoText.text = "Ammo: " + Player.currentWeapon.mag + "/" + Player.currentWeapon.ammo;
        }
    }
}
