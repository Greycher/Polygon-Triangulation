using UnityEngine;
using static PolygonTriangulation.Tester.Utility;

namespace PolygonTriangulation.Tester
{
    public class PolygonPointInclusionTester : MonoBehaviour
    {
        public Transform[] vertices;
        public Transform point;
        
        private void OnDrawGizmos()
        {
            var vertices = TransformToVector2(this.vertices);
            DrawPolygon(vertices);
            DrawVertices(vertices);

            if (point != null)
            {
                Gizmos.color = IsPolygonIncludePoint(vertices, point) ? Color.green : Color.red;
                Gizmos.DrawSphere(point.position, 0.2f);
            }
        }
    }
}
