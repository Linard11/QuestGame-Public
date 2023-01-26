using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class NavMeshPatrol : MonoBehaviour
{
    private static readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");
    
    #region Inspector

    [SerializeField] private Animator animator;

    [SerializeField] private bool randomOrder;
    
    [SerializeField] private List<Transform> waypoints;

    [SerializeField] private bool waitAtWaypoint = true;

    [Tooltip("Min/Max wait duration at each waypoint in seconds.")]
    [SerializeField] private Vector2 waitDuration = new Vector2(1, 5);

    #endregion

    private NavMeshAgent navMeshAgent;

    private int currentWaypointIndex = -1;

    private bool waiting;

    #region Unity Event Functions

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.autoBraking = waitAtWaypoint;
    }

    private void Start()
    {
        SetNextWaypoint();
    }

    private void Update()
    {
        animator.SetFloat(MovementSpeed, navMeshAgent.velocity.magnitude);
        CheckIfWaypointIsReached();
    }

    #endregion

    #region Navigation

    public void StopPatrolForDialogue()
    {
        StopPatrol();
        DialogueController.DialogueClosed += ResumePatrol;
    }

    public void StopPatrol()
    {
        navMeshAgent.isStopped = true;
    }

    public void ResumePatrol()
    {
        navMeshAgent.isStopped = false;
        // Will do nothing if never subscribed.
        DialogueController.DialogueClosed -= ResumePatrol;
    }

    private void SetNextWaypoint()
    {
        switch (waypoints.Count)
        {
            case 0:
                Debug.LogError("No waypoints set for NavMeshPatrol.", this);
                return;
            case 1:
                if (randomOrder)
                {
                    Debug.LogError("Only one waypoint set for NavMeshPatrol. Need at least 2 with randomOrder enabled.", this);
                    return;
                }
                break;
        }
        
        if (randomOrder)
        {
            int newWaypointIndex;
            do
            {
                newWaypointIndex = Random.Range(0, waypoints.Count);
            }
            while (newWaypointIndex == currentWaypointIndex);
            currentWaypointIndex = newWaypointIndex;
        }
        else
        {
            // Increase waypoint index and loop back around to 0.
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }

        // The navmesh agent automatically tries to reach this destination.
        navMeshAgent.destination = waypoints[currentWaypointIndex].position;
    }

    private void CheckIfWaypointIsReached()
    {
        if (waiting) { return; }
        if (navMeshAgent.pathPending) { return; }

        // Check if navmesh agent has sufficiently reached its destination.
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.01f)
        {
            if (waitAtWaypoint)
            {
                StartCoroutine(WaitBeforeNextWaypoint(Random.Range(waitDuration.x, waitDuration.y)));
            }
            else
            {
                SetNextWaypoint();
            }
        }
    }

    private IEnumerator WaitBeforeNextWaypoint(float duration)
    {
        waiting = true;
        yield return new WaitForSeconds(duration);
        SetNextWaypoint();
        waiting = false;
    }

    #endregion
}
