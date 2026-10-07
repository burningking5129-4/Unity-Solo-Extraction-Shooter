using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;


public class GameManager : MonoBehaviour
{
    public Enemy Enemy;

    public PlayerController Player;

    public Image healthbar;

    public TextMeshProUGUI AmmoText;

    public bool paused = false;
    public bool giveWarning = false;

    public GameObject pauseMenu;

    public GameObject Warning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1;
        
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            healthbar = GameObject.Find("healthbar").GetComponent<Image>();

            AmmoText = GameObject.Find("AmmoText").GetComponent<TextMeshProUGUI>();

            pauseMenu = GameObject.FindGameObjectWithTag("Pause");
            pauseMenu.SetActive(false);

            Warning = GameObject.FindGameObjectWithTag("warning");
            Warning.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            healthbar.fillAmount = (float)Player.hp / (float)Player.maxHp;

            if (Player.Weapon)
            {
                AmmoText.text = "Ammo: " + Player.Weapon.mag + "/" + Player.Weapon.ammo;
            }
        }

        if(Player.hp <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if(giveWarning == true)
        {
            Warning.SetActive(true);
        }
        if(giveWarning == false)
        {
            Warning.SetActive(false);
        }
    }
        
    public void Pause()
    {
        paused = !paused;

        if (paused)
        {
            Time.timeScale = 0;

            pauseMenu.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;

            pauseMenu.SetActive (false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void LoadLevel(int LevelID)
    {
        if(LevelID >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Level ID is too high" + LevelID);
        }
        else
            SceneManager.LoadScene(LevelID);
    }

    public void LoadNextLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void MainMenu()
    {
        LoadLevel(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
    
}

