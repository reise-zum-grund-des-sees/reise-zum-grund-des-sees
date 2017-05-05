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

			public float CamX => baseState.Camera.Position.X;
			public float CamY => baseState.Camera.Position.Y;
			public float CamZ => baseState.Camera.Position.Z;

			public float PlayerX => baseState.Player.Position.X;
			public float PlayerY => baseState.Player.Position.Y;
			public float PlayerZ => baseState.Player.Position.Z;

			public WorldBlock GetBlock(int x, int y, int z) => baseState.World.Blocks[x, y, z];
		}
	}

	delegate void UpdateDelegate(ref GameState gs);
	interface IUpdateable
	{
		/// <summary>
		/// Update Delegate
		/// </summary>
		/// <param name="_view">Die aktuelle Version des GameStates</param>
		/// <param name="_inputEvents">Nutzer-Ereignisse</param>
		/// <param name="_passedTime">Verstichene Zeit seit dem letzten Update</param>
		/// <returns>UpdateDelegate, dass den GameState verändern kann: return (ref GameState _state) => { /* CODE HERE */ };</returns>
		UpdateDelegate Update(GameState.View _view, InputEvent _inputEvents, double _passedTime);
	}
}