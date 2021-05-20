using UnityEngine;

namespace PolygonTriangulation.Segment.Test
{
    public class SegmentIntersectionTester : MonoBehaviour
    {
        public Segment s1;
        public Segment s2;
        public bool editMode;
        
        private const float PointRadius = 0.1f;

        private void OnDrawGizmos()
        {
            bool intersection = Segment.Intersect(s1, s2, out Vector2 intersectionPoint);
            var lineColor = intersection ? Color.red : Color.green;

            Utility.Draw(new Segment(transform.TransformPoint(s1.p), transform.TransformPoint(s1.q)), lineColor, PointRadius);
            Utility.Draw(new Segment(transform.TransformPoint(s2.p), transform.TransformPoint(s2.q)), lineColor, PointRadius);

            if (intersection) {
                var oldColor = Gizmos.color;
                Gizmos.color = Color.magenta;
                {
                    Gizmos.DrawSphere(intersectionPoint, PointRadius);
                }
                Gizmos.color = oldColor;
            }
        }
    }
}
