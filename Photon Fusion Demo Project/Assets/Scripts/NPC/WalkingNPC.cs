using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class WalkingNPC : NetworkBehaviour
{
    public ObstacleAvoidanceType obstacleAvoidanceType;
    public bool isTeleport;
    
    public List<Transform> wayPoints;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator anim;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float turnSpeed = .5f;
    [SerializeField] private float stoppingDistance = .5f;
    [SerializeField] private float angularAvoidanceSpeed = 120f;
    private int currentWayPoint = 0;

    private void Start()
    {
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.angularSpeed = angularAvoidanceSpeed;
        navMeshAgent.obstacleAvoidanceType = obstacleAvoidanceType;
        GoToNextWayPoint();
    }

    private void Update()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < stoppingDistance)
            GoToNextWayPoint();

        Vector3 targetDirection = navMeshAgent.steeringTarget - transform.position;
        float singleStep = turnSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        
        if (isTeleport)
            Invoke("ResetTeleportState", 3);
    }

    void GoToNextWayPoint()
    {
        if (wayPoints.Count == 0)
            return;

        navMeshAgent.stoppingDistance = stoppingDistance;
        navMeshAgent.SetDestination(wayPoints[currentWayPoint].position);
        currentWayPoint = (currentWayPoint + 1) % wayPoints.Count;
        anim.SetBool("IsWalking", true);
    }
    
    void ResetTeleportState()
    {
        isTeleport = false;
    }
}
