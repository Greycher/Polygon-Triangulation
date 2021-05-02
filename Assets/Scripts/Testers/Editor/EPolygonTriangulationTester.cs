using System;
using System.Collections.Generic;
using PolygonTriangulation.Tester;
using UnityEditor;
using UnityEngine;
using static PolygonTriangulation.Tester.Utility;

namespace PolygonTriangulation.Tester
{
    [CustomEditor(typeof(PolygonTriangulationTester))]
    public class EPolygonTriangulationTester : Editor
    {
        private int _maxStep = int.MaxValue;
        private bool _polygonFromSaveFoldout;
        private bool _triangulateFromSaveFoldout;
        private bool _saveFoldout;
        private string _setupSavePath = "Assets/PolygonSetups/";
        private PolygonSetup _setup;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            
            _maxStep = EditorGUILayout.IntField("Max Step", _maxStep);

            if (GUILayout.Button("Triangulate"))
            {
                ResetTriangulation();
                Triangulate();
                Save();
            }

            if (GUILayout.Button("Reset"))
            {
                ResetTriangulation();
                Save();
            }
            
            _polygonFromSaveFoldout = EditorGUILayout.Foldout(_polygonFromSaveFoldout, "Re-Create Polygon From Save");
            
            if (_polygonFromSaveFoldout)
            {
                _setup = (PolygonSetup)EditorGUILayout.ObjectField("Polygon Setup", _setup, typeof(PolygonSetup));
                
                if (GUILayout.Button("Polygon From Save"))
                {
                    if(_setup == null) throw new Exception("Setup can not be empty!");
                    SetVertices(_setup.vertices);
                    Save();
                }
            }
            
            _triangulateFromSaveFoldout = EditorGUILayout.Foldout(_triangulateFromSaveFoldout, "Triangulate From Save");
            
            if (_triangulateFromSaveFoldout)
            {
                _setup = (PolygonSetup)EditorGUILayout.ObjectField("Polygon Setup", _setup, typeof(PolygonSetup));
                
                if (GUILayout.Button("Triangulate From Save"))
                {
                    if(_setup == null) throw new Exception("Setup can not be empty!");
                    ResetTriangulation();
                    Triangulate(_setup.vertices);
                    Save();
                }
            }

            _saveFoldout = EditorGUILayout.Foldout(_saveFoldout, "Save Polygon Setup");
            
            if (_saveFoldout)
            {
                _setupSavePath = EditorGUILayout.TextField("Save Path", _setupSavePath);
                
                if (GUILayout.Button("Save"))
                {
                    SavePolygonSetup();
                }
            }
        }
        
        private void ResetTriangulation()
        {
            var tester = target as PolygonTriangulationTester;
            tester.triangles = new List<Vector2>();
            tester.triangulated = false;
            Debug.Log("Reset");
        }
        
        private void Triangulate(Vector2[] vertices)
        {
            var maxStep = _maxStep;
            var tester = target as PolygonTriangulationTester;
            tester.Triangulate(vertices, ref maxStep);
            tester.triangulated = true;
            Debug.Log("Triangulated");
        }

        private void Triangulate()
        {
            var tester = target as PolygonTriangulationTester;
            Triangulate(TransformToVector2(tester.vertices));
        }
        
        private void Save()
        {
            EditorUtility.SetDirty(target);
        }

        private void SetVertices(Vector2[] vertices)
        {
            var vertexCount = vertices.Length;
            SetVertexCount(vertexCount);
            var tester = target as PolygonTriangulationTester;
            for (int i = 0; i < vertexCount; i++)
            {
                tester.vertices[i].position = vertices[i];
            }
        }

        private void SetVertexCount(int count)
        {
            var tester = target as PolygonTriangulationTester;
            var verticesParent = tester.verticesParent;
            var childCount = tester.verticesParent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(verticesParent.GetChild(0).gameObject);
            }

            var vertices = tester.vertices;
            Array.Resize(ref vertices, count);        
                
            for (int i = 0; i < count; i++)
            {
                var vertex = new GameObject("Vertex " + i);
                vertex.transform.SetParent(verticesParent);
                vertices[i] = vertex.transform;
            }
        }
        
        private void SavePolygonSetup()
        {
            var number = PlayerPrefs.GetInt("PolygonSetupNo", 0);
            var tester = target as PolygonTriangulationTester;
            var setup = ScriptableObject.CreateInstance<PolygonSetup>();
            setup.vertices = TransformToVector2(tester.vertices);
            AssetDatabase.CreateAsset(setup, String.Format("{0}/{1}.asset", _setupSavePath, ++number));
            PlayerPrefs.SetInt("PolygonSetupNo", number);
            Debug.Log("Setup saved with a name of " + number);
        }
    }
}
