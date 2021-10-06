
using UnityEngine;

[System.Serializable]

public class Wave
{
    private int enemyTypesNumber = 0;
    public GameObject[] enemyTypes;
    public int[] enemyNumbers;
    public float spawnRate;

    void Awake()
    {
        foreach(GameObject g in enemyTypes)
        {
            enemyTypesNumber++;
        }
    }

   

}
