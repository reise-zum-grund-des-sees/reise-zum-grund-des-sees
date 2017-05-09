using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    class VertexGenerator
    {
        public static VertexPositionColorTexture[] GenerateVertices(ref WorldRegion _region)
        {
            int _sizeX = _region.Blocks.GetLength(0);
            int _sizeY = _region.Blocks.GetLength(1);
            int _sizeZ = _region.Blocks.GetLength(2);

            VertexPositionColorTexture[] _vertices = new VertexPositionColorTexture[_sizeX * _sizeY * _sizeZ * 6 * 6 / 8];
            int i = 0;

            for (int x = 0; x < _sizeX; x++)
                for (int y = 0; y < _sizeY; y++)
                    for (int z = 0; z < _sizeZ; z++)
                    {
                        switch (_region.Blocks[x, y, z])
                        {
                            case WorldBlock.Wall:
                                Vector3 ldf = new Vector3(x, y, z);
                                Vector3 luf = new Vector3(x, y + 1, z);
                                Vector3 rdf = new Vector3(x + 1, y, z);
                                Vector3 ruf = new Vector3(x + 1, y + 1, z);
                                Vector3 ldb = new Vector3(x, y, z + 1);
                                Vector3 lub = new Vector3(x, y + 1, z + 1);
                                Vector3 rdb = new Vector3(x + 1, y, z + 1);
                                Vector3 rub = new Vector3(x + 1, y + 1, z + 1);

                                // FRONT
                                _vertices[i++] = new VertexPositionColorTexture(luf, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(ruf, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(ldf, Color.White, new Vector2(0, 1));
                                _vertices[i++] = new VertexPositionColorTexture(ruf, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(rdf, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(ldf, Color.White, new Vector2(0, 1));

                                // BACK
                                _vertices[i++] = new VertexPositionColorTexture(lub, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(ldb, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(rub, Color.White, new Vector2(0, 1));
                                _vertices[i++] = new VertexPositionColorTexture(rub, Color.White, new Vector2(0, 1));
                                _vertices[i++] = new VertexPositionColorTexture(ldb, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(rdb, Color.White, new Vector2(0, 1));

                                // LEFT
                                _vertices[i++] = new VertexPositionColorTexture(lub, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(luf, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(ldb, Color.White, new Vector2(0, 1));
                                _vertices[i++] = new VertexPositionColorTexture(luf, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(ldf, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(ldb, Color.White, new Vector2(0, 1));

                                // RIGHT
                                _vertices[i++] = new VertexPositionColorTexture(rub, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(rdb, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(ruf, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(ruf, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(rdb, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(rdf, Color.White, new Vector2(0, 1));

                                // TOP
                                _vertices[i++] = new VertexPositionColorTexture(lub, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(rub, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(luf, Color.White, new Vector2(0, 1));
                                _vertices[i++] = new VertexPositionColorTexture(rub, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(ruf, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(luf, Color.White, new Vector2(0, 1));

                                // BOTTOM
                                _vertices[i++] = new VertexPositionColorTexture(ldb, Color.White, new Vector2(0, 1));
                                _vertices[i++] = new VertexPositionColorTexture(ldf, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(rdb, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(rdb, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(ldf, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(rdf, Color.White, new Vector2(1, 0));
                                break;
                        }
                    }

            VertexPositionColorTexture[] _finalArray = new VertexPositionColorTexture[i];
            Array.Copy(_vertices, _finalArray, i);
            return _finalArray;
        }

        /*private static Vector2[] TextureAtlasLookup(WorldBlock _block)
        {
            switch (_block)
            {
                case WorldBlock.Wall:
                    return new Vector2[] {
                        new Vector2()
                    };
            }
        }*/
    }
}
