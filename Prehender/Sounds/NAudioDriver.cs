using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BASeCamp.DataHandling;
using NAudio.Wave;
using NVorbis.NAudioSupport;

namespace Prehender.Sounds
{
    //Prehender uses a similar Sound setup to BASeBlock.
    //in fact I may back-copy this particular implementation to BASeBlock (NAudio) if it works.
    public class NAudioDriver : iSoundEngineDriver
    {
        public event OnSoundStopDelegate OnSoundStop;
        public event OnSoundPlayDelegate OnSoundPlay;
        public WaveMixerStream32 Mixer;
        public IntPtr AssociatedHandle { get; private set; }
        public NAudioDriver(IntPtr WindowHandle)
        {
            Name = "NAudio Driver";
            AssociatedHandle = WindowHandle;
            Mixer = new WaveMixerStream32();
            
            
        }

        public void RaiseStop(NAudioPlaying Stopped)
        {
            var copied = OnSoundStop;
            if (copied != null)
                copied(Stopped);
        }
        public void RaisePlay(NAudioPlaying Played)
        {
            var copied = OnSoundPlay;
            if (copied != null)
                copied(Played);
        }
        public iSoundSourceObject loadSound(string filename)
        {
            return new NAudioSource(this, filename);
        }

        public iSoundSourceObject LoadSound(byte[] data, string sName, string fileextension)
        {
            throw new NotImplementedException();
        }

        public iSoundSourceObject LoadSound(string filename)
        {
            throw new NotImplementedException();
        }

        public string Name { get; private set; }
        IEnumerable<string> iSoundEngineDriver.GetSupportedExtensions()
        {
            return GetSupportedExtensions();
        }

        public string[] GetSupportedExtensions()
        {
            return new string[]{".OGG"};
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class NAudioSource : iSoundSourceObject{
        public NAudioDriver ConnectedDriver { get; private set; }
        public WaveOut outWave { get; private set; }
        public IWaveProvider WaveProvider { get; private set; }
        public String sFileName { get; private set; }
        //private WaveStream Wave { get; set; }
        public NAudioSource(NAudioDriver Driver,String SourceFile)
        {
            sFileName = SourceFile;
            outWave = new WaveOut();
            
            ConnectedDriver = Driver;
        }

        public iActiveSoundObject Play(bool playlooped)
        {
            //throw new NotImplementedException();
            return Play(playlooped, 1.0f);
        }

        public iActiveSoundObject Play(bool playlooped, float volume)
        {
            var result = new NAudioPlaying(this);
            //ConnectedDriver.Mixer.AddInputStream(result.WStream);
            return result;
        }

        public float getLength()
        {
            throw new NotImplementedException();
        }
    }
    public class NAudioPlaying : iActiveSoundObject,IDisposable
    {
        private NAudioSource FromSource;
        public WaveOut Wave{get;private set;}
        public VorbisWaveReader WStream { get; private set; }
        public NAudioPlaying(NAudioSource Source)
        {
            lock (Source)
            {
                FromSource = Source;
                Wave = Source.outWave;
                WStream = new VorbisWaveReader(Source.sFileName);

                Wave.Init(WStream);

                Wave.PlaybackStopped += wave_PlaybackStopped;
                
                Wave.Play();
                //FromSource.ConnectedDriver.Mixer.AddInputStream(WStream);
                FromSource.ConnectedDriver.RaisePlay(this);
            }
        }

        void wave_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            FromSource.ConnectedDriver.RaiseStop(this);
           // FromSource.ConnectedDriver.Mixer.RemoveInputStream(WStream);
            Dispose();
        }

        public bool Finished { get; private set; }
        public float Tempo { get; set; }

        public void Stop()
        {
            Wave.Stop();
        }

        public void Pause()
        {
            Wave.Pause();
        }

        public void UnPause()
        {
            Wave.Play();
        }

        public bool Paused { get { return Wave.PlaybackState == PlaybackState.Paused; } set { if (Paused) Wave.Play(); } }
        public void setVolume(float volumeset)
        {
            throw new NotImplementedException();
        }

        public float Progress { get; private set; }
        public iSoundSourceObject Source { get; private set; }

        public void Dispose()
        {
            Wave.Dispose();
        }
    }

}
