using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PolygonTriangulation.Test {
    public class PolygonWithHolesSetup : ScriptableObject {
        public Vector3 polygonPosition;
        public Quaternion polygonRotation;
        public List<Vector2> vertices = new List<Vector2>();
        public List<Vector3> holePositions = new List<Vector3>();
        public List<Quaternion> holeRotations = new List<Quaternion>();
        public List<Hole> holes = new List<Hole>();

        private const string _savePath = "Assets/Polygon Triangulation/Test/Trangulation/Polygons With Holes/Setups";

        public static void Record(PolygonWithHoles polygonWithHoles) {
            var newSetup = ScriptableObject.CreateInstance<PolygonWithHolesSetup>();
            
            var polygon = polygonWithHoles.polygon;
            var polygonTr = polygon.transform;
            newSetup.polygonPosition = polygonTr.position;
            newSetup.polygonRotation = polygonTr.rotation;
            
            var vertices = polygon.vertices;
            for (int i = 0; i < vertices.Count; i++) {
                newSetup.vertices.Add(vertices[i]);
            }

            var holes = polygonWithHoles.holes;
            for (int i = 0; i < holes.Count; i++) {
                var hole = holes[i];
                var holeTr = hole.transform;
                newSetup.holePositions.Add(holeTr.position);
                newSetup.holeRotations.Add(holeTr.rotation);

                var holeVertices = new List<Vector2>();
                for (int j = 0; j < hole.vertices.Count; j++) {
                    holeVertices.Add(hole.vertices[j]);
                }
                newSetup.holes.Add(new Hole(holeVertices));
            }
            
            var number = PlayerPrefs.GetInt(nameof(PolygonWithHoles), 0); {
                AssetDatabase.CreateAsset(newSetup, String.Format("{0}/{1}.asset", _savePath, ++number));
            }
            PlayerPrefs.SetInt(nameof(PolygonWithHoles), number);
            Debug.Log("Setup saved with a name of " + number);
        }
        
        [Serializable]
        public struct Hole {
            public List<Vector2> vertices;

            public Hole(List<Vector2> vertices) {
                this.vertices = vertices;
            }
        }
    }
}
