using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;

namespace Amiguitos_Fighthers;

public class Game1 : Core
{
    public Game1() : base("Amiguitos Fighthers", 1280, 720, false)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
        ChangeScene(new EscenaTitulo());

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }
}
