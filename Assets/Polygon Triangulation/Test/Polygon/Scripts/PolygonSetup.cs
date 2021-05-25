using System;
using System.Collections.Generic;
using PolygonTriangulation.Test;
using UnityEditor;
using UnityEngine;

namespace PolygonTriangulation.Tester
{
    public class PolygonSetup : ScriptableObject
    {
        public List<Vector2> vertices = new List<Vector2>();
        
        private const string _savePath = "Assets/Polygon Triangulation/Test/Polygon/Setups";
        
        public static void Record(Polygon polygon) {
            var newSetup = ScriptableObject.CreateInstance<PolygonSetup>();
            
            var vertices = polygon.vertices;
            for (int i = 0; i < vertices.Count; i++) {
                newSetup.vertices.Add(vertices[i]);
            }
            
            var number = PlayerPrefs.GetInt(nameof(Polygon), 0); {
                AssetDatabase.CreateAsset(newSetup, String.Format("{0}/{1}.asset", _savePath, ++number));
            }
            PlayerPrefs.SetInt(nameof(Polygon), number);
            Debug.Log(nameof(PolygonSetup) + " saved with a name of " + number);
        }
    }
}
