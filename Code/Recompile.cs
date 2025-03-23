#if ROSLYN

using System;
using System.Collections.Generic;
using System.Reflection;

using GalliumMath;
using SDLPorts;

public static class Recompile
{
    public enum Recompiles {
        None,
        Gym,
    }

    static string Recompile_kvar = Recompiles.Gym.ToString();
    static string RecompilePath_kvar = "Scripts";

    public static void Init() {
        HotRoslyn.Log = o => Qonsole.Log( "Roslyn: " + o );
        HotRoslyn.Error = o => Qonsole.Error( "Roslyn: " + o );
        HotRoslyn.OnCompile = OnCompile;
        HotRoslyn.ScriptsRoot = $"{Application.dataPath}{RecompilePath_kvar}";
        if ( ! HotRoslyn.ScriptsRoot.EndsWith( "/" ) ) {
            HotRoslyn.ScriptsRoot += '/';
        }
        HotRoslyn.Defines = new [] {
            "ROSLYN",
            // to know if we are in a recompiled assembly
            "ROSLYN_ASSEMBLY",
        };
        UpdateRoslynSources();
        HotRoslyn.TryInit();
    }

    public static void Done() {
        HotRoslyn.Done();
    }

    public static void Update() {
        if (Cellophane.VarChanged(nameof(RecompilePath_kvar))) {
            Done();
            Init();
            return;
        }

        if (Cellophane.VarChanged(nameof(Recompile_kvar))) {
            Qonsole.Log("Recompiles changed...");
            _rt = null;
            WhatToRecompile();
            UpdateRoslynSources();
        }
        HotRoslyn.Update();
    }

    static Recompiles WhatToRecompile() {
        Recompiles rs = 0;

        var kvar = Recompile_kvar.ToLowerInvariant();

        foreach (Recompiles val in Enum.GetValues(typeof(Recompiles))) {
            var name = val.ToString().ToLowerInvariant();
            if (name.StartsWith(kvar)) {
                rs = val;
                break;
            }
        }

        if (rs != Recompiles.None)
            return rs;

        var list = "";
        foreach (Recompiles val in Enum.GetValues(typeof(Recompiles))) {
            var name = val.ToString().ToLowerInvariant();

            if (name.Contains(kvar)) {
                rs = val;
                break;
            }

            list += val + "\n";
        }

        if (rs == Recompiles.None) {
            Qonsole.Log(list);
            Qonsole.Error($"Unknown recompile: {Recompile_kvar}");
        }

        return rs;
    }

    static void UpdateRoslynSources() {
        Recompiles rs = WhatToRecompile();
        if (rs != Recompiles.None)
            Qonsole.Log($"Roslyn sources set to {rs}");

        if (rs == Recompiles.Gym) {
            HotRoslyn.ScriptFiles = new [] {
                ".Gym.cs",
            };
            return;
        }
    }

    static Type _rt;
    static void OnRecompile(Assembly assembly, string typeName) {
        if (_rt == null)
            _rt = System.Reflection.Assembly.GetEntryAssembly().GetType(typeName);

        if (_rt != null) {
            var method = _rt.GetMethod("BeforeRecompile");
            if (method == null) {
                Qonsole.Log("No 'BeforeRecompile' method supplied.");
            } else {
                Qonsole.Log("Invoking 'BeforeRecompile'...");
                try {
                    method.Invoke(null, null);
                } catch (Exception e) {
                    Qonsole.Error(e);
                }
            }
        }

        Cellophane.ImportAndReplace(assembly);
        var type = assembly.GetType(typeName);
        if (type == null)
        {
            Qonsole.Error($"Can't find type {typeName}");
            return;
        }
        _rt = type;

        {
            var method = _rt.GetMethod("AfterRecompile");
            if ( method != null ) {
                Qonsole.Log("Invoking 'AfterRecompile'...");
                method.Invoke(null, null);
            }
        }
    }

    static void OnCompile(Assembly assembly) {
        Cellophane.ImportAndReplace(assembly);
        try {
            Recompiles rs = WhatToRecompile();

            if (rs == Recompiles.Gym) {
                OnRecompile(assembly, "Gym.Gym");
                return;
            }
        } catch (Exception e) {
            Qonsole.Error(e);
        }
    }

    public static Dictionary<string,object> Map = new();
}

#else // Leave the debug vars working when switching branches


static class Recompile {
    static string Recompile_kvar = "No Roslyn :(";
}


#endif
