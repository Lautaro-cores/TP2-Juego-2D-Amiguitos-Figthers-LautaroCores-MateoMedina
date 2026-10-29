using Microsoft.Xna.Framework;

namespace Amiguitos_Fighthers;

public sealed class EstadoCombate
{
    private const float DuracionGolpe = 1.0f;
    private float _tiempoRestante;

    public bool EstaGolpeando =>
        _tiempoRestante > 0;

    public void ActivarGolpe()
    {
        _tiempoRestante = DuracionGolpe;
    }

    public void Actualizar(GameTime gameTime)
    {
        if (!EstaGolpeando)
        {
            return;
        }

        _tiempoRestante -=
            (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_tiempoRestante < 0)
        {
            _tiempoRestante = 0;
        }
    }
}