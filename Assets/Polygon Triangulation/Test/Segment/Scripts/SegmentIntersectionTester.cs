using UnityEngine;

namespace PolygonTriangulation.Test {
    public class SegmentIntersectionTester : MonoBehaviour {
        public Framework.Segment s1;
        public Framework.Segment s2;
        public bool editMode;

        private const float PointRadius = 0.3f;

        private void OnDrawGizmos() {
            var intersection = Framework.Segment.Intersect(s1, s2, out var intersectionPoint);
            var lineColor = intersection ? Color.red : Color.green;

            Draw(new Framework.Segment(transform.TransformPoint(s1.p), transform.TransformPoint(s1.q)), lineColor, Color.blue, PointRadius);
            Draw(new Framework.Segment(transform.TransformPoint(s2.p), transform.TransformPoint(s2.q)), lineColor, Color.blue, PointRadius);

            if (intersection) {
                var oldColor = Gizmos.color;
                Gizmos.color = Color.magenta;
                {
                    Gizmos.DrawSphere(transform.TransformPoint(intersectionPoint), PointRadius);
                }
                Gizmos.color = oldColor;
            }
        }

        private void Draw(Framework.Segment s, Color lineColor, Color sphereColor, float sphereRadius = 0.2f) {

            var oldColor = Gizmos.color;
            Gizmos.color = lineColor;
            {
                Gizmos.DrawLine(s.p, s.q);
            }

            Gizmos.color = sphereColor;
            {
                Gizmos.DrawSphere(s.p, sphereRadius);
                Gizmos.DrawSphere(s.q, sphereRadius);
            }
            Gizmos.color = oldColor;
        }
    }
}
