using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Physics;

public sealed class CollisionBody
{
    // Posición del collider en el mundo.
    public Vector2 Position { get; private set; }

    // Ancho real del collider que se usa para detectar colisiones.
    public int Width { get; private set; }

    // Alto real del collider que se usa para detectar colisiones.
    public int Height { get; private set; }

    // Rectángulo de colisión completo.
    public Rectangle Bounds => new(
        (int)Position.X,
        (int)Position.Y,
        Width,
        Height);

    // Constructor principal: usa ancho y alto explícitos.
    public CollisionBody(Vector2 position, int width, int height)
    {
        Position = position;
        SetSize(width, height);
    }

    // Constructor útil cuando querés basar la hitbox en el sprite visual usando proporciones.
    // Ejemplo: 0.45f y 0.80f significa 45% del ancho y 80% del alto del sprite.
    public CollisionBody(AnimatedSprite sprite, Vector2 position, float widthRatio, float heightRatio)
        : this(position, 1, 1)
    {
        SetSizeFromSprite(sprite, widthRatio, heightRatio);
    }

    // Cambia la posición del collider. 
    public void SetPosition(Vector2 value)
    {
        Position = value;
    }

    // Cambia el tamaño del collider manualmente.
    public void SetSize(int width, int height)
    {
        Width = Math.Max(1, width);
        Height = Math.Max(1, height);
    }

    // Calcula el tamaño de la hitbox a partir del sprite, con porcentajes.
    public void SetSizeFromSprite(AnimatedSprite sprite, float widthRatio, float heightRatio)
    {
        Width = (int)(sprite.Width * widthRatio);
        Height = (int)(sprite.Height * heightRatio);
    }

    // Mueve el collider y resuelve solapamiento con otro collider.
    public void Move(Vector2 movement, CollisionBody other)
    {
        Rectangle previousBounds = Bounds;
        Position += movement;

        if (!Bounds.Intersects(other.Bounds))
        {
            return;
        }

        // Si el personaje venía cayendo desde arriba del enemigo, lo apoyamos sobre él.
        if (previousBounds.Bottom <= other.Bounds.Top + 2 && movement.Y >= 0)
        {
            Position = new Vector2(Position.X, other.Bounds.Top - Height);
            return;
        }

        // Si venía desde abajo, lo empuja para afuera.
        if (previousBounds.Top >= other.Bounds.Bottom - 2 && movement.Y <= 0)
        {
            Position = new Vector2(Position.X, other.Bounds.Bottom);
            return;
        }

        // Si hubo solapamiento por derecha, lo empuja hacia la izquierda.
        if (previousBounds.Right <= other.Bounds.Left + 2 && movement.X >= 0)
        {
            Position = new Vector2(other.Bounds.Left - Width, Position.Y);
            return;
        }

        // Si hubo solapamiento por izquierda, lo empuja hacia la derecha.
        if (previousBounds.Left >= other.Bounds.Right - 2 && movement.X <= 0)
        {
            Position = new Vector2(other.Bounds.Right, Position.Y);
            return;
        }
    }

    // Mantiene el collider dentro del rectángulo de la pantalla.
    public void KeepInside(Rectangle screenBounds)
    {
        float maxX = Math.Max(screenBounds.Left, screenBounds.Right - Width);
        float maxY = Math.Max(screenBounds.Top, screenBounds.Bottom - Height);

        Position = new Vector2(
            Math.Clamp(Position.X, screenBounds.Left, maxX),
            Math.Clamp(Position.Y, screenBounds.Top, maxY)
        );
    }

    // Ajusta la altura desde abajo manteniendo el pie en el mismo lugar.
    public void SetHeightFromBottom(int height)
    {
        float bottomY = Position.Y + Height;
        Height = Math.Max(1, height);
        Position = new Vector2(Position.X, bottomY - Height);
    }
}
