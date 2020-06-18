using System.Diagnostics;
using System.IO.Packaging;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships
{
	[DebuggerDisplay("{RelatedPart.Location} : {RelationshipId}")]
	internal class Relationship
	{
		private string _relationshipId;

		private string _relatedPart;

		private string _relationshipType;

		private TargetMode _targetMode;

		public string RelationshipId
		{
			get
			{
				return _relationshipId;
			}
			set
			{
				_relationshipId = value;
			}
		}

		public string RelatedPart
		{
			get
			{
				return _relatedPart;
			}
			set
			{
				_relatedPart = value;
			}
		}

		public string RelationshipType
		{
			get
			{
				return _relationshipType;
			}
			set
			{
				_relationshipType = value;
			}
		}

		public TargetMode Mode
		{
			get
			{
				return _targetMode;
			}
			set
			{
				_targetMode = value;
			}
		}

		public Relationship()
		{
			_targetMode = TargetMode.Internal;
		}
	}
}
