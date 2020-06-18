using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class FieldChooser : Form
	{
		private Label label1;

		private Button okButton;

		private Button cancelButton;

		private string selectedField = "";

		private System.Windows.Forms.Panel gridPlaceHolder;

		private IContainer components;

		public string SelectedField => selectedField;

		public FieldChooser(MapControl mapControl)
		{
			InitializeComponent();
			DataTable dataTable = new DataTable("Fields")
			{
				Locale = CultureInfo.CurrentCulture
			};
			foreach (Field shapeField in mapControl.ShapeFields)
			{
				dataTable.Columns.Add(shapeField.Name, shapeField.Type);
			}
			foreach (Shape shape in mapControl.Shapes)
			{
				DataRow dataRow = dataTable.NewRow();
				foreach (Field shapeField2 in mapControl.ShapeFields)
				{
					if (shape[shapeField2.Name] != null)
					{
						dataRow[shapeField2.Name] = shape[shapeField2.Name];
					}
				}
				dataTable.Rows.Add(dataRow);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			label1 = new System.Windows.Forms.Label();
			okButton = new System.Windows.Forms.Button();
			cancelButton = new System.Windows.Forms.Button();
			gridPlaceHolder = new System.Windows.Forms.Panel();
			SuspendLayout();
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(12, 15);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(336, 13);
			label1.TabIndex = 0;
			label1.Text = "Please choose the shape field based on which groups will be created:";
			okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			okButton.Location = new System.Drawing.Point(340, 368);
			okButton.Name = "okButton";
			okButton.Size = new System.Drawing.Size(92, 26);
			okButton.TabIndex = 2;
			okButton.Text = "&OK";
			okButton.Click += new System.EventHandler(okButton_Click);
			cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			cancelButton.Location = new System.Drawing.Point(438, 368);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new System.Drawing.Size(92, 26);
			cancelButton.TabIndex = 3;
			cancelButton.Text = "&Cancel";
			gridPlaceHolder.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
			gridPlaceHolder.Location = new System.Drawing.Point(12, 37);
			gridPlaceHolder.Name = "gridPlaceHolder";
			gridPlaceHolder.Size = new System.Drawing.Size(518, 314);
			gridPlaceHolder.TabIndex = 4;
			base.AcceptButton = okButton;
			base.CancelButton = cancelButton;
			base.ClientSize = new System.Drawing.Size(542, 406);
			base.Controls.Add(gridPlaceHolder);
			base.Controls.Add(cancelButton);
			base.Controls.Add(okButton);
			base.Controls.Add(label1);
			base.MinimizeBox = false;
			base.Name = "FieldChooser";
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Map for .NET";
			ResumeLayout(false);
			PerformLayout();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
		}
	}
}
