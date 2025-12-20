using System;
using System.Collections.Generic;
using UnityEngine;

public class SheepFearZone : MonoBehaviour
{
    [Header("Paramètres de peur")]
    [SerializeField] private float fearRadius = 8f;  
    [SerializeField] private float fearForce = 10f;   
    [SerializeField] private bool showDebug = true;

    private List<SheepBoid> sheepList = new List<SheepBoid>();
    
    public GameObject particle;

    private void Update()
    {
        if (!GameData.instance.isSheepInside)
            return;
        
        ScareNearbySheep();

        particle.SetActive(GameManager.instance.grange.AllSheepAreOutside);
    }

    private void ScareNearbySheep()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, fearRadius, LayerMask.GetMask("Mouton"));
        foreach (Collider hit in hits)
        {
            SheepBoid boid = hit.GetComponent<SheepBoid>();
            if (boid == null) continue;

            Debug.Log($"Sheep {boid.name} effrayé !");
        
            Vector3 fleeDir = (boid.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, boid.transform.position);
            float intensity = Mathf.Lerp(fearForce, 0, distance / fearRadius);

            boid.AddFearForce(fleeDir * intensity);

            if (showDebug)
                Debug.DrawLine(transform.position, boid.transform.position, Color.red);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fearRadius);
    }
}