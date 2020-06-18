namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class CallbackManager
	{
		private string jsCode = "";

		private string controlUpdates = "";

		private bool disableClientUpdate;

		private string returnCommandName = string.Empty;

		private string returnCommandArgument = string.Empty;

		public bool DisableClientUpdate
		{
			get
			{
				return disableClientUpdate;
			}
			set
			{
				disableClientUpdate = value;
			}
		}

		public string ReturnCommandName
		{
			get
			{
				return returnCommandName;
			}
			set
			{
				returnCommandName = value;
			}
		}

		public string ReturnCommandArgument
		{
			get
			{
				return returnCommandArgument;
			}
			set
			{
				returnCommandArgument = value;
			}
		}

		public void ExecuteClientScript(string jsSourceCode)
		{
			jsCode = jsCode + jsSourceCode + "; ";
		}

		internal void Reset()
		{
			jsCode = "";
			controlUpdates = "";
			disableClientUpdate = false;
			returnCommandName = "";
			returnCommandArgument = "";
		}

		internal string GetJavaScript()
		{
			return jsCode;
		}

		internal string GetControlUpdates()
		{
			return controlUpdates;
		}
	}
}
