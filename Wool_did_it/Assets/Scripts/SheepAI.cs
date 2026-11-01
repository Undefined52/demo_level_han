using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.AI;

public class SheepAI : MonoBehaviour {
    [Header("List of Points of interest that the AI Sheep will navigate towards")]
    public List<Transform> interestPoints;

    [Header("Range of wait time at points of interest")]
    public float minWaitTime;
    public float maxWaitTime;

    [Header("Variables dictating the wandering of the sheep")]
    public float minWanderCooldown;
    public float maxWanderCooldown;
    public float maxWanderDist;


    private const float destArrivedDist = 3f;
    private const float maxTimeoutTime = 30f;
    private const float navigationCheckInterval = 0.1f;
    private Transform currentTarget;
    private Vector3 pointOffset;
    private NavMeshAgent agent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        if(!agent) {
            throw new MissingComponentException("Missing NavMeshAgent");
        }

        if(interestPoints.Count <= 0) {
            throw new NoNullAllowedException("Missing value for points of interest");
        }
        
        StartCoroutine(ChooseNewPOI());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ChooseNewPOI() {
        int pointIndex = UnityEngine.Random.Range(0, interestPoints.Count);
        print("moving to " + interestPoints[pointIndex].position);
        StartCoroutine(NavigateTowardsPOI(interestPoints[pointIndex].position + pointOffset));
        yield break;
    }
    
    IEnumerator NavigateTowardsPOI(Vector3 POI) {
        float watcherTimer = maxTimeoutTime;
        float wanderWatcher = maxTimeoutTime / 4;
        agent.destination = POI;
        while(agent.remainingDistance >= destArrivedDist && watcherTimer > 0) {
            print("remaining dist: " + agent.remainingDistance + " time: " + watcherTimer);
            
            watcherTimer -= navigationCheckInterval;
            yield return new WaitForSeconds(navigationCheckInterval);
        }

        agent.isStopped = true;

        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        agent.isStopped = false;
        StartCoroutine(ChooseNewPOI());
    }
}
