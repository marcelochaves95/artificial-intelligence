using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace NodeGraph
{
	// TODO:
	// Create editor variant of node, renders GL to the editor window
	// can be dragged around in the editor window
	// exports its data to Node class
	[Serializable]
	public class Node
	{
		[SerializeField]
		public string ClassName { get; set; }
		[SerializeField]
		public string FunctionName { get; set; }

		[SerializeField]
		private Node _nextFlowNode;
		[SerializeField]
		private List<Pin> _outputPins;

		public void Evaluate()
		{
			//
		}
	}
}
