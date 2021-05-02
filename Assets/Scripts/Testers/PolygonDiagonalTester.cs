using System;
using System.Collections.Generic;
using UnityEngine;

namespace PolygonTriangulation.Tester
{
    public class PolygonDiagonalTester : MonoBehaviour
    {
        public Transform[] vertices;
        
        private void OnDrawGizmos()
        {
            var vertices = Utility.TransformToVector2(this.vertices);
            Utility.DrawPolygon(vertices);
            Utility.DrawDiagonals(vertices);
            Utility.DrawVertices(vertices);
        }
    }
}
