using System;
using System.Collections.Generic;
using MonoGameLibrary.Graphics;

namespace Amiguitos_Fighthers;

public sealed record PersonajeSeleccion(
    string Nombre,
    string Imagen,
    string ImagenNombre,
    string ImagenTarjeta,
    string AtlasContenido,
    int Indice);

public sealed class PersonajeDefinicion
{
    // Esta lista es el registro de personajes disponibles. El indice representa
    // la posicion usada por EscenaSeleccionPersonaje para seleccionar el retrato.
    // AtlasContenido indica que archivo XML y que atlas del personaje debe cargar
    // EscenaCombate despues de confirmar la seleccion.
    public static IReadOnlyList<PersonajeSeleccion> Personajes { get; } =
        Array.AsReadOnly(new[]
        {
            new PersonajeSeleccion("Chufo", "PersonajeSeleccion/Chufo", "PersonajeLetras/Chufo", "PersonajeTarjetas/Chufo", "AtlasPersonaje/ChufoAtlas.xml", 0),
            new PersonajeSeleccion("El Mimo", "PersonajeSeleccion/Elmimo", "PersonajeLetras/Elmimo", "PersonajeTarjetas/Elmimo", "AtlasPersonaje/Atlas-Elmimo.xml", 1),
            new PersonajeSeleccion("Nacio", "PersonajeSeleccion/Igna", "PersonajeLetras/Nacio", "PersonajeTarjetas/Nacio", "AtlasPersonaje/AtlasNasio.xml", 2),
            new PersonajeSeleccion("Rana Magica", "PersonajeSeleccion/RanaMagica", "PersonajeLetras/RanaMagica", "PersonajeTarjetas/RanaMagica", "AtlasPersonaje/ranaMagicaAtlas.xml", 3),
            new PersonajeSeleccion("Rumstein", "PersonajeSeleccion/RumStein", "PersonajeLetras/RumStein", "PersonajeTarjetas/Rumstein", "AtlasPersonaje/AtlasRumMan.xml", 4),
            new PersonajeSeleccion("Tortuliano", "PersonajeSeleccion/Tortuliano", "PersonajeLetras/Tortuliano", "PersonajeTarjetas/tortuliano", "AtlasPersonaje/AtlasTortuliano.xml", 5),
        });

    public string Nombre { get; }
    public int VidaMaxima { get; }
    public MovimientoBasico MovimientoBasico { get; }

    public string AnimacionIdle { get; }
    public string AnimacionAtaque { get; }

    public string AnimacionBloquear { get; }

    public string AnimacionAgacharse { get; }

    public string AnimacionGolpeAgachado { get; }

    public string AnimacionPatadaAgachado { get; }

    // Animaciones especiales que solo se usan con golpe y patada ligera en el aire.
    public string AnimacionGolpeAire { get; }

    public string AnimacionPatadaAire { get; }

    public string AnimacionAire { get; }

    public string SpriteSalto { get; }

    public PersonajeDefinicion(
        string nombre,
        int vidaMaxima,
        MovimientoBasico movimientoBasico,
        string animacionIdle,
        string animacionAtaque,
        string animacionBloquear,
        string animacionAgacharse,
        string animacionGolpeAgachado,
        string animacionPatadaAgachado,
        string animacionGolpeAire,
        string animacionPatadaAire,
        string animacionAire,
        string spriteSalto
        )
    {
        Nombre = nombre;
        VidaMaxima = vidaMaxima;
        MovimientoBasico = movimientoBasico;
        AnimacionIdle = animacionIdle;
        AnimacionAtaque = animacionAtaque;
        AnimacionBloquear = animacionBloquear;
        AnimacionAgacharse = animacionAgacharse;
        AnimacionGolpeAgachado = animacionGolpeAgachado;
        AnimacionPatadaAgachado = animacionPatadaAgachado;
        AnimacionGolpeAire = animacionGolpeAire;
        AnimacionPatadaAire = animacionPatadaAire;
        AnimacionAire = animacionAire;
        SpriteSalto = spriteSalto;
    }

    public static PersonajeDefinicion Crear(PersonajeSeleccion personaje, TextureAtlas atlas)
    {
        // EscenaSeleccionPersonaje entrega el PersonajeSeleccion elegido.
        // EscenaCombate entrega el atlas asociado a ese personaje.
        return personaje.Indice switch
        {
            0 or 1 or 2 or 3 or 4 or 5 => CrearPersonaje(personaje, atlas),
            _ => throw new InvalidOperationException($"Indice de personaje desconocido: {personaje.Indice}")
        };
    }

    private static PersonajeDefinicion CrearPersonaje(
        PersonajeSeleccion personaje,
        TextureAtlas atlas,
        string animacionIdle = "idle",
        string animacionAtaque = "caminar",
        string animacionBloquear = "bloquear",
        string animacionAgacharse = "agacharse")
    {
        // Crea la configuracion completa con el método común. Todos los personajes,
        // incluidos Chufo y Rana Magica, usan las animaciones normalizadas que existen
        // dentro del atlas entregado por la selección.
        return new PersonajeDefinicion(
            nombre: personaje.Nombre,
            vidaMaxima: 1200,
            movimientoBasico: new MovimientoBasico(
                velocidadMovimiento: 5.0f,
                velocidadSalto: 900.0f,
                gravedad: 1500.0f,
                danioGolpeLigero: 25,
                danioGolpeMedio: 50,
                danioGolpePesado: 75,
                danioPatadaLigera: 30,
                danioPatadaMedia: 60,
                danioPatadaPesada: 100,
                animacionGolpeLigero: "puñoLigero",
                animacionGolpeMedio: "puñoMedio",
                animacionGolpePesado: "puñoPesado",
                animacionPatadaLigera: "patadaLigera",
                animacionPatadaMedia: "patadaMedia",
                animacionPatadaPesada: "patadaPesada"),
            animacionIdle: animacionIdle,
            animacionAtaque: animacionAtaque,
            animacionBloquear: animacionBloquear,
            animacionAgacharse: animacionAgacharse,
            animacionGolpeAgachado: "golpeAgachado",
            animacionPatadaAgachado: "patadaAgachado",
            animacionGolpeAire: "puñoAire",
            animacionPatadaAire: "patadaAire",
            animacionAire: "aire",
            spriteSalto: "salto"
            );
    }
}
