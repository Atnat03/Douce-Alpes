using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FakeSheepBoid : MonoBehaviour
{
    [HideInInspector] public FakeSheepBoidManager manager;
    [SerializeField] public NatureType natureType;
    public INatureStrategy natureStrategy;

    public Vector3 velocity;
    private bool isPaused;
    private float pauseTimer, nextPauseTime;

    public bool isAfraid = false;
    [SerializeField] private float fearSpeedMultiplier = 3f;
    private float fearDuration = 1f; 
    private float fearTimer;

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
        
        manager = FakeSheepBoidManager.instance;
        FakeSheepBoidManager.instance.RegisterSheep(this);

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

        Vector3 accel = Vector3.zero;

        accel += BoundaryRepulsion();

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
