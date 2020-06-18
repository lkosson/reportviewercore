using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ParameterInstance : BaseInstance, IPersistable
	{
		private bool m_omit;

		private object m_value;

		[NonSerialized]
		private bool m_isOldSnapshot;

		[NonSerialized]
		private bool m_valueReady;

		[NonSerialized]
		private bool m_omitReady;

		[NonSerialized]
		private bool m_omitAssigned;

		[NonSerialized]
		private Parameter m_parameterDef;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		public object Value
		{
			get
			{
				if (!m_isOldSnapshot && !m_valueReady)
				{
					m_valueReady = true;
					if (!m_parameterDef.Value.IsExpression)
					{
						m_value = m_parameterDef.Value.Value;
					}
					else if (m_parameterDef.ActionDef.Owner.ReportElementOwner == null || m_parameterDef.ActionDef.Owner.ReportElementOwner.CriOwner == null)
					{
						ActionInfo owner = m_parameterDef.ActionDef.Owner;
						m_value = m_parameterDef.ActionDef.ActionItemDef.EvaluateDrillthroughParamValue(ReportScopeInstance, owner.RenderingContext.OdpContext, owner.InstancePath, owner.ROMActionOwner.FieldsUsedInValueExpression, m_parameterDef.ParameterDef, owner.ObjectType, owner.ObjectName);
					}
				}
				return m_value;
			}
			set
			{
				ReportElement reportElementOwner = m_parameterDef.ActionDef.Owner.ReportElementOwner;
				Global.Tracer.Assert(m_parameterDef.Value != null, "(m_parameterDef.Value != null)");
				if (!m_parameterDef.ActionDef.Owner.IsChartConstruction && (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_parameterDef.Value.IsExpression)))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				if (value != null)
				{
					if (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition)
					{
						if (!(value is string))
						{
							throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWritebackStringExpected);
						}
					}
					else
					{
						bool flag;
						if (value is object[])
						{
							object[] obj = (object[])value;
							flag = true;
							object[] array = obj;
							for (int i = 0; i < array.Length; i++)
							{
								if (!ReportRuntime.IsVariant(array[i]))
								{
									flag = false;
									break;
								}
							}
						}
						else
						{
							flag = ReportRuntime.IsVariant(value);
						}
						if (!flag)
						{
							throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
						}
					}
				}
				m_valueReady = true;
				m_value = value;
			}
		}

		public bool Omit
		{
			get
			{
				if (!m_isOldSnapshot && !m_omitReady)
				{
					m_omitReady = true;
					if (!m_parameterDef.Omit.IsExpression)
					{
						m_omit = m_parameterDef.Omit.Value;
					}
					else if (m_parameterDef.ActionDef.Owner.ReportElementOwner == null || m_parameterDef.ActionDef.Owner.ReportElementOwner.CriOwner == null)
					{
						ActionInfo owner = m_parameterDef.ActionDef.Owner;
						m_omit = m_parameterDef.ActionDef.ActionItemDef.EvaluateDrillthroughParamOmit(ReportScopeInstance, owner.RenderingContext.OdpContext, owner.InstancePath, m_parameterDef.ParameterDef, owner.ObjectType, owner.ObjectName);
					}
				}
				return m_omit;
			}
			set
			{
				ReportElement reportElementOwner = m_parameterDef.ActionDef.Owner.ReportElementOwner;
				Global.Tracer.Assert(m_parameterDef.Omit != null, "(m_parameterDef.Omit != null)");
				if (!m_parameterDef.ActionDef.Owner.IsChartConstruction && (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_parameterDef.Omit.IsExpression)))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				m_omitReady = true;
				m_omitAssigned = true;
				m_omit = value;
			}
		}

		internal bool IsOmitAssined => m_omitAssigned;

		internal ParameterInstance(ActionItemInstance actionInstance, int index)
			: base(null)
		{
			m_isOldSnapshot = true;
			SetMembers(actionInstance, index);
		}

		internal ParameterInstance(Parameter parameterDef)
			: base(parameterDef.ActionDef.Owner.ReportScope)
		{
			m_isOldSnapshot = false;
			m_parameterDef = parameterDef;
		}

		private void SetMembers(ActionItemInstance actionInstance, int index)
		{
			m_value = null;
			m_omit = false;
			if (actionInstance != null)
			{
				if (actionInstance.DrillthroughParametersValues != null)
				{
					m_value = actionInstance.DrillthroughParametersValues[index];
				}
				if (actionInstance.DrillthroughParametersOmits != null)
				{
					m_omit = actionInstance.DrillthroughParametersOmits[index];
				}
			}
		}

		internal void Update(ActionItemInstance actionInstance, int index)
		{
			SetMembers(actionInstance, index);
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
		}

		protected override void ResetInstanceCache()
		{
			m_omitAssigned = false;
			m_omitReady = false;
			m_valueReady = false;
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Value:
				{
					object obj2 = null;
					if (m_parameterDef.Value.IsExpression)
					{
						obj2 = Value;
					}
					writer.Write(obj2);
					break;
				}
				case MemberName.Omit:
				{
					object obj = null;
					if (m_parameterDef.Omit.IsExpression && IsOmitAssined)
					{
						obj = Omit;
					}
					writer.Write(obj);
					break;
				}
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Value:
				{
					object obj2 = reader.ReadVariant();
					if (m_parameterDef.Value.IsExpression)
					{
						m_valueReady = true;
						m_value = obj2;
					}
					else
					{
						Global.Tracer.Assert(obj2 == null, "(value == null)");
					}
					break;
				}
				case MemberName.Omit:
				{
					object obj = reader.ReadVariant();
					if (m_parameterDef.Omit.IsExpression && obj != null)
					{
						m_omitReady = true;
						m_omit = (bool)obj;
					}
					else
					{
						Global.Tracer.Assert(obj == null, "(omit == null)");
					}
					break;
				}
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInstance;
		}

		private static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Value, Token.Object));
			list.Add(new MemberInfo(MemberName.Omit, Token.Object));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}
	}
}
