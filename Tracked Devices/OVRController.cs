using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Valve.VR;

namespace MerjTek.MonoGame.OpenVr
{
    /// <summary>
    /// Class for tracking an OpenVR controller.
    /// </summary>
    public class OVRController : OVRTrackedDeviceBase
    {
        #region ControllerHandedness Enum

        /// <summary>
        /// Enum that denotes the handedness of the controller.
        /// </summary>
        public enum ControllerHandedness
        {
            /// <summary>
            /// No handed controller.
            /// </summary>
            None = 0,

            /// <summary>
            /// Left handed controller.
            /// </summary>
            Left = 1,

            /// <summary>
            /// Right handed controller.
            /// </summary>
            Right = 2,
        }

        #endregion
        #region Private Variables

        private OVRControllerState currentState;
        private ETrackingUniverseOrigin universeOrigin;
        private OVRControllerProfile profile;

        #endregion
        #region Public Properties

        /// <summary>
        /// The current controller state.
        /// </summary>
        public OVRControllerState ControllerState { get { return currentState; } }

        /// <summary>
        /// The controller's profile.
        /// </summary>
        public OVRControllerProfile ControllerProfile
        {
            get { return profile; }
            set { profile = value; }
        }

        /// <summary>
        /// The Handedness of the controller.
        /// </summary>
        public ControllerHandedness Handedness { get; private set; }

        /// <summary>
        /// The controller's position matrix. (TODO)
        /// </summary>
        public Matrix PositionMatrix { get { return GetPose(); } }

        /// <summary>
        /// The controller's position vector. (TODO)
        /// </summary>
        public Vector3 PositionVector { get { return GetPose().Translation; } }

        /// <summary>
        /// The controller's rotation matrix. (TODO)
        /// </summary>
        public Matrix RotationMatrix { get { return GetPose(); } }

        /// <summary>
        /// Enables the controller to throw an exception when getting the state fails.
        /// </summary>
        public bool ThrowOnGetStateFailure { get; set; }

        /// <summary>
        /// The tracking origin of the universe.
        /// </summary>
        public ETrackingUniverseOrigin UniverseOrigin
        {
            get { return universeOrigin; }
            set { universeOrigin = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the OpenVRController class.
        /// </summary>
        /// <param name="index">The index of the device.</param>
        /// <param name="throwOnFailure">Determines if getting the controller state will throw an exception on failure.</param>
        public OVRController(int index,
                             bool throwOnFailure = false) :
            base(index)
        {
            universeOrigin = ETrackingUniverseOrigin.TrackingUniverseStanding;
            OVRDevice device = OVRDevice.Get();

            #region Set the controller profile

            var error = ETrackedPropertyError.TrackedProp_Success;

            string manufacturer = device.GetStringTrackedDeviceProperty(
                0,
                Valve.VR.ETrackedDeviceProperty.Prop_ManufacturerName_String,
                256,
                ref error);

            profile = manufacturer.ToLower() switch
            {
                "hpP" => OVRControllerProfile.HP,
                "htc" => OVRControllerProfile.HTC,
                "meta" or "oculus" => OVRControllerProfile.Oculus,
                "pico" => OVRControllerProfile.Pico,
                "valve" => OVRControllerProfile.Valve,
                "windowsmr" => OVRControllerProfile.WindowsMixedReality,
                _ => OVRControllerProfile.Default,
            };

            // NOTE: I may not need this yet
            //string model = device.GetStringTrackedDeviceProperty(
            //    0,
            //    Valve.VR.ETrackedDeviceProperty.Prop_ModelNumber_String,
            //    256,
            //    ref error);

            #endregion
            #region Set the controller handedness

            Handedness = device.GetControllerRole(index) switch
            {
                ETrackedControllerRole.LeftHand => ControllerHandedness.Left,
                ETrackedControllerRole.RightHand => ControllerHandedness.Right,
                //ETrackedControllerRole.Invalid => ControllerHandedness.None,
                //ETrackedControllerRole.OptOut=> ControllerHandedness.None,
                //ETrackedControllerRole.Treadmill=> ControllerHandedness.None,
                //ETrackedControllerRole.Stylus=> ControllerHandedness.None,
                _ => ControllerHandedness.None,
            };

            #endregion
            #region Create the controller state object.

            currentState = new OVRControllerState(Handedness);

            #endregion

            ThrowOnGetStateFailure = throwOnFailure;
        }

        #endregion

        #region Update

        /// <summary>
        /// Gets the current state of the controller.
        /// </summary>
        public void Update()
        {
            OVRDevice device = OVRDevice.Get();
            VRControllerState_t state = new();
            TrackedDevicePose_t trackedDevicePose = new();

            try
            {
                device.GetControllerStateWithPose(universeOrigin,
                                                  Index,
                                                  ref state,
                                                  Marshal.SizeOf<VRControllerState_t>(),
                                                  ref trackedDevicePose);

                currentState.UpdateControllerState(ref state, profile);
            }
            catch
            {
                if (ThrowOnGetStateFailure)
                    throw new OVRGetControllerStateFailedException(Handedness);
            }
        }

        #endregion
    }
}
