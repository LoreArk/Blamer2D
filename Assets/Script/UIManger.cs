using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManger : MonoBehaviour
{
    HealthBarHUD healthBar;
    PlayerStateManager player;
    DamageSystemComponent playerDamageComponent;
    public static UIManger instance;
    [SerializeField] private TMP_Text uiTextMessage;
    [SerializeField] private TMP_Text gunLoad;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameoverMenu;
    bool pause = false;
    bool gameOver = false;

    EventSystem eventSystem;
    GameObject lastSelected;
    AudioSource buttonAudio;

    [Header("Win Menu Components")]
    [SerializeField] private GameObject winMenu;
    [SerializeField] private TMP_Text enemies;
    [SerializeField] private TMP_Text energyConsumed;
    [SerializeField] private TMP_Text damageReceived;
    [SerializeField] private TMP_Text time;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        eventSystem = FindAnyObjectByType<EventSystem>();
        healthBar = GetComponentInChildren<HealthBarHUD>();
        
        player = PlayerStateManager.Instance;
        playerDamageComponent = player.gameObject.GetComponent<DamageSystemComponent>();
       // Debug.Log(playerDamageComponent);
        healthBar.UpdateHealthBar(playerDamageComponent.maxHealth);
        buttonAudio = GetComponentInChildren<AudioSource>();

        Time.timeScale = 1;
    }


    void Update()
    {
        if(pause || gameOver)
        if (lastSelected != eventSystem.currentSelectedGameObject && lastSelected != null)
        {
            AudioManager.instance.PlayButtonSelectionSound(buttonAudio);
        }

        lastSelected = eventSystem.currentSelectedGameObject;

        if (gameOver)
            return;

        if (InputManager.instance.exit.WasPressedThisFrame())
        {
            pause = !pause;
            PauseInput();
        }

        if(pause)
        if (InputManager.instance.inputAsset.currentControlScheme != "Gamepad")
        {
                //eventSystem.SetSelectedGameObject(null);
        }
    }

    public void UpdateHealthBar()
    {
        healthBar.UpdateHealthBar(playerDamageComponent.health);
    }

    public void UpdateGunLoad(float load)
    {
        gunLoad.text = load.ToString();
    }

    public void SendUIMessage(string message, float time)
    {
        uiTextMessage.gameObject.SetActive(true);
        uiTextMessage.text = message;

        if(time > 0)
        {
            StartCoroutine(MessageTime(time));
        }
    }

    IEnumerator MessageTime(float time)
    {
        yield return new WaitForSeconds(time);
        DisableUIMessage();
    }

    public void DisableUIMessage()
    {
        uiTextMessage.gameObject.SetActive(false);

    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    void PauseInput()
    {
        if (pause)
        {
            PauseMenu();
        }
        else
        {
            ExitPause();
        }
    }

    public void PauseMenu()
    {
        AudioManager.instance.PauseAmbience();
        InputManager.instance.DisableGameInput();
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void GameOverMenu()
    {
        gameOver = true;
        DisableUIMessage();
        InputManager.instance.DisableGameInput();
        GetComponent<Animator>().Play("death");
        StartCoroutine(WaitBeforeDeath());

    }

    IEnumerator WaitBeforeDeath()
    {
        yield return new WaitForSeconds(2);
        gameoverMenu.SetActive(true);
        AudioManager.instance.PlayDeathAudio();
        Time.timeScale = 0;
    }



    public void ExitPause()
    {
        AudioManager.instance.ResumeAmbience();
        InputManager.instance.EnableGameInput();

        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void WinMenu()
    {
        // Time.timeScale = 0;
        gameOver = true;
        GameManager manager = GameManager.instance;
        InputManager.instance.DisableGameInput();

        float minutes = Mathf.Floor(manager.time / 60);
        float seconds = Mathf.RoundToInt(manager.time % 60);
        string m = minutes.ToString();
        string s = seconds.ToString();
        if (minutes < 10)
        {
            m = "0" + minutes.ToString();
        }
        if (seconds < 10)
        {
            s = "0" + Mathf.RoundToInt(seconds).ToString();
        }

        time.text = m + ":" + s;
        enemies.text = manager.enemiesKilled.ToString() + "/" + manager.totalEnemies.ToString();
        damageReceived.text = manager.damageReceived.ToString();
        energyConsumed.text = manager.usedEnergy.ToString();

        GetComponent<Animator>().Play("win");
    }


    public void QuitApplication()
    {
        Application.Quit();
    }

    public void SetSelectedButton(GameObject gameObject)
    {
        eventSystem.SetSelectedGameObject(gameObject);
    }
}
