using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathing : MonoBehaviour
{
    //script to handle paths holds waypoint information for each path allowing for easy creation of new paths
    public static Transform[] pathPoints;

    private void Awake()
    {
        pathPoints = new Transform[transform.childCount];
        for (int i = 0; i < pathPoints.Length; i++)
        {
            pathPoints[i] = transform.GetChild(i);
        }
    }
}
