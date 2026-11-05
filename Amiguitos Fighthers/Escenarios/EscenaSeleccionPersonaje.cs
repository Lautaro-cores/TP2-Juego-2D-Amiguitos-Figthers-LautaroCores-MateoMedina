using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;

namespace Amiguitos_Fighthers;

public sealed class EscenaSeleccionPersonaje : Scene
{
    private Texture2D _fondo;
    private Texture2D[] _retratos;
    private Rectangle[] _areasRetrato;
    private Texture2D[] _nombres;
    private Rectangle[] _areasNombre;
    private SpriteFont _fuente;
    private int _indiceJugador1;
    private int _indiceJugador2 = 1;

    public override void LoadContent()
    {
        _fondo = Content.Load<Texture2D>("pruebas/selecio");
        _fuente = Content.Load<SpriteFont>("SpriteFonts/Default");
        _retratos = new Texture2D[PersonajeDefinicion.Personajes.Count];
        _areasRetrato = new Rectangle[_retratos.Length];
        _nombres = new Texture2D[_retratos.Length];
        _areasNombre = new Rectangle[_retratos.Length];

        for (int i = 0; i < _retratos.Length; i++)
        {
            PersonajeSeleccion personaje = PersonajeDefinicion.Personajes[i];
            _retratos[i] = Content.Load<Texture2D>(personaje.Imagen);
            _nombres[i] = Content.Load<Texture2D>(personaje.ImagenNombre);
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
        Texture2D retrato = _retratos[indice];
        Rectangle areaRetrato = _areasRetrato[indice];
        float rotacion = jugador == 0
            ? MathHelper.ToRadians(-15f)
            : MathHelper.ToRadians(15f);
        float seno = MathF.Abs(MathF.Sin(rotacion));
        float coseno = MathF.Abs(MathF.Cos(rotacion));
        float anchoRotado = areaRetrato.Width * coseno + areaRetrato.Height * seno;
        float altoRotado = areaRetrato.Width * seno + areaRetrato.Height * coseno;
        float escala = MathHelper.Min(
            anchoPantalla * 0.89f / anchoRotado,
            altoPantalla * 0.70f / altoRotado);
        Vector2 centroRetrato = new(
            anchoPantalla * (jugador == 0 ? 0.18f : 0.82f),
            altoPantalla * 0.42f);
        Vector2 origenRetrato = new(
            areaRetrato.Width / 2f,
            areaRetrato.Height / 2f);

        Core.SpriteBatch.Draw(
            retrato,
            centroRetrato,
            areaRetrato,
            Color.White,
            rotacion,
            origenRetrato,
            escala,
            SpriteEffects.None,
            0f);

        Texture2D nombre = _nombres[indice];
        Rectangle areaNombre = _areasNombre[indice];
        float anchoNombreRotado = areaNombre.Width * coseno + areaNombre.Height * seno;
        float altoNombreRotado = areaNombre.Width * seno + areaNombre.Height * coseno;
        float escalaNombre = MathHelper.Min(
            anchoPantalla * 0.29f / anchoNombreRotado,
            altoPantalla * 0.16f / altoNombreRotado);
        Vector2 centroNombre = new(
            anchoPantalla * (jugador == 0 ? 0.235f : 0.765f),
            altoPantalla * 0.79f);

        Core.SpriteBatch.Draw(
            nombre,
            centroNombre,
            areaNombre,
            Color.White,
            rotacion,
            new Vector2(areaNombre.Width / 2f, areaNombre.Height / 2f),
            escalaNombre,
            SpriteEffects.None,
            0f);
    }

    private static int SiguienteIndice(int indiceActual, int cambio)
    {
        int cantidadPersonajes = PersonajeDefinicion.Personajes.Count;
        return (indiceActual + cambio + cantidadPersonajes) % cantidadPersonajes;
    }
}
