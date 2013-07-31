using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prehender.HUD
{
    public partial class SplashScreen : Form,ISplashScreenCallback
    {
        //public but internal data class
        //used in constructor for creating a SplashScreen and segway-ing to a given target form.

        public class SplashScreenInfo 
        {
            private String _TitleText = "Title";
            private Image _TitleIcon = null;
            private String _VersionText = "Version 1.0";
            private String _CopyrightLine1 = "";
            private String _CopyrightLine2 = "";
            private String _LicenseText;
            //SplashScreen is passed a Loading lambda and a completion lambda.
            private Action<ISplashScreenCallback> _LoadAction = null;
            private Action _CompletionAction = null;
                
            public String TitleText { get { return _TitleText; } set { _TitleText = value; } }
            public Image TitleIcon { get { return _TitleIcon; } set { _TitleIcon = value; } }
            public String VersionText { get { return _VersionText;}  set {_VersionText=value;} }
            public String CopyrightLine1 { get { return _CopyrightLine1; } set { _CopyrightLine1 = value; } }
            public String CopyrightLine2 { get { return _CopyrightLine2; } set { _CopyrightLine2 = value; } }
            public String LicenseText { get { return _LicenseText; } set { _LicenseText = value; } }
            public Action<ISplashScreenCallback> LoadAction { get { return _LoadAction; } set { _LoadAction = value; } }
            public Action CompletionAction { get { return _CompletionAction; } set { _CompletionAction = value; } }
            public SplashScreenInfo(String pTitleText,Image pTitleIcon, String pVersion,String pCopyrightLine1,String pCopyrightLine2,
                String pLicenseText, Action<ISplashScreenCallback> pLoadAction, Action pCompletion)
            {
                _TitleText = pTitleText;
                _TitleIcon = pTitleIcon;
                _VersionText = pVersion;
                _CopyrightLine1 = pCopyrightLine1;
                _CopyrightLine2 = pCopyrightLine2;
                _LicenseText = pLicenseText;
                LoadAction = pLoadAction;
                CompletionAction = pCompletion;

                //when certain values are null, we inspect the calling assembly.
                
                if (_TitleText == null || _VersionText==null)
                {
                    Assembly calledFrom = Assembly.GetCallingAssembly();
                    var gotname = calledFrom.GetName();
                    if(_VersionText==null) _VersionText = gotname.Version.ToString();
                    if(_TitleText==null) _TitleText = gotname.Name;
                    
                }

            }
            



        }

        private SplashScreenInfo _info;

        //Win8 UI style Splash Screen.
        
        public SplashScreen(SplashScreenInfo info):this()
        {
            _info = info;   
        }

        protected SplashScreen()
        {
            InitializeComponent();
        }
        private System.Threading.Timer DelayShowTimer = null;
        
        private void LoaderCall(Object state)
        {
            //run on a separate thread...
            new Thread(() =>
            {
                if(_info.LoadAction !=null) _info.LoadAction(this);
                if(_info.CompletionAction!=null) _info.CompletionAction();
                Invoke((MethodInvoker)(Close));
            }).Start();
            


        }
        private void SplashScreen_Load(object sender, EventArgs e)
        {
            lblTitle.Text = _info.TitleText;
            lblVersion.Text = _info.VersionText;
            lblCopyright.Text = _info.CopyrightLine1;
            lblCopyright2.Text = _info.CopyrightLine2;
            lblLicense.Text = _info.LicenseText;
            if (_info.TitleIcon != null) this.laneicon.BackgroundImage = _info.TitleIcon;

            //delay a few moments, then run the loader on a separate thread.
            DelayShowTimer = new System.Threading.Timer(LoaderCall, null, new TimeSpan(0, 0, 0, 3), new TimeSpan(0, 0, 0, 0, -1));


        }
        private float ProgressPercentage = 0;
        private String ProgressMessage = "Loading";
        public void setProgress(float Percentage)
        {
            ProgressPercentage = Percentage;
            picProgress.Invoke((MethodInvoker)(() => { picProgress.Invalidate(); picProgress.Update(); }));
        }
        public void setProgressMessage(String pMessage)
        {
            ProgressMessage = pMessage;
            picProgress.Invoke((MethodInvoker)(() => { picProgress.Invalidate(); picProgress.Update(); }));
            
        }

        private static readonly Brush darkColorBrush = new SolidBrush(Color.FromArgb(37, 37, 39));
        private void picProgress_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Transparent);
            var percentageSize = (int) (picProgress.Size.Width * ProgressPercentage);
            e.Graphics.FillRectangle(darkColorBrush, 0, 0, percentageSize, picProgress.Size.Height);
            String pText = String.Format("{0} - {1:00.0%}", ProgressMessage, ProgressPercentage);
            //Measure that progress string.
            var Measured = e.Graphics.MeasureString(pText, picProgress.Font);
            var CenterPoint = new Point(picProgress.Size.Width/2,picProgress.Size.Height/2);
            //choose upper left corner for placement.
            Point usePosition = new Point((int) (CenterPoint.X - Measured.Width / 2), (int) (CenterPoint.Y -Measured.Height / 2));
            e.Graphics.DrawString(pText, picProgress.Font, Brushes.White, usePosition.X, usePosition.Y);
        }
    }
    public interface ISplashScreenCallback
    {
        void setProgress(float Percentage);
        void setProgressMessage(String str);
   
    }



}
