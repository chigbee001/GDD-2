using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private int pointsToSpend;
    private bool costsExceedPoints;
    public Enemy[] enemyTypes;
    public static int EnemiesAlive;

    private void Start()
    {
        EnemiesAlive = 0;
    }

    //wave constructor for enemy waves WIP


    public Wave CreateWave(int currentLvl, int pumpkinsAlive)
    {
        pointsToSpend = currentLvl * 10;
        costsExceedPoints = false;

        while(pointsToSpend > 0 && !costsExceedPoints)
        {

        }


        Debug.Log("Something went wrong with Manager wave Creation");
        return null;
    }



}
