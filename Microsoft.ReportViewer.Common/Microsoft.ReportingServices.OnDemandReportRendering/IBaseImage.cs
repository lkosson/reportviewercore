using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IBaseImage
	{
		Image.SourceType Source
		{
			get;
		}

		ReportProperty Value
		{
			get;
		}

		ReportStringProperty MIMEType
		{
			get;
		}

		string ImageDataPropertyName
		{
			get;
		}

		string ImageValuePropertyName
		{
			get;
		}

		string MIMETypePropertyName
		{
			get;
		}

		Image.EmbeddingModes EmbeddingMode
		{
			get;
		}

		ObjectType ObjectType
		{
			get;
		}

		string ObjectName
		{
			get;
		}

		byte[] GetImageData(out List<string> fieldsUsedInValue, out bool errorOccurred);

		string GetMIMETypeValue();

		string GetValueAsString(out List<string> fieldsUsedInValue, out bool errorOccured);

		string GetTransparentImageProperties(out string mimeType, out byte[] imageData);
	}
}
