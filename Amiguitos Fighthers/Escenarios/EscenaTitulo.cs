using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;

namespace Amiguitos_Fighthers;

public sealed class EscenaTitulo : Scene
{
    private Texture2D _portada;

    // Paneles de Gum
    private Panel _panelMenu;
    private Panel _panelOpciones;

    // Botones a los que hay que devolverle el foco
    private Button _botonJugar;
    private Button _botonOpciones;
    private Button _botonVolver;

    public override void LoadContent()
    {
        _portada = Content.Load<Texture2D>("pruebas/portada");
    }

    public override void Initialize()
    {
        // LoadContent se llama dentro de base.Initialize()
        base.Initialize();
        InicializarUI();
    }

    private void InicializarUI()
    {
        // Limpia cualquier UI previa por si venimos de otra escena
        GumService.Default.Root.Children.Clear();

        CrearPanelMenu();
        CrearPanelOpciones();
    }

    private void CrearPanelMenu()
    {
        _panelMenu = new Panel();
        _panelMenu.Dock(Gum.Wireframe.Dock.Fill);
        _panelMenu.AddToRoot();

        // El canvas de Gum es 1/4 de la resolución (320x180), por eso los valores chicos
        _botonJugar = new Button();
        _botonJugar.Anchor(Gum.Wireframe.Anchor.Center);
        _botonJugar.Y = 10;
        _botonJugar.Width = 100;
        _botonJugar.Height = 0.01f;
        _botonJugar.Text = "JUGAR";
        _botonJugar.Click += AlHacerClickJugar;
        _panelMenu.AddChild(_botonJugar);

        _botonOpciones = new Button();
        _botonOpciones.Anchor(Gum.Wireframe.Anchor.Center);
        _botonOpciones.Y = 40;
        _botonOpciones.Width = 100;
        _botonOpciones.Height = 0.01f;
        _botonOpciones.Text = "OPCIONES";
        _botonOpciones.Click += AlHacerClickOpciones;
        _panelMenu.AddChild(_botonOpciones);

        var botonSalir = new Button();
        botonSalir.Anchor(Gum.Wireframe.Anchor.Center);
        botonSalir.Y = 70;
        botonSalir.Width = 100;
        botonSalir.Height = 0.01f;
        botonSalir.Text = "SALIR";
        botonSalir.Click += AlHacerClickSalir;
        _panelMenu.AddChild(botonSalir);

        // El foco inicial permite navegar con teclado/gamepad
        _botonJugar.IsFocused = true;
    }

    private void CrearPanelOpciones()
    {
        _panelOpciones = new Panel();
        _panelOpciones.Dock(Gum.Wireframe.Dock.Fill);
        _panelOpciones.IsVisible = false;
        _panelOpciones.AddToRoot();

        var titulo = new TextRuntime();
        titulo.X = 10;
        titulo.Y = 10;
        titulo.Text = "OPCIONES";
        _panelOpciones.AddChild(titulo);

        var etiquetaMusica = new Label();
        etiquetaMusica.Text = "Musica";
        etiquetaMusica.X = 25;
        etiquetaMusica.Y = 35;
        _panelOpciones.AddChild(etiquetaMusica);

        var sliderMusica = new Slider();
        sliderMusica.Anchor(Gum.Wireframe.Anchor.Top);
        sliderMusica.Y = 30f;
        sliderMusica.Minimum = 0;
        sliderMusica.Maximum = 1;

        sliderMusica.SmallChange = .1;
        sliderMusica.LargeChange = .2;
        sliderMusica.ValueChanged += AlCambiarVolumenMusica;
        _panelOpciones.AddChild(sliderMusica);

        var etiquetaSfx = new Label();
        etiquetaSfx.Text = "Efectos";
        etiquetaSfx.X = 25;
        etiquetaSfx.Y = 80;
        _panelOpciones.AddChild(etiquetaSfx);

        var sliderSfx = new Slider();
        sliderSfx.Anchor(Gum.Wireframe.Anchor.Top);
        sliderSfx.Y = 93;
        sliderSfx.Minimum = 0;
        sliderSfx.Maximum = 1;

        sliderSfx.SmallChange = .1;
        sliderSfx.LargeChange = .2;
        sliderSfx.ValueChanged += AlCambiarVolumenEfectos;
        _panelOpciones.AddChild(sliderSfx);

        _botonVolver = new Button();
        _botonVolver.Text = "VOLVER";
        _botonVolver.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _botonVolver.X = -28f;
        _botonVolver.Y = -10f;
        _botonVolver.Click += AlHacerClickVolver;
        _panelOpciones.AddChild(_botonVolver);
    }

    // ----- Eventos -----

    private void AlHacerClickJugar(object sender, EventArgs e)
    {
        Core.ChangeScene(new EscenaSeleccionPersonaje());
    }

    private void AlHacerClickOpciones(object sender, EventArgs e)
    {
        _panelMenu.IsVisible = false;
        _panelOpciones.IsVisible = true;
        _botonVolver.IsFocused = true;
    }

    private void AlHacerClickSalir(object sender, EventArgs e)
    {
        Core.Instance.Exit();
    }

    private void AlHacerClickVolver(object sender, EventArgs e)
    {
        _panelOpciones.IsVisible = false;
        _panelMenu.IsVisible = true;
        _botonOpciones.IsFocused = true;
    }

    private void AlCambiarVolumenMusica(object sender, EventArgs e)
    {
        var slider = (Slider)sender;

    }

    private void AlCambiarVolumenEfectos(object sender, EventArgs e)
    {
        var slider = (Slider)sender;

    }

    // ----- Ciclo de la escena -----

    public override void Update(GameTime gameTime)
    {
        GumService.Default.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice graphicsDevice = Core.GraphicsDevice;
        Rectangle bounds = graphicsDevice.Viewport.Bounds;
        graphicsDevice.Clear(Color.Black);

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        Core.SpriteBatch.Draw(_portada, bounds, Color.White);
        Core.SpriteBatch.End();

        // Gum dibuja la UI encima de la portada
        GumService.Default.Draw();
    }

    public override void UnloadContent()
    {
        // Evita que los botones de esta escena queden en la siguiente
        GumService.Default.Root.Children.Clear();
        base.UnloadContent();
    }
}