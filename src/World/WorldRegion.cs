using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    class WorldRegion
    {
        public WorldBlock[,,] Blocks;
        /*public VertexPositionColorTexture[] Vertices;
        public VertexBuffer Buffer;*/

        public WorldRegion(Stream _inputStream, int _sizeX, int _sizeY, int _sizeZ)
        {
            using (BinaryReader _reader = new BinaryReader(_inputStream))
            {
                Blocks = new WorldBlock[_sizeX, _sizeY, _sizeZ];

                /*uint _count = 0;
                byte _value = 255;*/

                for (int x = 0; x < _sizeX; x++)
                    for (int y = 0; y < _sizeY; y++)
                        for (int z = 0; z < _sizeZ; z++)
                        {
                            /*if (_count > 0)
                            {
                                Blocks[x, y, z] = (WorldBlock)_value;
                                _count--;
                            }
                            else
                            {
                                _count = _reader.ReadUInt32() - 1;
                                _value = _reader.ReadByte();
                                Blocks[x, y, z] = (WorldBlock)_value;
                            }*/
                            Blocks[x, y, z] = (WorldBlock)_reader.ReadByte();
                        }
            }
        }

        public WorldRegion()
        { }

        public void Save(Stream _outputStream)
        {
            uint _count = 0;
            byte _value = 255;
            using (BinaryWriter _writer = new BinaryWriter(_outputStream))
            {
                int _sizeX = Blocks.GetLength(0);
                int _sizeY = Blocks.GetLength(1);
                int _sizeZ = Blocks.GetLength(2);

                for (int x = 0; x < _sizeX; x++)
                    for (int y = 0; y < _sizeY; y++)
                        for (int z = 0; z < _sizeZ; z++)
                        {
                            _writer.Write((byte)Blocks[x, y, z]);
                            /*byte b = (byte)Blocks[x, y, z];
                            if (_value == 255)
                            {
                                _count = 1;
                                _value = b;
                            }
                            else if (b != _value)
                            {
                                _writer.Write(_count);
                                _writer.Write(_value);
                                _count = 1;
                                _value = b;
                            }
                            else
                                _count++;*/
                        }

                /*_writer.Write(_count);
                _writer.Write(_value);*/
            }
        }
    }
}
