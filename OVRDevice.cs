using System;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Valve.VR;

namespace MerjTek.MonoGame.OpenVr
{
    /// <summary>
    /// Class for the OpenVR devie.
    /// </summary>
    public partial class OVRDevice
    {
        #region Singleton Pattern

        static private readonly OVRDevice singleton = new();

        /// <summary>
        /// Get the device singleton
        /// </summary>
        /// <returns>The device singleton object.</returns>
        static public OVRDevice Get()
        {
            return singleton;
        }

        #endregion
        #region Private Variables

        private CVRSystem cVrSystem;
        private readonly OVRTrackedDeviceBase[] trackedDevices;
        private readonly TrackedDevicePose_t[] renderPoseArray;
        private readonly TrackedDevicePose_t[] gamePoseArray;
        private readonly TrackedDevicePose_t[] ValidDevicePoses;
        private readonly TrackedDevicePose_t[] ValidNextDevicePoses;
        private VRTextureBounds_t textureBounds;
        private Texture_t texture;

        #endregion
        #region Public Properties

        /// <summary>
        /// The TrackedDevice Activation event handler.
        /// </summary>
        public event EventHandler<OVRTrackedDeviceActivateEventArgs> DeviceActivated;

        /// <summary>
        /// The TrackedDevice Deactivation event handler.
        /// </summary>
        public event EventHandler<OVRTrackedDeviceActivateEventArgs> DeviceDeactivated;

        /// <summary>
        /// The Display.
        /// </summary>
        public OVRDisplay Display => (OVRDisplay)trackedDevices[0];

        /// <summary>
        /// Is the OpenVRDevice initialized?
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// The Left Controller.
        /// </summary>
        public OVRController LeftController { get; private set; }

        /// <summary>
        /// The Right Controller.
        /// </summary>
        public OVRController RightController { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the OpenVRDevice class.
        /// </summary>
#pragma warning disable CS8618 // Cannot convert null literal to non-nullable reference type.
        public OVRDevice()
#pragma warning restore CS8618 // Cannot convert null literal to non-nullable reference type.
        {
            #region Null Variables

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            cVrSystem = null;
            DeviceActivated = null;
            DeviceDeactivated = null;
            LeftController = null;
            RightController = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            #endregion

            IsInitialized = false;

            trackedDevices = new OVRTrackedDeviceBase[OpenVR.k_unMaxTrackedDeviceCount];
            renderPoseArray = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
            gamePoseArray = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
            ValidDevicePoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
            ValidNextDevicePoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];

            // NOTE: Calls the platform specific version to get this information
            SetPlatformTextureInfo();
        }

        #endregion

        #region Initialize

        #region CanCallNativeDll

        static private bool CanCallNativeDll(out string error)
        {
            error = "";

            try
            {
                return OpenVR.IsRuntimeInstalled();
            }
            catch (DllNotFoundException)
            {
                error = "Can't find openvr_api library, make sure it's next to the executable.";
                return false;
            }
            catch (BadImageFormatException)
            {
                error = "Got a BadImageFormatException, this most often indicates the native library is " +
                        "for the wrong bitness. Make sure the bitness of the native dll matches the target " +
                        "of your .NET Project.";
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Initialize the OpenVR device and controllers (if connected).
        /// </summary>
        virtual public void Initialize()
        {
            if (!IsInitialized)
            {
                EVRInitError initError = EVRInitError.None;

                if (!CanCallNativeDll(out string nativeDllError))
                    throw new OVRRuntimeNotInstalledException(nativeDllError);

                if (!OpenVR.IsHmdPresent())
                    throw new OVRDisplayNotConnectedException();

                cVrSystem = OpenVR.Init(ref initError);
                if (EVRInitError.None == initError)
                {
                    // Look for the Left and Right controllers
                    for (var index = 0; index < OpenVR.k_unMaxTrackedDeviceCount; index++)
                        RegisterDevice(index);
                }
                else
                    throw new OVRInitializationFailedException(OpenVR.GetStringForHmdError(initError));

                IsInitialized = true;
            }
        }

        #endregion
        #region ShutDown

        /// <summary>
        /// Shuts down the OpenVR device.
        /// </summary>
        virtual public void ShutDown()
        {
            OpenVR.Shutdown();
            IsInitialized = false;
        }

        #endregion
        #region Update

        /// <summary>
        /// Called when the controllers should update.
        /// </summary>
        virtual public void Update()
        {
            LeftController?.Update();
            RightController?.Update();
        }

        #endregion

        #region Tracked Devices

        #region IsTrackedDeviceConnected

        /// <summary>
        /// Determnes if the tracked is connected (by index).
        /// </summary>
        /// <param name="index">The device's index.</param>
        /// <returns>true if device is connected, otherwise false.</returns>
        public bool IsTrackedDeviceConnected(int index)
        {
            return cVrSystem.IsTrackedDeviceConnected((uint)index);
        }

        #endregion
        #region RegisterDevice

        private OVRTrackedDeviceBase RegisterDevice(int index)
        {
            var device = CreateTrackedDevice(index);
            trackedDevices[index] = device;

            if (device is OVRController controller)
            {
                ETrackedControllerRole role =
                    cVrSystem.GetControllerRoleForTrackedDeviceIndex((uint)index);

                switch (role)
                {
                    case ETrackedControllerRole.LeftHand:
                        LeftController = controller;
                        break;

                    case ETrackedControllerRole.RightHand:
                        RightController = controller;
                        break;

                    case ETrackedControllerRole.Invalid:
                    case ETrackedControllerRole.OptOut:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(string.Format("RegisterDevice index:{0}", index));
                }
            }

            return device;
        }

        #endregion
        #region CreateTrackedDevice

        internal OVRTrackedDeviceBase CreateTrackedDevice(int index)
        {
#pragma warning disable CS8603 // Possible null reference return.

            return cVrSystem.GetTrackedDeviceClass((uint)index) switch
            {
                ETrackedDeviceClass.Invalid => null,
                ETrackedDeviceClass.HMD => new OVRDisplay(index),
                ETrackedDeviceClass.Controller => new OVRController(index),
                //ETrackedDeviceClass.GenericTracker => ??? // Unused for now
                //ETrackedDeviceClass.TrackingReference => ??? // Unused for now
                //ETrackedDeviceClass.DisplayRedirect => ??? // Unused for now
                _ => throw new ArgumentOutOfRangeException(string.Format("CreateTrackedDevice  index:{0}", index)),
            };

#pragma warning restore CS8603 // Possible null reference return.

        }

        #endregion
        #region GetControllerRole

        /// <summary>
        /// Gets the controller's role (by index).
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The controller's role.</returns>
        public ETrackedControllerRole GetControllerRole(int index)
        {
            return cVrSystem.GetControllerRoleForTrackedDeviceIndex((uint)index);
        }

        #endregion
        #region GetControllerState

        /// <summary>
        /// Get the controller's state.
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="state">A reference to a VRControllerState_t object.</param>
        /// <param name="size">The controller state's size.</param>
        /// <returns>If the call was successful in getting the controller's state.</returns>
        public bool GetControllerState(int index,
                                       ref VRControllerState_t state,
                                       int size)
        {
            return cVrSystem.GetControllerState((uint)index,
                                                ref state,
                                                (uint)size);
        }

        #endregion
        #region GetControllerStateWithPose

        /// <summary>
        /// Get the controller's state at the last button press' pose.
        /// </summary>
        /// <param name="origin">The tracking coordinate system to return the pose in.</param>
        /// <param name="index">The index of the controller</param>
        /// <param name="state">A reference to a VRControllerState_t object.</param>
        /// <param name="size">The controller state's size.</param>
        /// <param name="pose">A pose struct to fill with the pose of the controller when the last button event occurred.</param>
        /// <returns>If the call was successful in getting the controller's state.</returns>
        public bool GetControllerStateWithPose(ETrackingUniverseOrigin origin,
                                               int index,
                                               ref VRControllerState_t state,
                                               int size,
                                               ref TrackedDevicePose_t pose)
        {
            return cVrSystem.GetControllerStateWithPose(origin,
                                                        (uint)index,
                                                        ref state,
                                                        (uint)size,
                                                        ref pose);
        }

        #endregion
        #region GetTrackedDeviceProperty (Various Types)

        #region GetBoolTrackedDeviceProperty

        /// <summary>
        /// Returns a static property for a tracked device.
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="property">Which property to get.</param>
        /// <param name="error">The error returned when attempting to fetch this property. This can be NULL if the caller doesn't care about the source of a property error.</param>
        /// <returns>The type of axis.</returns>
        /// <returns>The static property.</returns>
        public bool GetBoolTrackedDeviceProperty(
            int index,
            ETrackedDeviceProperty property,
            ref ETrackedPropertyError error)
        {
            return cVrSystem.GetBoolTrackedDeviceProperty(
                (uint)index,
                property,
                ref error);
        }

        #endregion
        #region GetFloatTrackedDeviceProperty

        /// <summary>
        /// Returns a static property for a tracked device.
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="property">Which property to get.</param>
        /// <param name="error">The error returned when attempting to fetch this property. This can be NULL if the caller doesn't care about the source of a property error.</param>
        /// <returns>The static property.</returns>
        public float GetFloatTrackedDeviceProperty(
            int index,
            ETrackedDeviceProperty property,
            ref ETrackedPropertyError error)
        {
            return cVrSystem.GetFloatTrackedDeviceProperty(
                (uint)index,
                property,
                ref error);
        }

        #endregion
        #region GetInt32TrackedDeviceProperty

        /// <summary>
        /// Returns a static property for a tracked device.
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="property">Which property to get.</param>
        /// <param name="error">The error returned when attempting to fetch this property. This can be NULL if the caller doesn't care about the source of a property error.</param>
        /// <returns>The static property.</returns>
        public int GetInt32TrackedDeviceProperty(
            int index,
            ETrackedDeviceProperty property,
            ref ETrackedPropertyError error)
        {
            return cVrSystem.GetInt32TrackedDeviceProperty(
                (uint)index,
                property,
                ref error);
        }

        #endregion
        #region GetUInt64TrackedDeviceProperty

        /// <summary>
        /// Returns a static property for a tracked device.
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="property">Which property to get.</param>
        /// <param name="error">The error returned when attempting to fetch this property. This can be NULL if the caller doesn't care about the source of a property error.</param>
        /// <returns>The static property.</returns>
        public ulong GetUInt64TrackedDeviceProperty(
            int index,
            ETrackedDeviceProperty property,
            ref ETrackedPropertyError error)
        {
            return cVrSystem.GetUint64TrackedDeviceProperty(
                (uint)index,
                property,
                ref error);
        }

        #endregion
        //TODO: GetMatrix34TrackedDeviceProperty - identity matrix
        #region GetStringTrackedDeviceProperty

        /// <summary>
        /// Returns a static property for a tracked device.
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="property">Which property to get.</param>
        /// <param name="maxSize">The maximum string size for the property.</param>
        /// <param name="error">The error returned when attempting to fetch this property. This can be NULL if the caller doesn't care about the source of a property error.</param>
        /// <returns>The static property.</returns>
        public string GetStringTrackedDeviceProperty(
            int index,
            ETrackedDeviceProperty property,
            uint maxSize,
            ref ETrackedPropertyError error)
        {
            int capacity = (int)cVrSystem.GetStringTrackedDeviceProperty(
                (uint)index,
                property,
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                maxSize,
                ref error);

            if (capacity > 0)
            {
                StringBuilder sb = new StringBuilder(capacity);

                cVrSystem.GetStringTrackedDeviceProperty(
                    (uint)index,
                    property,
                    sb,
                    (uint)capacity,
                    ref error);

                if (error == ETrackedPropertyError.TrackedProp_Success)
                    return sb.ToString();
            }

            return string.Empty;
        }

        #endregion

        #endregion

        #endregion
        #region Pose Tracking

        #region GetDevicePose

        /// <summary>
        /// Gets the current device's post.
        /// </summary>
        /// <param name="index">The tracked device's index.</param>
        /// <returns>The device's pose.</returns>
        public TrackedDevicePose_t GetDevicePose(int index)
        {
            return ValidDevicePoses[index];
        }

        #endregion
        #region GetNextDevicePose

        /// <summary>
        /// Gets the next device's post.
        /// </summary>
        /// <param name="index">The tracked device's index.</param>
        /// <returns>The device's pose.</returns>
        public TrackedDevicePose_t GetNextDevicePose(int index)
        {
            return ValidNextDevicePoses[index];
        }

        #endregion
        #region GetDeviceToAbsoluteTrackingPose

        /// <summary>
        /// Gets the absolute tracking pose.
        /// </summary>
        /// <param name="origin">Tracking universe that returned poses should be relative to. This will be one of:
        ///<list type="number">
        ///    <item>
        ///        <term>ETrackingUniverseOrigin.TrackingUniverseSeated</term>
        ///    </item>
        ///    <item>
        ///           <term>ETrackingUniverseOrigin.TrackingUniverseStanding</term>
        ///    </item>
        ///    <item><term>ETrackingUniverseOrigin.TrackingUniverseRaw</term></item>
        ///
        ///
        ///     </list>
        ///</param>
        /// <param name="predictedSecondsToPhotonsFromNow">The number of seconds from now to when the next photons will come out of the HMD can be computed automatically. This assumes that the rendering pipeline doesn't have any extra frames buffering.</param>
        /// <param name="trackedDevicePoseArray">The array of poses.</param>
        public void GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin origin,
                                                    float predictedSecondsToPhotonsFromNow,
                                                    TrackedDevicePose_t[] trackedDevicePoseArray)
        {
            cVrSystem.GetDeviceToAbsoluteTrackingPose(origin,
                                                      predictedSecondsToPhotonsFromNow,
                                                      trackedDevicePoseArray);
        }

        #endregion
        #region GetSeatedZeroPoseToStandingAbsoluteTrackingPose

        /// <summary>
        /// Gets the seated zero pose and converts it to the standing absolute 
        /// tracking pose.
        /// </summary>
        /// <returns>The standing absolute tracking pose.</returns>
        public HmdMatrix34_t GetSeatedZeroPoseToStandingAbsoluteTrackingPose()
        {
            return cVrSystem.GetSeatedZeroPoseToStandingAbsoluteTrackingPose();
        }

        #endregion
        #region GetRawZeroPoseToStandingAbsoluteTrackingPose

        /// <summary>
        /// Gets the raw zero pose and converts it to the standing absolute 
        /// tracking pose.
        /// </summary>
        /// <returns>The standing absolute tracking pose.</returns>
        public HmdMatrix34_t GetRawZeroPoseToStandingAbsoluteTrackingPose()
        {
            return cVrSystem.GetRawZeroPoseToStandingAbsoluteTrackingPose();
        }

        #endregion

        #endregion
        #region Rendering

        #region GetRecommendedRenderTargetSize

        /// <summary>
        /// Gets the recommended target size.
        /// </summary>
        /// <param name="width">The recommended target width.</param>
        /// <param name="height">he recommended target height.</param>
        public void GetRecommendedRenderTargetSize(out int width, out int height)
        {
            uint w = 0, h = 0;
            cVrSystem.GetRecommendedRenderTargetSize(ref w, ref h);
            width = (int)w;
            height = (int)h;
        }

        #endregion
        #region GetProjectionMatrix

        /// <summary>
        /// Gets the pojection matrix.
        /// </summary>
        /// <param name="eye">The eye value.</param>
        /// <param name="zNear">The near value.</param>
        /// <param name="zFar">The far value.</param>
        /// <returns>The projection matrix based on the supplied values.</returns>
        public HmdMatrix44_t GetProjectionMatrix(EVREye eye,
                                                 float zNear,
                                                 float zFar)
        {
            return cVrSystem.GetProjectionMatrix(eye, zNear, zFar);
        }

        #endregion
        #region GetProjectionRaw

        /// <summary>
        /// Gets the raw projection values.
        /// </summary>
        /// <param name="eEye">The eye value.</param>
        /// <param name="pfLeft">Tangent of the half-angle from center axis to the left clipping plane.</param>
        /// <param name="pfRight">Tangent of the half-angle from center axis to the right clipping plane.</param>
        /// <param name="pfTop">Tangent of the half-angle from center axis to the top clipping plane.</param>
        /// <param name="pfBottom">Tangent of the half-angle from center axis to the bottom clipping plane.</param>
        public void GetProjectionRaw(EVREye eEye,
                                     ref float pfLeft,
                                     ref float pfRight,
                                     ref float pfTop,
                                     ref float pfBottom)
        {
            pfLeft = 0;
            pfRight = 0;
            pfTop = 0;
            pfBottom = 0;
            cVrSystem.GetProjectionRaw(eEye,
                                       ref pfLeft,
                                       ref pfRight,
                                       ref pfTop,
                                       ref pfBottom);
        }

        #endregion
        #region ComputeDistortion

        /// <summary>
        /// Computes the disterion value.
        /// </summary>
        /// <param name="eye">The eye value.</param>
        /// <param name="u">The horizontal texture coordinate for the output pixel within the viewport.</param>
        /// <param name="v">The vertical texture coordinate for the output pixel within the viewport.</param>
        /// <param name="distortionCoordinates">The struct to be updated with the distortion values.</param>
        /// <returns></returns>
        public bool ComputeDistortion(EVREye eye,
                                      float u,
                                      float v,
                                      ref DistortionCoordinates_t distortionCoordinates)
        {
            return cVrSystem.ComputeDistortion(eye,
                                               u,
                                               v,
                                               ref distortionCoordinates);
        }

        #endregion
        #region GetEyeToHeadTransform

        /// <summary>
        /// Gets the eye to head transform
        /// </summary>
        /// <param name="eye">The eye value.</param>
        /// <returns>The eye to head transform.</returns>
        public HmdMatrix34_t GetEyeToHeadTransform(EVREye eye)
        {
            return cVrSystem.GetEyeToHeadTransform(eye);
        }

        #endregion
        #region GetViewAndProjectionMatricesForEye

        /// <summary>
        /// Gets the view and projection matrices per eye.
        /// </summary>
        /// <param name="eye">The eye value.</param>
        /// <param name="zNear">The near plane for the eye.</param>
        /// <param name="zFar">The far plane for the eye.</param>
        /// <param name="viewMatrix">The view matrix value.</param>
        /// <param name="projectionMatrix">The projection matrix value.</param>
        public void GetViewAndProjectionMatricesForEye(
                                            EVREye eye,
                                            float zNear,
                                            float zFar,
                                            out Matrix viewMatrix,
                                            out Matrix projectionMatrix)
        {
            projectionMatrix = GetProjectionMatrix(eye, zNear, zFar).ToMatrix();
            viewMatrix = GetEyeToHeadTransform(eye).ToMatrix();
        }

        #endregion

        #endregion
        #region Event Processing

        /// <summary>
        /// Process all events.
        /// </summary>
        public void ProcessEvents()
        {
            var ev = new VREvent_t();

            while (cVrSystem.PollNextEvent(ref ev, (uint)Marshal.SizeOf<VREvent_t>()))
            {
                switch ((EVREventType)ev.eventType)
                {
                    case EVREventType.VREvent_TrackedDeviceActivated:
                        HandleDeviceActivated((int)ev.trackedDeviceIndex);
                        break;

                    case EVREventType.VREvent_TrackedDeviceDeactivated:
                        HandleDeviceDeactivated((int)ev.trackedDeviceIndex);
                        break;

                    default:
                        // NOTE: This is for ether events that are not handled.
                        // I may handle more in the future.
                        break;
                }
            }
        }

        #region HandleDeviceActivated (Private)

        private void HandleDeviceActivated(int index)
        {
            var device = RegisterDevice(index);
            DeviceActivated?.Invoke(this, new OVRTrackedDeviceActivateEventArgs(device));
        }

        #endregion
        #region HandleDevicDeaActivated (Private)

        private void HandleDeviceDeactivated(int index)
        {
            #region Null Variable

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

            trackedDevices[index] = null;

#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            #endregion

            var device = trackedDevices[index];
            DeviceDeactivated?.Invoke(this, new OVRTrackedDeviceActivateEventArgs(device));
        }

        #endregion

        #endregion
        #region Compositor

        #region WaitGetPoses

        /// <summary>
        /// Returns pose(s) to use to render scene.
        /// </summary>
        public void WaitGetPoses()
        {
            var error = OpenVR.Compositor.WaitGetPoses(renderPoseArray,
                                                       gamePoseArray);

            if (error != EVRCompositorError.None)
            {
                // TODO: How do I want to handle exceptions?
                //throw CreateException(error);
            }

            for (var i = 0; i < renderPoseArray.Length; i++)
            {
                var p = renderPoseArray[i];
                if (p.bPoseIsValid)
                    ValidDevicePoses[i] = p;
            }

            for (var i = 0; i < gamePoseArray.Length; i++)
            {
                var p = gamePoseArray[i];
                if (p.bPoseIsValid)
                    ValidNextDevicePoses[i] = p;
            }
        }

        #endregion
        #region Submit

        /// <summary>
        /// Submits the eyes to the compositor.
        /// </summary>
        /// <param name="eye">The eye value.</param>
        /// <param name="handle">A pointer handle to the RenderTarget2D for the current eye.</param>
        /// <returns></returns>
        public EVRCompositorError Submit(EVREye eye,
                           IntPtr handle)
        {
            texture.handle = handle;

            return OpenVR.Compositor.Submit(eye,
                                            ref texture,
                                            ref textureBounds,
                                            EVRSubmitFlags.Submit_Default);
        }

        #endregion

        #endregion
    }
}
