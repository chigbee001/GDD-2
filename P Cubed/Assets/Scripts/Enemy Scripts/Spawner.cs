﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private int wavePattern;
    private int waveNumber;
    public Wave[] waves;
    public Transform spawnPoint;
    public float waveTimer;
    public float timerReset;
    public bool waveAlive;
    public bool spawnerOn;
    private bool waveSpawned;

    [SerializeField]
    private Player player;


    //currently WIP this class will be used to control spawners to allow for waves to come from seperate spawn points with similar or different pathings
    void Start()
    {
        wavePattern = 0;
        waveNumber = 0;
        waveAlive = false;
        waveSpawned = false;
        spawnerOn = true;
    }

    private void Update()
    {
        if(EnemyManager.EnemiesAlive == 0 && waveSpawned)
        {
            waveAlive = false;
        }

        if(EnemyManager.EnemiesAlive > 0 || waveAlive || !spawnerOn)
        {
            return;
        }
        if (waveTimer <= 0f || !waveAlive)
        {
            GameManager.currentLevel += 1;
            StartCoroutine(WaveSpawn());
            waveTimer = timerReset;
            return;
        }
        waveTimer -= Time.deltaTime;
        waveTimer = Mathf.Clamp(waveTimer, 0f, Mathf.Infinity);

    }

    /// <summary>
    /// Coroutine to handle the spawning of each wave
    /// </summary>
    /// <returns></returns>
    IEnumerator WaveSpawn()
    {
        waveSpawned = false;
        waveAlive = true;
        Wave wave = waves[waveNumber];
        for(int i = 0; i < wave.enemyRanks.Length; i++)
        {
            if (wave.enemyRanks[i].enemyNumbers > 0)
            {                
                for (int j = 0; j < wave.enemyRanks[i].enemyNumbers; j++)
                {
                    SpawnEnemy(wave.enemyRanks[i].enemyType);
                    yield return new WaitForSeconds(1 * wave.spawnRate);
                }
            }
            else
            { 
                SpawnEnemy(wave.enemyRanks[i].enemyType);
                yield return new WaitForSeconds(1 * wave.spawnRate);
            }
        }        
        if (wavePattern < Pathing.pathCount - 1)
        {
            wavePattern++;
        }
        else
        {
            wavePattern = 0;
        }       
        if( waveNumber < waves.Length - 1)
        {
            waveNumber++;
        }
        else
        {
            waveNumber = 0;
        }
        waveSpawned = true;
    }

    void SpawnEnemy(GameObject enemy)
    {
        
        Enemy thisEnemy = enemy.GetComponent<Enemy>();
        thisEnemy.pathNumber = wavePattern;        
        thisEnemy = Instantiate(thisEnemy, spawnPoint);
        thisEnemy.transform.parent = this.transform.GetChild(1);
        thisEnemy.player = player;
        EnemyManager.EnemiesAlive++;

    }
}
