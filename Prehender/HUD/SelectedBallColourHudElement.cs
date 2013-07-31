using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace Prehender.HUD
{
    class SelectedBallColourHudElement : HUDElement
    {
        private Color[] Chooseables = new Color[] { Color.Red,Color.Yellow,Color.Green,Color.Blue};
        private int Chosen = 0;

        public Color BallColor { get { return Chooseables[Chosen]; } }
        public void     AdvanceColor(){
            Chosen = (Chosen + 1) % Chooseables.Length;
            Debug.Print("Color set to " + BallColor.ToString());
        }
        public void DecreaseColor()
        {

            Chosen = (Chosen + Chooseables.Length-1) % Chooseables.Length;
            Debug.Print("Color set to " + BallColor.ToString());
        }
        public override void Render(GameRunningState gameState)
        {
            
            
            //GL.LineWidth(5);
            //GL.Viewport(0, 0, gameState.Width, gameState.Height);
            GL.Begin(BeginMode.Quads);
            GL.Color3(BallColor);
            Vector3[] usearray = new Vector3[] { new Vector3(0, 0, 0), new Vector3(10, 0, 0), new Vector3(10, 10, 0), new Vector3(0, 10, 0) };
            
            foreach(var iterate in usearray){
         
                GL.Vertex3(iterate);
            }
            
            GL.End();
        }

        public override bool Update(GameRunningState gameState)
        {
            return false;
          //  throw new NotImplementedException();
        }
    }
}
