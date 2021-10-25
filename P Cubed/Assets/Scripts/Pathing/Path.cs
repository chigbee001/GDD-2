using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//class to hold values involving individual paths and their behavior
public class Path : MonoBehaviour
{
    public int pathCost;
    public int pathWeightedValue;

    private void Start()
    {
        pathCost = Mathf.Clamp(pathCost, 1, 50);
    }

}
