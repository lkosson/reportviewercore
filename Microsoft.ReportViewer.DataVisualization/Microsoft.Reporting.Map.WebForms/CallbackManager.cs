namespace Microsoft.Reporting.Map.WebForms
{
	internal class CallbackManager : MapObject
	{
		private string jsCode = "";

		private string controlUpdates = "";

		private bool disableClientUpdate;

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

		public CallbackManager()
			: this(null)
		{
		}

		public CallbackManager(object parent)
			: base(parent)
		{
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
		}

		internal string GetJavaScript()
		{
			return jsCode;
		}

		internal void ResetControlUpdates()
		{
			controlUpdates = "";
		}

		internal string GetControlUpdates()
		{
			return controlUpdates;
		}
	}
}
