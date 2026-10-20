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
    private const float Velocidad = 5.0f;

    private readonly Keys _teclaArriba;
    private readonly Keys _teclaAbajo;
    private readonly Keys _teclaIzquierda;
    private readonly Keys _teclaDerecha;
    private readonly Keys _teclaAtaque;

    private readonly Keys _teclaBolaFuego;

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
        Keys arriba,
        Keys abajo,
        Keys izquierda,
        Keys derecha,
        Keys ataque,
        Keys BolaFuego,
        bool espejo = false
        ) : base(definicion.VidaMaxima)
    {
        Definicion = definicion;
        _teclaArriba = arriba;
        _teclaAbajo = abajo;
        _teclaIzquierda = izquierda;
        _teclaDerecha = derecha;
        _teclaAtaque = ataque;
        _teclaBolaFuego = BolaFuego;

        AnimatedSprite spriteIdle = atlas.CreateAnimatedSprite(definicion.AnimacionIdle);
        spriteIdle.Scale = new Vector2(3.0f, 3.0f);

        AnimatedSprite spriteAtaque = atlas.CreateAnimatedSprite(definicion.AnimacionAtaque);
        spriteAtaque.Stop();
        spriteAtaque.Scale = new Vector2(3.0f, 3.0f);

        AnimatedSprite spriteLanzar = atlas.CreateAnimatedSprite(definicion.AnimacionLanzar);
        spriteLanzar.Stop();
        spriteLanzar.Scale = new Vector2(3.0f, 3.0f);

        


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
    }

    public void Update(GameTime gameTime, Rectangle bounds, Jugador otro)
    {
        ProcesarEntrada();
        ActualizarAnimacion(gameTime);

        Vector2 movimiento = ObtenerMovimiento();
        Collision.Move(movimiento, otro.Collision);
        Collision.KeepInside(bounds);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Sprite.Draw(spriteBatch, Position);
    }

    private void ProcesarEntrada()
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(_teclaAtaque) &&
            _estadoAnimacion == EstadoAnimacion.Idle)
        {
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
        Sprite.Play(loop: nuevoEstado == EstadoAnimacion.Idle);
    }

    private Vector2 ObtenerMovimiento()
    {
        Vector2 movimiento = Vector2.Zero;

        if (Core.Input.Keyboard.IsKeyDown(_teclaArriba)) movimiento.Y -= Velocidad;
        if (Core.Input.Keyboard.IsKeyDown(_teclaAbajo)) movimiento.Y += Velocidad;
        if (Core.Input.Keyboard.IsKeyDown(_teclaIzquierda)) movimiento.X -= Velocidad;
        if (Core.Input.Keyboard.IsKeyDown(_teclaDerecha)) movimiento.X += Velocidad;

        return movimiento;
    }
}
