using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SplineMaison : MonoBehaviour
{
    [Header("Points de la spline")]
    public List<Vector3> points = new List<Vector3>();

    [Header("Options")]
    [SerializeField] private int segmentsParSegment = 10;

    [Header("Debug")]
    public bool dessinerGizmos = true;

    /// <summary>
    /// Récupère la position interpolée sur la spline entre 0 et 1
    /// </summary>
    public Vector3 GetPosition(float t)
    {
        if (points.Count == 0) return Vector3.zero;
        if (points.Count == 1) return points[0];

        t = Mathf.Clamp01(t);
        int segmentCount = points.Count - 1;
        float totalT = t * segmentCount;
        int index = Mathf.FloorToInt(totalT);
        float localT = totalT - index;

        if (index >= points.Count - 1)
            return points[points.Count - 1];

        return Vector3.Lerp(points[index], points[index + 1], localT);
    }

    /// <summary>
    /// Met à jour le dernier point de la spline
    /// NE RAJOUTE JAMAIS DE NOUVEAU POINT
    /// </summary>
    public void SetDernierPoint(Vector3 pos)
    {
        if (points.Count < 2) return; // Doit déjà y avoir 2 points
        points[1] = pos; // Modifier uniquement le 2e point
    }

    /// <summary>
    /// Initialise la spline avec 2 points
    /// </summary>
    public void InitSpline(Vector3 debut, Vector3 fin)
    {
        points.Clear();
        points.Add(debut);
        points.Add(fin);
    }

    private void OnDrawGizmos()
    {
        if (!dessinerGizmos || points.Count < 2) return;

        Gizmos.color = Color.green;

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 start = points[i];
            Vector3 end = points[i + 1];
            Gizmos.DrawLine(start, end);

            for (int s = 1; s <= segmentsParSegment; s++)
            {
                float t = s / (float)segmentsParSegment;
                Vector3 pos = Vector3.Lerp(start, end, t);
                Gizmos.DrawSphere(pos, 0.02f);
            }
        }
    }
}
