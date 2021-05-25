using System;
using System.Collections.Generic;
using UnityEngine;

namespace PolygonTriangulation.Framework {
    public static class Simplifier {

        public static List<List<Vector2>> Simplify(List<Vector2> vertices) {
            var polygons = new List<List<Vector2>>();
            var vertexCount = vertices.Count;
            for (var edgeOneIndex = 0; edgeOneIndex < vertices.Count; edgeOneIndex++) {
                var lastEdgeIndex = vertexCount - 1;
                var edgeOne = new Segment(vertices[edgeOneIndex], vertices[(edgeOneIndex + 1) % vertexCount]);
                //First index is neighbor with last index
                if (edgeOneIndex == 0) lastEdgeIndex--;
                for (var edgeTwoIndex = edgeOneIndex + 2; edgeTwoIndex <= lastEdgeIndex; edgeTwoIndex++) {
                    var edgeTwo = new Segment(vertices[edgeTwoIndex], vertices[(edgeTwoIndex + 1) % vertexCount]);
                    if (Segment.Intersect(edgeOne, edgeTwo, out var intersectionPoint)) {
                        var subPolygonOne = new List<Vector2>();
                        for (var i = 0; i <= edgeOneIndex; i++) {
                            subPolygonOne.Add(vertices[i]);
                        }
                        subPolygonOne.Add(intersectionPoint);
                        for (var i = edgeTwoIndex + 1; i < vertexCount; i++) {
                            subPolygonOne.Add(vertices[i]);
                        }

                        var subPolygonTwo = new List<Vector2>();
                        subPolygonTwo.Add(intersectionPoint);
                        for (var i = edgeOneIndex + 1; i <= edgeTwoIndex; i++) {
                            subPolygonTwo.Add(vertices[i]);
                        }

                        var simplifiedSubPolygonOne = Simplify(subPolygonOne);
                        var simplifiedSubPolygonTwo = Simplify(subPolygonTwo);

                        for (var i = 0; i < simplifiedSubPolygonOne.Count; i++) {
                            polygons.Add(simplifiedSubPolygonOne[i]);
                        }

                        for (var i = 0; i < simplifiedSubPolygonTwo.Count; i++) {
                            polygons.Add(simplifiedSubPolygonTwo[i]);
                        }

                        return polygons;
                    }
                }
            }

            polygons.Add(vertices);
            return polygons;
        }

        public static List<List<Vector2>> Simplify(List<Vector2> vertices, List<List<Vector2>> holes) {
            var vertexIndexSet = new HashSet<int>();

            for (var holeIndex = holes.Count - 1; holeIndex >= 0; holeIndex--) {

                var holeVertexIndex = 0;
                if (!TryFindASurgerySegment(vertices, holes, holeIndex, ref holeVertexIndex, new Segment(), vertexIndexSet, out var surgerySegment1)) continue;

                var removedHole = holes[holeIndex];
                var surgerySegment = new Segment(removedHole[surgerySegment1.holeVertIndex], vertices[surgerySegment1.vertIndex]);
                holeVertexIndex++;
                if (!TryFindASurgerySegment(vertices, holes, holeIndex, ref holeVertexIndex, surgerySegment, vertexIndexSet, out var surgerySegment2)) continue;

                //Create subpolygons
                var vertexCount = vertices.Count;
                var holeVertexCount = removedHole.Count;

                var polygon1 = new List<Vector2>();
                var startIndex = surgerySegment1.vertIndex;
                var endIndex = surgerySegment2.vertIndex;
                var index = startIndex;
                while (index != endIndex) {
                    polygon1.Add(vertices[index]);
                    index = (index + 1) % vertexCount;
                }
                polygon1.Add(vertices[endIndex]);

                startIndex = surgerySegment2.holeVertIndex;
                endIndex = surgerySegment1.holeVertIndex;
                index = startIndex;
                while (index != endIndex) {
                    polygon1.Add(removedHole[index]);
                    if (--index < 0) index += holeVertexCount;
                }
                polygon1.Add(removedHole[endIndex]);

                var polygon2 = new List<Vector2>();
                startIndex = surgerySegment2.vertIndex;
                endIndex = surgerySegment1.vertIndex;
                index = startIndex;
                while (index != endIndex) {
                    polygon2.Add(vertices[index]);
                    index = (index + 1) % vertexCount;
                }
                polygon2.Add(vertices[endIndex]);

                startIndex = surgerySegment1.holeVertIndex;
                endIndex = surgerySegment2.holeVertIndex;
                index = startIndex;
                while (index != endIndex) {
                    polygon2.Add(removedHole[index]);
                    if (--index < 0) index += holeVertexCount;
                }
                polygon2.Add(removedHole[endIndex]);

                holes.RemoveAt(holeIndex);

                var polygon1Holes = new List<List<Vector2>>();
                var polygon2Holes = new List<List<Vector2>>();

                for (var i = 0; i < holes.Count; i++) {
                    var hole = holes[i];
                    if (PolygonInclude(polygon1, hole[0])) polygon1Holes.Add(hole);
                    else polygon2Holes.Add(hole);
                }

                var subPolygons = new List<List<Vector2>>(2);

                if (polygon1Holes.Count > 0) {
                    var innerSubPolygons = Simplify(polygon1, polygon1Holes);
                    for (var i = 0; i < innerSubPolygons.Count; i++) {
                        subPolygons.Add(innerSubPolygons[i]);
                    }
                }
                else subPolygons.Add(polygon1);

                if (polygon2Holes.Count > 0) {
                    var innerSubPolygons = Simplify(polygon2, polygon2Holes);
                    for (var i = 0; i < innerSubPolygons.Count; i++) {
                        subPolygons.Add(innerSubPolygons[i]);
                    }
                }
                else subPolygons.Add(polygon2);

                return subPolygons;
            }

            throw new Exception("Could not find the " + nameof(SurgerySegment) + " couple!");
        }

        private static bool PolygonInclude(List<Vector2> vertices, Vector2 q) {
            var intersectionCount = 0;
            var vertexCount = vertices.Count;
            for (var i = 0; i < vertexCount; i++) {
                var a = vertices[i];
                var b = vertices[(i + 1) % vertexCount];
                if (Mathf.Approximately(a.y, b.y)) continue;
                if (a.y > b.y) {
                    var temp = a;
                    a = b;
                    b = temp;
                }

                if (q.y < a.y || q.y >= b.y) continue;
                if (Triangle.CalculateOrientation(new Triangle(a, b, q)) == Triangle.TriangleOrientation.Clockwise) intersectionCount++;
            }

            return intersectionCount % 2 > 0;
        }

        private static bool TryFindASurgerySegment(List<Vector2> vertices, List<List<Vector2>> holes, int holeIndex, ref int holeVertexIndex,
            Segment otherSurgerySegment, HashSet<int> vertexIndexSet, out SurgerySegment surgerySegment) {
            var holeCount = holes.Count;
            var hole = holes[holeIndex];
            var holeVertexCount = hole.Count;
            for (; holeVertexIndex < holeVertexCount; holeVertexIndex++) {
                var holeVertex = hole[holeVertexIndex];
                var vertexCount = vertices.Count;
                for (var vertIndex = 0; vertIndex < vertexCount; vertIndex++) {
                    if (vertexIndexSet.Contains(vertIndex)) continue;

                    var vertex = vertices[vertIndex];
                    var segment = new Segment(holeVertex, vertex);

                    var intersection = Segment.Intersect(segment, otherSurgerySegment);
                    if (intersection) continue;

                    var nextHoleVertexIndex = (holeVertexIndex + 1) % holeVertexCount;
                    for (var j = 0; j < holeVertexCount - 2; j++) {
                        var index = (nextHoleVertexIndex + j) % holeVertexCount;
                        var comparedSegment = new Segment(hole[index], hole[(index + 1) % holeVertexCount]);
                        if (Segment.Intersect(segment, comparedSegment)) {
                            intersection = true;
                            break;
                        }
                    }

                    if (intersection) continue;

                    for (var j = 0; j < holeCount; j++) {
                        if (j == holeIndex) continue;
                        var comparedHole = holes[j];
                        if (Segment.Intersect(segment, comparedHole)) {
                            intersection = true;
                            break;
                        }
                    }

                    if (intersection) continue;

                    var nextVertexIndex = (vertIndex + 1) % vertexCount;
                    for (var j = 0; j < vertexCount - 2; j++) {
                        var index = (nextVertexIndex + j) % vertexCount;
                        var comparedSegment = new Segment(vertices[index], vertices[(index + 1) % vertexCount]);
                        if (Segment.Intersect(segment, comparedSegment)) {
                            intersection = true;
                            break;
                        }
                    }

                    if (!intersection) {
                        // Debug.DrawLine(segment.p, segment.q, Color.red, 10);
                        vertexIndexSet.Add(vertIndex);
                        surgerySegment = new SurgerySegment(segment, holeVertexIndex, vertIndex);
                        return true;
                    }
                }
            }

            surgerySegment = new SurgerySegment();
            return false;
        }

        private struct SurgerySegment {
            public readonly int holeVertIndex;
            public readonly int vertIndex;

            public SurgerySegment(Segment segment, int holeVertIndex, int vertIndex) {
                this.holeVertIndex = holeVertIndex;
                this.vertIndex = vertIndex;
            }
        }
    }
}
