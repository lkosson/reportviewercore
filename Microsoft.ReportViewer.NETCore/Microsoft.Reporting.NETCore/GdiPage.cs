using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class GdiPage : DrawablePage
	{
		internal class VHSort : IComparer<int>
		{
			private ReportActions m_collection;

			internal VHSort(ReportActions collection)
			{
				m_collection = collection;
			}

			public int Compare(int index1, int index2)
			{
				if (m_collection[index1].Position.Top == m_collection[index2].Position.Top)
				{
					if (m_collection[index1].Position.Left < m_collection[index2].Position.Left)
					{
						return -1;
					}
					if (m_collection[index1].Position.Left == m_collection[index2].Position.Left)
					{
						return 0;
					}
					return 1;
				}
				if (m_collection[index1].Position.Top >= m_collection[index2].Position.Top)
				{
					return 1;
				}
				return -1;
			}
		}

		private List<int> m_sortedActionItemIndices;

		private ClientGDIRenderer m_gdiRenderer;

		private List<FixedHeaderItem> m_fixedHeaders = new List<FixedHeaderItem>();

		private ReportActions m_actions = new ReportActions();

		private Dictionary<string, BookmarkPoint> m_bookmarks;

		private ReportActions m_toolTips = new ReportActions();

		private bool m_allActionsInitialized;

		private bool m_rplActionsInitialized;

		private int m_actionIndex = -1;

		private bool m_firstDraw = true;

		public override ReportActions Actions => m_actions;

		public override Dictionary<string, BookmarkPoint> Bookmarks => m_bookmarks;

		public override ReportActions ToolTips => m_toolTips;

		public override bool NeedsFrame => false;

		public override int ExternalMargin => 0;

		public override bool DrawInPixels => false;

		public override bool IsRequireFullRedraw
		{
			get
			{
				if (m_gdiRenderer != null)
				{
					return m_gdiRenderer.Report.FixedHeaders.Count > 0;
				}
				return false;
			}
		}

		public int ActionIndex
		{
			get
			{
				return m_actionIndex;
			}
			set
			{
				m_actionIndex = value;
			}
		}

		public Action CurrentAction
		{
			get
			{
				if (m_sortedActionItemIndices == null || m_actionIndex == -1 || m_actionIndex >= m_sortedActionItemIndices.Count)
				{
					return null;
				}
				return m_actions[m_sortedActionItemIndices[m_actionIndex]];
			}
		}

		public List<int> SortedActionItemIndices => m_sortedActionItemIndices;

		public GdiPage(ClientGDIRenderer renderer)
		{
			m_gdiRenderer = renderer;
		}

		public override void Draw(Graphics g, PointF scrollOffset, bool testMode)
		{
			if (m_firstDraw)
			{
				int actionsCount = 0;
				if (m_rplActionsInitialized)
				{
					actionsCount = m_gdiRenderer.Report.Actions.Count;
				}
				m_gdiRenderer.DrawToPage(g, m_firstDraw, testMode);
				InitializeInteractivity(g, actionsCount);
				m_allActionsInitialized = true;
				m_rplActionsInitialized = true;
				m_firstDraw = false;
			}
			else
			{
				m_gdiRenderer.DrawToPage(g, m_firstDraw, testMode);
			}
			if (m_actionIndex != -1)
			{
				RenderVisibleActionItem(g);
			}
			RenderFixedHeaders(g, scrollOffset);
		}

		public override void GetPageSize(Graphics g, out float width, out float height)
		{
			width = 2 + Global.ToPixels(m_gdiRenderer.PageWidth, g.DpiX) + 2 * ExternalMargin;
			height = 2 + Global.ToPixels(m_gdiRenderer.PageHeight, g.DpiY) + 2 * ExternalMargin;
		}

		public override void BuildInteractivityInfo(Graphics g)
		{
			if (!m_allActionsInitialized && !m_rplActionsInitialized)
			{
				InitializeInteractivity(g, 0);
				m_rplActionsInitialized = true;
			}
		}

		private void RenderVisibleActionItem(Graphics g)
		{
			if (m_actionIndex == -1)
			{
				return;
			}
			float dpiX = g.DpiX;
			float dpiY = g.DpiY;
			Action currentAction = CurrentAction;
			if (currentAction.Shape == ActionShape.Rectangle)
			{
				g.DrawRectangle(new Pen(new HatchBrush(HatchStyle.Percent50, Color.White, Color.Black)), Global.ToMillimeters(currentAction.Position.Left, dpiX), Global.ToMillimeters(currentAction.Position.Top, dpiY), Global.ToMillimeters(currentAction.Position.Width, dpiX), Global.ToMillimeters(currentAction.Position.Height, dpiY));
				return;
			}
			if (currentAction.Shape == ActionShape.Circle)
			{
				g.DrawEllipse(new Pen(new HatchBrush(HatchStyle.Percent50, Color.White, Color.Black)), Global.ToMillimeters(currentAction.Position.Left, dpiX), Global.ToMillimeters(currentAction.Position.Top, dpiY), Global.ToMillimeters(currentAction.Position.Width, dpiX), Global.ToMillimeters(currentAction.Position.Height, dpiY));
				return;
			}
			PointF[] array = new PointF[currentAction.Points.Length];
			int num = 0;
			Point[] points = currentAction.Points;
			for (int i = 0; i < points.Length; i++)
			{
				PointF pointF = points[i];
				array[num++] = new PointF(Global.ToMillimeters((int)pointF.X, dpiX), Global.ToMillimeters((int)pointF.Y, dpiY));
			}
			g.DrawPolygon(new Pen(new HatchBrush(HatchStyle.Percent50, Color.White, Color.Black)), array);
		}

		private void RenderFixedHeaders(Graphics g, PointF offset)
		{
			float x = offset.X;
			float y = offset.Y;
			float num = g.Transform.Elements[0];
			bool flag = false;
			foreach (FixedHeaderItem fixedHeader in m_gdiRenderer.Report.FixedHeaders)
			{
				if (!fixedHeader.FoundActions)
				{
					RectangleF rectangleF = new RectangleF(Global.ToPixels(fixedHeader.Bounds.X, g.DpiX), Global.ToPixels(fixedHeader.Bounds.Y, g.DpiY), Global.ToPixels(fixedHeader.Bounds.Width, g.DpiX) + 1, Global.ToPixels(fixedHeader.Bounds.Height, g.DpiY) + 1);
					foreach (Action action2 in Actions)
					{
						if (rectangleF.Contains(action2.Position))
						{
							fixedHeader.AddAction(action2);
						}
					}
					foreach (Action toolTip in ToolTips)
					{
						if (rectangleF.Contains(toolTip.Position))
						{
							fixedHeader.AddAction(toolTip);
						}
					}
					fixedHeader.FoundActions = true;
				}
				else if (fixedHeader.Actions != null && !flag)
				{
					for (int i = 0; i < fixedHeader.Actions.Count; i++)
					{
						Action action = fixedHeader.Actions[i];
						action.XOffset = 0;
						action.YOffset = 0;
					}
					flag = true;
				}
				float num2 = 0f;
				float num3 = 0f;
				float bottom = fixedHeader.Container.Position.Bottom;
				if (fixedHeader.Layout == FixedHeaderItem.LayoutType.Horizontal)
				{
					float num4 = (0f - y) / num;
					if (num4 < fixedHeader.Bounds.Y || num4 > bottom - fixedHeader.Bounds.Height)
					{
						continue;
					}
					num2 = 0f;
					num3 = 0f - y - fixedHeader.Bounds.Y * num;
					if (fixedHeader.Actions != null)
					{
						int num5 = (int)((float)Global.ToPixels(0f - y, g.DpiY) / num);
						int num6 = Global.ToPixels(fixedHeader.Container.Position.Top, g.DpiY);
						int value = num5 - num6;
						foreach (Action action3 in fixedHeader.Actions)
						{
							action3.YOffset = Convert.ToInt32(value);
						}
					}
				}
				else if (fixedHeader.Layout == FixedHeaderItem.LayoutType.Vertical)
				{
					float num7 = (0f - x) / num;
					num3 = 0f;
					float num8 = num7 + g.ClipBounds.Width;
					if (num7 > fixedHeader.Container.Position.Left + fixedHeader.Container.Position.Width - fixedHeader.Bounds.Width || num8 < fixedHeader.Container.Position.Left + fixedHeader.Bounds.Width)
					{
						continue;
					}
					float num9 = 0f;
					if (num7 > fixedHeader.Bounds.X)
					{
						num2 = 0f - x - fixedHeader.Bounds.X * num;
						num9 = Convert.ToSingle(Global.ToPixels((0f - x) / num - fixedHeader.Bounds.X, g.DpiX));
					}
					else
					{
						if (num8 > fixedHeader.Bounds.Right)
						{
							continue;
						}
						num2 = (num8 - fixedHeader.Bounds.Right) * num;
						num9 = Convert.ToSingle(Global.ToPixels(num8 - fixedHeader.Bounds.Right, g.DpiX));
					}
					if (fixedHeader.Actions != null)
					{
						foreach (Action action4 in fixedHeader.Actions)
						{
							action4.XOffset = Convert.ToInt32(num9);
						}
					}
				}
				else
				{
					float num10 = (0f - x) / num;
					float num11 = (0f - y) / num;
					if (fixedHeader.Container.Position.Y > num11 || fixedHeader.Container.Position.X > num10)
					{
						continue;
					}
					num3 = 0f - y - fixedHeader.Container.Position.Y * num;
					num2 = 0f - x - fixedHeader.Container.Position.X * num;
				}
				g.ResetTransform();
				g.ScaleTransform(num, num);
				g.TranslateTransform(num2, num3, MatrixOrder.Append);
				foreach (RenderingItem renderingItem in fixedHeader.RenderingItems)
				{
					renderingItem.DrawToPage(m_gdiRenderer.Context);
					renderingItem.DrawBorders(m_gdiRenderer.Context);
				}
			}
		}

		private void InitializeInteractivity(Graphics g, int actionsCount)
		{
			m_actions.Actions = m_gdiRenderer.Report.Actions;
			for (int i = actionsCount; i < m_gdiRenderer.Report.Actions.Count; i++)
			{
				m_gdiRenderer.Report.Actions[i].SetDpi(g.DpiX, g.DpiY);
			}
			m_toolTips.Actions = m_gdiRenderer.Report.ToolTips;
			for (int j = 0; j < m_toolTips.Count; j++)
			{
				m_toolTips[j].SetDpi(g.DpiX, g.DpiY);
			}
			m_sortedActionItemIndices = new List<int>(m_actions.Count);
			for (int k = 0; k < m_actions.Count; k++)
			{
				m_sortedActionItemIndices.Add(k);
			}
			m_sortedActionItemIndices.Sort(new VHSort(m_actions));
			if (m_rplActionsInitialized)
			{
				return;
			}
			m_bookmarks = m_gdiRenderer.Report.Bookmarks;
			foreach (string key in m_bookmarks.Keys)
			{
				m_bookmarks[key].SetDpi(g.DpiX, g.DpiY);
			}
			m_fixedHeaders = m_gdiRenderer.Report.FixedHeaders;
			for (int l = 0; l < m_fixedHeaders.Count; l++)
			{
				m_fixedHeaders[l].FoundActions = false;
			}
		}
	}
}
