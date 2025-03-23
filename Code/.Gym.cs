using System;
using System.Threading;

namespace Gym
{

public static class Gym
{


public static void AfterRecompile() 
{
}

public static void GameTick_kmd( string [] argv, double dt ) {
    if ( Qonsole.Active ) {
        Thread.Sleep( 33 );
        return;
    }
}

public static void Eval_kmd( string [] argv ) {
    try {
        var text = string.Join(" ", argv, 1, argv.Length - 1);
        Qonsole.Log( text );
    } catch (Exception e) {
        Qonsole.Error( e );
    }
}


} // Gym

} // namespace
