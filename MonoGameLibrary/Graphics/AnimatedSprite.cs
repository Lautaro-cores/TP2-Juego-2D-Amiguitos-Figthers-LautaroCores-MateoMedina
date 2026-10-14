using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics;

public class AnimatedSprite : Sprite 
{
private int _currentFrame;
private TimeSpan _elapsed;
private Animation _animation;

public bool IsPlaying { get; private set; }

private bool _isLooping = true;

/// <summary>
/// Gets or Sets the animation for this animated sprite.
/// </summary>
public Animation Animation
{
    get => _animation;
    set
    {
        _animation = value;
        _currentFrame = 0;
        Region = _animation.Frames[0];
        _elapsed = TimeSpan.Zero;
        IsPlaying = true;
        

    }
}


/// <summary>
/// Creates a new animated sprite.
/// </summary>
public AnimatedSprite() { }

/// <summary>
/// Creates a new animated sprite with the specified frames and delay.
/// </summary>
/// <param name="animation">The animation for this animated sprite.</param>
public AnimatedSprite(Animation animation)
{
    Animation = animation;
}


public void Play(bool loop = true)
{
    _currentFrame = 0;
    _elapsed = TimeSpan.Zero;
    _isLooping = loop;
    IsPlaying = true;
    Region = _animation.Frames[0];
}

public void Stop()
{
    IsPlaying = false;
}


/// <summary>
/// Updates this animated sprite.
/// </summary>
/// <param name="gameTime">A snapshot of the game timing values provided by the framework.</param>
public void Update(GameTime gameTime)
{

    if (!IsPlaying)
    {
        return;
    }


    _elapsed += gameTime.ElapsedGameTime;

    if (_elapsed >= _animation.Delay)
    {
        _elapsed -= _animation.Delay;
        _currentFrame++;

        if (_currentFrame >= _animation.Frames.Count)
        {
            if (_isLooping)
            {
                _currentFrame = 0;
            }
            else
            {
                _currentFrame = _animation.Frames.Count - 1;
                IsPlaying = false;
            }
        }

        Region = _animation.Frames[_currentFrame];
    }
}




}
