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
    private AnimatedSprite _slime;
    private AnimatedSprite _bat;    
    private AnimatedSprite Pose1;
    private AnimatedSprite Pose2;

    private Vector2 positionPose;

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
        TextureAtlas atlas = TextureAtlas.FromFile(Content, "pruebas/atlas-definition.xml");

        TextureAtlas atlaspj = TextureAtlas.FromFile(Content, "pruebas/atlaspj.xml");

            // Create the slime animated sprite from the atlas.
        _slime = atlas.CreateAnimatedSprite("slime-animation");
        _slime.Scale = new Vector2(4.0f, 4.0f);

        // Create the bat animated sprite from the atlas.
        _bat = atlas.CreateAnimatedSprite("bat-animation");
        _bat.Scale = new Vector2(4.0f, 4.0f);

        Pose1 = atlaspj.CreateAnimatedSprite ("Pose-animacion");
        Pose1.Scale = new Vector2(3.0f, 3.0f);

        Pose2 = atlaspj.CreateAnimatedSprite ("Pose1-animacion");
        Pose2.Scale = new Vector2(3.0f, 3.0f);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
             // Update the slime animated sprite.
        _slime.Update(gameTime);

        // Update the bat animated sprite.
        _bat.Update(gameTime);

        Pose1.Update(gameTime);

        Pose2.Update(gameTime);
        // TODO: Add your update logic here

        base.Update(gameTime);
        CheckKeyboardInput();   
    }

  private void CheckKeyboardInput()
    {
        // If the space key is held down, the movement speed increases by 1.5
        float speed = Velocidad;
        if (Input.Keyboard.IsKeyDown(Keys.Space))
        {
            speed *= 1.5f;
        }

        // If the W or Up keys are down, move the slime up on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.W) || Input.Keyboard.IsKeyDown(Keys.Up))
        {
            positionPose.Y -= speed;
        }

        // if the S or Down keys are down, move the slime down on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.S) || Input.Keyboard.IsKeyDown(Keys.Down))
        {
            positionPose.Y += speed;
        }

        // If the A or Left keys are down, move the slime left on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.A) || Input.Keyboard.IsKeyDown(Keys.Left))
        {
            positionPose.X -= speed;
        }

        // If the D or Right keys are down, move the slime right on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.D) || Input.Keyboard.IsKeyDown(Keys.Right))
        {
            positionPose.X += speed;
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

        Pose2.Draw(SpriteBatch, positionPose);
        // Always end the sprite batch when finished.
        SpriteBatch.End();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
