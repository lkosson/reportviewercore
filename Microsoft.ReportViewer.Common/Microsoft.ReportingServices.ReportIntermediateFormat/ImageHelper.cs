using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.IO;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal static class ImageHelper
	{
		internal static string StoreImageDataInChunk(Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes chunkType, byte[] imageData, string mimeType, OnDemandMetadata odpMetadata, IChunkFactory chunkFactory)
		{
			string text = GenerateImageStreamName();
			_ = odpMetadata.ReportSnapshot;
			using (Stream stream = chunkFactory.CreateChunk(text, chunkType, mimeType))
			{
				stream.Write(imageData, 0, imageData.Length);
				return text;
			}
		}

		internal static string GenerateImageStreamName()
		{
			return Guid.NewGuid().ToString("N");
		}
	}
}
