using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManger : MonoBehaviour
{
    HealthBarHUD healthBar;
    PlayerStateManager player;
    DamageSystemComponent playerDamageComponent;
    public static UIManger instance;
    [SerializeField] private TMP_Text uiTextMessage;
    [SerializeField] private TMP_Text gunLoad;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        healthBar = GetComponentInChildren<HealthBarHUD>();
        
        player = PlayerStateManager.Instance;
        Debug.Log(player);
        playerDamageComponent = player.gameObject.GetComponent<DamageSystemComponent>();
        Debug.Log(playerDamageComponent);
        healthBar.UpdateHealthBar(playerDamageComponent.maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
