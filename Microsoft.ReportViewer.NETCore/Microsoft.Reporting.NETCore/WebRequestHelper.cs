using Microsoft.ReportingServices.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

namespace Microsoft.Reporting.NETCore
{
	internal static class WebRequestHelper
	{
		private const string InfoQuery = "rs:MoreInformation";

		private const string SPUserTokenParam = "rs:TrustedUserToken";

		private const string SPUserNameParam = "rs:TrustedUserName";

		public static HttpWebRequest GetServerUrlAccessObject(string url, int timeout, ICredentials credentials, Cookie formsAuthCookie, IEnumerable<string> headers, IEnumerable<Cookie> cookies, string userName, string bearerToken, byte[] userToken)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Credentials = credentials;
			httpWebRequest.Timeout = timeout;
			if (!string.IsNullOrEmpty(bearerToken))
			{
				httpWebRequest.Credentials = null;
				httpWebRequest.Headers.Add("Authorization", $"Bearer {bearerToken}");
			}
			SetRequestHeaders(httpWebRequest, formsAuthCookie, headers, cookies);
			if (userToken != null && !string.IsNullOrEmpty(userName))
			{
				string input = Convert.ToBase64String(userToken);
				string s = string.Format(CultureInfo.InvariantCulture, "{0}={1}&{2}={3}", "rs:TrustedUserName", UrlUtil.UrlEncode(userName), "rs:TrustedUserToken", UrlUtil.UrlEncode(input));
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "application/x-www-form-urlencoded";
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				using (Stream stream = httpWebRequest.GetRequestStream())
				{
					stream.Write(bytes, 0, bytes.Length);
					return httpWebRequest;
				}
			}
			return httpWebRequest;
		}

		public static void SetRequestHeaders(HttpWebRequest request, Cookie formsAuthCookie, IEnumerable<string> headers, IEnumerable<Cookie> cookies)
		{
			request.Headers.Add("Accept-Language", Thread.CurrentThread.CurrentCulture.Name);
			request.CookieContainer = new CookieContainer();
			if (formsAuthCookie != null)
			{
				request.CookieContainer.Add(formsAuthCookie);
			}
			if (cookies != null)
			{
				foreach (Cookie cooky in cookies)
				{
					request.CookieContainer.Add(cooky);
				}
			}
			if (headers == null)
			{
				return;
			}
			foreach (string header in headers)
			{
				request.Headers.Add(header);
			}
		}

		public static ReportServerException ExceptionFromWebResponse(Exception e)
		{
			return ReportServerException.FromException(ExceptionFromWebResponseUnwrapped(e));
		}

		private static Exception ExceptionFromWebResponseUnwrapped(Exception e)
		{
			IOException ex = e as IOException;
			WebException ex2 = e as WebException;
			if (ex != null)
			{
				SocketException ex3 = ex.InnerException as SocketException;
				if (ex3 != null && ex3.SocketErrorCode == SocketError.Interrupted)
				{
					return new OperationCanceledException();
				}
			}
			else if (ex2 != null)
			{
				if (ex2.Status == WebExceptionStatus.RequestCanceled)
				{
					return new OperationCanceledException();
				}
				if (ex2.Response != null)
				{
					Stream responseStream = ex2.Response.GetResponseStream();
					try
					{
						XmlDocument xmlDocument = new XmlDocument();
						XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
						xmlReaderSettings.CheckCharacters = false;
						XmlReader reader = XmlReader.Create(responseStream, xmlReaderSettings);
						xmlDocument.Load(reader);
						XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
						xmlNamespaceManager.AddNamespace("rs", "http://www.microsoft.com/sql/reportingservices");
						if (xmlDocument.DocumentElement != null)
						{
							Exception ex4 = ReportServerException.FromMoreInformationNode(xmlDocument.DocumentElement.SelectSingleNode("rs:MoreInformation", xmlNamespaceManager));
							if (ex4 != null)
							{
								return ex4;
							}
							return e;
						}
						return e;
					}
					catch (Exception)
					{
						return e;
					}
					finally
					{
						responseStream.Close();
					}
				}
			}
			return e;
		}
	}
}
