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
		Spikes,
        Unknown
	}

    static class WorldBlockHelper
    {
        public static bool IsPartOfWorldObject(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.Lever:
                    return true;
                default:
                    return false;
            }
        }


        public static Vector3 GetBounds(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.Wall:
                case WorldBlock.InvisibleWall:
                case WorldBlock.Water4:
                case WorldBlock.Unknown:
                    return new Vector3(1.00f, 1.00f, 1.00f);
                case WorldBlock.Water3:
                    return new Vector3(1.00f, 0.75f, 1.00f);
                case WorldBlock.Water2:
                    return new Vector3(1.00f, 0.50f, 1.00f);
                case WorldBlock.Water1:
                case WorldBlock.Spikes:
                    return new Vector3(1.00f, 0.25f, 1.00f);
                case WorldBlock.Lever:
                    return new Vector3(1f, 1.00f, 1f);
                default:
                    throw new ArgumentException($"{b} has no Bounds");
            }
        }

        public static Vector2[] GetTextureOffsets(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.Wall:
                    return new Vector2[] {
                            new Vector2(0.0f, 0 / 3f),
                            new Vector2(0.5f, 0 / 3f),
                            new Vector2(0.0f, 1 / 3f),
                            new Vector2(0.5f, 1 / 3f),
                            new Vector2(0.0f, 2 / 3f)
                            //new Vector2(0.5f, 2 / 3f)
                    };
                case WorldBlock.Water4:
                case WorldBlock.Water3:
                case WorldBlock.Water2:
                case WorldBlock.Water1:
                    return new Vector2[] {
                            new Vector2(0.5f, 2 / 3f)
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        public static bool HasCollision(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.Wall:
                case WorldBlock.InvisibleWall:
                case WorldBlock.Spikes:
                case WorldBlock.Lever:
                case WorldBlock.Water1:
                case WorldBlock.Water2:
                case WorldBlock.Water3:
                case WorldBlock.Water4:
                case WorldBlock.Unknown:
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
