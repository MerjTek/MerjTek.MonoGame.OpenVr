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
        /// Sets up the fieldInfo, leftPtr, and rightPtr for the platform (DirectX)
        /// </summary>
        private void SetPlatformTextures()
        {
            var fieldInfo = typeof(Texture).GetField("_texture", BindingFlags.Instance | BindingFlags.NonPublic);

#pragma warning disable CS8602 // Dereference of a possibly null reference.

            var lHandle = fieldInfo.GetValue(leftEyeTarget) as SharpDX.Direct3D11.Resource;
            leftPtr = lHandle.NativePointer;
            
            var rHandle = fieldInfo.GetValue(rightEyeTarget) as SharpDX.Direct3D11.Resource;
            rightPtr = rHandle.NativePointer;

#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}
