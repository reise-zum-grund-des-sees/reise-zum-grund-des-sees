using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
	enum WorldBlock : byte
	{
		None,
		Wall, // add more wall types?
		Water,
		Lever,
		Spikes
	}

    static class WorldBlockHelper
    {
        public static Vector3 GetBounds(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.Wall:
                    return new Vector3(1.00f, 1.00f, 1.00f);
                default:
                    throw new ArgumentException($"{b} has no Bounds");
            }
        }

        public static bool HasCollision(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.Wall:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsVisible(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.Wall:
                    return true;
                default:
                    return false;
            }
        }
    }
}
