using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Update variables
    public bool paused;
    public static int currentLevel;
    public GameObject pauseScreen;
    public GameObject loseScreen;
    public GameObject upgradeScreen;
    public GameObject[] pumpkinPatch;
    public GameObject[] spawners;
    public CardManager cardManager;

    private float waveEndTime;
    public Button upgradeButton1;
    public Button upgradeButton2;

    // Upgade variables
    private string[] cardNames;
    private string[] upgradeNames;
    private int rng1stCard;
    private int rng2ndCard;
    private int rngUpgradeType;
    public Transform cardList1;
    public Transform cardList2;

    // Tutorial variables
    private bool tutorialOn = true;
    private int currentTutorialScreenNum = 1;
    public Transform tutorialParent;

    // Start is called before the first frame update
    void Start()
    {
        paused = true;
        cardNames = new string[]{ "Rupture", "Fireball", "Meteor", "Lightning", "Sun Disc"};
        upgradeNames = new string[] { "Mana", "Damage" };
        currentLevel = 1;
        Time.timeScale = 0;
        tutorialParent.gameObject.SetActive(true);
        upgradeButton1.onClick.AddListener(delegate { UpgradeCard(rng1stCard, rngUpgradeType); });
        upgradeButton2.onClick.AddListener(delegate { UpgradeCard(rng2ndCard, rngUpgradeType); });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && tutorialOn)
        {
            ProgressTutorial();
        }

        // Pause Game
        if (Input.GetKeyDown(KeyCode.Escape) && !tutorialOn)
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
            if (EnemyManager.EnemiesAlive == 0 && !EnemyManager.waveAlive)
            {
                paused = true;
                waveEndTime = Time.realtimeSinceStartup;
                Time.timeScale = 0;
                upgradeScreen.SetActive(true);
                rng1stCard = Random.Range(0, 4);
                rng2ndCard = Random.Range(0, 4);
                cardList1.GetChild(rng1stCard).gameObject.SetActive(true);
                Debug.Log(rng1stCard);
                Debug.Log(rng2ndCard);
                cardList2.GetChild(rng2ndCard).gameObject.SetActive(true);
                while (rng1stCard == rng2ndCard)
                {
                    rng2ndCard = Random.Range(0, 4);
                }
                rngUpgradeType = Random.Range(0, 1);
                cardManager.ManaRegenRate += 0.1f;
            }
        }

        if (upgradeScreen.activeSelf)
        {
            if (waveEndTime + EnemyManager.waveTimer < Time.realtimeSinceStartup)
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
                EnemyManager.spawnWave = true;
            }
            upgradeScreen.SetActive(false);
            cardList1.GetChild(rng1stCard).gameObject.SetActive(false);
            cardList2.GetChild(rng2ndCard).gameObject.SetActive(false);
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
    /// Upgrades the card using the method in Card Manager. Runs when button is clicked under card
    /// </summary>
    /// <param name="rng1st">First RNG(will</param>
    /// <param name="rng2nd"></param>
    public void UpgradeCard(int rng1st, int rng2nd)
    {
        cardManager.UpgradeCard(cardNames[rng1st], upgradeNames[rng2nd]);
        Pause();
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

    // Progresses the tutorial
    private void ProgressTutorial()
    {
        if (currentTutorialScreenNum >= 4)
        {
            paused = false;
            tutorialParent.gameObject.SetActive(false);
            Time.timeScale = 1;
            tutorialOn = false;
        }
        else
        {
            currentTutorialScreenNum++;

            tutorialParent.GetChild(currentTutorialScreenNum).gameObject.SetActive(true);

            if (currentTutorialScreenNum > 1)
            {
                tutorialParent.GetChild(currentTutorialScreenNum - 1).gameObject.SetActive(false);
            }
        }
    }
}
