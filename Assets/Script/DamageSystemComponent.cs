using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageSystemComponent : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;
    private bool isDead;
    private bool isInvincible;
    [SerializeField] public UnityEvent onDeath;
    [SerializeField] private float invincibilityTime;

    void Start()
    {

    }

    void Update()
    {
        
    }


    public void DoDamage(DamageSettings dmg)
    {
        if (isInvincible)
            return;

        health -= dmg.damage;

        if(health <= 0)
        {
            health = 0;
            isDead = true;
        }

        //StartCoroutine(Invincibility());

        Debug.Log(health);

        if (isDead)
        {
            onDeath.Invoke();
        }
    }

    public IEnumerator Invincibility() 
    { 
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

}
