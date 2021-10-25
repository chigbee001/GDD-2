using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyRank
{
    public GameObject enemyType;
    public int enemyNumbers;  
    

    public EnemyRank(GameObject eType, int eneNumbers)
    {
        this.enemyType = eType;
        this.enemyNumbers = eneNumbers;
    }
}
