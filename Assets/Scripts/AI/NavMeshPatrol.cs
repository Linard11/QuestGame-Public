using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;

public class NavMeshPatrol : MonoBehaviour
{
    private static readonly int MovementSpeedId = Animator.StringToHash("MovementSpeed");

    #region Inspector

    [SerializeField] private Animator animator;

    [Header("Waypoints")]

    [Tooltip("The next waypoint is chosen at random.")]
    [SerializeField] private bool randomOrder;

    [Tooltip("List of waypoints for the NavMeshAgent to walk to. Make sure to put at least two waypoints into this list!")]
    [SerializeField] private List<Transform> waypoints;

    [Tooltip("Wait a certain amount of time when reaching the waypoint.")]
    [SerializeField] private bool waitAtWaypoint = true;

    [Tooltip("Min/Max wait duration at each waypoint in seconds. WaitAtWaypoint needs to be enabled.")]
    [SerializeField] private Vector2 waitDuration = new Vector2(1, 5);

    [Header("Gizmos")]

    [SerializeField] private bool showWaypoints = true;

    #endregion

    private NavMeshAgent navMeshAgent;

    private int currentWaypointIndex = -1; // Is -1 on start so it is incremented to 0.

    private bool isWaiting;

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
        animator.SetFloat(MovementSpeedId, navMeshAgent.velocity.magnitude);
        if (!navMeshAgent.isStopped)
        {
            CheckIfWaypointIsReached();
        }
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
        DialogueController.DialogueClosed -= ResumePatrol;
    }

    private void SetNextWaypoint()
    {
        switch (waypoints.Count)
        {
            case 0:
                Debug.LogError("No waypoints set for NavMeshPatrol", this);
                return;
            case 1:
                if (randomOrder)
                {
                    Debug.LogError("Only one waypoint set for NavMeshPatrol. Need at least 2 with randomOrder enabled", this);
                    return;
                }
                else
                {
                    Debug.LogWarning("Only one waypoint set for NavMeshPatrol.", this);
                    return;
                }
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
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }

        navMeshAgent.destination = waypoints[currentWaypointIndex].position;
    }

    private void CheckIfWaypointIsReached()
    {
        if (isWaiting) { return; }

        if (navMeshAgent.pathPending) { return; }

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
        isWaiting = true;
        yield return new WaitForSeconds(duration);
        isWaiting = false;
        SetNextWaypoint();
    }

    #endregion

    private void OnDrawGizmos()
    {
        if (!showWaypoints) { return; }

        for (int i = 0; i < waypoints.Count; i++)
        {
            Transform waypoint = waypoints[i];

            Gizmos.color = currentWaypointIndex == i ? Color.green : Color.yellow;
            Gizmos.DrawSphere(waypoint.position, 0.3f);

            if (!randomOrder)
            {
                Gizmos.DrawLine(i == 0 ? waypoints[^1].position : waypoints[i - 1].position,
                                waypoints[i].position);
            }
        }
    }
}
