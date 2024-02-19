using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarHUD : MonoBehaviour
{
    [SerializeField] private GameObject healthUnitPrefab;
    public List<GameObject> createdSprites;

    public void UpdateHealthBar(int amount)
    {
        Debug.Log("UPDATE MANA: " + amount);
        ClearBar();

        for(int i =0; i < amount; i++)
        {
            GameObject newUnit = Instantiate(healthUnitPrefab, transform);
            createdSprites.Add(newUnit);
        }
    }

    void ClearBar()
    {
        foreach(GameObject go in createdSprites)
        {
            Destroy(go);
        }
    }
}
