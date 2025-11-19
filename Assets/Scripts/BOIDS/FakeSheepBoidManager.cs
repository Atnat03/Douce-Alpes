using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeSheepBoidManager : MonoBehaviour
{
    public static Action<FakeSheepBoid> OnListChanged;
    public static FakeSheepBoidManager instance;

    [Header("Réglages généraux")]
    public MeshCollider meshCollider;
    public float boundaryForceDistance = 1.5f;
    public float boundaryWeight = 5f;

    [Header("Spawn Points")]
    public List<Vector3> boundaryPoints = new List<Vector3>();
    public int pointsPerEdge = 20;

    [Header("Troupeau")]
    public List<FakeSheepBoid> sheepBoids = new List<FakeSheepBoid>();

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

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (meshCollider != null)
            GenerateBoundaryPointsFromMesh();

        Debug.Log("FakeSheepBoidManager initialisé : aucun mouton généré automatiquement.");
    }

    // Permet d’ajouter un mouton que tu places toi-même dans la scène
    public void RegisterSheep(FakeSheepBoid newSheep)
    {
        if (!sheepBoids.Contains(newSheep))
        {
            sheepBoids.Add(newSheep);
            OnListChanged?.Invoke(newSheep);
            Debug.Log($"Sheep ajouté : {newSheep.name}");
        }
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
        Debug.Log($"Points générés : {boundaryPoints.Count}");
    }

    private void AddEdge(Dictionary<(int, int), int> edges, int a, int b)
    {
        var key = a < b ? (a, b) : (b, a);
        if (edges.ContainsKey(key))
            edges[key]++;
        else
            edges[key] = 1;
    }

    private void OnDrawGizmos()
    {
        if (boundaryPoints.Count > 0)
        {
            Gizmos.color = Color.yellow;
            foreach (var p in boundaryPoints)
                Gizmos.DrawSphere(p, 0.05f);
        }
    }
}
