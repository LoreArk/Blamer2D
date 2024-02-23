using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCharger : MonoBehaviour
{

    float timer = 0;
    [SerializeField] private float chargeRatio = .02f;

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (!collision)
            return;

        PlayerStateManager player = collision.GetComponent<PlayerStateManager>();

        if (!player)
            return;

        if(player.gunLoad < player.maxGunLoad)
        {

            if (timer >=  chargeRatio)
            {
                timer = 0;
                player.AddGunLoad(1);
            }

            Debug.Log(timer);
            timer += Time.deltaTime;
            
        }

            
    }
    

   
}
