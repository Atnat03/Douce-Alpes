using System;
using System.Collections.Generic;
using UnityEngine;

public class SheepFearZone : MonoBehaviour
{
    [Header("Paramètres de peur")]
    [SerializeField] private float fearRadius = 8f;   // rayon d’effet
    [SerializeField] private float fearForce = 10f;   // intensité de la peur
    [SerializeField] private bool showDebug = true;

    private List<SheepBoid> sheepList = new List<SheepBoid>();

    private void OnEnable()
    {
        SheepBoidManager.OnListChanged += UpdateList;
    }
    
    private void OnDisable()
    {
        SheepBoidManager.OnListChanged -= UpdateList;
    }

    void UpdateList(SheepBoid boid)
    {
        sheepList.Add(boid);
    }

    private void Update()
    {
        ScareNearbySheep();
    }

    private void ScareNearbySheep()
    {
        foreach (SheepBoid boid in sheepList)
        {
            if (boid == null) continue;

            float distance = Vector3.Distance(transform.position, boid.transform.position);

            if (distance < fearRadius)
            {
                Debug.Log($"Sheep {boid.name} effrayé !");
                
                Vector3 fleeDir = (boid.transform.position - transform.position).normalized;

                float intensity = Mathf.Lerp(fearForce, 0, distance / fearRadius);

                boid.AddFearForce(fleeDir * intensity);

                if (showDebug)
                    Debug.DrawLine(transform.position, boid.transform.position, Color.red);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fearRadius);
    }
}