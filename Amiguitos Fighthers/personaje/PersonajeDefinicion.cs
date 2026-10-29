using MonoGameLibrary.Graphics;

namespace Amiguitos_Fighthers;

//este archivo define a los personajes y sus habilidades,
//para que puedan ser instanciados en el juego


public sealed class PersonajeDefinicion
{
    public string Nombre { get; }
    public int VidaMaxima { get; }
    public MovimientoBasico MovimientoBasico { get; }
    public Habilidad[] Habilidades { get; }
    public string AnimacionIdle { get; }
    public string AnimacionAtaque { get; }

    public string AnimacionBloquear { get; }

    public string AnimacionAgacharse { get; }




    public PersonajeDefinicion(
        string nombre,
        int vidaMaxima,
        MovimientoBasico movimientoBasico,
        Habilidad[] habilidades,
        string animacionIdle,
        string animacionAtaque,
        string animacionBloquear,
        string animacionAgacharse
        )
    {
        Nombre = nombre;
        VidaMaxima = vidaMaxima;
        MovimientoBasico = movimientoBasico;
        Habilidades = habilidades;
        AnimacionIdle = animacionIdle;
        AnimacionAtaque = animacionAtaque;
        AnimacionBloquear = animacionBloquear;
        AnimacionAgacharse = animacionAgacharse;
    }

    public static PersonajeDefinicion CrearGuerrero(TextureAtlas atlas)
    {
        Habilidad[] habilidades =
        {
            new HabilidadGolpe(
                nombre: "Golpe cargado",
                danio: 35,
                sprite: atlas.CreateAnimatedSprite("Attack"),
                altoHitbox: 30,
                anchoHitbox: 30),

        };


        return new PersonajeDefinicion(
            nombre: "Guerrero",
            vidaMaxima: 1200,
            // Los parámetros de movimiento básico se configuran individualmente por personaje.
            movimientoBasico: new MovimientoBasico(
                danio: 35,
                velocidadMovimiento: 5.0f,
                velocidadSalto: 900.0f,
                gravedad: 1500.0f,
                anchoHitbox: 30,
                altoHitbox: 30),
            habilidades: habilidades,
            animacionIdle: "Idle",
            animacionAtaque: "Attack",
            animacionBloquear: "BloquearYCaminar",
            animacionAgacharse: "Agacharse");
    }
}
