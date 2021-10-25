using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private int pointsToSpend;
    private bool costsExceedPoints;
    public GameObject[] enemyTypes;
    public static int EnemiesAlive;
    public Spawner[] spawners;
    public static float waveTimer;
    public float timerReset;
    public static bool waveAlive;
    public static bool spawnWave;

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
        spawnInProcess = true;
        currentPath = 0;
        rankCounter = 0;
        rand = Random.Range(1, 51);
        pointsToSpend = GameManager.currentLevel * 10;
        costsExceedPoints = false;
        enemiesInRankCounter = 0;
        rollCounter = 0;
        nextWave = new Wave();
        nextWave.spawnRate = 2;
        while(pointsToSpend > 0 && !costsExceedPoints && rollCounter < 10)
        {
            costsExceedPoints = true;
            foreach(GameObject e in enemyTypes)
            {
                enemiesInRankCounter = 0;
                while (e.GetComponent<Enemy>().spawnWeightedValue >= rand && pointsToSpend >= e.GetComponent<Enemy>().spawnCost)
                {
                    enemiesInRankCounter++;
                    rand = Random.Range(1, 51);
                    pointsToSpend -= e.GetComponent<Enemy>().spawnCost;
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

                        //Add(nextRank);
                }
                rand = Random.Range(1, 51);
                rollCounter++;
                if (e.GetComponent<Enemy>().spawnCost < pointsToSpend)
                {
                    costsExceedPoints = false;
                }

            }            
        }
        while (!pathChosen)
        {
            foreach (GameObject path in Pathing.paths)
            {
                if (path.GetComponent<Path>().pathWeightedValue >= rand)
                {
                    rand = Random.Range(1, 51);
                    pathChosen = true;                    
                    break;
                }
                else
                {
                    currentPath++;
                    rand = Random.Range(1, 100);
                }
            }
            if (!pathChosen)
            {
                currentPath = 0;
            }
        }
        foreach (Spawner s in spawners)
        {
            StartCoroutine(s.WaveSpawn(nextWave, currentPath));
        }
        
        waveSpawned = true;
        waveAlive = true;
        spawnInProcess = false;
        return;
    }



}
