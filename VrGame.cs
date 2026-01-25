using System;
using System.Runtime.CompilerServices;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Valve.VR;
using XNA = Microsoft.Xna.Framework;

namespace MerjTek.MonoGame.OpenVr
{
    /// <summary>
    /// Derives from the MonoGame Game class to add OpenVR functionaity.
    /// </summary>
    public partial class VrGame : Game
    {
        #region Private Variables

        private IntPtr leftPtr;
        private IntPtr rightPtr;

        private RenderTarget2D leftEyeTarget;
        private RenderTarget2D rightEyeTarget;

        private float nearPlane;
        private float farPlane;

        private Matrix viewMatrix;

        #endregion
        #region Public Properties

        /// <summary>
        /// The value for the near plane.
        /// </summary>
        public float NearPlane
        {
            get { return nearPlane; }
            set { nearPlane = value; }
        }

        /// <summary>
        /// The value for the far plane.
        /// </summary>
        public float FarPlane
        {
            get { return farPlane; }
            set { farPlane = value; }
        }

        /// <summary>
        /// The RenderTarget for the left eye.
        /// </summary>
        public RenderTarget2D LeftEyeTarget { get { return leftEyeTarget; } }

        /// <summary>
        /// The Left Controller.
        /// </summary>
        public OVRController LeftController { get { return VrDevice.LeftController; } }

        /// <summary>
        /// The Right Controller.
        /// </summary>
        public OVRController RightController { get { return VrDevice.RightController; } }

        /// <summary>
        /// The original camera's view matrix.
        /// </summary>
        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
            set { viewMatrix = value; }
        }

        /// <summary>
        /// The OpenVR device instance.
        /// </summary>
        public OVRDevice VrDevice { get; private set; }

        /// <summary>
        /// The OpenVR device Submit error (Left Eye).
        /// </summary>
        public EVRCompositorError SubmitErrorL;

        /// <summary>
        /// The OpenVR device Submit error (Right Eye).
        /// </summary>
        public EVRCompositorError SubmitErrorR;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the VrGame class.
        /// </summary>
        public VrGame() : base()
        {
            #region Null Variables

            leftEyeTarget = null;
            rightEyeTarget = null;

            #endregion

            VrDevice = OVRDevice.Get();

            nearPlane = 0;
            farPlane = 100000;

            viewMatrix = Matrix.Identity;
        }

        #endregion
        #region Deconstructor

        /// <summary>
        /// Deinitializes the instance of the VrGame class.
        /// </summary>
        ~VrGame()
        {
            VrDevice.ShutDown();
            leftEyeTarget.Dispose();
            rightEyeTarget.Dispose();
        }

        #endregion

        #region Overridable MonoGame Methods

        #region Initialize (Override)

        /// <summary>
        /// Initialize the game.
        /// </summary>
        protected override void Initialize()
        {
            VrDevice.Initialize();
            base.Initialize();

            VrDevice.GetRecommendedRenderTargetSize(out int w, out int h);
            leftEyeTarget = new RenderTarget2D(GraphicsDevice, w, h);
            rightEyeTarget = new RenderTarget2D(GraphicsDevice, w, h);

            SetPlatformTextures();
        }

        #endregion
        #region Update (Override)

        /// <summary>
        /// Called when the game should update.
        /// </summary>
        /// <param name="gameTime">The elapsed time since the last call to Update.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            VrDevice.Update();
        }

        #endregion
        #region UnloadContent (Override)

        /// <summary>
        /// Unload graphical resources loaded by the game.
        /// </summary>
        protected override void UnloadContent()
        {
            VrDevice?.ShutDown();

            leftEyeTarget?.Dispose();
            rightEyeTarget?.Dispose();

            leftPtr = IntPtr.Zero;
            rightPtr = IntPtr.Zero;

            base.UnloadContent();
        }

        #endregion
        #region Draw (Override)

        /// <summary>
        /// Called when the game should draw a frame.
        /// </summary>
        /// <param name="gameTime">A instance containing the elapsed time since the last call to Draw and the total time elapsed since the game started.</param>
        protected override void Draw(GameTime gameTime)
        {
            DrawVr();
            base.Draw(gameTime);
        }

        #endregion

        #endregion

        #region DrawScene (Protected)

        /// <summary>
        /// Renders the scene for one eye. Will be called twice.
        /// </summary>
        /// <param name="eyeView">The view matrix adjusted per eye. (DO NOT MODIFY)</param>
        /// <param name="eyeProjection">The projection matrix adjusted per eye. (DO NOT MODIFY)</param>
        virtual protected void DrawScene(Matrix eyeView,
                                        Matrix eyeProjection)
        {
            // NOTE: Render the scene.
        }

        #endregion

        #region Helper Methods

        #region DrawVr (Private)

        private void DrawVr()
        {
            VrDevice.WaitGetPoses();

            Matrix nextPose = VrDevice.Display.GetNextPose();

            GraphicsDevice.GetRenderTargets();
            GraphicsDevice.SetRenderTarget(leftEyeTarget);
            DrawEye(EVREye.Eye_Left, nextPose);

            GraphicsDevice.SetRenderTarget(rightEyeTarget);
            DrawEye(EVREye.Eye_Right, nextPose);

            GraphicsDevice.SetRenderTarget(null);

            SubmitErrorL = VrDevice.Submit(EVREye.Eye_Left, leftPtr);
            SubmitErrorR = VrDevice.Submit(EVREye.Eye_Right, rightPtr);
        }

        #endregion
        #region DrawEye (Private)

        private void DrawEye(EVREye eye,
                             Matrix pose)
        {
            VrDevice.GetViewAndProjectionMatricesForEye(eye,
                                                        nearPlane,
                                                        farPlane,
                                                        out Matrix view,
                                                        out Matrix projection);

            DrawScene(viewMatrix * Matrix.Invert(pose * view), projection);
        }

        #endregion
        #region RenderLeftEyeToScreen (Public)

        /// <summary>
        /// OPTIONAL: Render the left eye to the screen
        /// </summary>
        /// <param name="spriteBatch"><i>The <see cref="SpriteBatch"/> to use to render the left eye.</i></param>
        public void RenderLeftEyeToScreen(
            SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(leftEyeTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// An extension class to convert OpenVR strutures to MonoGame equivalents.
    /// </summary>
    static public class OpenVrExt
    {
        #region ToMatrix (Hmd34ToMG)

        /// <summary>
        /// Converts from an OpenVR column based matrix (HmdMatrix34_t) to 
        /// the MonoGame row based matrix (Matrix)
        /// </summary>
        /// <param name="mat">The matrix to be converted.</param>
        /// <returns>A MonoGame row-based Matrix.</returns>
        static public Matrix ToMatrix(this HmdMatrix34_t mat)
        {
            var m = new Matrix(
                mat.m0, mat.m4, mat.m8, 0.0f,
                mat.m1, mat.m5, mat.m9, 0.0f,
                mat.m2, mat.m6, mat.m10, 0.0f,
                mat.m3, mat.m7, mat.m11, 1.0f);

            return m;
        }

        #endregion
        #region ToMatrix (Hmd44ToMG)

        /// <summary>
        /// Converts from an OpenVR column based matrix (HmdMatrix44_t) to 
        /// the MonoGame row based matrix (Matrix)
        /// </summary>
        /// <param name="mat">The matrix to be converted.</param>
        /// <returns>A MonoGame row-based Matrix.</returns>
        static public Matrix ToMatrix(this HmdMatrix44_t mat)
        {
            var m = new Matrix(
                mat.m0, mat.m4, mat.m8, mat.m12,
                mat.m1, mat.m5, mat.m9, mat.m13,
                mat.m2, mat.m6, mat.m10, mat.m14,
                mat.m3, mat.m7, mat.m11, mat.m15);

            return m;
        }

        #endregion
        #region Vector3 (HmdVector3_t)

        /// <summary>
        /// Converts from an OpenVR based vector3 (HmdVector3_t) to 
        /// the MonoGame vector3 (Vector3)
        /// </summary>
        /// <param name="vector3">The vector3 to be converted.</param>
        /// <returns>A MonoGame based Vector3.</returns>
        static public XNA.Vector3 ToVector3(this HmdVector3_t vector3)
        {
            return new XNA.Vector3(vector3.v0, vector3.v1, vector3.v2);
        }

        #endregion
        #region GetRotation (Matrix)

        #region _copysign (Private)

        private static float _copysign(float sizeval, float signval)
        {
            return Math.Sign(signval) == 1 ? Math.Abs(sizeval) : -Math.Abs(sizeval);
        }

        #endregion

        /// <summary>
        /// Gets the rotation Quaternion from a atrix
        /// </summary>
        /// <param name="matrix">The Matrix from where the rotation is to extracted.</param>
        /// <returns>The quaternion holding the rotation.</returns>
        public static XNA.Quaternion GetRotation(this Matrix matrix)
        {
            XNA.Quaternion q = new XNA.Quaternion();

            q.W = (float)Math.Sqrt(Math.Max(0, 1 + matrix.M11 + matrix.M22 + matrix.M33)) / 2;
            q.X = (float)Math.Sqrt(Math.Max(0, 1 + matrix.M11 - matrix.M22 - matrix.M33)) / 2;
            q.Y = (float)Math.Sqrt(Math.Max(0, 1 - matrix.M11 + matrix.M22 - matrix.M33)) / 2;
            q.Z = (float)Math.Sqrt(Math.Max(0, 1 - matrix.M11 - matrix.M22 + matrix.M33)) / 2;
            q.X = _copysign(q.X, matrix.M32 - matrix.M23);
            q.Y = _copysign(q.Y, matrix.M13 - matrix.M31);
            q.Z = _copysign(q.Z, matrix.M21 - matrix.M12);

            return q;
        }

        #endregion
    }
}
