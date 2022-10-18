using OpenTK.Graphics.ES20;

namespace TizenGameEngine.Renderer.Common
{
    public static class TextureHelper
    {
        public static int mTexture = 0;
        /// <summary>
        /// Load texture image file
        /// </summary>
        /// <param name="imageName">image path</param>
        /// <returns>texture Id</returns>
        public static int CreateTexture2D(string imageName)
        {
            int[] textureId = { 0 };
            MBitmap bitm = MImageUtil.LoadImage(imageName);
            byte[] pixels = bitm.byteBuffer;
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);
            // Generate a texture object
            GL.GenTextures(1, textureId);
            // Bind the texture object
            GL.BindTexture(TextureTarget.Texture2D, textureId[0]);
            // Load the texture
            GL.TexImage2D(TextureTarget2d.Texture2D, 0, TextureComponentCount.Rgb, (int)bitm.inforHeader.biWidth, (int)bitm.inforHeader.biHeight, 0, PixelFormat.Rgb, PixelType.UnsignedByte, pixels);

            // Set the filtering mode
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)All.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)All.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)All.Linear);
            GL.GenerateMipmap(TextureTarget.Texture2D);
            return textureId[0];
        }
    }
}
