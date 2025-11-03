using UnityEngine;

public class SheepBoid : MonoBehaviour
{
    [HideInInspector] public SheepBoidManager manager;
    [SerializeField] private NatureType natureType;
    public INatureStrategy natureStrategy;

    public Vector3 velocity;
    private bool isPaused;
    private float pauseTimer, nextPauseTime;

    public bool isAfraid = false;
    private float fearSpeedMultiplier = 2f;

    void Start()
    {
        natureStrategy = NatureFactory.Create(natureType);
        ScheduleNextPause();
    }

    private void OnEnable()
    {
        if (manager == null)
            return;
        
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

        // --- Comportement Boid ---
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
            toOther.y = 0f;
            float dist = toOther.magnitude;
            if (dist == 0f) continue;

            if (dist < manager.separationRadius)
                separation -= toOther.normalized / dist;

            alignment += other.velocity;
            cohesion += other.transform.position;

            natureStrategy.ApplyNature(this, ref separation, ref alignment, ref cohesion, other);
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
        accel += Random.insideUnitSphere * manager.noise * Time.deltaTime;
        accel += StayInBounds();

        velocity += accel * Time.deltaTime;
        velocity.y = 0f;

        float currentMinSpeed = manager.minSpeed;
        float currentMaxSpeed = manager.maxSpeed;
        if (isAfraid)
        {
            currentMinSpeed *= fearSpeedMultiplier;
            currentMaxSpeed *= fearSpeedMultiplier;
        }

        float speed = Mathf.Clamp(velocity.magnitude, currentMinSpeed, currentMaxSpeed);
        // protect against zero velocity before normalization
        if (velocity.sqrMagnitude > 0.000001f)
            velocity = velocity.normalized * speed;
        else
            velocity = Random.insideUnitSphere.normalized * currentMinSpeed;

        natureStrategy.PostProcess(this, ref velocity);

        if (float.IsNaN(velocity.x) || float.IsNaN(velocity.y) || float.IsNaN(velocity.z))
            velocity = Vector3.zero;

        transform.position += velocity * Time.deltaTime;

        // Clamp dans la bounding box centrée sur manager
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, manager.transform.position.x - manager.bounds.x, manager.transform.position.x + manager.bounds.x);
        pos.z = Mathf.Clamp(pos.z, manager.transform.position.z - manager.bounds.z, manager.transform.position.z + manager.bounds.z);
        transform.position = pos;

        if (velocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 0.1f);
        }

        nextPauseTime -= Time.deltaTime;
        if (nextPauseTime <= 0f)
            StartPause();
    }

    public void AddFearForce(Vector3 fearForce)
    {
        velocity += fearForce * Time.deltaTime;
        isAfraid = true;
    }

    public void CalmDown()
    {
        isAfraid = false;
    }

    Vector3 StayInBounds()
    {
        Vector3 pos = transform.position;
        Vector3 center = manager.transform.position;
        Vector3 b = manager.bounds;
        Vector3 steer = Vector3.zero;

        float marginX = b.x * 0.9f;
        float marginZ = b.z * 0.9f;

        if (pos.x > center.x + marginX)
            steer += Vector3.left * ((pos.x - (center.x + marginX)) / Mathf.Max(0.0001f, (b.x - marginX)));
        else if (pos.x < center.x - marginX)
            steer += Vector3.right * ((((center.x - marginX) - pos.x)) / Mathf.Max(0.0001f, (b.x - marginX)));

        if (pos.z > center.z + marginZ)
            steer += Vector3.back * ((pos.z - (center.z + marginZ)) / Mathf.Max(0.0001f, (b.z - marginZ)));
        else if (pos.z < center.z - marginZ)
            steer += Vector3.forward * ((((center.z - marginZ) - pos.z)) / Mathf.Max(0.0001f, (b.z - marginZ)));

        // Force plus forte si vraiment en dehors des limites
        if (Mathf.Abs(pos.x - center.x) > b.x || Mathf.Abs(pos.z - center.z) > b.z)
        {
            steer += (center - pos).normalized * 2f;
        }

        return steer * manager.boundaryWeight;
    }

    public void SetNature(NatureType type)
    {
        natureType = type;
        natureStrategy = NatureFactory.Create(type);
    }

    void ScheduleNextPause() =>
        nextPauseTime = Random.Range(manager.minTimeBetweenPauses.x, manager.minTimeBetweenPauses.y);

    void StartPause()
    {
        isPaused = true;
        pauseTimer = Random.Range(manager.pauseDuration.x, manager.pauseDuration.y);
    }
}
