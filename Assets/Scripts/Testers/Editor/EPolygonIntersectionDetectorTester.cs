using System;
using System.Collections;
using System.Collections.Generic;
using PolygonTriangulation;
using PolygonTriangulation.Tester;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CustomEditor(typeof(PolygonIntersectionDetectorTester))]
public class EPolygonIntersectionDetectorTester : Editor
{
    private PolygonIntersectionDetectorTester _tester;
    private bool _pointEditing;
    
    private bool _saveFoldout;
    private string _setupSavePath = "Assets/PolygonSetups/";
    
    private bool _loadFoldout;
    private PolygonSetup _setup;
    
    // private int _vertexCount = 10;
    // private float _minVertexDistance = 1;
    // private float _maxVertexDistance = 5;

    private void OnEnable()
    {
        _tester = target as PolygonIntersectionDetectorTester;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _pointEditing = EditorGUILayout.Toggle("Point Editing", _pointEditing);
        if (GUILayout.Button("Simplify Polygon"))
        {
            _tester.SimplifyPolygon();
        }
        
        if (GUILayout.Button("Clear Simplification"))
        {
            _tester.ClearSimplification();
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
        
        _loadFoldout = EditorGUILayout.Foldout(_loadFoldout, "Load Polygon Setup");

        if (_loadFoldout)
        {
            _setup = (PolygonSetup)EditorGUILayout.ObjectField("Polygon Setup", _setup, typeof(PolygonSetup));
                
            if (GUILayout.Button("Load"))
            {
                if(_setup == null) throw new Exception("Setup can not be empty!");
                var vertexCount = _setup.vertices.Count;
                var vertices = _tester.vertices;
                if (vertices.Length != vertexCount)
                {
                    Array.Resize(ref vertices, vertexCount);
                    _tester.vertices = vertices;
                }
                for (int i = 0; i < vertexCount; i++)
                {
                    vertices[i] = _setup.vertices[i];
                }
                SaveTarget();
                _tester.SimplifyPolygon();
            }
        }
    }

    private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();
        var vertices = _tester.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            var vertex = vertices[i];
            if (_pointEditing) vertex = Handles.PositionHandle(vertex, Quaternion.identity);
            vertices[i] = vertex;
        }

        if (EditorGUI.EndChangeCheck())
        {
            SaveTarget();
        }
    }

    private void SaveTarget()
    {
        EditorUtility.SetDirty(target);
    }
    
    private void SavePolygonSetup()
    {
        var number = PlayerPrefs.GetInt("PolygonSetupNo", 0);
        var setup = ScriptableObject.CreateInstance<PolygonSetup>();
        // setup.vertices = _tester.vertices.Clone();
        AssetDatabase.CreateAsset(setup, String.Format("{0}/{1}.asset", _setupSavePath, ++number));
        PlayerPrefs.SetInt("PolygonSetupNo", number);
        Debug.Log("Setup saved with a name of " + number);
    }
}
