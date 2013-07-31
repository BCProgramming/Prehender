using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Prehender 
{
    public class GameBlock :BlockObject,IColoredElement 
    {
        //red,yellow,green,blue are the current colors.
        private Color[] selectableColours = new Color[] { Color.Red, Color.Yellow, Color.Green, Color.Blue };
        
        private Dictionary<Color,String> selectableTextures = new Dictionary<Color,String>()
        {{Color.Red,"redblock"},
            {Color.Yellow,"yellowblock"},
            {Color.Green,"greenblock"},{Color.Blue,"blueblock"}};
        private Color _ElementColor = Color.Red;
        public Color ElementColor { get { return _ElementColor; } set { _ElementColor = value; } }
        public GameBlock(PrehenderGame bbg, Vector3 pLocation, Vector3 pSize, Vector3 pVelocity, Color useColor)
            : this(bbg, pLocation, pSize, pVelocity)
        {
            _ElementColor = useColor;
            var copied = FaceTextures.Keys.ToList();
            foreach (var iterate in copied)
            {
                FaceTextures[iterate] = selectableTextures[_ElementColor];
            }
        }
        public GameBlock(PrehenderGame bbg, Vector3 pLocation, Vector3 pSize, Vector3 pVelocity) : base(bbg, pLocation, pSize, pVelocity)
        {
            
        }
        
        public void BallImpact(Ball ballhit)
        {
            if (ballhit is IColoredElement)
            {
                if ((ballhit as IColoredElement).ElementColor == _ElementColor)
                {
                    BlockBody.IsStatic = false;
                    BlockBody.AffectedByGravity = true;
                }
            }
        }
        
    }
}
