using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class RSDocMap : TreeView
	{
		private sealed class NodeData
		{
			private string m_label;

			public Point FocusPoint;

			private string m_id;

			public string Label => m_label;

			public string ID => m_id;

			public NodeData(DocumentMapNode node)
			{
				m_label = node.Label;
				m_id = node.Id;
			}
		}

		private Container components;

		private Dictionary<string, NodeData> m_allNodes = new Dictionary<string, NodeData>();

		private bool m_hasDocMap;

		public bool HasDocMap => m_hasDocMap;

		public event DocumentMapNavigationEventHandler DocumentMapNavigation;

		public RSDocMap()
		{
			InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			BackColor = System.Drawing.Color.White;
			base.Size = new System.Drawing.Size(176, 216);
		}

		public void Clear()
		{
			m_hasDocMap = false;
			base.Nodes.Clear();
			m_allNodes.Clear();
		}

		public void PopulateTree(DocumentMapNode rootNode)
		{
			Clear();
			if (rootNode != null)
			{
				SuspendLayout();
				TreeNode treeNode = new TreeNode(rootNode.Label);
				AddNodeData(rootNode, treeNode);
				AddChildren(rootNode, treeNode);
				base.Nodes.Add(treeNode);
				base.Nodes[0].Expand();
				ResumeLayout();
				m_hasDocMap = true;
			}
		}

		private void AddChildren(DocumentMapNode thisNode, TreeNode inTreeNode)
		{
			foreach (DocumentMapNode child in thisNode.Children)
			{
				TreeNode treeNode = new TreeNode(child.Label);
				inTreeNode.Nodes.Add(treeNode);
				AddNodeData(child, treeNode);
				AddChildren(child, treeNode);
			}
		}

		private void AddNodeData(DocumentMapNode node, TreeNode inTreeNode)
		{
			if (!m_allNodes.ContainsKey(node.Id))
			{
				NodeData nodeData = new NodeData(node);
				inTreeNode.Text = nodeData.Label;
				inTreeNode.Tag = nodeData.ID;
				m_allNodes.Add(nodeData.ID, nodeData);
			}
		}

		protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
		{
			base.OnNodeMouseClick(e);
			PerformNavigationForNode(e.Node);
		}

		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			base.OnAfterSelect(e);
			if (e.Action != TreeViewAction.ByMouse && e.Action != 0)
			{
				PerformNavigationForNode(e.Node);
			}
		}

		private void PerformNavigationForNode(TreeNode node)
		{
			if (node.Tag != null && this.DocumentMapNavigation != null)
			{
				DocumentMapNavigationEventArgs e = new DocumentMapNavigationEventArgs((string)node.Tag);
				this.DocumentMapNavigation(this, e);
			}
		}

		public Point GetFocusPoint(string id)
		{
			return m_allNodes[id]?.FocusPoint ?? default(Point);
		}

		public void UpdateTreeForPage(Dictionary<string, LabelPoint> labels)
		{
			using (Graphics graphics = CreateGraphics())
			{
				foreach (string key in labels.Keys)
				{
					if (m_allNodes.ContainsKey(key))
					{
						NodeData nodeData = m_allNodes[key];
						labels[key].SetDpi(graphics.DpiX, graphics.DpiY);
						nodeData.FocusPoint = labels[key].Point;
					}
				}
			}
		}
	}
}
