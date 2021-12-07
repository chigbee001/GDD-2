using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileSpawner : MonoBehaviour
{
    public bool inUse;
    public attackPattern[] attackPatterns;
    public bool turnedOn;

    private void Start()
    {
        inUse = false;
        turnedOn = false;
    }

    void Update()
    {
        
        if (!inUse && turnedOn)
        {            
            int rand;
            //pick random attack pattern and use it
            inUse = true;
            rand = Random.Range(0, attackPatterns.Length - 1);
            StartCoroutine(attackPatterns[0].Fire());
            
        }

    }

}
