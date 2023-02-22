using System;

namespace MerjTek.MonoGame.OpenVr
{
    #region OVRInitializationFailedException

    ///
    /// The exception that is thrown when the OpenVR initialization has failed.
    /// 
    public class OVRInitializationFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the OVRInitializationFailedException class.
        /// </summary>
        public OVRInitializationFailedException(string message) : 
            base(string.Format("OpenVR.Init failed! Message: {0}.", message))
        {
        }
    }

    #endregion
    #region OVRRuntimeNotInstalledException

    /// <summary>
    /// The exception that is thrown when the OpenVR runtime is not installed.
    /// </summary>
    public class OVRRuntimeNotInstalledException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the OVRRuntimeNotInstalledException class.
        /// </summary>
        public OVRRuntimeNotInstalledException(string errorMessage) : 
            base(errorMessage)
        {
        }
    }

    #endregion
    #region OVRDisplayNotConnectedException

    /// <summary>
    /// The exception that is thrown when an OpenVR display is not connected.
    /// </summary>
    public class OVRDisplayNotConnectedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the OVRDisplayNotConnectedException class.
        /// </summary>
        public OVRDisplayNotConnectedException() : 
            base("The OpenVR Display is not present!")
        {
        }
    }

    #endregion
    #region OVRDisplayNotConnectedException

    /// <summary>
    /// The exception that is thrown when an OVR controller states cannot be retrieved.
    /// </summary>
    public class OVRGetControllerStateFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the OpenVRGetControllerStateFailedException class.
        /// </summary>
        public OVRGetControllerStateFailedException(OVRController.ControllerHandedness handedness) : 
            base(string.Format("Getting the OpenVR controller state failed! ({0})", handedness.ToString()))
        {
        }
    }

    #endregion  
}
