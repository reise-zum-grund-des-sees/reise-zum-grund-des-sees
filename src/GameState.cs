using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
	struct GameState
	{
		public readonly Camera Camera;
		public readonly Player Player;
		public readonly World World;

		public struct View
		{
			private GameState baseState;
			public View(GameState s)
			{
				baseState = s;
			}

			public float CamX => baseState.Camera.xPos;
			public float CamY => baseState.Camera.yPos;
			public float CamZ => baseState.Camera.zPos;

			public float PlayerX => baseState.Player.xPos;
			public float PlayerY => baseState.Player.yPos;
			public float PlayerZ => baseState.Player.zPos;

			public WorldBlock GetBlock(int x, int y, int z) => baseState.World.Blocks[x, y, z];
		}
	}

	delegate void UpdateDelegate(ref GameState gs);
	interface IUpdateable
	{
		UpdateDelegate Update(GameState.View _state, InputManager.InputEvent _inputEvents, double _passedTime);
	}
}