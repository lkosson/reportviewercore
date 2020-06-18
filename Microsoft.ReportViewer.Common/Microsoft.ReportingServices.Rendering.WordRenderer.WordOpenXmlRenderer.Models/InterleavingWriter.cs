using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class InterleavingWriter
	{
		public delegate void FinalWriteInterleaver(IInterleave interleaver, TextWriter output);

		public delegate T CreateInterleaver<T>(int index, long location) where T : IInterleave;

		private Stream _stream;

		private IList<IInterleave> _interleavingContent;

		private TextWriter _textWriter;

		private IInterleave _currentInterleaver;

		public TextWriter TextWriter => _textWriter;

		public long Location
		{
			get
			{
				TextWriter.Flush();
				return _stream.Position;
			}
		}

		public InterleavingWriter(Stream stream, ScalabilityCache scalabilityCache)
			: this(stream)
		{
			_interleavingContent = new ScalableList<IInterleave>(1, scalabilityCache, 113, 13);
		}

		public InterleavingWriter(Stream stream)
		{
			_stream = stream;
			_textWriter = new StreamWriter(_stream);
			_interleavingContent = new List<IInterleave>();
		}

		public T WriteInterleaver<T>(CreateInterleaver<T> createInterleaver) where T : IInterleave
		{
			TextWriter.Flush();
			StoreInterleaver();
			T val = createInterleaver(_interleavingContent.Count, _stream.Position);
			_currentInterleaver = val;
			return val;
		}

		private void StoreInterleaver()
		{
			if (_currentInterleaver != null)
			{
				_interleavingContent.Add(_currentInterleaver);
				_currentInterleaver = null;
			}
		}

		public void CommitInterleaver(IInterleave interleaver)
		{
			if (_currentInterleaver == interleaver)
			{
				StoreInterleaver();
			}
			else
			{
				_interleavingContent[interleaver.Index] = interleaver;
			}
		}

		public void Interleave(Stream output, FinalWriteInterleaver writeInterleaver)
		{
			_textWriter.Flush();
			StoreInterleaver();
			_stream.Seek(0L, SeekOrigin.Begin);
			long num = 0L;
			foreach (IInterleave item in _interleavingContent)
			{
				WordOpenXmlUtils.CopyStream(_stream, output, item.Location - num);
				num = item.Location;
				StringBuilder stringBuilder = new StringBuilder();
				using (StringWriter output2 = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
				{
					writeInterleaver(item, output2);
				}
				byte[] bytes = _textWriter.Encoding.GetBytes(stringBuilder.ToString());
				output.Write(bytes, 0, bytes.Length);
			}
			WordOpenXmlUtils.CopyStream(_stream, output, _stream.Length - num);
		}

		public static InterleavingWriter CreateInterleavingWriterForTesting(Stream stream, Dictionary<string, string> namespaces)
		{
			return new InterleavingWriter(stream);
		}
	}
}
