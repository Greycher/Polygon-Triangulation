using UnityEngine;

namespace PolygonTriangulation.Framework {
    public struct Triangle {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;

        public Triangle(Vector2 a, Vector2 b, Vector2 c) {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public static TriangleOrientation CalculateOrientation(Triangle tri) {
            var a = tri.a;
            var b = tri.b;
            var c = tri.c;
            var value = (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);
            if (value < 0) return TriangleOrientation.Clockwise;
            if (value > 0) return TriangleOrientation.CounterClockwise;
            return TriangleOrientation.CoLinear;
        }

        public enum TriangleOrientation {
            Clockwise,
            CounterClockwise,
            CoLinear
        }
    }
}
