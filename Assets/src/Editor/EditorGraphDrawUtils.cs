using UnityEngine;
using UnityEditor;

public class EditorGraphDrawUtils
{

	public static void DrawRect(Vector2 TopLeft, Vector2 BottomRight, Color Fill)
	{
		EditorGUI.DrawRect(new Rect(TopLeft, BottomRight - TopLeft), Fill);
	}

	public static void Line(Vector2 From, Vector2 To, Color InColour, float Thickness = 5.0f)
	{
		Handles.BeginGUI();
		Vector3 FromTangent = new Vector3(0.5f * (From.x + To.x), From.y);
		Vector3 ToTangent = new Vector3(0.5f * (From.x + To.x), To.y);
		Handles.DrawBezier(From, To, FromTangent, ToTangent, InColour, null, Thickness);
		Handles.EndGUI();
	}

}
