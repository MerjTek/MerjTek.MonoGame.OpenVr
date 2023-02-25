using Microsoft.Xna.Framework;
using Valve.VR;

namespace MerjTek.MonoGame.OpenVr
{
    #region OVRControllerState

    /// <summary>
    /// Structure for tracking an OpenVR controller state.
    /// </summary>
    public struct OVRControllerState
    {
        #region Private Variables

        private readonly OVRController.ControllerHandedness handedness;

        #endregion
        #region Public Properties

        #region Buttons

        /// <summary>
        /// The System Button.
        /// </summary>
        public bool SystemButton { get; internal set; }

        /// <summary>
        /// The Menu Button.
        /// </summary>
        public bool MenuButton { get; internal set; }

        /// <summary>
        /// The A Button.
        /// </summary>
        public bool AButton { get; internal set; }

        /// <summary>
        /// The B Button.
        /// </summary>
        public bool BButton { get; internal set; }

        /// <summary>
        /// The X Button.
        /// </summary>
        public bool XButton { get; internal set; }

        /// <summary>
        /// The Y Button.
        /// </summary>
        public bool YButton { get; internal set; }

        /// <summary>
        /// The joystick button.
        /// </summary>
        public bool JoystickButton { get; internal set; }

        /// <summary>
        /// The System Button.
        /// </summary>
        public bool TriggerButton { get; internal set; }

        /// <summary>
        /// The Menu Button.
        /// </summary>
        public bool SideButton { get; internal set; }

        /// <summary>
        /// The Touch Pad has been touched
        /// </summary>
        public bool TouchPadTouched { get; internal set; }

        /// <summary>
        /// The Touch Pad has been pressed
        /// </summary>
        public bool TouchPadPressed { get; internal set; }

        #endregion
        #region Joystick, Triggers, and TouchPad

        /// <summary>
        /// The Front Trigger
        /// </summary>
        public float FrontTrigger { get; internal set; }

        /// <summary>
        /// The Joystick.
        /// </summary>
        public Vector2 Joystick { get; internal set; }

        /// <summary>
        /// The Side Trigger.
        /// </summary>
        public float SideTrigger { get; internal set; }

        /// <summary>
        /// The TouchPad.
        /// </summary>
        public Vector2 TouchPad { get; internal set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the OpenVRControllerState class.
        /// </summary>
        /// <param name="hand">The handedness of the controller.</param>
        public OVRControllerState(OVRController.ControllerHandedness hand)
        {
            SystemButton = false;
            MenuButton = false;
            AButton = false;
            BButton = false;
            XButton = false;
            YButton = false;
            JoystickButton = false;
            SideButton = false;
            TriggerButton = false;
            TouchPadTouched = false;
            TouchPadPressed = false;
            Joystick = Vector2.Zero;
            TouchPad = Vector2.Zero;
            FrontTrigger = 0;
            SideTrigger = 0;
            handedness = hand;
        }

        #endregion

        #region UpdateControllerState

        #region UpdatepAxes (Private)

        #region GetXAxisByIndex (Private)

        static private float GetXAxisByIndex(ref VRControllerState_t state, int index)
        {
            return index switch
            {
                0 => state.rAxis0.x,
                1 => state.rAxis1.x,
                2 => state.rAxis2.x,
                3 => state.rAxis3.x,
                4 => state.rAxis4.x,
                _ => 0,
            };
        }

        #endregion
        #region GetYAxisByIndex (Private)

        static private float GetYAxisByIndex(ref VRControllerState_t state, int index)
        {
            return index switch
            {
                0 => state.rAxis0.y,
                1 => state.rAxis1.y,
                2 => state.rAxis2.y,
                3 => state.rAxis3.y,
                4 => state.rAxis4.y,
                _ => 0,
            };
        }

        #endregion

        private void UpdateAxes(ref VRControllerState_t state,
                                OVRControllerProfile profile)
        {
            Joystick = new Vector2(
                GetXAxisByIndex(ref state, profile.Joystick_AxisX),
                GetYAxisByIndex(ref state, profile.Joystick_AxisY));

            TouchPad = new Vector2(
                GetXAxisByIndex(ref state, profile.TouchPad_AxisX),
                GetYAxisByIndex(ref state, profile.TouchPad_AxisY));

            // NOTE: Should the X or Y axis be used.
            if (-1 == profile.FrontTrigger_AxisY)
                FrontTrigger = GetXAxisByIndex(ref state, profile.FrontTrigger_AxisX);
            else if (-1 == profile.FrontTrigger_AxisX)
                FrontTrigger = GetYAxisByIndex(ref state, profile.FrontTrigger_AxisY);

            // NOTE: Should the X or Y axis be used.
            if (-1 == profile.SideTrigger_AxisY)
                SideTrigger = GetXAxisByIndex(ref state, profile.SideTrigger_AxisX);
            else if (-1 == profile.SideTrigger_AxisX)
                SideTrigger = GetYAxisByIndex(ref state, profile.SideTrigger_AxisY);
        }

        #endregion
        #region UpdateButtons (Private)

        #region IsBitSet (Private)

        static bool IsBitSet(ulong pressed, int shift)
        {
            ulong value = (1UL << shift);
            return (pressed & value) != 0;
        }

        #endregion

        private void UpdateButtons(ref VRControllerState_t state,
                                   OVRControllerProfile profile)
        {
            if (OVRController.ControllerHandedness.Left == handedness)
            {
                AButton = IsBitSet(state.ulButtonTouched, profile.AButtonIndex);
                BButton = IsBitSet(state.ulButtonTouched, profile.BButtonIndex);
            }

            if (OVRController.ControllerHandedness.Right == handedness)
            {
                XButton = IsBitSet(state.ulButtonTouched, profile.XButtonIndex);
                YButton = IsBitSet(state.ulButtonTouched, profile.YButtonIndex);
            }

            SystemButton = IsBitSet(state.ulButtonTouched, profile.SystemButtonIndex);
            MenuButton = IsBitSet(state.ulButtonTouched, profile.MenuButtonIndex);
            JoystickButton = IsBitSet(state.ulButtonTouched, profile.JoystickButtonIndex);
            SideButton = IsBitSet(state.ulButtonTouched, profile.SideButtonIndex);
            TriggerButton = IsBitSet(state.ulButtonTouched, profile.TriggerButtonIndex);
            TouchPadTouched = IsBitSet(state.ulButtonTouched, profile.TouchPadTouchedIndex);
            TouchPadPressed = IsBitSet(state.ulButtonPressed, profile.TouchPadTouchedIndex);
        }

        #endregion

        /// <summary>
        /// Updates the buttons, axes, triggers, and the touchpad information.
        /// </summary>
        /// <param name="state">The VR controller state.</param>
        /// <param name="profile">The controller profile.</param>
        public void UpdateControllerState(ref VRControllerState_t state,
                                          OVRControllerProfile profile)
        {
            UpdateAxes(ref state, profile);
            UpdateButtons(ref state, profile);
        }

        #endregion
    }

    #endregion
    #region OVRControllerProfile

    /// <summary>
    /// Structure for tracking the  OpenVR controller button indices.
    /// </summary>
    public struct OVRControllerProfile
    {
        #region Public Properties

        #region Axes

        /// <summary>
        /// Whick hardware axis is used as the joystick X axis [0 to 4].
        /// </summary>
        public int Joystick_AxisX { get; set; } = -1;

        /// <summary>
        /// Whick hardware axis is used as the joystick Y axis [0 to 4].
        /// </summary>
        public int Joystick_AxisY { get; set; } = -1;

        /// <summary>
        /// Whick hardware axis is used as the touchpad X axis [0 to 4].
        /// </summary>
        public int TouchPad_AxisX { get; set; } = -1;

        /// <summary>
        /// Whick hardware axis is used as the touchpad Y axis [0 to 4].
        /// </summary>
        public int TouchPad_AxisY { get; set; } = -1;

        /// <summary>
        /// Whick hardware X axis is used as the front trigger [0 to 4].
        /// </summary>
        public int FrontTrigger_AxisX { get; set; } = -1;

        /// <summary>
        /// Whick hardware Y axis is used as the front trigger [0 to 4].
        /// </summary>
        public int FrontTrigger_AxisY { get; set; } = -1;

        /// <summary>
        /// Whick hardware X axis is used as the side trigger [0 to 4].
        /// </summary>
        public int SideTrigger_AxisX { get; set; } = -1;

        /// <summary>
        /// Whick hardware Y axis is used as the side trigger [0 to 4].
        /// </summary>
        public int SideTrigger_AxisY { get; set; } = -1;

        #endregion
        #region Buttons

        /// <summary>
        /// The System Button index.
        /// </summary>
        public int SystemButtonIndex { get; set; } = 0;

        /// <summary>
        /// The Application Menu Button index.
        /// </summary>
        public int MenuButtonIndex { get; set; } = 0;

        /// <summary>
        /// The A Button index. On the same controller as the B Button index.
        /// </summary>
        public int AButtonIndex { get; set; } = 0;

        /// <summary>
        /// The B Button index. On the same controller as the A Button index.
        /// </summary>
        public int BButtonIndex { get; set; } = 0;

        /// <summary>
        /// The X Button index. On the same controller as the Y Button index.
        /// </summary>
        public int XButtonIndex { get; set; } = 0;

        /// <summary>
        /// The Y Button index. On the same controller as the X Button index.
        /// </summary>
        public int YButtonIndex { get; set; } = 0;

        /// <summary>
        /// Joystick Button index.
        /// </summary>
        public int JoystickButtonIndex { get; set; } = 0;

        /// <summary>
        /// The Grip Button index.
        /// </summary>
        public int SideButtonIndex { get; set; } = 0;

        /// <summary>
        /// The Trigget Button index.
        /// </summary>
        public int TriggerButtonIndex { get; set; } = 0;

        /// <summary>
        /// The TouchPad has been touched index.
        /// </summary>
        public int TouchPadTouchedIndex { get; set; } = 0;

        /// <summary>
        /// The TouchPad has been pressed index.
        /// </summary>
        public int TouchPadPressedIndex { get; set; } = 0;

        #endregion

        #endregion

        /// <summary>
        /// Constructs an instance of a OVRControllerProfile object.
        /// </summary>
        public OVRControllerProfile() { }

        #region Static Profiles

        #region Public Properties

        /// <summary>
        /// The Default controller profile.
        /// </summary>
        public static readonly OVRControllerProfile Default;

        #region Valve

        /// <summary>
        /// The controller profile compatible with Valve.
        /// </summary>
        public static readonly OVRControllerProfile Valve;

        #endregion
        #region Windows Mixed Reality

        /// <summary>
        /// The Windows Mixed Reality controller profile.
        /// </summary>
        public static readonly OVRControllerProfile WindowsMixedReality;

        #endregion
        #region Oculus / Meta

        /// <summary>
        /// The Oculous controller profile.
        /// </summary>
        public static readonly OVRControllerProfile Oculus;

        #endregion
        #region HTC

        /// <summary>
        /// The controller profile compatible with HTC.
        /// </summary>
        public static readonly OVRControllerProfile HTC;

        #endregion
        #region HP

        /// <summary>
        /// The controller profile compatible with HP.
        /// </summary>
        public static readonly OVRControllerProfile HP;

        #endregion
        #region Pico

        /// <summary>
        /// The controller profile compatible with Pico.
        /// </summary>
        public static readonly OVRControllerProfile Pico;

        #endregion

        #endregion

        static OVRControllerProfile()
        {
            // TODO: Set these bits to their appropriate values.

            #region Default

            Default = new OVRControllerProfile()
            {
                // Axes
                Joystick_AxisX = -1, // Unused
                Joystick_AxisY = -1, // Unused
                TouchPad_AxisX = -1, // Unused
                TouchPad_AxisY = -1, // Unused
                FrontTrigger_AxisX = -1, // Unused
                FrontTrigger_AxisY = -1, // Unused
                SideTrigger_AxisX = -1, // Unused
                SideTrigger_AxisY = -1, // Unused

                // Buttons
                SystemButtonIndex = (int)EVRButtonId.k_EButton_System,
                MenuButtonIndex = (int)EVRButtonId.k_EButton_ApplicationMenu,
                AButtonIndex = (int)EVRButtonId.k_EButton_A,
                BButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                XButtonIndex = (int)EVRButtonId.k_EButton_A,
                YButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                JoystickButtonIndex = (int)EVRButtonId.k_EButton_IndexController_JoyStick,
                SideButtonIndex = (int)EVRButtonId.k_EButton_Grip,
                TriggerButtonIndex = (int)EVRButtonId.k_EButton_SteamVR_Trigger,
                TouchPadTouchedIndex = (int)EVRButtonId.k_EButton_SteamVR_Touchpad,
                TouchPadPressedIndex = (int)EVRButtonId.k_EButton_SteamVR_Touchpad,
            };

            #endregion
            #region HP

            HP = new OVRControllerProfile()
            {
                // Axes
                Joystick_AxisX = -1, // Unused
                Joystick_AxisY = -1, // Unused
                TouchPad_AxisX = -1, // Unused
                TouchPad_AxisY = -1, // Unused
                FrontTrigger_AxisX = -1, // Unused
                FrontTrigger_AxisY = -1, // Unused
                SideTrigger_AxisX = -1, // Unused
                SideTrigger_AxisY = -1, // Unused

                // Buttons
                SystemButtonIndex = (int)EVRButtonId.k_EButton_System,
                MenuButtonIndex = (int)EVRButtonId.k_EButton_ApplicationMenu,
                AButtonIndex = (int)EVRButtonId.k_EButton_A,
                BButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                XButtonIndex = (int)EVRButtonId.k_EButton_A,
                YButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                JoystickButtonIndex = (int)EVRButtonId.k_EButton_IndexController_JoyStick,
                SideButtonIndex = (int)EVRButtonId.k_EButton_Grip,
                TriggerButtonIndex = (int)EVRButtonId.k_EButton_SteamVR_Trigger,
                TouchPadTouchedIndex = (int)EVRButtonId.k_EButton_SteamVR_Touchpad,
                TouchPadPressedIndex = (int)EVRButtonId.k_EButton_SteamVR_Touchpad,
            };

            #endregion
            #region HTC

            HTC = new OVRControllerProfile()
            {
                // Axes
                Joystick_AxisX = -1, // Unused
                Joystick_AxisY = -1, // Unused
                TouchPad_AxisX = 0,
                TouchPad_AxisY = 1,
                FrontTrigger_AxisX = 2, // Unused
                FrontTrigger_AxisY = -1, // Unused
                SideTrigger_AxisX = -1, // Unused
                SideTrigger_AxisY = -1, // Unused

                // Buttons
                SystemButtonIndex = (int)EVRButtonId.k_EButton_System,
                MenuButtonIndex = (int)EVRButtonId.k_EButton_ApplicationMenu,
                AButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                BButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                XButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                YButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                JoystickButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                SideButtonIndex = 2,
                TriggerButtonIndex = 15,
                TouchPadTouchedIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                TouchPadPressedIndex = 14,
            };

            #endregion
            #region Oculus / Meta

            Oculus = new OVRControllerProfile
            {
                // Axes
                Joystick_AxisX = -1, // Unused
                Joystick_AxisY = -1, // Unused
                TouchPad_AxisX = 0,
                TouchPad_AxisY = 1,
                FrontTrigger_AxisX = 2, // Unused
                FrontTrigger_AxisY = -1, // Unused
                SideTrigger_AxisX = 4, // Unused
                SideTrigger_AxisY = -1, // Unused

                // Buttons
                SystemButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                MenuButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                AButtonIndex = 7,
                BButtonIndex = 1,
                XButtonIndex = 7,
                YButtonIndex = 1,
                JoystickButtonIndex = 14,
                SideButtonIndex = 2,
                TriggerButtonIndex = 15,
                TouchPadTouchedIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                TouchPadPressedIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
            };

            #endregion
            #region Pico

            Pico = new OVRControllerProfile()
            {
                // Axes
                Joystick_AxisX = -1, // Unused
                Joystick_AxisY = -1, // Unused
                TouchPad_AxisX = -1, // Unused
                TouchPad_AxisY = -1, // Unused
                FrontTrigger_AxisX = -1, // Unused
                FrontTrigger_AxisY = -1, // Unused
                SideTrigger_AxisX = -1, // Unused
                SideTrigger_AxisY = -1, // Unused

                // Buttons                
                SystemButtonIndex = (int)EVRButtonId.k_EButton_System,
                MenuButtonIndex = (int)EVRButtonId.k_EButton_ApplicationMenu,
                AButtonIndex = (int)EVRButtonId.k_EButton_A,
                BButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                XButtonIndex = (int)EVRButtonId.k_EButton_A,
                YButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                JoystickButtonIndex = (int)EVRButtonId.k_EButton_IndexController_JoyStick,
                SideButtonIndex = (int)EVRButtonId.k_EButton_Grip,
                TriggerButtonIndex = (int)EVRButtonId.k_EButton_SteamVR_Trigger,
                TouchPadTouchedIndex = (int)EVRButtonId.k_EButton_SteamVR_Touchpad,
                TouchPadPressedIndex = (int)EVRButtonId.k_EButton_SteamVR_Touchpad,
            };

            #endregion
            #region Valve

            Valve = new OVRControllerProfile()
            {
                // Axes
                Joystick_AxisX = -1, // Unused
                Joystick_AxisY = -1, // Unused
                TouchPad_AxisX = -1, // Unused
                TouchPad_AxisY = -1, // Unused
                FrontTrigger_AxisX = -1, // Unused
                FrontTrigger_AxisY = -1, // Unused
                SideTrigger_AxisX = -1, // Unused
                SideTrigger_AxisY = -1, // Unused

                // Buttons
                SystemButtonIndex = (int)EVRButtonId.k_EButton_System,
                MenuButtonIndex = (int)EVRButtonId.k_EButton_ApplicationMenu,
                AButtonIndex = (int)EVRButtonId.k_EButton_A,
                BButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                XButtonIndex = (int)EVRButtonId.k_EButton_A,
                YButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                JoystickButtonIndex = (int)EVRButtonId.k_EButton_IndexController_JoyStick,
                SideButtonIndex = (int)EVRButtonId.k_EButton_Grip,
                TriggerButtonIndex = (int)EVRButtonId.k_EButton_SteamVR_Trigger,
                TouchPadTouchedIndex = (int)EVRButtonId.k_EButton_SteamVR_Touchpad,
                TouchPadPressedIndex = (int)EVRButtonId.k_EButton_SteamVR_Touchpad,

            };

            #endregion
            #region Windows Mixed Reality

            WindowsMixedReality = new OVRControllerProfile()
            {
                // Axes
                Joystick_AxisX = -1, // Unused
                Joystick_AxisY = -1, // Unused
                TouchPad_AxisX = -1, // Unused
                TouchPad_AxisY = -1, // Unused
                FrontTrigger_AxisX = -1, // Unused
                FrontTrigger_AxisY = -1, // Unused
                SideTrigger_AxisX = -1, // Unused
                SideTrigger_AxisY = -1, // Unused

                // Buttons                
                SystemButtonIndex = (int)EVRButtonId.k_EButton_System,
                MenuButtonIndex = (int)EVRButtonId.k_EButton_ApplicationMenu,
                AButtonIndex = (int)EVRButtonId.k_EButton_A,
                BButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                XButtonIndex = (int)EVRButtonId.k_EButton_A,
                YButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                JoystickButtonIndex = (int)EVRButtonId.k_EButton_IndexController_JoyStick,
                SideButtonIndex = (int)EVRButtonId.k_EButton_Grip,
                TriggerButtonIndex = (int)EVRButtonId.k_EButton_SteamVR_Trigger,
                TouchPadTouchedIndex = (int)EVRButtonId.k_EButton_SteamVR_Touchpad,
                TouchPadPressedIndex = (int)EVRButtonId.k_EButton_SteamVR_Touchpad,

            };

            #endregion
        }

        #endregion
    }

    #endregion
}
