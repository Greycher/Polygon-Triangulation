using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PolygonTriangulation.Test {
    public class PolygonWithHoles : MonoBehaviour {
        public Polygon polygon;
        public List<Polygon> holes;

        [HideInInspector] public Polygon.PolygonMode polygonMode = Polygon.PolygonMode.Default;
        [HideInInspector] public List<Polygon> simplifiedPolygons = new List<Polygon>();

        public void Centralize() {
            polygon.Centralize();
            var polyTr = polygon.transform;
            var diff = polyTr.localPosition;
            polyTr.localPosition = Vector3.zero;
            for (int i = 0; i < holes.Count; i++) {
                var hole = holes[i];
                hole.Centralize();
                hole.transform.localPosition -= diff;
            }
        }

        public void Simplify() {
            while (simplifiedPolygons.Count > 0) {
                var simplifiedPolygon = simplifiedPolygons[0];
                DestroyImmediate(simplifiedPolygon.gameObject);
                simplifiedPolygons.RemoveAt(0);
            }

            var vertices = GetVertices();
            var holes = GetHoles();
            var newSimplifiedPolygons = Simplifier.Simplify(vertices, holes);
            var polygonTr = polygon.transform;
            var polygonPos = polygonTr.position;
            var polygonRot = polygonTr.rotation;
            for (int i = 0; i < newSimplifiedPolygons.Count; i++) {
                var simplifiedPolygon = newSimplifiedPolygons[i];
                var newPolygon = Polygon.NewPolygon(simplifiedPolygon, polygonPos, polygonRot, transform, nameof(Polygon) + " " + i);
                newPolygon.Centralize();
                simplifiedPolygons.Add(newPolygon);
            }

            polygon.gameObject.SetActive(false);
            for (int i = 0; i < this.holes.Count; i++) {
                this.holes[i].gameObject.SetActive(false);
            }
            polygonMode = Polygon.PolygonMode.Simplified;
        }

        public void Triangulate() {
            var holes = GetHoles();
            Triangulator.Triangulate(polygon.vertices, holes);
        }

        public void ToDefault() {
            if (polygonMode == Polygon.PolygonMode.Simplified) {
                for (int i = 0; i < simplifiedPolygons.Count; i++) {
                    simplifiedPolygons[i].gameObject.SetActive(false);
                }
            }
            
            polygon.gameObject.SetActive(true);
            for (int i = 0; i < holes.Count; i++) {
                holes[i].gameObject.SetActive(true);
            }

            polygonMode = Polygon.PolygonMode.Default;
        }

        private List<List<Vector2>> GetHoles() {
            var fillHoles = new List<List<Vector2>>();
            for (int i = 0; i < holes.Count; i++) {
                var hole = holes[i];
                var holeTr = hole.transform;
                var fillHole = new List<Vector2>();
                for (int j = 0; j < hole.vertices.Count; j++) {
                    fillHole.Add(transform.InverseTransformPoint(holeTr.TransformPoint(hole.vertices[j])));
                }
                fillHoles.Add(fillHole);
            }

            return fillHoles;
        }
        
        private List<Vector2> GetVertices() {
            var fillPolygon = new List<Vector2>();
            var polygonTr = polygon.transform;
            for (int i = 0; i < polygon.vertices.Count; i++) {
                fillPolygon.Add(transform.InverseTransformPoint(polygonTr.TransformPoint(polygon.vertices[i])));
            }
            return fillPolygon;
        }

        public void Load(PolygonWithHolesSetup setup) {
            DestroyImmediate(polygon.gameObject);
            polygon = Polygon.NewPolygon(setup.vertices, setup.polygonPosition, setup.polygonRotation, transform);

            while (holes.Count > 0) {
                var hole = holes[0];
                DestroyImmediate(hole.gameObject);
                holes.RemoveAt(0);
            }
            
            while (holes.Count < setup.holes.Count) {
                var index = holes.Count;
                var name = String.Format("Hole ({0})", index);
                var hole = Polygon.NewPolygon(setup.holes[index].vertices, setup.holePositions[index], setup.holeRotations[index], transform, name);
                hole.drawVertex = false;
                hole.drawVertexIndex = false;
                holes.Add(hole);
            }
        }
    }
}
