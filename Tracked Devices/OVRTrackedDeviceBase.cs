using System;
using Valve.VR;
using Microsoft.Xna.Framework;

namespace MerjTek.MonoGame.OpenVr
{
    #region OVRTrackedDeviceActivateEventArgs Class

    /// <summary>
    /// even arguments.
    /// </summary>
    public class OVRTrackedDeviceActivateEventArgs : EventArgs
    {
        /// <summary>
        /// The tracked device.
        /// </summary>
        public readonly OVRTrackedDeviceBase TrackedDevice;

        /// <summary>
        /// Initializes a new instance of the OVRTrackedDeviceActivateEventArgs class.
        /// </summary>
        /// <param name="device">The tracked device.</param>
        public OVRTrackedDeviceActivateEventArgs(OVRTrackedDeviceBase device)
        {
            TrackedDevice = device;
        }
    }

    #endregion

    /// <summary>
    /// Base class for OpenVR tracked devices.
    /// </summary>
    public abstract class OVRTrackedDeviceBase
    {
        #region Public Properties

        /// <summary>
        /// The current device's index.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Is the current device connected?
        /// </summary>
        public bool IsConnected => OVRDevice.Get().IsTrackedDeviceConnected(Index);

        #endregion

        #region Construtor

        /// <summary>
        /// Initializes a new instance of the OVRTrackedDeviceBase class.
        /// </summary>
        /// <param name="index">THe index of the device.</param>
        public OVRTrackedDeviceBase(int index)
        {
            Index = index;
        }

        #endregion

        #region Pose

        internal TrackedDevicePose_t GetDevicePose()
        {
            OVRDevice device = OVRDevice.Get();
            return device.GetDevicePose(Index);
        }

        /// <summary>
        /// Get the current pose.
        /// </summary>
        /// <returns>The current pose.</returns>
        public Matrix GetPose()
        {
            return GetDevicePose().mDeviceToAbsoluteTracking.ToMatrix();
        }

        /// <summary>
        /// Get the current pose's velocity.
        /// </summary>
        /// <returns>The current pose's velocity.</returns>
        public Vector3 GetPoseVelocity()
        {
            return GetDevicePose().vVelocity.ToVector3();
        }

        /// <summary>
        /// Get the current pose's angular velocity.
        /// </summary>
        /// <returns>The current pose's angular velocity.</returns>
        public Vector3 GetPoseAngularVelocity()
        {
            return GetDevicePose().vAngularVelocity.ToVector3();
        }

        /// <summary>
        /// Get the current pose's tracking result.
        /// </summary>
        /// <returns>The current pose's tracking result.</returns>
        public ETrackingResult GetTrackingResult()
        {
            return GetDevicePose().eTrackingResult;
        }

        #endregion
        #region Next Pose

        internal TrackedDevicePose_t GetNextDevicePose()
        {
            OVRDevice device = OVRDevice.Get();
            return device.GetNextDevicePose(Index);
        }

        /// <summary>
        /// Get the next pose.
        /// </summary>
        /// <returns>The next pose.</returns>
        public Matrix GetNextPose()
        {
            return GetNextDevicePose().mDeviceToAbsoluteTracking.ToMatrix();
        }

        /// <summary>
        /// Get the next pose's velocity.
        /// </summary>
        /// <returns>The next pose's velocity.</returns>
        public Vector3 GetNextPoseVelocity()
        {
            return GetNextDevicePose().vVelocity.ToVector3();
        }

        /// <summary>
        /// Get the next pose's angular velocity.
        /// </summary>
        /// <returns>The next pose's angular velocity.</returns>
        public Vector3 GetNextPoseAngularVelocity()
        {
            return GetNextDevicePose().vAngularVelocity.ToVector3();
        }

        /// <summary>
        /// Get the next pose's tracking result.
        /// </summary>
        /// <returns>The next pose's tracking result.</returns>
        public ETrackingResult GetNextPoseTrackingResult()
        {
            return GetNextDevicePose().eTrackingResult;
        }

        #endregion
    }
}
