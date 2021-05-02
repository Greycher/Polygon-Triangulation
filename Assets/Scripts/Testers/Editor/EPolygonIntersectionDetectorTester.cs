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
    
    private int _vertexCount = 10;
    private float _minVertexDistance = 1;
    private float _maxVertexDistance = 5;

    private void OnEnable()
    {
        _tester = target as PolygonIntersectionDetectorTester;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _pointEditing = EditorGUILayout.Toggle("Point Editing", _pointEditing);
        if (GUILayout.Button("Detect Intersections"))
        {
            _tester.DetectIntersections();
        }
        
        if (GUILayout.Button("Clear Intersections"))
        {
            _tester.ClearIntersections();
        }
        
        // _vertexCount = Mathf.Clamp(EditorGUILayout.IntField("Vertex Count", _vertexCount), 3, int.MaxValue);
        // _minVertexDistance = EditorGUILayout.FloatField("Min Vertex Distance", _minVertexDistance);
        // _maxVertexDistance = EditorGUILayout.FloatField("Min Vertex Distance", _maxVertexDistance);
        // var sumOfVertices = Vector2.zero;
        // if (GUILayout.Button("Random Polygon"))
        // {
        //     Array.Resize(ref _tester.polygon.vertices, _vertexCount);
        //     var polygon = _tester.polygon;
        //     for (int i = 0; i < _vertexCount; i++)
        //     {
        //         var randomDistance = Random.Range(_minVertexDistance, _maxVertexDistance);
        //         if (i == 0)
        //         {
        //             polygon.vertices[0] = Random.insideUnitCircle.normalized * randomDistance;
        //         }
        //         else if (i == 1)
        //         {
        //             polygon.vertices[1] = polygon.vertices[0] + Random.insideUnitCircle.normalized * randomDistance;
        //         }
        //         else
        //         {
        //             var dir = (polygon.vertices[i - 1] - polygon.vertices[i - 2]).normalized;
        //             var rot = Quaternion.AngleAxis(Random.Range(5f, 179.99f), Vector3.left);
        //             polygon.vertices[i] = polygon.vertices[i - 1] + (Vector2)(rot * dir) * randomDistance;
        //         }
        //
        //         sumOfVertices += polygon.vertices[i];
        //     }
        //     
        //     var center = sumOfVertices / _vertexCount;
        //     for (int i = 0; i < _vertexCount; i++)
        //     {
        //         polygon.vertices[i] -= center;
        //     }
        //     _tester.DetectIntersections();
        //     SaveTarget();
        // }
        
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
                var vertexCount = _setup.vertices.Length;
                var vertices = _tester.polygon.vertices;
                Array.Resize(ref vertices, vertexCount);
                for (int i = 0; i < vertexCount; i++)
                {
                    vertices[i] = _setup.vertices[i];
                }
                SaveTarget();
                _tester.DetectIntersections();
            }
        }
    }

    private void OnSceneGUI()
    {
        if (_pointEditing)
        {
            EditorGUI.BeginChangeCheck();
            var polygon = _tester.polygon;
            var vertexCount = polygon.VertexCount;
            for (int i = 0; i < vertexCount; i++)
            {
                var vertex = polygon.vertices[i];
                vertex = Utility.PositionHandle(vertex, i.ToString());
                polygon.vertices[i] = vertex;
            }

            if (EditorGUI.EndChangeCheck())
            {
                SaveTarget();
            }
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
        setup.vertices = _tester.polygon.vertices;
        AssetDatabase.CreateAsset(setup, String.Format("{0}/{1}.asset", _setupSavePath, ++number));
        PlayerPrefs.SetInt("PolygonSetupNo", number);
        Debug.Log("Setup saved with a name of " + number);
    }
}
