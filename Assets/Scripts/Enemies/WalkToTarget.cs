using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkToTarget : MonoBehaviour
{
    [SerializeField] private bool autoAddNavMeshAgent = true;
    [SerializeField] private bool useGlobalFullPathway = true;
    [SerializeField] private List<GameObject> targetsOverride;
    [SerializeField] private bool autoAdvanceOnReach = false;
    [SerializeField] private float reachThreshold = 0.15f;
    [SerializeField] private int priority = 50;

    private List<GameObject> targets;
    private NavMeshAgent agent;
    private int currentIndex;

    public event Action<GameObject> OnTargetReached;

    private IEnumerator Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null && autoAddNavMeshAgent)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.speed = 3.5f;
            agent.angularSpeed = 120f;
            agent.acceleration = 8f;
            agent.stoppingDistance = 0.05f;
            agent.autoBraking = true;
        }

        if (agent == null)
        {
            enabled = false;
            Debug.LogError($"{name}: NavMeshAgent відсутній і autoAddNavMeshAgent вимкнено.");
            yield break;
        }

        agent.avoidancePriority = Mathf.Clamp(priority, 0, 99);

        if (useGlobalFullPathway)
        {
            while (FullPathway.Pathway == null || FullPathway.Pathway.Count == 0)
                yield return null;
            targets = FullPathway.Pathway;
        }
        else
        {
            targets = targetsOverride;
        }

        if (targets == null || targets.Count == 0)
        {
            enabled = false;
            Debug.LogWarning($"{name}: Список цілей порожній.");
            yield break;
        }

        currentIndex = Mathf.Clamp(currentIndex, 0, targets.Count - 1);
        SetDestinationToCurrent();
    }

    private void Update()
    {
        if (agent == null || targets == null || targets.Count == 0) return;
        if (agent.pathPending) return;

        float stopDist = Mathf.Max(agent.stoppingDistance, reachThreshold);
        if (agent.remainingDistance <= stopDist && agent.velocity.sqrMagnitude <= 0.0001f)
        {
            OnTargetReached?.Invoke(targets[currentIndex]);
            if (autoAdvanceOnReach && currentIndex < targets.Count - 1)
                GoToNextTarget();
            else
                agent.isStopped = true;
        }
    }

    public void GoToNextTarget()
    {
        if (agent == null || targets == null || targets.Count == 0) return;
        if (currentIndex < targets.Count - 1)
        {
            currentIndex++;
            agent.isStopped = false;
            SetDestinationToCurrent();
        }
    }

    public void ResetPath(List<GameObject> newTargets, bool startFromFirst = true)
    {
        targets = newTargets;
        if (targets == null || targets.Count == 0) return;
        currentIndex = startFromFirst ? 0 : Mathf.Min(currentIndex, targets.Count - 1);
        if (agent != null)
        {
            agent.isStopped = false;
            SetDestinationToCurrent();
        }
    }

    public void SetPriority(int newPriority)
    {
        priority = Mathf.Clamp(newPriority, 0, 99);
        if (agent != null) agent.avoidancePriority = priority;
    }

    private void SetDestinationToCurrent()
    {
        var t = targets[currentIndex];
        if (t == null) return;
        agent.SetDestination(t.transform.position);
    }
}
