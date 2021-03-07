using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace NodeGraph
{
	[System.Serializable]
	public class EditorNode
	{
		public Action OnNodeChanged;

		[SerializeField]
		private List<EditorPin> _pins;
		[SerializeField]
		private string _name;
		[SerializeField]
		private Vector2 _position;
		[SerializeField]
		private int _id;

		private EditorNodeRenderData _renderData;

		public string Name
		{
			get => _name;
			set => _name = value;
		}

		public Vector2 Position
		{
			get => _position;
			set => _position = value;
		}

		public int ID => _id;

		public int PinCount => _pins.Count;

		public EditorNode()
		{
			Debug.LogError($"EditorNode being constructed without its graph owner as a parameter, unable to generate a unique identifier. (ID = {_id})");
			Init();
		}

		public EditorNode(EditorGraph graph)
		{
			_id = graph.GenerateUniqueNodeID();
			Init();
		}

		private void Init()
		{
			_pins = new List<EditorPin>();
			UpdateNodeRect();
		}

		public EditorPin GetPin(int index)
		{
			if (index < 0 || index >= _pins.Count)
			{
				Debug.LogError($"Attempted to get pin of invalid index {index}, (max = {_pins.Count})");
			}

			return _pins[index];
		}

		public Rect GetNodeRect()
		{
			UpdateNodeRect();
			return _renderData.NodeRect;
		}

		public Rect GetPinRect(int id)
		{
			EditorPin pin = _pins[id];
			PinLinkType linkType = pin.GetPinLinkType();

			int typeIndex = 1;
			for (int i = 0; i < id; ++i)
			{
				if (_pins[i].GetPinLinkType() == linkType)
				{
					++typeIndex;
				}
			}

			Rect returnRect = new Rect();
			Vector2 rectPosition = new Vector2();
			returnRect.width = _renderData.PinSize;
			returnRect.height = _renderData.PinSize;
			rectPosition.y = _renderData.NodeRect.position.y + _renderData.PinVerticalOffset + typeIndex * (returnRect.height + _renderData.PinVerticalSpacing);
			if (linkType == PinLinkType.Input)
			{
				rectPosition.x = _renderData.NodeRect.position.x + _renderData.InputPinHorizontalOffset;
			}
			else
			{
				rectPosition.x = _renderData.NodeRect.position.x + _renderData.OutputPinHorizontalOffset - _renderData.PinSize;
			}

			returnRect.position = rectPosition;
			return returnRect;
		}

		public Rect GetPinTextRect(int id)
		{
			float estimatedCharacterWidth = 8.0f;
			float estimatedCharacterHeight = 16.0f;

			Rect pinRect = GetPinRect(id);
			pinRect.width = (_pins[id].GetPinName().Length + 1) * estimatedCharacterWidth;
			pinRect.height = estimatedCharacterHeight;
			Vector2 rectPosition = pinRect.position;
			if (_pins[id].GetPinLinkType() == PinLinkType.Input)
			{
				rectPosition.x += _renderData.PinSize;
			}
			else
			{
				rectPosition.x -= pinRect.width - _renderData.PinSize;
			}

			pinRect.position = rectPosition;
			return pinRect;
		}

		public string GetPinName(int id)
		{
			return _pins[id].GetPinName();
		}

		public EditorPinIdentifier GetPinIdentifier(int id)
		{
			EditorPinIdentifier identifier = new EditorPinIdentifier();
			identifier.NodeID = _id;
			identifier.PinID = id;
			return identifier;
		}

		public void SetNodePosition(Vector2 position)
		{
			Position = position;
			UpdateNodeRect();
		}

	    public Vector2 GetNodePosition()
	    {
	        return Position;
	    }

		public void UpdateNodeRect()
		{
			_renderData.PinSize = 10.0f;
			_renderData.NodeRect.position = Position;
			_renderData.NodeRect.width = 60.0f + GetLongestPinNameWidth(PinLinkType.Input) + GetLongestPinNameWidth(PinLinkType.Output);
			_renderData.NodeRect.height = 16.0f + (_renderData.PinVerticalOffset +
			                                       (_renderData.PinSize + _renderData.PinVerticalSpacing) *
			                                       Mathf.Max(GetNumPins(PinLinkType.Input), GetNumPins(PinLinkType.Output)));
			_renderData.InputPinHorizontalOffset = _renderData.PinSize;
			_renderData.OutputPinHorizontalOffset = _renderData.NodeRect.width - _renderData.PinSize;
			_renderData.PinVerticalOffset = 16.0f;
			_renderData.PinVerticalSpacing = 10.0f;
		}

		public void RenderNode(EditorGraph graph, bool isSelected)
		{
			const float SELECTION_BORDER = 5.0f;
			if (isSelected)
			{
				EditorGraphDrawUtils.DrawRect(_renderData.NodeRect.min - Vector2.one * SELECTION_BORDER, _renderData.NodeRect.max + Vector2.one * SELECTION_BORDER, Color.yellow);
			}

			GUI.Box(_renderData.NodeRect, " ");
			for (int i = 0; i < PinCount; ++i)
			{
				_pins[i].RenderPin(graph, this);
			}

			RenderNodeText();
		}

		private void RenderNodeText()
		{
			Rect nodeRect = _renderData.NodeRect;
			nodeRect.height = 16.0f;
			EditorGUI.LabelField(nodeRect, Name);

			for (int i = 0; i < PinCount; ++i)
			{
				EditorGUI.LabelField(GetPinTextRect(i), GetPinName(i));
			}
		}

		private float GetLongestPinNameWidth(PinLinkType linkType)
		{
			float width = 0.0f;
			for (int i = 0; i < _pins.Count; i++)
			{
				EditorPin pin = _pins[i];
				if (pin.GetPinLinkType() == linkType)
				{
					float currentPinWidth = EstimateStringWidth(pin.GetPinName());
					if (currentPinWidth > width)
					{
						width = currentPinWidth;
					}
				}
			}

			return width;
		}

		private float EstimateStringWidth(string value)
		{
			return value.Length * 8.0f;
		}

		private int AddPin(PinLinkType linkType, Type type, string name)
		{
			int pinID = _pins.Count;
			Debug.Log($"Adding pin to node with ID {ID}, pinID = {pinID}");
			EditorPin newPin = new EditorPin(type == null ? "null" : type.ToString(), name, ID, pinID, linkType);
			_pins.Add(newPin);
			UpdateNodeRect();
			return pinID;
		}

		private void ClearPins()
		{
			_pins.Clear();
		}

		private bool RemovePin(EditorPin pin)
		{
			return _pins.Remove(pin);
		}

		private int GetNumPins(PinLinkType pinLinkType)
		{
			int outNumPins = 0;
			for (int i = 0; i < PinCount; ++i)
			{
				EditorPin pin = _pins[i];
				if (pin.GetPinLinkType() == pinLinkType)
				{
					++outNumPins;
				}
			}

			return outNumPins;
		}

		private void NotifyGraphChange()
		{
			OnNodeChanged?.Invoke();
		}

		public static EditorNode CreateFromFunction(EditorGraph graph, Type type, string name, bool hasOutput = true, bool hasInput = true)
		{
			EditorNode node = new EditorNode(graph);
			if (type != null)
			{
				MethodInfo methodInfo = type.GetMethod(name);
				if (methodInfo != null)
				{
					node.Name = SanitizeName(name);
					if (hasOutput)
					{
						node.AddPin(PinLinkType.Output, null, string.Empty);
					}
					if (hasInput)
					{
						node.AddPin(PinLinkType.Input, null, string.Empty);
					}

					node.AddPin(PinLinkType.Output, methodInfo.ReturnParameter.ParameterType, "Output");

					ParameterInfo[] parameters = methodInfo.GetParameters();
					for (int i = 0; i < parameters.Length; i++)
					{
						ParameterInfo parameter = parameters[i];
						node.AddPin(PinLinkType.Input, parameter.ParameterType, parameter.Name);
					}
				}
				else
				{
					Debug.LogError($"Function '{type.ToString()}.{name}' not found.");
				}
			}
			else
			{
				Debug.LogError("Tried to create node from function from an unknown class type.");
			}

			node.NotifyGraphChange();
			return node;
		}

		private static string SanitizeName(string name)
		{
			string result = "" + name[0];
			bool wasCapital = name[0] >= 'A' && name[0] <= 'Z';
			for (int i = 1; i < name.Length; ++i)
			{
				if (name[i] >= 'A' && name[i] <= 'Z')
				{
					if (!wasCapital)
					{
						result += " " + name[i];
					}
					else
					{
						result += name[i];
					}
					wasCapital = true;
				}
				else
				{
					result += name[i];
					wasCapital = false;
				}
			}

			return result;
		}
	}
}