using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [SerializeField] private int amount = 1;
    bool used;
    AudioSource audioSource;
    SpriteRenderer sprite;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (used)
            return;
        if (!collision)
            return;

        PlayerStateManager player = collision.GetComponent<PlayerStateManager>();

        if (player)
        {
            if(player.damageSystem.health < player.damageSystem.maxHealth)
            {
                collision.GetComponent<PlayerStateManager>().Heal(amount);

                audioSource.volume = audioSource.volume * AudioManager.instance.masterVolume;
                audioSource.Play();
                used = true;

                sprite.enabled = false;
                Destroy(gameObject, 2);
            }
        }
    }

}
