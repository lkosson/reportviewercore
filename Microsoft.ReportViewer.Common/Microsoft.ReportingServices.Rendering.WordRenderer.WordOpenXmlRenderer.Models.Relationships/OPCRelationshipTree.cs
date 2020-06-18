using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models.Relationships
{
	internal sealed class OPCRelationshipTree
	{
		private Dictionary<string, RelPart> _parts;

		private Dictionary<string, List<Relationship>> _relationships;

		private Dictionary<ImageHash, string> _blobPathsByUniqueId;

		private string _documentRootRelationshipType;

		private string _docRootLocation;

		private Package _package;

		private int maxRelationshipId = 1;

		private int maxPartId;

		public OPCRelationshipTree(string documentRootRelationshipType, Package package)
		{
			_package = package;
			_documentRootRelationshipType = documentRootRelationshipType;
			_parts = new Dictionary<string, RelPart>();
			_relationships = new Dictionary<string, List<Relationship>>();
			_blobPathsByUniqueId = new Dictionary<ImageHash, string>();
		}

		public void WritePart(string location)
		{
			PackagePart part = _package.GetPart(new Uri(WordOpenXmlUtils.CleanName(location), UriKind.Relative));
			StreamWriter streamWriter = new StreamWriter(part.GetStream());
			((XmlPart)_parts[location]).HydratedPart.Write(streamWriter);
			streamWriter.Flush();
			PhantomPart value = new PhantomPart
			{
				ContentType = part.ContentType,
				Location = location
			};
			_parts[location] = value;
		}

		public void WriteTree()
		{
			foreach (string key in _parts.Keys)
			{
				if (!(_parts[key] is PhantomPart))
				{
					WritePart(key);
				}
			}
		}

		public RelPart GetPartByContentType(string contenttype)
		{
			foreach (RelPart value in _parts.Values)
			{
				if (value.ContentType == contenttype)
				{
					return value;
				}
			}
			return null;
		}

		public RelPart GetPartByLocation(string location)
		{
			if (!_parts.TryGetValue(location, out RelPart value))
			{
				return null;
			}
			return value;
		}

		public XmlPart GetRootPart()
		{
			return (XmlPart)GetPartByLocation(_docRootLocation);
		}

		public List<Relationship> GetRelationshipsByPath(string partLocation)
		{
			if (_relationships.TryGetValue(partLocation, out List<Relationship> value))
			{
				return value;
			}
			return new List<Relationship>();
		}

		private string UniqueLocation(string locationHint)
		{
			maxPartId++;
			return string.Format(CultureInfo.InvariantCulture, locationHint, maxPartId);
		}

		private string NextRelationshipId()
		{
			maxRelationshipId++;
			return string.Format(CultureInfo.InvariantCulture, "rId{0}", maxRelationshipId);
		}

		public Relationship AddRootPartToTree(OoxmlPart part, string contentType, string relationshipType, string locationHint)
		{
			return AddRootPartToTree(part, contentType, relationshipType, locationHint, ContentTypeAction.Override);
		}

		public Relationship AddRootPartToTree(OoxmlPart part, string contentType, string relationshipType, string locationHint, ContentTypeAction ctypeAction)
		{
			Relationship relationship = AddPart(part, contentType, relationshipType, locationHint, "/", ctypeAction);
			if (relationshipType == _documentRootRelationshipType)
			{
				_docRootLocation = relationship.RelatedPart;
			}
			return relationship;
		}

		public Relationship AddPartToTree(OoxmlPart part, string contentType, string relationshipType, string locationHint, XmlPart parent)
		{
			return AddPartToTree(part, contentType, relationshipType, locationHint, parent, ContentTypeAction.Override);
		}

		public Relationship AddPartToTree(OoxmlPart part, string contentType, string relationshipType, string locationHint, XmlPart parent, ContentTypeAction ctypeAction)
		{
			return AddPart(part, contentType, relationshipType, locationHint, parent.Location, ctypeAction);
		}

		private Relationship AddPart(OoxmlPart part, string contentType, string relationshipType, string locationHint, string parentLocation, ContentTypeAction ctypeAction)
		{
			XmlPart xmlPart = new XmlPart();
			xmlPart.ContentType = contentType;
			xmlPart.HydratedPart = part;
			if (locationHint.Contains("{0}"))
			{
				xmlPart.Location = UniqueLocation(locationHint);
			}
			else
			{
				xmlPart.Location = locationHint;
			}
			_parts.Add(xmlPart.Location, xmlPart);
			_package.CreatePart(new Uri(WordOpenXmlUtils.CleanName(xmlPart.Location), UriKind.Relative), xmlPart.ContentType, CompressionOption.Normal);
			return AddRelationship(xmlPart.Location, relationshipType, parentLocation);
		}

		public Relationship AddImageToTree(ImageHash hash, string extension, string relationshipType, string locationHint, string parentLocation, ContentTypeAction ctypeAction, out bool newBlob)
		{
			PhantomPart phantomPart = new PhantomPart();
			phantomPart.ContentType = "image/" + extension;
			if (_blobPathsByUniqueId.TryGetValue(hash, out string value))
			{
				phantomPart.Location = value;
				newBlob = false;
			}
			else
			{
				int num;
				switch (extension)
				{
				default:
					num = 0;
					break;
				case "jpg":
				case "png":
				case "gif":
					num = -1;
					break;
				}
				CompressionOption compressionOption = (CompressionOption)num;
				value = string.Format(CultureInfo.InvariantCulture, UniqueLocation(locationHint), extension);
				_blobPathsByUniqueId[hash] = value;
				phantomPart.Location = value;
				_parts.Add(phantomPart.Location, phantomPart);
				_package.CreatePart(new Uri(WordOpenXmlUtils.CleanName(phantomPart.Location), UriKind.Relative), phantomPart.ContentType, compressionOption);
				newBlob = true;
			}
			return AddRelationship(phantomPart.Location, relationshipType, parentLocation);
		}

		public Relationship AddStreamingPartToTree(string contentType, string relationshipType, string locationHint, XmlPart parent)
		{
			return AddStreamingPartToTree(contentType, relationshipType, locationHint, parent, ContentTypeAction.Override);
		}

		public Relationship AddStreamingPartToTree(string contentType, string relationshipType, string locationHint, XmlPart parent, ContentTypeAction ctypeAction)
		{
			return AddStreamingPartToTree(contentType, relationshipType, locationHint, parent.Location, ctypeAction);
		}

		public Relationship AddStreamingPartToTree(string contentType, string relationshipType, string locationHint, string parentLocation, ContentTypeAction ctypeAction)
		{
			string location = UniqueLocation(locationHint);
			PhantomPart phantomPart = new PhantomPart();
			phantomPart.ContentType = contentType;
			phantomPart.Location = location;
			_parts.Add(phantomPart.Location, phantomPart);
			_package.CreatePart(new Uri(WordOpenXmlUtils.CleanName(phantomPart.Location), UriKind.Relative), phantomPart.ContentType, CompressionOption.Normal);
			return AddRelationship(location, relationshipType, parentLocation);
		}

		public Relationship AddStreamingRootPartToTree(string contentType, string relationshipType, string locationHint)
		{
			return AddStreamingRootPartToTree(contentType, relationshipType, locationHint, ContentTypeAction.Override);
		}

		public Relationship AddStreamingRootPartToTree(string contentType, string relationshipType, string locationHint, ContentTypeAction ctypeAction)
		{
			Relationship relationship = AddStreamingPartToTree(contentType, relationshipType, locationHint, "/", ctypeAction);
			if (relationshipType == _documentRootRelationshipType)
			{
				_docRootLocation = relationship.RelatedPart;
			}
			return relationship;
		}

		public Relationship AddExternalPartToTree(string relationshipType, string externalLocation, XmlPart parent, TargetMode targetMode)
		{
			return AddRelationship(externalLocation, relationshipType, parent.Location, targetMode);
		}

		private Relationship AddRelationship(string location, string relationshipType, string parentLocation)
		{
			return AddRelationship(location, relationshipType, parentLocation, TargetMode.Internal);
		}

		private Relationship AddRelationship(string location, string relationshipType, string parentLocation, TargetMode mode)
		{
			if (!_relationships.ContainsKey(parentLocation) || _relationships[parentLocation] == null)
			{
				_relationships[parentLocation] = new List<Relationship>();
			}
			else
			{
				foreach (Relationship item in _relationships[parentLocation])
				{
					if (item.RelatedPart == location)
					{
						return item;
					}
				}
			}
			Relationship relationship = new Relationship();
			relationship.RelatedPart = location;
			relationship.RelationshipId = NextRelationshipId();
			relationship.RelationshipType = relationshipType;
			relationship.Mode = mode;
			_relationships[parentLocation].Add(relationship);
			Uri targetUri = (relationship.Mode != 0) ? new Uri(relationship.RelatedPart, UriKind.Absolute) : new Uri(WordOpenXmlUtils.CleanName(relationship.RelatedPart), UriKind.Relative);
			if (parentLocation == "/")
			{
				_package.CreateRelationship(targetUri, relationship.Mode, relationship.RelationshipType, relationship.RelationshipId);
			}
			else
			{
				_package.GetPart(new Uri(WordOpenXmlUtils.CleanName(parentLocation), UriKind.Relative)).CreateRelationship(targetUri, relationship.Mode, relationship.RelationshipType, relationship.RelationshipId);
			}
			return relationship;
		}
	}
}
