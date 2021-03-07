using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NodeGraph
{
	public class GraphEditor : EditorWindow, IGraphInputListener, IGraphInputHandler
	{
		private Dictionary<Type, List<string>> _registeredFunctionMap;
		private Dictionary<Type, bool> _showFunctions;
		private List<EditorNode> _nodes = new List<EditorNode>();
		private List<EditorLink> _links = new List<EditorLink>();
		private EditorGraph _graph;
		private int _controlID = -1;
		private Vector2 _oldDragPosition = new Vector2();
		private Vector2 _drag = new Vector2();

		[MenuItem("Window/Node Graph", priority = 999999)]
		public static void Init()
		{
			GraphEditor window = (GraphEditor) GetWindow(typeof(GraphEditor));
			window.titleContent = new GUIContent("Node Graph");
			window.Reset();
		}

		public void Reset()
		{
			if (_graph != null)
			{
				_graph.Deselect();
			}

			UpdateFunctionMap();
		}

		private bool HitTestPointToRect(Vector2 point, Rect rect)
		{
			if (point.x >= rect.min.x
			    && point.x <= rect.max.x
			    && point.y >= rect.min.y
			    && point.y <= rect.max.y)
			{
				return true;
			}

			return false;
		}

		private void OnGUI()
		{
			DrawGrid(20, 0.2f, Color.gray);
			DrawGrid(100, 0.4f, Color.gray);
			ProcessEvents(Event.current);

			if (_graph == null)
			{
				Reset();
				return;
			}

			if (_graph != null && _registeredFunctionMap == null)
			{
				Reset();
			}
			
			RenderGraph();
			
			// Draw editors for selected node
			if (_graph != null && _graph.IsPinSelected())
			{
				EditorNode ownerNode = _graph.GetSelectedNode();
				Vector2 pinPosition = ownerNode.GetPinRect(_graph.GetSelectedElementID().PinID).center;
				EditorGraphDrawUtils.Line(pinPosition, Event.current.mousePosition, Color.magenta);
			}
		}

		private void OnClickNewGraph()
		{
			EditorGraph newGraph = CreateInstance<EditorGraph>();
			string folderPath = "Assets/NodeGraph";
			if (!AssetDatabase.IsValidFolder(folderPath))
			{
				string guid = AssetDatabase.CreateFolder("Assets", "NodeGraph");
				folderPath = AssetDatabase.GUIDToAssetPath(guid);
			}

			string assetName = "NewGraph";
			string extension = "asset";
			string savePanelPath = EditorUtility.SaveFilePanel("Creating asset...", folderPath, assetName, extension);
			string newAssetName = Path.GetFileNameWithoutExtension(savePanelPath);
			if (!string.IsNullOrEmpty(newAssetName))
			{
				string newAssetPath = $"{folderPath}/{newAssetName}.{extension}";
				AssetDatabase.CreateAsset(newGraph, newAssetPath);
				AssetDatabase.SaveAssets();
				newGraph.Deselect();
				OnGraphLoaded(newGraph);
			}
		}

		private void OnClickLoadGraph()
		{
			_controlID = GUIUtility.GetControlID(FocusType.Passive);
			EditorGUIUtility.ShowObjectPicker<EditorGraph>(null, false, string.Empty, _controlID);
		}

		private void AddFunctionListToContextMenu(GenericMenu menu, Vector2 mousePos)
		{
			if (_graph == null)
			{
				menu.AddDisabledItem(new GUIContent("No graph available"));
				return;
			}

			foreach (Type key in _registeredFunctionMap.Keys)
			{
				string libraryName = key.ToString();
				for (int i = 0; i < _registeredFunctionMap[key].Count; i++)
				{
					string name = _registeredFunctionMap[key][i];
					menu.AddItem(new GUIContent($"{libraryName}/{name}"), false, AddNode);
					void AddNode() => OnClickAddNode(key, name, mousePos);
				}
			}
		}

		private void OnClickAddNode(Type type, string name, Vector2 mousePosition)
		{
			EditorNode editorNode = EditorNode.CreateFromFunction(_graph, type, name, false, false);
			_graph.AddNode(editorNode, out int nodeID);
			EditorNode node = _graph.GetNodeFromID(nodeID);
			node.SetNodePosition(mousePosition);
			Repaint();
		}

		private void ProcessEvents(Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
					OnMouseDown(e.button, e.mousePosition);
					break;
				case EventType.MouseUp:
					OnMouseUp(e.button, e.mousePosition);
					break;
				case EventType.MouseDrag:
					OnMouseMove(e.mousePosition);
					break;
				case EventType.KeyUp:
					if (e.keyCode == KeyCode.Delete && _graph.IsNodeSelected())
					{
						_graph.RemoveNode(_graph.GetSelectedNode());
						_graph.Deselect();
						Repaint();
					}
					break;
			}

			if (e.commandName.Equals("ObjectSelectorClosed"))
			{
				OnGraphLoaded(EditorGUIUtility.GetObjectPickerObject() as EditorGraph);
				_controlID = -1;
			}
		}

		private void ProcessContextMenu(Vector2 mousePosition)
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent("New Graph"), false, NewGraph);
			genericMenu.AddItem(new GUIContent("Load Graph"), false, LoadGraph);
			void NewGraph() => OnClickNewGraph();
			void LoadGraph() => OnClickLoadGraph();
			genericMenu.AddSeparator(string.Empty);
			AddFunctionListToContextMenu(genericMenu, mousePosition);
			genericMenu.ShowAsContext();
		}

		private void UpdateFunctionMap()
		{
			UpdateGraphCache();

			_registeredFunctionMap = new Dictionary<Type, List<string>>();
			_showFunctions = new Dictionary<Type, bool>();

			List<Type> types = new List<Type>();
			TypeUtilities.GetAllSubclasses(typeof(FunctionLibrary), types);
			for (int i = 0; i < types.Count; i++)
			{
				Type type = types[i];
				_registeredFunctionMap[type] = new List<string>();
				MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
				for (int j = 0; j < methodInfos.Length; j++)
				{
					MethodInfo methodInfo = methodInfos[j];
					_registeredFunctionMap[type].Add(methodInfo.Name);
				}
			}
		}

		public void SetGraph(EditorGraph graph)
		{
			EditorGraph oldGraph = _graph;
			_graph = graph;
			if (_graph != null)
			{
				if (oldGraph != null && oldGraph != _graph)
				{
					oldGraph.OnGraphChanged -= OnGraphChanged;
				}

				UpdateGraphCache();
				_graph.OnGraphChanged += OnGraphChanged;
			}
		}

		public void OnGraphChanged()
		{
			UpdateGraphCache();
			SaveGraph();
		}

		private void SaveGraph()
		{
			if (_graph != null)
			{
				EditorUtility.SetDirty(_graph);
				AssetDatabase.SaveAssets();
			}
		}

		public void RenderGraph()
		{
			if (_graph != null)
			{
				_graph.RenderGraph();
			}
		}

		public void OnGraphLoaded(EditorGraph graph)
		{
			SetGraph(graph);
			UpdateFunctionMap();
		}

		private void UpdateGraphCache()
		{
			if (_graph != null)
			{
				_nodes = _graph.GetNodeList();

				for (int i = 0; i < _nodes.Count; i++)
				{
					EditorNode node = _nodes[i];
					node.UpdateNodeRect();
				}

				_links = _graph.GetLinkList();
				Repaint();
			}
		}

		public void OnMouseDown(int button, Vector2 mousePosition)
		{
			if (_graph == null)
			{
				return;
			}

			if (button == 0)
			{
				_oldDragPosition = mousePosition;

				for (int i = 0; i < _nodes.Count; i++)
				{
					EditorNode node = _nodes[i];
					int pinCount = node.PinCount;
					for (int j = 0; j < pinCount; ++j)
					{
						Rect pinRect = node.GetPinRect(j);
						if (HitTestPointToRect(mousePosition, pinRect))
						{
							_graph.SelectPin(node.GetPinIdentifier(j));
							return;
						}
					}

					Rect nodeRect = node.GetNodeRect();
					if (HitTestPointToRect(mousePosition, nodeRect))
					{
						_graph.Deselect();
						_graph.SelectNode(node.ID);
						Repaint();
						return;
					}
				}

				_graph.Deselect();
			}
			else if (button == 1)
			{
				ProcessContextMenu(mousePosition);
			}

			Repaint();
		}

		public void OnMouseUp(int button, Vector2 mousePosition)
		{
			if (button == 0)
			{
				if (_graph == null)
				{
					return;
				}

				bool mouseOverNode = false;
				for (int i = 0; i < _nodes.Count; i++)
				{
					EditorNode node = _nodes[i];
					Rect nodeRect = node.GetNodeRect();
					if (HitTestPointToRect(mousePosition, nodeRect))
					{
						mouseOverNode = true;
						int pinCount = node.PinCount;
						for (int j = 0; j < pinCount; ++j)
						{
							Rect pinRect = node.GetPinRect(j);
							if (HitTestPointToRect(mousePosition, pinRect))
							{
								if (_graph.IsPinSelected())
								{
									EditorPinIdentifier selectedPinIdentifier = _graph.GetSelectedElementID();
									LinkPins(selectedPinIdentifier, node.GetPinIdentifier(j));
									UpdateGraphCache();
									break;
								}
							}
						}
					}
				}

				if (!mouseOverNode || _graph.IsPinSelected())
				{
					_graph.Deselect();
				}
			}
			else if (button == 1)
			{
				ProcessContextMenu(mousePosition);
			}

			Repaint();
		}

		public void OnMouseMove(Vector2 mousePosition)
		{
			Vector2 mouseDelta = mousePosition - _oldDragPosition;
			if (_graph == null)
			{
				return;
			}

			if (_graph.IsNodeSelected())
			{
				MoveNode(_graph.GetSelectedNode(), mouseDelta);
			}

			_oldDragPosition = mousePosition;
			Repaint();
		}
		
		public bool LinkPins(EditorPinIdentifier lhsPin, EditorPinIdentifier rhsPin)
		{
			EditorPin lhsPinData = _graph.GetPinFromID(lhsPin);
			EditorPin rhsPinData = _graph.GetPinFromID(rhsPin);

			if (!rhsPinData.CanLinkTo(lhsPinData))
			{
				Debug.LogWarning($"Failed to link pin {lhsPin} to {rhsPin}.");
				return false;
			}

			_graph.LinkPins(lhsPin, rhsPin);
			return true;
		}

		public void MoveNode(EditorNode node, Vector2 delta)
		{
	        Vector2 position = node.GetNodePosition();
			node.SetNodePosition(position + delta);
			Repaint();
		}

		private void DrawGrid(float cellSize, float opacity, Color color)
		{
			int widthDivs = Mathf.CeilToInt(position.width / cellSize);
			int heightDivs = Mathf.CeilToInt(position.height / cellSize);

			Handles.BeginGUI();
			Handles.color = new Color(color.r, color.g, color.b, opacity);

			Vector3 newOffset = new Vector3();
			if (_graph != null)
			{
				_graph._editorViewportOffset += _drag * 0.5f;
				newOffset = new Vector3(_graph._editorViewportOffset.x % cellSize, _graph._editorViewportOffset.y % cellSize, 0);
			}

			for (int i = 0; i < widthDivs; ++i)
			{
				Handles.DrawLine(new Vector3(cellSize * i, -cellSize, 0) + newOffset, new Vector3(cellSize * i, position.height, 0) + newOffset);
			}

			for (int i = 0; i < heightDivs; ++i)
			{
				Handles.DrawLine(new Vector3(-cellSize, cellSize * i, 0) + newOffset, new Vector3(position.width, cellSize * i, 0) + newOffset);
			}

			Handles.color = Color.white;
			Handles.EndGUI();
		}
	}
}
