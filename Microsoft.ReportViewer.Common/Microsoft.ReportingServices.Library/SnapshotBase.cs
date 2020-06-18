using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.IO;

namespace Microsoft.ReportingServices.Library
{
	[Serializable]
	internal abstract class SnapshotBase
	{
		private Guid m_snapshotDataID;

		private bool m_isPermanentSnapshot;

		internal Guid SnapshotDataID => m_snapshotDataID;

		internal bool IsPermanentSnapshot => m_isPermanentSnapshot;

		protected SnapshotBase(Guid snapshotDataID, bool isPermanentSnapshot)
		{
			m_snapshotDataID = snapshotDataID;
			m_isPermanentSnapshot = isPermanentSnapshot;
		}

		protected SnapshotBase(bool isPermanentSnapshot)
		{
			m_snapshotDataID = Guid.NewGuid();
			m_isPermanentSnapshot = isPermanentSnapshot;
		}

		protected SnapshotBase(SnapshotBase snapshotDataToCopy)
		{
			m_snapshotDataID = snapshotDataToCopy.SnapshotDataID;
			m_isPermanentSnapshot = snapshotDataToCopy.m_isPermanentSnapshot;
		}

		public abstract SnapshotBase Duplicate();

		[Obsolete("Use PrepareExecutionSnapshot instead")]
		public abstract void CopyImageChunksTo(SnapshotBase target);

		public abstract void PrepareExecutionSnapshot(SnapshotBase target, string compiledDefinitionChunkName);

		public abstract Stream GetChunk(string name, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, out string mimeType);

		public abstract string GetStreamMimeType(string name, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type);

		public abstract Stream CreateChunk(string name, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, string mimeType);

		public abstract void DeleteSnapshotAndChunks();

		internal virtual void UpdatePerfData(Stream chunk)
		{
		}

		internal virtual void WritePerfData()
		{
		}
	}
}
