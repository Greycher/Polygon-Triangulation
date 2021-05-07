using System.Collections.Generic;
using PolygonTriangulation;
using PolygonTriangulation.Tester;
using UnityEngine;

public class PolygonIntersectionDetectorTester : MonoBehaviour
{
    public Vector2[] vertices;

    private const float PointRad = 0.2f;

    private PolygonSimplifier _polygonSimplifier = new PolygonSimplifier();
    private List<Vector2> _intersections;

    public void DetectIntersections()
    {
        _intersections = _polygonSimplifier.Simplify(vertices);
    }
    
    private void OnDrawGizmos()
    {
        if (vertices.Length > 0)
        {
            Utility.DrawPolygon(vertices);
            Utility.DrawVertices(vertices, 0.6f);
        }

        if (_intersections != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < _intersections.Count; i++)
            {
                Gizmos.DrawSphere(_intersections[i], PointRad);
            }
        }
    }

    public void ClearIntersections()
    {
        _intersections = null;
    }
}
