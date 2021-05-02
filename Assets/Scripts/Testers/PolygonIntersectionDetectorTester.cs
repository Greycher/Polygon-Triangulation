using PolygonTriangulation;
using PolygonTriangulation.Tester;
using UnityEngine;

public class PolygonIntersectionDetectorTester : MonoBehaviour
{
    public Geometry.Polygon polygon;

    private const float PointRad = 0.2f;

    private BentleyOttmann _bentleyOttmann = new BentleyOttmann();
    private Vector2[] _intersections;

    public void DetectIntersections()
    {
        var vertexCount = polygon.VertexCount;
        var segments = new Geometry.Segment[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            segments[i] = new Geometry.Segment(polygon.vertices[i], polygon.vertices[(i + 1) % vertexCount]);
        }
        _intersections = _bentleyOttmann.DetectIntersections(segments, true);
    }
    
    private void OnDrawGizmos()
    {
        var vertexCount = polygon.VertexCount;
        if (vertexCount == 0) return;

        Utility.DrawPolygon(polygon.vertices);

        if (_intersections != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < _intersections.Length; i++)
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
