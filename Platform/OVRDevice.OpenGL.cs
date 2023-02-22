using Valve.VR;

namespace MerjTek.MonoGame.OpenVr
{
    /// <summary>
    /// Class for the OpenVR devie.
    /// </summary>
    public partial class OVRDevice
    {
        /// <summary>
        /// Sets up the textureBounds and texture for the platform (OpenGL)
        /// </summary>
        private void SetPlatformTextureInfo()
        {
            textureBounds = new VRTextureBounds_t()
            {
                uMin = 0,
                uMax = 1,
                vMin = 1,
                vMax = 0
            };

            texture = new Texture_t()
            {
                eType = ETextureType.OpenGL,
                eColorSpace = EColorSpace.Auto
            };
        }
    }
}
