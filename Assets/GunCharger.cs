using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCharger : MonoBehaviour
{

    float timer = 0;
    [SerializeField] private float chargeRatio = .02f;

    ParticleSystemForceField forceField;
    ParticleSystem loadParticleSystem;
    [SerializeField] private Transform particlesStart;
    AudioSource audioSource;
    public PlayerStateManager player;
    public float distance;
    private void Start()
    {
        forceField = GetComponent<ParticleSystemForceField>();
        loadParticleSystem = particlesStart.GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0;
    }

    private void Update()
    {
        if (player)
        {
            distance = Vector2.Distance(player.transform.position, transform.position);
        }

        if (distance > 3 && player || player == null)
        {
            if (loadParticleSystem.isPlaying)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.volume = 0;
                    audioSource.loop = false;
                    audioSource.Stop();
                }

                loadParticleSystem.Stop();
            }
            return;
        }
    }



    private void OnTriggerStay2D(Collider2D collision)
    {

        player = collision.GetComponent<PlayerStateManager>();
        Debug.Log(player);
        if (distance > 3 && player || player == null)
        {
            if (loadParticleSystem.isPlaying)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.volume = 0;
                    audioSource.loop = false;
                    audioSource.Stop();
                }

                loadParticleSystem.Stop();
            }
            return;
        }

        if(player.gunLoad < player.maxGunLoad)
        {

            audioSource.volume += Time.deltaTime;
            if (!loadParticleSystem.isPlaying)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.loop = true;
                    audioSource.Play();

                }

                loadParticleSystem.Play();
            }

            if (timer >=  chargeRatio)
            {
                timer = 0;
                player.AddGunLoad(1);
            }

            Debug.Log(timer);
            timer += Time.deltaTime;
        }
        else if (loadParticleSystem.isPlaying)
        {
            if (audioSource.isPlaying)
            {
                audioSource.loop = false;
                audioSource.volume = 0;
                audioSource.Stop();
            }
            loadParticleSystem.Stop();
        }
            
    }
    

   
}
