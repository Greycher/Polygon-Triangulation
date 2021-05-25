using System;
using System.Collections.Generic;
using UnityEngine;

namespace PolygonTriangulation {
    public static class Triangulator {
        public static List<List<Vector2>> Triangulate(List<Vector2> vertices, List<List<Vector2>> holes) {
            return Simplifier.Simplify(vertices, holes);
        }
        
        public static List<List<Vector2>> Triangulate(List<Vector2> vertices) {
            var triangles = new List<List<Vector2>>();
            
            var vertexCount = vertices.Count;
            if (vertexCount == 3) {
                triangles.Add(vertices);
                return triangles;
            }

            bool isDiagonalFound = false;
            for (int vertOneIndex = 0; vertOneIndex < vertexCount - 2; vertOneIndex++) {
                var limit = vertexCount;
                //First index is neighbor with last index
                if (vertOneIndex == 0) limit--;
                for (int vertTwoIndex = vertOneIndex + 2; vertTwoIndex < limit; vertTwoIndex++) {
                    if (IsFormsDiagonal(vertices, vertOneIndex, vertTwoIndex))
                    {
                        var subPoly = new List<Vector2>();
                        for (int i = vertOneIndex; i <= vertTwoIndex; i++) {
                            subPoly.Add(vertices[i]);
                        }

                        var innerTriangles = Triangulate(subPoly);
                        for (int i = 0; i < innerTriangles.Count; i++) {
                            triangles.Add(innerTriangles[i]);
                        }

                        subPoly = new List<Vector2>();
                        var vertIndex = vertTwoIndex;
                        while (vertIndex != vertOneIndex) {
                            subPoly.Add(vertices[vertIndex]);
                            vertIndex = (vertIndex + 1) % vertexCount;
                        }
                        subPoly.Add(vertices[vertOneIndex]);
                        
                        innerTriangles = Triangulate(subPoly);
                        for (int i = 0; i < innerTriangles.Count; i++) {
                            triangles.Add(innerTriangles[i]);
                        }

                        return triangles;
                    }
                }
            }

            throw new Exception("Couldn't find diagonals! Make sure that the given polygon is simple.");
        }
        
        private static bool IsFormsDiagonal(List<Vector2> vertices, int vertOneIndex, int vertTwoIndex)
        {
            var vertexCount = vertices.Count;
            var A = vertices[vertOneIndex];
            var B = vertices[vertTwoIndex];
            var APrev = vertices[vertOneIndex == 0 ? vertexCount - 1 : vertOneIndex - 1];
            var ANext = vertices[(vertOneIndex + 1) % vertexCount];
            
            //if A is convex vert
            if (Triangle.CalculateOrientation(new Triangle(APrev, A, ANext)) == Triangle.TriangleOrientation.CounterClockwise)
            {
                if (Triangle.CalculateOrientation(new Triangle(A, B, APrev)) != Triangle.TriangleOrientation.CounterClockwise ||
                    Triangle.CalculateOrientation(new Triangle(B, A, ANext)) != Triangle.TriangleOrientation.CounterClockwise)
                {
                    return false;
                }
            }
            else
            {
                if (Triangle.CalculateOrientation(new Triangle(ANext, A, B)) == Triangle.TriangleOrientation.CounterClockwise &&
                    Triangle.CalculateOrientation(new Triangle(B, A, APrev)) == Triangle.TriangleOrientation.CounterClockwise)
                {
                    return false;
                }
            }

            for (int i = 0; i < vertexCount; i++)
            {
                var segmentIntersectionType = Segment.Segment.CalculateIntersectionType(new Segment.Segment(A, B), new Segment.Segment(vertices[i], vertices[(i + 1) % vertexCount]));
                if (segmentIntersectionType == Segment.Segment.SegmentIntersectionType.IntersectionOnTheMiddle ||
                    segmentIntersectionType == Segment.Segment.SegmentIntersectionType.IntersectionFromOneEnd) return false;
            }

            return true;
        }
    }
}
