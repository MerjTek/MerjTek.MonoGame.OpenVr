using System;
using System.Runtime.CompilerServices;
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
        #region Constants

        const int cAxisTypesCount = 5;

        #endregion
        #region Private Variables

        private readonly EVRControllerAxisType[] axisTypes;
        private readonly int axisCount;
        private readonly int touchPadCount;
        private readonly int triggerCount;
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
        public bool GripButton { get; internal set; }

        #endregion
        #region Axes, Triggers, and TouchPads

        /// <summary>
        /// 
        /// </summary>
        public OVRControllerAxis[] Axes { get; internal set; }

        /// <summary>
        /// THe number of axes on this controller.
        /// </summary>
        public int AxisCount { get { return axisCount; } }

        /// <summary>
        /// The Triggers
        /// </summary>
        public OVRControllerTrigger[] Triggers { get; internal set; }

        /// <summary>
        /// The numbers of triggers on this controller.
        /// </summary>
        public int TriggerCount { get { return triggerCount; } }

        /// <summary>
        /// 
        /// </summary>
        public OVRControllerAxis TouchPad { get; internal set; }

        /// <summary>
        /// The numbers of touch pads on this controller.
        /// </summary>
        public int TouchPadCount { get { return touchPadCount; } }

        /// <summary>
        /// The Touch Pad has been touched
        /// </summary>
        public bool TouchPadTouched { get; internal set; }

        /// <summary>
        /// The Touch Pad has been pressed
        /// </summary>
        public bool TouchPadPressed { get; internal set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the OpenVRControllerState class.
        /// </summary>
        /// <param name="deviceIndex">The device index of the controller.</param>
        /// <param name="hand">The handedness of the controller.</param>
        public OVRControllerState(int deviceIndex,
                                  OVRController.ControllerHandedness hand)
        {
            OVRDevice device = OVRDevice.Get();

            #region Count the number of Axis types to use.

            axisCount = 0;
            touchPadCount = 0;
            triggerCount = 0;
            axisTypes = new EVRControllerAxisType[cAxisTypesCount];

            ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;
            for (int i = 0; i < cAxisTypesCount; i++)
            {
                axisTypes[i] = (EVRControllerAxisType)device.GetInt32TrackedDeviceProperty(
                                  deviceIndex,
                                  ETrackedDeviceProperty.Prop_Axis0Type_Int32 + i,
                                  ref error);

                switch (axisTypes[i])
                {
                    case EVRControllerAxisType.k_eControllerAxis_TrackPad: touchPadCount++; break;
                    case EVRControllerAxisType.k_eControllerAxis_Joystick: axisCount++; break;
                    case EVRControllerAxisType.k_eControllerAxis_Trigger: triggerCount++; break;
                    case EVRControllerAxisType.k_eControllerAxis_None:
                    default:
                        break;
                };
            }

            #region Based on the counts, allocate each type of array

            if (axisCount > 0)
                Axes = new OVRControllerAxis[axisCount];
            else
                Axes = new OVRControllerAxis[1];

            if (triggerCount > 0)
                Triggers = new OVRControllerTrigger[triggerCount];
            else
                Triggers = new OVRControllerTrigger[1];

            // NOTE: Always allocate one of these
            TouchPad = new OVRControllerAxis();

            #endregion

            #endregion

            SystemButton = false;
            MenuButton = false;
            AButton = false;
            BButton = false;
            XButton = false;
            YButton = false;
            JoystickButton = false;
            GripButton = false;
            TriggerButton = false;
            TouchPadTouched = false;
            TouchPadPressed = false;
            handedness = hand;
        }

        #endregion

        #region UpdateControllerState

        #region UpdatepAxes (Private)

        private void UpdateAxes(ref VRControllerState_t state,
                                OVRControllerProfile profile)
        {
            int axisCount = 0;
            int triggerCount = 0;

            #region state.rAxis0

            switch (axisTypes[0])
            {
                case EVRControllerAxisType.k_eControllerAxis_TrackPad:
                    TouchPad = state.rAxis0.ToVector2().ToOVRControllerAxis();
                    break;

                case EVRControllerAxisType.k_eControllerAxis_Trigger:
                    Triggers[triggerCount] = state.rAxis0.ToVector2().X.ToOVRControllerTrigger();
                    triggerCount++;
                    break;

                case EVRControllerAxisType.k_eControllerAxis_Joystick:
                    Axes[axisCount] = state.rAxis0.ToVector2().ToOVRControllerAxis();
                    axisCount++;
                    break;

                case EVRControllerAxisType.k_eControllerAxis_None:
                default:
                    break;
            }

            #endregion
            #region state.rAxis1

            switch (axisTypes[1])
            {
                case EVRControllerAxisType.k_eControllerAxis_TrackPad:
                    TouchPad = state.rAxis1.ToVector2().ToOVRControllerAxis();
                    break;

                case EVRControllerAxisType.k_eControllerAxis_Trigger:
                    Triggers[triggerCount] = state.rAxis1.ToVector2().X.ToOVRControllerTrigger();
                    triggerCount++;
                    break;

                case EVRControllerAxisType.k_eControllerAxis_Joystick:
                    Axes[axisCount] = state.rAxis1.ToVector2().ToOVRControllerAxis();
                    axisCount++;
                    break;

                case EVRControllerAxisType.k_eControllerAxis_None:
                default:
                    break;
            }

            #endregion
            #region state.rAxis2

            switch (axisTypes[2])
            {
                case EVRControllerAxisType.k_eControllerAxis_TrackPad:
                    TouchPad = state.rAxis2.ToVector2().ToOVRControllerAxis();
                    break;

                case EVRControllerAxisType.k_eControllerAxis_Trigger:
                    Triggers[triggerCount] = state.rAxis2.ToVector2().X.ToOVRControllerTrigger();
                    triggerCount++;
                    break;

                case EVRControllerAxisType.k_eControllerAxis_Joystick:
                    Axes[axisCount] = state.rAxis2.ToVector2().ToOVRControllerAxis();
                    axisCount++;
                    break;

                case EVRControllerAxisType.k_eControllerAxis_None:
                default:
                    break;
            }

            #endregion
            #region state.rAxis3

            switch (axisTypes[3])
            {
                case EVRControllerAxisType.k_eControllerAxis_TrackPad:
                    TouchPad = state.rAxis3.ToVector2().ToOVRControllerAxis();
                    break;

                case EVRControllerAxisType.k_eControllerAxis_Trigger:
                    Triggers[triggerCount] = state.rAxis3.ToVector2().X.ToOVRControllerTrigger();
                    triggerCount++;
                    break;

                case EVRControllerAxisType.k_eControllerAxis_Joystick:
                    Axes[axisCount] = state.rAxis3.ToVector2().ToOVRControllerAxis();
                    axisCount++;
                    break;

                case EVRControllerAxisType.k_eControllerAxis_None:
                default:
                    break;
            }

            #endregion
            #region state.rAxis4

            switch (axisTypes[4])
            {
                case EVRControllerAxisType.k_eControllerAxis_TrackPad:
                    TouchPad = state.rAxis4.ToVector2().ToOVRControllerAxis();
                    break;

                case EVRControllerAxisType.k_eControllerAxis_Trigger:
                    Triggers[triggerCount] = state.rAxis4.ToVector2().X.ToOVRControllerTrigger();
                    triggerCount++;
                    break;

                case EVRControllerAxisType.k_eControllerAxis_Joystick:
                    Axes[axisCount] = state.rAxis4.ToVector2().ToOVRControllerAxis();
                    axisCount++;
                    break;

                case EVRControllerAxisType.k_eControllerAxis_None:
                default:
                    break;
            }

            #endregion
        }

        #endregion
        #region UpdateButtons (Private)

        #region IsBitSet (Private)

        bool IsBitSet(ulong pressed, int shift)
        {
            ulong value = (1UL << shift);
            return (pressed & value) == value;
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

            if (OVRController.ControllerHandedness.Left == handedness)
            {
                XButton = IsBitSet(state.ulButtonTouched, profile.XButtonIndex);
                YButton = IsBitSet(state.ulButtonTouched, profile.YButtonIndex);
            }

            SystemButton = IsBitSet(state.ulButtonTouched, profile.SystemButtonIndex);
            MenuButton = IsBitSet(state.ulButtonTouched, profile.MenuButtonIndex);
            JoystickButton = IsBitSet(state.ulButtonTouched, profile.JoystickButtonIndex);
            GripButton = IsBitSet(state.ulButtonTouched, profile.GripButtonIndex);
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
        public int GripButtonIndex { get; set; } = 0;

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
            #region Default

            Default = new OVRControllerProfile()
            {
                SystemButtonIndex = (int)EVRButtonId.k_EButton_System,
                MenuButtonIndex = (int)EVRButtonId.k_EButton_ApplicationMenu,
                AButtonIndex = (int)EVRButtonId.k_EButton_A,
                BButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                XButtonIndex = (int)EVRButtonId.k_EButton_A,
                YButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                JoystickButtonIndex = (int)EVRButtonId.k_EButton_IndexController_JoyStick,
                GripButtonIndex = (int)EVRButtonId.k_EButton_Grip,
                TriggerButtonIndex = (int)EVRButtonId.k_EButton_SteamVR_Trigger,
                TouchPadTouchedIndex = (int)EVRButtonId.k_EButton_SteamVR_Touchpad,
                TouchPadPressedIndex = (int)EVRButtonId.k_EButton_SteamVR_Touchpad,
            };

            #endregion
            #region Valve

            Valve = new OVRControllerProfile()
            {
                SystemButtonIndex = (int)EVRButtonId.k_EButton_System,
                MenuButtonIndex = (int)EVRButtonId.k_EButton_ApplicationMenu,
                AButtonIndex = 0,
                BButtonIndex = 0,
                XButtonIndex = 0,
                YButtonIndex = 0,
                JoystickButtonIndex = 0,
                GripButtonIndex = 0,
                TriggerButtonIndex = 0,
                TouchPadTouchedIndex = 0,
                TouchPadPressedIndex = 0,
            };

            #endregion
            #region Windows Mixed Reality
            #endregion
            #region Oculus / Meta

            Oculus = new OVRControllerProfile
            {
                /*
                Axis 0 = Left/Right on the joystick
                Axis 1 = Forward/Backward on the joystick
                Axis 2 = Front trigger
                Axis 4 = Side trigger
                */
                SystemButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                MenuButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                AButtonIndex = 7,
                BButtonIndex = 1,
                XButtonIndex = 7,
                YButtonIndex = 1,
                JoystickButtonIndex = 14,
                GripButtonIndex = 2,
                TriggerButtonIndex = 15,
                TouchPadTouchedIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                TouchPadPressedIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
            };

            #endregion
            #region HTC

            HTC = new OVRControllerProfile()
            {
                /*
                Axis 0 = Left/Right on the touchpad
                Axis 1 = Forward/Backward on the touchpad
                Axis 2 = Front trigger
                */
                SystemButtonIndex = (int)EVRButtonId.k_EButton_System,
                MenuButtonIndex = (int)EVRButtonId.k_EButton_ApplicationMenu,
                AButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                BButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                XButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                YButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                JoystickButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                GripButtonIndex = 2,
                TriggerButtonIndex = 15,
                TouchPadTouchedIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                TouchPadPressedIndex = 14,
            };

            #endregion
            #region HP

            HP = new OVRControllerProfile()
            {
                SystemButtonIndex = 0,
                MenuButtonIndex = 0,
                AButtonIndex = 0,
                BButtonIndex = 0,
                XButtonIndex = 0,
                YButtonIndex = 0,
                JoystickButtonIndex = 0,
                GripButtonIndex = 0,
                TriggerButtonIndex = 0,
                TouchPadTouchedIndex = 0,
                TouchPadPressedIndex = 0,
            };

            #endregion
            #region Pico

            Pico = new OVRControllerProfile()
            {
                SystemButtonIndex = (int)EVRButtonId.k_EButton_System,
                MenuButtonIndex = (int)EVRButtonId.k_EButton_ApplicationMenu,
                AButtonIndex = (int)EVRButtonId.k_EButton_A,
                BButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                XButtonIndex = (int)EVRButtonId.k_EButton_A,
                YButtonIndex = (int)EVRButtonId.k_EButton_Max, // TODO:
                JoystickButtonIndex = (int)EVRButtonId.k_EButton_IndexController_JoyStick,
                GripButtonIndex = (int)EVRButtonId.k_EButton_Grip,
                TriggerButtonIndex = (int)EVRButtonId.k_EButton_SteamVR_Trigger,
                TouchPadTouchedIndex = (int)EVRButtonId.k_EButton_SteamVR_Touchpad,
                TouchPadPressedIndex = (int)EVRButtonId.k_EButton_SteamVR_Touchpad,
            };

            #endregion
        }

        #endregion
    }

    #endregion
    #region OVRControllerAxis

    /// <summary>
    /// Holds an axis value.
    /// </summary>
    public struct OVRControllerAxis
    {
        #region Private Variables

        private float x;
        private float y;

        #endregion
        #region Public Properties

        /// <summary>
        /// The X value for the axis.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// The Y value for the axis.
        /// </summary>
        public float Y { get; set; }

        #endregion

        /// <summary>
        /// Creates an instance of the OVRControllerAxis class.
        /// </summary>
        /// <param name="axisPos">The axis value to set.</param>
        public OVRControllerAxis(Vector2 axisPos)
        {
            this = default;
            X = Math.Clamp(axisPos.X, -1, 1);
            Y = Math.Clamp(axisPos.Y, -1, 1);
        }

        #region Operators

        /// <summary>
        /// The equals operator.
        /// </summary>
        /// <param name="left">The left side of the equation.</param>
        /// <param name="right">he right side of the equation.</param>
        /// <returns>True if both sides are equal.</returns>
        [MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
        public static bool operator ==(OVRControllerAxis left, OVRControllerAxis right)
        {
            if (left.X == right.X && left.Y == right.Y)
                return true;

            return false;
        }

        /// <summary>
        /// The not equals operator.
        /// </summary>
        /// <param name="left">The left side of the equation.</param>
        /// <param name="right">he right side of the equation.</param>
        /// <returns>True if both sides are not equal.</returns>
        [MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
        public static bool operator !=(OVRControllerAxis left, OVRControllerAxis right)
        {
            return !(left == right);
        }

        #endregion
        #region IEquatable Implementation

        /// <summary>
        /// The equals method.
        /// </summary>
        /// <param name="obj">The object to be compared.</param>
        /// <returns>True if the passed in value is equal to the oiginal value.</returns>
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        public override bool Equals(object obj)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        {

            switch (obj)
            {
                case OVRControllerAxis:
                    return this == (OVRControllerAxis)obj;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Retreiving the hash code for this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        /// <summary>
        /// Converting this object to a string.
        /// </summary>
        /// <returns>A string describing this object.</returns>
        public override string ToString()
        {
            return "[OVRControllerAxis: Axis= [" + X.ToString() + ", " + Y.ToString() + "]";
        }

        #endregion
    }

    #endregion
    #region OVRControllerTrigger

    /// <summary>
    /// Holds a trigger value.
    /// </summary>
    public struct OVRControllerTrigger
    {
        #region Public Properties

        /// <summary>
        /// The trigger for this value.
        /// </summary>
        public float Trigger { get; private set; }

        #endregion

        /// <summary>
        /// Creates an instance of the OVRControllerTrigger class.
        /// </summary>
        /// <param name="trigger">The axis value to set.</param>
        public OVRControllerTrigger(float trigger)
        {
            this = default;
            Trigger = MathHelper.Clamp(trigger, 0f, 1f);
        }

        #region Operators

        /// <summary>
        /// The equals operator.
        /// </summary>
        /// <param name="left">The left side of the equation.</param>
        /// <param name="right">he right side of the equation.</param>
        /// <returns>True if both sides are equal.</returns>
        [MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
        public static bool operator ==(OVRControllerTrigger left, OVRControllerTrigger right)
        {
            if (left.Trigger == right.Trigger)
            {
                return left.Trigger == right.Trigger;
            }

            return false;
        }

        /// <summary>
        /// The not equals operator.
        /// </summary>
        /// <param name="left">The left side of the equation.</param>
        /// <param name="right">he right side of the equation.</param>
        /// <returns>True if both sides are not equal.</returns>
        [MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
        public static bool operator !=(OVRControllerTrigger left, OVRControllerTrigger right)
        {
            return !(left == right);
        }

        #endregion
        #region IEquatable Implementation

        /// <summary>
        /// The equals method.
        /// </summary>
        /// <param name="obj">The object to be compared.</param>
        /// <returns>True if the passed in value is equal to the oiginal value.</returns>
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        public override bool Equals(object obj)
        {
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
            if (obj is OVRControllerTrigger trigger)
                return this == trigger;

            return false;
        }

        /// <summary>
        /// Retreiving the hash code for this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Trigger.GetHashCode();
        }

        /// <summary>
        /// Converting this object to a string.
        /// </summary>
        /// <returns>A string describing this object.</returns>
        public override string ToString()
        {
            return "[OVRControllerTrigger: Trigger=" + Trigger + "]";
        }

        #endregion
    }

    #endregion
    #region OVRControllerExt

    /// <summary>
    /// Extension class tha adds static methods for OpenVRController classes.
    /// </summary>
    static public class OVRControllerExt
    {
        #region ToVector2 (VRControllerAxis_t)

        static internal Vector2 ToVector2(this VRControllerAxis_t axis)
        {
            return new Vector2(axis.x, axis.y);
        }

        #endregion
        #region ToOVRControllerAxis (Vector2)

        static internal OVRControllerAxis ToOVRControllerAxis(this Vector2 axis)
        {
            return new OVRControllerAxis(axis);
        }

        #endregion
        #region ToOVRControllerTrigger (float)

        static internal OVRControllerTrigger ToOVRControllerTrigger(this float trigger)
        {
            return new OVRControllerTrigger(trigger);
        }

        #endregion
    }

    #endregion
}
