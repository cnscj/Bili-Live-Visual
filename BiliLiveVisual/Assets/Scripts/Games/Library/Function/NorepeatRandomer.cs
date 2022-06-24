using System.Collections;
using System.Collections.Generic;

namespace BLVisual
{
    public class NorepeatRandomer
    {
		List<int> list = new List<int>();

		public void Clear()
        {
			list.Clear();

		}
		public int Range(int min, int max)
		{
			int random = UnityEngine.Random.Range(min, max);
			while (true)
			{

				if (!list.Contains(random))
				{
					list.Add(random);

					break;
				}
				else
				{
					random = UnityEngine.Random.Range(min, max);

					if (list.Count >= max)
					{

						break;
					}
				}
			}

			return random;
		}

	}
}
