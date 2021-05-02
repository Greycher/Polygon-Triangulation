using System;
using PolygonTriangulation;
using PolygonTriangulation.Tester;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CustomEditor(typeof(IntersectionDetectorTester))]
public class EIntersectionDetectorTester : Editor
{
    private IntersectionDetectorTester _tester;
    private bool _pointEditing;
    private int _segmentCount = 20;
    private float _randomPointRadius = 10;

    private void OnEnable()
    {
        _tester = target as IntersectionDetectorTester;
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
        
        _segmentCount = EditorGUILayout.IntField("Segment Count", _segmentCount);
        _randomPointRadius = EditorGUILayout.FloatField("Random Point Radius", _randomPointRadius);
        if (GUILayout.Button("Random Segments"))
        {
            Array.Resize(ref _tester.segments, _segmentCount);
            for (int i = 0; i < _segmentCount; i++)
            {
                _tester.segments[i] = new Geometry.Segment(Random.insideUnitCircle * _randomPointRadius, Random.insideUnitCircle * _randomPointRadius);
            }
            _tester.DetectIntersections();
            SaveTarget();
        }
    }

    private void OnSceneGUI()
    {
        if (_pointEditing)
        {
            EditorGUI.BeginChangeCheck();
            var segments = _tester.segments;
            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                segment.a = Utility.PositionHandle(segment.a, "A" + i);
                segment.b = Utility.PositionHandle(segment.b, "B" + i);
                segments[i] = segment;
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
}
