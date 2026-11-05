using System;
using System.Collections.Generic;
using MonoGameLibrary.Graphics;

namespace Amiguitos_Fighthers;

public sealed record PersonajeSeleccion(
    string Nombre,
    string Imagen,
    string ImagenNombre);

public sealed class PersonajeDefinicion
{
    public static IReadOnlyList<PersonajeSeleccion> Personajes { get; } =
        Array.AsReadOnly(new[]
        {
            new PersonajeSeleccion("Chufo", "PersonajeSeleccion/Chufo", "PersonajeLetras/Chufo"),
            new PersonajeSeleccion("El Mimo", "PersonajeSeleccion/Elmimo", "PersonajeLetras/Elmimo"),
            new PersonajeSeleccion("Nacio", "PersonajeSeleccion/Igna", "PersonajeLetras/Nacio"),
            new PersonajeSeleccion("Rana Magica", "PersonajeSeleccion/RanaMagica", "PersonajeLetras/RanaMagica"),
            new PersonajeSeleccion("Rumstein", "PersonajeSeleccion/Rumstein", "PersonajeLetras/RumStein"),
            new PersonajeSeleccion("Tortuliano", "PersonajeSeleccion/Tortuliano", "PersonajeLetras/Tortuliano"),
        });

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

    public static PersonajeDefinicion Crear(PersonajeSeleccion personaje, TextureAtlas atlas)
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
            nombre: personaje.Nombre,
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
