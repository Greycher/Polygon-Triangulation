using System.Collections.Generic;
using PolygonTriangulation;
using UnityEngine;

public class PolygonSimplifier
{
    private SweepLine _sweepLine = new SweepLine();
    private PointList _pointList = new PointList();
    private List<Vector2[]> _polygons = new List<Vector2[]>();
    private List<Vector2> _decomposedPolygonOne = new List<Vector2>();
    private List<Vector2> _decomposedPolygonTwo = new List<Vector2>();

    public List<Vector2[]> Simplify(Vector2[] vertices, int maxDecomposingStep)
    {
        _polygons.Clear();
        return InternalSimplify(vertices, maxDecomposingStep);
    }

    private List<Vector2[]> InternalSimplify(Vector2[] vertices, int maxDecomposingStep)
    {
        if (maxDecomposingStep == 0)
        {
            _polygons.Add(vertices);
            return _polygons;
        }
        
        Prepare(vertices);

        _pointList.Sort();

        while (_pointList.TryGetNextPoint(out Point point))
        {
            switch (point.pointType)
            {
                case PointType.LeftEndpoint:
                {
                    _sweepLine.UpdateYPositions(point);
                    _sweepLine.Sort();
                    var index = _sweepLine.Add(point);
                    if (HandleIntersection(index, out Point intersectionPoint))
                    {
                        DecomposeNonSimplePolygon(vertices, intersectionPoint);
                        maxDecomposingStep--;
                        var p1 = _decomposedPolygonOne.ToArray();
                        var p2 = _decomposedPolygonTwo.ToArray();
                        if (p1.Length > 3) InternalSimplify(p1, maxDecomposingStep);
                        else _polygons.Add(p1);
                        if (p2.Length > 3) InternalSimplify(p2, maxDecomposingStep);
                        else _polygons.Add(p2);
                        return _polygons;
                    }
                    break;
                }

                case PointType.RightEndpoint:
                {
                    _sweepLine.UpdateYPositions(point);
                    _sweepLine.Sort();
                    var index = _sweepLine.RemoveFromSweepLine(point);
                    if (index < _sweepLine.PointCount)
                    {
                        if (HandleIntersection(index, out Point intersectionPoint))
                        {
                            DecomposeNonSimplePolygon(vertices, intersectionPoint);
                            maxDecomposingStep--;
                            var p1 = _decomposedPolygonOne.ToArray();
                            var p2 = _decomposedPolygonOne.ToArray();
                            if (p1.Length > 3) InternalSimplify(p1, maxDecomposingStep);
                            else _polygons.Add(p1);
                            if (p2.Length > 3) InternalSimplify(p2, maxDecomposingStep);
                            else _polygons.Add(p2);
                            return _polygons;
                        }
                    }
                    break;
                }
            }
        }
        
        _polygons.Add(vertices);
        return _polygons;
    }

    private void DecomposeNonSimplePolygon(Vector2[] vertices, Point intersectionPoint)
    {
        _decomposedPolygonOne.Clear();
        _decomposedPolygonTwo.Clear();
        
        var s1Index = intersectionPoint.s1Index;
        var s2Index = intersectionPoint.s2Index;
        if (s2Index < s1Index)
        {
            var temp = s1Index;
            s1Index = s2Index;
            s2Index = temp;
        }

        for (int i = 0; i <= s1Index; i++)
        {
            _decomposedPolygonOne.Add(vertices[i]);
        }
        _decomposedPolygonOne.Add(intersectionPoint.position);
        for (int i = s2Index + 1; i < vertices.Length; i++)
        {
            _decomposedPolygonOne.Add(vertices[i]);
        }
        
        _decomposedPolygonTwo.Add(intersectionPoint.position);
        for (int i = s1Index + 1; i <= s2Index; i++)
        {
            _decomposedPolygonTwo.Add(vertices[i]);
        }
    }

    private bool HandleIntersection(int index, out Point intersectionPoint)
    {
        return _sweepLine.CheckForIntersection(index, out intersectionPoint);
    }

    private void Prepare(Vector2[] vertices)
    {
        _sweepLine.Prepare(vertices);
        _pointList.Prepare(vertices);
    }
    
    private class PointList
    {
        List<Point> _points = new List<Point>();
        private int _pointIndex;

        public void Prepare(Vector2[] vertices)
        {
            _pointIndex = -1;
            _points.Clear();
            
            var vertexCount = vertices.Length;
            for (int i = 0; i < vertexCount; i++)
            {
                var a = vertices[i];
                var b = vertices[(i+1) % vertexCount];
                var isASmaller = Smaller(a, b);
                _points.Add(new Point(isASmaller ? PointType.LeftEndpoint : PointType.RightEndpoint, a, i));
                _points.Add(new Point(!isASmaller ? PointType.LeftEndpoint : PointType.RightEndpoint, b, i));
            }
        }
        
        private bool Smaller(Vector2 a, Vector2 b)
        {
            return a.x < b.x || (Mathf.Approximately(a.x, b.x) && a.y < b.y);
        }

        public void Sort()
        {
            CommonAlgorithms.QuickSort(_points, 0, _points.Count - 1, Smaller);
        }
        
        private bool Smaller(Point p, Point q)
        {
            return Smaller(p.position, q.position);
        }
        
        public bool TryGetNextPoint(out Point point)
        {
            if (++_pointIndex < _points.Count)
            {
                point = _points[_pointIndex];
                return true;
            }

            point = new Point();
            return false;
        }
    }

    private class SweepLine
    {
        private Vector2[] _vertices;
        List<SweepLinePoint> _sweepLine = new List<SweepLinePoint>();
        private int[,] _intersectionCheckMatrix;

        public int PointCount => _sweepLine.Count;

        public void Prepare(Vector2[] vertices)
        {
            _sweepLine.Clear();
            _vertices = vertices;
            var vertexCount = _vertices.Length;
            _intersectionCheckMatrix = new int[vertexCount, vertexCount];
        }
        
        public void UpdateYPositions(Point point)
        {
            var sweepLineCount = _sweepLine.Count;
            if (sweepLineCount == 0) return;
        
            var line1 = new Geometry.Segment(new Vector2(point.position.x, 1), new Vector2(point.position.x, -1));
            for (int i = 0; i < sweepLineCount; i++)
            {
                var sweepLinePoint = _sweepLine[i];
                var segmentIndex = sweepLinePoint.segmentIndex;
                var line2 = new Geometry.Segment(_vertices[segmentIndex], _vertices[(segmentIndex + 1) % _vertices.Length]);
                if (Geometry.TryGetIntersectionPointOfTwoLines(line1.a, line1.b, line2.a, line2.b, out Vector2 intersectionPoint))
                {
                    sweepLinePoint.y = intersectionPoint.y;
                    _sweepLine[i] = sweepLinePoint;
                }
            }
        }

        public void Sort()
        {
            CommonAlgorithms.QuickSort(_sweepLine, 0, _sweepLine.Count - 1, Smaller);
        }
        
        private bool Smaller(SweepLinePoint p, SweepLinePoint q)
        {
            return p.y < q.y;
        }

        public int Add(Point point)
        {
            var newSL = new SweepLinePoint(point.position.y, point.s1Index);
            var index = CommonAlgorithms.BinarySearchForIndex(_sweepLine, 0, _sweepLine.Count - 1, newSL, Equals, Smaller);
            _sweepLine.Insert(index, newSL);
            return index;
        }
        
        private bool Equals(SweepLinePoint p, SweepLinePoint q)
        {
            return Mathf.Approximately(p.y, q.y);
        }

        public bool CheckForIntersection(int index, out Point intersectionPoint)
        {
            var sweepLineCount = _sweepLine.Count;
            intersectionPoint = new Point();
            if (sweepLineCount < 2) return false;
        
            var s1Index = _sweepLine[index].segmentIndex;
            var lowerLimit = 0;
            while (index > lowerLimit)
            {
                var s2Index = _sweepLine[index - lowerLimit - 1].segmentIndex;
                if (IsSegmentsAreNeighbors(s1Index, s2Index))
                {
                    lowerLimit++;
                    continue;
                }
            
                if(Intersects(s1Index, s2Index, out Vector2 intersectionPos))
                {
                    intersectionPoint.pointType = PointType.Intersection;
                    intersectionPoint.s1Index = s1Index;
                    intersectionPoint.s2Index = s2Index;
                    intersectionPoint.position = intersectionPos;
                    return true;
                }
                break;
            }
        
            var upperLimit = sweepLineCount - 1;
            while (index < upperLimit)
            {
                var s2Index = _sweepLine[index + (sweepLineCount - upperLimit)].segmentIndex;
                if (IsSegmentsAreNeighbors(s1Index, s2Index))
                {
                    upperLimit--;
                    continue;
                }

                if(Intersects(s1Index, s2Index, out Vector2 intersectionPos))
                {
                    intersectionPoint.pointType = PointType.Intersection;
                    intersectionPoint.s1Index = s1Index;
                    intersectionPoint.s2Index = s2Index;
                    intersectionPoint.position = intersectionPos;
                    return true;
                }
                break;
            }

            return false;
        }

        private bool IsSegmentsAreNeighbors(int s1Index, int s2Index)
        {
            var vertexCount = _vertices.Length;
            var nextSegment = (s1Index + 1) % vertexCount;
            var prevSegment = s1Index - 1;
            if (prevSegment < 0) prevSegment = vertexCount - 1;
            return s2Index == nextSegment || s2Index == prevSegment;
        }

        private bool Intersects(int s1Index, int s2Index, out Vector2 intersectionPos)
        {
            if (_intersectionCheckMatrix[s1Index, s2Index] == 0)
            {
                _intersectionCheckMatrix[s1Index, s2Index] = 1;
                _intersectionCheckMatrix[s2Index, s1Index] = 1;
                var vertexCount = _vertices.Length;
                var s1 = new Geometry.Segment(_vertices[s1Index], _vertices[(s1Index + 1) % vertexCount]);
                var s2 = new Geometry.Segment(_vertices[s2Index], _vertices[(s2Index + 1) % vertexCount]);
                return Geometry.TryGetIntersectionPointOfTwoSegments(s1, s2, out intersectionPos);
            }
            
            intersectionPos = Vector2.zero;
            return false;
        }
        
        public int RemoveFromSweepLine(Point point)
        {
            for (int j = 0; j < _sweepLine.Count; j++)
            {
                if (_sweepLine[j].segmentIndex == point.s1Index)
                {
                    _sweepLine.RemoveAt(j);
                    return j;
                }
            }

            return -1;
        }
        
        private struct SweepLinePoint
        {
            public float y;
            public int segmentIndex;

            public SweepLinePoint(float y, int segmentIndex)
            {
                this.y = y;
                this.segmentIndex = segmentIndex;
            }
        }
    }

    public enum PointType
    {
        LeftEndpoint,
        RightEndpoint,
        Intersection
    }
    
    public struct Point
    {
        public PointType pointType;
        public Vector2 position;
        public int s1Index;
        public int s2Index;

        public Point(PointType pointType, Vector2 position, int s1Index, int s2Index = 0)
        {
            this.pointType = pointType;
            this.position = position;
            this.s1Index = s1Index;
            this.s2Index = s2Index;
        }
    }
}
