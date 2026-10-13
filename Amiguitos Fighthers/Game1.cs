using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace Amiguitos_Fighthers;

public class Game1 : Core
{

    private AnimatedSprite Pose1;
    private AnimatedSprite PJ1;

    private AnimatedSprite PJ2;

    private Vector2 positionP1;

    private Vector2 positionP2;

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


        Pose1 = atlaspj.CreateAnimatedSprite ("Pose-animacion");
        Pose1.Scale = new Vector2(3.0f, 3.0f);

        PJ1 = atlaspj.CreateAnimatedSprite ("Pose1-animacion");
        PJ1.Scale = new Vector2(3.0f, 3.0f);

        PJ2 = atlaspj.CreateAnimatedSprite ("Pose1-animacion");
        PJ2.Scale = new Vector2(3.0f, 3.0f);
        PJ2.Effects = SpriteEffects.FlipHorizontally;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Pose1.Update(gameTime);

        PJ1.Update(gameTime);
        PJ2.Update(gameTime);
        // TODO: Add your update logic here

        base.Update(gameTime);
        CheckKeyboardInput();   
    }

  private void CheckKeyboardInput()
    {
        // If the space key is held down, the movement speed increases by 1.5
        float speed = Velocidad;

        // If the W or Up keys are down, move the slime up on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.W))
        {
            positionP1.Y -= speed;
        }

        // if the S or Down keys are down, move the slime down on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.S))
        {
            positionP1.Y += speed;
        }

        // If the A or Left keys are down, move the slime left on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.A))
        {
            positionP1.X -= speed;
        }

        // If the D or Right keys are down, move the slime right on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.D))
        {
            positionP1.X += speed;
        }

        // If the W or Up keys are down, move the slime up on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.Up))
        {
            positionP2.Y -= speed;
        }

        // if the S or Down keys are down, move the slime down on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.Down))
        {
            positionP2.Y += speed;
        }

        // If the A or Left keys are down, move the slime left on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.Left))
        {
            positionP2.X -= speed;
        }

        // If the D or Right keys are down, move the slime right on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.Right))
        {
            positionP2.X += speed;
        }
    }
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the slime texture region at a scale of 4.0
     //  _slime.Draw(SpriteBatch, Vector2.Zero);

        // Draw the bat sprite 10px to the right of the slime.
    //    _bat.Draw(SpriteBatch, new Vector2(_slime.Width + 10, 0));
        
        Pose1.Draw(SpriteBatch, Vector2.Zero);
        
        PJ1.Draw(SpriteBatch, positionP1);
        PJ2.Draw(SpriteBatch, positionP2);
        // Always end the sprite batch when finished.
        SpriteBatch.End();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
