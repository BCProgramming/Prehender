using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Prehender
{
    //A level determines the initial state for a given... well, level.
    class cLevel
    {
        private IList<Ball> _Balls = new List<Ball>();
        private IList<GameBlock> _Blocks = new List<GameBlock>();
        private ArenaStyle LevelArena = new BoxArena(new Vector3(500, 500, 500));
        private Vector3 _CameraPosition;
        private Vector3 _CameraTarget;
        public Vector3 CameraPosition { get { return _CameraPosition; } set { _CameraPosition = value; } }
        public Vector3 CameraTarget { get { return _CameraTarget; } set { _CameraTarget = value; } }
        public IList<Ball> LevelBalls { get { return _Balls; } set { _Balls = value; } }
        public IList<GameBlock> LevelBlocks { get { return _Blocks; } set { _Blocks = value; } }

        public cLevel()
        {

        }

        public void Apply(PrehenderGame ToGame){
            
        }
        




    }
}
