using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool paused;

    public GameObject pauseScreen;



    // Start is called before the first frame update
    void Start()
    {
        paused = false;
    }

    // Update is called once per frame
    void Update()
    {

        // Pause Game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        // Game stuff happens here
        if (!paused)
        {

        }
    }

    public void Pause()
    {
        if (paused)
        {
            paused = false;
            pauseScreen.SetActive(false);

        }
        else
        {
            paused = true;
            pauseScreen.SetActive(true);
        }


    }
}
