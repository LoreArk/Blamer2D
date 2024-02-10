using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveTile : MonoBehaviour, I_Shootable
{
    public GameObject explosionPrefab;
    public LayerMask layers;
    public SpriteRenderer tileSprite;
    bool exploded;

    void Start()
    {
        tileSprite = GetComponentInChildren<SpriteRenderer>(); 
    }

    public void DoDamage(DamageSettings newDamage)
    {
        if (exploded)
            return;
        exploded = true;

        tileSprite.enabled = false;
        GameObject explostionInstance = Instantiate(explosionPrefab);
        Vector3 spawnPos = gameObject.transform.localPosition;
        spawnPos.y += 0.5f;
        explostionInstance.transform.position = spawnPos;
        Debug.DrawRay(spawnPos, transform.right, Color.black, 3f);
        Debug.DrawRay(spawnPos, transform.up, Color.black, 3f);

        StartCoroutine(DealDamage(spawnPos));
    }

    IEnumerator DealDamage(Vector3 pos)
    {
        yield return new WaitForSeconds(0.2f);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, 0.75f, layers);
        //Debug.Log(colliders.Length);

        foreach (Collider2D c in colliders)
        {
            if (c.GetComponent<I_Shootable>() != null)
            {
                // Debug.Log(c.gameObject.name);
                DamageSettings newDamage = new DamageSettings();
                newDamage.damage = 100;
                newDamage.explosive = true;
                c.GetComponent<I_Shootable>().DoDamage(newDamage);
            }
        }

        Destroy(gameObject);
    }


}

[System.Serializable]
public class DamageSettings
{
    public int damage;
    public bool explosive;

}

