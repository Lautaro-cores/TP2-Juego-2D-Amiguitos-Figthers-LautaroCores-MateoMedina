using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;

namespace Amiguitos_Fighthers;

public sealed class EscenaCombate : Scene
{
    private const int AnchoEscenario = 2010;

    private EstadoCombate _estadoCombate;
    private Jugador _jugador1;
    private Jugador _jugador2;
    private Texture2D _fondo;
    private Texture2D _pixel;
    private SpriteFont _fuente;
    private float _camaraX;
    private BarraDeVida _barraVidaJugador1;
    private BarraDeVida _barraVidaJugador2;

    public override void LoadContent()
    {
        _estadoCombate = new EstadoCombate();
        TextureAtlas atlaspj = TextureAtlas.FromFile(Content, "pruebas/atlaspj.xml");
        _fondo = Content.Load<Texture2D>("pruebas/bufet");
        _fuente = Content.Load<SpriteFont>("SpriteFonts/Default");

        const float posicionInicialJugador1 = 340f;
        const float posicionInicialJugador2 = 900f;
        float desplazamientoInicial = AnchoEscenario / 2f -
            (posicionInicialJugador1 + posicionInicialJugador2) / 2f;

        _jugador1 = new Jugador(
            PersonajeDefinicion.CrearGuerrero(atlaspj),
            atlaspj,
            new Vector2(posicionInicialJugador1 + desplazamientoInicial, 500.0f),
            Keys.W,
            Keys.S,
            Keys.A,
            Keys.D,
            Keys.J);

        _jugador2 = new Jugador(
            PersonajeDefinicion.CrearGuerrero(atlaspj),
            atlaspj,
            new Vector2(posicionInicialJugador2 + desplazamientoInicial, 500.0f),
            Keys.Up,
            Keys.Down,
            Keys.Left,
            Keys.Right,
            Keys.Space,
            espejo: true);

        _pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
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
                Core.GraphicsDevice.Viewport.Width - margen - anchoBarra,
                margen,
                anchoBarra,
                altoBarra));
    }

    public override void Update(GameTime gameTime)
    {
        int anchoPantalla = Core.GraphicsDevice.Viewport.Width;
        int altoPantalla = Core.GraphicsDevice.Viewport.Height;
        int anchoEscenario = Math.Max(anchoPantalla, AnchoEscenario);
        Rectangle limitesCamara = new((int)_camaraX, 0, anchoPantalla, altoPantalla);

        _estadoCombate.Actualizar(gameTime);
        _jugador1.Update(gameTime, limitesCamara, _jugador2, _estadoCombate);
        _jugador2.Update(gameTime, limitesCamara, _jugador1, _estadoCombate);

        float puntoMedio = (_jugador1.Collision.Bounds.Center.X + _jugador2.Collision.Bounds.Center.X) / 2f;
        _camaraX = Math.Clamp(puntoMedio - anchoPantalla / 2f, 0f, anchoEscenario - anchoPantalla);

        limitesCamara = new Rectangle((int)_camaraX, 0, anchoPantalla, altoPantalla);
        _jugador1.Collision.KeepInside(limitesCamara);
        _jugador2.Collision.KeepInside(limitesCamara);

        if (_jugador1.GolpeIniciado || _jugador2.GolpeIniciado)
        {
            _estadoCombate.ActivarGolpe();
        }
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice graphicsDevice = Core.GraphicsDevice;
        int anchoPantalla = graphicsDevice.Viewport.Width;
        int altoPantalla = graphicsDevice.Viewport.Height;
        int anchoEscenario = Math.Max(anchoPantalla, AnchoEscenario);
        Matrix transformacionCamara = Matrix.CreateTranslation(-_camaraX, 0f, 0f);

        graphicsDevice.Clear(Color.CornflowerBlue);
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: transformacionCamara);
        Core.SpriteBatch.Draw(_fondo, new Rectangle(0, 0, anchoEscenario, altoPantalla), Color.White);
        _jugador1.Draw(Core.SpriteBatch);
        _jugador2.Draw(Core.SpriteBatch);
        Core.SpriteBatch.End();

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _barraVidaJugador1.Draw(Core.SpriteBatch);
        _barraVidaJugador2.Draw(Core.SpriteBatch);
        Core.SpriteBatch.DrawString(_fuente, "JUGADOR 1", new Vector2(32, 68), Color.White);
        Core.SpriteBatch.DrawString(
            _fuente,
            "JUGADOR 2",
            new Vector2(anchoPantalla - 532, 68),
            Color.White);
        Core.SpriteBatch.End();
    }

    public override void UnloadContent()
    {
        _pixel?.Dispose();
        base.UnloadContent();
    }
}
