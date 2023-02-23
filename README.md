# MerjTek.MonoGame.OpenVr

MerjTek.MonoGame.OpenVr is a library that works on WIndows to add OpenVR (SteamVR) abilities to a MonoGame project.


## ToDo List
* Controllers
    * Update the OVRControllerProfile to include axis information for the touchpad, joystick, and or triggers
    * Update the bits of each button in every profile once I obtain the correct information.


## Nuget Packages
-[ MerjTek.MonoGame.OpenVr.DirectX ](https://www.nuget.org/packages/MerjTek.MonoGame.OpenVr.DirectX) v1.0.0  
-[ MerjTek.MonoGame.OpenVr.OpenGL ](https://www.nuget.org/packages/MerjTek.MonoGame.OpenVr.OpenGL) v1.0.0  


## How To Use
__After Installing the Nuget Package:__

        ...
        // In Game1.cs add the using statement below:
	    using MerjTek.MonoGame.OpenVr;
        ...
	

        ...
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            // DirectX must use the HiDef profile. OpenGL can use both profiles.
            _graphics.GraphicsProfile = GraphicsProfile.HiDef; 
        ...


        ...
        // Change the Game1 base class definiton:

        // Original (From)
        public class Game1 : Game

        // Updated (To)
        public class Game1 : VrGame
        ...


        ...
        // In the Draw method
        protected override void Draw(GameTime gameTime)
        {
            // Drawing code here
            
            // NOTE: The NearPlane, FarPlane, and ViewMatrix properties must be  
            // set anywhere before base.Draw is called.

            base.Draw(gameTime);
            
            // OPTIONAL: Render the left eye to the screen
            _spriteBatch.Begin();
            _spriteBatch.Draw(LeftEyeTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }
        ...


        ...
        // Add and implement DrawScene:

        /// <summary>
        /// Renders the scene for one eye. Will be called twice.
        /// </summary>
        /// <param name="eyeView">The view matrix adjusted per eye. (DO NOT MODIFY)</param>
        /// <param name="eyeProjection">The projection matrix adjusted per eye. (DO NOT MODIFY)</param>
        protected override void DrawScene(Matrix eyeView, Matrix eyeProjection)
        {
            // NOTE: Implement scene drawing code here
        }
        ...


## Caveats

* Make sure you are using the HiDef profile for DirectX. OpenGL can use both the Reach and the HiDef profiles.


## Building

* No problems building in Visual Studio 2022.
* The nuget for OpenGL after building contains a dependency on Monogame.Framework.WindowsDX. I have to use the .nupkg editing tool to correct the dependency to Monogame.Framework.OpenGL.


## License

The MerjTek.MonoGame.OpenVr project is under the [MIT License](https://github.com/MerjTek/MerjTek.MonoGame.PostProcessing/blob/main/LICENSE).
