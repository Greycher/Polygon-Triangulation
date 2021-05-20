using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonWithHoles : Polygon {
    public List<Polygon> holes;

    // private CutPoint FindCutPoints() {
    //     var hole = holes[0];
    //     var holeVertCount = holes.Count;
    //     for (int i = 0; i < holeVertCount; i++) {
    //         var holeVert = hole.vertices[i];
    //         var vertexCount = vertices.Length;
    //         for (int j = 0; j < vertexCount; j++) {
    //             
    //         }
    //     }
    // }

    private struct CutPoint {
        public int segmentPolyVertIndex1;
        public int segmentHoleVertIndex1;
        public int segmentPolyVertIndex2;
        public int segmentHoleVertIndex2;
    }
}
