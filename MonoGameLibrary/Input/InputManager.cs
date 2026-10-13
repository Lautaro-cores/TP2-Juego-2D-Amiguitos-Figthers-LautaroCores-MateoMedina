using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Input;

public class InputManager
{
    
    /// <summary>
/// Gets the state information of keyboard input.
/// </summary>
public KeyboardInfo Keyboard { get; private set; }

/// <summary>
/// Gets the state information of mouse input.
/// </summary>
public MouseInfo Mouse { get; private set; }

/// <summary>
/// Gets the state information of a gamepad.
/// </summary>

/// <summary>
/// Creates a new InputManager.
/// </summary>
public InputManager()
{
    Keyboard = new KeyboardInfo();
    Mouse = new MouseInfo();


}

/// <summary>
/// Updates the state information for the keyboard, mouse, and gamepad inputs.
/// </summary>
/// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
public void Update(GameTime gameTime)
{
    Keyboard.Update();
    Mouse.Update();
}

}

