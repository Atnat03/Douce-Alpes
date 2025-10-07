using System.Collections.Generic;
using UnityEngine;

public class ShapeDetection : MonoBehaviour
{
    public SwipeDetection swipeDetection;

    [Header("Paramètres")]
    public float closureDistance = 80f;
    public float cornerAngleMin = 40f;
    public float cornerAngleMax = 140f;
    public float minCircleLength = 300f;

    private void OnEnable()
    {
        if (swipeDetection == null)
            swipeDetection = SwipeDetection.instance;

        swipeDetection.OnSwipeFinished += AnalyzeShape;
    }

    private void OnDisable()
    {
        if (swipeDetection == null) return;
        swipeDetection.OnSwipeFinished -= AnalyzeShape;
    }

    private void AnalyzeShape(List<Vector2> points)
    {
        if (points.Count < 10) return;

        var simplified = SimplifyPath(points, 5f);
        simplified = NormalizePoints(simplified);

        float totalLength = TotalLength(points);
        float closure = Vector2.Distance(points[0], points[^1]);
        float closureRatio = closure / totalLength;

        if (closureRatio > 0.2f || totalLength < minCircleLength)
            return;

        float circularity = ComputeCircularity(points);
        int corners = CountCorners(simplified);

        if (circularity > 0.5f && corners <= 2)
            SwipeDetection.instance.OnSwipeDetected?.Invoke(SwipeType.Circle);
        else if (corners >= 3 && corners <= 5)
            SwipeDetection.instance.OnSwipeDetected?.Invoke(SwipeType.Square);
    }

    private int CountCorners(List<Vector2> points)
    {
        int corners = 0;
        for (int i = 1; i < points.Count - 1; i++)
        {
            Vector2 a = (points[i] - points[i - 1]).normalized;
            Vector2 b = (points[i + 1] - points[i]).normalized;
            float angle = Vector2.Angle(a, b);
            if (angle > cornerAngleMin && angle < cornerAngleMax)
                corners++;
        }
        return corners;
    }

    private float TotalLength(List<Vector2> points)
    {
        float total = 0f;
        for (int i = 1; i < points.Count; i++)
            total += Vector2.Distance(points[i], points[i - 1]);
        return total;
    }

    private List<Vector2> SimplifyPath(List<Vector2> path, float tolerance)
    {
        if (path.Count < 3) return new List<Vector2>(path);
        int index = -1;
        float distMax = 0f;

        for (int i = 1; i < path.Count - 1; i++)
        {
            float dist = DistanceToLine(path[i], path[0], path[^1]);
            if (dist > distMax)
            {
                distMax = dist;
                index = i;
            }
        }

        if (distMax > tolerance)
        {
            var rec1 = SimplifyPath(path.GetRange(0, index + 1), tolerance);
            var rec2 = SimplifyPath(path.GetRange(index, path.Count - index), tolerance);
            rec1.RemoveAt(rec1.Count - 1);
            rec1.AddRange(rec2);
            return rec1;
        }
        else return new List<Vector2> { path[0], path[^1] };
    }

    private float DistanceToLine(Vector2 p, Vector2 a, Vector2 b)
    {
        if (a == b) return Vector2.Distance(p, a);
        return Mathf.Abs((b.y - a.y) * p.x - (b.x - a.x) * p.y + b.x * a.y - b.y * a.x)
               / Vector2.Distance(a, b);
    }

    private List<Vector2> NormalizePoints(List<Vector2> points)
    {
        Vector2 min = points[0], max = points[0];
        foreach (var p in points)
        {
            min = Vector2.Min(min, p);
            max = Vector2.Max(max, p);
        }

        Vector2 size = max - min;
        Vector2 offset = -min;

        List<Vector2> normalized = new();
        foreach (var p in points)
        {
            Vector2 norm = (p + offset);
            norm.x /= size.x != 0 ? size.x : 1;
            norm.y /= size.y != 0 ? size.y : 1;
            normalized.Add(norm);
        }
        return normalized;
    }

    private float ComputeCircularity(List<Vector2> points)
    {
        if (points.Count < 3) return 0f;

        Vector2 center = Vector2.zero;
        foreach (var p in points) center += p;
        center /= points.Count;

        float avgRadius = 0f;
        foreach (var p in points) avgRadius += Vector2.Distance(center, p);
        avgRadius /= points.Count;

        float variance = 0f;
        foreach (var p in points)
        {
            float dist = Vector2.Distance(center, p);
            variance += Mathf.Abs(dist - avgRadius);
        }
        variance /= points.Count;

        return Mathf.Clamp01(1f - (variance / avgRadius));
    }
}
