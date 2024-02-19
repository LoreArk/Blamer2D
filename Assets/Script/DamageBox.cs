using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBox : MonoBehaviour
{
    List<I_Shootable> hits = new List<I_Shootable>();
    [SerializeField] private GameObject sparkPrefab;
    AnimatorHook animHook;


    private void OnEnable()
    {
        hits.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
            return;

        I_Shootable shootable = collision.GetComponent<I_Shootable>();

        if (shootable != null)
        {
            //GameObject instance =
            Instantiate(sparkPrefab, collision.ClosestPoint(collision.transform.position), Quaternion.Euler(0, 0, 0));


            if (!hits.Contains(shootable))
            {
                hits.Add(shootable);


                DamageSettings dmg = new DamageSettings();
                dmg.damage = 20;

                shootable.DoDamage(dmg);
            }

        }


    }
}
