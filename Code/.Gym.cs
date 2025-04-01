using System;
using System.Threading;

namespace Gym
{

public static class Gym
{

const int EOF = -1;

class Context {
    public bool fail;

    public string text;
    public int cb, ci;
    public int c => cb + ci >= text.Length ? EOF : text[cb + ci];

    public string terminal;

    public float constant;
}

static Context ctx = new();

public static void AfterRecompile() {
    Qonsole.TryExecute("eval \"+ 765. * -10f / .20 + 1.4 / 5\"");
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
        Eval( text );
    } catch (Exception e) {
        Qonsole.Error( e );
    }
}

static void Eval( string text ) {
    ctx.text = text;
    ctx.cb = ctx.ci = 0;
    ctx.fail = false;

    Const();
}

#if false

static void Ex_Add() {
    ExpectExpression();

    Ex_Mul();
    Or(); Ex_Add(); Op_Plus(); Ex_Mul();
    Or(); Ex_Add(); Op_Minus(); Ex_Mul();

    ExpectFail( "Enter some expression to evaluate." );
}

static void Ex_Mul() {
    ExpectExpression();

    Const();
    Or(); Ex_Mul(); Op_Mul(); Const();
    Or(); Ex_Mul(); Op_Div(); Const();
    Or(); Ex_Mul(); Op_Mod(); Const();

    ExpectFail( "Expected expression." );
}

#endif

static void Const() {
    TerminalBegin();

    Number();
    Or(); Minus(); Number();
    Or(); Plus(); Number();

    TerminalEnd( fail: "Expected a constant." );
}

static void Number() {
    PushLex();

    Numeric();
    Or(); Numeric(); Dot(); Numeric();
    Or(); Dot(); Numeric();
    Or(); Numeric(); Dot();

    PopLex();
}

static void Numeric() => ExpectRangeOneOrMore( '0', '9' );
static void Dot() => Expect( '.' );
static void Plus() => Expect( '+' );
static void Minus() => Expect( '-' );
static void Delimiter() => Expect( '+', '-', '*', '/', '%' );

static void Or() {
    Qonsole.Log( "OR" );
    if ( ctx.fail ) {
        ctx.fail = false;
        ctx.ci = ctx.cb;
    }
}

static void TerminalBegin() {
    ctx.terminal = "";
}

static void TerminalEnd( string fail ) {
    if ( ctx.fail ) {
        Qonsole.Error( fail );
    }
    Qonsole.Log( "terminal: " + ctx.terminal );
}

static void Expect( params int [] cp ) {
    while ( LookAhead( ' ' ) ) {}

    Qonsole.Print( ( char )ctx.c + " expect: " );

    foreach ( int c in cp )
        Qonsole.Print( ( char )c + " " );

    foreach ( int c in cp ) {
        if ( LookAhead( c ) ) {
            Qonsole.Print( "[ffc000]match[-]\n" );
            return;
        }
    }
    Fail();
    Qonsole.Print( "fail\n" );
}

static void ExpectRangeOneOrMore( int cFrom, int cTo ) {

    while ( Consume( ' ' ) ) {}

    Qonsole.Print( ( char )ctx.c + " expect: " + ( char )cFrom + "-" + ( char )cTo + " " );

    if ( ! LookAheadRange( cFrom, cTo ) ) {
        Fail();
        Qonsole.Print( "fail\n" );
        return;
    }

    Qonsole.Print( ( char )ctx.c + " expect: " + ( char )cFrom + "-" + ( char )cTo + " " );

    while ( LookAheadRange( cFrom, cTo ) ) {
        Qonsole.Print( ( char )ctx.c + " expect: " + ( char )cFrom + "-" + ( char )cTo + " " );
    }

    Qonsole.Print( "[ffc000]match[-]\n" );
}

static bool LookAheadRange( int cFrom, int cTo ) {
    if ( ctx.fail ) {
        return false;
    }

    if ( ctx.c == EOF ) {
        EndOfFile();
        return false;
    }

    if ( ctx.c >= cFrom && ctx.c <= cTo ) {
        ctx.terminal += ( char )ctx.c;
        ctx.ci++;
        return true;
    } 

    return false;
}

static bool LookAhead( int c ) {
    if ( ctx.fail ) {
        return false;
    }

    if ( ctx.c == EOF ) {
        EndOfFile();
        return false;
    }

    if ( ctx.c == c ) {
        ctx.terminal += ( char )ctx.c;
        ctx.ci++;
        return true;
    } 

    return false;
}

static bool Consume( int c ) {
    if ( ctx.fail ) {
        return false;
    }

    if ( ctx.c == EOF ) {
        EndOfFile();
        return false;
    }

    if ( ctx.c == c ) {
        ctx.ci++;
        return true;
    } 

    return false;
}

static void Fail() {
    ctx.fail = true;
}

static void EndOfFile() {
    Qonsole.Log( "EOF" );
}

} // Gym

} // namespace
