using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;

namespace Amiguitos_Fighthers;

public sealed class EscenaTitulo : Scene
{
    private Texture2D _portada;
    private Texture2D _pixel;
    private SpriteFont _fuente;

    public override void LoadContent()
    {
        _portada = Content.Load<Texture2D>("pruebas/portada");
        _fuente = Content.Load<SpriteFont>("SpriteFonts/Default");
        _pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public override void Update(GameTime gameTime)
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
        {
            Core.ChangeScene(new EscenaCombate());
        }
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice graphicsDevice = Core.GraphicsDevice;
        Rectangle bounds = graphicsDevice.Viewport.Bounds;
        graphicsDevice.Clear(Color.Black);

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        Core.SpriteBatch.Draw(_portada, bounds, Color.White);


        const string texto = "PRESIONA ENTER PARA COMENZAR";
        Vector2 tamanoTexto = _fuente.MeasureString(texto);

        Core.SpriteBatch.DrawString(_fuente,  texto , new((bounds.Width - tamanoTexto.X) / 2f, bounds.Bottom - 450  / 2f), Color.White);
        Core.SpriteBatch.End();
    }

    public override void UnloadContent()
    {
        _pixel?.Dispose();
        base.UnloadContent();
    }
}
