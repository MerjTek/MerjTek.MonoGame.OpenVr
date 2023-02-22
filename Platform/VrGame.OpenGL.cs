using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MerjTek.MonoGame.OpenVr
{
    /// <summary>
    /// Class for the OpenVR devie.
    /// </summary>
    public partial class VrGame : Game
    {
        /// <summary>
        /// Sets up the fieldInfo, leftPtr, and rightPtr for the platform (OpenGL)
        /// </summary>
        private void SetPlatformTextures()
        {
            var fieldInfo = typeof(Texture2D).GetField("glTexture", BindingFlags.Instance | BindingFlags.NonPublic);
            leftPtr = new IntPtr((int)fieldInfo.GetValue(leftEyeTarget));
            rightPtr = new IntPtr((int)fieldInfo.GetValue(rightEyeTarget));
        }
    }
}
