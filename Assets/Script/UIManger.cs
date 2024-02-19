using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManger : MonoBehaviour
{
    HealthBarHUD healthBar;
    PlayerStateManager player;
    DamageSystemComponent playerDamageComponent;

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
}
