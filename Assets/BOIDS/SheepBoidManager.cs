using UnityEngine;

public class SheepBoidManager : MonoBehaviour
{
    [Header("Prefab & spawn")]
    public GameObject sheepPrefab;
    public int sheepCount = 30;
    public Vector3 bounds = new Vector3(25f, 0f, 25f);

    [Header("Vitesse")]
    public float minSpeed = 1f;
    public float maxSpeed = 3f;

    [Header("Interactions")]
    public float neighborRadius = 3f;
    public float separationRadius = 1f;

    [Header("Poids des règles")]
    public float separationWeight = 1.2f;
    public float alignmentWeight = 0.8f;
    public float cohesionWeight = 0.8f;

    [Header("Comportement aléatoire")]
    public float noise = 0.5f;
    public Vector2 minTimeBetweenPauses = new Vector2(5f, 10f);
    public Vector2 pauseDuration = new Vector2(1f, 3f);

    void Start()
    {
        for (int i = 0; i < sheepCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-bounds.x, bounds.x),
                0f,
                Random.Range(-bounds.z, bounds.z)
            );
            GameObject go = Instantiate(sheepPrefab, pos, Random.rotation);
            SheepBoid s = go.GetComponent<SheepBoid>();
            if (s == null)
                s = go.AddComponent<SheepBoid>();
            s.manager = this;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.1f, bounds * 2f);
    }
}