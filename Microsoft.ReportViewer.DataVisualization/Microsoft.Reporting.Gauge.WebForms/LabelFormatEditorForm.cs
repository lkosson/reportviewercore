using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class LabelFormatEditorForm : Form
	{
		internal string resultFormat = "";

		private string formatString = "";

		private string formatNumeric = "";

		private TabControl tabControl;

		private ComboBox comboBoxFormatType;

		private TabPage tabPageNumeric;

		private TabPage tabPageCustom;

		private TextBox textBoxFormatString;

		private TextBox textBoxCustomSample;

		private Label labelCustomDescription;

		private Label labelNumericFormatDescription;

		private TextBox textBoxNumericSample;

		private Button buttonOk;

		private Button buttonCancel;

		private Label label1;

		private Label label2;

		private Label label6;

		private Label label4;

		private Label label3;

		private TextBox textBoxPrecision;

		private Container components;

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Microsoft.Reporting.Gauge.WebForms.LabelFormatEditorForm));
			tabControl = new System.Windows.Forms.TabControl();
			tabPageNumeric = new System.Windows.Forms.TabPage();
			textBoxPrecision = new System.Windows.Forms.TextBox();
			textBoxNumericSample = new System.Windows.Forms.TextBox();
			label3 = new System.Windows.Forms.Label();
			labelNumericFormatDescription = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			comboBoxFormatType = new System.Windows.Forms.ComboBox();
			label1 = new System.Windows.Forms.Label();
			tabPageCustom = new System.Windows.Forms.TabPage();
			textBoxFormatString = new System.Windows.Forms.TextBox();
			label6 = new System.Windows.Forms.Label();
			textBoxCustomSample = new System.Windows.Forms.TextBox();
			label4 = new System.Windows.Forms.Label();
			labelCustomDescription = new System.Windows.Forms.Label();
			buttonOk = new System.Windows.Forms.Button();
			buttonCancel = new System.Windows.Forms.Button();
			tabControl.SuspendLayout();
			tabPageNumeric.SuspendLayout();
			tabPageCustom.SuspendLayout();
			SuspendLayout();
			tabControl.Controls.Add(tabPageNumeric);
			tabControl.Controls.Add(tabPageCustom);
			resources.ApplyResources(tabControl, "tabControl");
			tabControl.Name = "tabControl";
			tabControl.SelectedIndex = 0;
			tabControl.SelectedIndexChanged += new System.EventHandler(tabControl_SelectedIndexChanged);
			tabPageNumeric.Controls.Add(textBoxPrecision);
			tabPageNumeric.Controls.Add(textBoxNumericSample);
			tabPageNumeric.Controls.Add(label3);
			tabPageNumeric.Controls.Add(labelNumericFormatDescription);
			tabPageNumeric.Controls.Add(label2);
			tabPageNumeric.Controls.Add(comboBoxFormatType);
			tabPageNumeric.Controls.Add(label1);
			resources.ApplyResources(tabPageNumeric, "tabPageNumeric");
			tabPageNumeric.Name = "tabPageNumeric";
			resources.ApplyResources(textBoxPrecision, "textBoxPrecision");
			textBoxPrecision.Name = "textBoxPrecision";
			textBoxPrecision.TextChanged += new System.EventHandler(textBoxPrecision_TextChanged);
			resources.ApplyResources(textBoxNumericSample, "textBoxNumericSample");
			textBoxNumericSample.Name = "textBoxNumericSample";
			textBoxNumericSample.ReadOnly = true;
			resources.ApplyResources(label3, "label3");
			label3.Name = "label3";
			resources.ApplyResources(labelNumericFormatDescription, "labelNumericFormatDescription");
			labelNumericFormatDescription.Name = "labelNumericFormatDescription";
			resources.ApplyResources(label2, "label2");
			label2.Name = "label2";
			comboBoxFormatType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboBoxFormatType.Items.AddRange(new object[6]
			{
				resources.GetString("comboBoxFormatType.Items"),
				resources.GetString("comboBoxFormatType.Items1"),
				resources.GetString("comboBoxFormatType.Items2"),
				resources.GetString("comboBoxFormatType.Items3"),
				resources.GetString("comboBoxFormatType.Items4"),
				resources.GetString("comboBoxFormatType.Items5")
			});
			resources.ApplyResources(comboBoxFormatType, "comboBoxFormatType");
			comboBoxFormatType.Name = "comboBoxFormatType";
			comboBoxFormatType.SelectedIndexChanged += new System.EventHandler(comboBoxFormatType_SelectedIndexChanged);
			resources.ApplyResources(label1, "label1");
			label1.Name = "label1";
			tabPageCustom.Controls.Add(textBoxFormatString);
			tabPageCustom.Controls.Add(label6);
			tabPageCustom.Controls.Add(textBoxCustomSample);
			tabPageCustom.Controls.Add(label4);
			tabPageCustom.Controls.Add(labelCustomDescription);
			resources.ApplyResources(tabPageCustom, "tabPageCustom");
			tabPageCustom.Name = "tabPageCustom";
			resources.ApplyResources(textBoxFormatString, "textBoxFormatString");
			textBoxFormatString.Name = "textBoxFormatString";
			textBoxFormatString.TextChanged += new System.EventHandler(textBoxFormatString_TextChanged);
			resources.ApplyResources(label6, "label6");
			label6.Name = "label6";
			resources.ApplyResources(textBoxCustomSample, "textBoxCustomSample");
			textBoxCustomSample.Name = "textBoxCustomSample";
			textBoxCustomSample.ReadOnly = true;
			resources.ApplyResources(label4, "label4");
			label4.Name = "label4";
			resources.ApplyResources(labelCustomDescription, "labelCustomDescription");
			labelCustomDescription.Name = "labelCustomDescription";
			buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(buttonOk, "buttonOk");
			buttonOk.Name = "buttonOk";
			buttonOk.Click += new System.EventHandler(buttonOk_Click);
			buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(buttonCancel, "buttonCancel");
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
			base.AcceptButton = buttonOk;
			resources.ApplyResources(this, "$this");
			base.Controls.Add(buttonCancel);
			base.Controls.Add(buttonOk);
			base.Controls.Add(tabControl);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Name = "LabelFormatEditorForm";
			base.Load += new System.EventHandler(LabelFormatEditorForm_Load);
			tabControl.ResumeLayout(false);
			tabPageNumeric.ResumeLayout(false);
			tabPageNumeric.PerformLayout();
			tabPageCustom.ResumeLayout(false);
			tabPageCustom.PerformLayout();
			ResumeLayout(false);
		}

		public LabelFormatEditorForm()
		{
			InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void UpdateNumericSample()
		{
			formatString = formatNumeric + textBoxPrecision.Text;
			if (formatString.StartsWith("D", StringComparison.Ordinal))
			{
				textBoxNumericSample.Text = string.Format(CultureInfo.CurrentCulture, "{0:" + formatString + "}", 12345);
			}
			else if (formatString.StartsWith("P", StringComparison.Ordinal))
			{
				textBoxNumericSample.Text = string.Format(CultureInfo.CurrentCulture, "{0:" + formatString + "}", 0.126);
			}
			else
			{
				textBoxNumericSample.Text = string.Format(CultureInfo.CurrentCulture, "{0:" + formatString + "}", 12345.6789);
			}
		}

		private void comboBoxFormatType_SelectedIndexChanged(object sender, EventArgs e)
		{
			string[] array = new string[6]
			{
				"F",
				"C",
				"E",
				"G",
				"N",
				"P"
			};
			string[] array2 = new string[6]
			{
				SR.LabelFormatDescriptionF,
				SR.LabelFormatDescriptionC,
				SR.LabelFormatDescriptionE,
				SR.LabelFormatDescriptionG,
				SR.LabelFormatDescriptionN,
				SR.LabelFormatDescriptionP
			};
			formatNumeric = array[comboBoxFormatType.SelectedIndex];
			labelNumericFormatDescription.Text = array2[comboBoxFormatType.SelectedIndex];
			UpdateNumericSample();
		}

		private void LabelFormatEditorForm_Load(object sender, EventArgs e)
		{
			comboBoxFormatType.SelectedIndex = 0;
		}

		private void textBoxPrecision_TextChanged(object sender, EventArgs e)
		{
			if (textBoxPrecision.Text.Length >= 1 && !char.IsDigit(textBoxPrecision.Text[0]))
			{
				MessageBox.Show(this, SR.LabelFormatPrecisionMsg, SR.LabelFormatPrecisionMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, GlobalizationHelper.GetMessageBoxOptions(this));
				textBoxPrecision.Text = string.Empty;
			}
			else if (textBoxPrecision.Text.Length >= 2 && (!char.IsDigit(textBoxPrecision.Text[0]) || !char.IsDigit(textBoxPrecision.Text[1])))
			{
				MessageBox.Show(this, SR.LabelFormatPrecisionMsg, SR.LabelFormatPrecisionMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, GlobalizationHelper.GetMessageBoxOptions(this));
				textBoxPrecision.Text = string.Empty;
			}
			UpdateNumericSample();
		}

		private void textBoxFormatString_TextChanged(object sender, EventArgs e)
		{
			UpdateCustomExample();
		}

		private void UpdateCustomExample()
		{
			bool flag = false;
			formatString = textBoxFormatString.Text;
			if (!flag)
			{
				try
				{
					textBoxCustomSample.Text = string.Format(CultureInfo.CurrentCulture, "{0:" + textBoxFormatString.Text + "}", 12345.6789);
					flag = true;
				}
				catch (Exception)
				{
				}
				if (!flag)
				{
					try
					{
						textBoxCustomSample.Text = string.Format(CultureInfo.CurrentCulture, "{0:" + textBoxFormatString.Text + "}", 12345);
						flag = true;
					}
					catch (Exception)
					{
					}
				}
			}
			if (!flag)
			{
				textBoxCustomSample.Text = SR.LabelFormatInvalidCustomFormat;
			}
		}

		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tabControl.SelectedIndex == 0)
			{
				comboBoxFormatType.SelectedIndex = 0;
				comboBoxFormatType.Focus();
			}
			else if (tabControl.SelectedIndex == 1)
			{
				formatString = "";
				textBoxFormatString.Focus();
			}
		}

		private void buttonOk_Click(object sender, EventArgs e)
		{
			resultFormat = formatString;
			Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
