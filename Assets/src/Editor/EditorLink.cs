using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [Serializable]
    public class EditorLink
    {
	    [SerializeField]
	    private int _fromNodeID;
    	[SerializeField]
        private int _fromPinID;
    	[SerializeField]
        private int _toNodeID;
    	[SerializeField]
        private int _toPinID;

        public int FromNodeID => _fromNodeID;
        public int ToNodeID => _toNodeID;
        public int FromPinID => _fromPinID;
        public int ToPinID => _toPinID;

    	public EditorLink(EditorPinIdentifier lhsPin, EditorPinIdentifier rhsPin)
    	{
    		_fromNodeID = lhsPin.NodeID;
    		_fromPinID = lhsPin.PinID;
    		_toNodeID = rhsPin.NodeID;
    		_toPinID = rhsPin.PinID;
    	}

        public void RenderLink(EditorGraph graph)
    	{
    		EditorNode fromNode = graph.GetNodeFromID(FromNodeID);
    		EditorNode toNode = graph.GetNodeFromID(ToNodeID);

    		if (fromNode == null || toNode == null)
    		{
    			return;
    		}

    		Rect fromRect = fromNode.GetPinRect(FromPinID);
    		Rect toRect = toNode.GetPinRect(ToPinID);

    		EditorGraphDrawUtils.Line(fromRect.center, toRect.center, Color.black);
    	}

        public override string ToString()
        {
	        return $"{FromNodeID}.{FromPinID} to {ToNodeID}.{ToPinID}";
        }
    }
}
