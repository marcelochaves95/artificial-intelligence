using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
	[Serializable]
	public class EditorGraph : ScriptableObject
	{
		public Action OnGraphChanged;

		[SerializeField]
		private List<EditorNode> _nodes;
		[SerializeField]
		private List<EditorLink> _links;
		[SerializeField]
		public Vector2 _editorViewportOffset = new Vector2();
		[SerializeField]
		private int _uniqueNodeIDCounter = -1;

		private Dictionary<int, EditorNode> _nodeMap;
		private Dictionary<EditorPinIdentifier, EditorPin> _pinMap;
		private EditorPinIdentifier _selectedElement;

		public EditorPinIdentifier GetSelectedElementID()
		{
			return _selectedElement;
		}

		public EditorNode GetSelectedNode()
		{
			return GetNodeFromID(_selectedElement.NodeID);
		}

		public void SelectNode(int nodeID)
		{
			_selectedElement.NodeID = nodeID;
			_selectedElement.PinID = -1;
		}

		public void SelectPin(EditorPinIdentifier pinIdentifier)
		{
			_selectedElement = pinIdentifier;
		}

		public void Deselect()
		{
			_selectedElement.NodeID = -1;
			_selectedElement.PinID = -1;
		}

		public void AddNode(EditorNode node, out int nodeID)
		{
			if (_nodes == null)
			{
				_nodes = new List<EditorNode>();
			}

			if (_nodeMap == null)
			{
				_nodeMap = new Dictionary<int, EditorNode>();
			}

			_nodeMap[node.ID] = node;
			_nodes.Add(node);
			node.OnNodeChanged += NotifyGraphChange;
			NotifyGraphChange();
			nodeID = node.ID;
		}

		public bool RemoveNode(EditorNode node)
		{
			if (_nodes == null)
			{
				return false;
			}

			int nodeID = node.ID;
			bool isRemoved = _nodes.Remove(node);

			if (isRemoved)
			{
				// Remove all associated links
				for (int i = _links.Count - 1; i >= 0; --i)
				{
					EditorLink link = _links[i];
					if (link.FromNodeID == nodeID || link.ToNodeID == nodeID)
					{
						_links.RemoveAt(i);
					}
				}

				node.OnNodeChanged -= NotifyGraphChange;
				NotifyGraphChange();
				return true;
			}
			else
			{
				return false;
			}
		}

		public EditorNode GetNodeFromID(int id)
		{
			if (_nodeMap == null)
			{
				_nodeMap = new Dictionary<int, EditorNode>();
			}
			else if (_nodeMap.ContainsKey(id))
			{
				return _nodeMap[id];
			}

			for (int i = 0; i < _nodes.Count; i++)
			{
				EditorNode node = _nodes[i];
				if (node.ID == id)
				{
					_nodeMap[id] = node;
					return node;
				}
			}

			Debug.LogError($"Trying to get Node with invalid ID {id}.");
			return null;
		}

		public EditorPin GetPinFromID(EditorPinIdentifier pinId)
		{
			if (_pinMap == null)
			{
				_pinMap = new Dictionary<EditorPinIdentifier, EditorPin>();
			}
			else if (_pinMap.ContainsKey(pinId))
			{
				return _pinMap[pinId];
			}

			EditorNode node = GetNodeFromID(pinId.NodeID);
			if (node != null)
			{
				EditorPin pin = node.GetPin(pinId.PinID);
				_pinMap[pinId] = pin;
				return pin;
			}

			return null;
		}

		public EditorNode CreateFromFunction(Type type, string name, bool hasOutput = false, bool hasInput = false)
		{
			return EditorNode.CreateFromFunction(this, type, name, hasInput, hasOutput);
		}

		public void LinkPins(EditorPinIdentifier lhsPin, EditorPinIdentifier rhsPin)
		{
			if (_links == null)
			{
				_links = new List<EditorLink>();
			}

			EditorLink newLink = new EditorLink(lhsPin, rhsPin);
			_links.Add(newLink);
			NotifyGraphChange();
		}

		public List<EditorNode> GetNodeList()
		{
			if (_nodes == null)
			{
				_nodes = new List<EditorNode>();
			}
			return _nodes;
		}

		public List<EditorLink> GetLinkList()
		{
			if (_links == null)
			{
				_links = new List<EditorLink>();
			}
			return _links;
		}

		public void NotifyGraphChange()
		{
			OnGraphChanged?.Invoke();
		}

		public void RenderGraph()
		{
			for (int i = 0; i < _nodes.Count; i++)
			{
				EditorNode node = _nodes[i];
				node.RenderNode(this, IsNodeSelected() && node == GetNodeFromID(_selectedElement.NodeID));
			}

			for (int i = 0; i < _links.Count; i++)
			{
				EditorLink link = _links[i];
				link.RenderLink(this);
			}
		}

		public bool IsPinSelected()
		{
			return _selectedElement.PinID != -1;
		}

		public bool IsNodeSelected()
		{
			return _selectedElement.NodeID != -1 && _selectedElement.PinID == -1;
		}

		public int GenerateUniqueNodeID()
		{
			return ++_uniqueNodeIDCounter;
		}
	}
}
