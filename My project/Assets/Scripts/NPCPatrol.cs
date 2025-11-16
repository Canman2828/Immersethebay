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

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (waitTimeAtPoint > 0f)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer < waitTimeAtPoint)
                    return;

                waitTimer = 0f;
            }

            currentIndex = (currentIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentIndex].position);
        }
    }
}
