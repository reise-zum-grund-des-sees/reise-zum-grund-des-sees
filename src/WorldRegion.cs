using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace ReiseZumGrundDesSees
{
	struct WorldRegion
	{
		public readonly WorldBlock[,,] Blocks;
		public bool Loaded => Blocks != null;

		public WorldRegion(Stream _inputStream, int _sizeX, int _sizeY, int _sizeZ)
		{
			using (BinaryReader _reader = new BinaryReader(_inputStream))
			{
				Blocks = new WorldBlock[_sizeX, _sizeY, _sizeZ];

				for (int x = 0; x < _sizeX; x++)
					for (int y = 0; y < _sizeY; y++)
						for (int z = 0; z < _sizeZ; z++)
							Blocks[x, y, z] = (WorldBlock)_reader.ReadByte();
			}
		}
		public WorldRegion(int _sizeX, int _sizeY, int _sizeZ)
		{
			Blocks = new WorldBlock[_sizeX, _sizeY, _sizeZ];
		}

		public void Save(Stream _outputStream)
		{
			using (BinaryWriter _writer = new BinaryWriter(_outputStream))
			{
				int _sizeX = Blocks.GetLength(0);
				int _sizeY = Blocks.GetLength(1);
				int _sizeZ = Blocks.GetLength(2);

				for (int x = 0; x < _sizeX; x++)
					for (int y = 0; y < _sizeY; y++)
						for (int z = 0; z < _sizeZ; z++)
							_writer.Write((byte)Blocks[x, y, z]);
			}
		}
	}
}
