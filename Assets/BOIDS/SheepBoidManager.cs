using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SheepBoidManager : MonoBehaviour
{
    public static Action<SheepBoid> OnListChanged;

    [Header("Réglages généraux")] 
    public Vector3 bounds;
    public GameObject prefab;
    public int count = 50;
    public Vector3 spawnPosition = Vector3.zero; // tous les moutons spawnent au même endroit
    public float spawnDelay = 0.2f; // délai entre chaque mouton

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
        StartCoroutine(SpawnSheepRoutine());
    }

    private IEnumerator SpawnSheepRoutine()
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
            SheepBoid sheep = go.GetComponent<SheepBoid>();
            sheep.manager = this;

            NatureType randomNature = (NatureType)Random.Range(0, Enum.GetValues(typeof(NatureType)).Length);
            sheep.SetNature(randomNature);

            Sheep sheepScript = sheep.GetComponent<Sheep>();
            sheepScript.sheepId = nbInstantSheep;
            GameManager.instance.sheepList.Add(sheepScript);

            nbInstantSheep++;
            OnListChanged?.Invoke(sheep);

            yield return new WaitForSeconds(spawnDelay);
        }

        GameData.instance.nbSheep = nbInstantSheep;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnPosition, new Vector3(1f, 0.1f, 1f));
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(bounds.x * 2, 0.1f, bounds.z * 2));
    }
}
