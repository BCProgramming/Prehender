using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Prehender
{
    class BoxArena : ArenaStyle
    {
        private Vector3 _ArenaSize;
        private float _WallWidth = 10;
        public Vector3 ArenaSize { get { return _ArenaSize; } set { _ArenaSize = value; } }
        public BoxArena(Vector3 pArenaSize)
        {
            _ArenaSize = pArenaSize;
        }

        public override void Build(GameRunningState Game)
        {
            //we build a interior rectangular prism centered on 0,0.

            //first, create the Top face.
            BlockObject TopFace = new BlockObject(Game.Owner,new Vector3(-_ArenaSize.X/2,_ArenaSize.Y/2-_WallWidth,-ArenaSize.Z/2),new Vector3(_ArenaSize.X,_WallWidth,_ArenaSize.Z),Vector3.Zero);

            //Bottom face is similar but at the bottom.
            BlockObject BottomFace = new BlockObject(Game.Owner, new Vector3(-_ArenaSize.X / 2, -_ArenaSize.Y / 2 , -ArenaSize.Z / 2), new Vector3(_ArenaSize.X, _WallWidth, _ArenaSize.Z), Vector3.Zero);


            //Side face, for X Direction.
            BlockObject LeftFace = new BlockObject(Game.Owner,
                new Vector3(-_ArenaSize.X / 2 - _WallWidth,
                    -_ArenaSize.Y / 2, -_ArenaSize.Z / 2),
                    new Vector3(_WallWidth, _ArenaSize.Y, _ArenaSize.Z), Vector3.Zero);
            BlockObject RightFace = new BlockObject(Game.Owner,
                new Vector3(_ArenaSize.X / 2,
                    -_ArenaSize.Y / 2, -_ArenaSize.Z / 2),
                    new Vector3(_WallWidth, _ArenaSize.Y, _ArenaSize.Z), Vector3.Zero);

            BlockObject[] Faces = new BlockObject[] { TopFace, BottomFace, LeftFace, RightFace };


            foreach (var facebox in Faces)
            {
                facebox.PhysicsObject.IsStatic = true;
                Game.GameObjects.Add(facebox);
            }
            
        }
    }
}
