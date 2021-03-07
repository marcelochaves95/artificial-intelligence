using UnityEngine;

namespace NodeGraph
{
	public interface IGraphInputListener
	{
		void OnMouseDown(int button, Vector2 mousePos);
		void OnMouseUp(int button, Vector2 mousePos);
		void OnMouseMove(Vector2 mousePos);
	}
}