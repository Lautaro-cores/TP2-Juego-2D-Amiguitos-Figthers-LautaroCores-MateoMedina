using System;



namespace Amiguitos_Fighthers;


//esto se va a encargar de manejar el estado de los personajes, 
// como la vida y el daño que reciben


public abstract class Personaje
{
    public int Vida { get; private set; }
    public int VidaMaxima { get; }
    public float PorcentajeVida => Vida / (float)VidaMaxima;

    protected Personaje(int vidaInicial)
    {

        VidaMaxima = vidaInicial;
        Vida = vidaInicial;
    }

    public void RecibirDanio(int danio)
    {
        if (danio <= 0)
        {
            return;
        }

        Vida = Math.Max(0, Vida - danio);
    }
}