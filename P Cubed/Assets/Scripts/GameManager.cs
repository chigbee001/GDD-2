using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool paused;
    public static int currentLevel;
    public GameObject pauseScreen;
    public GameObject loseScreen;
    public GameObject[] pumpkinPatch;

    // Start is called before the first frame update
    void Start()
    {
        paused = false;
        currentLevel = 0;
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

        }

        // If pumpkin dead show game over screen
        // if(Input.GetKeyDown(KeyCode.I))
        if (!paused && !PumpkinsAlive(pumpkinPatch))
        {
            loseScreen.SetActive(true);
            paused = true;
            Time.timeScale = 0;
            Debug.Log("HERE");
        }
    }

    public void Pause()
    {
        if (paused)
        {
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
