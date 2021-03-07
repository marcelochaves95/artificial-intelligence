using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGraphInputHandler
{

	void OnGraphLoaded(EditorGraph graph);
	
	void MoveNode(EditorNode node, Vector2 newPosition);
	bool LinkPins(EditorPinIdentifier pinA, EditorPinIdentifier pinB);

}
