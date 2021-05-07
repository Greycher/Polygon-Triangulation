using System.Collections.Generic;
using PolygonTriangulation;
using UnityEngine;

public class PolygonSimplifier
{
    private SweepLine _sweepLine = new SweepLine();
    private PointList _pointList = new PointList();
    private List<Vector2> _intersections = new List<Vector2>();

    public List<Vector2> Simplify(Vector2[] vertices)
    {
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
                    HandleIntersection(index);
                    break;
                }

                case PointType.RightEndpoint:
                {
                    _sweepLine.UpdateYPositions(point);
                    _sweepLine.Sort();
                    var index = _sweepLine.RemoveFromSweepLine(point);
                    if (index < _sweepLine.PointCount) HandleIntersection(index);
                    break;
                }
                
                case PointType.Intersection:
                {
                    var index1 = _sweepLine.Find(point.s1Index);
                    var index2 = _sweepLine.Find(point.s2Index);
                    _sweepLine.Swap(index1, index2);
                    HandleIntersection(index1);
                    HandleIntersection(index2);
                    break;
                }
            }
        }

        return _intersections;
    }

    private void HandleIntersection(int index)
    {
        var newIntersections = _sweepLine.CheckForIntersection(_pointList, index);
        for (int i = 0; i < newIntersections.Count; i++)
        {
            _intersections.Add(newIntersections[i]);
        }
    }

    private void Prepare(Vector2[] vertices)
    {
        _intersections.Clear();
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

        public void Add(Point newPoint)
        {
            var pointIndex = CommonAlgorithms.BinarySearchForIndex(_points, 0, _points.Count - 1, newPoint, Equals, Smaller);
            _points.Insert(pointIndex, newPoint);
        }
        
        private bool Equals(Point p, Point q)
        {
            return p.position == q.position;
        }
    }

    private class SweepLine
    {
        private Vector2[] _vertices;
        List<SweepLinePoint> _sweepLine = new List<SweepLinePoint>();
        private int[,] _intersectionCheckMatrix;
        private List<Vector2> _intersections = new List<Vector2>();

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

        public List<Vector2> CheckForIntersection(PointList pointList, int index)
        {
            _intersections.Clear();
            
            var sweepLineCount = _sweepLine.Count;
            if (sweepLineCount < 2) return _intersections;
        
            var s1Index = _sweepLine[index].segmentIndex;
            var lowerLimit = 0;
            while (index > lowerLimit)
            {
                var s2Index = _sweepLine[index - lowerLimit - 1].segmentIndex;
                if (Mathf.Abs(s1Index - s2Index) == 1)
                {
                    lowerLimit++;
                    continue;
                }
            
                Intersects(pointList, s1Index, s2Index);
                break;
            }
        
            var upperLimit = sweepLineCount - 1;
            while (index < upperLimit)
            {
                var s2Index = _sweepLine[index + (sweepLineCount - upperLimit)].segmentIndex;
                if (Mathf.Abs(s1Index - s2Index) == 1)
                {
                    upperLimit--;
                    continue;
                }
            
                Intersects(pointList, s1Index, s2Index);
                break;
            }

            return _intersections;
        }
        
        private void Intersects(PointList pointList, int s1Index, int s2Index)
        {
            if (_intersectionCheckMatrix[s1Index, s2Index] == 0)
            {
                _intersectionCheckMatrix[s1Index, s2Index] = 1;
                _intersectionCheckMatrix[s2Index, s1Index] = 1;
                var vertexCount = _vertices.Length;
                var s1 = new Geometry.Segment(_vertices[s1Index], _vertices[(s1Index + 1) % vertexCount]);
                var s2 = new Geometry.Segment(_vertices[s2Index], _vertices[(s2Index + 1) % vertexCount]);
                if (Geometry.TryGetIntersectionPointOfTwoSegments(s1, s2, out Vector2 intersectionPoint))
                {
                    var newPoint = new Point(PointType.Intersection, intersectionPoint, s1Index, s2Index);
                    pointList.Add(newPoint);
                    _intersections.Add(intersectionPoint);
                }
            }
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

        public int Find(int segmentIndex)
        {
            for (int j = 0; j < _sweepLine.Count; j++)
            {
                if (_sweepLine[j].segmentIndex == segmentIndex) return j;
            }

            return -1;
        }

        public void Swap(int i, int j)
        {
            var temp = _sweepLine[i];
            _sweepLine[i] = _sweepLine[j];
            _sweepLine[j] = temp;
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
