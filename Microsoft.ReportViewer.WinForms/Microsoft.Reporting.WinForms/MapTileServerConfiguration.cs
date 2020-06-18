using System;
using System.ComponentModel;

namespace Microsoft.Reporting.WinForms
{
	[TypeConverter(typeof(TypeNameHidingExpandableObjectConverter))]
	public sealed class MapTileServerConfiguration
	{
		private LocalProcessingHostMapTileServerConfiguration m_underlyingConfiguration;

		[SRDescription("MapTileServerConfigurationMaxConnectionsDesc")]
		[DefaultValue(2)]
		[NotifyParentProperty(true)]
		public int MaxConnections
		{
			get
			{
				return m_underlyingConfiguration.MaxConnections;
			}
			set
			{
				m_underlyingConfiguration.MaxConnections = value;
			}
		}

		[SRDescription("MapTileServerConfigurationTimeoutDesc")]
		[DefaultValue(10)]
		[NotifyParentProperty(true)]
		public int Timeout
		{
			get
			{
				return m_underlyingConfiguration.Timeout;
			}
			set
			{
				m_underlyingConfiguration.Timeout = value;
			}
		}

		[SRDescription("MapTileServerConfigurationAppIDDesc")]
		[DefaultValue("(Default)")]
		[NotifyParentProperty(true)]
		public string AppID
		{
			get
			{
				return m_underlyingConfiguration.AppID;
			}
			set
			{
				m_underlyingConfiguration.AppID = value;
			}
		}

		internal MapTileServerConfiguration(LocalProcessingHostMapTileServerConfiguration underlyingConfiguration)
		{
			if (underlyingConfiguration == null)
			{
				throw new ArgumentNullException("underlyingConfiguration");
			}
			m_underlyingConfiguration = underlyingConfiguration;
		}
	}
}
