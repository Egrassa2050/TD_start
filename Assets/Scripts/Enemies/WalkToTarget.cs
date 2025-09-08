using UnityEngine;
using UnityEngine.AI;

public class WalkToTarget : MonoBehaviour
{
    [SerializeField] private string targetTag = "Target";
    [SerializeField] private bool autoAddAgent = true;

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null && autoAddAgent)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.speed = 3.5f;
            agent.angularSpeed = 120f;
            agent.acceleration = 8f;
        }
    }

    void Start()
    {
        if (agent == null) return;

        var target = GameObject.FindGameObjectWithTag(targetTag);
        if (target != null)
        {
            agent.SetDestination(target.transform.position);
        }
    }
}
