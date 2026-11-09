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


    public float destArrivedDist = 3f;
    private const float maxTimeoutTime = 30f;
    private const float navigationCheckInterval = 0.1f;
    private Transform currentTarget;
    private Vector3 pointOffset;
    private NavMeshAgent agent;
    private Transform character;
    private Animator animator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        if(!agent) {
            throw new MissingComponentException("Missing NavMeshAgent");
        }

        if(interestPoints.Count <= 0) {
            throw new NoNullAllowedException("Missing value for points of interest");
        }
        
        character = transform.Find("Character");
        animator = character.GetComponent<Animator>();
        
        StartCoroutine(ChooseNewPOI());
        StartCoroutine(WanderFromPath());
        StartCoroutine(CheckForPickup());
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("moving",  agent.velocity != Vector3.zero);
        animator.SetFloat("speed", Vector3.Dot(transform.forward, agent.velocity));
        animator.SetFloat("Strafe", Vector3.Dot(transform.right, agent.velocity));
    }

    IEnumerator ChooseNewPOI() {
        int pointIndex = UnityEngine.Random.Range(0, interestPoints.Count);
        Vector2 randPoint = ((maxWanderDist/2) * Random.insideUnitCircle);
        Vector3 interestPoint = interestPoints[pointIndex].position;
        interestPoint.x += randPoint.x;
        interestPoint.z += randPoint.y;
        if(Physics.Raycast(interestPoint + (5 * Vector3.up), Vector3.down, out RaycastHit hit, 10)) {
            pointOffset = hit.point - interestPoints[pointIndex].position;
        } else {
            pointOffset = Vector3.zero;
            pointOffset.x += randPoint.x;
            pointOffset.z += randPoint.y;
        }
        // print("moving to " + (interestPoints[pointIndex].position + pointOffset));
        StartCoroutine(NavigateTowardsPOI(interestPoints[pointIndex].position + pointOffset));
        yield break;
    }
    
    IEnumerator NavigateTowardsPOI(Vector3 POI) {
        float watcherTimer = maxTimeoutTime;
        agent.destination = POI;
        while(agent.remainingDistance >= destArrivedDist && watcherTimer > 0) {
            // print("remaining dist: " + agent.remainingDistance + " time: " + watcherTimer);
            
            watcherTimer -= navigationCheckInterval;
            yield return new WaitForSeconds(navigationCheckInterval);
        }

        agent.isStopped = true;

        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        agent.isStopped = false;
        StartCoroutine(ChooseNewPOI());
    }

    IEnumerator WanderFromPath() {
        while(true) {
            yield return new WaitForSeconds(Random.Range(minWanderCooldown, maxWanderCooldown));
            Vector3 prevDest = agent.destination;
            Vector2 wanderOffset = maxWanderDist * Random.insideUnitCircle;
            agent.destination = agent.transform.position + new Vector3(wanderOffset.x, 0, wanderOffset.y);
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            agent.destination = prevDest;
        }
    }

    IEnumerator CheckForPickup() {
        while(true) {
            if(agent.remainingDistance <= destArrivedDist) {
                animator.SetTrigger("pickup");
            }

            yield return new WaitForSeconds(3f);
        }
    }
}
