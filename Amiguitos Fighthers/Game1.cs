using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace Amiguitos_Fighthers;

public class Game1 : Core
{
    private Jugador _jugador1;
    private Jugador _jugador2;
    private Texture2D _pixel;
    private BarraDeVida _barraVidaJugador1;
    private BarraDeVida _barraVidaJugador2;

    public Game1() : base("Amiguitos Fighthers", 1280, 720, false)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        TextureAtlas atlaspj = TextureAtlas.FromFile(Content, "pruebas/atlaspj.xml");

        _jugador1 = new Jugador(
            PersonajeDefinicion.CrearGuerrero(atlaspj),
            atlaspj,
            new Vector2(200.0f, 300.0f),
            Keys.W,
            Keys.S,
            Keys.A,
            Keys.D,
            Keys.J,
            Keys.K
            );

        _jugador2 = new Jugador(
            PersonajeDefinicion.CrearGuerrero(atlaspj),
            atlaspj,
            new Vector2(900.0f, 300.0f),
            Keys.Up,
            Keys.Down,
            Keys.Left,
            Keys.Right,
            Keys.Space,
            Keys.M,
            espejo: true);

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);

        Rectangle screenBounds = new Rectangle(
            0,
            0,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight);

        // Se actualiza a cada jugador con su rival correspondiente.
        // Esto es lo que permite que el ataque de jugador1 detecte a jugador2
        // y viceversa, sin depender de una única instancia de colisión.
        _jugador1.Update(gameTime, screenBounds, _jugador2);
        _jugador2.Update(gameTime, screenBounds, _jugador1);
    }

    protected override void Draw(GameTime gameTime)
    {
        CrearBarrasDeVida();

        GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _jugador1.Draw(SpriteBatch);
        _jugador2.Draw(SpriteBatch);
        _barraVidaJugador1.Draw(SpriteBatch);
        _barraVidaJugador2.Draw(SpriteBatch);

        SpriteBatch.End();
        base.Draw(gameTime);
    }

    private void CrearBarrasDeVida()
    {
        if (_pixel != null)
        {
            return;
        }

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        const int anchoBarra = 500;
        const int altoBarra = 30;
        const int margen = 32;
        _barraVidaJugador1 = new BarraDeVida(
            _jugador1,
            _pixel,
            new Rectangle(margen, margen, anchoBarra, altoBarra));
        _barraVidaJugador2 = new BarraDeVida(
            _jugador2,
            _pixel,
            new Rectangle(
                GraphicsDevice.PresentationParameters.BackBufferWidth - margen - anchoBarra,
                margen,
                anchoBarra,
                altoBarra));
    }

    protected override void UnloadContent()
    {
        _pixel.Dispose();
        base.UnloadContent();
    }
}
