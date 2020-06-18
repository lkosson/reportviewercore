using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class ROMInstanceObjectCreator : PersistenceHelper, IRIFObjectCreator
	{
		private ReportItemInstance m_reportItemInstance;

		private ProcessingRIFObjectCreator __processingRIFObjectCreator;

		private ActionInfo m_currentActionInfo;

		private int m_currentActionIndex;

		private ParameterCollection m_currentParameterCollection;

		private int m_currentParameterIndex;

		private IRIFObjectCreator ProcessingRIFObjectCreator
		{
			get
			{
				if (__processingRIFObjectCreator == null)
				{
					__processingRIFObjectCreator = new ProcessingRIFObjectCreator(null, null);
				}
				return __processingRIFObjectCreator;
			}
		}

		internal ROMInstanceObjectCreator(ReportItemInstance reportItemInstance)
		{
			m_reportItemInstance = reportItemInstance;
		}

		internal void StartActionInfoInstancesDeserialization(ActionInfo actionInfo)
		{
			m_currentActionInfo = actionInfo;
			m_currentActionIndex = 0;
		}

		internal void CompleteActionInfoInstancesDeserialization()
		{
			m_currentActionInfo = null;
			m_currentActionIndex = 0;
		}

		internal void StartParameterInstancesDeserialization(ParameterCollection paramCollection)
		{
			m_currentParameterCollection = paramCollection;
			m_currentParameterIndex = 0;
		}

		internal void CompleteParameterInstancesDeserialization()
		{
			m_currentParameterCollection = null;
			m_currentParameterIndex = 0;
		}

		public IPersistable CreateRIFObject(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType objectType, ref IntermediateFormatReader context)
		{
			IPersistable persistable;
			switch (objectType)
			{
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Null:
				return null;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageInstance:
				persistable = (ImageInstance)m_reportItemInstance;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StyleInstance:
				persistable = m_reportItemInstance.Style;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInstance:
				Global.Tracer.Assert(m_currentActionInfo != null && m_currentActionInfo.Actions.Count > m_currentActionIndex, "Ensure m_currentActionInfo is setup properly");
				persistable = m_currentActionInfo.Actions[m_currentActionIndex].Instance;
				m_currentActionIndex++;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInstance:
				Global.Tracer.Assert(m_currentParameterCollection != null && m_currentParameterCollection.Count > m_currentParameterIndex, "Ensure m_currentParameterCollection is setup properly");
				persistable = m_currentParameterCollection[m_currentParameterIndex].Instance;
				m_currentParameterIndex++;
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInfoWithDynamicImageMap:
				persistable = new ActionInfoWithDynamicImageMap(m_reportItemInstance.RenderingContext, null, (ReportItem)m_reportItemInstance.ReportElementDef, (IROMActionOwner)m_reportItemInstance.ReportElementDef);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageMapAreaInstance:
				persistable = new ImageMapAreaInstance();
				break;
			default:
				return ProcessingRIFObjectCreator.CreateRIFObject(objectType, ref context);
			}
			persistable.Deserialize(context);
			return persistable;
		}
	}
}
