using System;
using Microsoft.Xna.Framework;

namespace Amiguitos_Fighthers;

public sealed class MovimientoBasico
{
    // TipoAtaque identifica la accion seleccionada por la tecla del jugador.
    // La animacion, el dano y la forma de la hitbox se eligen a partir de este valor.
    public enum TipoAtaque
    {
        GolpeLigero,
        GolpeMedio,
        GolpePesado,
        PatadaLigera,
        PatadaMedia,
        PatadaPesada
    }

    private readonly Hitbox _hitbox = new();
    private bool _direccionAtaqueDerecha;
    private TipoAtaque _tipoAtaque;

    // Cuando es verdadero, ObtenerHitbox usa la zona inferior del cuerpo.
    private bool _ataqueAereo;

    // Estos valores permiten ajustar el movimiento y el ataque de cada personaje.
    public float VelocidadMovimiento { get; }
    public float VelocidadSalto { get; }
    public float Gravedad { get; }
    public bool EstaEnElSuelo { get; private set; }
    private float _velocidadVertical;

    public int DanioGolpeLigero { get; }
    public int DanioGolpeMedio { get; }
    public int DanioGolpePesado { get; }
    public int DanioPatadaLigera { get; }
    public int DanioPatadaMedia { get; }
    public int DanioPatadaPesada { get; }

    public string AnimacionGolpeLigero { get; }
    public string AnimacionGolpeMedio { get; }
    public string AnimacionGolpePesado { get; }
    public string AnimacionPatadaLigera { get; }
    public string AnimacionPatadaMedia { get; }
    public string AnimacionPatadaPesada { get; }

    public int AnchoHitbox => EsPatada ? 50 : 30;
    public int AltoHitbox => 30;
    private bool EsPatada => _tipoAtaque is TipoAtaque.PatadaLigera or TipoAtaque.PatadaMedia or TipoAtaque.PatadaPesada;

    public bool GolpeAcertado { get; private set; }

    public MovimientoBasico(
        float velocidadMovimiento,
        float velocidadSalto,
        float gravedad,
        int danioGolpeLigero,
        int danioGolpeMedio,
        int danioGolpePesado,
        int danioPatadaLigera,
        int danioPatadaMedia,
        int danioPatadaPesada,
        string animacionGolpeLigero,
        string animacionGolpeMedio,
        string animacionGolpePesado,
        string animacionPatadaLigera,
        string animacionPatadaMedia,
        string animacionPatadaPesada)
    {
        VelocidadMovimiento = velocidadMovimiento;
        VelocidadSalto = velocidadSalto;
        Gravedad = gravedad;
        DanioGolpeLigero = danioGolpeLigero;
        DanioGolpeMedio = danioGolpeMedio;
        DanioGolpePesado = danioGolpePesado;
        DanioPatadaLigera = danioPatadaLigera;
        DanioPatadaMedia = danioPatadaMedia;
        DanioPatadaPesada = danioPatadaPesada;
        AnimacionGolpeLigero = animacionGolpeLigero;
        AnimacionGolpeMedio = animacionGolpeMedio;
        AnimacionGolpePesado = animacionGolpePesado;
        AnimacionPatadaLigera = animacionPatadaLigera;
        AnimacionPatadaMedia = animacionPatadaMedia;
        AnimacionPatadaPesada = animacionPatadaPesada;
    }

    public MovimientoBasico CrearInstancia()
    {
        // Cada jugador necesita su propio estado de salto y su propia hitbox.
        return new MovimientoBasico(
            VelocidadMovimiento,
            VelocidadSalto,
            Gravedad,
            DanioGolpeLigero,
            DanioGolpeMedio,
            DanioGolpePesado,
            DanioPatadaLigera,
            DanioPatadaMedia,
            DanioPatadaPesada,
            AnimacionGolpeLigero,
            AnimacionGolpeMedio,
            AnimacionGolpePesado,
            AnimacionPatadaLigera,
            AnimacionPatadaMedia,
            AnimacionPatadaPesada);
    }

    public void Saltar()
    {
        if (!EstaEnElSuelo)
        {
            return;
        }

        _velocidadVertical = -VelocidadSalto;
        EstaEnElSuelo = false;
    }

    public float ActualizarSalto(GameTime gameTime)
    {
        if (EstaEnElSuelo)
        {
            return 0;
        }

        // La gravedad se calcula según el tiempo transcurrido para mantener el salto uniforme.
        float segundos = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _velocidadVertical += Gravedad * segundos;
        return _velocidadVertical * segundos;
    }

    public void Aterrizar()
    {
        _velocidadVertical = 0;
        EstaEnElSuelo = true;
    }

    public void IniciarAtaque(
        bool mirandoDerecha,
        TipoAtaque tipoAtaque = TipoAtaque.GolpeLigero,
        bool ataqueAereo = false)
    {
        // 1. Reinicia el indicador de impacto para que el ataque solo pueda acertar
        // una vez durante esta ejecucion.
        // 2. Guarda hacia donde debe aparecer la hitbox.
        // 3. Guarda el tipo de ataque seleccionado para elegir dano y medidas.
        GolpeAcertado = false;
        _direccionAtaqueDerecha = mirandoDerecha;
        _tipoAtaque = tipoAtaque;
        _ataqueAereo = ataqueAereo;
        _hitbox.Activar();
    }

    public bool IntentarAtacar(Rectangle boundsAtacante, Personaje objetivo, Rectangle boundsObjetivo)
    {
        if (!_hitbox.Activa)
        {
            return false;
        }

        _hitbox.Actualizar(ObtenerHitbox(boundsAtacante));

        if (!_hitbox.Colisiona(boundsObjetivo))
        {
            return false;
        }

        // El daño se aplica una sola vez al impactar. Si el golpe entra dentro
        // de la protección del rival, se calcula un daño reducido en lugar del normal.
        int danio = _tipoAtaque switch
        {
            TipoAtaque.GolpeLigero => DanioGolpeLigero,
            TipoAtaque.GolpeMedio => DanioGolpeMedio,
            TipoAtaque.GolpePesado => DanioGolpePesado,
            TipoAtaque.PatadaLigera => DanioPatadaLigera,
            TipoAtaque.PatadaMedia => DanioPatadaMedia,
            TipoAtaque.PatadaPesada => DanioPatadaPesada,
            _ => DanioGolpeLigero
        };

        int danioFinal = danio;
        Jugador jugadorObjetivo = objetivo as Jugador;

        if (jugadorObjetivo != null && jugadorObjetivo.BloqueaAtaque(_hitbox.Bounds))
        {
            danioFinal = Math.Max(1, danio / 2);
        }

        objetivo.RecibirDanio(danioFinal);
        GolpeAcertado = true;
        return true;
    }

    public bool ConsumirGolpeAcertado()
{
    if (!GolpeAcertado)
    {
        return false;
    }

    GolpeAcertado = false;
    return true;
}

    public void FinalizarAtaque()
    {
            GolpeAcertado = false;
        _ataqueAereo = false;
        _hitbox.Desactivar();
    }

    private Rectangle ObtenerHitbox(Rectangle boundsAtacante)
    {
        // La coordenada horizontal depende de hacia donde mira el atacante.
        // Derecha: aparece pegada al borde derecho de su cuerpo.
        // Izquierda: se coloca delante de su borde izquierdo usando el ancho
        // calculado por AnchoHitbox para que el rectangulo quede dentro de su lado.
        int x = _direccionAtaqueDerecha
            ? boundsAtacante.Right - 10
            : boundsAtacante.Left - AnchoHitbox + 10;

        // Los ataques normales usan la parte superior del cuerpo. En el aire,
        // la hitbox comienza desde la parte inferior para que alcance al rival
        // por debajo mientras el personaje permanece flotando.
        int y = _ataqueAereo
            ? boundsAtacante.Bottom - AltoHitbox
            : boundsAtacante.Top + (int)(boundsAtacante.Height * 0.3f);

        // boundsAtacante solo aporta la posicion del personaje. AnchoHitbox y
        // AltoHitbox aportan las dimensiones: golpes 30x30 y patadas 50x30.
        return new Rectangle(x, y, AnchoHitbox, AltoHitbox);
    }
}
