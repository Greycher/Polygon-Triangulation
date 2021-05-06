using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static PolygonTriangulation.Geometry;


namespace PolygonTriangulation.Tester
{
    public static class Utility 
    {
        public static Vector2[] TransformToVector2(Transform[] trVertices)
        {
            var vertexCount = trVertices.Length;
            var vertices = new Vector2[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                vertices[i] = trVertices[i].position;
            }

            return vertices;
        }
        
        public static void DrawVertices(Vector2[] vertices, float pointRad = 0.2f)
        {
            var length = vertices.Length;
            for (int i = 0; i < length; i++)
            {
                var orientation = DetermineTriangleOrientation(vertices[i], vertices[(i + 1) % length],
                    vertices[(i + 2) % length]);
                switch (orientation)
                {
                    case Geometry.TriangleOrientation.Clockwise:
                        Gizmos.color = Color.blue;
                        break;
                    case Geometry.TriangleOrientation.CounterClockwise:
                        Gizmos.color = Color.green;
                        break;
                    default:
                        Gizmos.color = Color.red;
                        break;
                }

                Gizmos.DrawSphere(vertices[(i + 2) % length], pointRad);
            }
        }

        public static void DrawPolygon(Vector2[] vertices)
        {
            var length = vertices.Length;
            Gizmos.color = Color.black;
            for (int i = 0; i < length; i++)
            {
                Gizmos.DrawLine(vertices[i], vertices[(i + 1) % length]);
            }
        }
        
        public static void DrawDiagonals(Vector2[] vertices)
        {
            var length = vertices.Length;
            if (length <= 3) return;

            var diagonals = new List<Vector2>();
            var foundDiagonalCount = 0;
            for (int i = 0; i < length; i++)
            {
                var vertOneIndex = i;
                for (int j = 0; j < length - 3; j++)
                {
                    var vertTwoIndex = i + 2 + j;
                    if (vertTwoIndex >= length) break;
                    if (IsFormsDiagonal(vertices, vertOneIndex, vertTwoIndex))
                    {
                        foundDiagonalCount++;
                        diagonals.Add(vertices[vertOneIndex]);
                        diagonals.Add(vertices[vertTwoIndex]);
                    }
                }
            }
            
            // Debug.Log(foundDiagonalCount + " Diagonal found");
        
            Gizmos.color = Color.green;
            var diagonalCount = diagonals.Count;
            for (int i = 0; i < diagonalCount * 0.5; i++)
            {
                Gizmos.DrawLine(diagonals[i * 2], diagonals[i * 2 + 1]);
            }
        }
        
        private static bool IsFormsDiagonal(Vector2[] vertices, int vertOneIndex, int vertTwoIndex)
        {
            var length = vertices.Length;
            var A = vertices[vertOneIndex];
            var B = vertices[vertTwoIndex];
            var APrev = vertices[(vertOneIndex - 1 < 0 ? length - 1 : vertOneIndex - 1) % length];
            var ANext= vertices[(vertOneIndex + 1) % length];
            
            //if A is convex vert
            if (DetermineTriangleOrientation(APrev, A, ANext) == TriangleOrientation.CounterClockwise)
            {
                if (DetermineTriangleOrientation(A, B, APrev) != TriangleOrientation.CounterClockwise || DetermineTriangleOrientation(B, A, ANext) != TriangleOrientation.CounterClockwise)
                {
                    return false;
                }
            }
            else
            {
                if (DetermineTriangleOrientation(ANext, A, B) == TriangleOrientation.CounterClockwise && DetermineTriangleOrientation(B, A, APrev) == TriangleOrientation.CounterClockwise)
                {
                    return false;
                }
            }
            
            for (int i = 0; i < length; i++)
            {
                var segmentIntersectionType = GetSegmentIntersectionType(A, B, vertices[i], vertices[(i + 1) % length]);
                if (segmentIntersectionType == SegmentIntersectionType.IntersectionOnTheMiddle || segmentIntersectionType == SegmentIntersectionType.IntersectionFromOneEnd) return false;
            }

            return true;
        }
        
        public static bool IsPolygonIncludePoint(Vector2[] vertices, Transform point)
        {
            int intersectionCount = 0;
            Vector2 q = point.position;
            var length = vertices.Length;
            for (int i = 0; i < length; i++)
            {
                Vector2 a = vertices[i];
                Vector2 b = vertices[(i + 1) % length];
                if (Mathf.Approximately(a.y, b.y)) continue;
                if (a.y > b.y)
                {
                    var temp = a;
                    a = b;
                    b = temp;
                }

                if (q.y < a.y || q.y >= b.y) continue;
                if (DetermineTriangleOrientation(a, b, q) == TriangleOrientation.Clockwise) intersectionCount++;
            }

            return intersectionCount % 2 > 0;
        }
        
        public static Vector3 PositionHandle(Vector3 vector)
        {
            return PositionHandle(vector, "", 16, Color.blue);
        }
        
        public static Vector3 PositionHandle(Vector3 vector, string name)
        {
            return PositionHandle(vector, name, 16, Color.blue);
        }
        
        public static Vector3 PositionHandle(Vector3 vector, string name, int fontSize)
        {
            return PositionHandle(vector, name, fontSize, Color.blue);
        }

        public static Vector3 PositionHandle(Vector3 vector, string name, int fontSize, Color labelColor)
        {
            var style = new GUIStyle(GUI.skin.label);
            style.fontSize = fontSize;
            style.normal.textColor = labelColor;
            Handles.Label(vector, name, style);
            return Handles.PositionHandle(vector, Quaternion.identity);
        }
    }
}
