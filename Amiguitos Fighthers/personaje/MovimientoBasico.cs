using System;
using Microsoft.Xna.Framework;

namespace Amiguitos_Fighthers;

public sealed class MovimientoBasico
{
    private readonly Hitbox _hitbox = new();
    private bool _direccionAtaqueDerecha;

    public int Danio { get; }
    public float VelocidadMovimiento { get; }
    public int AnchoHitbox { get; }
    public int AltoHitbox { get; }

    public MovimientoBasico(int danio, float velocidadMovimiento, int anchoHitbox, int altoHitbox)
    {

        Danio = danio;
        VelocidadMovimiento = velocidadMovimiento;
        AnchoHitbox = anchoHitbox;
        AltoHitbox = altoHitbox;
    }

    public MovimientoBasico CrearInstancia()
    {
        return new MovimientoBasico(Danio, VelocidadMovimiento, AnchoHitbox, AltoHitbox);
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
