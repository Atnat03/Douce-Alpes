using UnityEngine;

public class SheepBoid : MonoBehaviour
{
    [HideInInspector] public SheepBoidManager manager;
    [SerializeField] private NatureType natureType = NatureType.Suiveur;
    public INatureStrategy natureStrategy;

    private Vector3 velocity;
    private bool isPaused;
    private float pauseTimer, nextPauseTime;

    void Start()
    {
        velocity = Random.insideUnitSphere;
        velocity.y = 0;
        velocity = velocity.normalized * Random.Range(manager.minSpeed, manager.maxSpeed);

        // On crée la stratégie selon le type
        natureStrategy = NatureFactory.Create(natureType);

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

            // On laisse la stratégie ajuster selon la nature
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
        velocity.y = 0;

        float speed = Mathf.Clamp(velocity.magnitude, manager.minSpeed, manager.maxSpeed);
        velocity = velocity.normalized * speed;

        // post-traitement de la stratégie
        natureStrategy.PostProcess(this, ref velocity);

        transform.position += velocity * Time.deltaTime;
        if (velocity.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity), 0.1f);

        nextPauseTime -= Time.deltaTime;
        if (nextPauseTime <= 0)
            StartPause();
    }

    Vector3 StayInBounds()
    {
        Vector3 pos = transform.position;
        Vector3 b = manager.bounds;
        Vector3 steer = Vector3.zero;
        float margin = manager.boundMargin;

        if (pos.x > b.x - margin) steer += Vector3.left;
        else if (pos.x < -b.x + margin) steer += Vector3.right;

        if (pos.z > b.z - margin) steer += Vector3.back;
        else if (pos.z < -b.z + margin) steer += Vector3.forward;

        if (Mathf.Abs(pos.x) > b.x || Mathf.Abs(pos.z) > b.z)
            steer += (-pos.normalized) * 2f;

        return steer.normalized * manager.boundaryWeight;
    }
    
    public void SetNature(NatureType type)
    {
        natureType = type;
        natureStrategy = NatureFactory.Create(type);

        // Optionnel : donner une couleur selon la nature
        Color color = type switch
        {
            NatureType.Dominant => Color.red,
            NatureType.Peureux => Color.blue,
            NatureType.Curieux => Color.yellow,
            _ => Color.white,
        };
        GetComponent<Renderer>().material.color = color;
    }


    void ScheduleNextPause() => nextPauseTime = Random.Range(manager.minTimeBetweenPauses.x, manager.minTimeBetweenPauses.y);
    void StartPause() { isPaused = true; pauseTimer = Random.Range(manager.pauseDuration.x, manager.pauseDuration.y); }

    public Vector3 GetVelocity() => velocity;
}
