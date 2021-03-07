using System;
using UnityEngine;

namespace NodeGraph
{
	[Serializable]
	public class EditorPin
	{
		[SerializeField]
		private EditorPinTypeInfo _typeInfo = null;
		[SerializeField]
		private string _name;
		[SerializeField]
		private int _ownerID;
		[SerializeField]
		private int _id;
		[SerializeField]
		private PinLinkType _linkType;

		public EditorPin()
		{
			_name = string.Empty;
			_ownerID = -1;
			_id = -1;
			_linkType = PinLinkType.None;
		}

		public EditorPin(string type, string name, int ownerID, int id, PinLinkType linkType)
		{
			_typeInfo = new EditorPinTypeInfo(type);
			_name = name;
			_ownerID = ownerID;
			_id = id;
			_linkType = linkType;
		}

		public override string ToString()
		{
			if (_typeInfo == null)
			{
				return _name;
			}

			return $"({_typeInfo.Type}) {_name}";
		}

		public string GetPinName()
		{
			return _name;
		}

		public int GetOwnerID()
		{
			return _ownerID;
		}

		public int GetID()
		{
			return _id;
		}

		public PinLinkType GetPinLinkType()
		{
			return _linkType;
		}

		public Type GetPinType()
		{
			return Type.GetType(_typeInfo.Type);
		}

		public bool CanLinkTo(EditorPin other)
		{
			if (_ownerID != other._ownerID)
			{
				if (_linkType == PinLinkType.Input && other._linkType == PinLinkType.Output)
				{
					if (_typeInfo == null && other._typeInfo == null)
					{
						return true;
					}
					else
					{
						if (_typeInfo.Type.Equals(other._typeInfo.Type))
						{
							return true;
						}
						else
						{
							Debug.LogWarning($"My type = {_typeInfo.Type}, their type = {_typeInfo.Type}");
						}
					}
				}
				else
				{
					Debug.LogWarning($"My Link Type = {_linkType}, other Link Type = {other._linkType}");
					Debug.LogWarning("Mine should be input, theirs output.");
				}
			}
			else
			{
				Debug.LogWarning($"{_ownerID} == {other._ownerID}");
			}

			return false;
		}

		public void RenderPin(EditorGraph graph, EditorNode node)
		{
			Rect pinRect = node.GetPinRect(_id);
			GUI.Button(pinRect, " ");
		}

	}
}
