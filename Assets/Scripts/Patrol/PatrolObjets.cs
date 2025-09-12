using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PatrolObjets :  MonoBehaviour
{
    private int currentPatrolPointIndex;

    private NavMeshAgent agent;

    private float currentWaintingTime;
    float maxWaintingTime;
    public float maxValueTime = 8;
    public float minValueTime = 3;

    public float raduisCercle = 5;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        currentPatrolPointIndex = -1;

        currentWaintingTime = 0;
        maxWaintingTime = 0;
    }

    void Update()
    {
        if (agent.remainingDistance < 0.5f)
        {
            if (maxWaintingTime == 0)
            {
                maxWaintingTime = Random.Range(minValueTime, maxValueTime);
            }
            else
            {
                //WaitingAction();
            }

            if (currentWaintingTime >= maxWaintingTime)
            {
                maxWaintingTime = 0;
                currentWaintingTime = 0;
                GoNextPoint();
            }
            else
            {
                currentWaintingTime += Time.deltaTime;
            }
        }
    }

    public virtual void WaitingAction()
    {
        Debug.Log($"{transform.name} attend");
    }

    public void StopAgent(bool state)
    {
        agent.SetDestination(transform.position);
        agent.isStopped = state;
    }
    
    void GoNextPoint()
    {
        Vector3 randomPos = Random.insideUnitSphere * raduisCercle;
        Vector3 newPos = new Vector3(transform.position.x + randomPos.x, transform.position.y, transform.position.z + randomPos.z);
        agent.SetDestination(newPos);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raduisCercle);
    }
}
