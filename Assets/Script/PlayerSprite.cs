using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : MonoBehaviour
{
    public Transform arm;
    public Transform bulletSpawn;
    public Transform target;

    private void OnEnable()
    {
        SpriteSetup();
    }

    public void SpriteSetup()
    {
        //GetComponentInParent<PlayerStateManager>().arm = arm;
        GetComponentInParent<PlayerStateManager>().bulletSpawn = bulletSpawn;
        //GetComponentInParent<PlayerStateManager>().aimTarget = target;
    }
}
