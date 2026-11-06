using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;

namespace Amiguitos_Fighthers;

public sealed class EscenaSeleccionPersonaje : Scene
{
    private Texture2D _fondo;
    private Texture2D[] _tarjetas;
    private SpriteFont _fuente;

    // Chufo comienza seleccionado para el jugador 1.
    private int _indiceJugador1;

    // Rana Magica comienza seleccionada para el jugador 2.
    private int _indiceJugador2 = 3;

    public override void LoadContent()
    {
        _fondo = Content.Load<Texture2D>("PersonajeTarjetas/fondo_2x");
        _fuente = Content.Load<SpriteFont>("SpriteFonts/Default");
        _tarjetas = new Texture2D[PersonajeDefinicion.Personajes.Count];

        for (int i = 0; i < _tarjetas.Length; i++)
        {
            PersonajeSeleccion personaje = PersonajeDefinicion.Personajes[i];
            _tarjetas[i] = Content.Load<Texture2D>(personaje.ImagenTarjeta);
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.W))
        {
            _indiceJugador1 = SiguienteIndice(_indiceJugador1, -1);
        }
        else if (Core.Input.Keyboard.WasKeyJustPressed(Keys.S))
        {
            _indiceJugador1 = SiguienteIndice(_indiceJugador1, 1);
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Up))
        {
            _indiceJugador2 = SiguienteIndice(_indiceJugador2, -1);
        }
        else if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Down))
        {
            _indiceJugador2 = SiguienteIndice(_indiceJugador2, 1);
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
        {
            Core.ChangeScene(new EscenaCombate(
                PersonajeDefinicion.Personajes[_indiceJugador1],
                PersonajeDefinicion.Personajes[_indiceJugador2]));
        }
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice graphicsDevice = Core.GraphicsDevice;
        int anchoPantalla = graphicsDevice.Viewport.Width;
        int altoPantalla = graphicsDevice.Viewport.Height;

        graphicsDevice.Clear(Color.Black);
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        Core.SpriteBatch.Draw(_fondo, graphicsDevice.Viewport.Bounds, Color.White);
        DrawJugador(_indiceJugador1, 0, anchoPantalla, altoPantalla);
        DrawJugador(_indiceJugador2, 1, anchoPantalla, altoPantalla);

        const string controles = "J1: W / S     ENTER: JUGAR     J2: FLECHAS ARRIBA / ABAJO";
        Vector2 tamanoControles = _fuente.MeasureString(controles);
        float escalaControles = MathHelper.Min(
            1f,
            anchoPantalla * 0.4f / tamanoControles.X);
        Core.SpriteBatch.DrawString(
            _fuente,
            controles,
            new Vector2(anchoPantalla / 2f, altoPantalla * 0.955f),
            Color.White,
            0f,
            tamanoControles / 2f,
            escalaControles,
            SpriteEffects.None,
            0f);
        Core.SpriteBatch.End();
    }

    private void DrawJugador(int indice, int jugador, int anchoPantalla, int altoPantalla)
    {
        Texture2D tarjeta = _tarjetas[indice];
        float rotacion = MathHelper.ToRadians(jugador == 0 ? -15f : 15f);
        float escala = MathHelper.Min(
            anchoPantalla * 0.5f / tarjeta.Width,
            altoPantalla * 1f / tarjeta.Height);
        Vector2 centroTarjeta = new(
            anchoPantalla * (jugador == 0 ? 0.21f : 0.79f),
            altoPantalla * 0.50f);

        Core.SpriteBatch.Draw(
            tarjeta,
            centroTarjeta,
            null,
            Color.White,
            rotacion,
            new Vector2(tarjeta.Width / 2f, tarjeta.Height / 2f),
            escala,
            SpriteEffects.None,
            0f);
    }

    private static int SiguienteIndice(int indiceActual, int cambio)
    {
        int cantidadPersonajes = PersonajeDefinicion.Personajes.Count;
        return (indiceActual + cambio + cantidadPersonajes) % cantidadPersonajes;
    }
}
