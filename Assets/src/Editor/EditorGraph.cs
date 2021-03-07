using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class EditorGraph : ScriptableObject {

	public delegate void EditorGraphEvent();
	public event EditorGraphEvent OnGraphChanged;

	[SerializeField] private List<EditorNode> Nodes;
	[SerializeField] private List<EditorLink> Links;
	[SerializeField] public Vector2 EditorViewportOffset = new Vector2();
	[SerializeField] private int UIDCounter = -1;

	private Dictionary<int, EditorNode> NodeMap;
	private Dictionary<EditorPinIdentifier, EditorPin> PinMap;

	private EditorPinIdentifier SelectedElement;

	public EditorPinIdentifier GetSelectedElementID()
	{
		return SelectedElement;
	}

	public EditorNode GetSelectedNode()
	{
		return GetNodeFromID(SelectedElement.NodeID);
	}

	public void SelectNode(int NodeID)
	{
		SelectedElement.NodeID = NodeID;
		SelectedElement.PinID = -1;
	}

	public void SelectPin(EditorPinIdentifier PinIdentifier)
	{
		SelectedElement = PinIdentifier;
	}

	public void Deselect()
	{
		SelectedElement.NodeID = -1;
		SelectedElement.PinID = -1;
	}

	public int AddNode(EditorNode _Node)
	{
		if (Nodes == null)
		{
			Nodes = new List<EditorNode>();
		}
		if (NodeMap == null)
		{
			NodeMap = new Dictionary<int, EditorNode>();
		}

		NodeMap[_Node.ID] = _Node;
		Nodes.Add(_Node);
		_Node.OnNodeChanged += NotifyGraphChange;
		NotifyGraphChange();
		return _Node.ID;
	}

	public bool RemoveNode(EditorNode _Node)
	{
		if (Nodes == null)
		{
			return false;
		}

		int NodeID = _Node.ID;
		bool bSuccess = Nodes.Remove(_Node);

		if (bSuccess)
		{
			// remove all associated links
			for (int Index = Links.Count - 1; Index >= 0; --Index)
			{
				EditorLink Link = Links[Index];
				if (Link.NodeID_From == NodeID || Link.NodeID_To == NodeID)
				{
					Links.RemoveAt(Index);
				}
			}

			_Node.OnNodeChanged -= NotifyGraphChange;
			NotifyGraphChange();
			return true;
		}
		else
		{
			return false;
		}
	}

	public EditorNode GetNodeFromID(int ID)
	{
		if (NodeMap == null)
		{
			NodeMap = new Dictionary<int, EditorNode>();
		}
		else if (NodeMap.ContainsKey(ID))
		{
			return NodeMap[ID];
		}

		foreach (EditorNode _Node in Nodes)
		{
			if (_Node.ID == ID)
			{
				NodeMap[ID] = _Node;
				return _Node;
			}
		}

		Debug.LogError("Trying to get Node with invalid ID " + ID + ".");
		return null;
	}

	public EditorPin GetPinFromID(EditorPinIdentifier PinIdentifier)
	{
		if (PinMap == null)
		{
			PinMap = new Dictionary<EditorPinIdentifier, EditorPin>();
		}
		else if (PinMap.ContainsKey(PinIdentifier))
		{
			return PinMap[PinIdentifier];
		}

		EditorNode _Node = GetNodeFromID(PinIdentifier.NodeID);
		if (_Node != null)
		{
			EditorPin Pin = _Node.GetPin(PinIdentifier.PinID);
			PinMap[PinIdentifier] = Pin;
			return Pin;
		}

		return null;
	}

	public EditorNode CreateFromFunction(System.Type ClassType, string Methodname, bool bHasOutput = false, bool bHasInput = false)
	{
		return EditorNode.CreateFromFunction(this, ClassType, Methodname, bHasInput, bHasOutput);
	}

	public void LinkPins(EditorPinIdentifier LHSPin, EditorPinIdentifier RHSPin)
	{
		if (Links == null)
		{
			Links = new List<EditorLink>();
		}

		EditorLink NewLink = new EditorLink(LHSPin, RHSPin);
		Links.Add(NewLink);
		NotifyGraphChange();
	}

	public List<EditorNode> GetNodeList()
	{
		if (Nodes == null)
		{
			Nodes = new List<EditorNode>();
		}
		return Nodes;
	}

	public List<EditorLink> GetLinkList()
	{
		if (Links == null)
		{
			Links = new List<EditorLink>();
		}
		return Links;
	}

	public void NotifyGraphChange()
	{
		if (OnGraphChanged != null)
		{
			OnGraphChanged();
		}
	}

	public void RenderGraph()
	{
		foreach (EditorNode Node in Nodes)
		{
			Node.RenderNode(this, IsNodeSelected() && Node == GetNodeFromID(SelectedElement.NodeID));
		}

		foreach (EditorLink Link in Links)
		{
			Link.RenderLink(this);
		}
	}

	public bool IsPinSelected()
	{
		return SelectedElement.PinID != -1;
	}

	public bool IsNodeSelected()
	{
		return SelectedElement.NodeID != -1 && SelectedElement.PinID == -1;
	}

	public int GenerateUniqueNodeID()
	{
		return ++UIDCounter;
	}

}
