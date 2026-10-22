using Microsoft.Xna.Framework;

namespace Amiguitos_Fighthers;

public sealed class Hitbox
{
    public Rectangle Bounds { get; private set; }
    public bool Activa { get; private set; }

    public Hitbox()
    {
        Bounds = Rectangle.Empty;
        Activa = false;
    }

    public void Activar()
    {
        
        Activa = true;
    }

    public void Actualizar(Rectangle bounds)
    {
        Bounds = bounds;
    }

    public void Desactivar()
    {
        Activa = false;
        
    }

    public bool Colisiona(Rectangle objetivo)
    {
        if (!Activa)
        {
            return false;
        }

        bool impacto = Bounds.Intersects(objetivo);

        if (impacto)
        {
            Desactivar();
        }

        return impacto;
    }
}