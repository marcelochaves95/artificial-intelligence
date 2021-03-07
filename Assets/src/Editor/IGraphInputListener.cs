using UnityEngine;

namespace NodeGraph
{
	public interface IGraphInputListener
	{
		void OnMouseDown(int button, Vector2 mousePosition);
		void OnMouseUp(int button, Vector2 mousePosition);
		void OnMouseMove(Vector2 mousePos);
	}
}