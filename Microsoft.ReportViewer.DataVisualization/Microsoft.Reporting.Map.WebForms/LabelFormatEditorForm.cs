using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Microsoft.Reporting.Map.WebForms
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
			tabControl.Controls.AddRange(new System.Windows.Forms.Control[2]
			{
				tabPageNumeric,
				tabPageCustom
			});
			tabControl.Location = new System.Drawing.Point(8, 8);
			tabControl.Name = "tabControl";
			tabControl.SelectedIndex = 0;
			tabControl.Size = new System.Drawing.Size(488, 216);
			tabControl.TabIndex = 0;
			tabControl.SelectedIndexChanged += new System.EventHandler(tabControl_SelectedIndexChanged);
			tabPageNumeric.Controls.AddRange(new System.Windows.Forms.Control[7]
			{
				textBoxPrecision,
				textBoxNumericSample,
				label3,
				labelNumericFormatDescription,
				label2,
				comboBoxFormatType,
				label1
			});
			tabPageNumeric.Location = new System.Drawing.Point(4, 22);
			tabPageNumeric.Name = "tabPageNumeric";
			tabPageNumeric.Size = new System.Drawing.Size(480, 190);
			tabPageNumeric.TabIndex = 0;
			tabPageNumeric.Text = "Numeric";
			textBoxPrecision.Location = new System.Drawing.Point(112, 40);
			textBoxPrecision.MaxLength = 2;
			textBoxPrecision.Name = "textBoxPrecision";
			textBoxPrecision.Size = new System.Drawing.Size(200, 20);
			textBoxPrecision.TabIndex = 13;
			textBoxPrecision.Text = "";
			textBoxPrecision.TextChanged += new System.EventHandler(textBoxPrecision_TextChanged);
			textBoxNumericSample.Location = new System.Drawing.Point(112, 64);
			textBoxNumericSample.Name = "textBoxNumericSample";
			textBoxNumericSample.ReadOnly = true;
			textBoxNumericSample.Size = new System.Drawing.Size(200, 20);
			textBoxNumericSample.TabIndex = 12;
			textBoxNumericSample.Text = "";
			label3.Location = new System.Drawing.Point(8, 64);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(104, 23);
			label3.TabIndex = 6;
			label3.Text = "Sample:";
			label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			labelNumericFormatDescription.Location = new System.Drawing.Point(8, 96);
			labelNumericFormatDescription.Name = "labelNumericFormatDescription";
			labelNumericFormatDescription.Size = new System.Drawing.Size(464, 88);
			labelNumericFormatDescription.TabIndex = 5;
			label2.Location = new System.Drawing.Point(8, 40);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(104, 23);
			label2.TabIndex = 2;
			label2.Text = "Precision specifier:";
			label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			comboBoxFormatType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboBoxFormatType.Items.AddRange(new object[6]
			{
				"Fixed-point",
				"Currency",
				"Scientific",
				"General",
				"Number",
				"Percent"
			});
			comboBoxFormatType.Location = new System.Drawing.Point(112, 16);
			comboBoxFormatType.Name = "comboBoxFormatType";
			comboBoxFormatType.Size = new System.Drawing.Size(200, 21);
			comboBoxFormatType.TabIndex = 10;
			comboBoxFormatType.SelectedIndexChanged += new System.EventHandler(comboBoxFormatType_SelectedIndexChanged);
			label1.Location = new System.Drawing.Point(8, 16);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(104, 23);
			label1.TabIndex = 0;
			label1.Text = "Format type:";
			label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			tabPageCustom.Controls.AddRange(new System.Windows.Forms.Control[5]
			{
				textBoxFormatString,
				label6,
				textBoxCustomSample,
				label4,
				labelCustomDescription
			});
			tabPageCustom.Location = new System.Drawing.Point(4, 22);
			tabPageCustom.Name = "tabPageCustom";
			tabPageCustom.Size = new System.Drawing.Size(480, 190);
			tabPageCustom.TabIndex = 2;
			tabPageCustom.Text = "Custom";
			textBoxFormatString.Location = new System.Drawing.Point(112, 12);
			textBoxFormatString.Name = "textBoxFormatString";
			textBoxFormatString.Size = new System.Drawing.Size(200, 20);
			textBoxFormatString.TabIndex = 31;
			textBoxFormatString.Text = "";
			textBoxFormatString.TextChanged += new System.EventHandler(textBoxFormatString_TextChanged);
			label6.Location = new System.Drawing.Point(8, 12);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(104, 23);
			label6.TabIndex = 8;
			label6.Text = "Format string:";
			label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			textBoxCustomSample.Location = new System.Drawing.Point(112, 36);
			textBoxCustomSample.Name = "textBoxCustomSample";
			textBoxCustomSample.ReadOnly = true;
			textBoxCustomSample.Size = new System.Drawing.Size(200, 20);
			textBoxCustomSample.TabIndex = 32;
			textBoxCustomSample.Text = "";
			label4.Location = new System.Drawing.Point(8, 36);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(104, 23);
			label4.TabIndex = 11;
			label4.Text = "Sample:";
			label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			labelCustomDescription.Location = new System.Drawing.Point(8, 72);
			labelCustomDescription.Name = "labelCustomDescription";
			labelCustomDescription.Size = new System.Drawing.Size(464, 112);
			labelCustomDescription.TabIndex = 10;
			labelCustomDescription.Text = "Characters that can be used to create custom numeric format strings:  '0' - Zero placeholder, '#' - Digit placeholder, '.' - Decimal point, ',' - Thousand separator and number scaling, '%' - Percentage placeholder, 'E0' - Scientific notation, '\\\\' - Escape character, ';' - Section separator.\\r\\n\\r\\nAll other characters are copied to the output string as literals in the position they appear.";
			buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			buttonOk.Location = new System.Drawing.Point(504, 40);
			buttonOk.Name = "buttonOk";
			buttonOk.TabIndex = 40;
			buttonOk.Text = "Ok";
			buttonOk.Click += new System.EventHandler(buttonOk_Click);
			buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			buttonCancel.Location = new System.Drawing.Point(504, 80);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.TabIndex = 41;
			buttonCancel.Text = "Cancel";
			buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
			base.AcceptButton = buttonOk;
			AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			base.ClientSize = new System.Drawing.Size(592, 229);
			base.Controls.AddRange(new System.Windows.Forms.Control[3]
			{
				buttonCancel,
				buttonOk,
				tabControl
			});
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Name = "LabelFormatEditorForm";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Format Editor";
			base.Load += new System.EventHandler(LabelFormatEditorForm_Load);
			tabControl.ResumeLayout(false);
			tabPageNumeric.ResumeLayout(false);
			tabPageCustom.ResumeLayout(false);
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
				"The number is converted to the most compact decimal form, using fixed or scientific notation. The precision specifier determines the number of significant digits in the resulting string.",
				"The number is converted to a string that represents a currency amount. The conversion is controlled by the system currency format information. The precision specifier indicates the desired number of decimal places.",
				"The number is converted to a string of the form \"-d.ddd…E+ddd\" or \"-d.ddd…e+ddd\", where each 'd' indicates a digit (0-9). The string starts with a minus sign if the number is negative. One digit always precedes the decimal point. The precision specifier indicates the desired number of digits after the decimal point.",
				"The number is converted to a string of the form \"-ddd.ddd…\" where each 'd' indicates a digit (0-9). The string starts with a minus sign if the number is negative. The precision specifier indicates the desired number of decimal places.",
				"The number is converted to a string of the form \"-d,ddd,ddd.ddd…\", where each 'd' indicates a digit (0-9). The string starts with a minus sign if the number is negative. Thousand separators are inserted between each group of three digits to the left of the decimal point. The precision specifier indicates the desired number of decimal places.",
				"The number is converted to a string that represents a percent. The precision specifier indicates the desired number of decimal places."
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
				textBoxCustomSample.Text = "Invalid custom format string";
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
