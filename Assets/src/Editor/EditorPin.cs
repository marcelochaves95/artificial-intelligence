using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPinLinkType
{
	None,
	Input,
	Output,
}

[System.Serializable]
public class EditorPinTypeInfo
{
	[SerializeField] private string Type;

	public EditorPinTypeInfo(string _Type)
	{
		Type = _Type;
	}

	public string TypeString
	{
		get
		{
			return Type;
		}
	}
}

[System.Serializable]
public class EditorPin
{

	[SerializeField] private EditorPinTypeInfo TypeInfo = null;
	[SerializeField] private string Name;
	[SerializeField] private int OwnerID;
	[SerializeField] private int ID;
	[SerializeField] private EPinLinkType PinLinkType;

	public EditorPin()
	{
		Name = "";
		OwnerID = -1;
		ID = -1;
		PinLinkType = EPinLinkType.None;
	}

	public EditorPin(string _Type, string _Name, int _OwnerID, int _ID, EPinLinkType _PinLinkType)
	{
		TypeInfo = new EditorPinTypeInfo(_Type);
		Name = _Name;
		OwnerID = _OwnerID;
		ID = _ID;
		PinLinkType = _PinLinkType;
	}

	public override string ToString()
	{
		if (TypeInfo == null)
		{
			return Name;
		}
		return "(" + TypeInfo.TypeString + ") " + Name;
	}

	public string GetPinName()
	{
		return Name;
	}

	public int GetOwnerID()
	{
		return OwnerID;
	}

	public int GetID()
	{
		return ID;
	}

	public EPinLinkType GetPinLinkType()
	{
		return PinLinkType;
	}

	public System.Type GetPinType()
	{
		return System.Type.GetType(TypeInfo.TypeString);
	}

	public bool CanLinkTo(EditorPin Other)
	{
		if (OwnerID != Other.OwnerID)
		{
			if (PinLinkType == EPinLinkType.Input && Other.PinLinkType == EPinLinkType.Output)
			{
				if (TypeInfo == null && Other.TypeInfo == null)
				{
					return true;
				}
				else
				{
					if (TypeInfo.TypeString.Equals(Other.TypeInfo.TypeString))
					{
						return true;
					}
					else
					{
						Debug.LogWarning("My type = " + TypeInfo.TypeString + ", their type = " + TypeInfo.TypeString);
					}
				}
			}
			else
			{
				Debug.LogWarning("My Link Type = " + PinLinkType + ", other Link Type = " + Other.PinLinkType);
				Debug.LogWarning("Mine should be input, theirs output.");
			}
		}
		else
		{
			Debug.LogWarning(OwnerID + " == " + Other.OwnerID);
		}

		return false;
	}

	public void RenderPin(EditorGraph Graph, EditorNode Node)
	{
		Rect PinRect = Node.GetPinRect(ID);
		GUI.Button(PinRect, " ");
	}

}
