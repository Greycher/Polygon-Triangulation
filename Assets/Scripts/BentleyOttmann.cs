using System.Collections.Generic;
using UnityEngine;
using static PolygonTriangulation.Geometry;

public class BentleyOttmann
{
    Segment[] _segments;
    List<Point> _points = new List<Point>();
    List<SweepLinePoint> _sweepLine = new List<SweepLinePoint>();
    List<Vector2> _intersections = new List<Vector2>();
    private int[,] _intersectionCheckMatrix;
    
    public Vector2[] DetectIntersections(Segment[] segments)
    {
        Reset();

        _segments = segments;
        var segmentCount = segments.Length;
        _intersectionCheckMatrix = new int[segmentCount, segmentCount];
        
        for (int i = 0; i < segmentCount; i++)
        {
            var segment = segments[i];
            var isLeftEntPoint = IsSmall(segment.a, segment.b);
            _points.Add(new Point(isLeftEntPoint ? PointType.LeftEndpoint : PointType.RightEndpoint, segment.a, i));
            _points.Add(new Point(!isLeftEntPoint ? PointType.LeftEndpoint : PointType.RightEndpoint, segment.b, i));
        }
        
        QuickSort(ref _points, 0, _points.Count - 1);
        
        for (int i = 0; i < _points.Count; i++)
        {
            var point = _points[i];

            switch (point.pointType)
            {
                case PointType.LeftEndpoint:
                {
                    UpdateYPositions(point);
                    QuickSort(ref _sweepLine, 0, _sweepLine.Count - 1);
                    AddToSweepLine(point);
                    break;
                }

                case PointType.RightEndpoint:
                {
                    UpdateYPositions(point);
                    QuickSort(ref _sweepLine, 0, _sweepLine.Count - 1);
                    RemoveFromSweepLine(point);
                    break;
                }
                
                case PointType.Intersection:
                {
                    SwapIntersectionPoints(point);
                    break;
                }
            }
        }

        return _intersections.ToArray();
    }

    private void UpdateYPositions(Point point)
    {
        var sweepLineCount = _sweepLine.Count;
        if (sweepLineCount == 0) return;
        
        var line1 = new Segment(new Vector2(point.position.x, 1), new Vector2(point.position.x, -1));
        for (int i = 0; i < sweepLineCount; i++)
        {
            var sweepLinePoint = _sweepLine[i];
            var line2 = _segments[sweepLinePoint.segmentIndex];
            if (TryGetIntersectionPointOfTwoLines(line1.a, line1.b, line2.a, line2.b, out Vector2 intersectionPoint))
            {
                sweepLinePoint.y = intersectionPoint.y;
                _sweepLine[i] = sweepLinePoint;
            }
        }
    }

    private void SwapIntersectionPoints(Point point)
    {
        var sweepLineIndex1 = -1;
        var sweepLineIndex2 = -1;
        for (int j = 0; j < _sweepLine.Count; j++)
        {
            if (sweepLineIndex1 == -1 && _sweepLine[j].segmentIndex == point.s1Index)
            {
                sweepLineIndex1 = j;
                if (sweepLineIndex2 != -1) break;

            }
            else if(sweepLineIndex2 == -1 && _sweepLine[j].segmentIndex == point.s2Index)
            {
                sweepLineIndex2 = j;
                if (sweepLineIndex1 != -1) break;
            }
        }
        
        var temp = _sweepLine[sweepLineIndex1];
        _sweepLine[sweepLineIndex1] = _sweepLine[sweepLineIndex2];
        _sweepLine[sweepLineIndex2] = temp;

        CheckForNeighborSegmentIntersections(sweepLineIndex1);
        CheckForNeighborSegmentIntersections(sweepLineIndex2);
    }

    private void AddToSweepLine(Point point)
    {
        var y = point.position.y;
        var index = SweepLineBinarySearchIndexToInsert(0, _sweepLine.Count - 1, y);
        _sweepLine.Insert(index, new SweepLinePoint(y, point.s1Index));
        CheckForNeighborSegmentIntersections(index);
    }
    
    private void RemoveFromSweepLine(Point point)
    {
        for (int j = 0; j < _sweepLine.Count; j++)
        {
            if (_sweepLine[j].segmentIndex == point.s1Index)
            {
                _sweepLine.RemoveAt(j);
                if (j != _sweepLine.Count)
                {
                    CheckForNeighborSegmentIntersections(j);
                }
                break;
            }
        }
    }

    private void CheckForNeighborSegmentIntersections(int index)
    {
        var sweepLineCount = _sweepLine.Count;
        if (sweepLineCount < 2) return;
        
        var s1Index = _sweepLine[index].segmentIndex;

        if (index > 0)
        {
            var s2Index = _sweepLine[index - 1].segmentIndex;
            
            if (_intersectionCheckMatrix[s1Index, s2Index] == 0)
            {
                _intersectionCheckMatrix[s1Index, s2Index] = 1;
                _intersectionCheckMatrix[s2Index, s1Index] = 1;
                if (TryGetIntersectionPointOfTwoSegments(_segments[s1Index], _segments[s2Index], out Vector2 intersectionPoint))
                {
                    var pointIndex = BinarySearchIndexToInsert(_points, 0, _points.Count - 1, intersectionPoint);
                    _points.Insert(pointIndex, new Point(PointType.Intersection, intersectionPoint, s1Index, s2Index));
                    _intersections.Add(intersectionPoint);
                }
            }
        }
        
        if (index < sweepLineCount - 1)
        {
            var s2Index = _sweepLine[index + 1].segmentIndex;
            if (_intersectionCheckMatrix[s1Index, s2Index] == 0)
            {
                _intersectionCheckMatrix[s1Index, s2Index] = 1;
                _intersectionCheckMatrix[s2Index, s1Index] = 1;
                if (TryGetIntersectionPointOfTwoSegments(_segments[s1Index], _segments[s2Index], out Vector2 intersectionPoint))
                {
                    var pointIndex = BinarySearchIndexToInsert(_points, 0, _points.Count - 1, intersectionPoint);
                    _points.Insert(pointIndex, new Point(PointType.Intersection, intersectionPoint, s1Index, s2Index));
                    _intersections.Add(intersectionPoint);
                }
            }
        }
    }
    
    private void Reset()
    {
        _points.Clear();
        _sweepLine.Clear();
        _intersections.Clear();
    }

    private int BinarySearchIndexToInsert(List<Point> points, int startIndex, int endIndex, Vector2 point)
    {
        if (endIndex >= startIndex) 
        {
            int mid = startIndex + (endIndex - startIndex) / 2;

            var midPoint = points[mid].position;
            if (midPoint == point)
                return mid;
            
            if (IsSmall(point, midPoint))
                return BinarySearchIndexToInsert(points, startIndex, mid - 1, point);
 
            return BinarySearchIndexToInsert(points, mid + 1, endIndex, point);
        }
        
        return startIndex;
    }
    
    private int SweepLineBinarySearchIndexToInsert(int startIndex, int endIndex, float y)
    {
        if (endIndex >= startIndex) 
        {
            int mid = startIndex + (endIndex - startIndex) / 2;

            var midPoint = _sweepLine[mid].y;
            if (midPoint == y)
                return mid + 1;
            
            if (y < midPoint)
                return SweepLineBinarySearchIndexToInsert(startIndex, mid - 1, y);
 
            return SweepLineBinarySearchIndexToInsert(mid + 1, endIndex, y);
        }
        
        return startIndex;
    }
    
    private void Swap(ref List<Point> arr, int firstIndex, int secondIndex)
    {
        var temp = arr[firstIndex];
        arr[firstIndex] = arr[secondIndex];
        arr[secondIndex] = temp;
    }

    private int Partition(ref List<Point> arr, int startIndex, int endIndex)
    {
        var i = startIndex - 1;
        var j = startIndex;
        var pivotIndex = endIndex;
        while (j < pivotIndex)
        {
            if (IsSmall(arr[j].position, arr[pivotIndex].position))
            {
                i += 1;
                Swap(ref arr, i, j);
            }
            j++;
        }

        Swap(ref arr, ++i, j);
        return i;
    }

    private void QuickSort(ref List<Point> arr, int startIndex, int endIndex)
    {
        if (endIndex <= startIndex) return;
        var pi = Partition(ref arr, startIndex, endIndex);
        QuickSort(ref arr, startIndex, pi - 1);
        QuickSort(ref arr, pi + 1, endIndex);
    }
    
    private void Swap(ref List<SweepLinePoint> arr, int firstIndex, int secondIndex)
    {
        var temp = arr[firstIndex];
        arr[firstIndex] = arr[secondIndex];
        arr[secondIndex] = temp;
    }

    private int Partition(ref List<SweepLinePoint> arr, int startIndex, int endIndex)
    {
        var i = startIndex - 1;
        var j = startIndex;
        var pivotIndex = endIndex;
        while (j < pivotIndex)
        {
            if (arr[j].y < arr[pivotIndex].y)
            {
                i += 1;
                Swap(ref arr, i, j);
            }
            j++;
        }

        Swap(ref arr, ++i, j);
        return i;
    }

    private void QuickSort(ref List<SweepLinePoint> arr, int startIndex, int endIndex)
    {
        if (endIndex <= startIndex) return;
        var pi = Partition(ref arr, startIndex, endIndex);
        QuickSort(ref arr, startIndex, pi - 1);
        QuickSort(ref arr, pi + 1, endIndex);
    }

    private bool IsSmall(Vector2 a, Vector2 b)
    {
        return a.x < b.x || (a.x == b.x && a.y < b.y);
    }

    private void PrintPoints(List<Point> points)
    {
        foreach (var point in points)
        {
            Debug.Log(point.position);
        }
    }

    private struct Point
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
    
    private enum PointType
    {
        LeftEndpoint,
        RightEndpoint,
        Intersection
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
