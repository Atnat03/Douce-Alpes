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
    
    Animator animator;

    private Color[] natureColors = new Color[]
    {
        Color.red,
        Color.blue,
        Color.gray,
        Color.yellow
    };

    void Start()
    {
        if (manager == null)
        {
            manager = FakeSheepBoidManager.instance;
            if (manager == null)
            {
                Debug.LogError("Aucun FakeSheepBoidManager trouvé !");
                enabled = false;
                return;
            }
        }
        
        animator = transform.GetChild(0).transform.GetChild(0).GetComponent<Animator>();
        animator.SetBool("Walk", true);

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
        if (manager == null) return;

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

        accel += Separation() * manager.separationWeight;
        accel += Alignment() * manager.alignmentWeight;
        accel += Cohesion() * manager.cohesionWeight;
        accel += BoundaryRepulsion();
        accel += ColliderRepulsion();

        // Sécurité : éviter NaN dans accel
        if (float.IsNaN(accel.x) || float.IsNaN(accel.y) || float.IsNaN(accel.z))
            accel = Vector3.zero;

        velocity += accel * Time.deltaTime;
        velocity.y = 0f;

        // Si velocity devient trop petit, on lui donne un petit vecteur aléatoire
        if (velocity.sqrMagnitude < 0.0001f || float.IsNaN(velocity.x))
        {
            velocity = Random.insideUnitSphere;
            velocity.y = 0f;
        }

        float speed = Mathf.Clamp(velocity.magnitude, manager.minSpeed, manager.maxSpeed);

        if (isAfraid)
        {
            speed *= fearSpeedMultiplier;
            fearTimer -= Time.deltaTime;
            if (fearTimer <= 0f)
                CalmDown();
        }

        velocity = velocity.normalized * speed;
        
        // Sécurité finale
        if (float.IsNaN(velocity.x) || float.IsNaN(velocity.y) || float.IsNaN(velocity.z))
            velocity = Random.insideUnitSphere;

        transform.position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 0.1f);
        }

        nextPauseTime -= Time.deltaTime;
        if (nextPauseTime <= 0f)
        {
            StartPause();
            ScheduleNextPause();
        }
    }


    // ---------------- Règles Boids ----------------

    Vector3 Separation()
    {
        Vector3 away = Vector3.zero;
        int count = 0;

        foreach (var other in manager.sheepBoids)
        {
            if (other == this) continue;
            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < manager.separationRadius)
            {
                away += (transform.position - other.transform.position) / dist;
                count++;
            }
        }

        if (count > 0) away /= count;
        return away;
    }
    
    Vector3 ColliderRepulsion()
    {
        Vector3 steer = Vector3.zero;
        Vector3 pos = transform.position;

        foreach (Collider col in manager.forbiddenColliders)
        {
            if (col == null) continue;

            // Trouver le point le plus proche sur le collider
            Vector3 closest = col.ClosestPoint(pos);
            Vector3 delta = pos - closest;
            delta.y = 0f;

            float dist = delta.magnitude;

            // Si trop proche → répulsion
            if (dist < manager.boundaryForceDistance && dist > 0f)
            {
                Vector3 repulsion = delta.normalized * (manager.boundaryForceDistance - dist);
                steer += repulsion;

                Debug.DrawLine(pos + Vector3.up * 0.5f, closest + Vector3.up * 0.5f, Color.cyan);
            }
        }

        return steer * manager.boundaryWeight;
    }

    Vector3 Alignment()
    {
        Vector3 avgVel = Vector3.zero;
        int count = 0;

        foreach (var other in manager.sheepBoids)
        {
            if (other == this) continue;
            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < manager.neighborRadius)
            {
                avgVel += other.velocity;
                count++;
            }
        }

        if (count > 0) avgVel /= count;
        return (avgVel - velocity) * 0.5f;
    }

    Vector3 Cohesion()
    {
        Vector3 center = Vector3.zero;
        int count = 0;

        foreach (var other in manager.sheepBoids)
        {
            if (other == this) continue;
            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < manager.neighborRadius)
            {
                center += other.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            center /= count;
            return (center - transform.position) * 0.5f;
        }

        return Vector3.zero;
    }

    // ---------------- Frontières ----------------

    Vector3 BoundaryRepulsion()
    {
        if (manager.boundaryPoints == null || manager.boundaryPoints.Count == 0)
            return Vector3.zero;

        float weight = manager.boundaryWeight;
        if (isAfraid) weight *= 30f;

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

    public void SetNature(NatureType type)
    {
        natureType = type;
        natureStrategy = NatureFactory.Create(type);
    }

    void ScheduleNextPause() => nextPauseTime = Random.Range(manager.minTimeBetweenPauses.x, manager.minTimeBetweenPauses.y);
    void StartPause() => pauseTimer = Random.Range(manager.pauseDuration.x, manager.pauseDuration.y);

    private void OnDrawGizmos()
    {
        if ((int)natureType < natureColors.Length)
        {
            Gizmos.color = natureColors[(int)natureType];
            Gizmos.DrawCube(transform.position + Vector3.up * 0.5f, Vector3.one * 0.2f);
        }
    }
}
