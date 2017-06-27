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
        public readonly ICamera Camera;
        public readonly IPlayer Player;
        public readonly World World;
        public readonly CollisionDetector CollisionDetector;

        public GameState(World _world, IPlayer _player, Camera _camera)
        {
            World = _world;
            Player = _player;
            Camera = _camera;
            CollisionDetector = new CollisionDetector(_world.Blocks);
        }

        public void Save(string _baseDir)
        {
            ConfigFile f = new ConfigFile();

            ObjectIDMapper _idMapper = new ObjectIDMapper();
            f.Nodes["world"] = World.GetState(_idMapper);
            f.Nodes["player"] = Player.GetState(_idMapper);

            ConfigFile.ConfigNode _node = new ConfigFile.ConfigNode();
            _node.Items["count"] = MovingBlock.MovingBlockList.Count.ToString();

            f.Nodes["movingBlockCount"] = _node;
            for (int i=0;i<MovingBlock.MovingBlockList.Count;i++)
            f.Nodes["movingBlock"+i] = MovingBlock.MovingBlockList[i].GetState();

            f.Write(System.IO.Path.Combine(_baseDir, "state.conf"));
            World.SaveRegions(_baseDir);
        }

        public static GameState Load(string _baseDir)
        {
            ConfigFile _config = ConfigFile.Load(System.IO.Path.Combine(_baseDir, "state.conf"));

            ObjectIDMapper _idMapper = new ObjectIDMapper();
            World w = new RenderableWorld(_config.Nodes["world"], _idMapper, _baseDir);
            Player p = new Player(_config.Nodes["player"]);
            Camera c = new Camera();
            c.CenterOn(p);
            MovingBlock.MovingBlockList.Clear();
            for (int i = 0; i < Int32.Parse(_config.Nodes["movingBlockCount"].Items["count"]); i++) { 
                new MovingBlock(_config.Nodes["movingBlock"+i]);
            
            }

            return new GameState(w, p, c);
        }

        public struct View
        {
            private GameState baseState;
            public View(GameState s)
            {
                baseState = s;
            }

            public IReadonlyCollisionDetector CollisionDetector => baseState.CollisionDetector;
            public IReadonlyPlayer Player => baseState.Player;
            public IReadonlyCamera Camera => baseState.Camera;

            public float CamX => baseState.Camera.Position.X;
            public float CamY => baseState.Camera.Position.Y;
            public float CamZ => baseState.Camera.Position.Z;
            public IReadonlyPositionObject CameraCenter => baseState.Camera.Center;
            public float CamAngle => baseState.Camera.Angle;

            public float PlayerX => baseState.Player.Position.X;
            public float PlayerY => baseState.Player.Position.Y;
            public float PlayerZ => baseState.Player.Position.Z;

            public IReadonlyBlockWorld BlockWorld => baseState.World.Blocks;
            public IReadonlyWorldObjectContainer WorldObjects => baseState.World;
        }
    }
}
