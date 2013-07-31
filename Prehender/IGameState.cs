using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Prehender
{
    public abstract class GameState
    {
        public abstract void Update(PrehenderGame bbg, FrameEventArgs e);
        public abstract void Render(PrehenderGame bbg,FrameEventArgs e);
        public virtual void RenderHUD(PrehenderGame bbg, FrameEventArgs e)
        {

        }
        private PrehenderGame _OwnerWindow = null;
        public PrehenderGame Owner { get { return _OwnerWindow;} set {_OwnerWindow=value;}}
        protected HashSet<MouseButton> PressedMouse = new HashSet<MouseButton>();
        protected HashSet<Key> PressedKeys = new HashSet<Key>();
      
        protected Vector3 UnProject(Vector2 Pos,Vector2 Size,Matrix4 modelView,Matrix4 perspective)
        {
            // generate an object space ray
            // convert the viewport coords to openGL normalized device coords
            float xpos = 2 * (Pos.X / Size.X) - 1;
            float ypos = 2 * (1 - Pos.Y / Size.Y) - 1;
            
            Vector4 startRay = new Vector4(xpos, ypos, -1, 1);
            Vector4 endRay = new Vector4(xpos, ypos, 1, 1);

            // Reverse Project
            Matrix4 trans = modelView * perspective;
            trans.Invert();
            startRay = Vector4.Transform(startRay, trans);
            endRay = Vector4.Transform(endRay, trans);
            Vector3 sr = startRay.Xyz / startRay.W;
            Vector3 er = endRay.Xyz / endRay.W;

            return er;
        }

        protected GameState(PrehenderGame OwnerGame)
        {
            _OwnerWindow = OwnerGame;
            _OwnerWindow.Mouse.ButtonDown += Mouse_ButtonDown;
            _OwnerWindow.Mouse.ButtonUp += Mouse_ButtonUp;
            _OwnerWindow.Mouse.Move += Mouse_Move;
            _OwnerWindow.Mouse.WheelChanged += Mouse_WheelChanged;
            _OwnerWindow.Keyboard.KeyDown += Keyboard_KeyDown;
            _OwnerWindow.Keyboard.KeyUp += Keyboard_KeyUp;
            
        }

        void Keyboard_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            
            PressedKeys.Remove(e.Key);
            if (Owner != null && Owner.currentState == this)
            {
                
                KeyUp(sender, e);
                KeyPressed(e.Key);
            }
        }

        void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            PressedKeys.Add(e.Key);
            if (Owner != null && Owner.currentState == this) KeyDown(sender, e);
        }
        protected virtual void KeyPressed(Key keyelement)
        {

        }
        protected virtual void KeyDown(object sender, KeyboardKeyEventArgs e)
        {

        }
        protected virtual void KeyUp(object sender,KeyboardKeyEventArgs e)
        {
        
        }
        
        protected virtual void MouseButtonUp(object sender,MouseButtonEventArgs e){
        

        }
        protected virtual void MouseButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        protected virtual void MouseMove(object sender, MouseMoveEventArgs e)
        {

        }
        protected virtual void MouseWheel(object sender,MouseWheelEventArgs e)
        {

        }
        void Mouse_WheelChanged(object sender, MouseWheelEventArgs e)
        {
            if (_OwnerWindow != null && _OwnerWindow.currentState == this)
            {
                MouseWheel(sender, e);
            }
        }
        void Mouse_Move(object sender, MouseMoveEventArgs e)
        {
            if (_OwnerWindow != null && _OwnerWindow.currentState == this)
            {
                MouseMove(sender, e);
            }
        }

        void Mouse_ButtonUp(object sender, MouseButtonEventArgs e)
        {
            PressedMouse.Remove(e.Button);
            if (_OwnerWindow!=null && _OwnerWindow.currentState == this)
                MouseButtonUp(sender, e);
        }

        void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            PressedMouse.Add(e.Button);
            if (_OwnerWindow!=null && _OwnerWindow.currentState == this)
            {
                MouseButtonDown(sender, e);
            }
        }

      
    }
}
