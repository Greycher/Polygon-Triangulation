using System;
using UnityEngine;

namespace PolygonTriangulation
{
    public static class Geometry
    {
        internal static TriangleOrientation DetermineTriangleOrientation(Vector2 a, Vector2 b, Vector3 c)
        {
            var value = (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);
            if (value < 0) return TriangleOrientation.Clockwise;
            if (value > 0) return TriangleOrientation.CounterClockwise;
            return TriangleOrientation.CoLinear;
        }

        public static bool TryGetIntersectionPointOfTwoSegments(Segment s1, Segment s2, out Vector2 intersectionPoint)
        {
            var segmentIntersectionType = GetSegmentIntersectionType(s1, s2);
            if (segmentIntersectionType != SegmentIntersectionType.NoIntersection && segmentIntersectionType != SegmentIntersectionType.Overlapping)
            {
                return TryGetIntersectionPointOfTwoLines(s1.a, s1.b, s2.a, s2.b, out intersectionPoint);
            }
            
            intersectionPoint = Vector2.zero;
            return false;
        }
        
        public static bool TryGetIntersectionPointOfTwoLines(Vector2 A, Vector2 B, Vector2 C, Vector2 D, out Vector2 intersectionPoint)
        {
            float a1 = B.y - A.y;
            float b1 = A.x - B.x;
            float c1 = a1 * (A.x) + b1 * (A.y);
  
            float a2 = D.y - C.y;
            float b2 = C.x - D.x;
            float c2 = a2 * (C.x) + b2 * (C.y);
  
            float determinant = a1 * b2 - a2 * b1;
  
            if (determinant == 0)
            {
                intersectionPoint = Vector2.zero;
                return false;
            }
            
            float x = (b2 * c1 - b1 * c2) / determinant;
            float y = (a1 * c2 - a2 * c1) / determinant;
            intersectionPoint = new Vector2(x, y);
            return true;
        }
        
        public static SegmentIntersectionType GetSegmentIntersectionType(Segment s1, Segment s2)
        {
            return GetSegmentIntersectionType(s1.a, s1.b, s2.a, s2.b);
        }

        public static SegmentIntersectionType GetSegmentIntersectionType(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            var abc = DetermineTriangleOrientation(a, b, c);
            var abd = DetermineTriangleOrientation(a, b, d);
            var cda = DetermineTriangleOrientation(c, d, a);
            var cdb = DetermineTriangleOrientation(c, d, b);
            if (abc != abd && cda != cdb)
            {
                var coLinerCount = 0;
                if (abc == TriangleOrientation.CoLinear) coLinerCount++;
                if (abd == TriangleOrientation.CoLinear) coLinerCount++;
                if (cda == TriangleOrientation.CoLinear) coLinerCount++;
                if (cdb == TriangleOrientation.CoLinear) coLinerCount++;
                switch (coLinerCount)
                {
                    case 0:
                        return SegmentIntersectionType.IntersectionOnTheMiddle;
                    
                    case 1:
                        return SegmentIntersectionType.IntersectionFromOneEnd;
                    
                    case 2:
                        return SegmentIntersectionType.IntersectionFromTwoEnd;
                    
                    case 4:
                        return SegmentIntersectionType.Overlapping;
                }
            }
            
            return SegmentIntersectionType.NoIntersection;
        }
        
        [Serializable]
        public struct Polygon
        {
            public Vector2[] vertices;

            public int VertexCount => vertices.Length;
            
            public Polygon(Vector2[] vertices)
            {
                this.vertices = vertices;
            }
        }
        
        [Serializable]
        public struct Segment
        {
            public Vector2 a;
            public Vector2 b;

            public Segment(Vector2 a, Vector2 b)
            {
                this.a = a;
                this.b = b;
            }
            
            public static bool operator ==(Segment s1, Segment s2)
            {
                return s1.a == s2.a && s1.b == s2.b;
            }

            public static bool operator !=(Segment s1, Segment s2) 
            {
                return s1.a != s2.a || s1.b != s2.b;
            }
        }

        internal enum TriangleOrientation
        {
            Clockwise,
            CounterClockwise,
            CoLinear
        }
        
        public enum SegmentIntersectionType
        {
            NoIntersection,
            IntersectionOnTheMiddle,
            IntersectionFromOneEnd,
            IntersectionFromTwoEnd,
            Overlapping
        }
    }
}

