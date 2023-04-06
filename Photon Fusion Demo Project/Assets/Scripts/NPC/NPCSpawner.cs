using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NPCSpawner : NetworkBehaviour
{
    [SerializeField] private WalkingNPC[] NPCPrefab;
    [SerializeField] private Transform[] targetWaypoints;
    [SerializeField] private float spawnTime = 5;
    [SerializeField] private float currentSpawnTime;

    private void Start()
    {
        // StartCoroutine(SpawnNPC());
        currentSpawnTime = spawnTime;
    }

    public override void FixedUpdateNetwork()
    {
        currentSpawnTime -= Runner.DeltaTime;
        
        if (currentSpawnTime <= 0)
        {
            WalkingNPC NPC = Runner.Spawn(NPCPrefab[Random.Range(0, NPCPrefab.Length)], transform.position, transform.rotation, Object.InputAuthority);

            for (int i = 0; i < targetWaypoints.Length; i++)
            {
                NPC.wayPoints.Add(targetWaypoints[i]);
            }
            
            currentSpawnTime = spawnTime;
        }
    }
}
