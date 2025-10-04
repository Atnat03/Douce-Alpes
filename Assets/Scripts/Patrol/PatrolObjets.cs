using UnityEngine;
using UnityEngine.AI;

public class PatrolObjets : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Patrol")]
    public float patrolRadius = 5f;
    public float patrolSpeed = 3.5f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    [Header("Flee")]
    public float fleeDistance = 6f;
    public float fleeSpeed = 6f;
    public float fleeTriggerDistance = 2f;
    public float fleeRecalcInterval = 0.35f;

    [Header("Predators")]
    private GameObject predatorLocal;

    private GameObject predatorChien;
    
    private bool isFleeing;
    private float lastFleeTime;

    private float waitTimer = 0f;
    private float waitDuration = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!agent) { enabled = false; return; }

        predatorLocal = TouchManager.instance.sphereSheepLeak;

        predatorChien = GameManager.instance.chien;
        
        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0.2f;

        SetWaitTime();
        GoToRandomPatrolPoint();
    }

     void Update()
    {
        GameObject nearestPredator = GetNearestPredator();

        if (nearestPredator != null &&
            Vector3.Distance(transform.position, nearestPredator.transform.position) < fleeTriggerDistance)
        {
            Flee(nearestPredator.transform.position);
        }

        if (isFleeing)
        {
            CheckFlee();
            return;
        }

        Patrol();
    }

    void Patrol()
    {
        if (agent.pathPending) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitDuration)
            {
                waitTimer = 0f;
                SetWaitTime();
                GoToRandomPatrolPoint();
            }
        }
    }

    public void StopAgent(bool state)
    {
        agent.isStopped = state;
    }

    void GoToRandomPatrolPoint()
    {
        Vector3 randomOffset = Random.insideUnitSphere * patrolRadius;
        randomOffset.y = 0;
        Vector3 target = transform.position + randomOffset;

        agent.speed = patrolSpeed;
        agent.SetDestination(target);
    }

    void SetWaitTime()
    {
        waitDuration = Random.Range(minWaitTime, maxWaitTime);
    }

    void Flee(Vector3 predatorPos)
    {
        if (Time.time - lastFleeTime < fleeRecalcInterval) return;
        lastFleeTime = Time.time;

        Vector3 dir = (transform.position - predatorPos).normalized;
        if (dir.sqrMagnitude < 0.001f) dir = Random.insideUnitSphere.normalized;

        Vector3 target = transform.position + dir * fleeDistance;

        agent.speed = fleeSpeed;
        agent.SetDestination(target);
        isFleeing = true;
    }

    void CheckFlee()
    {
        if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance) return;

        GameObject nearestPredator = GetNearestPredator();

        if (nearestPredator != null &&
            Vector3.Distance(transform.position, nearestPredator.transform.position) < fleeTriggerDistance)
        {
            Flee(nearestPredator.transform.position);
        }
        else
        {
            isFleeing = false;
            SetWaitTime();
            GoToRandomPatrolPoint();
        }
    }

    GameObject GetNearestPredator()
    {
        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        if (predatorLocal != null)
        {
            float dist = Vector3.Distance(transform.position, predatorLocal.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = predatorLocal;
            }
        }

        if (predatorChien != null)
        {
            float dist = Vector3.Distance(transform.position, predatorChien.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = predatorChien;
            }
        }

        return nearest;
    }
}
