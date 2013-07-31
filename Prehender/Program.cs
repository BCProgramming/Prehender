using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prehender.HUD;

namespace Prehender
{
    public static class Program
    {
        [STAThread]
        public static void Main(String[] args)
        {

            SplashScreen.SplashScreenInfo ssi = new SplashScreen.SplashScreenInfo(
                null,null,null,"© 2013 BASeCamp Corporation","All Rights Reserved","Licensed To:" + Environment.NewLine + "Michael Burgwin",
                LoadAction, () => { new Thread(AfterInvocation).Start(); });

            SplashScreen ss = new SplashScreen(ssi);
            ss.ShowDialog();


        }
        private static void LoadAction(ISplashScreenCallback callback)
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(100);
                callback.setProgress((float)i / (float)100);
            }
            callback.setProgressMessage("Load Complete. Initializing Program...");
            Thread.Sleep(500);
        }
        private static void AfterInvocation(Object state){
            new PrehenderGame().Run(60, 60);
        }

    }
}
