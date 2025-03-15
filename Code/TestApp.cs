using System;
using System.ComponentModel;

using GalliumMath;
using SDLPorts;

static class TestApp {


[Description( "Frame duration in milliseconds." )]
static bool ShowFrameTime_kvar = false;

static bool _initialized = false;

static void QonsolePreConfig_kmd( string [] _ ) {
    Application.SetTitle( "Skeleton Application" );

    // change this to wipe cfg to defaults
    Cellophane.ConfigVersion_kvar = 1;

    Qonsole.TryExecute( @"
        bind Alpha1 ""echo pressed 1"" play;
        bind K ""echo pressed K"" play;

        bind A +camera_left;
        bind D +camera_right;
        bind W +camera_up;
        bind S +camera_down;

        sdl_screen_height 1000;
        sdl_screen_width 900;
        sdl_screen_x 512;
        sdl_screen_y 40;
    " );

    Qonsole.onStoreCfg_f = () => KeyBinds.StoreConfig();

    KeyBinds.Log = s => Qonsole.Log( "Keybinds: " + s );
    KeyBinds.Error = s => Qonsole.Error( "Keybinds: " + s );

    QGL.Log = o => Qonsole.Log( "QGL: " + o );
    QGL.Error = s => Qonsole.Error( "QGL: " + s );
}

static void QonsolePostStart_kmd( string [] _ ) {
    _initialized = false;

    try {

    if ( ! Application.isPlaying ) {
        return;
    }

    HotRoslyn.Log = s => Qonsole.Log($"Roslyn: {s}");
    HotRoslyn.Error = s => Qonsole.Log($"Roslyn: {s}");
    HotRoslyn.TryInit();

    _clockDate = DateTime.UtcNow;
    _clockPrevDate = DateTime.UtcNow;
    Qonsole.Log( Guid.NewGuid() );
    _initialized = true;

    } catch ( Exception e ) {
        Qonsole.Error( e );
    }
}

static DateTime _clockDate, _clockPrevDate;
static void QonsoleTick_kmd( string [] _ ) {
    if ( ! _initialized ) {
        return;
    }

    try {

    var bgColor = new Color( 0.1f, 0.13f, 0.2f );
    QGL.LateBlit( 0, 0, QGL.ScreenWidth, QGL.ScreenHeight, color: bgColor );

    _clockDate = DateTime.UtcNow;

    // using integers leads to client outrunning the server clock
    double timeDelta = ( _clockDate - _clockPrevDate ).TotalMilliseconds;
    int timeDeltaMs = ( int )timeDelta;
    _clockPrevDate = _clockDate;

    if ( ShowFrameTime_kvar ) {
        QGL.LatePrint( ( ( int )( Time.deltaTime * 1000 ) ).ToString("00"), Screen.width - 50, 20 );
    }

    } catch ( Exception e ) {
        Qonsole.Error( e );
    }
}

static void QonsoleDone_kmd( string [] _ ) {
}


} // Main
