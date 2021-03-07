using UnityEngine;

namespace NodeGraph
{
	public interface IGraphInputHandler
	{
		void OnGraphLoaded(EditorGraph graph);
		void MoveNode(EditorNode node, Vector2 newPosition);
		bool LinkPins(EditorPinIdentifier lhsPin, EditorPinIdentifier rhsPin);

	}
}