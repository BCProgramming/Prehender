using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BCFileSearch;
using OpenTK.Graphics.OpenGL;

namespace Prehender
{
    public class TextureManager
    {
        protected Dictionary<String, int> TextureMapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase); 
        //OpenGL textures are indexed as integers
        

        //special logic for  textures that need to change. This is similar to LoadTexture, but we are
        //changing an existing Texture binding to point to new data.

        static int CreateTexture(Bitmap source)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            BitmapData bmp_data = source.LockBits(new Rectangle(0,0,source.Width,source.Height),ImageLockMode.ReadOnly,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat.Rgba,bmp_data.Width,bmp_data.Height,0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,PixelType.UnsignedByte,bmp_data.Scan0);

            source.UnlockBits(bmp_data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            return id;
        }

        //we load png images into textures, and index them in a hashmap, and expose that also through an indexer.
        static int LoadTexture(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            int id = GL.GenTexture();
            
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(filename);
            /*
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return id;*/
            return CreateTexture(bmp);
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        private int AddTextureFile(String sPath)
        {
            int TextureID =  LoadTexture(sPath);
            //add to our Dictionary.
            String basename = Path.GetFileNameWithoutExtension(sPath);
            TextureMapping.Add(basename, TextureID);
            return TextureID;
        }

        readonly List<FileFinder> ActiveTextureSearches = new List<FileFinder>(); 



        public TextureManager(IEnumerable<DirectoryInfo> SourceFolders)
        {
            //create a Searcher for each directory.
            foreach (var iterate in SourceFolders)
            {
                
                foreach(var iteratePic in FileFinder.Enumerate(iterate.FullName,"*.png",null,true)){
                    AddTextureFile(iteratePic.FullPath);
                }
                
                

            }
        }
        public void WaitonSearchComplete()
        {
            while (!ActiveTextureSearches.All((w) => w.Finished)) { Thread.Sleep(0); }
        }
        public int this[String indexer]
        {
            get {
                return TextureMapping[indexer];
            }
            
        }
    }
}
