using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixHeadingInstance : InstanceInfoOwner, IShowHideContainer
	{
		private int m_uniqueName;

		private ReportItemInstance m_content;

		private MatrixHeadingInstanceList m_subHeadingInstances;

		private bool m_isSubtotal;

		private RenderingPagesRangesList m_renderingPages;

		[NonSerialized]
		[Reference]
		private MatrixHeading m_matrixHeadingDef;

		[NonSerialized]
		private int m_headingDefIndex;

		internal int UniqueName
		{
			get
			{
				return m_uniqueName;
			}
			set
			{
				m_uniqueName = value;
			}
		}

		internal MatrixHeading MatrixHeadingDef
		{
			get
			{
				return m_matrixHeadingDef;
			}
			set
			{
				m_matrixHeadingDef = value;
			}
		}

		internal ReportItemInstance Content
		{
			get
			{
				return m_content;
			}
			set
			{
				m_content = value;
			}
		}

		internal MatrixHeadingInstanceList SubHeadingInstances
		{
			get
			{
				return m_subHeadingInstances;
			}
			set
			{
				m_subHeadingInstances = value;
			}
		}

		internal bool IsSubtotal
		{
			get
			{
				return m_isSubtotal;
			}
			set
			{
				m_isSubtotal = value;
			}
		}

		internal MatrixHeadingInstanceInfo InstanceInfo
		{
			get
			{
				if (m_instanceInfo is OffsetInfo)
				{
					Global.Tracer.Assert(condition: false, string.Empty);
					return null;
				}
				return (MatrixHeadingInstanceInfo)m_instanceInfo;
			}
		}

		internal int HeadingIndex
		{
			get
			{
				return m_headingDefIndex;
			}
			set
			{
				m_headingDefIndex = value;
			}
		}

		internal RenderingPagesRangesList ChildrenStartAndEndPages
		{
			get
			{
				return m_renderingPages;
			}
			set
			{
				m_renderingPages = value;
			}
		}

		internal MatrixHeadingInstance(ReportProcessing.ProcessingContext pc, int headingCellIndex, MatrixHeading matrixHeadingDef, bool isSubtotal, int reportItemDefIndex, VariantList groupExpressionValues, out NonComputedUniqueNames nonComputedUniqueNames)
		{
			m_uniqueName = pc.CreateUniqueName();
			if (isSubtotal && matrixHeadingDef.Subtotal.StyleClass != null)
			{
				m_instanceInfo = new MatrixSubtotalHeadingInstanceInfo(pc, headingCellIndex, matrixHeadingDef, this, isSubtotal, reportItemDefIndex, groupExpressionValues, out nonComputedUniqueNames);
				if (matrixHeadingDef.GetInnerStaticHeading() != null)
				{
					m_subHeadingInstances = new MatrixHeadingInstanceList();
				}
			}
			else
			{
				m_instanceInfo = new MatrixHeadingInstanceInfo(pc, headingCellIndex, matrixHeadingDef, this, isSubtotal, reportItemDefIndex, groupExpressionValues, out nonComputedUniqueNames);
				if (matrixHeadingDef.SubHeading != null)
				{
					m_subHeadingInstances = new MatrixHeadingInstanceList();
				}
			}
			m_renderingPages = new RenderingPagesRangesList();
			m_matrixHeadingDef = matrixHeadingDef;
			m_isSubtotal = isSubtotal;
			m_headingDefIndex = reportItemDefIndex;
			if (!matrixHeadingDef.IsColumn)
			{
				pc.Pagination.EnterIgnoreHeight(matrixHeadingDef.StartHidden);
			}
			if (matrixHeadingDef.FirstHeadingInstances == null)
			{
				int count = matrixHeadingDef.ReportItems.Count;
				matrixHeadingDef.FirstHeadingInstances = new BoolList(count);
				for (int i = 0; i < count; i++)
				{
					matrixHeadingDef.FirstHeadingInstances.Add(true);
				}
			}
		}

		internal MatrixHeadingInstance()
		{
		}

		void IShowHideContainer.BeginProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.BeginProcessContainer(m_uniqueName, m_matrixHeadingDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(m_uniqueName, m_matrixHeadingDef.Visibility);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Content, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance));
			memberInfoList.Add(new MemberInfo(MemberName.SubHeadingInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.IsSubtotal, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ChildrenStartAndEndPages, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RenderingPagesRangesList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}

		internal object Find(int index, int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			object obj = null;
			ReportItemCollection reportItemCollection = (!IsSubtotal) ? MatrixHeadingDef.ReportItems : MatrixHeadingDef.Subtotal.ReportItems;
			if (reportItemCollection.Count > 0)
			{
				if (reportItemCollection.Count == 1)
				{
					index = 0;
				}
				if (reportItemCollection.IsReportItemComputed(index))
				{
					Global.Tracer.Assert(m_content != null, "The instance of a computed report item cannot be null.");
					obj = ((ISearchByUniqueName)m_content).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
				else
				{
					NonComputedUniqueNames nonCompNames2 = GetInstanceInfo(chunkManager).ContentUniqueNames;
					obj = ((ISearchByUniqueName)reportItemCollection[index]).Find(targetUniqueName, ref nonCompNames2, chunkManager);
					if (obj != null)
					{
						nonCompNames = nonCompNames2;
						return obj;
					}
				}
			}
			if (m_subHeadingInstances != null)
			{
				return ((ISearchByUniqueName)m_subHeadingInstances).Find(targetUniqueName, ref nonCompNames, chunkManager);
			}
			return null;
		}

		internal MatrixHeadingInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (m_instanceInfo is OffsetInfo)
			{
				return chunkManager.GetReader(((OffsetInfo)m_instanceInfo).Offset).ReadMatrixHeadingInstanceInfoBase();
			}
			return (MatrixHeadingInstanceInfo)m_instanceInfo;
		}
	}
}
