using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LavaTiles : MonoBehaviour
{
     private DamageSettings damageSettings;

    I_Shootable target;

    void Start()
    {
        InvokeRepeating("DoDamage", 1f, 2f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (target != collision.GetComponent<I_Shootable>() || collision == null)
            target = null;

        target = collision.GetComponent<I_Shootable>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        //Debug.Log(collision.transform.gameObject.name);
    }

    void DoDamage()
    {
        if (target == null)
            return;
        damageSettings = new DamageSettings();
        damageSettings.damage = 1;
        target.DoDamage(damageSettings);
    }
}
