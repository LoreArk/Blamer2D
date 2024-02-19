using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedShotSprite : MonoBehaviour
{
    public Transform arm;
    public Transform bulletSpawn;
    public Transform target;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        GetComponentInParent<PlayerStateManager>().arm = arm;
        GetComponentInParent<PlayerStateManager>().bulletSpawn = bulletSpawn;
        GetComponentInParent<PlayerStateManager>().aimTarget = target;
    }
}
