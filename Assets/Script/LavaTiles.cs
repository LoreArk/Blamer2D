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
        target = collision.GetComponent<I_Shootable>();
        
        if (target != collision.GetComponent<I_Shootable>() || collision == null)
            target = null;


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<I_Shootable>() != null)
        {
            collision.GetComponent<I_Shootable>();

            damageSettings = new DamageSettings();
            damageSettings.damage = 1;
            collision.GetComponent<I_Shootable>().DoDamage(damageSettings);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(target == collision.GetComponent<I_Shootable>())
        {
            Debug.Log("target null");
            target = null;
        }    
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
