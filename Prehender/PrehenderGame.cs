using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BASeCamp.DataHandling;
using BASeCamp.Utilities;
using OpenTK.Platform;
using Prehender.HUD;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints;
using Jitter.LinearMath;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Prehender.Sounds;
using XRamExtension = OpenTK.Audio.OpenAL.XRamExtension;

namespace Prehender
{
    public class PrehenderGame : GameWindow
    {
        protected ConcurrentQueue<NextUpdateCall> NextUpdateCalls = new ConcurrentQueue<NextUpdateCall>();  
        float angle;
        public static PrehenderGame  instance = null;

        public TextureManager TextureMan = null;
        //public SoundManager SoundMan = null;
        public cNewSoundManager SoundMan = null;
        public GameState currentState = null;
        public void Defer(Action a)
        {
            
            NextUpdateCalls.Enqueue(new NextUpdateCall(a));
        }
        public PrehenderGame()
        {
            if(instance==null) instance = this;
        }

        public static String GetTempPath()
        {
            String tpath = Path.GetTempPath();
            tpath = Path.Combine(tpath, "Prehender");
            if (!Directory.Exists(tpath)) Directory.CreateDirectory(tpath);
            return tpath;

        }
        public static String GetTempFile(String useextension)
        {
            String tpath = GetTempPath();
            if (!useextension.StartsWith(".")) useextension = "." + useextension;
            //GetTempPath(1023,tpath);
            tpath = tpath.Replace('\0', ' ').Trim();
            String destfilename = Guid.NewGuid().ToString() + useextension;
            return Path.Combine(tpath, destfilename);





        }
        public static Random rgen = new Random();
        public int listnum;
        public BlockObject DeathBlock = null;
        AudioContext ac;
        XRamExtension xram;
        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);

           // ac = new AudioContext();
           // xram = new XRamExtension();
SoundMan= new cNewSoundManager(new BASSDriver(), new []{new DirectoryInfo(GetSoundFolder())});

            GL.ClearColor(Color.Gray);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            currentState = new GameRunningState(this);
            
           

            TextureMan = new TextureManager(new DirectoryInfo[] { new DirectoryInfo(GetImageFolder()) });
           // SoundMan = new SoundManager(hnd,new DirectoryInfo[] { new DirectoryInfo(GetSoundFolder()) });
            
           
            //BlockTexture = LoadTexture(@"D:\testtexture.png");
            //BlockTexture2 = LoadTexture(@"D:\testtexture2.png");
            TextureMan.WaitonSearchComplete();
        }

        

       

        
        public String GetImageFolder()
        {
            return Path.Combine(GetAppDataFolder(), "Images");
        }
        public String GetSoundFolder()
        {
            return Path.Combine(GetAppDataFolder(), "Sounds");
        }
        public String GetAppDataFolder()
        {
            var AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(AppData, @"Prehender");
        }
        
        float accTime = 0.0f;
        private String title = "Prehender";


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            NextUpdateCall nupdate = null;
            while(NextUpdateCalls.TryDequeue(out nupdate))
            
            {
               if (nupdate!=null && !(nupdate.Invoke(this)))
                    NextUpdateCalls.Enqueue(nupdate);
            }

            accTime += 1.0f / (float)RenderFrequency;

            if (accTime > 1.0f)
            {
                this.Title = title + " " + RenderFrequency.ToString("##.#") + " fps";
                accTime = 0.0f;
            }

            if (currentState != null) currentState.Update(this, e);

            if (Keyboard[OpenTK.Input.Key.Escape])
            {
                this.Exit();
                return;
            }
            
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

           
            //cam.YRotation += rotation_speed * (float)e.Time;
            
           /* Matrix4 lookat = Matrix4.LookAt(0, 80, 80, 0, 0, 0, 0, 1, 0);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);
            
            angle += rotation_speed * (float)e.Time;
            GL.Rotate(angle, 0.0f, 1.0f, 0.0f);
             */
            if (currentState != null)
            {
                currentState.Render(this, e);
                toOrthogonal();
                currentState.RenderHUD(this,e);
                BacktoPerspective();
            }
            this.SwapBuffers();
            Thread.Sleep(1);
        }

        protected void BacktoPerspective()
        {

            GL.Enable(EnableCap.DepthTest);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);

        }

        protected void toOrthogonal()
        {
            //we don't need depth testing for orthogonal mode.
            GL.Disable(EnableCap.DepthTest);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Ortho(0, Size.Width, 0, Size.Height, -5, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }
    }
}
