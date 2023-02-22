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
        public OVRControllerProfile ControllerProfile => profile;

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
        /// The controller's rotation matrix.
        /// </summary>
        public Matrix RotationMatrix { get { return GetPose(); } }

        /// <summary>
        /// Enables the controller to throw an exception when getting the state fails.
        /// </summary>
        public bool ThrowOnGetStateFailure { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the OpenVRController class.
        /// </summary>
        /// <param name="index">The index of the device.</param>
        /// <param name="throwOnFailure">Determines if getting the controller state with throw an exception on failure.</param>
        public OVRController(int index,
                             bool throwOnFailure = false) :
            base(index)
        {
            universeOrigin = ETrackingUniverseOrigin.TrackingUniverseStanding;
            profile = OVRControllerProfile.Default;
            OVRDevice device = OVRDevice.Get();

            // Get controller handedness
            switch (device.GetControllerRole(index))
            {
                case ETrackedControllerRole.LeftHand:
                    Handedness = ControllerHandedness.Left;
                    break;

                case ETrackedControllerRole.RightHand:
                    Handedness = ControllerHandedness.Right;
                    break;

                default:
                    Handedness = ControllerHandedness.None;
                    break;
            }

            // Create the controller state object.
            currentState = new OVRControllerState(Index, Handedness);

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
            VRControllerState_t state = new VRControllerState_t();
            TrackedDevicePose_t trackedDevicePose = new TrackedDevicePose_t();

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
