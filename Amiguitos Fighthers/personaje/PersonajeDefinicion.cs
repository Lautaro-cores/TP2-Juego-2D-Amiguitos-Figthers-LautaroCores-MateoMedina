using MonoGameLibrary.Graphics;

namespace Amiguitos_Fighthers;

//este archivo define a los personajes y sus habilidades,
//para que puedan ser instanciados en el juego


public sealed class PersonajeDefinicion
{
    public string Nombre { get; }
    public int VidaMaxima { get; }
    public Habilidad[] Habilidades { get; }
    public string AnimacionIdle { get; }
    public string AnimacionAtaque { get; }

    public string AnimacionLanzar { get; }

    public PersonajeDefinicion(
        string nombre,
        int vidaMaxima,
        Habilidad[] habilidades,
        string animacionIdle,
        string animacionAtaque,
        string animacionLanzar)
    {
        Nombre = nombre;
        VidaMaxima = vidaMaxima;
        Habilidades = habilidades;
        AnimacionIdle = animacionIdle;
        AnimacionAtaque = animacionAtaque;
        AnimacionLanzar = animacionLanzar;
    }

    public static PersonajeDefinicion CrearGuerrero(TextureAtlas atlas)
    {
        Habilidad[] habilidades =
        {
            new HabilidadGolpe(
                nombre: "Golpe cargado",
                danio: 35,
                alcance: 80.0f,
                sprite: atlas.CreateAnimatedSprite("Attack")),
            
            new HabilidadProyectil(
                nombre: "Bola de fuego",
                danio:20,
                velocidad: 10.0f,
                sprite: atlas.CreateAnimatedSprite("Lanzar"),
                spriteProyectil: atlas.CreateAnimatedSprite("FireBall"))
        };


        return new PersonajeDefinicion(
            nombre: "Guerrero",
            vidaMaxima: 1200,
            habilidades: habilidades,
            animacionIdle: "Idle",
            animacionAtaque: "Attack",
            animacionLanzar: "Lanzar");
    }
}
