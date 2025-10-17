using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PatrolObjets : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Patrol")]
    public float patrolRadius = 5f;
    public float patrolSpeed = 3.5f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    [Header("Drink Behavior")]
    public float minCheckDrinkTime = 5f;
    public float maxCheckDrinkTime = 15f;
    public float drinkDuration = 3f;

    [Header("Flee")]
    public float fleeDistance = 6f;
    public float fleeSpeed = 6f;
    public float fleeTriggerDistance = 2f;
    public float fleeRecalcInterval = 0.35f;

    private bool isFleeing;
    private float lastFleeTime;
    private GameObject predatorLocal;
    private GameObject predatorChien;

    private float waitTimer;
    private float waitDuration;
    private bool isDrinking = false;
    private Transform currentDrinkPlace;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!agent) { enabled = false; return; }

        predatorLocal = TouchManager.instance.sphereSheepLeak;
        predatorChien = GameManager.instance.chien;

        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0.2f;

        SetWaitTime();
        GoToRandomPatrolPoint();

        // Lance la vérification aléatoire périodique de l'abreuvoir
        StartCoroutine(CheckAbreuvoirRoutine());
    }

    private void Update()
    {
        if (isDrinking) return;

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

    private IEnumerator CheckAbreuvoirRoutine()
    {
        while (true)
        {
            float wait = Random.Range(minCheckDrinkTime, maxCheckDrinkTime);
            yield return new WaitForSeconds(wait);

            if (isFleeing || isDrinking) continue;

            if (Abreuvoir.instance.TryReservePlace(out Transform drinkPlace))
            {
                isDrinking = true;
                currentDrinkPlace = drinkPlace;
                agent.speed = patrolSpeed;
                Vector3 pos = new Vector3(drinkPlace.position.x, 0, drinkPlace.position.z);
                agent.SetDestination(pos);
            }
        }
    }

    private void Patrol()
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

    private void SetWaitTime() => waitDuration = Random.Range(minWaitTime, maxWaitTime);

    private void GoToRandomPatrolPoint()
    {
        Vector3 randomOffset = Random.insideUnitSphere * patrolRadius;
        randomOffset.y = 0;
        Vector3 target = transform.position + randomOffset;
        agent.speed = patrolSpeed;
        agent.SetDestination(target);
    }

    private void Flee(Vector3 predatorPos)
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

    private void CheckFlee()
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

    private GameObject GetNearestPredator()
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }

    private void LateUpdate()
    {
        if (isDrinking && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(DrinkRoutine());
        }
    }

    public void StopAgent(bool state)
    {
        agent.velocity = Vector3.zero;
        agent.enabled = !state;
        agent.isStopped = state;
    }

    private IEnumerator DrinkRoutine()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(drinkDuration);

        // Libère la place
        Abreuvoir.instance.FreePlace(currentDrinkPlace);
        currentDrinkPlace = null;
        isDrinking = false;
        agent.isStopped = false;

        SetWaitTime();
        GoToRandomPatrolPoint();
    }
}
