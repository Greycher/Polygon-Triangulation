using System;
using UnityEngine;

namespace PolygonTriangulation.Segment {
    [Serializable]
    public struct Segment {
        public Vector2 p;
        public Vector2 q;

        public Segment(Vector2 p, Vector2 q) {
            this.p = p;
            this.q = q;
        }

        public static bool operator ==(Segment s1, Segment s2) {
            return s1.p == s2.p && s1.q == s2.q;
        }

        public static bool operator !=(Segment s1, Segment s2) {
            return s1.p != s2.p || s1.q != s2.q;
        }

        public static bool Intersect(Segment s1, Segment s2, out Vector2 intersectionPoint) {
            var segmentIntersectionType = CalculateIntersectionType(s1, s2);
            if (segmentIntersectionType != SegmentIntersectionType.NoIntersection && segmentIntersectionType != SegmentIntersectionType.Overlapping) {
                return Line.TryGetIntersectionPointOfTwoLines(new Line(s1.p, s1.q), new Line(s2.p, s2.q), out intersectionPoint);
            }

            intersectionPoint = Vector2.zero;
            return false;
        }

        public static SegmentIntersectionType CalculateIntersectionType(Segment s1, Segment s2) {
            var abc = Triangle.CalculateOrientation(new Triangle(s1.p, s1.q, s2.p));
            var abd = Triangle.CalculateOrientation(new Triangle(s1.p, s1.q, s2.q));
            var cda = Triangle.CalculateOrientation(new Triangle(s2.p, s2.q, s1.p));
            var cdb = Triangle.CalculateOrientation(new Triangle(s2.p, s2.q, s1.q));
            if (abc != abd && cda != cdb) {
                var coLinerCount = 0;
                if (abc == Triangle.TriangleOrientation.CoLinear) coLinerCount++;
                if (abd == Triangle.TriangleOrientation.CoLinear) coLinerCount++;
                if (cda == Triangle.TriangleOrientation.CoLinear) coLinerCount++;
                if (cdb == Triangle.TriangleOrientation.CoLinear) coLinerCount++;
                switch (coLinerCount) {
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



        public enum SegmentIntersectionType {
            NoIntersection,
            IntersectionOnTheMiddle,
            IntersectionFromOneEnd,
            IntersectionFromTwoEnd,
            Overlapping
        }
    }
}
