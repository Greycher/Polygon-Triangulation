using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PolygonTriangulation.Segment.Test {
    public static class Utility 
    {
        public static void Draw(Segment s) {
            Draw(s, Color.green, Color.blue);
        }
        
        public static void Draw(Segment s, Color lineColor) {
            Draw(s, lineColor, Color.blue);
        }
        
        public static void Draw(Segment s, Color lineColor, float sphereRadius) {
            Draw(s, lineColor, Color.blue, sphereRadius);
        }
        
        public static void Draw(Segment s, Color lineColor, Color sphereColor, float sphereRadius = 0.2f) {
            
            var oldColor = Gizmos.color;
            Gizmos.color = lineColor; {
                Gizmos.DrawLine(s.p, s.q);
            }
            
            Gizmos.color = sphereColor; {
                Gizmos.DrawSphere(s.p, sphereRadius);
                Gizmos.DrawSphere(s.q, sphereRadius);
            }
            Gizmos.color = oldColor;
        }

        public static Segment PositionHandle(Segment s) {
            return PositionHandle(s, "p", "q");
        }

        public static Segment PositionHandle(Segment s, string pLabel, string qLabel) {
            s.p = Handles.PositionHandle(s.p, Quaternion.identity);
            s.q = Handles.PositionHandle(s.q, Quaternion.identity);

            Handles.Label(s.p, pLabel);
            Handles.Label(s.p, qLabel);
            
            return s;
        }
    }
}

