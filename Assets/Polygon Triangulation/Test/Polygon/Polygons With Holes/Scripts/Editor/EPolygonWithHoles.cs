using UnityEditor;
using UnityEngine;

namespace PolygonTriangulation.Test {
    [CustomEditor(typeof(PolygonWithHoles))]
    public class EPolygonWithHoles : Editor {
        private PolygonWithHoles _polygonWithHoles;
        private PolygonWithHolesSetup _setup;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EnsureObjects();

            if (GUILayout.Button("Centralize")) _polygonWithHoles.Centralize();

            if (_polygonWithHoles.polygonMode == Polygon.PolygonMode.Simplified) {
                if (GUILayout.Button("Non-Simplify")) _polygonWithHoles.ToDefault();
            }
            else {
                if (GUILayout.Button("Simplify")) _polygonWithHoles.Simplify();
            }

            if (_polygonWithHoles.polygonMode == Polygon.PolygonMode.Triangulated) {
                if (GUILayout.Button("Non-Triangulate")) _polygonWithHoles.ToDefault();
            }
            else {
                if (GUILayout.Button("Triangulate")) _polygonWithHoles.Triangulate();
            }

            if (GUILayout.Button("Save")) PolygonWithHolesSetup.Record(_polygonWithHoles);

            _setup = (PolygonWithHolesSetup) EditorGUILayout.ObjectField("Setup", _setup, typeof(PolygonWithHolesSetup));
            if (GUILayout.Button("Load")) {
                _polygonWithHoles.Load(_setup);
                _polygonWithHoles.ToDefault();
            }
        }

        private void EnsureObjects() {
            _polygonWithHoles = target as PolygonWithHoles;
        }
    }
}
