using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SplineMeshGenerator : MonoBehaviour
{
    public SplineMaison spline;
    public float width = 0.1f;
    public bool updateEveryFrame = true;

    private Mesh mesh;

    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        if (updateEveryFrame && spline != null)
        {
            GenerateMesh();
        }
    }

    public void GenerateMesh()
    {
        if (spline == null || spline.points.Count < 2) return;

        Vector3 start = spline.points[0];
        Vector3 end = spline.points[1];

        // Direction
        Vector3 forward = (end - start).normalized;
        Vector3 right = Vector3.Cross(forward, Vector3.forward).normalized * width;

        // Vertices
        Vector3[] vertices = new Vector3[4];
        vertices[0] = start + right;
        vertices[1] = start - right;
        vertices[2] = end + right;
        vertices[3] = end - right;

        // Triangles
        int[] triangles = new int[6] { 0, 2, 1, 1, 2, 3 };

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}