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

    // Estado lógico del jugador: true cuando está presionada la tecla de agacharse.
    private bool _estaAgachado = false;
    // Altura normal y altura reducida de la hitbox, calculadas según el sprite.
    private int _alturaNormalHitbox;
    private int _alturaAgachadoHitbox;

    // El suelo queda fijo respecto al borde inferior de la pantalla.
    private const int AlturaSuelo = 50;
    private readonly Keys _teclaSalto;
    private readonly Keys _teclaAbajo;
    private readonly Keys _teclaIzquierda;
    private readonly Keys _teclaDerecha;
    private readonly Keys _teclaAtaque;

    private readonly MovimientoBasico _movimientoBasico;
    private bool _mirandoDerecha = true;

    // Conserva hacia qué lado apunta la protección mientras el jugador retrocede.
    private bool _direccionBloqueoDerecha;

    // Se activa solo durante el frame en que se presiona la tecla de ataque.
    private bool _golpeIniciado;

    public bool EstaAgachado
    {
        get => _estaAgachado;
        private set => _estaAgachado = value;
    }

    private enum EstadoAnimacion
    {
        Idle,
        Atacando,

        Bloqueando,

        Agachandose
    }
    private readonly Dictionary<EstadoAnimacion, AnimatedSprite> _animaciones;

    private EstadoAnimacion _estadoAnimacion;


    public bool GolpeAcertado => _movimientoBasico.GolpeAcertado;

    // Permite que Game1 sepa si cualquiera debe iniciar el periodo de bloqueo.
    public bool GolpeIniciado => _golpeIniciado;

    public AnimatedSprite Sprite => _animaciones[_estadoAnimacion];

    public CollisionBody Collision { get; }

    public Vector2 Position => Collision.Position;

    public bool EstaAtacando => _estadoAnimacion == EstadoAnimacion.Atacando;

    // Indica que el jugador retrocede dentro de la ventana creada por un golpe.
    public bool EstaBloqueando { get; private set; }

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

        AnimatedSprite spriteIdle = atlas.CreateAnimatedSprite(definicion.AnimacionIdle);
        spriteIdle.Scale = new Vector2(3.5f, 3.5f);
        spriteIdle.Play(loop: true);

        AnimatedSprite spriteAtaque = atlas.CreateAnimatedSprite(definicion.AnimacionAtaque);
        spriteAtaque.Scale = new Vector2(3.5f, 3.5f);
        spriteAtaque.Stop();

        AnimatedSprite spriteBloquear = atlas.CreateAnimatedSprite(definicion.AnimacionBloquear);
        spriteBloquear.Scale = new Vector2(3.5f,3.5f);
        spriteBloquear.Stop();

        AnimatedSprite spriteAgacharse = atlas.CreateAnimatedSprite(definicion.AnimacionAgacharse);
        spriteAgacharse.Scale = new Vector2(3.5f,3.5f);
        spriteAgacharse.Stop();


        if (espejo)
        {
            spriteIdle.Effects = SpriteEffects.FlipHorizontally;
            spriteAtaque.Effects = SpriteEffects.FlipHorizontally;
            spriteBloquear.Effects = SpriteEffects.FlipHorizontally;
            spriteAgacharse.Effects = SpriteEffects.FlipHorizontally;
        }

        _animaciones = new Dictionary<EstadoAnimacion, AnimatedSprite>
        {
            [EstadoAnimacion.Idle] = spriteIdle,
            [EstadoAnimacion.Atacando] = spriteAtaque,
            [EstadoAnimacion.Bloqueando] = spriteBloquear,
            [EstadoAnimacion.Agachandose] = spriteAgacharse,
        };

        Collision = new CollisionBody(spriteIdle, posicionInicial);

        // Se guarda la altura original y se define el 30% para el estado agachado.
        _alturaNormalHitbox = Collision.Bounds.Height;
        _alturaAgachadoHitbox = (int)(_alturaNormalHitbox * 0.30f);

        _mirandoDerecha = !espejo;
    }

    public void Update(GameTime gameTime, Rectangle bounds, Jugador otro, EstadoCombate estadoCombate)
    {
        // La señal dura un solo frame: evita que un golpe anterior active el timer otra vez.
        _golpeIniciado = false;

        // La validación de colisión se hace con el rival actual del jugador.
        // Como Game1 llama Update para jugador1 y luego para jugador2,
        // ambos jugadores chequearán su ataque contra el otro personaje.
        ProcesarEntrada(otro);
        ActualizarHitboxAgachado();
        



        Vector2 movimiento = ObtenerMovimiento(otro, estadoCombate);

        // El movimiento vertical lo controla el salto y la gravedad, no las teclas de dirección.
        movimiento.Y = _movimientoBasico.ActualizarSalto(gameTime);
        Collision.Move(movimiento, otro.Collision);

        int sueloY = Math.Max(bounds.Top, bounds.Bottom - AlturaSuelo);

        Rectangle limitesEscenario = new(bounds.Left, bounds.Top, bounds.Width, sueloY - bounds.Top);
        // Impide salir del escenario y coloca los pies sobre el suelo al aterrizar.
        Collision.KeepInside(limitesEscenario);

        if (Collision.Bounds.Bottom >= sueloY)
        {
            _movimientoBasico.Aterrizar();
        }
        _movimientoBasico.IntentarAtacar(Collision.Bounds, otro, otro.Collision.Bounds);
        
        ActualizarEstadoMovimiento();
        
        ActualizarAnimacion(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Sprite.Draw(spriteBatch, Position);
    }

    // Rectángulo frontal que representa la protección creada al retroceder.
    public Rectangle HitboxBloqueo
    {
        get
        {
            if (!EstaBloqueando)
            {
                return Rectangle.Empty;
            }

            Rectangle cuerpo = Collision.Bounds;
            const int anchoBloqueo = 20;
            int x = cuerpo.Left - anchoBloqueo;

            if (_direccionBloqueoDerecha)
            {
                x = cuerpo.Right;
            }

            return new Rectangle(x, cuerpo.Y, anchoBloqueo, cuerpo.Height);
        }
    }

    // Indica si una hitbox de ataque golpeó la parte protegida de este personaje.
    public bool BloqueaAtaque(Rectangle hitboxAtaque)
    {
        return EstaBloqueando && HitboxBloqueo.Intersects(hitboxAtaque);
    }

    public bool ConsumirGolpeAcertado()
    {
        return _movimientoBasico.ConsumirGolpeAcertado();
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
            // Al aceptar la tecla se inicia el golpe, haya o no impacto.
            _golpeIniciado = true;

            _mirandoDerecha = Position.X <= otro.Position.X;

            _movimientoBasico.IniciarAtaque(_mirandoDerecha);

            CambiarEstado(EstadoAnimacion.Atacando);
        }

        // Cuando se toca la tecla abajo, el estado agachado cambia inmediatamente.
        if (Core.Input.Keyboard.IsKeyDown(_teclaAbajo))
        {

            CambiarEstado(EstadoAnimacion.Agachandose);
            EstaAgachado = true;
        }
        else
        {
            EstaAgachado = false;
        }



    }

    private void ActualizarAnimacion(GameTime gameTime)
    {
        Sprite.Update(gameTime);

        if (EstaBloqueando)
        {
            CambiarEstado(EstadoAnimacion.Bloqueando);
        }

        if (_estadoAnimacion == EstadoAnimacion.Atacando && !Sprite.IsPlaying)
        {
            _movimientoBasico.FinalizarAtaque();
            CambiarEstado(EstadoAnimacion.Idle);
        }

       

    }

    private void ActualizarEstadoMovimiento()
{
    if (_estadoAnimacion == EstadoAnimacion.Atacando)
    {
        return;
    }

    if (EstaAgachado)
    {
        CambiarEstado(EstadoAnimacion.Agachandose);
        return;
    }

    if (EstaBloqueando)
    {
        CambiarEstado(EstadoAnimacion.Bloqueando);
        return;
    }

    CambiarEstado(EstadoAnimacion.Idle);
}

    private void CambiarEstado(EstadoAnimacion nuevoEstado)
    {
        
    if (_estadoAnimacion == nuevoEstado)
    {
        return;
    }

    _estadoAnimacion = nuevoEstado;

    if (nuevoEstado == EstadoAnimacion.Idle)
    {
        _animaciones[nuevoEstado].Play(loop: true);
    }
    else if (nuevoEstado == EstadoAnimacion.Bloqueando)
    {
        _animaciones[nuevoEstado].Play(loop: true);
    }
    else if (nuevoEstado == EstadoAnimacion.Agachandose)
    {
        _animaciones[nuevoEstado].Play(loop: true);
    }
    else
    {
        _animaciones[nuevoEstado].Play(loop: false);
    }
    }

    private Vector2 ObtenerMovimiento(Jugador otro, EstadoCombate estadoCombate)
    {
        Vector2 movimiento = Vector2.Zero;

        if (Core.Input.Keyboard.IsKeyDown(_teclaIzquierda)) movimiento.X -= _movimientoBasico.VelocidadMovimiento;
        if (Core.Input.Keyboard.IsKeyDown(_teclaDerecha)) movimiento.X += _movimientoBasico.VelocidadMovimiento;

        // Comprueba si el movimiento va alejándose del enemigo.
        bool enemigoEstaDerecha = otro.Position.X > Position.X;
        bool retrocede = false;

        if (enemigoEstaDerecha)
        {
            retrocede = movimiento.X < 0;
        }
        else
        {
            retrocede = movimiento.X > 0;
        }

        // Durante la ventana de 5 segundos, retroceder permite bloquear y va más lento.
        // Esta propiedad se consulta desde MovimientoBasico para aplicar daño reducido.
        EstaBloqueando = estadoCombate.EstaGolpeando && retrocede;

        if (EstaBloqueando)
        {
            _direccionBloqueoDerecha = enemigoEstaDerecha;
            movimiento *= 0.30f;
        }

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

    private void ActualizarHitboxAgachado()
    {
        //si EstáAgachado es true usa la altura agachada;
        // si es false vuelve a usar la altura normal.
        int alturaDeseada = _alturaNormalHitbox;

        if (EstaAgachado)
        {
            alturaDeseada = _alturaAgachadoHitbox;
        }

        // Solo modifica la hitbox si su altura actual es distinta de la deseada.
        if (Collision.Bounds.Height != alturaDeseada)
        {
            Collision.SetHeightFromBottom(alturaDeseada);
        }
    }
}
