using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadItem : MonoBehaviour
{
    [SerializeField] private float amount;
    bool used;
    SpriteRenderer sprite;

    private void Start()
    {
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
            if (player.gunLoad < player.maxGunLoad)
            {
                float load = amount;
                if(player.gunLoad + amount > player.maxGunLoad)
                {
                    load = player.maxGunLoad - player.gunLoad;
                }

                collision.GetComponent<PlayerStateManager>().AddGunLoad(load);

               // audioSource.volume = audioSource.volume * AudioManager.instance.masterVolume;
               // audioSource.Play();
                used = true;

                sprite.enabled = false;
                Destroy(gameObject, 2);
            }
        }
    }
}
