using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;

namespace Amiguitos_Fighthers;

public sealed class EscenaVictoria : Scene
{
    private readonly string _resultado;
    private SpriteFont _fuente;

    public EscenaVictoria(string resultado)
    {
        _resultado = resultado;
    }

    public override void LoadContent()
    {
        _fuente = Content.Load<SpriteFont>("SpriteFonts/Default");
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

        const string textoVictoria = "FIN DEL COMBATE";
        const string textoContinuar = "PRESIONA ENTER PARA VOLVER AL MENU";
        Vector2 tamanoVictoria = _fuente.MeasureString(textoVictoria);
        Vector2 tamanoResultado = _fuente.MeasureString(_resultado);
        Vector2 tamanoContinuar = _fuente.MeasureString(textoContinuar);

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
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
        Core.SpriteBatch.End();
    }
}
