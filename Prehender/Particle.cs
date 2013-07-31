using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter;
using OpenTK;
using OpenTK.Graphics;
using BeginMode = OpenTK.Graphics.OpenGL.BeginMode;
using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;
using GL = OpenTK.Graphics.OpenGL.GL;
using TextureEnvParameter = OpenTK.Graphics.OpenGL.TextureEnvParameter;
using TextureEnvTarget = OpenTK.Graphics.OpenGL.TextureEnvTarget;
using TextureTarget = OpenTK.Graphics.OpenGL.TextureTarget;

namespace Prehender
{
    public class Particle : GameObject ,ILocatable,IMovingObject 
    {
        public Vector3 Location { get; set; }
        public Vector3 Velocity { get; set; }
        private Vector3 _FallOff = new Vector3(0.99f, 0.99f, 0.99f);
        private TimeSpan _TTL = new TimeSpan(0, 0, 0, 5);
        private DateTime _BirthTime = DateTime.Now;
        private float _Size = 3;
        private String _TextureKey="";
        public String TextureKey { get { return _TextureKey; } set { _TextureKey = value; } }
        public float Size { get { return _Size; } set { _Size = value; } }
        private Color _PointColor = Color.Red;
        public Color PointColor { get { return _PointColor; } set { _PointColor = value; } }
        public Vector3 FallOff { get { return _FallOff; } set { _FallOff = value; } }
        public TimeSpan TTL { get { return _TTL; } set { _TTL = value; } }
        public DateTime Birth { get { return _BirthTime; } set { _BirthTime = value; } }
        public Particle(Vector3 pLocation, Vector3 pVelocity, Vector3 pFalloff, TimeSpan pTTL,Color pColor)
        {
            Location = pLocation;
            Velocity = pVelocity;
            _FallOff = pFalloff;
            _TTL = pTTL;
            _PointColor = pColor;

        }
        public Particle(Vector3 pLocation, Vector3 pVelocity)
        {
            Location = pLocation;
            Velocity = pVelocity;
           
            
        }
        public static void RenderPoints(IEnumerable<Particle> Particles ){

            Color4? CurrColor = null;
            float? CurrSize = null;
            String CurrTextureKey = null;
            var result = from p in Particles orderby p.PointColor.ToArgb(), p.Size,p.TextureKey select p;
            GL.Enable(EnableCap.PointSprite);
            GL.Begin(BeginMode.Points);
            foreach (var iterate in result)
            {
                if (iterate.PointColor != CurrColor || iterate.Size != CurrSize || iterate.TextureKey != CurrTextureKey)
                {
                    GL.End();
                    CurrColor = iterate.PointColor;
                    CurrSize = iterate.Size;
                    CurrTextureKey = iterate.TextureKey;
                    
                    //GL.Color4(CurrColor.Value);
                    GL.BindTexture(TextureTarget.Texture2D, PrehenderGame.instance.TextureMan[iterate.TextureKey]);
                    GL.StencilMask(PrehenderGame.instance.TextureMan[iterate.TextureKey]);
                    
                    GL.Enable(EnableCap.PointSprite);
                    GL.TexEnv(TextureEnvTarget.PointSprite, TextureEnvParameter.CoordReplace,1 );
                    GL.PointSize(CurrSize.Value);
                    GL.Begin(BeginMode.Points);
                    
                }

                GL.Vertex3(iterate.Location);



            }
            GL.End();
            

        }
        public static void UpdatePoints(IEnumerable<Particle> Particles,GameRunningState grs,Action<Particle> RemoveAction){

            List<Particle> removeElements = new List<Particle>();
            foreach (var iterate in Particles)
            {
                if (iterate.Update(grs))
                {
                    RemoveAction(iterate);
                }
            }


        
        }
        public override void Render(GameRunningState gameState)
        {
            //unused.
        }

        public override bool Update(GameRunningState gameState)
        {
            Location += Velocity;
            Velocity = Vector3.Multiply(Velocity, _FallOff);
            if (DateTime.Now - Birth > TTL)
            {
                return true;
            }
            return false;
        }

        
    }
}
