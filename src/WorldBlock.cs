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
        InvisibleWall,
		Water1,
		Water2,
		Water3,
		Water4,
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
                case WorldBlock.InvisibleWall:
                case WorldBlock.Water4:
                    return new Vector3(1.00f, 1.00f, 1.00f);
                case WorldBlock.Water3:
                    return new Vector3(1.00f, 0.75f, 1.00f);
                case WorldBlock.Water2:
                    return new Vector3(1.00f, 0.50f, 1.00f);
                case WorldBlock.Water1:
                case WorldBlock.Spikes:
                    return new Vector3(1.00f, 0.25f, 1.00f);
                default:
                    throw new ArgumentException($"{b} has no Bounds");
            }
        }

        public static bool HasCollision(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.Wall:
                case WorldBlock.InvisibleWall:
                case WorldBlock.Spikes:
                case WorldBlock.Water1:
                case WorldBlock.Water2:
                case WorldBlock.Water3:
                case WorldBlock.Water4:
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
                case WorldBlock.Water1:
                case WorldBlock.Water2:
                case WorldBlock.Water3:
                case WorldBlock.Water4:
                case WorldBlock.Spikes:
                    return true;
                default:
                    return false;
            }
        }
    }
}
