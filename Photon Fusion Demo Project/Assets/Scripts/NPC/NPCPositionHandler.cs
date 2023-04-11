using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCPositionHandler : NetworkBehaviour
{
    [SerializeField] private Transform[] checkPoints;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NPC")
        {
            WalkingNPC walkingNpc = other.GetComponent<WalkingNPC>();
            
            if (!walkingNpc.isTeleport)
            {
                other.GetComponent<WalkingNPC>().isTeleport = true;

                int checkPointIndex = Random.Range(0, checkPoints.Length);
                
                other.transform.position = checkPoints[checkPointIndex].position;
                walkingNpc.wayPoints.Clear();

                for (int i = 0; i < checkPoints[checkPointIndex].GetComponent<NPCWaypoint>().wayPoints.Length; i++)
                {
                    walkingNpc.wayPoints.Add(checkPoints[checkPointIndex].GetComponent<NPCWaypoint>().wayPoints[i]);
                }
            }
        }
    }
}
