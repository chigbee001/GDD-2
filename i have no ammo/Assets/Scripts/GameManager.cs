using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool paused;
    public GameObject pauseScreen;
    public GameObject loseScreen;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !loseScreen.activeInHierarchy)
        {
            Pause();
        }

        if (!player.GetComponent<player>().IsAlive)
        {
            Time.timeScale = 0;
            loseScreen.SetActive(true);
        }
    }

    /// <summary>
    /// Pause the game
    /// </summary>
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
    /// 
    /// </summary>
    public void RestartGame()
    {

        loseScreen.SetActive(false);
        Time.timeScale = 1;
        // Need multiple scenes b4 I change this
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
    /// <summary>
    /// Returns if game is paused
    /// </summary>
    public bool IsPaused
    {
        get { return paused; }
    }
}
