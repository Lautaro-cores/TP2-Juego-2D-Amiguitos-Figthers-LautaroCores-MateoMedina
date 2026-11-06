using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;

namespace Amiguitos_Fighthers;

public sealed class EscenaVictoria : Scene
{
    private readonly string _resultado;
    private readonly PersonajeSeleccion _ganador;
    private readonly PersonajeSeleccion _perdedor;

    private Texture2D _fondo;
    private Texture2D _imagenGanador;
    private Texture2D _nombreGanador;
    private Texture2D _imagenPerdedor;
    private Rectangle _areaGanador;
    private Rectangle _areaNombreGanador;
    private Rectangle _areaPerdedor;
    private SpriteFont _fuente;

    public EscenaVictoria(string resultado)
    {
        _resultado = resultado;
    }

    public EscenaVictoria(PersonajeSeleccion ganador, PersonajeSeleccion perdedor)
    {
        _ganador = ganador;
        _perdedor = perdedor;
    }

    public override void LoadContent()
    {
        _fuente = Content.Load<SpriteFont>("SpriteFonts/Default");

        if (_ganador == null || _perdedor == null)
        {
            return;
        }

        _fondo = Content.Load<Texture2D>("pruebas/fondoganador");
        _imagenGanador = Content.Load<Texture2D>(_ganador.Imagen);
        _nombreGanador = Content.Load<Texture2D>(_ganador.ImagenNombre);
        _imagenPerdedor = Content.Load<Texture2D>(_perdedor.Imagen);
        _areaGanador = ObtenerAreaOpaca(_imagenGanador);
        _areaNombreGanador = ObtenerAreaOpaca(_nombreGanador);
        _areaPerdedor = ObtenerAreaOpaca(_imagenPerdedor);
    }

    public override void Update(GameTime gameTime)
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
        {
            Core.ChangeScene(new EscenaTitulo());
        }
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice graphicsDevice = Core.GraphicsDevice;
        Rectangle bounds = graphicsDevice.Viewport.Bounds;
        graphicsDevice.Clear(Color.Black);

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        if (_ganador == null || _perdedor == null)
        {
            DibujarResultadoEmpate(bounds);
        }
        else
        {
            Core.SpriteBatch.Draw(_fondo, bounds, Color.White);
            DibujarAjustado(
                _nombreGanador,
                _areaNombreGanador,
                CrearArea(bounds, 0.025f, 0.10f, 0.45f, 0.30f));
            DibujarAjustado(
                _imagenGanador,
                _areaGanador,
                CrearArea(bounds, 0.46f, 0.02f, 0.54f, 0.96f));
            DibujarAjustado(
                _imagenPerdedor,
                _areaPerdedor,
                CrearArea(bounds, 0.08f, 0.69f, 0.35f, 0.27f));
        }

        Core.SpriteBatch.End();
    }

    private void DibujarResultadoEmpate(Rectangle bounds)
    {
        const string textoVictoria = "FIN DEL COMBATE";
        const string textoContinuar = "PRESIONA ENTER PARA VOLVER AL MENU";
        Vector2 tamanoVictoria = _fuente.MeasureString(textoVictoria);
        Vector2 tamanoResultado = _fuente.MeasureString(_resultado);
        Vector2 tamanoContinuar = _fuente.MeasureString(textoContinuar);

        Core.SpriteBatch.DrawString(
            _fuente,
            textoVictoria,
            new Vector2((bounds.Width - tamanoVictoria.X) / 2f, bounds.Height / 2f - 100f),
            Color.White);
        Core.SpriteBatch.DrawString(
            _fuente,
            _resultado,
            new Vector2((bounds.Width - tamanoResultado.X) / 2f, bounds.Height / 2f - 20f),
            Color.Gold);
        Core.SpriteBatch.DrawString(
            _fuente,
            textoContinuar,
            new Vector2((bounds.Width - tamanoContinuar.X) / 2f, bounds.Height / 2f + 70f),
            Color.White);
    }

    private static void DibujarAjustado(
        Texture2D textura,
        Rectangle area,
        Rectangle destino)
    {
        float escala = MathHelper.Min(
            destino.Width / (float)area.Width,
            destino.Height / (float)area.Height);
        Vector2 centro = new(
            destino.X + destino.Width / 2f,
            destino.Y + destino.Height / 2f);

        Core.SpriteBatch.Draw(
            textura,
            centro,
            area,
            Color.White,
            0f,
            new Vector2(area.Width / 2f, area.Height / 2f),
            escala,
            SpriteEffects.None,
            0f);
    }

    private static Rectangle CrearArea(
        Rectangle bounds,
        float izquierda,
        float arriba,
        float ancho,
        float alto)
    {
        return new Rectangle(
            (int)(bounds.Width * izquierda),
            (int)(bounds.Height * arriba),
            (int)(bounds.Width * ancho),
            (int)(bounds.Height * alto));
    }

    private static Rectangle ObtenerAreaOpaca(Texture2D textura)
    {
        Color[] pixeles = new Color[textura.Width * textura.Height];
        textura.GetData(pixeles);

        int izquierda = textura.Width;
        int arriba = textura.Height;
        int derecha = -1;
        int abajo = -1;

        for (int y = 0; y < textura.Height; y++)
        {
            for (int x = 0; x < textura.Width; x++)
            {
                if (pixeles[y * textura.Width + x].A <= 8)
                {
                    continue;
                }

                izquierda = System.Math.Min(izquierda, x);
                arriba = System.Math.Min(arriba, y);
                derecha = System.Math.Max(derecha, x);
                abajo = System.Math.Max(abajo, y);
            }
        }

        if (derecha < izquierda || abajo < arriba)
        {
            throw new System.InvalidOperationException(
                $"La imagen '{textura.Name}' no contiene pixeles visibles.");
        }

        return new Rectangle(
            izquierda,
            arriba,
            derecha - izquierda + 1,
            abajo - arriba + 1);
    }
}
