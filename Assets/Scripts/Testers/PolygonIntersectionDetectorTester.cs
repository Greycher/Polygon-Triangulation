using System;
using System.Collections.Generic;
using PolygonTriangulation;
using PolygonTriangulation.Tester;
using UnityEditor;
using UnityEngine;

public class PolygonIntersectionDetectorTester : MonoBehaviour
{
    public Vector2[] vertices;
    public Color[] polygonColors = new Color[1];
    public int maxDecomposingStep = Int32.MaxValue;

    private const float PointRad = 0.2f;

    private PolygonSimplifier _polygonSimplifier = new PolygonSimplifier();
    private List<Vector2[]> _simplifiedPolygons;

    public void SimplifyPolygon()
    {
        ClearSimplification();
        _simplifiedPolygons = _polygonSimplifier.Simplify(vertices, maxDecomposingStep);
    }
    
    private void OnDrawGizmos()
    {
        if (_simplifiedPolygons == null)
        {
            Utility.DrawPolygon(vertices);
            // Utility.DrawVertices(vertices, 0.6f);
            // Utility.DrawVertexLabels(vertices, Color.yellow);
        }
        else
        {
            for (int i = 0; i < _simplifiedPolygons.Count; i++)
            {
                Utility.DrawPolygon(_simplifiedPolygons[i], polygonColors[i % polygonColors.Length]);
                // Utility.DrawVertices(_simplifiedPolygons[i], 0.6f);
            }   
            // Utility.DrawVertexLabels(_simplifiedPolygons[0], Color.yellow);
        }
    }

    public void ClearSimplification()
    {
        _simplifiedPolygons = null;
    }
}
