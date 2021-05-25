using UnityEditor;
using UnityEngine;

namespace PolygonTriangulation.Test {
    [CustomEditor(typeof(SegmentIntersectionTester))]
    public class ESegmentIntersectionTester : Editor {
        private SegmentIntersectionTester _tester;

        private void OnEnable() {
            _tester = target as SegmentIntersectionTester;
        }

        private void OnSceneGUI() {

            if (!_tester.editMode) return;

            var tr = _tester.transform;

            var s1 = _tester.s1;
            var s = new Framework.Segment(tr.TransformPoint(s1.p), tr.TransformPoint(s1.q));
            s = PositionHandle(s);
            _tester.s1 = new Framework.Segment(tr.InverseTransformPoint(s.p), tr.InverseTransformPoint(s.q));

            var s2 = _tester.s2;
            s = new Framework.Segment(tr.TransformPoint(s2.p), tr.TransformPoint(s2.q));
            s = PositionHandle(s);
            _tester.s2 = new Framework.Segment(tr.InverseTransformPoint(s.p), tr.InverseTransformPoint(s.q));
        }

        private Framework.Segment PositionHandle(Framework.Segment s) {
            return PositionHandle(s, "p", "q");
        }

        private Framework.Segment PositionHandle(Framework.Segment s, string pLabel, string qLabel) {
            s.p = Handles.PositionHandle(s.p, Quaternion.identity);
            s.q = Handles.PositionHandle(s.q, Quaternion.identity);

            Handles.Label(s.p, pLabel);
            Handles.Label(s.p, qLabel);

            return s;
        }
    }
}
