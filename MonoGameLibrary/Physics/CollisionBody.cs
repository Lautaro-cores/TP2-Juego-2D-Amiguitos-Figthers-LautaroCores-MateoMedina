using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Physics;

public sealed class CollisionBody
{
    private readonly Point _size;

    public Vector2 Position { get; private set; }

    public Rectangle Bounds => new(
        (int)Position.X,
        (int)Position.Y,
        _size.X,
        _size.Y);

    public CollisionBody(AnimatedSprite sprite, Vector2 position)
    {
        Position = position;
        _size = GetSize(sprite);
    }

    public void Move(Vector2 movement, CollisionBody other)
    {
        TryMove(movement.X, true, other);
        TryMove(movement.Y, false, other);
    }

    private void TryMove(float amount, bool horizontal, CollisionBody other)
    {
        if (amount == 0.0f)
        {
            return;
        }

        Vector2 nextPosition = Position;
        if (horizontal)
        {
            nextPosition.X += amount;
        }
        else
        {
            nextPosition.Y += amount;
        }

        Rectangle nextBounds = new(
            (int)nextPosition.X,
            (int)nextPosition.Y,
            _size.X,
            _size.Y);

        if (!nextBounds.Intersects(other.Bounds))
        {
            Position = nextPosition;
        }
    }

    private static Point GetSize(AnimatedSprite sprite)
    {
        int width = 0;
        int height = 0;

        foreach (TextureRegion frame in sprite.Animation.Frames)
        {
            width = Math.Max(width, (int)Math.Ceiling(frame.Width * sprite.Scale.X));
            height = Math.Max(height, (int)Math.Ceiling(frame.Height * sprite.Scale.Y));
        }

        return new Point(width, height);
    }

    public void KeepInside(Rectangle screenBounds)
{
    float maxX = Math.Max(screenBounds.Left, screenBounds.Right - _size.X);
    float maxY = Math.Max(screenBounds.Top, screenBounds.Bottom - _size.Y);

    Position = new Vector2(
        Math.Clamp(Position.X, screenBounds.Left, maxX),
        Math.Clamp(Position.Y, screenBounds.Top, maxY)
    );
}

}
