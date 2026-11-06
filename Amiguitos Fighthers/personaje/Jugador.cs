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
    private readonly Keys _teclaGolpeLigero;
    private readonly Keys _teclaGolpeMedio;
    private readonly Keys _teclaGolpePesado;
    private readonly Keys _teclaPatadaLigera;
    private readonly Keys _teclaPatadaMedia;
    private readonly Keys _teclaPatadaPesada;

    private readonly MovimientoBasico _movimientoBasico;
    private bool _mirandoDerecha = true;

    // Conserva hacia qué lado apunta la protección mientras el jugador retrocede.
    private bool _direccionBloqueoDerecha;

    // Se activa solo durante el frame en que se presiona la tecla de ataque.
    private bool _golpeIniciado;
    private bool _estaCaminando;

    public bool EstaAgachado
    {
        get => _estaAgachado;
        private set => _estaAgachado = value;
    }

    private enum EstadoAnimacion
    {
        Idle,
        Caminando,
        Atacando,

        Bloqueando,

        Agachandose,
        InicioSalto,
        Saltando
    }
    private readonly Dictionary<EstadoAnimacion, AnimatedSprite> _animaciones;
    private readonly Dictionary<MovimientoBasico.TipoAtaque, AnimatedSprite> _animacionesAtaque;
    private readonly AnimatedSprite _animacionGolpeAgachado;
    private readonly AnimatedSprite _animacionPatadaAgachado;
    private readonly AnimatedSprite _animacionGolpeAire;
    private readonly AnimatedSprite _animacionPatadaAire;
    private MovimientoBasico.TipoAtaque _tipoAtaqueActual;
    private AtaqueAgachadoTipo _ataqueAgachado;

    // Solo se permite un ataque de golpe/patada ligera por cada salto.
    private bool _ataqueAereoRealizado;

    // Identifica qué sprite especial debe reproducirse durante un ataque.
    private enum AtaqueAgachadoTipo
    {
        Ninguno,
        GolpeLigero,
        PatadaLigera,
        GolpeAire,
        PatadaAire
    }

    private EstadoAnimacion _estadoAnimacion;


    public bool GolpeAcertado => _movimientoBasico.GolpeAcertado;

    // Permite que Game1 sepa si cualquiera debe iniciar el periodo de bloqueo.
    public bool GolpeIniciado => _golpeIniciado;

    public AnimatedSprite Sprite => _estadoAnimacion == EstadoAnimacion.Atacando
        ? _ataqueAgachado == AtaqueAgachadoTipo.GolpeLigero
            ? _animacionGolpeAgachado
            : _ataqueAgachado == AtaqueAgachadoTipo.PatadaLigera
            ? _animacionPatadaAgachado
            : _ataqueAgachado == AtaqueAgachadoTipo.GolpeAire
            ? _animacionGolpeAire
            : _ataqueAgachado == AtaqueAgachadoTipo.PatadaAire
            ? _animacionPatadaAire
            : _animacionesAtaque[_tipoAtaqueActual]
        : _animaciones[_estadoAnimacion];

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
        Keys golpeLigero,
        Keys golpeMedio,
        Keys golpePesado,
        Keys patadaLigera,
        Keys patadaMedia,
        Keys patadaPesada,
        bool espejo = false
        ) : base(definicion.VidaMaxima)
    {
        // La definicion llega desde PersonajeDefinicion.Crear(). Contiene la vida,
        // el movimiento, los danos y los nombres de animacion del personaje elegido.
        Definicion = definicion;
        _movimientoBasico = definicion.MovimientoBasico.CrearInstancia();

        // EscenaCombate envia las teclas correspondientes a este jugador. Asi cada
        // personaje usa las mismas acciones, aunque se puedan definir otros controles.
        _teclaSalto = salto;
        _teclaAbajo = abajo;
        _teclaIzquierda = izquierda;
        _teclaDerecha = derecha;
        _teclaGolpeLigero = golpeLigero;
        _teclaGolpeMedio = golpeMedio;
        _teclaGolpePesado = golpePesado;
        _teclaPatadaLigera = patadaLigera;
        _teclaPatadaMedia = patadaMedia;
        _teclaPatadaPesada = patadaPesada;

        AnimatedSprite spriteIdle = atlas.CreateAnimatedSprite(definicion.AnimacionIdle);
        spriteIdle.Scale = new Vector2(3.5f, 3.5f);
        spriteIdle.Play(loop: true);

        // La animación de caminar debe cargarse de su nombre propio. Antes se
        // reutilizaba accidentalmente el campo de ataque, por lo que caminando
        // podía mostrar otra animación, verse cortado o cambiar su geometría.
        AnimatedSprite spriteCaminar = atlas.CreateAnimatedSprite("caminar");
        spriteCaminar.Scale = new Vector2(3.5f, 3.5f);
        spriteCaminar.Stop();

        AnimatedSprite spriteBloquear = atlas.CreateAnimatedSprite(definicion.AnimacionBloquear);
        spriteBloquear.Scale = new Vector2(3.5f, 3.5f);
        spriteBloquear.Stop();

        AnimatedSprite spriteAgacharse = atlas.CreateAnimatedSprite(definicion.AnimacionAgacharse);
        spriteAgacharse.Scale = new Vector2(3.5f, 3.5f);
        spriteAgacharse.Stop();

        AnimatedSprite spriteAire = atlas.CreateAnimatedSprite(definicion.AnimacionAire);
        spriteAire.Scale = new Vector2(3.5f, 3.5f);
        spriteAire.Play(loop: true);

        AnimatedSprite spriteSalto = atlas.CreateAnimatedSprite(definicion.SpriteSalto);
        spriteSalto.Scale = new Vector2(3.5f, 3.5f);
        spriteSalto.Stop();


        if (espejo)
        {
            spriteIdle.Effects = SpriteEffects.FlipHorizontally;
            spriteCaminar.Effects = SpriteEffects.FlipHorizontally;
            spriteBloquear.Effects = SpriteEffects.FlipHorizontally;
            spriteAgacharse.Effects = SpriteEffects.FlipHorizontally;
            spriteAire.Effects = SpriteEffects.FlipHorizontally;
            spriteSalto.Effects = SpriteEffects.FlipHorizontally;
        }

        _animaciones = new Dictionary<EstadoAnimacion, AnimatedSprite>
        {
            [EstadoAnimacion.Idle] = spriteIdle,
            [EstadoAnimacion.Caminando] = spriteCaminar,
            [EstadoAnimacion.Bloqueando] = spriteBloquear,
            [EstadoAnimacion.Agachandose] = spriteAgacharse,
            [EstadoAnimacion.InicioSalto] = spriteSalto,
            [EstadoAnimacion.Saltando] = spriteAire,
        };

            // Cada TipoAtaque tiene su propio sprite. Los nombres se toman de
            // MovimientoBasico, que recibe su configuracion desde PersonajeDefinicion.
            // El sprite se crea una vez durante el constructor y despues se reutiliza.
            _animacionesAtaque = new Dictionary<MovimientoBasico.TipoAtaque, AnimatedSprite>
        {
            [MovimientoBasico.TipoAtaque.GolpeLigero] =
                CrearSpriteAtaque(atlas, definicion.MovimientoBasico.AnimacionGolpeLigero, espejo),
            [MovimientoBasico.TipoAtaque.GolpeMedio] =
                CrearSpriteAtaque(atlas, definicion.MovimientoBasico.AnimacionGolpeMedio, espejo),
            [MovimientoBasico.TipoAtaque.GolpePesado] =
                CrearSpriteAtaque(atlas, definicion.MovimientoBasico.AnimacionGolpePesado, espejo),
            [MovimientoBasico.TipoAtaque.PatadaLigera] =
                CrearSpriteAtaque(atlas, definicion.MovimientoBasico.AnimacionPatadaLigera, espejo),
            [MovimientoBasico.TipoAtaque.PatadaMedia] =
                CrearSpriteAtaque(atlas, definicion.MovimientoBasico.AnimacionPatadaMedia, espejo),
            [MovimientoBasico.TipoAtaque.PatadaPesada] =
                CrearSpriteAtaque(atlas, definicion.MovimientoBasico.AnimacionPatadaPesada, espejo),
        };

        _animacionGolpeAgachado = _crearGolpeAgachado(atlas, definicion.AnimacionGolpeAgachado, espejo);
        _animacionPatadaAgachado = _crearPatadaAgachado(atlas, definicion.AnimacionPatadaAgachado, espejo);

        // Los ataques aéreos usan sprites propios independientemente del personaje
        // elegido, siempre que esos nombres existan en su atlas.
        _animacionGolpeAire = _crearAtaqueAire(
            atlas, definicion.AnimacionGolpeAire, definicion.MovimientoBasico.AnimacionGolpeLigero, espejo);
        _animacionPatadaAire = _crearAtaqueAire(
            atlas, definicion.AnimacionPatadaAire, definicion.MovimientoBasico.AnimacionPatadaLigera, espejo);

        int anchoCollider = (int)(spriteIdle.Width * 0.80f);
        int altoCollider = (int)(spriteIdle.Height * 0.80f);


        Collision = new CollisionBody(
     new Vector2(
         posicionInicial.X + (spriteIdle.Width - anchoCollider) / 2f,
         posicionInicial.Y + spriteIdle.Height - altoCollider),
     anchoCollider,
     altoCollider);

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
        _estaCaminando = movimiento.X != 0 && _movimientoBasico.EstaEnElSuelo;

        // El movimiento vertical lo controla el salto y la gravedad, no las teclas de dirección.
        movimiento.Y = EstaAtacando ? 0 : _movimientoBasico.ActualizarSalto(gameTime);
        Collision.Move(movimiento, otro.Collision);

        int sueloY = Math.Max(bounds.Top, bounds.Bottom - AlturaSuelo);

        Rectangle limitesEscenario = new(bounds.Left, bounds.Top, bounds.Width, sueloY - bounds.Top);
        // Impide salir del escenario y coloca los pies sobre el suelo al aterrizar.
        Collision.KeepInside(limitesEscenario);

        if (Collision.Bounds.Bottom >= sueloY)
        {
            _movimientoBasico.Aterrizar();
            // Cada nuevo salto vuelve a permitir un golpe o una patada aérea ligera.
            _ataqueAereoRealizado = false;
        }
        _movimientoBasico.IntentarAtacar(Collision.Bounds, otro, otro.Collision.Bounds);

        ActualizarEstadoMovimiento();

        ActualizarAnimacion(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Vector2 posicionDibujo = new(
            Collision.Bounds.Center.X - Sprite.Width / 2f,
            Collision.Bounds.Bottom - Sprite.Height);
        Sprite.Draw(spriteBatch, posicionDibujo);
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

            // La zona de bloqueo usa el cuerpo completo del jugador como referencia.
            // Tiene 20 pixeles de ancho y la misma altura del cuerpo. Solo existe
            // mientras EstaBloqueando es verdadero.
            Rectangle cuerpo = Collision.Bounds;
            const int anchoBloqueo = 20;
            int x = cuerpo.Left - anchoBloqueo;

            // Si el bloqueo mira hacia la derecha, aparece junto al borde derecho.
            // Si mira hacia la izquierda, aparece junto al borde izquierdo.
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
        if (EstaAtacando)
        {
            return; // no permite agacharse, bloquearse ni atacar otra vez
        }


        if (Core.Input.Keyboard.WasKeyJustPressed(_teclaSalto) &&
            _movimientoBasico.EstaEnElSuelo)
        {
            _movimientoBasico.Saltar();
            _ataqueAereoRealizado = false;
            CambiarEstado(EstadoAnimacion.InicioSalto);
        }

        // Se actualiza el estado agachado antes de buscar el ataque. Así se puede
        // atacar el mismo frame en que se mantiene la tecla de agacharse.
        EstaAgachado = Core.Input.Keyboard.IsKeyDown(_teclaAbajo);
        if (EstaAgachado)
        {
            CambiarEstado(EstadoAnimacion.Agachandose);
        }

        // Primero traduce la tecla presionada a un TipoAtaque. Solo inicia el
        // ataque si el jugador esta quieto en idle y no hay otra accion activa.
        MovimientoBasico.TipoAtaque? ataque = ObtenerAtaquePresionado();
        if (ataque.HasValue &&
            !_movimientoBasico.EstaEnElSuelo &&
            ataque != MovimientoBasico.TipoAtaque.GolpeLigero &&
            ataque != MovimientoBasico.TipoAtaque.PatadaLigera)
        {
            // Los ataques medios y pesados solo existen en el suelo. Al estar
            // en el aire se ignora el resto de los botones sin iniciar un ataque.
            return;
        }

        if (ataque.HasValue &&
            (_estadoAnimacion == EstadoAnimacion.Idle ||
             _estadoAnimacion == EstadoAnimacion.Agachandose ||
             _estadoAnimacion == EstadoAnimacion.Saltando ||
             _estadoAnimacion == EstadoAnimacion.InicioSalto))
        {
            // Importante: la dirección del golpe no depende del sprite o del último movimiento.
            // Siempre debe mirarse al rival actual para que el jugador de la izquierda
            // ataque hacia la derecha cuando el enemigo está a su derecha.
            // Al aceptar la tecla se inicia el golpe, haya o no impacto.
            _golpeIniciado = true;
            _tipoAtaqueActual = ataque.Value;
            _ataqueAgachado = AtaqueAgachadoTipo.Ninguno;
            if (EstaAgachado &&
                ataque == MovimientoBasico.TipoAtaque.GolpeLigero)
            {
                _ataqueAgachado = AtaqueAgachadoTipo.GolpeLigero;
            }
            else if (EstaAgachado &&
                     ataque == MovimientoBasico.TipoAtaque.PatadaLigera)
            {
                _ataqueAgachado = AtaqueAgachadoTipo.PatadaLigera;
            }
            else if (!EstaAgachado &&
                     !_movimientoBasico.EstaEnElSuelo &&
                     !_ataqueAereoRealizado &&
                     ataque == MovimientoBasico.TipoAtaque.GolpeLigero)
            {
                _ataqueAgachado = AtaqueAgachadoTipo.GolpeAire;
            }
            else if (!EstaAgachado &&
                     !_movimientoBasico.EstaEnElSuelo &&
                     !_ataqueAereoRealizado &&
                     ataque == MovimientoBasico.TipoAtaque.PatadaLigera)
            {
                _ataqueAgachado = AtaqueAgachadoTipo.PatadaAire;
            }
            EstaBloqueando = false;
            _estaCaminando = false;

            _mirandoDerecha = Collision.Bounds.Center.X <= otro.Collision.Bounds.Center.X;

            _movimientoBasico.IniciarAtaque(
                _mirandoDerecha,
                ataque.Value,
                ataqueAereo: !_movimientoBasico.EstaEnElSuelo);
            if (!_movimientoBasico.EstaEnElSuelo)
            {
                // El bloqueo persiste hasta aterrizar, aunque termine antes la animación.
                _ataqueAereoRealizado = true;
            }

            CambiarEstado(EstadoAnimacion.Atacando);
        }

    }

    private MovimientoBasico.TipoAtaque? ObtenerAtaquePresionado()
    {
        // Comprueba las seis teclas de ataque. Al encontrar una tecla presionada
        // devuelve el TipoAtaque correspondiente, que despues se usa para:
        // 1. Buscar la animacion dentro de _animacionesAtaque.
        // 2. Configurar el tipo de ataque en MovimientoBasico.
        // 3. Calcular el dano y el tamano de hitbox correspondientes.
        if (Core.Input.Keyboard.WasKeyJustPressed(_teclaGolpeLigero))
        {
            return MovimientoBasico.TipoAtaque.GolpeLigero;
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(_teclaGolpeMedio))
        {
            return MovimientoBasico.TipoAtaque.GolpeMedio;
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(_teclaGolpePesado))
        {
            return MovimientoBasico.TipoAtaque.GolpePesado;
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(_teclaPatadaLigera))
        {
            return MovimientoBasico.TipoAtaque.PatadaLigera;
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(_teclaPatadaMedia))
        {
            return MovimientoBasico.TipoAtaque.PatadaMedia;
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(_teclaPatadaPesada))
        {
            return MovimientoBasico.TipoAtaque.PatadaPesada;
        }

        return null;
    }

    private static AnimatedSprite CrearSpriteAtaque(TextureAtlas atlas, string nombreAnimacion, bool espejo)
    {
        // nombreAnimacion viene de MovimientoBasico.AnimacionGolpeLigero,
        // AnimacionGolpeMedio, AnimacionGolpePesado y sus patadas equivalentes.
        // Atlas busca ese nombre entre las animaciones definidas en el XML.
        AnimatedSprite sprite = atlas.CreateAnimatedSprite(nombreAnimacion);
        sprite.Scale = new Vector2(3.5f, 3.5f);
        sprite.Stop();

        // El jugador izquierdo mira hacia la derecha y el derecho hacia la izquierda.
        // Cada ataque creado para el jugador derecho se refleja horizontalmente.
        if (espejo)
        {
            sprite.Effects = SpriteEffects.FlipHorizontally;
        }

        return sprite;
    }

    private static AnimatedSprite _crearGolpeAgachado(TextureAtlas atlas, string nombreAnimacion, bool espejo)
    {
        AnimatedSprite sprite;
        try
        {
            sprite = atlas.CreateAnimatedSprite(nombreAnimacion);
        }
        catch (KeyNotFoundException)
        {
            try
            {
                sprite = atlas.CreateAnimatedSprite("puñoLigero");
            }
            catch (KeyNotFoundException)
            {
                sprite = atlas.CreateAnimatedSprite("Attack");
            }
        }
        sprite.Scale = new Vector2(3.5f, 3.5f);
        sprite.Stop();

        if (espejo)
        {
            sprite.Effects = SpriteEffects.FlipHorizontally;
        }

        return sprite;
    }

    private static AnimatedSprite _crearPatadaAgachado(TextureAtlas atlas, string nombreAnimacion, bool espejo)
    {
        AnimatedSprite sprite;
        try
        {
            sprite = atlas.CreateAnimatedSprite(nombreAnimacion);
        }
        catch (KeyNotFoundException)
        {
            try
            {
                sprite = atlas.CreateAnimatedSprite("patadaLigera");
            }
            catch (KeyNotFoundException)
            {
                sprite = atlas.CreateAnimatedSprite("Attack");
            }
        }
        sprite.Scale = new Vector2(3.5f, 3.5f);
        sprite.Stop();

        if (espejo)
        {
            sprite.Effects = SpriteEffects.FlipHorizontally;
        }

        return sprite;
    }

    private static AnimatedSprite _crearAtaqueAire(
        TextureAtlas atlas,
        string nombreAire,
        string nombreSuelo,
        bool espejo)
    {
        // Usa primero la animación aérea del atlas. Si no existe, conserva el
        // ataque normal correspondiente para evitar romper la selección del personaje.
        AnimatedSprite sprite;
        try
        {
            sprite = atlas.CreateAnimatedSprite(nombreAire);
        }
        catch (KeyNotFoundException)
        {
            try
            {
                sprite = atlas.CreateAnimatedSprite(nombreSuelo);
            }
            catch (KeyNotFoundException)
            {
                sprite = atlas.CreateAnimatedSprite("Attack");
            }
        }
        sprite.Scale = new Vector2(3.5f, 3.5f);
        sprite.Stop();

        if (espejo)
        {
            sprite.Effects = SpriteEffects.FlipHorizontally;
        }

        return sprite;
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
            _ataqueAgachado = AtaqueAgachadoTipo.Ninguno;
            CambiarEstado(EstaAgachado
                ? EstadoAnimacion.Agachandose
                : !_movimientoBasico.EstaEnElSuelo
                ? EstadoAnimacion.Saltando
                : EstadoAnimacion.Idle);
        }

        if (_estadoAnimacion == EstadoAnimacion.InicioSalto && !Sprite.IsPlaying)
        {
            CambiarEstado(EstadoAnimacion.Saltando);
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

        if (_estadoAnimacion == EstadoAnimacion.InicioSalto)
        {
            return;
        }

        if (!_movimientoBasico.EstaEnElSuelo)
        {
            CambiarEstado(EstadoAnimacion.Saltando);
            return;
        }

        if (_estaCaminando)
        {
            CambiarEstado(EstadoAnimacion.Caminando);
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
        else if (nuevoEstado == EstadoAnimacion.Caminando)
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
        else if (nuevoEstado == EstadoAnimacion.Saltando)
        {
            _animaciones[nuevoEstado].Play(loop: true);
        }
        else if (nuevoEstado == EstadoAnimacion.InicioSalto)
        {
            _animaciones[nuevoEstado].Play(loop: false);
        }
        else if (nuevoEstado == EstadoAnimacion.Atacando)
        {
            // Atacando usa el sprite seleccionado en _animacionesAtaque, no el
            // diccionario _animaciones de estados normales.
            if (_ataqueAgachado == AtaqueAgachadoTipo.GolpeLigero)
            {
                _animacionGolpeAgachado.Play(loop: false);
            }
            else if (_ataqueAgachado == AtaqueAgachadoTipo.PatadaLigera)
            {
                _animacionPatadaAgachado.Play(loop: false);
            }
            else if (_ataqueAgachado == AtaqueAgachadoTipo.GolpeAire)
            {
                _animacionGolpeAire.Play(loop: false);
            }
            else if (_ataqueAgachado == AtaqueAgachadoTipo.PatadaAire)
            {
                _animacionPatadaAire.Play(loop: false);
            }
            else
            {
                _animacionesAtaque[_tipoAtaqueActual].Play(loop: false);
            }
        }
        else
        {
            _animaciones[nuevoEstado].Play(loop: false);
        }
    }

    private Vector2 ObtenerMovimiento(Jugador otro, EstadoCombate estadoCombate)
    {
        Vector2 movimiento = Vector2.Zero;

        if (EstaAtacando)
        {
            return movimiento; // no permite moverse ni bloquear durante el ataque
        }

        if (Core.Input.Keyboard.IsKeyDown(_teclaIzquierda)) movimiento.X -= _movimientoBasico.VelocidadMovimiento;
        if (Core.Input.Keyboard.IsKeyDown(_teclaDerecha)) movimiento.X += _movimientoBasico.VelocidadMovimiento;

        // Comprueba si el movimiento va alejándose del enemigo.
        bool enemigoEstaDerecha = otro.Collision.Bounds.Center.X > Collision.Bounds.Center.X;
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
