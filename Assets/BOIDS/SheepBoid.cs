using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Boid version "mouton" : mouvement doux sur le plan XZ, avec pauses.
/// </summary>
public class SheepBoid : MonoBehaviour
{
    [HideInInspector] public SheepBoidManager manager;

    Vector3 velocity;
    bool isPaused = false;
    float pauseTimer = 0f;
    float nextPauseTime = 0f;

    void Start()
    {
        // vitesse initiale
        velocity = Random.insideUnitSphere;
        velocity.y = 0;
        velocity = velocity.normalized * Random.Range(manager.minSpeed, manager.maxSpeed);
        ScheduleNextPause();
    }

    void Update()
    {
        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0)
            {
                isPaused = false;
                ScheduleNextPause();
            }
            return;
        }

        // comportement des boids
        Vector3 accel = Vector3.zero;
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;

        int count = 0;
        Collider[] hits = Physics.OverlapSphere(transform.position, manager.neighborRadius);
        foreach (var c in hits)
        {
            if (c.gameObject == gameObject) continue;
            SheepBoid other = c.GetComponent<SheepBoid>();
            if (other == null) continue;

            Vector3 toOther = other.transform.position - transform.position;
            toOther.y = 0;
            float dist = toOther.magnitude;
            if (dist == 0) continue;

            if (dist < manager.separationRadius)
                separation -= toOther.normalized / dist;

            alignment += other.velocity;
            cohesion += other.transform.position;
            count++;
        }

        if (count > 0)
        {
            alignment /= count;
            alignment = alignment.normalized * velocity.magnitude - velocity;

            cohesion /= count;
            cohesion = (cohesion - transform.position).normalized * velocity.magnitude - velocity;
        }

        accel += separation * manager.separationWeight;
        accel += alignment * manager.alignmentWeight;
        accel += cohesion * manager.cohesionWeight;

        // bruit léger pour un comportement plus organique
        accel += Random.insideUnitSphere * manager.noise * Time.deltaTime;

        // mise à jour vitesse
        velocity += accel * Time.deltaTime;
        velocity.y = 0;
        float speed = velocity.magnitude;
        speed = Mathf.Clamp(speed, manager.minSpeed, manager.maxSpeed);
        velocity = velocity.normalized * speed;

        // déplacement
        transform.position += velocity * Time.deltaTime;

        // rotation vers la direction du déplacement
        if (velocity.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity), 0.1f);

        // garder les moutons dans le pâturage
        Vector3 pos = transform.position;
        Vector3 b = manager.bounds;
        if (pos.x > b.x) pos.x = -b.x;
        if (pos.x < -b.x) pos.x = b.x;
        if (pos.z > b.z) pos.z = -b.z;
        if (pos.z < -b.z) pos.z = b.z;
        transform.position = pos;

        // déclenchement pause aléatoire
        nextPauseTime -= Time.deltaTime;
        if (nextPauseTime <= 0)
        {
            StartPause();
        }
    }

    void ScheduleNextPause()
    {
        nextPauseTime = Random.Range(manager.minTimeBetweenPauses.x, manager.minTimeBetweenPauses.y);
    }

    void StartPause()
    {
        isPaused = true;
        pauseTimer = Random.Range(manager.pauseDuration.x, manager.pauseDuration.y);
    }

    public Vector3 Velocity => _velocity;
    private Vector3 _velocity
    {
        get => velocityField;
        set => velocityField = value;
    }
    private Vector3 velocityField;
}
