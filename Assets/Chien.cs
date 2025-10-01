using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chien : MonoBehaviour
{
    List<GameObject> sheepList = new List<GameObject>();
    
    [SerializeField] private float waitingTime;
    private float timer = 0;
    bool isWaiting = false;
    
    NavMeshAgent agent;
    Transform sheepDest;

    private void OnEnable()
    {
        GameManager.instance.SheepEnter += ChangeSheepList;
    }

    private void OnDisable()
    {
        GameManager.instance.SheepEnter -= ChangeSheepList;
    }

    private void Start()
    {
        foreach (Sheep sheep in GameManager.instance.sheepList)
        {
            sheepList.Add(sheep.gameObject);
        }
        
        agent = GetComponent<NavMeshAgent>();
        sheepDest = GameManager.instance.grange.GetSheepDestroyer();
    }

    void Update()
    {
        if (GameData.instance.isSheepInside) 
            return;
        
        PerformSheepManagement();
        
    }

    private void PerformSheepManagement()
    {
        if (agent.pathPending) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            timer += Time.deltaTime;
            if (timer >= waitingTime)
            {
                timer = 0f;
                GoToRandomPatrolPoint();
            }
        }
    }

    void GoToRandomPatrolPoint()
    {
        agent.SetDestination(GetNextDestination());
    }

    void ChangeSheepList(GameObject sheep)
    { 
        sheepList.Remove(sheep);
    }

    Vector3 GetNextDestination()
    {
        float distance = 0;
        Vector3 sheepMoreFar = Vector3.zero;

        foreach (GameObject sheep in sheepList)
        {
            float d = Vector3.Distance(sheep.transform.position, sheepDest.position);

            if (d > distance)
            {
                distance = d;
                sheepMoreFar = sheep.transform.position;
            }
        }

        Vector3 dir = (sheepMoreFar - sheepDest.position).normalized;

        float offset = 1.5f;
        Vector3 nextDestination = sheepMoreFar + dir * offset;

        return nextDestination;
    }

}
