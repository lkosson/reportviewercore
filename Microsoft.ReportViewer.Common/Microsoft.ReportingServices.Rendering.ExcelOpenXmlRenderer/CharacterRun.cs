using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class CharacterRun
	{
		private readonly ICharacterRunModel _model;

		public Font Font
		{
			set
			{
				_model.SetFont(value);
			}
		}

		internal CharacterRun(ICharacterRunModel model)
		{
			_model = model;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is CharacterRun))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((CharacterRun)obj)._model.Equals(_model);
		}

		public override int GetHashCode()
		{
			return _model.GetHashCode();
		}
	}
}
