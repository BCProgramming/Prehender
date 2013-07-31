using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Prehender.HUD;

namespace Prehender
{
    public class GameRunningState : GameState
    {
        public IList<GameObject> GameObjects = new List<GameObject>();
        public ParticleBatch pBatch = new ParticleBatch();
        private BlockObject DeathBlock = null;
        public World PhysicsWorld;
        private Camera _cam = null;
        public Camera cam { get { return _cam ; } set { _cam = value; } }
        private SelectedBallColourHudElement HudObject = null;
        
        public GameRunningState(PrehenderGame OwnerGame) :base(OwnerGame)
        {

            OwnerGame.Resize += OwnerGame_Resize;

            PhysicsWorld = new World(new CollisionSystemSAP());
            PhysicsWorld.Gravity = new JVector(0, -20, 0);
            PhysicsWorld.CollisionSystem.CollisionDetected += CollisionSystem_CollisionDetected;
            PhysicsWorld.Events.BodiesBeginCollide += Events_BodiesBeginCollide;
            PhysicsWorld.Events.DeactivatedBody += Events_DeactivatedBody;
            PhysicsWorld.Events.BodiesEndCollide += Events_BodiesEndCollide;
            cam = new Camera(new Vector3(-140, 0, 0), new Vector3(0, 0, 0));
            Color[] usecolors = new Color[]{Color.Red,Color.Yellow,Color.Green,Color.Blue};
            int colorint = 0;

            int xpos = 0, ypos = 0, zpos = 0;
            //Blocks.Add(bb);
            for (int i = 0; i < 1000; i++)
            {
                xpos++;
                if (xpos == 10)
                {
                    ypos++; xpos = 0;
                    if (ypos == 10)
                    {
                        zpos++; ypos = 0;
                    }
                }
                colorint = PrehenderGame.rgen.Next(usecolors.Length);
                
                GameBlock bx = new GameBlock(Owner, new Vector3(-30 + (float)xpos * 6, -30 + ypos * 6, -30 + zpos * 6),
                    new Vector3(5, 5, 5), Vector3.Zero,usecolors[colorint]);
                GameObjects.Add(bx);
                lock (PhysicsWorld)
                {
                    PhysicsWorld.AddBody(bx.PhysicsObject);
                }
            }
            // Balls.Add(new Ball(PhysicsWorld,this, new Vector3(-70, 0, 0), new Vector3(120,10,-20)));

            BlockObject bb = new BlockObject(Owner, new Vector3(0, -200, 0), new Vector3(9000, 10, 9000), Vector3.Zero);
            bb.PhysicsObject.Mass = 32768;
            bb.PhysicsObject.AffectedByGravity = false;
            GameObjects.Add(bb);
            bb.PhysicsObject.IsStatic = true;
            lock (PhysicsWorld) { PhysicsWorld.AddBody(bb.PhysicsObject); }
            DeathBlock = bb;
            GameObjects.Add(pBatch);
            HudObject = new SelectedBallColourHudElement();
            GameObjects.Add(HudObject);
            //GameObjects.Add(new SelectedBallColourHudElement());
            

        }

        void Events_BodiesEndCollide(RigidBody arg1, RigidBody arg2)
        {
            
        }

        void Events_DeactivatedBody(RigidBody obj)
        {
            if (obj.Tag is Ball)
            {

                Owner.Defer(() =>
                {
                    GameObjects.Remove((GameObject)(obj.Tag));
                    PhysicsWorld.RemoveBody(obj);
                    Debug.Print("Removed:" + obj.Tag.GetType().Name);
                });

            }
            else if(obj.Tag is GameBlock)
            {
               // Owner.Defer(() => { obj.IsStatic = true; });
            }
        }
        protected override void KeyPressed(Key keyelement)
        {
            if (keyelement == Key.Pause)
            {
                Owner.currentState = new PausedGameState(Owner);
            }
        }
         
        void OwnerGame_Resize(object sender, EventArgs e)
        {
            var Width = Owner.Width;
            var Height = Owner.Height;
            GL.Viewport(0, 0, Width, Height);
            cam.AspectRatio =  Width / (double)Height;

        }
        private TimeSpan CollisionEventTimeout = new TimeSpan(0,0,0,1);
        ConcurrentDictionary<GameObject, DateTime> LastCollisions = new ConcurrentDictionary<GameObject, DateTime>();


        void CollisionSystem_CollisionDetected(RigidBody body1, RigidBody body2, JVector point1, JVector point2, JVector normal, float penetration)
        {
            var FirstPoint = new Vector3(point1.X, point1.Y, point1.Z);
            var SecondPoint = new Vector3(point2.X, point2.Y, point2.Z);
            if (body1.Tag != null && body2.Tag != null)
            {

                GameObject First = body1.Tag as GameObject;
                GameObject Second = body2.Tag as GameObject;
                if (First == null || Second == null) return;
                if (!LastCollisions.ContainsKey(First))
                {
                    LastCollisions[First] = DateTime.Now;
                }
                if (!LastCollisions.ContainsKey(Second))
                {
                    LastCollisions[Second] = DateTime.Now;
                }

                var Previous = new[] { DateTime.Now - LastCollisions[First], DateTime.Now - LastCollisions[Second] };

                if (Previous.All((p) => p > CollisionEventTimeout))
                {

                  /*  Owner.Defer(() =>
                    {

                        for (int i = 0; i < 4; i++)
                        {
                            pBatch.AddParticle(FirstPoint, 1);
                        }


                    });*/




                    //get last collision for each.


                }
            }
        }
        void Events_BodiesBeginCollide(RigidBody body1, RigidBody body2)
        {


            if (body1.Tag == DeathBlock || body2.Tag == DeathBlock)
            {
                BlockObject killblock = DeathBlock;
                GameObject RemoveThis = (body1.Tag == killblock ? body2.Tag : body1.Tag) as GameObject;
                GameObjects.Remove(RemoveThis);
            }

            Ball ballcollide = null;
            GameBlock blockcollide = null;
            if (body1.Tag is Ball) ballcollide = body1.Tag as Ball;
            if (body2.Tag is Ball) ballcollide = body2.Tag as Ball;
            if (body1.Tag is GameBlock) blockcollide = body1.Tag as GameBlock;
            if (body2.Tag is GameBlock) blockcollide = body2.Tag as GameBlock;

            if (ballcollide != null && blockcollide != null)
            {
                if ((ballcollide.PhysicsElement.LinearVelocity - blockcollide.PhysicsObject.LinearVelocity).Length() > 10)
                    PrehenderGame.instance.SoundMan["bounce"].Play(false);
                blockcollide.BallImpact(ballcollide);

             

            }
            else if (body1.Tag is GameBlock && body2.Tag is GameBlock)
            {
                GameBlock A = (GameBlock)body1.Tag;
                GameBlock B = (GameBlock)body2.Tag;
                if (((IColoredElement)A).ElementColor == ((IColoredElement)B).ElementColor)
                {
                    foreach (var p in new GameBlock[] { A, B })
                    {
                        p.PhysicsObject.IsStatic = false;
                        p.PhysicsObject.AffectedByGravity = true;
                    }
                }
            }
        }
       
        public IEnumerable<T> getGameElements<T>() where T : GameObject
        {
            return from obj in GameObjects where obj is T select obj as T;
        }
        public void removeGameElements<T>(Predicate<T> test = null) where T : GameObject
        {

            foreach (var iterateobject in from obj in GameObjects where obj is T select obj as T)
            {
                if (iterateobject is IPhysical)
                {
                    (iterateobject as IPhysical).RemoveFromWorld(PhysicsWorld);
                }
                GameObjects.Remove(iterateobject);

            }
        }

        public override void Update(PrehenderGame bbg,FrameEventArgs e)
        {
            HandlePolledKeys(e);
            float step = 1.0f / (float)Owner.RenderFrequency;
            if (step > 1.0f / 100.0f) step = 1.0f / 100.0f;
            lock (PhysicsWorld)
            {
                
                PhysicsWorld.Step(step*2, true);
            }
            List<GameObject> removeItems = new List<GameObject>();
            foreach (var iterate in GameObjects) 
            {
                if (iterate.Update(this))
                {
                    removeItems.Add(iterate);
                }

            }
            bbg.Defer(() =>
            {
                foreach (var iterate in removeItems)
                {
                    GameObjects.Remove(iterate);
                }

            });
            //
            
        }
        private void HandlePolledKeys(FrameEventArgs e)
        {
            float XImpulse = 0, YImpulse = 0, ZImpulse = 0;
            float CamX, CamY, CamZ;
            CamX = CamY = CamZ = 0;
            if (Owner.Keyboard[Key.W])
            {
                //forward
                YImpulse += 1;
            }
            if (Owner.Keyboard[Key.S])
            {
                YImpulse -= 1;
            }
            if (Owner.Keyboard[Key.A])
            {
                XImpulse -= 1;
            }
            if (Owner.Keyboard[Key.D])
            {
                XImpulse += 1;
            }
            if (Owner.Keyboard[Key.Q])
            {
                ZImpulse += 1;
            }
            if (Owner.Keyboard[Key.A])
                ZImpulse -= 1;
            //I moves up, J moves left, K moves down, L moves right.
            if (Owner.Keyboard[Key.I])
            {
                CamY = 1;
            }
            if (Owner.Keyboard[Key.K])
                CamY = -1;
            if (Owner.Keyboard[Key.J])
                CamZ = -1;
            if (Owner.Keyboard[Key.L])
                CamZ = 1;
            
            Vector3 useTranslation = new Vector3(CamX,CamY,CamZ);
            //move the camera.
            //cam = new Camera(cam.Location + useTranslation, cam.Target + useTranslation);
            if (!(useTranslation.Equals(Vector3.Zero)))
            {
                cam.Location += useTranslation;
                cam.Target += useTranslation;
            }
            foreach (var iterate in this.getGameElements<Ball>())
            {
                var CreatedVector = new JVector(XImpulse, YImpulse, ZImpulse);

                CreatedVector *= 100;
                
                iterate.PhysicsElement.ApplyImpulse(CreatedVector);
            }

        }
        public override void Render(PrehenderGame bbg,FrameEventArgs e)
        {
            GL.Enable(EnableCap.AlphaTest);
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            float[] light_position = { 0, 70, 70, 0 };
            GL.Light(LightName.Light0, LightParameter.Position, light_position);
            GL.Light(LightName.Light0, LightParameter.Diffuse, 1.0f);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Multisample);
            cam.Apply();



            DoRender();
        }
        public override void RenderHUD(PrehenderGame bbg,FrameEventArgs e)
        {
            foreach (var Render in (from t in GameObjects where (t is HUDElement) select (t as HUDElement)))
            {
                Render.Render(this);
            }

        }
        private void DoRender()
        {


            foreach (var Render in (from t in GameObjects where !(t is HUDElement) select t))
            {

                Render.Render(this);
            }
            Vector3 MousePos = base.UnProject(new Vector2(Owner.Mouse.X, Owner.Mouse.Y),new Vector2(Owner.Size.Width,Owner.Size.Height),   cam.Model, cam.Projection);
            Vector3 XDiff = new Vector3(2,0,0);
            Vector3 YDiff = new Vector3(0,2,0);
            Vector3 ZDiff = new Vector3(0,0,2);
            GL.PointSize(10);
            GL.LineWidth(3);
            GL.Begin(BeginMode.LineLoop);
            GL.Color3(Color.White);
            GL.Vertex3(MousePos + XDiff);
            GL.Vertex3(MousePos - XDiff);
            GL.Vertex3(MousePos + XDiff + YDiff);
            GL.Vertex3(MousePos + YDiff);
            GL.End();


            

           
        }
        protected override void MouseWheel(object sender, MouseWheelEventArgs e)
        {
           // float useamount = e.DeltaPrecise / 100;
           // cam.ZoomIn(useamount);
            if (e.Delta > 0) HudObject.AdvanceColor();
            else if (e.Delta < 0) HudObject.DecreaseColor();
        }
        protected override void MouseButtonDown(Object sender,MouseButtonEventArgs e)
        {
           // Debug.Print("mousedown:" + e.Button);
           // Debug.Print(new StackTrace().ToString());

            Vector3 usevelocity = (cam.Target - cam.Location);
            usevelocity.Normalize();
            usevelocity *= 150;
            //Ball b = new Ball(PhysicsWorld, this, cam.Location, usevelocity, 4);
            Ball b = new Ball(PhysicsWorld, Owner, cam.Location, usevelocity);
            b.ElementColor = HudObject.BallColor;
            GameObjects.Add(b);
        }
        
        protected override void MouseMove(Object sender,MouseMoveEventArgs e){
            if (PressedMouse.Contains(MouseButton.Left))
            {
                cam.XRotation += e.XDelta;
                cam.YRotation += e.YDelta;
            }
            else if (PressedMouse.Contains(MouseButton.Middle))
            {
                cam.ZRotation += e.XDelta;
            }
            else if (PressedMouse.Contains(MouseButton.Right))
            {
                cam.Location = new Vector3(cam.Location.X + e.XDelta, cam.Location.Y, cam.Location.Z + e.YDelta);
                cam.Target = new Vector3(cam.Target.X + e.XDelta, cam.Target.Y, cam.Target.Z + e.YDelta);
            }
            
            
            
        }

    }
}
