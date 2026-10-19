using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Physics;

namespace Amiguitos_Fighthers;

public sealed class Jugador
{
    private const float Velocidad = 5.0f;

    private readonly Keys _teclaArriba;
    private readonly Keys _teclaAbajo;
    private readonly Keys _teclaIzquierda;
    private readonly Keys _teclaDerecha;
    private readonly Keys _teclaAtaque;

    private enum EstadoAnimacion
    {
        Idle,
        Atacando
    }
    private readonly Dictionary<EstadoAnimacion, AnimatedSprite> _animaciones;

    private EstadoAnimacion _estadoAnimacion;

    public AnimatedSprite Sprite => _animaciones[_estadoAnimacion];

    public CollisionBody Collision { get; }

    public Vector2 Position => Collision.Position;

    public bool EstaAtacando => _estadoAnimacion == EstadoAnimacion.Atacando;

    public Jugador(
        TextureAtlas atlas,
        Vector2 posicionInicial,
        Keys arriba,
        Keys abajo,
        Keys izquierda,
        Keys derecha,
        Keys ataque,
        bool espejo = false)
    {
        _teclaArriba = arriba;
        _teclaAbajo = abajo;
        _teclaIzquierda = izquierda;
        _teclaDerecha = derecha;
        _teclaAtaque = ataque;

        AnimatedSprite spriteIdle = atlas.CreateAnimatedSprite("Idle");
        spriteIdle.Scale = new Vector2(3.0f, 3.0f);

        AnimatedSprite spriteAtaque = atlas.CreateAnimatedSprite("Attack");
        spriteAtaque.Stop();
        spriteAtaque.Scale = new Vector2(3.0f, 3.0f);

        if (espejo)
        {
            spriteIdle.Effects = SpriteEffects.FlipHorizontally;
            spriteAtaque.Effects = SpriteEffects.FlipHorizontally;
        }

        _animaciones = new Dictionary<EstadoAnimacion, AnimatedSprite>
        {
            [EstadoAnimacion.Idle] = spriteIdle,
            [EstadoAnimacion.Atacando] = spriteAtaque
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
    }

    private void ActualizarAnimacion(GameTime gameTime)
    {
        Sprite.Update(gameTime);

        if (_estadoAnimacion == EstadoAnimacion.Atacando && !Sprite.IsPlaying)
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
