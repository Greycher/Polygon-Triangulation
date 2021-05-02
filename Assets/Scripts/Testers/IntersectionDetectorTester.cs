using PolygonTriangulation;
using UnityEngine;

public class IntersectionDetectorTester : MonoBehaviour
{
    public Geometry.Segment[] segments;

    private const float PointRad = 0.2f;

    private BentleyOttmann _bentleyOttmann = new BentleyOttmann();
    private Vector2[] _intersections;

    public void DetectIntersections()
    {
        _intersections = _bentleyOttmann.DetectIntersections(segments);
    }
    
    private void OnDrawGizmos()
    {
        if (segments.Length < 0) return;

        for (int i = 0; i < segments.Length; i++)
        {
            var segment = segments[i];
            Gizmos.DrawLine(segment.a, segment.b);
            Gizmos.DrawSphere(segment.a, PointRad);
            Gizmos.DrawSphere(segment.b, PointRad);
        }

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
