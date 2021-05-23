using UnityEditor;
using UnityEngine;

namespace PolygonTriangulation.Test {
    [CustomEditor(typeof(Polygon))]
    public class EPolygon : Editor {
        private Polygon _polygon;
        private bool _editMode;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EnsureObjects();
            _editMode = EditorGUILayout.Toggle("Edit Mode", _editMode);
            if (GUILayout.Button("Centralize")) _polygon.Centralize();
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

                if (EditorGUI.EndChangeCheck()) {
                    EditorUtility.SetDirty(target);
                }
            }
        }

        private void EnsureObjects() {
            if (_polygon == null) _polygon = target as Polygon;
        }
    }
}