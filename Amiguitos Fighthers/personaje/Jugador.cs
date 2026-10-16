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

    private readonly AnimatedSprite _spriteIdle;
    private readonly AnimatedSprite _spriteAtaque;

    private bool _fuePresionadaAtaque;

    public AnimatedSprite Sprite { get; private set; }

    public CollisionBody Collision { get; }

    public Vector2 Position => Collision.Position;

    public bool EstaAtacando => _spriteAtaque.IsPlaying;

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

        _spriteIdle = atlas.CreateAnimatedSprite("Pose1-animacion");
        _spriteIdle.Scale = new Vector2(3.0f, 3.0f);

        _spriteAtaque = atlas.CreateAnimatedSprite("Pose-animacion");
        _spriteAtaque.Stop();
        _spriteAtaque.Scale = new Vector2(3.0f, 3.0f);

        if (espejo)
        {
            _spriteIdle.Effects = SpriteEffects.FlipHorizontally;
            _spriteAtaque.Effects = SpriteEffects.FlipHorizontally;
        }

        Sprite = _spriteIdle;
        Collision = new CollisionBody(_spriteIdle, posicionInicial);
    }

    public void Update(GameTime gameTime, Rectangle bounds, Jugador otro)
    {
        ManejarAtaque();
        ActualizarAnimacion(gameTime);

        Vector2 movimiento = ObtenerMovimiento();
        Collision.Move(movimiento, otro.Collision);
        Collision.KeepInside(bounds);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Sprite.Draw(spriteBatch, Position);
    }

    private void ManejarAtaque()
    {
        bool estaPresionada = Core.Input.Keyboard.IsKeyDown(_teclaAtaque);

        if (estaPresionada && !_fuePresionadaAtaque && !_spriteAtaque.IsPlaying)
        {
            _spriteAtaque.Play(loop: false);
        }

        _fuePresionadaAtaque = estaPresionada;
    }

    private void ActualizarAnimacion(GameTime gameTime)
    {
        if (_spriteAtaque.IsPlaying)
        {
            _spriteAtaque.Update(gameTime);
            Sprite = _spriteAtaque;
        }
        else
        {
            _spriteIdle.Update(gameTime);
            Sprite = _spriteIdle;
        }
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
