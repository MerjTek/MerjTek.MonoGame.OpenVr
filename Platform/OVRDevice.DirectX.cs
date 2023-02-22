using Valve.VR;

namespace MerjTek.MonoGame.OpenVr
{
    /// <summary>
    /// Class for the OpenVR devie.
    /// </summary>
    public partial class OVRDevice
    {
        /// <summary>
        /// Sets up the textureBounds and texture for the platform (DirectX)
        /// </summary>
        private void SetPlatformTextureInfo()
        {
            textureBounds = new VRTextureBounds_t()
            {
                uMin = 0,
                uMax = 1,
                vMin = 0,
                vMax = 1
            };

            texture = new Texture_t()
            {
                eType = ETextureType.DirectX,
                eColorSpace = EColorSpace.Gamma
            };
        }
    }
}
