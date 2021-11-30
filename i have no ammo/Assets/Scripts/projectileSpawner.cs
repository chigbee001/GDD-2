using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileSpawner : MonoBehaviour
{
    public bool inUse;
    public attackPattern[] attackPatterns;


    private void Start()
    {
        inUse = false;
    }

    void Update()
    {
        if(!inUse)
        {
            //pick random attack pattern and use it
        }

    }

}
