using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool paused;
    public GameObject pauseScreen;
    public GameObject loseScreen;
    public GameObject coreScreen;
    public GameObject player;
    public GameObject boss;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        coreScreen.SetActive(true);
        boss.SetActive(false);
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
    /// Sets the core and unpauses the game
    /// </summary>
    public void SetCore(string coreString)
    {
        PlayerCore core = (PlayerCore)System.Enum.Parse(typeof(PlayerCore), coreString);
        player.GetComponent<player>().SetCore(core);
        Debug.Log(player.GetComponent<player>().ShootType);
        Time.timeScale = 1;
        coreScreen.SetActive(false);
        boss.SetActive(true);
    }

    public void Tooltip(GameObject button)
    {
        if (!button.transform.GetChild(1).gameObject.activeInHierarchy)
        {
            button.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            button.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Returns if game is paused
    /// </summary>
    public bool IsPaused
    {
        get { return paused; }
    }
}
