using UnityEditor;
using UnityEngine;

namespace PolygonTriangulation.Test {
    [CustomEditor(typeof(Polygon))]
    public class EPolygon : Editor {
        private Polygon _polygon;
        private PolygonSetup _setup;
        private bool _editMode;
        private float _scale = 1;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EnsureObjects();
            _editMode = EditorGUILayout.Toggle("Edit Mode", _editMode);
            if (GUILayout.Button("Centralize")) _polygon.Centralize();

            if (_polygon.polygonMode == Polygon.PolygonMode.Simplified) {
                if (GUILayout.Button("Non-Simplify")) _polygon.ToDefault();
            }
            else {
                if (GUILayout.Button("Simplify")) _polygon.Simplify();
            }

            if (_polygon.polygonMode == Polygon.PolygonMode.Triangulated) {
                if (GUILayout.Button("Non-Triangulate")) _polygon.ToDefault();
            }
            else {
                if (GUILayout.Button("Triangulate")) _polygon.Triangulate();
            }

            if (GUILayout.Button("Reverse Order")) _polygon.ReverseOrder();

            if (GUILayout.Button("Save")) PolygonSetup.Record(_polygon);
            
            _scale = EditorGUILayout.FloatField("Scale", _scale);
            if (GUILayout.Button("Scale")) _polygon.Scale(_scale);

            _setup = (PolygonSetup) EditorGUILayout.ObjectField("Setup", _setup, typeof(PolygonSetup));
            if (GUILayout.Button("Load")) {
                _polygon.ToDefault();
                _polygon.Load(_setup);
            }
        }

        private void OnSceneGUI() {
            if (_editMode) {
                EditorGUI.BeginChangeCheck();
                EnsureObjects();
                var parent = _polygon.transform;
                var vertices = _polygon.vertices;
                var vertexCount = vertices.Count;
                for (var i = 0; i < vertexCount; i++) {
                    vertices[i] = parent.InverseTransformPoint(Handles.PositionHandle(parent.TransformPoint(vertices[i]), Quaternion.identity));
                }

                if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
            }
        }

        private void EnsureObjects() {
            if (_polygon == null) _polygon = target as Polygon;
        }
    }
}
