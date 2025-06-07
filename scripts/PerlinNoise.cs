using System;
using Godot;

namespace godot_planets.scripts;

public class PerlinNoise {
    public static float PerlinNoise2D(int seed, float x, float y) {
        x = Math.Abs(x);
        y = Math.Abs(y);

        return (float)Math.Sin(x+y);
    }
}