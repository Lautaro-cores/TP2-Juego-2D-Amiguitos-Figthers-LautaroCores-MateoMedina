using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Amiguitos_Fighthers;

public sealed class BarraDeVida
{
    private readonly Personaje _personaje;
    private readonly Texture2D _pixel;
    private readonly Rectangle _bounds;

    public BarraDeVida(Personaje personaje, Texture2D pixel, Rectangle bounds)
    {
        _personaje = personaje;
        _pixel = pixel;
        _bounds = bounds;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        const int borde = 2;
        Rectangle interior = new(
            _bounds.X + borde,
            _bounds.Y + borde,
            _bounds.Width - borde * 2,
            _bounds.Height - borde * 2);

        spriteBatch.Draw(_pixel, _bounds, Color.Black);
        spriteBatch.Draw(_pixel, interior, Color.DarkRed);

        int anchoVida = (int)(interior.Width * _personaje.PorcentajeVida);
        if (anchoVida > 0)
        {
            Rectangle relleno = new(interior.X, interior.Y, anchoVida, interior.Height);
            Color colorVida = Color.LimeGreen;

            spriteBatch.Draw(_pixel, relleno, colorVida);
        }
    }
}