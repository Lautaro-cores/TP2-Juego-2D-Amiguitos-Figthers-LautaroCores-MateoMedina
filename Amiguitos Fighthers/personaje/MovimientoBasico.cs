using System;
using Microsoft.Xna.Framework;

namespace Amiguitos_Fighthers;

public sealed class MovimientoBasico
{
    private readonly Hitbox _hitbox = new();
    private bool _direccionAtaqueDerecha;

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
        if (danio <= 0)
            throw new ArgumentOutOfRangeException(nameof(danio), "El daño debe ser mayor que cero.");
        if (velocidadMovimiento <= 0)
            throw new ArgumentOutOfRangeException(nameof(velocidadMovimiento), "La velocidad debe ser mayor que cero.");
        if (velocidadSalto <= 0)
            throw new ArgumentOutOfRangeException(nameof(velocidadSalto), "La velocidad de salto debe ser mayor que cero.");
        if (gravedad <= 0)
            throw new ArgumentOutOfRangeException(nameof(gravedad), "La gravedad debe ser mayor que cero.");
        if (anchoHitbox <= 0)
            throw new ArgumentOutOfRangeException(nameof(anchoHitbox), "El ancho de la hitbox debe ser mayor que cero.");
        if (altoHitbox <= 0)
            throw new ArgumentOutOfRangeException(nameof(altoHitbox), "El alto de la hitbox debe ser mayor que cero.");

        Danio = danio;
        VelocidadMovimiento = velocidadMovimiento;
        VelocidadSalto = velocidadSalto;
        Gravedad = gravedad;
        AnchoHitbox = anchoHitbox;
        AltoHitbox = altoHitbox;
    }

    public MovimientoBasico CrearInstancia()
    {
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
