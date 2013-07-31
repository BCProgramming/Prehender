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
    public class Ball : GameObject,IPhysical,IColoredElement
    {
        private Vector3 _Location;
        private Vector3 _Velocity;
        private double _Radius;
        private Color _elementColor = Color.Red;
        public Color ElementColor { get { return _elementColor; } set { _elementColor = value; } }
        private String _TextureMapName = "generic_1";
        public double Radius { get { return _Radius;} set {_Radius=value;}}
        public Vector3 Location { get { return _Location;} set {_Location=value;}}
        public Vector3 Velocity { get { return _Velocity;} set {_Velocity=value;}}
        public String TextureName { get { return _TextureMapName; } set { _TextureMapName = value; } }
        GeometryHelper.Vertex[] SphereVertices;
        ushort[] SphereElements;
        RigidBody rb = null;
        public RigidBody PhysicsElement { get { return rb; } }
        public Ball(World physicsWorld,PrehenderGame game,Vector3 pLocation, Vector3 pVelocity, double pRadius = 3)
        {
            _Location = pLocation;
            _Velocity = pVelocity;
            _Radius = pRadius;
            rb = new RigidBody(new SphereShape((float)_Radius));
            rb.Position = new JVector(pLocation.X, pLocation.Y, pLocation.Z);
            rb.LinearVelocity = new JVector(pVelocity.X, pVelocity.Y, pVelocity.Z);
            rb.Mass = 50;
            rb.Tag = this;
            physicsWorld.AddBody(rb);
        }
        public override void Render(GameRunningState gameState)
        {
            
            //GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan[_TextureMapName]);
            GL.Disable(EnableCap.Texture2D);
            GL.Color3(_elementColor);
            SphereShape sphereshape = rb.Shape as SphereShape;
            dsDrawSphere(rb.Position, rb.Orientation, sphereshape.Radius - 0.1f);
            
        }
        protected void dsDrawSphere(JVector pos, JMatrix R, float radius)
        {
            float[] pos2 = Conversion.ToFloat(pos);
            float[] R2 = Conversion.ToFloat(R);
            dsDrawSphere(pos2, R2, radius);
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
        protected void dsDrawSphere(float[] pos, float[] R, float radius)
        {
    
          
            //GL.Enable(EnableCap.Normalize);
           // GL.ShadeModel(ShadingModel.Smooth);
            //GL.Enable(EnableCap.Texture2D);
            
            setTransform(pos, R);
            GL.Scale(radius, radius, radius);
            drawSphere();
            GL.PopMatrix();
            //GL.Disable(EnableCap.Normalize);

            
        }
        private static Dictionary<Color,int> listnum = new Dictionary<Color, int>(); //GLunint TZ
        private const float ICX = 0.525731112119133606f;
        private const float ICZ = 0.850650808352039932f;
        private readonly float[][] idata = new float[][]
        {
            new float[]{-ICX, 0, ICZ},
            new float[]{ICX, 0, ICZ},
            new float[]{-ICX, 0, -ICZ},
            new float[]{ICX, 0, -ICZ},
            new float[]{0, ICZ, ICX},
            new float[]{0, ICZ, -ICX},
            new float[]{0, -ICZ, ICX},
            new float[]{0, -ICZ, -ICX},
            new float[]{ICZ, ICX, 0},
            new float[]{-ICZ, ICX, 0},
            new float[]{ICZ, -ICX, 0},
            new float[]{-ICZ, -ICX, 0}
        };
        private readonly int[][] index = new int[][]
        {
            new int[]{0, 4, 1}, new int[]{0, 9, 4},
            new int[]{9, 5, 4},	  new int[]{4, 5, 8},
            new int[]{4, 8, 1},	  new int[]{8, 10, 1},
            new int[]{8, 3, 10},   new int[]{5, 3, 8},
            new int[]{5, 2, 3},	  new int[]{2, 7, 3},
            new int[]{7, 10, 3},   new int[]{7, 6, 10},
            new int[]{7, 11, 6},   new int[]{11, 0, 6},
            new int[]{0, 1, 6},	  new int[]{6, 1, 10},
            new int[]{9, 0, 11},   new int[]{9, 11, 2},
            new int[]{9, 2, 5},	 new int[] {7, 2, 11},
        };
        private int sphere_quality = 1;
        private void drawSphere()
        {
            // icosahedron data for an icosahedron of radius 1.0
            //		# define ICX 0.525731112119133606f
            //		# define ICZ 0.850650808352039932f
            if (!listnum.ContainsKey(ElementColor))
            {
                int getnum = GL.GenLists(1);
                listnum.Add(ElementColor, getnum);
                GL.NewList(getnum, ListMode.Compile);
                GL.Enable(EnableCap.Normalize);
                GL.Enable(EnableCap.PolygonSmooth);
                
                //GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan[_TextureMapName]);
                GL.Begin(BeginMode.Triangles);
                GL.Color3(this.ElementColor);
                for (int i = 0; i < 20; i++)
                {
                    //				drawPatch (&idata[index[i][2]][0],&idata[index[i][1]][0],
                    //						&idata[index[i][0]][0],sphere_quality);
                    drawPatch(idata[index[i][2]], idata[index[i][1]],
                            idata[index[i][0]], sphere_quality);
                }
                GL.End();
                GL.EndList();
            }
            GL.CallList(listnum[ElementColor]);
        }
        // This is recursively subdivides a triangular area (vertices p1,p2,p3) into
        // smaller triangles, and then draws the triangles. All triangle vertices are
        // normalized to a distance of 1.0 from the origin (p1,p2,p3 are assumed
        // to be already normalized). Note this is not super-fast because it draws
        // triangles rather than triangle strips.

        //	static void drawPatch (float p1[3], float p2[3], float p3[3], int level)
        private void drawPatch(float[] p1, float[] p2, float[] p3, int level)
        {
            int i;
            if (level > 0)
            {
                float[] q1 = new float[3], q2 = new float[3], q3 = new float[3];		 // sub-vertices
                for (i = 0; i < 3; i++)
                {
                    q1[i] = 0.5f * (p1[i] + p2[i]);
                    q2[i] = 0.5f * (p2[i] + p3[i]);
                    q3[i] = 0.5f * (p3[i] + p1[i]);
                }
                float length1 = (float)(1.0 / Math.Sqrt(q1[0] * q1[0] + q1[1] * q1[1] + q1[2] * q1[2]));
                float length2 = (float)(1.0 / Math.Sqrt(q2[0] * q2[0] + q2[1] * q2[1] + q2[2] * q2[2]));
                float length3 = (float)(1.0 / Math.Sqrt(q3[0] * q3[0] + q3[1] * q3[1] + q3[2] * q3[2]));
                for (i = 0; i < 3; i++)
                {
                    q1[i] *= length1;
                    q2[i] *= length2;
                    q3[i] *= length3;
                }
                drawPatch(p1, q1, q3, level - 1);
                drawPatch(q1, p2, q2, level - 1);
                drawPatch(q1, q2, q3, level - 1);
                drawPatch(q3, q2, p3, level - 1);
            }
            else
            {
                GL.Color3(this.ElementColor);
                GL.Normal3(p1[0], p1[1], p1[2]);
                GL.Vertex3(p1[0], p1[1], p1[2]);
                GL.Normal3(p2[0], p2[1], p2[2]);
                GL.Vertex3(p2[0], p2[1], p2[2]);
                GL.Normal3(p3[0], p3[1], p3[2]);
                GL.Vertex3(p3[0], p3[1], p3[2]);
            }
        }
        private void setTransform(JVector pos, JMatrix R)
        {
            //GLdouble
            double[] matrix = new double[16];
            matrix[0] = R.M11;
            matrix[1] = R.M21;
            matrix[2] = R.M31;
            matrix[3] = 0;
            matrix[4] = R.M12;
            matrix[5] = R.M22;
            matrix[6] = R.M32;
            matrix[7] = 0;
            matrix[8] = R.M13;
            matrix[9] = R.M23;
            matrix[10] = R.M33;
            matrix[11] = 0;
            matrix[12] = pos.X;
            matrix[13] = pos.Y;
            matrix[14] = pos.Z;
            matrix[15] = 1;
            GL.PushMatrix();
            GL.MultMatrix(matrix);
        }
        public override bool Update(GameRunningState gameState)
        {
           // rb.LinearVelocity = new JVector(Velocity.X,Velocity.Y,Velocity.Z);
            
            //throw new NotImplementedException();
          /*  SphereVertices = GeometryHelper.CalculateSphereVertices((float) Radius, (float) Radius, 100, 100);
            SphereElements = GeometryHelper.CalculateSphereElements((float) Radius, (float) Radius, 100, 100);*/
            return false;
        }

        public void AddToWorld(World Target)
        {
            Target.AddBody(this.PhysicsElement);
        }

        public void RemoveFromWorld(World Target)
        {
            Target.RemoveBody(PhysicsElement);
        }
    }
}
