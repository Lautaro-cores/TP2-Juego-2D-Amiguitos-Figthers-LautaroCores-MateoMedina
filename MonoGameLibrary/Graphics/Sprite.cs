using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class Sprite 
{

// Los atributos de la clase de Sprite
public TextureRegion Region { get; set; }

public Color Color { get; set; } = Color.White;

public float Rotation { get; set; } = 0.0f;

public Vector2 Scale { get; set; } = Vector2.One;

public Vector2 Origin { get; set; } = Vector2.Zero;

public SpriteEffects Effects { get; set; } = SpriteEffects.None;

public float LayerDepth { get; set; } = 0.0f;

public float Width => Region.Width * Scale.X;

public float Height => Region.Height * Scale.Y;





// Los contructores de la clase

// Constructor default para configurar despues
public Sprite() { }

// Constructor con una region ya especificada
public Sprite(TextureRegion region)
{
    Region = region;
}






// Metodos de la clase



// Establece el punto de origen del spite direntamente en el centro de este
public void CenterOrigin()
{
    Origin = new Vector2(Region.Width, Region.Height) * 0.5f;
}


public void Draw(SpriteBatch spriteBatch, Vector2 position)
{
    Region.Draw(spriteBatch, position, Color, Rotation, Origin, Scale, Effects, LayerDepth);
}

}
