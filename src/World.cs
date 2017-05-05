using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
	class World
	{
		public World(string _filepath)
		{
			throw new NotImplementedException();
		}

		public readonly BlockWrapper Blocks;

		public struct BlockWrapper
		{
			public WorldBlock this[int x, int y, int z]
			{
				get
				{
					throw new NotImplementedException();
				}

				set
				{
					throw new NotImplementedException();
				}
			}
		}
	}
}
