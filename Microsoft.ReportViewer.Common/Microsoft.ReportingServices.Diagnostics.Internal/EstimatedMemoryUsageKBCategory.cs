using System.Xml.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Internal
{
	public sealed class EstimatedMemoryUsageKBCategory
	{
		public long? Pagination
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool PaginationSpecified => Pagination.HasValue;

		public long? Rendering
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool RenderingSpecified => Rendering.HasValue;

		public long? Processing
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool ProcessingSpecified => Processing.HasValue;

		internal EstimatedMemoryUsageKBCategory()
		{
		}
	}
}
