using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int damageReceived;
    public int totalEnemies = 0;
    public int enemiesKilled = 0;
    public float time = 0;
    public float usedEnergy = 0;

    private void Awake()
    {
        instance = this;        
    }

    public void Start()
    {

    }

    public void Update()
    {
        time += Time.deltaTime;
    }

    
}
