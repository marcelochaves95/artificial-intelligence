using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EditorLink
{

	[SerializeField] private int FromNodeID;
	[SerializeField] private int FromPinID;
	[SerializeField] private int ToNodeID;
	[SerializeField] private int ToPinID;

	public EditorLink(EditorPinIdentifier LHSPin, EditorPinIdentifier RHSPin)
	{
		FromNodeID = LHSPin.NodeID;
		FromPinID = LHSPin.PinID;
		ToNodeID = RHSPin.NodeID;
		ToPinID = RHSPin.PinID;
	}

	public int NodeID_From
	{
		get
		{
			return FromNodeID;
		}
	}

	public int NodeID_To
	{
		get
		{
			return ToNodeID;
		}
	}

	public int PinID_From
	{
		get
		{
			return FromPinID;
		}
	}

	public int PinID_To
	{
		get
		{
			return ToPinID;
		}
	}

	public override string ToString()
	{
		return NodeID_From + "." + PinID_From + " to " + NodeID_To + "." + PinID_To;
	}

	public void RenderLink(EditorGraph Graph)
	{
		EditorNode FromNode = Graph.GetNodeFromID(NodeID_From);
		EditorNode ToNode = Graph.GetNodeFromID(NodeID_To);

		if (FromNode == null || ToNode == null)
		{
			return;
		}

		Rect FromRect = FromNode.GetPinRect(PinID_From);
		Rect ToRect = ToNode.GetPinRect(PinID_To);
	
		EditorGraphDrawUtils.Line(FromRect.center, ToRect.center, Color.black);
	}

}
