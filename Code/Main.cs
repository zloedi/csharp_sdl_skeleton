using SDLPorts;

static class MainClass
{


static void Init() {
    Qonsole.Init();
    Qonsole.Start();
    // SDL ports
    Application.Log = s => Qonsole.Log( "Application: " + s );
    Application.Error = s => Qonsole.Error( "Application: " + s );
}

static void Tick() {
    Qonsole.HandleSDLMouseMotion( Input.mousePosition.x, Input.mousePosition.y );

    // will invoke our ticks on qonsole_tick
    Qonsole.Update();

    Qonsole.RenderGL();
}

static void Done() {
    Qonsole.OnApplicationQuit();
}

static void OnText( string txt ) {
    Qonsole.HandleSDLTextInput( txt );
}

static void OnKey( KeyCode kc ) {
    Qonsole.HandleSDLKeyDown( kc );
}

static void Main( string [] argv ) {
    Application.Init = Init;
    Application.Tick = Tick;
    Application.Done = Done;
    Application.OnText = OnText;
    Application.OnKey = OnKey;

    // main loop goes on
    Application.Run( argv );
}


} // MainClass
