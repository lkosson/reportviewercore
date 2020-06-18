namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal class FixedItemSizes : ItemSizes
	{
		internal override double Left
		{
			set
			{
			}
		}

		internal override double Top
		{
			set
			{
			}
		}

		internal override double Width
		{
			set
			{
			}
		}

		internal override double Height
		{
			set
			{
			}
		}

		internal FixedItemSizes(double width, double height)
			: base(0.0, 0.0, width, height)
		{
		}

		internal override void AdjustHeightTo(double amount)
		{
		}

		internal override void AdjustWidthTo(double amount)
		{
		}

		internal override void MoveVertical(double delta)
		{
		}

		internal override void MoveHorizontal(double delta)
		{
		}
	}
}
