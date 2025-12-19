using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chien : MonoBehaviour
{
    [Header("Paramètres")]
    [SerializeField] private float waitingTime = 2f;
    [SerializeField] private float scareRadius = 6f;
    [SerializeField] private float scareForce = 8f;

    private float timer = 0f;
    private NavMeshAgent agent;
    private Transform sheepDest;
    private List<GameObject> sheepList = new List<GameObject>();

    [SerializeField] private ParticleSystem heartParticle;
    [SerializeField] private ParticleSystem barkEffect;

    private bool isMiniGameActive = false;

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

    private void Update()
    {
        if (!isMiniGameActive) return;

        PerformSheepManagement();
        ScareNearbySheep();
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

    private void GoToRandomPatrolPoint()
    {
        agent.SetDestination(GetNextDestination());
    }

    private void ChangeSheepList(GameObject sheep)
    {
        sheepList.Remove(sheep);
    }

    private Vector3 GetNextDestination()
    {
        float maxDistance = 0f;
        Vector3 sheepFarPos = Vector3.zero;

        foreach (GameObject sheep in sheepList)
        {
            if (sheep == null) continue;

            float d = Vector3.Distance(sheep.transform.position, sheepDest.position);
            if (d > maxDistance)
            {
                maxDistance = d;
                sheepFarPos = sheep.transform.position;
            }
        }

        Vector3 dir = (sheepFarPos - sheepDest.position).normalized;
        float offset = 1.2f;
        Vector3 nextDestination = sheepFarPos + dir * offset;

        if (!barkEffect.isPlaying)
            barkEffect.Play();

        return nextDestination;
    }

    private void ScareNearbySheep()
    {
        foreach (GameObject sheep in sheepList)
        {
            if (sheep == null) continue;

            float distance = Vector3.Distance(transform.position, sheep.transform.position);
            if (distance < scareRadius)
            {
                Vector3 fleeDir = (sheep.transform.position - transform.position).normalized;

                SheepBoid boid = sheep.GetComponent<SheepBoid>();
                if (boid != null)
                {
                    boid.AddFearForce(fleeDir * scareForce * (1f - distance / scareRadius));
                }
            }
        }
    }

    // Pour activer/désactiver le comportement de poursuite
    public void SetMiniGameActive(bool value)
    {
        isMiniGameActive = value;
        if (!value)
        {
            agent.ResetPath(); // Stop le chien s'il rentre à la niche
        }
    }

    public void Carresse()
    {
        if (GameManager.instance.currentCameraState == CamState.Dog)
        {
            heartParticle.Play();
        }
    }
}
