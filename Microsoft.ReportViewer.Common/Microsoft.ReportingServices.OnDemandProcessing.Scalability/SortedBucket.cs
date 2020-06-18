namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class SortedBucket
	{
		internal int Limit;

		internal int Minimum;

		internal Heap<long, Space> m_spaces;

		internal int Count => m_spaces.Count;

		internal int Maximum => (int)Peek().Size;

		internal SortedBucket(int maxSpacesPerBucket)
		{
			int num = 500;
			if (num > maxSpacesPerBucket)
			{
				num = maxSpacesPerBucket;
			}
			m_spaces = new Heap<long, Space>(num, maxSpacesPerBucket);
			Minimum = int.MaxValue;
		}

		internal SortedBucket Split(int maxSpacesPerBucket)
		{
			SortedBucket sortedBucket = new SortedBucket(maxSpacesPerBucket);
			int num = Count / 2;
			for (int i = 0; i < num; i++)
			{
				sortedBucket.Insert(ExtractMax());
			}
			sortedBucket.Limit = sortedBucket.Minimum;
			return sortedBucket;
		}

		internal void Insert(Space space)
		{
			if (space.Size < Minimum)
			{
				Minimum = (int)space.Size;
			}
			m_spaces.Insert(space.Size, space);
		}

		internal Space Peek()
		{
			return m_spaces.Peek();
		}

		internal Space ExtractMax()
		{
			Space result = m_spaces.ExtractMax();
			if (m_spaces.Count == 0)
			{
				Minimum = int.MaxValue;
			}
			return result;
		}
	}
}
