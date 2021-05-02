using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PolygonTriangulation.Tester
{
    [CustomEditor(typeof(IntersectionTesterOfTwoSegments))]
    public class EIntersectionTesterOfTwoSegments : Editor
    {
        private IntersectionTesterOfTwoSegments _tester;

        private void OnEnable()
        {
            _tester = target as IntersectionTesterOfTwoSegments;
        }
        
        private void OnSceneGUI()
        {
            var s = _tester.s1;
            s.a = Utility.PositionHandle(s.a, "A");
            s.b = Utility.PositionHandle(s.b, "B");
            _tester.s1 = s;
            
            s = _tester.s2;
            s.a = Utility.PositionHandle(s.a, "A");
            s.b = Utility.PositionHandle(s.b, "B");
            _tester.s2 = s;
        }
    }
}
