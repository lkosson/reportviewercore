using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Visibility : IPersistable
	{
		private ExpressionInfo m_hidden;

		private string m_toggle;

		private bool m_recursiveReceiver;

		private TablixMember m_recursiveMember;

		private TextBox m_toggleSender;

		[NonSerialized]
		private bool m_isClone;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal ExpressionInfo Hidden
		{
			get
			{
				return m_hidden;
			}
			set
			{
				m_hidden = value;
			}
		}

		internal string Toggle
		{
			get
			{
				return m_toggle;
			}
			set
			{
				m_toggle = value;
			}
		}

		internal TextBox ToggleSender
		{
			get
			{
				return m_toggleSender;
			}
			set
			{
				m_toggleSender = value;
			}
		}

		internal bool RecursiveReceiver
		{
			get
			{
				return m_recursiveReceiver;
			}
			set
			{
				m_recursiveReceiver = value;
			}
		}

		internal TablixMember RecursiveMember
		{
			get
			{
				return m_recursiveMember;
			}
			set
			{
				m_recursiveMember = value;
			}
		}

		internal bool IsToggleReceiver
		{
			get
			{
				if (m_toggle != null)
				{
					return m_toggle.Length > 0;
				}
				return false;
			}
		}

		internal bool IsConditional => m_hidden != null;

		internal bool IsClone => m_isClone;

		internal void Initialize(InitializationContext context)
		{
			Initialize(context, registerVisibilityToggle: true);
		}

		internal void Initialize(InitializationContext context, bool registerVisibilityToggle)
		{
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.GenericVisibilityHidden(m_hidden);
			}
			if (registerVisibilityToggle)
			{
				RegisterVisibilityToggle(context);
			}
		}

		internal VisibilityToggleInfo RegisterVisibilityToggle(InitializationContext context)
		{
			return context.RegisterVisibilityToggle(this);
		}

		internal static SharedHiddenState GetSharedHidden(Visibility visibility)
		{
			if (visibility == null)
			{
				return SharedHiddenState.Never;
			}
			if (visibility.Toggle == null)
			{
				if (visibility.Hidden == null)
				{
					return SharedHiddenState.Never;
				}
				if (ExpressionInfo.Types.Constant == visibility.Hidden.Type)
				{
					if (visibility.Hidden.BoolValue)
					{
						return SharedHiddenState.Always;
					}
					return SharedHiddenState.Never;
				}
			}
			return SharedHiddenState.Sometimes;
		}

		internal static bool HasToggle(Visibility visibility)
		{
			return visibility?.IsToggleReceiver ?? false;
		}

		internal object PublishClone(AutomaticSubtotalContext context, bool isSubtotalMember)
		{
			Visibility visibility = null;
			if (isSubtotalMember)
			{
				visibility = new Visibility();
				visibility.m_hidden = ExpressionInfo.CreateConstExpression(value: true);
			}
			else
			{
				visibility = (Visibility)MemberwiseClone();
				if (m_hidden != null)
				{
					visibility.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
				}
				if (m_toggle != null)
				{
					context.AddVisibilityWithToggleToUpdate(visibility);
					visibility.m_toggle = (string)m_toggle.Clone();
				}
			}
			visibility.m_isClone = true;
			return visibility;
		}

		internal void UpdateToggleItemReference(AutomaticSubtotalContext context)
		{
			if (m_toggle != null)
			{
				m_toggle = context.GetNewReportItemName(m_toggle);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Toggle, Token.String));
			list.Add(new MemberInfo(MemberName.RecursiveReceiver, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ToggleSender, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox, Token.Reference));
			list.Add(new MemberInfo(MemberName.RecursiveMember, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Visibility, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Hidden:
					writer.Write(m_hidden);
					break;
				case MemberName.Toggle:
					writer.Write(m_toggle);
					break;
				case MemberName.RecursiveReceiver:
					writer.Write(m_recursiveReceiver);
					break;
				case MemberName.ToggleSender:
					writer.WriteReference(m_toggleSender);
					break;
				case MemberName.RecursiveMember:
					writer.WriteReference(m_recursiveMember);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Toggle:
					m_toggle = reader.ReadString();
					break;
				case MemberName.RecursiveReceiver:
					m_recursiveReceiver = reader.ReadBoolean();
					break;
				case MemberName.ToggleSender:
					m_toggleSender = reader.ReadReference<TextBox>(this);
					break;
				case MemberName.RecursiveMember:
					m_recursiveMember = reader.ReadReference<TablixMember>(this);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				switch (item.MemberName)
				{
				case MemberName.ToggleSender:
				{
					if (referenceableItems.TryGetValue(item.RefID, out IReferenceable value3))
					{
						m_toggleSender = (value3 as TextBox);
					}
					break;
				}
				case MemberName.RecursiveMember:
				{
					if (referenceableItems.TryGetValue(item.RefID, out IReferenceable value2))
					{
						m_recursiveMember = (value2 as TablixMember);
					}
					break;
				}
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Visibility;
		}

		internal static bool ComputeHidden(IVisibilityOwner visibilityOwner, Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, ToggleCascadeDirection direction, out bool valueIsDeep)
		{
			valueIsDeep = false;
			bool flag = false;
			Visibility visibility = visibilityOwner.Visibility;
			if (visibility != null)
			{
				switch (GetSharedHidden(visibility))
				{
				case SharedHiddenState.Always:
					flag = true;
					break;
				case SharedHiddenState.Never:
					flag = false;
					break;
				case SharedHiddenState.Sometimes:
					flag = visibilityOwner.ComputeStartHidden(renderingContext);
					if (visibility.IsToggleReceiver)
					{
						TextBox toggleSender = visibility.ToggleSender;
						Global.Tracer.Assert(toggleSender != null, "Missing Persisted Toggle Receiver -> Sender Link");
						string senderUniqueName = visibilityOwner.SenderUniqueName;
						if (senderUniqueName != null && renderingContext.IsSenderToggled(senderUniqueName))
						{
							flag = !flag;
						}
						if (!flag)
						{
							flag = ComputeDeepHidden(flag, visibilityOwner, direction, renderingContext);
						}
						valueIsDeep = true;
					}
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
			return flag;
		}

		internal static bool ComputeDeepHidden(bool hidden, IVisibilityOwner visibilityOwner, ToggleCascadeDirection direction, Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext)
		{
			Visibility visibility = visibilityOwner.Visibility;
			if (hidden && (visibility == null || !visibility.IsToggleReceiver))
			{
				hidden = false;
			}
			if (!hidden && visibility != null && visibility.IsToggleReceiver)
			{
				hidden = ((!visibility.RecursiveReceiver || !(visibilityOwner is TablixMember)) ? (hidden | visibility.ToggleSender.ComputeDeepHidden(renderingContext, direction)) : (hidden | ((TablixMember)visibilityOwner).ComputeToggleSenderDeepHidden(renderingContext)));
			}
			if (!hidden && (visibility == null || !visibility.RecursiveReceiver) && visibilityOwner.ContainingDynamicVisibility != null)
			{
				hidden |= visibilityOwner.ContainingDynamicVisibility.ComputeDeepHidden(renderingContext, direction);
			}
			if (!hidden && direction != ToggleCascadeDirection.Column && visibilityOwner.ContainingDynamicRowVisibility != null)
			{
				hidden |= visibilityOwner.ContainingDynamicRowVisibility.ComputeDeepHidden(renderingContext, direction);
			}
			if (!hidden && direction != ToggleCascadeDirection.Row && visibilityOwner.ContainingDynamicColumnVisibility != null)
			{
				hidden |= visibilityOwner.ContainingDynamicColumnVisibility.ComputeDeepHidden(renderingContext, direction);
			}
			return hidden;
		}
	}
}
