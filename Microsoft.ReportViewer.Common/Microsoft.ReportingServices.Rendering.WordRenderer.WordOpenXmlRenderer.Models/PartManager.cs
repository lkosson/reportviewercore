using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models.Relationships;
using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser;
using System;
using System.IO;
using System.IO.Packaging;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class PartManager
	{
		private OPCRelationshipTree _relationshipTree;

		private Package _zipPackage;

		public PartManager(Package zipPackage)
		{
			_zipPackage = zipPackage;
			_relationshipTree = new OPCRelationshipTree("http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", _zipPackage);
		}

		public void Write()
		{
			try
			{
				_relationshipTree.WriteTree();
			}
			finally
			{
				Package zipPackage = _zipPackage;
				zipPackage.Flush();
				zipPackage.Close();
			}
		}

		public Relationship AddPartToTree(OoxmlPart part, string contentType, string relationshipType, string locationHint, XmlPart parent)
		{
			return _relationshipTree.AddPartToTree(part, contentType, relationshipType, locationHint, parent);
		}

		public Relationship AddStreamingPartToTree(string contentType, string relationshipType, string locationHint, XmlPart parent)
		{
			return _relationshipTree.AddStreamingPartToTree(contentType, relationshipType, locationHint, parent);
		}

		public Relationship AddStreamingRootPartToTree(string contentType, string relationshipType, string locationHint)
		{
			return _relationshipTree.AddStreamingRootPartToTree(contentType, relationshipType, locationHint);
		}

		public Relationship AddExternalPartToTree(string relationshipType, string externalTarget, XmlPart parent, TargetMode targetMode)
		{
			return _relationshipTree.AddExternalPartToTree(relationshipType, externalTarget, parent, targetMode);
		}

		public Relationship AddImageToTree(Stream data, ImageHash hash, string extension, string relationshipType, string locationHint, string parentLocation)
		{
			bool newBlob;
			Relationship relationship = _relationshipTree.AddImageToTree(hash, extension, relationshipType, locationHint, parentLocation, ContentTypeAction.Default, out newBlob);
			if (newBlob)
			{
				Stream stream = _zipPackage.GetPart(new Uri(WordOpenXmlUtils.CleanName(relationship.RelatedPart), UriKind.Relative)).GetStream();
				WordOpenXmlUtils.CopyStream(data, stream);
			}
			return relationship;
		}

		public Relationship WriteStaticRootPart(OoxmlPart part, string contentType, string relationshipType, string locationHint)
		{
			Relationship relationship = _relationshipTree.AddRootPartToTree(part, contentType, relationshipType, locationHint);
			_relationshipTree.WritePart(relationship.RelatedPart);
			return relationship;
		}

		public Relationship WriteStaticPart(OoxmlPart part, string contentType, string relationshipType, string locationHint, XmlPart parent)
		{
			Relationship relationship = _relationshipTree.AddPartToTree(part, contentType, relationshipType, locationHint, parent);
			_relationshipTree.WritePart(relationship.RelatedPart);
			return relationship;
		}

		public RelPart GetPartByContentType(string contenttype)
		{
			return _relationshipTree.GetPartByContentType(contenttype);
		}

		public XmlPart GetPartByLocation(string location)
		{
			return (XmlPart)_relationshipTree.GetPartByLocation(location);
		}

		public XmlPart GetRootPart()
		{
			return _relationshipTree.GetRootPart();
		}

		public static string CleanName(string name)
		{
			name = name.Replace('\\', '/');
			if (name.StartsWith("/", StringComparison.Ordinal) || name.StartsWith("#", StringComparison.Ordinal))
			{
				return name;
			}
			return "/" + name;
		}
	}
}
