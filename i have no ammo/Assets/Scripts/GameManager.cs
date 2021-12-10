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
    public GameObject winScreen;
    public GameObject player;
    public baseBoss boss;

    //tutorial variables
    public Transform tutorialParent;
    private int currentTutorialScreen;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        coreScreen.SetActive(true);
        boss.gameObject.SetActive(false);
        currentTutorialScreen = 0;
        paused = true;
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
            paused = true;
            loseScreen.SetActive(true);
        }

        if (currentTutorialScreen < tutorialParent.childCount && Input.anyKeyDown)
        {
            ProgressTutorial();
        }
        
        if(boss.currentHealth <= 0)
        {
            paused = true;
            Time.timeScale = 0;
            winScreen.SetActive(true);
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
        boss.gameObject.SetActive(true);
        paused = false;
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

    //progress tutorial to next tutorial screen
    private void ProgressTutorial()
    {
        if (currentTutorialScreen >= tutorialParent.childCount - 1)
        {
            tutorialParent.gameObject.SetActive(false);
        }
        else
        {
            currentTutorialScreen++;

            tutorialParent.GetChild(currentTutorialScreen).gameObject.SetActive(true);
            tutorialParent.GetChild(currentTutorialScreen - 1).gameObject.SetActive(false);
        }
    }
}
