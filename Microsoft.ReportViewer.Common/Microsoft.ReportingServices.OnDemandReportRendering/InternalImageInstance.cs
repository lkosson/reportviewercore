using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalImageInstance : ImageInstance
	{
		private ActionInfoWithDynamicImageMapCollection m_actionInfoImageMapAreas;

		private readonly ImageDataHandler m_imageDataHandler;

		public override byte[] ImageData
		{
			get
			{
				return m_imageDataHandler.ImageData;
			}
			set
			{
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
			}
		}

		public override string StreamName
		{
			get
			{
				return m_imageDataHandler.StreamName;
			}
			internal set
			{
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
			}
		}

		public override string MIMEType
		{
			get
			{
				return m_imageDataHandler.MIMEType;
			}
			set
			{
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		public override TypeCode TagDataType
		{
			get
			{
				if (base.ImageDef.Tags != null)
				{
					return base.ImageDef.Tags[0].Instance.DataType;
				}
				return TypeCode.Empty;
			}
		}

		public override object Tag
		{
			get
			{
				if (base.ImageDef.Tags != null)
				{
					return base.ImageDef.Tags[0].Instance.Value;
				}
				return null;
			}
		}

		internal override string ImageDataId => m_imageDataHandler.ImageDataId;

		public override ActionInfoWithDynamicImageMapCollection ActionInfoWithDynamicImageMapAreas
		{
			get
			{
				if (m_actionInfoImageMapAreas == null)
				{
					m_actionInfoImageMapAreas = new ActionInfoWithDynamicImageMapCollection();
				}
				return m_actionInfoImageMapAreas;
			}
		}

		internal override bool IsNullImage => m_imageDataHandler.IsNullImage;

		internal InternalImageInstance(Image reportItemDef)
			: base(reportItemDef)
		{
			m_imageDataHandler = ImageDataHandlerFactory.Create(m_reportElementDef, reportItemDef);
		}

		internal override List<string> GetFieldsUsedInValueExpression()
		{
			return m_imageDataHandler.FieldsUsedInValue;
		}

		public override ActionInfoWithDynamicImageMap CreateActionInfoWithDynamicImageMap()
		{
			throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_actionInfoImageMapAreas = null;
			m_imageDataHandler.ClearCache();
		}

		internal string LoadAndCacheTransparentImage(out string mimeType, out byte[] imageData)
		{
			return m_imageDataHandler.LoadAndCacheTransparentImage(out mimeType, out imageData);
		}
	}
}
