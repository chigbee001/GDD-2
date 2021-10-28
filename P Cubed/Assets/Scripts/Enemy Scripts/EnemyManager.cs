using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private int pointsToSpend;
    private bool costsExceedPoints;
    public int pointsScaling;
    public GameObject[] enemyTypes;
    public static int EnemiesAlive;
    public Spawner[] spawners;
    public static float waveTimer;
    public float timerReset;
    public static bool waveAlive;
    public static bool spawnWave;
    public static bool allComplete;

    private Wave nextWave;
    private EnemyRank nextRank;
    private int rand;
    private int enemiesInRankCounter;
    private int rollCounter;
    private int currentPath;
    private bool pathChosen;
    private bool waveSpawned;
    private bool spawnInProcess;
    private int rankCounter;


    private void Start()
    {
        EnemiesAlive = 0;
        waveAlive = false;
        waveSpawned = false;
        spawnWave = true;
        spawnInProcess = false;
        waveTimer = 0;
    }

    private void Update()
    {
        if (allComplete == false)
        {
            foreach (Spawner s in spawners)
            {
                if(!s.complete)
                {
                    allComplete = false;
                    break;
                }
                allComplete = true;
            }
        }

        if (EnemiesAlive == 0 && waveSpawned)
        {
            waveAlive = false;
            waveSpawned = false;
            GameManager.currentLevel += 1;
            return;
        }
        if (EnemiesAlive > 0)
        {
            return;
        }
        ///////// Runs once at start, and then once again after the whole coroutine is finished
        ///////// Since its set to false inside the statement after this one, it will trigger the second time this is run turning the spawner off
        ///////// Turning the spawner off triggers the game to show upgrade screen where it is set to true along with waveReadyToBeSpawned
        ///////if (EnemiesAlive == 0 && spawningOn && !GameManager.waveReadyToBeSpawned)
        ///////{
        ///////    spawningOn = false;
        ///////}
        ///////// waveReadyToBeSpawned param prevents a wave from spawning before the GameManager's Pause method runs, pausing and unpausing the game
        ///////// waveReadyToBeSpawned is set to true once you leave the upgrade screen
        ///////if (EnemiesAlive == 0 && !spawningOn)
        ///////{
        ///////    spawningOn = true;
        ///////}
        ///////if (EnemiesAlive > 0)
        ///////{
        ///////    
        ///////    return;
        ///////}
        if (spawnWave)
        {
            if (!spawnInProcess)
            {
                Spawn();
            
            
            //// Waves are not ready to spawn another wave when a wave is already in progress
            //GameManager.waveReadyToBeSpawned = false;
            
            waveTimer = timerReset;
            }
            return;
        }       
        
    }

    //wave constructor for enemy waves WIP


    public void Spawn()
    {
        allComplete = false;
        spawnInProcess = true;
        currentPath = 0;
        rankCounter = 0;
        rand = Random.Range(1, 51);
        pointsToSpend = GameManager.currentLevel * pointsScaling;
        costsExceedPoints = false;
        enemiesInRankCounter = 0;
        rollCounter = 0;
        nextWave = new Wave();
        nextWave.spawnRate = 2;
        while(pointsToSpend > 0 && !costsExceedPoints && rollCounter < 12)
        {
            costsExceedPoints = true;
            rand = Random.Range(1, 100);
            foreach (GameObject e in enemyTypes)
            {
                enemiesInRankCounter = 0;
                if (e.GetComponent<Enemy>().spawnWeightedValue >= rand && pointsToSpend >= e.GetComponent<Enemy>().spawnCost)
                {
                    rand = Random.Range(1, pointsToSpend / e.GetComponent<Enemy>().spawnCost);
                    enemiesInRankCounter += rand;
                    
                    pointsToSpend -= e.GetComponent<Enemy>().spawnCost * enemiesInRankCounter;
                }
                if (enemiesInRankCounter > 0)
                {
                    rankCounter++;
                    EnemyRank[] oldRanks = nextWave.enemyRanks;
                    nextRank = new EnemyRank(e, enemiesInRankCounter);
                    nextWave.enemyRanks = new EnemyRank[rankCounter];
                    for(int i = 1; i <= rankCounter; i++)
                    {
                        if(i < rankCounter)
                        {
                            nextWave.enemyRanks[i-1] = oldRanks[i-1];
                        }
                        else
                        {
                            nextWave.enemyRanks[i - 1] = nextRank;
                        }

                    }
                    costsExceedPoints = false;
                    break; 
                }                
                rollCounter++;
                if (e.GetComponent<Enemy>().spawnCost < pointsToSpend)
                {
                    costsExceedPoints = false;
                }

            }            
        }
        
        foreach (Spawner s in spawners)
        {
            while (!pathChosen)
            {
                rand = Random.Range(1, 100);
                foreach (GameObject path in s.ownedPathing.editablePathArray)
                {
                    if (path.GetComponent<Path>().pathWeightedValue >= rand)
                    {
                        
                        pathChosen = true;
                        break;
                    }
                    else
                    {
                        currentPath++;
                        
                    }
                }
                if (!pathChosen)
                {
                    currentPath = 0;
                }
            }
            StartCoroutine(s.WaveSpawn(nextWave, currentPath));
            pathChosen = false;
        }
        
        waveSpawned = true;
        waveAlive = true;
        spawnInProcess = false;
        return;
    }



}
