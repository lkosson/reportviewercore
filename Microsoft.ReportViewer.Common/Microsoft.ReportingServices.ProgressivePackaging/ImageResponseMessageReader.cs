using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal class ImageResponseMessageReader : ImageMessageReader<ImageResponseMessageElement>
	{
		private readonly IMessageReader m_reader;

		private readonly IEnumerator<MessageElement> m_enumerator;

		private bool m_hasCurrentElement;

		private bool m_isEnumeratorEmpty;

		public ImageResponseMessageReader(IMessageReader reader)
		{
			m_reader = reader;
			m_enumerator = reader.GetEnumerator();
		}

		public bool PeekIsErrorResponse(out MessageElement error)
		{
			error = null;
			MessageElement messageElement = Peek();
			if (ProgressiveTypeDictionary.IsErrorMessageElement(messageElement))
			{
				error = messageElement;
				return true;
			}
			return false;
		}

		private MessageElement Peek()
		{
			if (!m_hasCurrentElement && !m_isEnumeratorEmpty)
			{
				m_hasCurrentElement = m_enumerator.MoveNext();
			}
			if (m_hasCurrentElement)
			{
				return m_enumerator.Current;
			}
			m_isEnumeratorEmpty = true;
			return null;
		}

		public override IEnumerator<ImageResponseMessageElement> GetEnumerator()
		{
			while (true)
			{
				MessageElement messageElement = Peek();
				m_hasCurrentElement = false;
				if (messageElement != null)
				{
					yield return ReadImageResponseFromMessageElement(messageElement);
					continue;
				}
				break;
			}
		}

		private ImageResponseMessageElement ReadImageResponseFromMessageElement(MessageElement messageElement)
		{
			Stream stream = messageElement.Value as Stream;
			if (!"getExternalImagesResponse".Equals(messageElement.Name) || stream == null)
			{
				throw new InvalidOperationException("MessageElement is not an image response message element.");
			}
			ImageResponseMessageElement imageResponseMessageElement = new ImageResponseMessageElement();
			using (BinaryReader reader = new BinaryReader(stream, MessageUtil.StringEncoding))
			{
				imageResponseMessageElement.Read(reader);
				return imageResponseMessageElement;
			}
		}

		public override void InternalDispose()
		{
			m_reader.Dispose();
			m_enumerator.Dispose();
		}
	}
}
