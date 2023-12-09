using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal class ReportToolBar : UserControl
	{
		public class ToolStripButtonOverride : ToolStripButton
		{
			public override bool CanSelect => Enabled;
		}

		private bool m_ignoreZoomEvents;

		private ReportViewer m_currentViewerControl;

		private ToolStripButton firstPage;

		private ToolStripButton previousPage;

		private ToolStripTextBox currentPage;

		private ToolStripLabel labelOf;

		private ToolStripLabel totalPages;

		private ToolStripButton nextPage;

		private ToolStripButton lastPage;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripButton back;

		private ToolStripButton stop;

		private ToolStripButton refresh;

		private ToolStripSeparator toolStripSeparator3;

		private ToolStripButton print;

		private ToolStripButton printPreview;

		private ToolStripSeparator separator4;

		private ToolStrip toolStrip1;

		private ToolStripComboBox zoom;

		private ToolStripTextBox textToFind;

		private ToolStripButton find;

		private ToolStripSeparator toolStripSeparator4;

		private ToolStripButton findNext;

		private ToolStripButton pageSetup;

		private ToolStripDropDownButton export;

		public override Size MinimumSize
		{
			get
			{
				return GetIdealSize();
			}
			set
			{
			}
		}

		public override Size MaximumSize
		{
			get
			{
				return GetIdealSize();
			}
			set
			{
			}
		}

		internal ReportViewer ViewerControl
		{
			get
			{
				return m_currentViewerControl;
			}
			set
			{
				if (m_currentViewerControl != null)
				{
					m_currentViewerControl.StatusChanged -= OnReportViewerStateChanged;
				}
				m_currentViewerControl = value;
				if (m_currentViewerControl != null)
				{
					m_currentViewerControl.StatusChanged += OnReportViewerStateChanged;
				}
			}
		}

		public event ZoomChangedEventHandler ZoomChange;

		public event PageNavigationEventHandler PageNavigation;

		public event ExportEventHandler Export;

		public event SearchEventHandler Search;

		public event EventHandler ReportRefresh;

		public event EventHandler Print;

		public event EventHandler Back;

		public event EventHandler PageSetup;

		public ReportToolBar()
		{
			InitializeComponent();
			using (Bitmap image = new Bitmap(1, 1))
			{
				using (Graphics graphics = Graphics.FromImage(image))
				{
					currentPage.Width = graphics.MeasureString("12345", currentPage.Font).ToSize().Width;
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				currentPage.TextBox.Visible = false;
				textToFind.TextBox.Visible = false;
			}
			base.Dispose(disposing);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!base.DesignMode)
			{
				ApplyLocalizedResources();
			}
			base.OnLoad(e);
		}

		internal void ApplyCustomResources()
		{
			firstPage.ToolTipText = LocalizationHelper.Current.FirstPageButtonToolTip;
			previousPage.ToolTipText = LocalizationHelper.Current.PreviousPageButtonToolTip;
			currentPage.ToolTipText = LocalizationHelper.Current.CurrentPageTextBoxToolTip;
			labelOf.Text = LocalizationHelper.Current.PageOf;
			totalPages.ToolTipText = LocalizationHelper.Current.TotalPagesToolTip;
			nextPage.ToolTipText = LocalizationHelper.Current.NextPageButtonToolTip;
			lastPage.ToolTipText = LocalizationHelper.Current.LastPageButtonToolTip;
			back.ToolTipText = LocalizationHelper.Current.BackButtonToolTip;
			stop.ToolTipText = LocalizationHelper.Current.StopButtonToolTip;
			refresh.ToolTipText = LocalizationHelper.Current.RefreshButtonToolTip;
			print.ToolTipText = LocalizationHelper.Current.PrintButtonToolTip;
			printPreview.ToolTipText = LocalizationHelper.Current.PrintLayoutButtonToolTip;
			pageSetup.ToolTipText = LocalizationHelper.Current.PageSetupButtonToolTip;
			export.ToolTipText = LocalizationHelper.Current.ExportButtonToolTip;
			zoom.ToolTipText = LocalizationHelper.Current.ZoomControlToolTip;
			textToFind.ToolTipText = LocalizationHelper.Current.SearchTextBoxToolTip;
			find.Text = LocalizationHelper.Current.FindButtonText;
			find.ToolTipText = LocalizationHelper.Current.FindButtonToolTip;
			findNext.Text = LocalizationHelper.Current.FindNextButtonText;
			findNext.ToolTipText = LocalizationHelper.Current.FindNextButtonToolTip;
		}

		private void ApplyLocalizedResources()
		{
			firstPage.AccessibleName = ReportPreviewStrings.FirstPageAccessibleName;
			previousPage.AccessibleName = ReportPreviewStrings.PreviousPageAccessibleName;
			currentPage.AccessibleName = ReportPreviewStrings.CurrentPageAccessibleName;
			nextPage.AccessibleName = ReportPreviewStrings.NextPageAccessibleName;
			lastPage.AccessibleName = ReportPreviewStrings.LastPageAccessibleName;
			back.AccessibleName = ReportPreviewStrings.BackAccessibleName;
			stop.AccessibleName = ReportPreviewStrings.StopAccessibleName;
			refresh.AccessibleName = ReportPreviewStrings.RefreshAccessibleName;
			print.AccessibleName = ReportPreviewStrings.PrintAccessibleName;
			printPreview.AccessibleName = ReportPreviewStrings.PrintPreviewAccessibleName;
			pageSetup.AccessibleName = ReportPreviewStrings.PageSetupAccessibleName;
			export.AccessibleDescription = ReportPreviewStrings.ExportAccessibleDescription;
			export.AccessibleName = ReportPreviewStrings.ExportAccessibleName;
			zoom.AccessibleName = ReportPreviewStrings.ZoomAccessibleName;
			textToFind.AccessibleName = ReportPreviewStrings.SearchTextBoxAccessibleName;
			find.AccessibleName = ReportPreviewStrings.FindAccessibleName;
			findNext.AccessibleName = ReportPreviewStrings.FindNextAccessibleName;
			base.AccessibleName = ReportPreviewStrings.ReportToolBarAccessibleName;
			ApplyCustomResources();
		}

		internal void SetToolStripRenderer(ToolStripRenderer renderer)
		{
			toolStrip1.Renderer = renderer;
		}

		private Size GetIdealSize()
		{
			Size result = base.Size;
			if (toolStrip1 != null && base.Parent != null)
			{
				result = new Size(base.Parent.Width, toolStrip1.PreferredSize.Height);
			}
			return result;
		}

		private void InitializeComponent()
		{
			firstPage = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			previousPage = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			currentPage = new System.Windows.Forms.ToolStripTextBox();
			labelOf = new System.Windows.Forms.ToolStripLabel();
			totalPages = new System.Windows.Forms.ToolStripLabel();
			nextPage = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			lastPage = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			back = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			stop = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			refresh = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			toolStrip1 = new System.Windows.Forms.ToolStrip();
			toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			print = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			printPreview = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			pageSetup = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			export = new System.Windows.Forms.ToolStripDropDownButton();
			separator4 = new System.Windows.Forms.ToolStripSeparator();
			zoom = new System.Windows.Forms.ToolStripComboBox();
			textToFind = new System.Windows.Forms.ToolStripTextBox();
			find = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			findNext = new Microsoft.Reporting.WinForms.ReportToolBar.ToolStripButtonOverride();
			toolStrip1.SuspendLayout();
			SuspendLayout();
			firstPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			firstPage.Image = new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAABX0lEQVQ4T2OgG7h7/+Z/KBMTFM56/r9p/vP/Gf0PsCq6ee/2//B27HJgkDvz2f9VR95jVXTt9u3/8d0P/gc3XMNtQMaUp/8X7vuAoej0ZaDmngf/l+7//9+/+gJuAxJ6nvyftuU9iqKTl27/j2i/D9acOev/f6/SE7gNiOx69L939Qe4IpDm0NZ7YM2t64C2d/7975J/ALcBgc0P/9fNfwdWtP/U1f/BLXf/L9r7/38HUHPGnJ//7ep+/rfP2I7bAI+q+//zp72BK1q1/dR/u+Lb/9Pn/P7v3/Xtv07hu/8WCetxG2Bfevd/9uTXKIrW7z71Xz/l/H+Tohf/VbI+/TeOXIbbAIu8O/9T+19jKNp95Ox/g6QT/2XTvv7XDZ6H2wDDrNv/w1peYlW0ed/p//qpl/9r+E7DbYBO2o3/ftUPcSpau+v0fxWPCbgNcMy5DFaAT9GanadxGzCAgIEBAAzl7FJZZhZDAAAAAElFTkSuQmCC")));
			firstPage.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			firstPage.Name = "firstPage";
			firstPage.RightToLeftAutoMirrorImage = true;
			firstPage.Size = new System.Drawing.Size(23, 22);
			firstPage.Click += new System.EventHandler(OnPageNavButtonClick);
			previousPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			previousPage.Image = new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAA60lEQVQ4T2MY3ODmvdv/w9sf/IdySQPXbt/+H9/94H9wwzXSDTh9Gai558H/pfv///evvkCaAScv3f4f0X4frDlz1v//XqUniDcApDm09R5Yc+s6oO2df/+75B8gzoD9p67+D265+3/R3v//O4CaM+b8/G9X9/O/fcZ24l2wavup/3bFt/+nz/n937/r23+dwnf/LRLWkxYG63ef+q+fcv6/SdGL/ypZn/4bRy4jzQAQ2H3k7H+DpBP/ZdO+/tcNnke6ASCwed/p//qpl/9r+E4jzwAQWLvr9H8VjwnkGwACa3aepswAGgMGBgBBFoi1/0HwSQAAAABJRU5ErkJggg==")));
			previousPage.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			previousPage.Name = "previousPage";
			previousPage.RightToLeftAutoMirrorImage = true;
			previousPage.Size = new System.Drawing.Size(23, 22);
			previousPage.Click += new System.EventHandler(OnPageNavButtonClick);
			currentPage.AcceptsReturn = true;
			currentPage.AcceptsTab = true;
			currentPage.MaxLength = 10;
			currentPage.Name = "currentPage";
			currentPage.Size = new System.Drawing.Size(40, 25);
			currentPage.WordWrap = false;
			currentPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(CurrentPage_KeyPress);
			labelOf.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			labelOf.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			labelOf.Name = "labelOf";
			labelOf.Size = new System.Drawing.Size(0, 22);
			totalPages.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			totalPages.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			totalPages.Name = "totalPages";
			totalPages.Size = new System.Drawing.Size(0, 22);
			totalPages.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			nextPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			nextPage.Image = new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAA2UlEQVQ4T2OgKrh7/+Z/KJM8UDjr+f+b926Tb0juzGf/E7rv/b92m0xDMqY8/T9v/+//cR03/5++TIYhCT1P/s/e9+t/59a//91KL/8/eYlEQyK7Hv2fsu3X/5Klf/8nTP/73yb3LGmGBDY//N+69j1Ys3HJl//S0df+G0cu+79m7xXiDPGouv+/ZO47uGZFl57/q7afIt4F9qV3/we2PINrXr+bBM0gYJF3579NwRWw5t1HzpKmGQQMs26DNW/ed5p0zSCgk3bj/9pdZGoGgTU7KdA8QICBAQD+b4sqkggKoQAAAABJRU5ErkJggg==")));
			nextPage.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			nextPage.Name = "nextPage";
			nextPage.RightToLeftAutoMirrorImage = true;
			nextPage.Size = new System.Drawing.Size(23, 22);
			nextPage.Click += new System.EventHandler(OnPageNavButtonClick);
			lastPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			lastPage.Image = new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAABXUlEQVQ4T2OgCrh7/+Z/KJM8UDjr+f+b925jNQQk1zT/+f+M/ge4Lcmd+ex/Qve9/9duYxoCklt15P3/8HY8BmRMefp/3v7f/+M6bv4/fRnVEJDcwn0f/gc3XMNtQELPk/+z9/3637n173+30sv/T15CGAKSm7bl/X//6gu4DYjsevR/yrZf/0uW/v2fMP3vf5vcs3BDQHK9qz/89yo9gduAwOaH/1vXvgdrNi758l86+tp/48hl/9fsvfIfJFc3/91/l/wDuA3wqLr/v2TuO7hmRZee/6u2nwJrAMnlT3vz3z5jO24D7Evv/g9seQbXvH43RDMIgOSyJ7/+b5GwHrcBFnl3/tsUXAFr3n3kLIpCkFxq/2uwl6BCmMAw6zZY8+Z9pzEUgeTCW1781w2eh9sAnbQb/9fuwtQMAiA5v+qH/zV8p+E2YM1O7JpBwDHn8n8VjwlgDBUaVICBAQDe4eszTGiWXQAAAABJRU5ErkJggg==")));
			lastPage.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			lastPage.Name = "lastPage";
			lastPage.RightToLeftAutoMirrorImage = true;
			lastPage.Size = new System.Drawing.Size(23, 22);
			lastPage.Click += new System.EventHandler(OnPageNavButtonClick);
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			back.Image = new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAA8klEQVQ4T2MYBbhBTP1ZlbI5L/5DuaQB3egN6n5lR/8Xz3xKugFaIRtVtCPW/c+e9f1/1oTb/+NbL/7P6Lv1v2Dao/8EXQTTHNj89H/+3B//a1f8/9+67t//efv+/l92+Pf/ohlP8BsA0uyYd+y/T/WN/8EtT/9H9nz6Hzf5z/+KpX//T9/553/OpHv4DVD2nmZsGLbov1Pekf8eZRf/+9ff+x/W8eZ/zITv/zs3/P2f2n2dcJiADNEKWPDfPmv/f82Q1f/V/Rf9V/We9V/ZfdL/2ObzxAUqyBANv7n/VX3nEqcBGwAZouI5jXwDhgpgYAAA+u6CcalyZfsAAAAASUVORK5CYII=")));
			back.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			back.Name = "back";
			back.RightToLeftAutoMirrorImage = true;
			back.Size = new System.Drawing.Size(23, 22);
			back.Click += new System.EventHandler(OnBack);
			stop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			stop.Image = new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAACBklEQVQ4T41TTU/bQBD1n6py5xdw6wWk3vtxKAjRnvslpEpVCwFLpYBQaYBWCik0pDEEA4oQCkGQEJPQfBA72E0tIdwYChi97qyN20AOGelpd8fzxm92ZoWbllf1QEguiy+nFfS8z6B3NAPak6+kmgEv7LaZphmYShTFhyM7+Jb6iaJh4/cfB9aZw/fko4RSuip6lH9G5LdzBXxa1RjhCucOWiJfszEmVRGcPwBxPLogTElF8aOswT5HW6DYWVYSJxdUo4tkm40rnJyhLRwdX+KRuIsSuy9hkv09sllnCcCRGn+H8N07HJU9hfukvm5+ptWoW9z3OWm4Kp6HFKTLp9BP4CM59IITog86sTnmJowzsqpbfgxxiCv0fcggb1xCPUYT5IF+X0n0ficqNavpO3GIK1BrlvYdrP6ADzljYf6xK5swd68Dy7LSFBPPeQlIRmT7FOtF+Fh87ZXwrB+xoFsCJVnLWn5MeMvGqxlWAg3P6HIdG2VwfPfIEUZI5iyOBU8NrXSmuOGYjvA6u0QaXWpJsuQgVWVdaANrB24bK0e/uvgshBJlMbioYaeGtjAU1fDlepDIaCwHIwVxJKZhu+pgT0dLSNkG3nw9hLhQaP0eSAlJm5QNrCg2tg4vkKpcIJFrYGLF4I8pkdZuk/83Gm2aMLrhJ+NZPJ3IYmB2n0v2a/ZNEP4CWS3b5mYdkBkAAAAASUVORK5CYII=")));
			stop.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			stop.Name = "stop";
			stop.Size = new System.Drawing.Size(23, 22);
			stop.Click += new System.EventHandler(OnStopClick);
			refresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			refresh.Image = new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAB70lEQVQ4T22TX0/TUBjG+4nARO654ZKP4FeQG8AE7o3EeEFMQJqAQNA5QRMYihKGY4MQQvYnsMnGsGx0xdXGjKArCJQ8nOdsp0jZm/ySp+f0eXr+vNWCVajYbeF4SX/2Po/e8Sz6JrKg5pgh5pqv3a9ardYWihn647EdLCV/wbBd/P3n4c+5JzXHGBhNm3rTcls0v1wo4l3CEoZrXHiQdIU6fU32j11MRk2MfjoAPU27poWihv42bsG9wB0YQILjfHdObEmajbLTzWX/rl/j9BwSZSQdL9olao78PLlCj74LeSYz4uuRbUcEQEJTz26/5OHQA2lWc//zYdNurOJpOI906QzVU2DSfCMDqAnNSgehh17tyess9u0rVE4aAfwyNYk4S74OQg+9Gq9mteAh8QMtYajSDFfPK3vNAC4jkjnDhoGWcBtKc3sMoJ5PuRiaFVtg80x8c7BVwj1o5kGqQ2WAmnu1XMX8hjhEXiOvZPPQQ9KED82EJoWaWz9oXGPZcrplL4RjJX30q4WdY9xBGYPjI19EIyXKj6SZxbYcWSxibNlCxvTwvQofBigdzdUxvHgE/XOx9f/AlXBpM3Eba3kXqaNLJMuXiO3VMb1my58plqncfrlVsT3ZYTzhgakcBqdzeD5XwEcx5u/ZL027AahOz3nvXF5TAAAAAElFTkSuQmCC")));
			refresh.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			refresh.Name = "refresh";
			refresh.Size = new System.Drawing.Size(23, 22);
			refresh.Click += new System.EventHandler(OnRefresh);
			toolStrip1.AccessibleName = "Toolstrip";
			toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
			toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[22]
			{
				firstPage,
				previousPage,
				currentPage,
				labelOf,
				totalPages,
				nextPage,
				lastPage,
				toolStripSeparator2,
				back,
				stop,
				refresh,
				toolStripSeparator3,
				print,
				printPreview,
				pageSetup,
				export,
				separator4,
				zoom,
				textToFind,
				find,
				toolStripSeparator4,
				findNext
			});
			toolStrip1.Location = new System.Drawing.Point(0, 0);
			toolStrip1.Name = "toolStrip1";
			toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			toolStrip1.Size = new System.Drawing.Size(714, 25);
			toolStrip1.TabIndex = 3;
			toolStrip1.TabStop = true;
			toolStripSeparator3.Name = "toolStripSeparator3";
			toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			print.Image = new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAABt0lEQVQ4T5WTPU/CUBSG+7v0F6iggzg4okyCJBgTE1wQBCwtSKuU4CAYFxIj+DEoAzKRsEHChDqTiMEwkCBg+9p7oAiJH3iSZ+m573NvTk6578orpzHO3pBh++9iISoN6H+oxOqm538CTdOInh7u9bXpBKfJJJYtFno2C6tM0FfRHQqWLSuQ5KOfRaIYht3uIAELM7o9Fe86TODYdMLr800KqtUqLi8zhCzL+g06kkREoxJCfAg8zxPB4AECgSBkSYYoiri/uwOXSJwMBqbXYTSKWq32K759P9rtNmGzOcFJuq3T6aDVaiGVOkOlUvkVRVHQbDbRaDQGgnAkgnK5jFKppD8vgHQ6PYHtAiMymSzc7l0Ui0Xk8/mBQBAE5HK5HwXZbJaCBobg+vrmSyAIR/qQJLhcW1PBzno8/NcMkskU4vE4ZmZmiW1/HPV6HVfPb7h9eiWsLt+oz84zRjM4jsXog3HALZ7TIlVfALP7kQRsD4y+GD4kSPBQKCAkRLC25sDcvAmmxaWRwLjdEJjMS1hYMMFq3aCwElO+30omoE1UQZvImOpfMMq1f4L1nRCFxhm2x4rjPgGq/jNVSPBBDwAAAABJRU5ErkJggg==")));
			print.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			print.Name = "print";
			print.Size = new System.Drawing.Size(23, 22);
			print.Click += new System.EventHandler(OnPrint);
			printPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			printPreview.Image = new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAACPklEQVQ4T6WT3U+SYRjG3z/Bkw4866ROO5HDWtNaszWjhoLIcAa6ogWD5SKa78jISitKClJRF4zoDZFBGSCJH2hBBQWjQR9jy9Q+1uyktrbarnqeJ1gHOQ66tt/Zff3ud3vul3v1rgD7TB9sMQus0dMYuM+jL2SCOdANk0+Pbu8x6NyHcWRcDZWzHcobbZBfl6HF1oxkMQmOlJ8uP8LLzwWEFooVJuN5ihDNwB1KwumbpzhuxXD5YT/4iAlyvQwc2UzK2Q/PaLFaRI0q8LGTULuVOKDaD84aMSP/KYcnqykEZl/8Gds4RHBiygDlSBt2SRrA9U/1IPM+jcXleUw8yNKh6GKxwt3f0olojkJCBIZJLRQOKUT1deAsQSOSK0uYfhOBN5ymQ//Kt+/Aj59MoBU0kA5KsHXbFnC8/zhmSzMIFQNVv4BIiEDj7kLzJTFqN9eCMwo6hF/fgy/vrfoF61+ZoGv0EMTnmlCzqQac3qNBsOCHOztOn4vEFcxUGL7zGBecCcraOhN0OJTY17uXCY7e7ISQ92Ak7cCYf4EK/g7ZvPYFKKwCpY9MoLgqR2PPHiboHO2A6/kYbCkrhoQ4LZU3Es445sAPximFFSaQDbRgt7GBCdqHFBhO23Fx6TzsnmkqKG8tb06XGLm3TCA5exD1hp1MILe34lrqCixzZiog5WoCsbkJO3TbmYD8FL0xHprbathcYTpQDVIWKeuYIJFPoPWUFHKtjN42OU9yYeRIyDuToY3wCl5w/xeO+wV7l7et3v3gxwAAAABJRU5ErkJggg==")));
			printPreview.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			printPreview.Name = "printPreview";
			printPreview.Size = new System.Drawing.Size(23, 22);
			printPreview.Click += new System.EventHandler(OnPrintPreviewClick);
			pageSetup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			pageSetup.Image = new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAACMklEQVQ4T8WSW2sTQRiG82e0RU2rpYog6o0i9kJFQZAKigfUCxUPVHojCFaiFVNJm5QkG5LmZNJoWtKKQYnaakQrxmqDmoNtQrs50OxukjXmoL7OzkZDEdQLwRe+i2Hmeb6Zj1H8k9zoN0Fn9qLf6IJaa4W0rm/9Oc67L8E4xvA+EsECyyKRTMJitcM45Pk7iSRIzieR43kIBQGZTBbRWAx9GiNGfGO0PN4RONzDMA/ZMGgwQa3RolXZKjdwjk8hu5ilsFRsmkVoehrHLqhQrX2j9an8FXyxhjRXwVyqDANjbgisvufgBB4FsQA+z+NDNIoHgQAVcByHEoW/IJWrYjb1Ge8SJWgNTENg8T6BWBKRLxaQyWbwNhyG584ojp6/QgVSZxkuUzgUFaHR6RsC0+3HqNQqZAYcYvE43szMQG+yYN+Ji1SQ5gicrsMREcFwgcxH1xDoXQGUqxUscjnEP84iMDEJxmLDjgNdVCC9+UdnCX4YEtDbp0HLqhZZoHPcR4UI5tkFTASDGPWNw+p0Y8veU+AFYSn8WoB/ioeqV90QDNj8CDx6ioNnerD7UBduMi443R4iOEkFrwj8LFwknfO4R2BfkMNl1XUoV9YF3dfMWLv9MFZv3oMVbRvRvrUTHZ1nsWHnccwlEgSWrp2H/4UMeydzuNRzlQiUsoDCm3bJi3rWb9uPdR1Hfn4k+61hMhc7NIMMef8ATp/rRtua9iXML2la1oTm5c2/rfrR/xqF4juGl/ozJ73NXgAAAABJRU5ErkJggg==")));
			pageSetup.ImageTransparentColor = System.Drawing.Color.Magenta;
			pageSetup.Name = "pageSetup";
			pageSetup.Size = new System.Drawing.Size(23, 22);
			pageSetup.Click += new System.EventHandler(OnPageSetupClick);
			export.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			export.Image = new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAACIklEQVQ4T62Ra0iTARRA/RkhCIkQYRhYhCUZGBOCJqWoZFoYUtOFmWmlgcIYtImZkDEls1phTm2KWE7b7LGWzlKxlOYgH+1HI6yBpPkoTd2QxJ0mLkL8CIzO/3Pu5V6f/8JTi5NHL1y0vnbS2TeLuWeaZx1TGNrG0RnHqNOPcFHZT4qyl7zcQbzaH0xdLmqqFrF/WEQYNwmpJvbFFiEWF6wNPG6fp7JinoEBp1dYwe2GWedPRsbniE1tQ6n4LBxoNs2ivv0dq3XGqy7LbuZcK7JteBKxxEy6rE848ODJDCWqUayWKa8Oi+4lvv1w8Wl0mn77V0ITW0jKbBUOaJumqNJMYvAcy/pugm7LmOeIDhqMdu7q3qPQPse/NJo4qV44cKv6Cz1v4HrZBFnnh5Fm2EmSDhJ9/C0Rh7sIKkkmMreYyBNa4YBKPeKZDg31IFOayJY1kpFTy8nTd8jM0RCiiyJGUo8oUSMcKCx1oL0PN28scOxUARv8fElOz6Z36CPX1JWEvTxIaJ6csIQy4vqOrg3Irwxzr2KJgsuTxEvyORBzhPLqZhY8b6xrakHUcwi/qyKC4/PZ9SocUbd4dURRaeeSykGE1MiWyDSKaw2U64zUGNuRyuTst0SRbstih3kPZ4bOcdZ2gZ0de9du8jeC9btJGUxD5BAT2hnO5vptBJlD1hfZ1LiVwIfb8VcH4lsUsD75N77FAWws9P83eZnVso/PL0cQoHaggexKAAAAAElFTkSuQmCC")));
			export.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			export.Name = "export";
			export.Size = new System.Drawing.Size(29, 22);
			export.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(OnExport);
			separator4.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			separator4.Name = "separator4";
			separator4.Size = new System.Drawing.Size(6, 25);
			zoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			zoom.Margin = new System.Windows.Forms.Padding(7, 0, 1, 0);
			zoom.MaxDropDownItems = 9;
			zoom.Name = "zoom";
			zoom.Size = new System.Drawing.Size(110, 25);
			zoom.SelectedIndexChanged += new System.EventHandler(OnZoomChanged);
			textToFind.AcceptsReturn = true;
			textToFind.Margin = new System.Windows.Forms.Padding(10, 0, 1, 0);
			textToFind.Name = "textToFind";
			textToFind.Size = new System.Drawing.Size(75, 25);
			textToFind.KeyPress += new System.Windows.Forms.KeyPressEventHandler(textToFind_KeyPress);
			textToFind.TextChanged += new System.EventHandler(textToFind_TextChanged);
			find.Enabled = false;
			find.ForeColor = System.Drawing.Color.Blue;
			find.Margin = new System.Windows.Forms.Padding(3, 1, 1, 2);
			find.Name = "find";
			find.Size = new System.Drawing.Size(23, 22);
			find.Click += new System.EventHandler(find_Click);
			toolStripSeparator4.AutoSize = false;
			toolStripSeparator4.Name = "toolStripSeparator4";
			toolStripSeparator4.Size = new System.Drawing.Size(6, 20);
			findNext.Enabled = false;
			findNext.ForeColor = System.Drawing.Color.Blue;
			findNext.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
			findNext.Name = "findNext";
			findNext.Size = new System.Drawing.Size(23, 22);
			findNext.Click += new System.EventHandler(findNext_Click);
			BackColor = System.Drawing.SystemColors.Control;
			base.Controls.Add(toolStrip1);
			base.Name = "ReportToolBar";
			base.Size = new System.Drawing.Size(714, 25);
			toolStrip1.ResumeLayout(false);
			toolStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		private void OnZoomChanged(object sender, EventArgs e)
		{
			if (!m_ignoreZoomEvents && this.ZoomChange != null)
			{
				ZoomItem zoomItem = (ZoomItem)zoom.SelectedItem;
				ZoomChangeEventArgs e2 = new ZoomChangeEventArgs(zoomItem.ZoomMode, zoomItem.ZoomPercent);
				this.ZoomChange(this, e2);
			}
		}

		private void OnPageNavigation(int newPage)
		{
			if (this.PageNavigation != null)
			{
				PageNavigationEventArgs e = new PageNavigationEventArgs(newPage);
				this.PageNavigation(this, e);
			}
		}

		private void OnExport(object sender, ToolStripItemClickedEventArgs e)
		{
			if (this.Export != null)
			{
				ReportExportEventArgs e2 = new ReportExportEventArgs((RenderingExtension)e.ClickedItem.Tag);
				this.Export(this, e2);
			}
		}

		private void OnSearch(object sender, SearchEventArgs se)
		{
			if (this.Search != null)
			{
				this.Search(this, se);
			}
		}

		private void OnRefresh(object sender, EventArgs e)
		{
			if (this.ReportRefresh != null)
			{
				this.ReportRefresh(this, EventArgs.Empty);
			}
		}

		private void OnPrint(object sender, EventArgs e)
		{
			if (this.Print != null)
			{
				toolStrip1.Capture = false;
				this.Print(this, EventArgs.Empty);
			}
		}

		private void OnBack(object sender, EventArgs e)
		{
			if (this.Back != null)
			{
				this.Back(this, e);
			}
		}

		private void OnPageSetupClick(object sender, EventArgs e)
		{
			if (this.PageSetup != null)
			{
				this.PageSetup(this, EventArgs.Empty);
			}
		}

		public void SetZoom()
		{
			try
			{
				m_ignoreZoomEvents = true;
				ZoomMenuHelper.Populate(zoom, ViewerControl.ZoomMode, ViewerControl.ZoomPercent);
			}
			finally
			{
				m_ignoreZoomEvents = false;
			}
		}

		private void PopulateExportList()
		{
			RenderingExtension[] extensions = ViewerControl.Report.ListRenderingExtensions();
			RenderingExtensionsHelper.Populate(export, null, extensions);
		}

		private void OnPageNavButtonClick(object sender, EventArgs e)
		{
			if (sender == firstPage)
			{
				OnPageNavigation(1);
			}
			else if (sender == previousPage)
			{
				OnPageNavigation(ViewerControl.CurrentPage - 1);
			}
			else if (sender == nextPage)
			{
				OnPageNavigation(ViewerControl.CurrentPage + 1);
			}
			else if (sender == lastPage)
			{
				PageCountMode pageCountMode;
				int newPage = ViewerControl.GetTotalPages(out pageCountMode);
				if (pageCountMode != 0)
				{
					OnPageNavigation(int.MaxValue);
				}
				else
				{
					OnPageNavigation(newPage);
				}
			}
		}

		private void find_Click(object sender, EventArgs e)
		{
			OnSearch(sender, new SearchEventArgs(textToFind.Text, ViewerControl.CurrentPage, isFindNext: false));
		}

		private void findNext_Click(object sender, EventArgs e)
		{
			OnSearch(sender, new SearchEventArgs(textToFind.Text, ViewerControl.CurrentPage, isFindNext: true));
		}

		private void textToFind_TextChanged(object sender, EventArgs e)
		{
			find.Enabled = !string.IsNullOrEmpty(textToFind.Text);
			findNext.Enabled = false;
		}

		private void textToFind_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r' && textToFind.Text.Length > 0)
			{
				if (!findNext.Enabled)
				{
					find_Click(sender, null);
				}
				else
				{
					findNext_Click(sender, null);
				}
			}
		}

		private void OnPrintPreviewClick(object sender, EventArgs e)
		{
			try
			{
				ViewerControl.SetDisplayMode((!printPreview.Checked) ? DisplayMode.PrintLayout : DisplayMode.Normal);
			}
			catch (Exception e2)
			{
				ViewerControl.UpdateUIState(e2);
			}
		}

		private void OnStopClick(object sender, EventArgs e)
		{
			try
			{
				ViewerControl.CancelRendering(0);
			}
			catch (Exception e2)
			{
				ViewerControl.UpdateUIState(e2);
			}
		}

		private void OnReportViewerStateChanged(object sender, EventArgs e)
		{
			ReportViewer reportViewer = (ReportViewer)sender;
			PageCountMode pageCountMode = PageCountMode.Actual;
			int num = 0;
			try
			{
				num = ViewerControl.GetTotalPages(out pageCountMode);
			}
			catch (Exception e2)
			{
				if (!ViewerControl.CurrentStatus.IsInFailedState)
				{
					ViewerControl.UpdateUIState(e2);
				}
			}
			if (num < 1)
			{
				totalPages.Text = "";
			}
			else
			{
				totalPages.Text = LocalizationHelper.Current.TotalPages(num, pageCountMode);
			}
			ReportViewerStatus currentStatus = reportViewer.CurrentStatus;
			if (currentStatus.CanNavigatePages)
			{
				currentPage.Text = ViewerControl.CurrentPage.ToString(CultureInfo.CurrentCulture);
			}
			bool flag = ViewerControl.CurrentPage <= 1;
			bool flag2 = ViewerControl.CurrentPage >= num && pageCountMode != PageCountMode.Estimate;
			firstPage.Enabled = (currentStatus.CanNavigatePages && !flag);
			previousPage.Enabled = (currentStatus.CanNavigatePages && !flag);
			currentPage.Enabled = currentStatus.CanNavigatePages;
			nextPage.Enabled = (currentStatus.CanNavigatePages && !flag2);
			lastPage.Enabled = (currentStatus.CanNavigatePages && !flag2);
			back.Enabled = currentStatus.CanNavigateBack;
			stop.Enabled = currentStatus.InCancelableOperation;
			refresh.Enabled = currentStatus.CanRefreshData;
			print.Enabled = currentStatus.CanPrint;
			printPreview.Enabled = currentStatus.CanChangeDisplayModes;
			printPreview.Checked = (reportViewer.DisplayMode == DisplayMode.PrintLayout);
			pageSetup.Enabled = print.Enabled;
			export.Enabled = currentStatus.CanExport;
			zoom.Enabled = currentStatus.CanChangeZoom;
			textToFind.Enabled = currentStatus.CanSearch;
			find.Enabled = (textToFind.Enabled && !string.IsNullOrEmpty(textToFind.Text));
			findNext.Enabled = currentStatus.CanContinueSearch;
			if (currentStatus.CanSearch && ViewerControl.SearchState != null)
			{
				textToFind.Text = ViewerControl.SearchState.Text;
			}
			bool showPageNavigationControls = reportViewer.ShowPageNavigationControls;
			firstPage.Visible = showPageNavigationControls;
			previousPage.Visible = showPageNavigationControls;
			currentPage.Visible = showPageNavigationControls;
			labelOf.Visible = showPageNavigationControls;
			totalPages.Visible = showPageNavigationControls;
			nextPage.Visible = showPageNavigationControls;
			lastPage.Visible = showPageNavigationControls;
			toolStripSeparator2.Visible = showPageNavigationControls;
			back.Visible = reportViewer.ShowBackButton;
			stop.Visible = reportViewer.ShowStopButton;
			refresh.Visible = reportViewer.ShowRefreshButton;
			toolStripSeparator3.Visible = (back.Visible || stop.Visible || refresh.Visible);
			print.Visible = reportViewer.ShowPrintButton;
			printPreview.Visible = reportViewer.ShowPrintButton;
			pageSetup.Visible = (print.Visible || printPreview.Visible);
			export.Visible = reportViewer.ShowExportButton;
			separator4.Visible = (print.Visible || printPreview.Visible || export.Visible);
			zoom.Visible = reportViewer.ShowZoomControl;
			bool showFindControls = reportViewer.ShowFindControls;
			toolStripSeparator4.Visible = showFindControls;
			find.Visible = showFindControls;
			findNext.Visible = showFindControls;
			textToFind.Visible = showFindControls;
			if (export.Visible && export.Enabled)
			{
				PopulateExportList();
			}
		}

		private void CurrentPage_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r' && currentPage.Text.Length > 0)
			{
				bool flag = false;
				if (int.TryParse(currentPage.Text, out int result) && ViewerControl.CanMoveToPage(result))
				{
					flag = true;
					OnPageNavigation(result);
				}
				if (!flag)
				{
					currentPage.Text = ViewerControl.CurrentPage.ToString(CultureInfo.CurrentCulture);
				}
				e.Handled = true;
			}
			else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
			{
				e.Handled = true;
			}
		}
	}
}
