using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool paused;
    public static int currentLevel;
    public static bool waveReadyToBeSpawned;
    public GameObject pauseScreen;
    public GameObject loseScreen;
    public GameObject upgradeScreen;
    public GameObject[] pumpkinPatch;
    public GameObject[] spawners;
    private float waveEndTime;

    // Start is called before the first frame update
    void Start()
    {
        paused = false;
        currentLevel = 0;
        waveReadyToBeSpawned = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Pause Game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        // Game stuff to happen here
        if (!paused)
        {
            // If pumpkin dead show game over screen
            // if(Input.GetKeyDown(KeyCode.I))
            if (!PumpkinsAlive(pumpkinPatch))
            {
                loseScreen.SetActive(true);
                paused = true;
                Time.timeScale = 0;
            }
            // If all waves are done, pause the game for upgrades(For now 5 seconds to test)
            // Selecting an upgrade also resumes game
            if (EnemyManager.EnemiesAlive == 0 && !Spawner.spawnerOn)
            {
                paused = true;
                waveEndTime = Time.realtimeSinceStartup;
                Time.timeScale = 0;
                upgradeScreen.SetActive(true);
            }
        }

        if (upgradeScreen.activeSelf)
        {
            if (waveEndTime + 5 < Time.realtimeSinceStartup)
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        if (paused)
        {
            if(upgradeScreen.activeSelf == true)
            {
                Spawner.spawnerOn = true;
                waveReadyToBeSpawned = true;
            }
            upgradeScreen.SetActive(false);
            paused = false;
            pauseScreen.SetActive(false);
            Time.timeScale = 1;

        }
        else
        {
            paused = true;
            pauseScreen.SetActive(true);
            Time.timeScale = 0;
        }


    }
    /// <summary>
    /// Checks all the pumpkins to see if alive. Returns false if all dead
    /// </summary>
    /// <param name="pumpkins">An array of all pumpkin objects in the game</param>
    /// <returns></returns>
    public bool PumpkinsAlive(GameObject[] pumpkins)
    {
        int count = 0;
        foreach (GameObject p in pumpkins)
        {
            if (p.GetComponent<PumpkinPatch>().IsAlive == false)
            {
                count++;
            }
        }
        return !(count == pumpkins.Length);
    }

    /// <summary>
    /// Restarts the game
    /// </summary>
    public void RestartGame()
    {
        loseScreen.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    /// <summary>
    /// property for if the game is paused
    /// </summary>
    public bool IsPaused
    {
        get { return paused; }
    }
}
