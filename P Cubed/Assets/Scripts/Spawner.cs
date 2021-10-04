using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private int wavePattern;

    //currently WIP this class will be used to control spawners to allow for waves to come from seperate spawn points with similar or different pathings
    void Start()
    {
        wavePattern = 0;
    }

    void WaveSpawn()
    {
        if(wavePattern < 4)
        {
            wavePattern++;
        }
        else
        {
            wavePattern = 0;
        }        
    }
}
