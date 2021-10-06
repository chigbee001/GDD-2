using System.Collections;
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
    public static int EnemiesAlive;


    //currently WIP this class will be used to control spawners to allow for waves to come from seperate spawn points with similar or different pathings
    void Start()
    {
        wavePattern = 0;
        waveNumber = 0;
    }

    private void Update()
    {
        if(EnemiesAlive > 0)
        {
            return;
        }
        if (waveTimer <= 0f)
        {
            
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
    }

    void SpawnEnemy(GameObject enemy)
    {
        
        Enemy thisEnemy = enemy.GetComponent<Enemy>();
        thisEnemy.pathNumber = wavePattern;        
        thisEnemy = Instantiate(thisEnemy, spawnPoint);
        thisEnemy.transform.parent = this.transform.GetChild(1);
        EnemiesAlive++;

    }
}
