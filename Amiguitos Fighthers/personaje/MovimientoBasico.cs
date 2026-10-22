using System;
using Microsoft.Xna.Framework;

namespace Amiguitos_Fighthers;

public sealed class MovimientoBasico
{
    private readonly Hitbox _hitbox = new();
    private bool _direccionAtaqueDerecha;

    // Estos valores permiten ajustar el movimiento y el ataque de cada personaje.
    public int Danio { get; }
    public float VelocidadMovimiento { get; }
    public float VelocidadSalto { get; }
    public float Gravedad { get; }
    public int AnchoHitbox { get; }
    public int AltoHitbox { get; }
    public bool EstaEnElSuelo { get; private set; }
    private float _velocidadVertical;

    public MovimientoBasico(
        int danio,
        float velocidadMovimiento,
        float velocidadSalto,
        float gravedad,
        int anchoHitbox,
        int altoHitbox)
    {
        Danio = danio;
        VelocidadMovimiento = velocidadMovimiento;
        VelocidadSalto = velocidadSalto;
        Gravedad = gravedad;
        AnchoHitbox = anchoHitbox;
        AltoHitbox = altoHitbox;
    }

    public MovimientoBasico CrearInstancia()
    {
        // Cada jugador necesita su propio estado de salto y su propia hitbox.
        return new MovimientoBasico(
            Danio,
            VelocidadMovimiento,
            VelocidadSalto,
            Gravedad,
            AnchoHitbox,
            AltoHitbox);
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

    public void IniciarAtaque(bool mirandoDerecha)
    {
        _direccionAtaqueDerecha = mirandoDerecha;
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

        // El daño del movimiento básico se aplica una sola vez al impactar.
        objetivo.RecibirDanio(Danio);
        return true;
    }

    public void FinalizarAtaque()
    {
        _hitbox.Desactivar();
    }

    private Rectangle ObtenerHitbox(Rectangle boundsAtacante)
    {
        int x = _direccionAtaqueDerecha
            ? boundsAtacante.Right - 10
            : boundsAtacante.Left - AnchoHitbox + 10;
        int y = boundsAtacante.Top + (int)(boundsAtacante.Height * 0.3f);

        return new Rectangle(x, y, AnchoHitbox, AltoHitbox);
    }
}
