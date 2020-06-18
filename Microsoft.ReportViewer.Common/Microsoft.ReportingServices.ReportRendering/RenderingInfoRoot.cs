using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportRendering
{
	[Serializable]
	internal sealed class RenderingInfoRoot
	{
		private Hashtable m_renderingInfo;

		private Hashtable m_sharedRenderingInfo;

		private Hashtable m_pageSectionRenderingInfo;

		private PaginationInfo m_paginationInfo;

		internal Hashtable RenderingInfo => m_renderingInfo;

		internal Hashtable SharedRenderingInfo => m_sharedRenderingInfo;

		internal Hashtable PageSectionRenderingInfo => m_pageSectionRenderingInfo;

		internal PaginationInfo PaginationInfo
		{
			get
			{
				if (m_paginationInfo == null)
				{
					m_paginationInfo = new PaginationInfo();
				}
				return m_paginationInfo;
			}
			set
			{
				m_paginationInfo = value;
			}
		}

		internal RenderingInfoRoot()
		{
			m_renderingInfo = new Hashtable();
			m_sharedRenderingInfo = new Hashtable();
			m_pageSectionRenderingInfo = new Hashtable();
			m_paginationInfo = null;
		}
	}
}
