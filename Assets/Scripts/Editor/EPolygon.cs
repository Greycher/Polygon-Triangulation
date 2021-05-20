using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Polygon))]
public class EPolygon : Editor {
    private Polygon _polygon;
    private bool _editMode;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        _editMode = EditorGUILayout.Toggle("Edit Mode", _editMode);
        if (GUILayout.Button("Centralize")) _polygon.Centralize();
    }

    private void OnSceneGUI() {
        if (_editMode) {
            EnsureObjects();
            var parent = _polygon.transform;
            var vertices = _polygon.vertices;
            var vertexCount = vertices.Length;
            for (var i = 0; i < vertexCount; i++)
                //Draw Handle
                vertices[i] = parent.InverseTransformPoint(Handles.PositionHandle(vertices[i], Quaternion.identity));
        }
    }

    private void EnsureObjects() {
        if (_polygon == null) _polygon = target as Polygon;
    }
}