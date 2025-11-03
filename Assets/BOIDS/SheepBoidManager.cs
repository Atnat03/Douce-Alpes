using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SheepBoidManager : MonoBehaviour
{
    public static Action<SheepBoid> OnListChanged;
    
    [Header("Réglages généraux")]
    GameObject prefab;
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
    
    private int nbInstantSheep = 0;

    void Start()
    {
        prefab = GameData.instance.sheepPrefab;
        
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(transform.position.x + (-bounds.x), transform.position.x + bounds.x),
                transform.position.y,
                Random.Range(transform.position.z + -bounds.z, transform.position.z + bounds.z)
            );

            GameObject go = Instantiate(prefab, pos, Quaternion.identity, transform);
            SheepBoid sheep = go.GetComponent<SheepBoid>();
            sheep.manager = this;

            // Donne une nature aléatoire (ou tu peux forcer pour tester)
            NatureType randomNature = (NatureType)Random.Range(0, System.Enum.GetValues(typeof(NatureType)).Length);
            sheep.SetNature(randomNature);

            Sheep sheepScript = sheep.GetComponent<Sheep>();
            sheepScript.sheepId = nbInstantSheep;
            GameManager.instance.sheepList.Add(sheepScript);
            
            nbInstantSheep++;
            
            OnListChanged?.Invoke(sheep);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(bounds.x * 2, 0.1f, bounds.z * 2));

        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
        Gizmos.DrawWireCube(transform.position, new Vector3((bounds.x - boundMargin) * 2, 0.1f, (bounds.z - boundMargin) * 2));
    }

}