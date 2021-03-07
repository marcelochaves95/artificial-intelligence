using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

// TODO:
// Create editor variant of node, renders GL to the editor window
// can be dragged around in the editor window
// exports its data to Node class

[System.Serializable]
public class Pin
{
	[SerializeField] public string Name;
	[SerializeField] public string Type;
	[SerializeField] public Pin Next;
}

[System.Serializable]
public class Node
{

	[SerializeField] public string ClassName { get; set; }
	[SerializeField] public string FunctionName { get; set; }
	[SerializeField] private Node NextFlowNode;
	[SerializeField] private List<Pin> OutputPins;

	public void Evaluate()
	{

	}

}
