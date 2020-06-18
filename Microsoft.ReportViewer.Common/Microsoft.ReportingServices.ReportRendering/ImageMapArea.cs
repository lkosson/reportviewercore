using Microsoft.ReportingServices.ReportProcessing;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ImageMapArea
	{
		public enum ImageMapAreaShape
		{
			Rectangle,
			Polygon,
			Circle
		}

		private string m_id;

		private ImageMapAreaShape m_shape = ImageMapAreaShape.Polygon;

		private float[] m_coordinates;

		private ActionInfo m_actionInfo;

		private MemberBase m_members;

		public string ID
		{
			get
			{
				return m_id;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				m_id = value;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (!IsCustomControl && Rendering.m_mapAreaInstance != null)
				{
					actionInfo = new ActionInfo(Rendering.m_mapAreaInstance.Action, Rendering.m_mapAreaInstance.ActionInstance, Rendering.m_mapAreaInstance.UniqueName.ToString(CultureInfo.InvariantCulture), Rendering.m_renderingContext);
					if (Rendering.m_renderingContext.CacheState)
					{
						m_actionInfo = actionInfo;
					}
				}
				return actionInfo;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				m_actionInfo = value;
			}
		}

		public ImageMapAreaShape Shape => m_shape;

		public float[] Coordinates => m_coordinates;

		private bool IsCustomControl => m_members.IsCustomControl;

		private ImageMapAreaRendering Rendering
		{
			get
			{
				Global.Tracer.Assert(!m_members.IsCustomControl);
				ImageMapAreaRendering imageMapAreaRendering = m_members as ImageMapAreaRendering;
				if (imageMapAreaRendering == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return imageMapAreaRendering;
			}
		}

		public ImageMapArea()
		{
			m_members = new ImageMapAreaProcessing();
		}

		internal ImageMapArea(ImageMapAreaInstance mapAreaInstance, RenderingContext renderingContext)
		{
			m_members = new ImageMapAreaRendering();
			Rendering.m_mapAreaInstance = mapAreaInstance;
			Rendering.m_renderingContext = renderingContext;
			if (mapAreaInstance != null)
			{
				m_id = mapAreaInstance.ID;
				m_shape = mapAreaInstance.Shape;
				m_coordinates = mapAreaInstance.Coordinates;
			}
		}

		public void SetCoordinates(ImageMapAreaShape shape, float[] coordinates)
		{
			if (!IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			if (coordinates == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "coordinates");
			}
			m_shape = shape;
			m_coordinates = coordinates;
		}

		internal ImageMapArea DeepClone()
		{
			Global.Tracer.Assert(IsCustomControl);
			ImageMapArea imageMapArea = new ImageMapArea();
			imageMapArea.m_members = null;
			imageMapArea.m_shape = m_shape;
			if (m_id != null)
			{
				imageMapArea.m_id = string.Copy(m_id);
			}
			if (m_coordinates != null)
			{
				imageMapArea.m_coordinates = new float[m_coordinates.Length];
				m_coordinates.CopyTo(imageMapArea.m_coordinates, 0);
			}
			if (m_actionInfo != null)
			{
				imageMapArea.m_actionInfo = m_actionInfo.DeepClone();
			}
			return imageMapArea;
		}

		internal ImageMapAreaInstance Deconstruct(Microsoft.ReportingServices.ReportProcessing.CustomReportItem context)
		{
			Global.Tracer.Assert(context != null);
			ImageMapAreaInstance imageMapAreaInstance = new ImageMapAreaInstance(context.ProcessingContext);
			imageMapAreaInstance.ID = m_id;
			imageMapAreaInstance.Shape = m_shape;
			imageMapAreaInstance.Coordinates = m_coordinates;
			if (m_actionInfo != null)
			{
				Microsoft.ReportingServices.ReportProcessing.Action action = null;
				m_actionInfo.Deconstruct(imageMapAreaInstance.UniqueName, ref action, out ActionInstance actionInstance, context);
				imageMapAreaInstance.Action = action;
				imageMapAreaInstance.ActionInstance = actionInstance;
			}
			return imageMapAreaInstance;
		}
	}
}
