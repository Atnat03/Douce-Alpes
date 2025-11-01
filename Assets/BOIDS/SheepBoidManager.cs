using UnityEngine;

public class SheepBoidManager : MonoBehaviour
{
    [Header("Réglages généraux")]
    public GameObject prefab;
    public int count = 50;
    public Vector3 bounds = new Vector3(25, 0, 25);

    [Header("Mouvement Boids")]
    public float neighborRadius = 3f;
    public float separationRadius = 1f;
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    public float separationWeight = 1.5f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float noise = 0.3f;

    [Header("Limites de la zone")]
    public float boundMargin = 3f;
    public float boundaryWeight = 2f;

    [Header("Pauses")]
    public Vector2 minTimeBetweenPauses = new Vector2(5, 10);
    public Vector2 pauseDuration = new Vector2(1, 3);

    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-bounds.x, bounds.x),
                0,
                Random.Range(-bounds.z, bounds.z)
            );

            GameObject go = Instantiate(prefab, pos, Quaternion.identity, transform);
            SheepBoid sheep = go.GetComponent<SheepBoid>();
            sheep.manager = this;

            // Donne une nature aléatoire (ou tu peux forcer pour tester)
            NatureType randomNature = (NatureType)Random.Range(0, System.Enum.GetValues(typeof(NatureType)).Length);
            sheep.SetNature(randomNature);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(bounds.x * 2, 0.1f, bounds.z * 2));
    }
}