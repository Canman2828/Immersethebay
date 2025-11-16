using UnityEngine;
using UnityEngine.AI;

public class NPCPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    public float waitTimeAtPoint = 0f;

    private int currentIndex = 0;
    private NavMeshAgent agent;
    private float waitTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        // Make sure the agent starts on the NavMesh
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);   // snap onto nearest NavMesh point
            }
            else
            {
                Debug.LogWarning($"{name}: No NavMesh nearby, disabling patrol.");
                enabled = false;
                return;
            }
        }

        if (waypoints != null && waypoints.Length > 0)
        {
            currentIndex = 0;
            agent.SetDestination(waypoints[currentIndex].position);
        }
    }

    void Update()
    {
        // Don’t do anything if we somehow ended up off the NavMesh
        if (!agent.isOnNavMesh || agent.pathPending || waypoints == null || waypoints.Length == 0)
            return;

        // "Close enough" distance so they don't jitter
        float threshold = agent.stoppingDistance + 0.2f;

        if (agent.remainingDistance <= threshold)
        {
            // Optional wait at the waypoint
            if (waitTimeAtPoint > 0f)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer < waitTimeAtPoint)
                    return;

                waitTimer = 0f;
            }

            // ➜ Move to the next waypoint, but DO NOT loop back to 0
            currentIndex++;

            if (currentIndex < waypoints.Length)
            {
                agent.SetDestination(waypoints[currentIndex].position);
            }
            else
            {
                // Reached the final waypoint: stop moving
                agent.isStopped = true;
                enabled = false;   // optional: turn off this script
            }
        }
    }
}
