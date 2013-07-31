using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NVorbis.OpenTKSupport;
using OpenTK.Audio.OpenAL;

namespace Prehender.Sounds
{
    public class SoundItem
    {
        
        OggStream useStream = null;
        public SoundItem(String sourcefile)
        {
        
            OggStream ostream = new OggStream(sourcefile);
            ostream.Prepare();
            useStream = ostream;
            //AL.BufferData(_Buffer,ostream.g)
                
            
            
            
        }
        public void Play()
        {
            useStream.Play();
        }
    }
}
