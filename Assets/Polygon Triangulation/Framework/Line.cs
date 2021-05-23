using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line {
    private Vector2 a;
    private Vector2 b;

    public Line(Vector2 a, Vector2 b)
    {
        this.a = a;
        this.b = b;
    }
    
    public static bool TryGetIntersectionPointOfTwoLines(Line l1, Line l2, out Vector2 intersectionPoint) {
        var a = l1.a;
        var b = l1.b;
        var c = l2.a;
        var d = l2.b;
        
        float a1 = b.y - a.y;
        float b1 = a.x - b.x;
        float c1 = a1 * (a.x) + b1 * (a.y);
    
        float a2 = d.y - c.y;
        float b2 = c.x - d.x;
        float c2 = a2 * (c.x) + b2 * (c.y);
    
        float determinant = a1 * b2 - a2 * b1;
    
        if (determinant == 0)
        {
            intersectionPoint = Vector2.zero;
            return false;
        }
            
        float x = (b2 * c1 - b1 * c2) / determinant;
        float y = (a1 * c2 - a2 * c1) / determinant;
        intersectionPoint = new Vector2(x, y);
        return true;
    }
}
