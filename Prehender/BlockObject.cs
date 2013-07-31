using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Prehender
{
    /// <summary>
    /// Base class for all Block-type objects. These are not necessarily the game blocks.
    /// So named because if I called it "Block" I would get really confused between BlockObject and BASeBlock.
    /// 
    /// </summary>
    public class BlockObject : GameObject, IPhysical,ILocatable,IMovingObject
    {
        
        public enum BlockFaceConstants
        {
            Front,
            Back,
            Top,
            Bottom,
            Left,
            Right
        }

        protected Dictionary<BlockFaceConstants, String> FaceTextures = new Dictionary<BlockFaceConstants, string>();
        protected Dictionary<BlockFaceConstants, Color> FaceColors = new Dictionary<BlockFaceConstants, Color>();
        protected Dictionary<BlockFaceConstants, Color> ActiveFaceColors = new Dictionary<BlockFaceConstants, Color>(); 
        protected RigidBody BlockBody = null;
        protected Vector3 _Location;
        protected Vector3 _Velocity;
        protected Vector3 _Size;
        public Vector3 Location { get { return _Location; } set { _Location = value; } }
        public Vector3 Velocity { get { return _Velocity; } set { _Velocity = value; } }
        public Vector3 Size { get { return _Size; } set { _Size = value; } }
        public RigidBody PhysicsObject { get { return BlockBody; } }
      
        public Vector3 getCenterPoint()
        {
            return Location + (Size / 2);
        }
        public BlockObject(PrehenderGame bbg,Vector3 pLocation, Vector3 pSize, Vector3 pVelocity)
        {
            
            _Location = pLocation;
            _Size = pSize;
            _Velocity = pVelocity;
            FaceTextures = new Dictionary<BlockFaceConstants, string>(){
                {BlockFaceConstants.Front, "Generic_1"},
                {BlockFaceConstants.Back, "Generic_2"},
                {BlockFaceConstants.Top, "Generic_3"},
                {BlockFaceConstants.Bottom,"Generic_4"},
                {BlockFaceConstants.Left,"Invincible"},
                {BlockFaceConstants.Right,"Strong1"}};

            foreach (var iterate in Enum.GetValues(typeof(BlockFaceConstants)))
            {
                FaceColors.Add((BlockFaceConstants)iterate, Color.FromArgb(128,Color.White));
            }
            foreach (var iterate in Enum.GetValues(typeof(BlockFaceConstants)))
            {
                ActiveFaceColors.Add((BlockFaceConstants)iterate, Color.FromArgb(128, Color.Orange));
            }
            BlockBody = new RigidBody(new BoxShape(_Size.X, _Size.Y, _Size.Z));
            BlockBody.AffectedByGravity = false;
            BlockBody.IsStatic = true;
            BlockBody.Mass = 5;
            BlockBody.Tag = this;
            BlockBody.Position = new JVector(_Location.X, _Location.Y, _Location.Z);
            
            //bbg.PhysicsWorld.AddBody(BlockBody);
            
        }

        public override void Render(GameRunningState gameState)
        {
          
            
            var A = new Vector3(Location.X, Location.Y, Location.Z);
            var B = new Vector3(Location.X + Size.X, Location.Y, Location.Z);
            var C = new Vector3(Location.X + Size.X, Location.Y+Size.Y, Location.Z);
            var D = new Vector3(Location.X, Location.Y+Size.Y, Location.Z);

            var E = new Vector3(Location.X, Location.Y, Location.Z+Size.Z);
            var F = new Vector3(Location.X + Size.X, Location.Y, Location.Z+Size.Z);
            var G = new Vector3(Location.X + Size.X, Location.Y + Size.Y, Location.Z+Size.Z);
            var H = new Vector3(Location.X, Location.Y + Size.Y, Location.Z+Size.Z);
            if (BlockBody.Shape is BoxShape)
            {
                BoxShape bs = BlockBody.Shape as BoxShape;
                dsDrawBox(BlockBody.Position, BlockBody.Orientation, bs.Size);

            }
            //GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan[FaceTextures[BlockFaceConstants.Front]]);


            /*
            GL.Begin(BeginMode.Quads);
            //GL.Color3(Color.Blue);
            //Front face ABCD.
            GL.TexCoord2(0, 0);
            GL.Vertex3(A);
            GL.TexCoord2(1, 0);
            GL.Vertex3(B);
            GL.TexCoord2(1, 1);
            GL.Vertex3(C);
            GL.TexCoord2(0, 1);
            GL.Vertex3(D);
            GL.End();
            GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan["GENERIC_2"]);
            GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan[FaceTextures[BlockFaceConstants.Back]]);
            GL.Begin(BeginMode.Quads);
            
            //Front: ABCD
            //Left: ADGE
            //Right: BFHC
            //Top:EFBA
            //Bottom:DCHG
            //Back:FEGH
            GL.TexCoord2(0, 0);
            GL.Vertex3(E);
            GL.TexCoord2(1, 0);
            GL.Vertex3(F);
            GL.TexCoord2(1, 1);
            GL.Vertex3(G);
            GL.TexCoord2(0, 1);
            GL.Vertex3(H);
            GL.End();
            //Top Face
            GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan[FaceTextures[BlockFaceConstants.Bottom]]);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex3(E);
            GL.TexCoord2(1, 0);
            GL.Vertex3(F);
            GL.TexCoord2(1, 1);
            GL.Vertex3(B);
            GL.TexCoord2(0, 1);
            GL.Vertex3(A);
            GL.End();
            GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan[FaceTextures[BlockFaceConstants.Top]]);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex3(D);
            GL.TexCoord2(1, 0);
            GL.Vertex3(C);
            GL.TexCoord2(1, 1);
            GL.Vertex3(G);
            GL.TexCoord2(0, 1);
            GL.Vertex3(H);
            GL.End();
            GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan[FaceTextures[BlockFaceConstants.Right]]);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex3(G);
            GL.TexCoord2(1, 0);
            GL.Vertex3(E);
            GL.TexCoord2(1, 1);
            GL.Vertex3(A);
            GL.TexCoord2(0, 1);
            GL.Vertex3(D);
            GL.End();
            
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex3(B);
            GL.TexCoord2(1, 0);
            GL.Vertex3(F);
            GL.TexCoord2(1, 1);
            GL.Vertex3(G);
            GL.TexCoord2(0, 1);
            GL.Vertex3(C);
            GL.End();
            
            GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan[FaceTextures[BlockFaceConstants.Left]]);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex3(E);
            GL.TexCoord2(1, 0);
            GL.Vertex3(A);
            GL.TexCoord2(1, 1);
            GL.Vertex3(D);
            GL.TexCoord2(0, 1);
            GL.Vertex3(H);
            GL.End();
           
          /*  GL.Color3(Color.Firebrick);
            GL.Vertex3(_Location.X+Size.X, _Location.Y, _Location.Z);
            GL.Vertex3(_Location.X+Size.X, _Location.X, _Location.Z + Size.Z);
            GL.Vertex3(_Location.X+Size.X, _Location.Y + Size.Y, _Location.Z + Size.Z);
            GL.Vertex3(_Location.X+Size.X, _Location.Y + Size.Y, _Location.Z);*/

            
        }
        private void setTransform(float[] pos, float[] R)
        {
            //GLfloat
            float[] matrix = new float[16];
            matrix[0] = R[0];
            matrix[1] = R[4];
            matrix[2] = R[8];
            matrix[3] = 0;
            matrix[4] = R[1];
            matrix[5] = R[5];
            matrix[6] = R[9];
            matrix[7] = 0;
            matrix[8] = R[2];
            matrix[9] = R[6];
            matrix[10] = R[10];
            matrix[11] = 0;
            matrix[12] = pos[0];
            matrix[13] = pos[1];
            matrix[14] = pos[2];
            matrix[15] = 1;
            GL.PushMatrix();
            GL.MultMatrix(matrix);
        }
        private void drawBox(float[] sides)
        {
            float lx = sides[0] * 0.5f;
            float ly = sides[1] * 0.5f;
            float lz = sides[2] * 0.5f;

            Vector3 A = new Vector3(-lx, ly, -lz);
            Vector3 B = new Vector3(lx, ly, -lz);
            Vector3 C = new Vector3(lx, -ly, -lz);
            Vector3 D = new Vector3(-lx, -ly, -lz);

            var dblx = new Vector3(0,0,lz * 2);

            Vector3 E = new Vector3(-lx, ly, lz);
            Vector3 F = new Vector3(lx, ly, lz);
            Vector3 G = new Vector3(lx, -ly, lz);
            Vector3 H = new Vector3(-lx, -ly, lz);
           
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Normalize);
            
            GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan[FaceTextures[BlockFaceConstants.Left]]);
            GL.Color4(FaceColors[BlockFaceConstants.Left]);
            GL.Begin(BeginMode.TriangleFan);
            

           //Left Side... DHAE...
            //GL.Normal3(-1.0f, 0, 0); // GL.Normal3(-1, 0, 0) no funciona
            GL.Normal3(-1, 0, 0);
            GL.TexCoord2(1, 1);
            
            GL.Vertex3(D);
            GL.TexCoord2(0, 1);
            
            GL.Vertex3(H);
            GL.TexCoord2(0, 0);
            
            GL.Vertex3(E);
            GL.TexCoord2(1, 0);
            GL.Vertex3(A);

            GL.End();

            //Right
            GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan[FaceTextures[BlockFaceConstants.Right]]);
            GL.Color4(FaceColors[BlockFaceConstants.Right]);
            GL.Begin(BeginMode.Quads);
            GL.Normal3(1, 0, 0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(B);
            GL.TexCoord2(1, 0);
            GL.Vertex3(F);
            GL.TexCoord2(1, 1);
            GL.Vertex3(G);
            GL.TexCoord2(0, 1);
            GL.Vertex3(C);
            
            //Front
            GL.End();
            GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan[FaceTextures[BlockFaceConstants.Front]]);
            
            GL.Color4(FaceColors[BlockFaceConstants.Front]);
            GL.Begin(BeginMode.Quads);
            GL.Normal3(0, 0, -1);
            GL.TexCoord2(0, 0);
            GL.Vertex3(A);
            GL.TexCoord2(1, 0);
            GL.Vertex3(B);
            GL.TexCoord2(1, 1);
            GL.Vertex3(C);
            GL.TexCoord2(0, 1);
            GL.Vertex3(D);
            

            //Back
            GL.End();
            GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan[FaceTextures[BlockFaceConstants.Back]]);
            GL.Color4(FaceColors[BlockFaceConstants.Back]);
            
            GL.Begin(BeginMode.Quads);
            GL.Normal3(0, 0, 1);
            GL.TexCoord2(0, 0);
            GL.Vertex3(F);
            GL.TexCoord2(1, 0);
            GL.Vertex3(E);
            GL.TexCoord2(1, 1);
            GL.Vertex3(H);
            GL.TexCoord2(0, 1);
            GL.Vertex3(G);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan[FaceTextures[BlockFaceConstants.Top]]);
            GL.Color4(FaceColors[BlockFaceConstants.Top]);
            
            GL.Begin(BeginMode.Quads);
            GL.Normal3(0, 1, 0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(E);
            GL.TexCoord2(1, 0);
            GL.Vertex3(F);
            GL.TexCoord2(1, 1);
            GL.Vertex3(B);
            GL.TexCoord2(0, 1);
            GL.Vertex3(A);
            GL.End();

            //Bottom
            GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan[FaceTextures[BlockFaceConstants.Bottom]]);
            GL.Color4(FaceColors[BlockFaceConstants.Bottom]);
            
            GL.Begin(BeginMode.Quads);
            GL.Normal3(0, -1, 0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(D);
            GL.TexCoord2(1, 0);
            GL.Vertex3(C);
            GL.TexCoord2(1, 1);
            GL.Vertex3(G);
            GL.TexCoord2(0, 1);
            GL.Vertex3(H);
            GL.End();

            //GL.Normal3(0, 1.0f, 0); // GL.Normal3(0, 1, 0) no funciona

            /*
            GL.TexCoord2(1, 1);
            
            GL.Vertex3(lx, ly, -lz);
            GL.TexCoord2(0, 1);
            
            GL.Vertex3(lx, ly, lz);
            //GL.Normal3(1.0f, 0, 0); // GL.Normal3(1, 0, 0) no funciona
            GL.TexCoord2(1, 0);
            GL.Vertex3(lx, -ly, -lz);
            GL.TexCoord2(0, 0);
            GL.Vertex3(lx, -ly, lz);
            //GL.Normal3(0, -1.0f, 0); // GL.Normal3(0, -1, 0) no funciona
            GL.TexCoord2(1, 1);
            GL.Vertex3(-lx, -ly, -lz);
            GL.TexCoord2(1, 0);
            GL.Vertex3(-lx, -ly, lz);
            GL.End();



           */

           
        }
        protected void dsDrawBox(JVector pos, JMatrix R, JVector sides)
        {
            float[] pos2 = Conversion.ToFloat(pos);
            float[] R2 = Conversion.ToFloat(R);
            float[] fsides = Conversion.ToFloat(sides);
            dsDrawBox(pos2, R2, fsides);
        }
       
        private const bool use_shadows = false;
        protected void dsDrawBox(float[] pos, float[] R, float[] sides)
        {
            
           
            setTransform(pos, R);
            var previousfacecolors = FaceColors;
            if(PhysicsObject.IsActive && ! PhysicsObject.IsStatic){
                FaceColors=ActiveFaceColors;
            }
            drawBox(sides);
            FaceColors = previousfacecolors;
            GL.PopMatrix();

         
        }

        public override bool Update(GameRunningState gameState)
        {
            //BlockBody.Update();

            return false;

        }

        public void AddToWorld(World Target)
        {
            Target.AddBody(PhysicsObject);
        }

        public void RemoveFromWorld(World Target)
        {
            Target.RemoveBody(PhysicsObject);
        }
    }
}
