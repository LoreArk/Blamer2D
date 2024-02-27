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

    private void Start()
    {
        forceField = GetComponent<ParticleSystemForceField>();
        loadParticleSystem = particlesStart.GetComponent<ParticleSystem>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (!collision)
            return;

        PlayerStateManager player = collision.GetComponent<PlayerStateManager>();

        if (!player)
        {
            if (loadParticleSystem.isPlaying)
            {
                loadParticleSystem.Stop();
            }
            return;
        }

        if(player.gunLoad < player.maxGunLoad)
        {

           
            if (!loadParticleSystem.isPlaying)
                loadParticleSystem.Play();

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
            loadParticleSystem.Stop();
        }
            
    }
    

   
}
