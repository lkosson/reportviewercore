using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	internal sealed class BingMapsService
	{
		private class InternalAsyncRequestState
		{
			public HttpWebRequest Request
			{
				get;
				set;
			}

			public Action<Response> ResponseCallBack
			{
				get;
				set;
			}

			public Action<Exception> ErrorCallBack
			{
				get;
				set;
			}
		}

		public static Response GetImageryMetadata(ImageryMetadataRequest imageryRequest)
		{
			return ReadResponse((HttpWebResponse)(WebRequest.Create(imageryRequest.GetRequestUrl()) as HttpWebRequest).GetResponse());
		}

		public static IAsyncResult GetImageryMetadataAsync(ImageryMetadataRequest imageryRequest, Action<Response> clientCallback, Action<Exception> clientErrorCallback)
		{
			HttpWebRequest httpWebRequest = WebRequest.Create(imageryRequest.GetRequestUrl()) as HttpWebRequest;
			InternalAsyncRequestState state = new InternalAsyncRequestState
			{
				Request = httpWebRequest,
				ResponseCallBack = clientCallback,
				ErrorCallBack = clientErrorCallback
			};
			return httpWebRequest.BeginGetResponse(RespCallback, state);
		}

		private static void RespCallback(IAsyncResult asynchronousResult)
		{
			InternalAsyncRequestState internalAsyncRequestState = (InternalAsyncRequestState)asynchronousResult.AsyncState;
			try
			{
				Response response = ReadResponse((HttpWebResponse)internalAsyncRequestState.Request.EndGetResponse(asynchronousResult));
				if (response != null)
				{
					internalAsyncRequestState.ResponseCallBack(response);
				}
				else
				{
					internalAsyncRequestState.ErrorCallBack(new Exception("Error parsing Bing Maps Response"));
				}
			}
			catch (WebException obj)
			{
				internalAsyncRequestState.ErrorCallBack(obj);
			}
		}

		private static Response ReadResponse(HttpWebResponse httpResponse)
		{
			if (httpResponse.StatusCode == HttpStatusCode.OK)
			{
				DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(Response));
				using (Stream stream = httpResponse.GetResponseStream())
				{
					return dataContractJsonSerializer.ReadObject(stream) as Response;
				}
			}
			return null;
		}
	}
}
