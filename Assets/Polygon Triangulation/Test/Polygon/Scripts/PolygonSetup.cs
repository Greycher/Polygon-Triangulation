using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PolygonTriangulation.Test {
    public class PolygonSetup : ScriptableObject {
        public List<Vector2> vertices = new List<Vector2>();

        private const string _savePath = "Assets/Polygon Triangulation/Test/Polygon/Setups";

        public static void Record(Polygon polygon) {
            var newSetup = CreateInstance<PolygonSetup>();

            var vertices = polygon.vertices;
            for (var i = 0; i < vertices.Count; i++) {
                newSetup.vertices.Add(vertices[i]);
            }

            var number = PlayerPrefs.GetInt(nameof(Polygon), 0);
            {
                AssetDatabase.CreateAsset(newSetup, string.Format("{0}/{1}.asset", _savePath, ++number));
            }
            PlayerPrefs.SetInt(nameof(Polygon), number);
            Debug.Log(nameof(PolygonSetup) + " saved with a name of " + number);
        }
    }
}
