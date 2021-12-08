using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileSpawner : MonoBehaviour
{
    public bool inUse;
    public AttackPatternList[] attackPatterns;
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
            rand = Random.Range(0, attackPatterns.Length);
            
            for (int i = 0; i < attackPatterns.Length; i++)
            {
                for (int j = 0; j < attackPatterns[i].arr.Length; j++)
                {
                    attackPatterns[i].arr[j].StopAllCoroutines();
                }
            }

            for (int i = 0; i < attackPatterns[rand].arr.Length; i++)
            {
                StartCoroutine(attackPatterns[rand].arr[i].Fire());
            }
        }
    }
}

[System.Serializable]
public class AttackPatternList
{
    public attackPattern[] arr;
}