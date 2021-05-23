using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PolygonTriangulation.Test {
    public class Polygon : MonoBehaviour {
        public List<Vector2> vertices = new List<Vector2>();
        public bool drawVertex = true;
        public bool drawVertexIndex = true;

        private const float EdgeThickness = 0.2f;
        private const float PointRad = 0.3f;
        private Vector3 _vertexLabelOffset = new Vector3(-0.08f, 0.1f, 0);

        public void Centralize() {
            var center = CalculateCenter();
            for (var i = 0; i < vertices.Count; i++) vertices[i] -= center;
            transform.position += (Vector3) center;
        }

        private Vector2 CalculateCenter() {
            var vertexSum = Vector2.zero;
            var vertexCount = vertices.Count;
            for (var i = 0; i < vertexCount; i++) vertexSum += vertices[i];
            return vertexSum / vertexCount;
        }

        private void OnDrawGizmos() {
            var style = new GUIStyle(GUI.skin.label) {fontSize = 24, normal = new GUIStyleState() {textColor = Color.green}};
            var vertexCount = vertices.Count;
            for (var i = 0; i < vertexCount; i++) {
                var vertex = transform.TransformPoint(vertices[i]);

                //Draw Edge
                var oldColor = Handles.color;
                Handles.color = Color.black;
                {
                    Handles.DrawLine(vertex, transform.TransformPoint(vertices[(i + 1) % vertexCount]), EdgeThickness);
                }
                Handles.color = oldColor;

                if (drawVertex) {
                    oldColor = Handles.color;
                    Handles.color = Color.blue;
                    {
                        Handles.DrawSolidDisc(vertex, Vector3.back, PointRad);
                    }
                    Handles.color = oldColor;
                }

                if (drawVertexIndex) {
                    Handles.Label(vertex + _vertexLabelOffset, i.ToString(), style);
                }
            }
        }

        public static Polygon NewPolygon() {
            return NewPolygon(new List<Vector2>(), Vector3.zero, Quaternion.identity, null);
        }

        public static Polygon NewPolygon(List<Vector2> vertices) {
            return NewPolygon(vertices, Vector3.zero, Quaternion.identity, null);
        }

        public static Polygon NewPolygon(List<Vector2> vertices, Vector3 position, Quaternion rotation) {
            return NewPolygon(vertices, position, rotation, null);
        }

        public static Polygon NewPolygon(List<Vector2> vertices, Vector3 position, Quaternion rotation, Transform parent, string name = nameof(Polygon)) {
            var go = new GameObject(name);
            var tr = go.transform;
            if (parent != null) {
                position = parent.InverseTransformPoint(position);
                rotation = Quaternion.Inverse(parent.rotation) * rotation;
                tr.SetParent(parent);
                tr.localPosition = position;
                tr.localRotation = rotation;
            }
            else {
                tr.position = position;
                tr.rotation = rotation;
            }

            var polygon = go.AddComponent<Polygon>();
            for (int i = 0; i < vertices.Count; i++) {
                polygon.vertices.Add(vertices[i]);
            }

            return polygon;
        }

        [Serializable]
        public enum PolygonMode {
            Default,
            Simplified,
            Triangulated
        }
    }
}