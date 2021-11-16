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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !loseScreen.activeInHierarchy)
        {
            Pause();
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
        // Need multiple scenes b4 I write this
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
    /// <summary>
    /// Returns if game is paused
    /// </summary>
    public bool IsPaused
    {
        get { return paused; }
    }
}
