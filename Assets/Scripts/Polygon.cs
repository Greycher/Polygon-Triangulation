using UnityEditor;
using UnityEngine;

public class Polygon : MonoBehaviour {
    public Vector2[] vertices = new Vector2[0];
    public bool drawVertex = true;
    public bool drawVertexIndex = true;
    
    private const float EdgeThickness = 0.2f;
    private const float PointRad = 0.3f;
    private Vector3 _vertexLabelOffset = new Vector3(-0.08f, 0.1f, 0);

    public void Centralize() {
        var center = CalculateCenter();
        for (var i = 0; i < vertices.Length; i++) vertices[i] -= center;
        transform.position += (Vector3) center;
    }

    private Vector2 CalculateCenter() {
        var vertexSum = Vector2.zero;
        var vertexCount = vertices.Length;
        for (var i = 0; i < vertexCount; i++) vertexSum += vertices[i];
        return vertexSum / vertexCount;
    }

    private void OnDrawGizmos() {
        var style = new GUIStyle(GUI.skin.label) {fontSize = 24, normal = new GUIStyleState() {textColor = Color.green}};
        var vertexCount = vertices.Length;
        for (var i = 0; i < vertexCount; i++) {
            var vertex = transform.TransformPoint(vertices[i]);
            
            //Draw Edge
            var oldColor = Handles.color;
            Handles.color = Color.black; {
                Handles.DrawLine(vertex, transform.TransformPoint(vertices[(i + 1) % vertexCount]), EdgeThickness);
            }
            Handles.color = oldColor;

            if (drawVertex) {
                oldColor = Handles.color;
                Handles.color = Color.blue; {
                    Handles.DrawSolidDisc(vertex, Vector3.back, PointRad);
                }
                Handles.color = oldColor;
            }

            if (drawVertexIndex) {
                Handles.Label(vertex + _vertexLabelOffset, i.ToString(), style);
            }
        }
    }
}