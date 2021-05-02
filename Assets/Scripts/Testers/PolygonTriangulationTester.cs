using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static PolygonTriangulation.Geometry;
using static PolygonTriangulation.Tester.Utility;

namespace PolygonTriangulation.Tester
{
    public class PolygonTriangulationTester : MonoBehaviour
    {
        public Transform[] vertices;
        public Transform verticesParent;
        
        [HideInInspector] public List<Vector2> triangles = new List<Vector2>();
        [HideInInspector] public bool triangulated;

        public bool IsFormsDiagonal(Vector2[] vertices, int vertOneIndex, int vertTwoIndex)
        {
            var length = vertices.Length;
            var A = vertices[vertOneIndex];
            var B = vertices[vertTwoIndex];
            var APrev = vertices[(vertOneIndex - 1 < 0 ? length - 1 : vertOneIndex - 1) % length];
            var ANext = vertices[(vertOneIndex + 1) % length];

            //if A is convex vert
            if (DetermineTriangleOrientation(APrev, A, ANext) == TriangleOrientation.CounterClockwise)
            {
                if (DetermineTriangleOrientation(A, B, APrev) != TriangleOrientation.CounterClockwise ||
                    DetermineTriangleOrientation(B, A, ANext) != TriangleOrientation.CounterClockwise)
                {
                    return false;
                }
            }
            else
            {
                if (DetermineTriangleOrientation(ANext, A, B) == TriangleOrientation.CounterClockwise &&
                    DetermineTriangleOrientation(B, A, APrev) == TriangleOrientation.CounterClockwise)
                {
                    return false;
                }
            }

            for (int i = 0; i < length; i++)
            {
                var segmentIntersectionType =
                    GetSegmentIntersectionType(A, B, vertices[i], vertices[(i + 1) % length]);
                if (segmentIntersectionType == SegmentIntersectionType.IntersectionOnTheMiddle ||
                    segmentIntersectionType == SegmentIntersectionType.IntersectionFromOneEnd) return false;
            }

            return true;
        }

        public void Triangulate(Vector2[] vertices, ref int maxStep)
        {
            if (maxStep == 0) return;
            var vertexCount = vertices.Length;

            if (vertexCount == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    triangles.Add(vertices[i]);
                }
                maxStep--;
                return;
            }

            bool isDiagonalFound = false;
            for (int i = 0; i < vertexCount; i++)
            {
                var vertOneIndex = i;
                for (int j = 0; j < vertexCount - 3; j++)
                {
                    var vertTwoIndex = i + 2 + j;
                    if (vertTwoIndex >= vertexCount) break;
                    if (IsFormsDiagonal(vertices, vertOneIndex, vertTwoIndex))
                    {
                        var firstPolygonVertCount = j + 3;
                        var firstDivVertices = new Vector2[firstPolygonVertCount];
                        for (int k = 0; k < firstPolygonVertCount; k++)
                        {
                            firstDivVertices[k] = vertices[(vertOneIndex + k) % vertexCount];
                        }

                        Triangulate(firstDivVertices, ref maxStep);

                        var secondPolygonVertCount = vertexCount - (j + 1);
                        var secondDivVertices = new Vector2[secondPolygonVertCount];
                        for (int k = 0; k < secondPolygonVertCount; k++)
                        {
                            secondDivVertices[k] = vertices[(vertTwoIndex + k) % vertexCount];
                        }

                        Triangulate(secondDivVertices, ref maxStep);

                        isDiagonalFound = true;
                        break;
                    }
                }

                if (isDiagonalFound) break;
            }
        }

        private void OnDrawGizmos()
        {
            if (triangulated)
            {
                DrawTriangles();
            }
            else
            {
                var vertices = TransformToVector2(this.vertices);
                DrawPolygon(vertices);
                DrawVertices(vertices);
            }
        }

        private void DrawTriangles()
        {
            var triangleCount = triangles.Count / 3;
            for (int i = 0; i < triangleCount; i++)
            {
                var vertices = new Vector2[3];
                var index = i * 3;
                vertices[0] = triangles[index];
                vertices[1] = triangles[++index];
                vertices[2] = triangles[++index];
                DrawPolygon(vertices);
                DrawVertices(vertices);
            }
        }
    }
}
