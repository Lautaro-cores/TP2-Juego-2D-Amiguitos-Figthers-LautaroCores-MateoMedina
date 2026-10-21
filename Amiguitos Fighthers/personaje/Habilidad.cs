using MonoGameLibrary.Graphics;

namespace Amiguitos_Fighthers;

//aca  estoy creando las habilidades de los personajes,
//  para que puedan ser instanciadas en el juego

public abstract class Habilidad
{
    public string Nombre { get; }
    public int Danio { get; }
    public AnimatedSprite Sprite { get; }

    public int AnchoHitbox { get; }
    public int AltoHitbox { get; }

    protected Habilidad(string nombre, int danio, AnimatedSprite sprite, int anchoHitbox, int altoHitbox)
    {
        Nombre = nombre;
        Danio = danio;
        Sprite = sprite;
        AnchoHitbox = anchoHitbox;
        AltoHitbox = altoHitbox;    
    }
}

//las habilidades de embestida van a ser un ataque que va a recorrer una distancia determinada,
//  y va a terminar cuando llegue a esa distancia o toque al enemigo

public sealed class HabilidadEmbestida : Habilidad
{
    public float Distancia { get; }

    public HabilidadEmbestida(
        string nombre,
        int danio,
        float distancia,
        AnimatedSprite sprite,
        int anchoHitbox,
        int altoHitbox)
        : base(nombre, danio, sprite, altoHitbox, anchoHitbox)
    {
        Distancia = distancia;
    }
}

//las habilidades de proyectil son un tiro que van a recorrer todo el escenaro, 
// y van a desaparecer cuando toquen el fondo o al enemigo
public sealed class HabilidadProyectil : Habilidad
{
    public float Velocidad { get; }
    public AnimatedSprite SpriteProyectil { get; }

    public HabilidadProyectil(
        string nombre,
        int danio,
        float velocidad,
        AnimatedSprite sprite,
        AnimatedSprite spriteProyectil,
        int anchoHitbox,
        int altoHitbox)
        : base(nombre, danio, sprite, anchoHitbox, altoHitbox)
    {
        Velocidad = velocidad;
        SpriteProyectil = spriteProyectil;
    }
}


//las habilidades de golpe van a ser como un golpe mas fuerte nomas,
//  que va a variar la distancia del ataque
public sealed class HabilidadGolpe : Habilidad
{
    

    public HabilidadGolpe(
        string nombre,
        int danio,
        AnimatedSprite sprite,
        int anchoHitbox,
        int altoHitbox)
        : base(nombre, danio, sprite, anchoHitbox, altoHitbox)
    {
        
        
    }
}

