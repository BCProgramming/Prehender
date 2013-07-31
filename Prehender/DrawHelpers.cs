using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Prehender
{

    public static class ColorMatrices
    {

        private static float[][] _colorMatrixElements = { 
   new float[] {1,  0,  0,  0, 0},        // red scaling factor of 2
   new float[] {0,  1,  0,  0, 0},        // green scaling factor of 1
   new float[] {0,  0,  2,  0, 0},        // blue scaling factor of 1
   new float[] {0,  0,  0,  1, 0},        // alpha scaling factor of 1
   new float[] {-0.5f, -0.5f, .8f, 0, 1}};    // three translations of 0.2

        public static ColorMatrix GetFader(float alpha)
        {

            return new ColorMatrix(new float[][] {
                new float[]{1,0,0,0,0},
                new float[]{0,1,0,0,0},
                new float[]{0,0,1,0,0},
                new float[]{0,0,0,alpha,0},
                new float[]{0,0,0,0,1}});



        }

        public static ColorMatrix GetColourizer(Color fromcolor)
        {
            return GetColourizer(fromcolor.R, fromcolor.G, fromcolor.B, fromcolor.A);



        }
        public static ColorMatrix GetColourizer(float red, float green, float blue)
        {
            return GetColourizer(red, green, blue, 1);

        }
        public static void AddColourizer(ImageAttributes toia, Color usecolor)
        {
            ColorMatrix grayscaler = GrayScale();
            toia.SetColorMatrices(GetColourizer(usecolor), grayscaler);



        }


        public static ColorMatrix GrayScale()
        {
            return new ColorMatrix(
      new float[][]
      {
         new float[] {.3f, .3f, .3f, 0, 0},
         new float[] {.59f, .59f, .59f, 0, 0},
         new float[] {.11f, .11f, .11f, 0, 0},
         new float[] {0, 0, 0, 1, 0},
         new float[] {0, 0, 0, 0, 1}
      });

        }
        public static float[][] GetIdentity()
        {
            return new float[][]
      {
         new float[] {1, 0, 0, 0, 0},
         new float[] {0, 1, 0, 0, 0},
         new float[] {0, 0, 1, 0, 0},
         new float[] {0, 0, 0, 1, 0},
         new float[] {0, 0, 0, 0, 1}
      };

        }
        public static ColorMatrix GetRedColourizer(float red, float green, float blue, float alpha)
        {
            float[][] matElement =  { 
   new float[] {red,  0,  0,  0, 0},        //red scaling factor
   new float[] {0,  green,  0,  0, 0},        // green scaling factor of 1
   new float[] {0,  0,  blue,  0, 0},        // blue scaling factor of 1
   new float[] {0,  0,  0,  alpha, 0},        // alpha scaling factor of 1
   new float[] {0, 0, 0, 0, 1}};    // three translations of 0.2
            //change the appropriate elements to match....
            matElement[0][0] = red;
            matElement[1][1] = green;
            matElement[2][2] = blue;
            matElement[3][3] = alpha;
            matElement[4][4] = 1;
            return new ColorMatrix(matElement);


        }

        public static ColorMatrix GetColourizer(float red, float green, float blue, float alpha)
        {





            /*
            float[][] MatElement =  { 
   new float[] {red,  0,  0,  0, 0},        //red scaling factor
   new float[] {0,  green,  0,  0, 0},        // green scaling factor of 1
   new float[] {0,  0,  blue,  0, 0},        // blue scaling factor of 1
   new float[] {0,  0,  0,  alpha, 0},        // alpha scaling factor of 1
   new float[] {-0.5f, -0.5f, .8f, 0, alpha}};    // three translations of 0.2
             */
            float[][] matElement =  { 
   new float[] {red,  0,  0,  0, 0},        //red scaling factor
   new float[] {0,  green,  0,  0, 0},        // green scaling factor of 1
   new float[] {0,  0,  blue,  0, 0},        // blue scaling factor of 1
   new float[] {0,  0,  0,  alpha, 0},        // alpha scaling factor of 1
   new float[] {0, 0, 0, 0, 1}};    // three translations of 0.2
            //change the appropriate elements to match....
            matElement[0][0] = red;
            matElement[1][1] = green;
            matElement[2][2] = blue;
            matElement[3][3] = alpha;
            matElement[4][4] = 1;
            return new ColorMatrix(matElement);

        }


    }
    #region HSLColor
    /// <summary>
    /// Class used to convert to and from Hue,Saturation, and Luminousity.
    /// </summary>
    public class HSLColor
    {
        // Private data members below are on scale 0-1
        // They are scaled for use externally based on scale
        private double _hue = 1.0;
        private double _saturation = 1.0;
        private double _luminosity = 1.0;

        private const double Scale = 240.0;

        public double Hue
        {
            get { return _hue * Scale; }
            set { _hue = CheckRange(value / Scale); }
        }
        public double Saturation
        {
            get { return _saturation * Scale; }
            set { _saturation = CheckRange(value / Scale); }
        }
        public double Luminosity
        {
            get { return _luminosity * Scale; }
            set { _luminosity = CheckRange(value / Scale); }
        }

        private double CheckRange(double value)
        {
            if (value < 0.0)
                value = 0.0;
            else if (value > 1.0)
                value = 1.0;
            return value;
        }

        public override string ToString()
        {
            return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}", Hue, Saturation, Luminosity);
        }

        public string ToRGBString()
        {
            Color color = (Color)this;
            return String.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B);
        }

        #region Casts to/from System.Drawing.Color
        public static implicit operator Color(HSLColor hslColor)
        {
            double r = 0, g = 0, b = 0;
            if (hslColor._luminosity != 0)
            {
                if (hslColor._saturation == 0)
                    r = g = b = hslColor._luminosity;
                else
                {
                    double temp2 = GetTemp2(hslColor);
                    double temp1 = 2.0 * hslColor._luminosity - temp2;

                    r = GetColorComponent(temp1, temp2, hslColor._hue + 1.0 / 3.0);
                    g = GetColorComponent(temp1, temp2, hslColor._hue);
                    b = GetColorComponent(temp1, temp2, hslColor._hue - 1.0 / 3.0);
                }
            }
            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        private static double GetColorComponent(double temp1, double temp2, double temp3)
        {
            temp3 = MoveIntoRange(temp3);
            if (temp3 < 1.0 / 6.0)
                return temp1 + (temp2 - temp1) * 6.0 * temp3;
            else if (temp3 < 0.5)
                return temp2;
            else if (temp3 < 2.0 / 3.0)
                return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
            else
                return temp1;
        }
        private static double MoveIntoRange(double temp3)
        {
            if (temp3 < 0.0)
                temp3 += 1.0;
            else if (temp3 > 1.0)
                temp3 -= 1.0;
            return temp3;
        }
        private static double GetTemp2(HSLColor hslColor)
        {
            double temp2;
            if (hslColor._luminosity < 0.5)  //<=??
                temp2 = hslColor._luminosity * (1.0 + hslColor._saturation);
            else
                temp2 = hslColor._luminosity + hslColor._saturation - (hslColor._luminosity * hslColor._saturation);
            return temp2;
        }

        public static implicit operator HSLColor(Color color)
        {
            HSLColor hslColor = new HSLColor();
            hslColor._hue = color.GetHue() / 360.0; // we store hue as 0-1 as opposed to 0-360 
            hslColor._luminosity = color.GetBrightness();
            hslColor._saturation = color.GetSaturation();
            return hslColor;
        }
        #endregion

        public void SetRGB(int red, int green, int blue)
        {
            HSLColor hslColor = (HSLColor)Color.FromArgb(red, green, blue);
            this._hue = hslColor._hue;
            this._saturation = hslColor._saturation;
            this._luminosity = hslColor._luminosity;
        }

        public HSLColor() { }
        public HSLColor(Color color)
        {
            SetRGB(color.R, color.G, color.B);
        }
        public HSLColor(int red, int green, int blue)
        {
            SetRGB(red, green, blue);
        }
        public HSLColor(double hue, double saturation, double luminosity)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Luminosity = luminosity;
        }


        public static Color RandomHue(double useSat, double uselum)
        {
            return new HSLColor(new Random().NextDouble() * 240, useSat, uselum);
        }
    }
    #endregion
}
