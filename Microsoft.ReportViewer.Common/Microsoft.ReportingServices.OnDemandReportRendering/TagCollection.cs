using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TagCollection : ReportElementCollectionBase<Tag>
	{
		private readonly Image m_image;

		private readonly List<Tag> m_collection;

		public override int Count => m_collection.Count;

		public override Tag this[int i] => m_collection[i];

		internal TagCollection(Image image)
		{
			m_image = image;
			List<ExpressionInfo> tags = m_image.ImageDef.Tags;
			m_collection = new List<Tag>(tags.Count);
			for (int i = 0; i < tags.Count; i++)
			{
				m_collection.Add(new Tag(image, tags[i]));
			}
		}

		internal void SetNewContext()
		{
			for (int i = 0; i < m_collection.Count; i++)
			{
				m_collection[i].SetNewContext();
			}
		}
	}
}
