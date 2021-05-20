using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Triangle {
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;

    public Triangle(Vector3 a, Vector3 b, Vector3 c) {
        this.a = a;
        this.b = b;
        this.c = c;
    }
    
    public static TriangleOrientation CalculateOrientation(Triangle tri) {
        var a = tri.a;
        var b = tri.b;
        var c = tri.c;
        var value = (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);
        if (value < 0) return TriangleOrientation.Clockwise;
        if (value > 0) return TriangleOrientation.CounterClockwise;
        return TriangleOrientation.CoLinear;
    }
    
    public enum TriangleOrientation
    {
        Clockwise,
        CounterClockwise,
        CoLinear
    }
}
