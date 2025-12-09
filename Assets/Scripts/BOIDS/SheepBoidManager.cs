using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SheepBoidManager : MonoBehaviour
{
    public static Action<SheepBoid> OnListChanged;
    public static SheepBoidManager instance;

    [Header("Réglages généraux")]
    public MeshCollider meshCollider;
    public float boundaryForceDistance = 1.5f;
    public float boundaryWeight = 5f;

    [Header("Spawn Points")]
    public List<Vector3> boundaryPoints = new List<Vector3>();
    public int pointsPerEdge = 20;

    [Header("Moutons")]
    public GameObject prefab;
    public int countStart = 50;
    public Vector3 spawnPosition = Vector3.zero;
    public float spawnDelay = 0.2f;

    [Header("Mouvement Boids")]
    public float neighborRadius = 3f;
    public float separationRadius = 1f;
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    public float separationWeight = 1.5f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float noise = 0.3f;

    [Header("Pauses")]
    public Vector2 minTimeBetweenPauses = new Vector2(5, 10);
    public Vector2 pauseDuration = new Vector2(1, 3);

    [Header("UI")]
    [SerializeField] public InputField nameInputField;
    
    public int nbInstantSheep = 0;

    private void Awake() => instance = this;

    private void Start()
    {
        prefab = GameData.instance.sheepPrefab;
        if(meshCollider != null) GenerateBoundaryPointsFromMesh();
        StartCoroutine(SpawnSheepRoutine());
    }

    private void GenerateBoundaryPointsFromMesh()
    {
        boundaryPoints.Clear();

        if (meshCollider == null)
        {
            Debug.LogError("Aucun MeshCollider assigné !");
            return;
        }

        Mesh mesh = meshCollider.sharedMesh;
        if (mesh == null)
        {
            Debug.LogError("MeshCollider sans mesh !");
            return;
        }

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        var edgeCount = new Dictionary<(int, int), int>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int a = triangles[i];
            int b = triangles[i + 1];
            int c = triangles[i + 2];

            AddEdge(edgeCount, a, b);
            AddEdge(edgeCount, b, c);
            AddEdge(edgeCount, c, a);
        }

        Vector3 meshCenter = meshCollider.bounds.center;
        float maxDistance = 0f;
        foreach (var v in vertices)
        {
            float d = Vector3.Distance(meshCenter, meshCollider.transform.TransformPoint(v));
            if (d > maxDistance) maxDistance = d;
        }

        int subdivisions = 3;
        HashSet<Vector3> uniquePoints = new HashSet<Vector3>();

        foreach (var edge in edgeCount)
        {
            if (edge.Value == 1)
            {
                Vector3 start = meshCollider.transform.TransformPoint(vertices[edge.Key.Item1]);
                Vector3 end = meshCollider.transform.TransformPoint(vertices[edge.Key.Item2]);

                if (Vector3.Distance(meshCenter, start) < maxDistance * 0.5f) continue;
                if (Vector3.Distance(meshCenter, end) < maxDistance * 0.5f) continue;

                start.y = meshCollider.transform.position.y;
                end.y = meshCollider.transform.position.y;

                uniquePoints.Add(start);
                uniquePoints.Add(end);

                for (int s = 1; s <= subdivisions; s++)
                {
                    float t = s / (float)(subdivisions + 1);
                    Vector3 mid = Vector3.Lerp(start, end, t);
                    if (Vector3.Distance(meshCenter, mid) < maxDistance * 0.5f) continue;
                    uniquePoints.Add(mid);
                }
            }
        }

        boundaryPoints.AddRange(uniquePoints);
    }

    private void AddEdge(Dictionary<(int, int), int> edges, int a, int b)
    {
        var key = a < b ? (a, b) : (b, a);
        if (edges.ContainsKey(key))
            edges[key]++;
        else
            edges[key] = 1;
    }

    private IEnumerator SpawnSheepRoutine()
    {
        for (int i = 0; i < countStart; i++)
        {
            SpawnNewSheep("Antoine");
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void CreateSheep()
    {
        if (!string.IsNullOrEmpty(nameInputField.text))
            SpawnNewSheep(nameInputField.text);
        else
            Debug.LogError("Entrée de nom incorrect");
    }

    public void SpawnNewSheep(string name)
    {
        GameObject go = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
        SheepBoid sheep = go.GetComponent<SheepBoid>();
        sheep.manager = this;

        NatureType randomNature = GetRandomNature();
        sheep.SetNature(randomNature);
        sheep.natureBase = randomNature;

        Sheep sheepScript = sheep.GetComponent<Sheep>();
        sheepScript.sheepId = nbInstantSheep;
        
        sheepScript.Initialize(nbInstantSheep, name);

        GameManager.instance.sheepList.Add(sheepScript);

        nbInstantSheep++;
        GameData.instance.nbSheep++;


        OnListChanged?.Invoke(sheep);
    }

    private int nbDominant = 0;
    private int nbPeureux = 0;
    private int nbSolitaire = 0;
    
    NatureType GetRandomNature()
    {
        float pD = 30 - nbDominant * 9;    
        float pP = 20 - nbPeureux * 3.5f; 
        float pSo = 45 - nbSolitaire * 11;

        pD = Mathf.Max(pD, 0);
        pP = Mathf.Max(pP, 0);
        pSo = Mathf.Max(pSo, 0);

        float pSt = 100 - (pD + pP + pSo);
        pSt = Mathf.Max(pSt, 0);

        float rand = Random.Range(0f, 100f);

        if (rand < pD)
            return NatureType.Dominant;
        else if (rand < pD + pP)
            return NatureType.Peureux;
        else if (rand < pD + pP + pSo)
            return NatureType.Solitaire;
        else
            return NatureType.Standard;
    }

    public GameObject SheepGetOffAndRecreate(SheepData data, Vector3 spawnP)
    {
        GameObject go = Instantiate(prefab, spawnP, Quaternion.identity, transform);
        SheepBoid sheep = go.GetComponent<SheepBoid>();
        sheep.manager = this;

        Sheep sheepScript = sheep.GetComponent<Sheep>();
        sheepScript.sheepId = data.id;
        sheepScript.currentSkinHat = data.skinHat;
        sheepScript.currentSkinClothe = data.skinClothe;
        sheepScript.hasLaine = data.hasWhool;
        sheepScript.sheepName = data.name;
        sheep.natureType = data.nature;

        GameManager.instance.sheepList.Add(sheepScript);
        sheep.enabled = false;

        nbInstantSheep++;
        GameData.instance.nbSheep++;

        sheepScript.Initialize(nbInstantSheep, data.name);
        OnListChanged?.Invoke(sheep);
        return go;
    }

    private void OnDrawGizmos()
    {
        if(boundaryPoints.Count > 0)
        {
            Gizmos.color = Color.yellow;
            foreach(var p in boundaryPoints)
                Gizmos.DrawSphere(p, 0.05f);
        }
    }
}
