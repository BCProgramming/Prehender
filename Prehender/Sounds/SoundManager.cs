using System;
using System.Collections.Generic;
using System.IO;
using BCFileSearch;
using NAudio.Wave;
using NVorbis.OpenTKSupport;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using XRamExtension = OpenTK.Audio.OpenAL.XRamExtension;


namespace Prehender.Sounds
{
    public class SoundManager
    {
        protected Dictionary<string, SoundItem> Sounds = new Dictionary<string, SoundItem>();
        private OggStreamer ostreamer;
        IWaveProvider Core = null;
        public SoundManager(IntPtr Handle,IEnumerable<DirectoryInfo> DirSource)
        {
            
            //Core = new WaveOut(Handle);
            

            
            foreach (var iterate in DirSource)
            {

                FileFinder ff = new FileFinder(iterate.FullName, "*.ogg", (a)=>!a.Attributes.HasFlag(FileAttributes.Directory), (a) => LoadSound(a.FullPath), true);
                ff.Start();
                
            }
            


        
        }
        public SoundItem this[string indexer]
        {
            get
            {
                return Sounds[indexer];
            }
        }
        private void LoadSound(string sPath)
        {

            

            SoundItem makeitem = new SoundItem(sPath);
            string sname = Path.GetFileNameWithoutExtension(sPath);
            Sounds.Add(sname, makeitem);
            

        }

    }
}
