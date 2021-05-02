using UnityEngine;
using static PolygonTriangulation.Geometry;

namespace PolygonTriangulation.Tester
{
    public class IntersectionTesterOfTwoSegments : MonoBehaviour
    {
        public Segment s1;
        public Segment s2;
        
        private const float PointRadius = 0.1f;

        private void OnDrawGizmos()
        {
            bool intersectionExist = TryGetIntersectionPointOfTwoSegments(s1, s2, out Vector2 intersectionPoint);
            Gizmos.color = intersectionExist ? Color.red : Color.green;

            Gizmos.DrawLine(s1.a, s1.b);
            Gizmos.DrawLine(s2.a, s2.b);
            
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(s1.a, PointRadius);
            Gizmos.DrawSphere(s1.b, PointRadius);
            Gizmos.DrawSphere(s2.a, PointRadius);
            Gizmos.DrawSphere(s2.b, PointRadius);

            if (intersectionExist)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(intersectionPoint, PointRadius);
            }
        }
    }
}
