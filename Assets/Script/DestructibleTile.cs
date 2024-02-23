using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleTile : MonoBehaviour, I_Shootable
{
    [SerializeField] private int health = 20;
    bool invincible;
    [SerializeField] private ParticleSystem damagedParticle;
    [SerializeField] private ParticleSystem destroyedParticle;
    [SerializeField] private BoxCollider2D spriteCollider;


    void Start()
    {
        
    }


    public void DoDamage(DamageSettings dmg)
    {
        if (!invincible)
        {
            health -= dmg.damage;
            //Debug.Log("Destructible damage: " + dmg.damage);
            invincible = true;
            damagedParticle.Play();
            StartCoroutine(Invincibility());
        }

        if (health <= 0)
        {
            //Debug.Log("Dead");
            SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
            sprite.enabled = false;
            destroyedParticle.Play();
            GetComponent<BoxCollider2D>().enabled = false;
            Destroy(spriteCollider.gameObject);
            Destroy(gameObject, 2);
        }
    }

    IEnumerator Invincibility()
    {
        yield return new WaitForSeconds(0.2f);
        invincible = false;
    }
}
