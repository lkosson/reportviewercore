using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeTitles")]
	internal class TitleCollection : CollectionBase
	{
		internal Chart chart;

		private IServiceContainer serviceContainer;

		private Chart Chart
		{
			get
			{
				if (chart == null && serviceContainer != null)
				{
					chart = (Chart)serviceContainer.GetService(typeof(Chart));
				}
				return chart;
			}
		}

		public Title this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (Title)base.List[(int)parameter];
				}
				if (parameter is string)
				{
					foreach (Title inner in base.InnerList)
					{
						if (inner.Name == (string)parameter)
						{
							return inner;
						}
					}
					throw new ArgumentException(SR.ExceptionTitleNameNotFound((string)parameter));
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
			}
			set
			{
				int num = IndexOf(value.Name);
				if (parameter is int)
				{
					if (num != -1 && num != (int)parameter)
					{
						throw new ArgumentException(SR.ExceptionTitleNameAddedIsNotUnique(value.Name));
					}
					base.List[(int)parameter] = value;
					return;
				}
				if (parameter is string)
				{
					int num2 = 0;
					foreach (Title item in base.List)
					{
						if (item.Name == (string)parameter)
						{
							if (num != -1 && num != num2)
							{
								throw new ArgumentException(SR.ExceptionTitleNameAddedIsNotUnique(value.Name));
							}
							base.List[num2] = value;
							break;
						}
						num2++;
					}
					return;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
			}
		}

		private TitleCollection()
		{
		}

		public TitleCollection(IServiceContainer serviceContainer)
		{
			this.serviceContainer = serviceContainer;
		}

		public int Add(Title title)
		{
			return base.List.Add(title);
		}

		public Title Add(string text)
		{
			Title title = new Title(text);
			base.List.Add(title);
			return title;
		}

		public Title Add(string text, Docking docking)
		{
			Title title = new Title(text, docking);
			base.List.Add(title);
			return title;
		}

		public Title Add(string text, Docking docking, Font font, Color color)
		{
			Title title = new Title(text, docking, font, color);
			base.List.Add(title);
			return title;
		}

		public void Insert(int index, Title title)
		{
			base.List.Insert(index, title);
		}

		public void Insert(int index, string text)
		{
			Title value = new Title(text);
			base.List.Insert(index, value);
		}

		public void Insert(int index, string text, Docking docking)
		{
			Title value = new Title(text, docking);
			base.List.Insert(index, value);
		}

		public void Insert(int index, string text, Docking docking, Font font, Color color)
		{
			Title value = new Title(text, docking, font, color);
			base.List.Insert(index, value);
		}

		public bool Contains(Title value)
		{
			return base.List.Contains(value);
		}

		protected override void OnSet(int index, object oldValue, object newValue)
		{
			if (!(((Title)oldValue).Name != ((Title)newValue).Name))
			{
				return;
			}
			if (((Title)newValue).Name.Length == 0 || ((Title)newValue).Name == "Chart Title")
			{
				string name = string.Empty;
				int num = 1;
				while (num < int.MaxValue)
				{
					name = "Title" + num.ToString(CultureInfo.InvariantCulture);
					num++;
					if (IsUniqueName(name))
					{
						break;
					}
				}
				((Title)newValue).Name = name;
				return;
			}
			int num2 = 0;
			while (true)
			{
				if (num2 < base.InnerList.Count)
				{
					if (num2 != index && string.Compare(((Title)base.InnerList[num2]).Name, ((Title)newValue).Name, StringComparison.Ordinal) == 0)
					{
						break;
					}
					num2++;
					continue;
				}
				return;
			}
			throw new ArgumentException(SR.ExceptionChartTitleSetIsNotUnique(((Title)newValue).Name));
		}

		protected override void OnInsert(int index, object value)
		{
			if (((Title)value).Name == "Default Title")
			{
				for (int i = 0; i < base.InnerList.Count; i++)
				{
					if (((Title)base.InnerList[i]).Name == "Default Title")
					{
						base.InnerList.RemoveAt(i);
						break;
					}
				}
			}
			if (((Title)value).Name.Length == 0 || ((Title)value).Name == "Chart Title")
			{
				string name = string.Empty;
				int num = 1;
				while (num < int.MaxValue)
				{
					name = "Title" + num.ToString(CultureInfo.InvariantCulture);
					num++;
					if (IsUniqueName(name))
					{
						break;
					}
				}
				((Title)value).Name = name;
			}
			else if (IndexOf(((Title)value).Name) != -1)
			{
				throw new ArgumentException(SR.ExceptionChartTitleAddedIsNotUnique(((Title)value).Name));
			}
		}

		protected override void OnInsertComplete(int index, object value)
		{
			((Title)value).Chart = Chart;
			Invalidate();
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			Invalidate();
		}

		protected override void OnClearComplete()
		{
			Invalidate();
		}

		private bool IsUniqueName(string name)
		{
			foreach (Title inner in base.InnerList)
			{
				if (inner.Name == name)
				{
					return false;
				}
			}
			return true;
		}

		public int IndexOf(string name)
		{
			int result = -1;
			for (int i = 0; i < base.InnerList.Count; i++)
			{
				if (string.Compare(((Title)base.InnerList[i]).Name, name, StringComparison.Ordinal) == 0)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public int GetIndex(string name)
		{
			return IndexOf(name);
		}

		public void Remove(Title title)
		{
			base.List.Remove(title);
		}

		public int IndexOf(Title title)
		{
			return base.InnerList.IndexOf(title);
		}

		private void Invalidate()
		{
		}

		internal static void CalcOutsideTitlePosition(ChartPicture chartPicture, ChartGraphics chartGraph, ChartArea area, ref RectangleF chartAreasRectangle, float elementSpacing)
		{
			if (chartPicture == null)
			{
				return;
			}
			float num = Math.Min(chartAreasRectangle.Height / 100f * elementSpacing, chartAreasRectangle.Width / 100f * elementSpacing);
			foreach (Title title in chartPicture.Titles)
			{
				if (!title.IsVisible())
				{
					continue;
				}
				if (title.DockToChartArea != "NotSet")
				{
					try
					{
						_ = chartPicture.ChartAreas[title.DockToChartArea];
					}
					catch
					{
						throw new ArgumentException(SR.ExceptionChartTitleDockedChartAreaIsMissing(title.DockToChartArea));
					}
				}
				if (title.DockInsideChartArea || !(title.DockToChartArea == area.Name) || !title.Position.Auto)
				{
					continue;
				}
				RectangleF frameTitlePosition = RectangleF.Empty;
				RectangleF rectangleF = chartAreasRectangle;
				title.CalcTitlePosition(chartGraph, ref chartAreasRectangle, ref frameTitlePosition, num);
				RectangleF rectangleF2 = title.Position.ToRectangleF();
				if (title.Docking == Docking.Top)
				{
					rectangleF2.Y -= num;
					if (!area.Position.Auto)
					{
						rectangleF2.Y -= rectangleF2.Height;
						rectangleF.Y -= rectangleF2.Height + num;
						rectangleF.Height += rectangleF2.Height + num;
					}
				}
				else if (title.Docking == Docking.Bottom)
				{
					rectangleF2.Y += num;
					if (!area.Position.Auto)
					{
						rectangleF2.Y = rectangleF.Bottom + num;
						rectangleF.Height += rectangleF2.Height + num;
					}
				}
				if (title.Docking == Docking.Left)
				{
					rectangleF2.X -= num;
					if (!area.Position.Auto)
					{
						rectangleF2.X -= rectangleF2.Width;
						rectangleF.X -= rectangleF2.Width + num;
						rectangleF.Width += rectangleF2.Width + num;
					}
				}
				if (title.Docking == Docking.Right)
				{
					rectangleF2.X += num;
					if (!area.Position.Auto)
					{
						rectangleF2.X = rectangleF.Right + num;
						rectangleF.Width += rectangleF2.Width + num;
					}
				}
				title.Position.SetPositionNoAuto(rectangleF2.X, rectangleF2.Y, rectangleF2.Width, rectangleF2.Height);
				if (!area.Position.Auto)
				{
					chartAreasRectangle = rectangleF;
				}
			}
		}

		internal static void CalcInsideTitlePosition(ChartPicture chartPicture, ChartGraphics chartGraph, float elementSpacing)
		{
			if (chartPicture == null)
			{
				return;
			}
			foreach (Title title3 in chartPicture.Titles)
			{
				if (title3.IsVisible() && title3.DockToChartArea != "NotSet")
				{
					try
					{
						_ = chartPicture.ChartAreas[title3.DockToChartArea];
					}
					catch
					{
						throw new ArgumentException(SR.ExceptionChartTitleDockedChartAreaIsMissing(title3.DockToChartArea));
					}
				}
			}
			foreach (ChartArea chartArea in chartPicture.ChartAreas)
			{
				if (!chartArea.Visible)
				{
					continue;
				}
				RectangleF chartAreasRectangle = chartArea.PlotAreaPosition.ToRectangleF();
				float elementSpacing2 = Math.Min(chartAreasRectangle.Height / 100f * elementSpacing, chartAreasRectangle.Width / 100f * elementSpacing);
				foreach (Title title4 in chartPicture.Titles)
				{
					if (title4.DockInsideChartArea && title4.DockToChartArea == chartArea.Name && title4.Position.Auto)
					{
						RectangleF frameTitlePosition = RectangleF.Empty;
						title4.CalcTitlePosition(chartGraph, ref chartAreasRectangle, ref frameTitlePosition, elementSpacing2);
					}
				}
			}
		}
	}
}
