using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Physics;

namespace Amiguitos_Fighthers;

public sealed class Jugador : Personaje
{
    private const int AlturaSuelo = 200;

    private readonly Keys _teclaSalto;
    private readonly Keys _teclaAbajo;                              
    private readonly Keys _teclaIzquierda;
    private readonly Keys _teclaDerecha;
    private readonly Keys _teclaAtaque;

    private readonly Keys _teclaBolaFuego;

    private readonly MovimientoBasico _movimientoBasico;
    private bool _mirandoDerecha = true;

    private enum EstadoAnimacion
    {
        Idle,
        Atacando,

        Lanzando
    }
    private readonly Dictionary<EstadoAnimacion, AnimatedSprite> _animaciones;

    private EstadoAnimacion _estadoAnimacion;

    public AnimatedSprite Sprite => _animaciones[_estadoAnimacion];

    public CollisionBody Collision { get; }

    public Vector2 Position => Collision.Position;

    public bool EstaAtacando => _estadoAnimacion == EstadoAnimacion.Atacando;

    public bool EstaLanzando => _estadoAnimacion == EstadoAnimacion.Lanzando;

    public PersonajeDefinicion Definicion { get; }

    public Jugador(
        PersonajeDefinicion definicion,
        TextureAtlas atlas,
        Vector2 posicionInicial,
        Keys salto,
        Keys abajo,
        Keys izquierda,
        Keys derecha,
        Keys ataque,
        Keys BolaFuego,
        bool espejo = false
        ) : base(definicion.VidaMaxima)
    {
        Definicion = definicion;
        _movimientoBasico = definicion.MovimientoBasico.CrearInstancia();
        _teclaSalto = salto;
        _teclaAbajo = abajo;
        _teclaIzquierda = izquierda;
        _teclaDerecha = derecha;
        _teclaAtaque = ataque;
        _teclaBolaFuego = BolaFuego;

        AnimatedSprite spriteIdle = atlas.CreateAnimatedSprite(definicion.AnimacionIdle);
        spriteIdle.Scale = new Vector2(3.0f, 3.0f);
        spriteIdle.Play(loop: true);

        AnimatedSprite spriteAtaque = atlas.CreateAnimatedSprite(definicion.AnimacionAtaque);
        spriteAtaque.Scale = new Vector2(3.0f, 3.0f);
        spriteAtaque.Stop();

        AnimatedSprite spriteLanzar = atlas.CreateAnimatedSprite(definicion.AnimacionLanzar);
        spriteLanzar.Scale = new Vector2(3.0f, 3.0f);
        spriteLanzar.Stop();

        if (espejo)
        {
            spriteIdle.Effects = SpriteEffects.FlipHorizontally;
            spriteAtaque.Effects = SpriteEffects.FlipHorizontally;
            spriteLanzar.Effects = SpriteEffects.FlipHorizontally;
        }

        _animaciones = new Dictionary<EstadoAnimacion, AnimatedSprite>
        {
            [EstadoAnimacion.Idle] = spriteIdle,
            [EstadoAnimacion.Atacando] = spriteAtaque,
            [EstadoAnimacion.Lanzando] = spriteLanzar
        };

        Collision = new CollisionBody(spriteIdle, posicionInicial);

        _mirandoDerecha = !espejo;
    }

    public void Update(GameTime gameTime, Rectangle bounds, Jugador otro)
    {
        // La validación de colisión se hace con el rival actual del jugador.
        // Como Game1 llama Update para jugador1 y luego para jugador2,
        // ambos jugadores chequearán su ataque contra el otro personaje.
        ProcesarEntrada(otro);
        ActualizarAnimacion(gameTime);

        Vector2 movimiento = ObtenerMovimiento();
        movimiento.Y = _movimientoBasico.ActualizarSalto(gameTime);
        Collision.Move(movimiento, otro.Collision);

        int sueloY = Math.Max(bounds.Top, bounds.Bottom - AlturaSuelo);
        Rectangle limitesEscenario = new(bounds.Left, bounds.Top, bounds.Width, sueloY - bounds.Top);
        Collision.KeepInside(limitesEscenario);

        if (Collision.Bounds.Bottom >= sueloY)
        {
            _movimientoBasico.Aterrizar();
        }

        _movimientoBasico.IntentarAtacar(Collision.Bounds, otro, otro.Collision.Bounds);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Sprite.Draw(spriteBatch, Position);
    }

    private void ProcesarEntrada(Jugador otro)
    {
        if (Core.Input.Keyboard.IsKeyDown(_teclaSalto))
        {
            _movimientoBasico.Saltar();
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(_teclaAtaque) &&
            _estadoAnimacion == EstadoAnimacion.Idle)
        {
            // Importante: la dirección del golpe no depende del sprite o del último movimiento.
            // Siempre debe mirarse al rival actual para que el jugador de la izquierda
            // ataque hacia la derecha cuando el enemigo está a su derecha.
            _mirandoDerecha = Position.X <= otro.Position.X;

            _movimientoBasico.IniciarAtaque(_mirandoDerecha);

            CambiarEstado(EstadoAnimacion.Atacando);
        }




        if (Core.Input.Keyboard.WasKeyJustPressed(_teclaBolaFuego) &&
            _estadoAnimacion == EstadoAnimacion.Idle)
        {
            CambiarEstado(EstadoAnimacion.Lanzando);
        }
    }

    private void ActualizarAnimacion(GameTime gameTime)
    {
        Sprite.Update(gameTime);

        if (_estadoAnimacion == EstadoAnimacion.Atacando && !Sprite.IsPlaying)
        {
            _movimientoBasico.FinalizarAtaque();
            CambiarEstado(EstadoAnimacion.Idle);
        }

        if (_estadoAnimacion == EstadoAnimacion.Lanzando && !Sprite.IsPlaying)
        {
            CambiarEstado(EstadoAnimacion.Idle);
        }
    }

    private void CambiarEstado(EstadoAnimacion nuevoEstado)
    {
        if (_estadoAnimacion == nuevoEstado)
        {
            return;
        }

        _estadoAnimacion = nuevoEstado;
        _animaciones[nuevoEstado].Play(loop: nuevoEstado == EstadoAnimacion.Idle);
    }

    private Vector2 ObtenerMovimiento()
    {
        Vector2 movimiento = Vector2.Zero;

        if (Core.Input.Keyboard.IsKeyDown(_teclaIzquierda)) movimiento.X -= _movimientoBasico.VelocidadMovimiento;
        if (Core.Input.Keyboard.IsKeyDown(_teclaDerecha)) movimiento.X += _movimientoBasico.VelocidadMovimiento;

        if (movimiento.X > 0)
        {
            _mirandoDerecha = true;
        }
        else if (movimiento.X < 0)
        {
            _mirandoDerecha = false;
        }

        return movimiento;
    }
}
