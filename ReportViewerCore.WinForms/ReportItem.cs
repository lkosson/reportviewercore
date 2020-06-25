using System;
using System.Collections.Generic;
using System.Text;

namespace ReportViewerCore
{
	public class ReportItem
	{
		public string Description { get; set; }
		public decimal Price { get; set; }
		public int Qty { get; set; }
		public decimal Total => Price * Qty;
	}
}
