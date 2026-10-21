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
    private const float Velocidad = 5.0f;

    private readonly Keys _teclaArriba;
    private readonly Keys _teclaAbajo;
    private readonly Keys _teclaIzquierda;
    private readonly Keys _teclaDerecha;
    private readonly Keys _teclaAtaque;

    private readonly Keys _teclaBolaFuego;

    private Hitbox _hitboxGolpe;


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

        _hitboxGolpe = new Hitbox();

        _mirandoDerecha = !espejo;
    }

    private Rectangle ObtenerHitbox(Habilidad habilidad)
    {


        int ancho = habilidad.AnchoHitbox;
        int alto = habilidad.AltoHitbox;

        int x = (int)Position.X;
        //esto es para poner la hitbox a la altura del brazo, asi se moldea a cualquier personaje, 
        // pensando que cada personaje va a tener la piña a la altura del brazo
        int y = Collision.Bounds.Top + (int)(Collision.Bounds.Height * 0.3f);


    //aca estaba el error de la hitbox en la espalda, el problema era que como el sprite quedaba muy grande, la hitbox quedaba dentro de el,
    //  y cuando estaba a la derecha del personaje enenemigo, como la hitbox se invertia, si llegaba

        if (_mirandoDerecha)
        {
            // Colocar la hitbox cerca del borde derecho del personaje.
            x = Collision.Bounds.Right - 10;
        }
        else
        {
            // Colocar la hitbox cerca del borde izquierdo.
            x = Collision.Bounds.Left - ancho + 10;
        }


        return new Rectangle(x, y, ancho, alto);
    }

    public void Update(GameTime gameTime, Rectangle bounds, Jugador otro)
    {
        // La validación de colisión se hace con el rival actual del jugador.
        // Como Game1 llama Update para jugador1 y luego para jugador2,
        // ambos jugadores chequearán su ataque contra el otro personaje.
        ProcesarEntrada(otro);
        ActualizarAnimacion(gameTime);

        Vector2 movimiento = ObtenerMovimiento();
        Collision.Move(movimiento, otro.Collision);
        Collision.KeepInside(bounds);

        // Si el jugador está atacando, la hitbox se mueve junto con él y
        // se compara con el rectángulo del rival que se pasó por parámetro.
        if (_hitboxGolpe.Activa)
        {
            if (_hitboxGolpe.Colisiona(otro.Collision.Bounds))
            {
                Console.WriteLine(
                    $"{Definicion.Nombre} golpeo a {otro.Definicion.Nombre}");

                // Aplicar el daño de la habilidad de golpe.
                otro.RecibirDanio(Definicion.Habilidades[0].Danio);

                _hitboxGolpe.Desactivar();
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Sprite.Draw(spriteBatch, Position);
    }

    private void ProcesarEntrada(Jugador otro)
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(_teclaAtaque) &&
            _estadoAnimacion == EstadoAnimacion.Idle)
        {
            // Importante: la dirección del golpe no depende del sprite o del último movimiento.
            // Siempre debe mirarse al rival actual para que el jugador de la izquierda
            // ataque hacia la derecha cuando el enemigo está a su derecha.
            _mirandoDerecha = Position.X <= otro.Position.X;

            _hitboxGolpe.Actualizar(ObtenerHitbox(Definicion.Habilidades[0]));
            _hitboxGolpe.Activar();

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
            _hitboxGolpe.Desactivar();
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

        if (Core.Input.Keyboard.IsKeyDown(_teclaArriba)) movimiento.Y -= Velocidad;
        if (Core.Input.Keyboard.IsKeyDown(_teclaAbajo)) movimiento.Y += Velocidad;
        if (Core.Input.Keyboard.IsKeyDown(_teclaIzquierda)) movimiento.X -= Velocidad;
        if (Core.Input.Keyboard.IsKeyDown(_teclaDerecha)) movimiento.X += Velocidad;

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
