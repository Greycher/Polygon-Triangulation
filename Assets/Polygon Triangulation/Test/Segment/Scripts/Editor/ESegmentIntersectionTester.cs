using UnityEditor;

namespace PolygonTriangulation.Segment.Test {
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
            var s = new Segment(tr.TransformPoint(s1.p), tr.TransformPoint(s1.q));
            s = Utility.PositionHandle(s);
            _tester.s1 = new Segment(tr.InverseTransformPoint(s.p), tr.InverseTransformPoint(s.q));
            
            var s2 = _tester.s2;
            s = new Segment(tr.TransformPoint(s2.p), tr.TransformPoint(s2.q));
            s = Utility.PositionHandle(s);
            _tester.s2 = new Segment(tr.InverseTransformPoint(s.p), tr.InverseTransformPoint(s.q));
        }
    }
}
