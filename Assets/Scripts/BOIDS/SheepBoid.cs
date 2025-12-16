using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SheepBoid : MonoBehaviour
{
    [HideInInspector] public SheepBoidManager manager;
    [SerializeField] public NatureType natureType;
    public INatureStrategy natureStrategy;

    public Vector3 velocity;
    private bool isPaused;
    private float pauseTimer, nextPauseTime;

    public bool isAfraid = false;
    [SerializeField] private float fearSpeedMultiplier = 3f;
    private float fearDuration = 1f; 
    private float fearTimer;

    public NatureType natureBase;

    public GameObject particleRun;

    private Color[] natureColors = new Color[]
    {
        Color.red, 
        Color.blue,  
        Color.gray,  
        Color.yellow  
    };
    void Start()
    {
        natureStrategy = NatureFactory.Create(natureType);
        ScheduleNextPause();

        if (velocity == Vector3.zero)
        {
            velocity = Random.insideUnitSphere;
            velocity.y = 0f;
            velocity = velocity.normalized * Random.Range(manager.minSpeed, manager.maxSpeed);
        }
    }

    void Update()
    {
        if (isPaused && !isAfraid)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                ScheduleNextPause();
            }
            return;
        }
        
        particleRun.SetActive(isAfraid);

        Vector3 accel = Vector3.zero;

        accel += BoundaryRepulsion();
        accel += ColliderRepulsion();

        velocity += accel * Time.deltaTime;
        velocity.y = 0f;

        float speed = Mathf.Clamp(velocity.magnitude, manager.minSpeed, manager.maxSpeed);

        if (isAfraid)
        {
            speed *= fearSpeedMultiplier;
            fearTimer -= Time.deltaTime;
            if (fearTimer <= 0f)
                CalmDown();
        }

        velocity = velocity.normalized * speed;

        // Déplacement
        transform.position += velocity * Time.deltaTime;

        // Rotation du mouton vers la direction du mouvement
        if (velocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 0.1f);
        }

        // Gestion de la pause naturelle
        nextPauseTime -= Time.deltaTime;
        if (nextPauseTime <= 0f)
            StartPause();
    }

    Vector3 BoundaryRepulsion()
    {
        if (manager.boundaryPoints == null || manager.boundaryPoints.Count == 0)
            return Vector3.zero;
        
        float weight = manager.boundaryWeight;

        if (isAfraid)
            weight *= 30f;

        Vector3 steer = Vector3.zero;
        Vector3 pos = transform.position;

        foreach (Vector3 p in manager.boundaryPoints)
        {
            Vector3 delta = pos - p;
            delta.y = 0f;
            float dist = delta.magnitude;

            if (dist < manager.boundaryForceDistance && dist > 0f)
            {
                Vector3 repulsion = delta.normalized * (manager.boundaryForceDistance - dist);
                steer += repulsion;
                Debug.DrawLine(pos + Vector3.up * 0.5f, p + Vector3.up * 0.5f, Color.red);
            }
        }

        return steer * weight;
    }

    public void AddFearForce(Vector3 fearForce)
    {
        velocity += fearForce * Time.deltaTime;
        isAfraid = true;
        fearTimer = fearDuration;
    }
    
    Vector3 ColliderRepulsion()
    {
        Vector3 steer = Vector3.zero;
        Vector3 pos = transform.position;

        foreach (Collider col in manager.forbiddenColliders)
        {
            if (col == null) continue;

            // Trouve le point le plus proche sur le collider
            Vector3 closest = col.ClosestPoint(pos);
            Vector3 delta = pos - closest;
            delta.y = 0f;

            float dist = delta.magnitude;

            // Si trop proche → repousse
            if (dist < manager.boundaryForceDistance && dist > 0f)
            {
                Vector3 repulsion = delta.normalized * (manager.boundaryForceDistance - dist);
                steer += repulsion;

                Debug.DrawLine(pos + Vector3.up * 0.5f, closest + Vector3.up * 0.5f, Color.cyan);
            }
        }

        return steer * manager.boundaryWeight;
    }


    public void CalmDown() => isAfraid = false;
    public void SetNature(NatureType type) { natureType = type; natureStrategy = NatureFactory.Create(type); }

    void ScheduleNextPause() => nextPauseTime = Random.Range(manager.minTimeBetweenPauses.x, manager.minTimeBetweenPauses.y);
    void StartPause() { isPaused = true; pauseTimer = Random.Range(manager.pauseDuration.x, manager.pauseDuration.y); }

    private void OnDrawGizmos()
    {
        Gizmos.color = natureColors[(int)natureType];
        Gizmos.DrawCube(transform.position + Vector3.up * 0.5f, Vector3.one * 0.2f);
    }
}
