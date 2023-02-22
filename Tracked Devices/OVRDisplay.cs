//using System;

namespace MerjTek.MonoGame.OpenVr
{
    /// <summary>
    /// Class for tracking an OpenVR display.
    /// </summary>
    public class OVRDisplay : OVRTrackedDeviceBase
    {
        /// <summary>
        /// Initializes a new instance of the OpenVRDisplay class.
        /// </summary>
        /// <param name="index">The index of the device.</param>
        public OVRDisplay(int index) : 
            base(index)
        { 
        }
    }
}
