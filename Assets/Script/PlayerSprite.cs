using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : MonoBehaviour
{
    public Transform arm;
    public Transform bulletSpawn;
    public Transform target;
    public List<SpriteRenderer> sprites = new List<SpriteRenderer>();

    private void OnEnable()
    {
        SpriteSetup();
    }

    public void SpriteSetup()
    {
        //GetComponentInParent<PlayerStateManager>().arm = arm;
        GetComponentInParent<PlayerStateManager>().aimBulletSpawn = bulletSpawn;
        //GetComponentInParent<PlayerStateManager>().aimTarget = target;
    }
}
