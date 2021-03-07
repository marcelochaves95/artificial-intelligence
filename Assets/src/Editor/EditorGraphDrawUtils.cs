using UnityEditor;
using UnityEngine;

namespace NodeGraph
{
	public class EditorGraphDrawUtils
	{
		public static void DrawRect(Vector2 topLeft, Vector2 bottomRight, Color color)
		{
			EditorGUI.DrawRect(new Rect(topLeft, bottomRight - topLeft), color);
		}

		public static void Line(Vector2 from, Vector2 to, Color color, float thickness = 5.0f)
		{
			Handles.BeginGUI();
			Vector3 fromTangent = new Vector3(0.5f * (from.x + to.x), from.y);
			Vector3 toTangent = new Vector3(0.5f * (from.x + to.x), to.y);
			Handles.DrawBezier(from, to, fromTangent, toTangent, color, null, thickness);
			Handles.EndGUI();
		}
	}
}