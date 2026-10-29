using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace Amiguitos_Fighthers;

public class Game1 : Core
{
    private EstadoCombate _estadoCombate;


    private Jugador _jugador1;
    private Jugador _jugador2;
    private Texture2D _fondo;
    private Texture2D _pixel;
    private BarraDeVida _barraVidaJugador1;
    private BarraDeVida _barraVidaJugador2;

    public Game1() : base("Amiguitos Fighthers", 1280, 720, false)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
        _estadoCombate = new EstadoCombate();

    }

    protected override void LoadContent()
    {
        TextureAtlas atlaspj = TextureAtlas.FromFile(Content, "pruebas/atlaspj.xml");
        // La textura se carga como contenido administrado y se reutiliza como fondo.
        _fondo = Content.Load<Texture2D>("pruebas/bufet");

        _jugador1 = new Jugador(
            PersonajeDefinicion.CrearGuerrero(atlaspj),
            atlaspj,
            new Vector2(200.0f, 500.0f),
            Keys.W,
            Keys.S,
            Keys.A,
            Keys.D,
            Keys.J
            );

        _jugador2 = new Jugador(
            PersonajeDefinicion.CrearGuerrero(atlaspj),
            atlaspj,
            new Vector2(900.0f, 500.0f),
            Keys.Up,
            Keys.Down,
            Keys.Left,
            Keys.Right,
            Keys.Space,
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

        _estadoCombate.Actualizar(gameTime);

        // Se actualiza a cada jugador con su rival correspondiente.
        // Esto es lo que permite que el ataque de jugador1 detecte a jugador2
        // y viceversa, sin depender de una única instancia de colisión.
        // El estado compartido permite que ambos jugadores sepan si está activa la ventana de bloqueo.
        _jugador1.Update(gameTime, screenBounds, _jugador2, _estadoCombate);
        _jugador2.Update(gameTime, screenBounds, _jugador1, _estadoCombate);

        // Se activa al apretar el ataque, incluso aunque el golpe no alcance al rival.
        if (_jugador1.GolpeIniciado || _jugador2.GolpeIniciado)
        {
            _estadoCombate.ActivarGolpe();
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        CrearBarrasDeVida();

        GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // El fondo se dibuja primero para que jugadores y HUD queden por encima.
        SpriteBatch.Draw(_fondo, GraphicsDevice.Viewport.Bounds, Color.White);
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

        // Se crea durante el dibujo, cuando el GraphicsDevice de Core ya está inicializado.
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
