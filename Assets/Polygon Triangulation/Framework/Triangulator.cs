using System.Collections.Generic;
using UnityEngine;

namespace PolygonTriangulation {
    public class Triangulator {
        public static void Triangulate(List<Vector2> vertices, List<List<Vector2>> holes) {
            Simplifier.Simplify(vertices, holes);
        }
    }
}
