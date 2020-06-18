using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAnnotationPathPointCollection_AnnotationPathPointCollection")]
	internal class AnnotationPathPointCollection : CollectionBase
	{
		internal PolylineAnnotation annotation;

		[SRDescription("DescriptionAttributeAnnotationPathPointCollection_Item")]
		public AnnotationPathPoint this[int index]
		{
			get
			{
				return (AnnotationPathPoint)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public void Remove(AnnotationPathPoint point)
		{
			base.List.Remove(point);
		}

		public int Add(AnnotationPathPoint point)
		{
			return base.List.Add(point);
		}

		public void Insert(int index, AnnotationPathPoint point)
		{
			base.List.Insert(index, point);
		}

		public bool Contains(AnnotationPathPoint value)
		{
			return base.List.Contains(value);
		}

		public int IndexOf(AnnotationPathPoint value)
		{
			return base.List.IndexOf(value);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			OnCollectionChanged();
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			OnCollectionChanged();
		}

		protected override void OnClearComplete()
		{
			OnCollectionChanged();
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			OnCollectionChanged();
		}

		private void OnCollectionChanged()
		{
			if (annotation == null)
			{
				return;
			}
			if (annotation.path != null)
			{
				annotation.path.Dispose();
				annotation.path = null;
			}
			if (base.List.Count > 0)
			{
				PointF[] array = new PointF[base.List.Count];
				byte[] array2 = new byte[base.List.Count];
				for (int i = 0; i < base.List.Count; i++)
				{
					array[i] = new PointF(this[i].X, this[i].Y);
					array2[i] = this[i].PointType;
				}
				annotation.path = new GraphicsPath(array, array2);
			}
			else
			{
				annotation.path = new GraphicsPath();
			}
			annotation.Invalidate();
		}
	}
}
