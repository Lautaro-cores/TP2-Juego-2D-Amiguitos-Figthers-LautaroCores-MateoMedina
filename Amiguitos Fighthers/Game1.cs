using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Physics;

namespace Amiguitos_Fighthers;

public class Game1 : Core
{

    private AnimatedSprite Pose1;
    private AnimatedSprite PJ1;

    private AnimatedSprite PJ2;

    private CollisionBody collisionP1;

    private CollisionBody collisionP2;

    //para la verificacion de la tecla
    private bool _wasJDown;
    private const float Velocidad = 5.0f;

    public Game1() : base("Amiguitos Fighthers", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {

        TextureAtlas atlaspj = TextureAtlas.FromFile(Content, "pruebas/atlaspj.xml");

        //animacion piña
        Pose1 = atlaspj.CreateAnimatedSprite("Pose-animacion");
        Pose1.Stop();
        Pose1.Scale = new Vector2(3.0f, 3.0f);

        PJ1 = atlaspj.CreateAnimatedSprite("Pose1-animacion");
        PJ1.Scale = new Vector2(3.0f, 3.0f);

        PJ2 = atlaspj.CreateAnimatedSprite("Pose1-animacion");
        PJ2.Scale = new Vector2(3.0f, 3.0f);
        PJ2.Effects = SpriteEffects.FlipHorizontally;

        collisionP1 = new CollisionBody(PJ1, new Vector2(200.0f, 300.0f));
        collisionP2 = new CollisionBody(PJ2, new Vector2(900.0f, 300.0f));
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();


        // Check for keyboard input and handle it.

        if (Pose1.IsPlaying)
        {
            Pose1.Update(gameTime);
        }
        else
        {
            PJ1.Update(gameTime);
        }




        PJ2.Update(gameTime);
        // TODO: Add your update logic here

        base.Update(gameTime);
        CheckKeyboardInput();

        Rectangle screenBounds = new Rectangle(
    0,
    0,
    GraphicsDevice.PresentationParameters.BackBufferWidth,
    GraphicsDevice.PresentationParameters.BackBufferHeight
);

        collisionP1.KeepInside(screenBounds);
        collisionP2.KeepInside(screenBounds);

    }

    private void CheckKeyboardInput()
    {
        Vector2 movementP1 = Vector2.Zero;
        Vector2 movementP2 = Vector2.Zero;

        if (Input.Keyboard.IsKeyDown(Keys.W)) movementP1.Y -= Velocidad;
        if (Input.Keyboard.IsKeyDown(Keys.S)) movementP1.Y += Velocidad;
        if (Input.Keyboard.IsKeyDown(Keys.A)) movementP1.X -= Velocidad;
        if (Input.Keyboard.IsKeyDown(Keys.D)) movementP1.X += Velocidad;

        if (Input.Keyboard.IsKeyDown(Keys.Up)) movementP2.Y -= Velocidad;
        if (Input.Keyboard.IsKeyDown(Keys.Down)) movementP2.Y += Velocidad;
        if (Input.Keyboard.IsKeyDown(Keys.Left)) movementP2.X -= Velocidad;
        if (Input.Keyboard.IsKeyDown(Keys.Right)) movementP2.X += Velocidad;


        //aca verifico que la tecla este apretada y arranco la animacion sin el loop
        bool isJDown = Input.Keyboard.IsKeyDown(Keys.J);
        if (isJDown && !_wasJDown && !Pose1.IsPlaying)
        {
            Pose1.Play(loop: false);
        }

        _wasJDown = isJDown;

        collisionP1.Move(movementP1, collisionP2);
        collisionP2.Move(movementP2, collisionP1);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the slime texture region at a scale of 4.0
        //  _slime.Draw(SpriteBatch, Vector2.Zero);

        // Draw the bat sprite 10px to the right of the slime.
        //    _bat.Draw(SpriteBatch, new Vector2(_slime.Width + 10, 0));


        AnimatedSprite sprite = Pose1.IsPlaying
            ? Pose1
            : PJ1;
        sprite.Draw(SpriteBatch, collisionP1.Position);



        PJ2.Draw(SpriteBatch, collisionP2.Position);
        // Always end the sprite batch when finished.
        SpriteBatch.End();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
