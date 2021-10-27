using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pathing : MonoBehaviour
{
    //script to handle paths holds waypoint information for each path allowing for easy creation of new paths
    public static Transform[] pathPoints;
    public static GameObject currentPath;
    public GameObject[] editablePathArray;
    public static GameObject[] paths;
    public static int pathCount;
    public Transform spawnPoint;
    private static Transform myNewTransform;

    /// <summary>
    /// sets path count, copys the editable array into the static array and places paths in proper container
    /// </summary>
    private void Awake()
    {
        pathCount = 0;
        paths = editablePathArray;
        foreach (GameObject g in paths)
        {
            GameObject thisPath = Instantiate(g, spawnPoint);
            thisPath.transform.parent = transform;
            pathCount++;
        }
        myNewTransform = transform;
    }

    /// <summary>
    /// Takes in an integer checks it is within range and sets path to that path number
    /// If number is outside of range sets path to the first path in the array
    /// </summary>
    /// <param name="newPath"></param>
    public static void SetPath(int newPath)
    {
        if (newPath < pathCount)
        {
            currentPath = paths[newPath];
            SetPathPoints(myNewTransform, newPath);
        }
        else
        {

            currentPath = paths[0];
            SetPathPoints(myNewTransform, 0);
        }
       
        
    }

    /// <summary>
    /// Called by set path in order set path points of new path into pathpoint array
    /// </summary>
    /// <param name="myTransform"></param>
    /// <param name="newPath"></param>
    private static void SetPathPoints(Transform myTransform, int newPath)
    {
        Debug.Log("SetPathPoints Called");
        pathPoints = new Transform[currentPath.transform.childCount];
        for (int i = 0; i < pathPoints.Length; i++)
        {
            pathPoints[i] = myTransform.GetChild(newPath).GetChild(i);
        }
    }

}
