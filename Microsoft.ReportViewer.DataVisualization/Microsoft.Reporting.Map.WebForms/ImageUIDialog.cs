using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ImageUIDialog : Form
	{
		private ListView listView1;

		private ImageList imageList1;

		private Button btnDelete;

		private Button btnAdd;

		private Button btnOk;

		private Button btnCancel;

		private IContainer components;

		private MapCore map;

		private string selectedValue;

		internal string SelectedImage
		{
			get
			{
				if (listView1.SelectedItems.Count > 0)
				{
					return listView1.SelectedItems[0].Text;
				}
				return "";
			}
		}

		public ImageUIDialog()
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

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			listView1 = new System.Windows.Forms.ListView();
			imageList1 = new System.Windows.Forms.ImageList(components);
			btnDelete = new System.Windows.Forms.Button();
			btnAdd = new System.Windows.Forms.Button();
			btnOk = new System.Windows.Forms.Button();
			btnCancel = new System.Windows.Forms.Button();
			SuspendLayout();
			listView1.LargeImageList = imageList1;
			listView1.Location = new System.Drawing.Point(8, 6);
			listView1.Name = "listView1";
			listView1.Size = new System.Drawing.Size(294, 280);
			listView1.TabIndex = 0;
			listView1.DoubleClick += new System.EventHandler(listView1_DoubleClick);
			listView1.SelectedIndexChanged += new System.EventHandler(listView1_SelectedIndexChanged);
			imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			imageList1.ImageSize = new System.Drawing.Size(100, 100);
			imageList1.TransparentColor = System.Drawing.Color.Transparent;
			btnDelete.Enabled = false;
			btnDelete.Location = new System.Drawing.Point(8, 296);
			btnDelete.Name = "btnDelete";
			btnDelete.Size = new System.Drawing.Size(70, 24);
			btnDelete.TabIndex = 1;
			btnDelete.Text = "Delete";
			btnDelete.Click += new System.EventHandler(btnDelete_Click);
			btnAdd.Location = new System.Drawing.Point(82, 296);
			btnAdd.Name = "btnAdd";
			btnAdd.Size = new System.Drawing.Size(70, 24);
			btnAdd.TabIndex = 2;
			btnAdd.Text = "Add";
			btnAdd.Click += new System.EventHandler(btnAdd_Click);
			btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			btnOk.Enabled = false;
			btnOk.Location = new System.Drawing.Point(156, 296);
			btnOk.Name = "btnOk";
			btnOk.Size = new System.Drawing.Size(70, 24);
			btnOk.TabIndex = 3;
			btnOk.Text = "Ok";
			btnOk.Click += new System.EventHandler(btnOk_Click);
			btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			btnCancel.Location = new System.Drawing.Point(230, 296);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new System.Drawing.Size(70, 24);
			btnCancel.TabIndex = 4;
			btnCancel.Text = "Cancel";
			base.AcceptButton = btnOk;
			AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			base.CancelButton = btnCancel;
			base.ClientSize = new System.Drawing.Size(310, 327);
			base.Controls.AddRange(new System.Windows.Forms.Control[5]
			{
				btnCancel,
				btnOk,
				btnAdd,
				btnDelete,
				listView1
			});
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ImageUIDialog";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Select image";
			ResumeLayout(false);
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "All Image Files|*.jpg;*.jpeg;*.jpe;*.jif;*.bmp;*.png;*.gif;*.ico;*.emf;*.wmf|Bitmap Files (*.bmp)|*.bmp|GIF Files (*.gif)|*.gif|JPEG Files (*.jpg; *.jpeg; *.jpe; *.jif )|*.jpg;*.jpeg;*.jpe;*.jfif|Meta Files (*.emf; *.wmf)|*.emf,*.wmf|PNG Files (*.png)|*.png|All Files (*.*)|*.*";
				openFileDialog.RestoreDirectory = true;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					NamedImage namedImage = new NamedImage();
					map.NamedImages.Add(namedImage);
					string fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
					if (map.NamedImages.GetIndex(fileName) == -1)
					{
						namedImage.Name = fileName;
					}
					namedImage.Image = Image.FromFile(openFileDialog.FileName);
					imageList1.Images.Add(GetResizedImage(namedImage.Image));
					ListViewItem listViewItem = new ListViewItem(namedImage.Name, imageList1.Images.Count - 1);
					listView1.Select();
					listView1.Items.Add(listViewItem);
					listView1.SelectedItems.Clear();
					listViewItem.Focused = true;
					listViewItem.Selected = true;
				}
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			int index = map.NamedImages.GetIndex(SelectedImage);
			if (index != -1)
			{
				map.NamedImages.RemoveAt(index);
				InitImages();
			}
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			btnOk.Enabled = (listView1.SelectedItems.Count > 0);
			btnDelete.Enabled = btnOk.Enabled;
		}

		private void listView1_DoubleClick(object sender, EventArgs e)
		{
			btnOk.Enabled = (listView1.SelectedItems.Count > 0);
			if (btnOk.Enabled)
			{
				btnOk.PerformClick();
			}
		}

		internal DialogResult Execute(MapCore map, string selectedValue)
		{
			this.map = map;
			this.selectedValue = selectedValue;
			InitImages();
			return ShowDialog();
		}

		public bool ThumbnailCallback()
		{
			return false;
		}

		private Image GetResizedImage(Image image)
		{
			float num = (float)image.Size.Width / (float)image.Size.Height;
			Bitmap bitmap = new Bitmap(100, 100);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.FillRectangle(Brushes.White, 0, 0, 100, 100);
				Rectangle destRect = new Rectangle(0, 0, 100, 100);
				if (image.Size.Width < 100 && image.Size.Height < 100)
				{
					destRect = new Rectangle(0, 0, image.Size.Width, image.Size.Height);
					destRect.X = 50 - destRect.Width / 2;
					destRect.Y = 50 - destRect.Height / 2;
					graphics.DrawImage(image, destRect, 0, 0, image.Size.Width, image.Size.Height, GraphicsUnit.Pixel);
					return bitmap;
				}
				if (image.Size.Width > image.Size.Height)
				{
					destRect.Height = (int)(100f / num);
					destRect.Y = 50 - destRect.Height / 2;
					graphics.DrawImage(image, destRect, 0, 0, image.Size.Width, image.Size.Height, GraphicsUnit.Pixel);
					return bitmap;
				}
				destRect.Width = (int)(100f * num);
				destRect.X = 50 - destRect.Width / 2;
				graphics.DrawImage(image, destRect, 0, 0, image.Size.Width, image.Size.Height, GraphicsUnit.Pixel);
				return bitmap;
			}
		}

		private void InitImages()
		{
			imageList1.Images.Clear();
			listView1.Items.Clear();
			foreach (NamedImage namedImage in map.NamedImages)
			{
				if (namedImage.Image != null)
				{
					imageList1.Images.Add(GetResizedImage(namedImage.Image));
					listView1.Items.Add(namedImage.Name, imageList1.Images.Count - 1);
				}
			}
			int index = map.NamedImages.GetIndex(selectedValue);
			if (index != -1)
			{
				listView1.Items[index].Selected = true;
			}
		}
	}
}
