using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixHeadingInstanceList : ArrayList, ISearchByUniqueName
	{
		[NonSerialized]
		private MatrixHeadingInstance m_lastHeadingInstance;

		internal new MatrixHeadingInstance this[int index] => (MatrixHeadingInstance)base[index];

		internal MatrixHeadingInstanceList()
		{
		}

		internal MatrixHeadingInstanceList(int capacity)
			: base(capacity)
		{
		}

		internal void Add(MatrixHeadingInstance matrixHeadingInstance, ReportProcessing.ProcessingContext pc)
		{
			if (m_lastHeadingInstance != null)
			{
				m_lastHeadingInstance.InstanceInfo.HeadingSpan = matrixHeadingInstance.InstanceInfo.HeadingCellIndex - m_lastHeadingInstance.InstanceInfo.HeadingCellIndex;
				bool flag = true;
				MatrixHeading matrixHeadingDef = m_lastHeadingInstance.MatrixHeadingDef;
				if (pc.ReportItemsReferenced)
				{
					Matrix matrix = (Matrix)matrixHeadingDef.DataRegionDef;
					if (matrixHeadingDef.IsColumn)
					{
						if (matrix.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
						{
							flag = false;
						}
					}
					else if (matrix.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Row)
					{
						flag = false;
					}
				}
				if (flag)
				{
					bool flag2;
					if (m_lastHeadingInstance.IsSubtotal)
					{
						flag2 = m_lastHeadingInstance.MatrixHeadingDef.Subtotal.FirstInstance;
						m_lastHeadingInstance.MatrixHeadingDef.Subtotal.FirstInstance = false;
					}
					else
					{
						BoolList firstHeadingInstances = m_lastHeadingInstance.MatrixHeadingDef.FirstHeadingInstances;
						flag2 = firstHeadingInstances[m_lastHeadingInstance.HeadingIndex];
						firstHeadingInstances[m_lastHeadingInstance.HeadingIndex] = false;
					}
					pc.ChunkManager.AddInstance(m_lastHeadingInstance.InstanceInfo, m_lastHeadingInstance, flag2 || matrixHeadingDef.InFirstPage, pc.InPageSection);
				}
			}
			base.Add(matrixHeadingInstance);
			m_lastHeadingInstance = matrixHeadingInstance;
			matrixHeadingInstance.MatrixHeadingDef.InFirstPage = pc.ChunkManager.InFirstPage;
		}

		internal void SetLastHeadingSpan(int currentCellIndex, ReportProcessing.ProcessingContext pc)
		{
			if (m_lastHeadingInstance == null)
			{
				return;
			}
			m_lastHeadingInstance.InstanceInfo.HeadingSpan = currentCellIndex - m_lastHeadingInstance.InstanceInfo.HeadingCellIndex;
			bool flag = true;
			MatrixHeading matrixHeadingDef = m_lastHeadingInstance.MatrixHeadingDef;
			if (pc.ReportItemsReferenced)
			{
				Matrix matrix = (Matrix)matrixHeadingDef.DataRegionDef;
				if (matrixHeadingDef.IsColumn)
				{
					if (matrix.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
					{
						flag = false;
					}
				}
				else if (matrix.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Row)
				{
					flag = false;
				}
			}
			if (flag)
			{
				bool flag2;
				if (m_lastHeadingInstance.IsSubtotal)
				{
					flag2 = m_lastHeadingInstance.MatrixHeadingDef.Subtotal.FirstInstance;
					m_lastHeadingInstance.MatrixHeadingDef.Subtotal.FirstInstance = false;
				}
				else
				{
					BoolList firstHeadingInstances = m_lastHeadingInstance.MatrixHeadingDef.FirstHeadingInstances;
					flag2 = firstHeadingInstances[m_lastHeadingInstance.HeadingIndex];
					firstHeadingInstances[m_lastHeadingInstance.HeadingIndex] = false;
				}
				pc.ChunkManager.AddInstance(m_lastHeadingInstance.InstanceInfo, m_lastHeadingInstance, flag2 || matrixHeadingDef.InFirstPage, pc.InPageSection);
			}
		}

		object ISearchByUniqueName.Find(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			int count = Count;
			object obj = null;
			for (int i = 0; i < count; i++)
			{
				obj = this[i].Find(i, targetUniqueName, ref nonCompNames, chunkManager);
				if (obj != null)
				{
					break;
				}
			}
			return obj;
		}
	}
}
