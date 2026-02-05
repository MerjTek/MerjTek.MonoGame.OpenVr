using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MerjTek.MonoGame.OpenVr;

namespace MGNamespace;

public class Game1 : VrGame
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            GraphicsProfile = GraphicsProfile.HiDef,
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // TODO: Set the NearPlane, FarPlane, and ViewMatrix
        // before callng base.Draw.

        // TODO: Add your non-VR drawing code here

        base.Draw(gameTime);

        // OPTIONAL: Render sometheing to the game window
        RenderLeftEyeToScreen(_spriteBatch);
    }

    protected override void DrawScene(Matrix eyeView, Matrix eyeProjection)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your VR drawing code here
    }
}
