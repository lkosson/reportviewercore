using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixSubtotalHeadingInstanceInfo : MatrixHeadingInstanceInfo
	{
		private object[] m_styleAttributeValues;

		internal object[] StyleAttributeValues
		{
			get
			{
				return m_styleAttributeValues;
			}
			set
			{
				m_styleAttributeValues = value;
			}
		}

		internal MatrixSubtotalHeadingInstanceInfo(ReportProcessing.ProcessingContext pc, int headingCellIndex, MatrixHeading matrixHeadingDef, MatrixHeadingInstance owner, bool isSubtotal, int reportItemDefIndex, VariantList groupExpressionValues, out NonComputedUniqueNames nonComputedUniqueNames)
			: base(pc, headingCellIndex, matrixHeadingDef, owner, isSubtotal, reportItemDefIndex, groupExpressionValues, out nonComputedUniqueNames)
		{
			Global.Tracer.Assert(isSubtotal);
			Global.Tracer.Assert(matrixHeadingDef.Subtotal != null);
			Global.Tracer.Assert(matrixHeadingDef.Subtotal.StyleClass != null);
			if (matrixHeadingDef.Subtotal.StyleClass.ExpressionList != null)
			{
				m_styleAttributeValues = new object[matrixHeadingDef.Subtotal.StyleClass.ExpressionList.Count];
				ReportProcessing.RuntimeRICollection.EvaluateStyleAttributes(ObjectType.Subtotal, matrixHeadingDef.Grouping.Name, matrixHeadingDef.Subtotal.StyleClass, owner.UniqueName, m_styleAttributeValues, pc);
			}
		}

		internal MatrixSubtotalHeadingInstanceInfo()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.StyleAttributeValues, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
