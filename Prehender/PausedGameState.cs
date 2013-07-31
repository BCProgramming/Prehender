using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace Prehender
{
    class PausedGameState : GameState
    {
        private GameState _PausedState;
        private GameState _ResumeState;
        public GameState PausedState { get { return _PausedState; } set { _PausedState = value; } }
        public GameState ResumeState { get { return _ResumeState; } set { _ResumeState = value; } }
        public PausedGameState(PrehenderGame Owner,GameState PreviousState,GameState ResumeState):base(Owner)
        {
            _PausedState = PreviousState;
            _ResumeState = ResumeState;
        }
        public PausedGameState(PrehenderGame Owner):this(Owner,Owner.currentState,Owner.currentState)
        {

        }
        protected override void KeyPressed(OpenTK.Input.Key keyelement)
        {
            if (keyelement == Key.Pause)
            {
                //unpause.
                Owner.currentState = ResumeState;
            }
        }

        public override void Update(PrehenderGame bbg, FrameEventArgs e)
        {
            //no update!
        }

        public override void Render(PrehenderGame bbg, FrameEventArgs e)
        {
            //render the paused state...
            _PausedState.Render(bbg, e);
            
        }
    }
}
