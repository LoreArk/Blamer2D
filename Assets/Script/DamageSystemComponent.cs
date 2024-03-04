using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageSystemComponent : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    [SerializeField] public int health;
    public bool isDead;
    [HideInInspector] public bool isInvincible;
    [SerializeField] public UnityEvent onDeath;
    [SerializeField] public UnityEvent onDamage;
    [SerializeField] public UnityEvent onHeal;
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
        if (isDead)
            return;


        health -= dmg.damage;
        onDamage.Invoke();

        if(health <= 0)
        {
            health = 0;
            isDead = true;
        }

        //StartCoroutine(Invincibility());

        //Debug.Log(health);

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

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;

        onHeal.Invoke();

    }
}
