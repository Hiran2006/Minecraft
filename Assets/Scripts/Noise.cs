using System;

public class Noise
{
    // Seed is the only true global instance variable
    public int Seed { get; private set; }
    private readonly int[] _p = new int[512];

    public Noise(int? seed = null)
    {
        Seed = seed ?? new Random().Next(0, 100000);
        InitializePermutationTable();
    }

    private void InitializePermutationTable()
    {
        int[] permutation = new int[256];
        for (int i = 0; i < 256; i++) permutation[i] = i;

        Random rand = new Random(Seed);
        for (int i = 255; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            int temp = permutation[i];
            permutation[i] = permutation[j];
            permutation[j] = temp;
        }

        for (int i = 0; i < 256; i++)
        {
            _p[i] = permutation[i];
            _p[i + 256] = permutation[i];
        }
    }

    // --- Perlin Math Helpers ---
    private double Fade(double t) => t * t * t * (t * (t * 6 - 15) + 10);
    private double Lerp(double t, double a, double b) => a + t * (b - a);

    private double Grad2d(int hash, double x, double y)
    {
        int h = hash & 7;
        double u = h < 4 ? x : y;
        double v = h < 4 ? y : x;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    private double Grad3d(int hash, double x, double y, double z)
    {
        int h = hash & 15;
        double u = h < 8 ? x : y;
        double v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    private double RawNoise2d(double x, double y)
    {
        int X = (int)Math.Floor(x) & 255;
        int Y = (int)Math.Floor(y) & 255;
        x -= Math.Floor(x); y -= Math.Floor(y);
        double u = Fade(x); v = Fade(y);
        int aa = _p[_p[X] + Y], ab = _p[_p[X] + Y + 1], ba = _p[_p[X + 1] + Y], bb = _p[_p[X + 1] + Y + 1];
        return Lerp(v, Lerp(u, Grad2d(aa, x, y), Grad2d(ba, x - 1, y)),
                       Lerp(u, Grad2d(ab, x, y - 1), Grad2d(bb, x - 1, y - 1)));
    }

    private double RawNoise3d(double x, double y, double z)
    {
        int X = (int)Math.Floor(x) & 255; int Y = (int)Math.Floor(y) & 255; int Z = (int)Math.Floor(z) & 255;
        x -= Math.Floor(x); y -= Math.Floor(y); z -= Math.Floor(z);
        double u = Fade(x); double v = Fade(y); double w = Fade(z);
        int A = _p[X] + Y; int AA = _p[A] + Z; int AB = _p[A + 1] + Z;
        int B = _p[X + 1] + Y; int BA = _p[B] + Z; int BB = _p[B + 1] + Z;
        return Lerp(w, Lerp(v, Lerp(u, Grad3d(_p[AA], x, y, z), Grad3d(_p[BA], x - 1, y, z)),
                               Lerp(u, Grad3d(_p[AB], x, y - 1, z), Grad3d(_p[BB], x - 1, y - 1, z))),
                       Lerp(v, Lerp(u, Grad3d(_p[AA + 1], x, y, z - 1), Grad3d(_p[BA + 1], x - 1, y, z - 1)),
                               Lerp(u, Grad3d(_p[AB + 1], x, y - 1, z - 1), Grad3d(_p[BB + 1], x - 1, y - 1, z - 1))));
    }

    // --- Public Generic API ---

    /// <summary>
    /// Generates 2D fractal noise mapped between 0.0 and 1.0 with passing configurations.
    /// </summary>
    public double Noise2d(double x, double y, int octaves = 1, double scale = 1.0, double offsetX = 0.0, double offsetY = 0.0, double persistence = 0.5)
    {
        // Inputs transformed dynamically within execution scope
        double sampledX = (x + offsetX) * scale;
        double sampledY = (y + offsetY) * scale;

        double total = 0;
        double frequency = 1;
        double amplitude = 1;
        double maxValue = 0;

        for (int i = 0; i < octaves; i++)
        {
            double normalizedNoise = (RawNoise2d(sampledX * frequency, sampledY * frequency) + 1) / 2.0;
            total += normalizedNoise * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;
        }

        return total / maxValue;
    }

    /// <summary>
    /// Generates 3D fractal noise mapped between 0.0 and 1.0 with passing configurations.
    /// </summary>
    public double Noise3d(double x, double y, double z, int octaves = 1, double scale = 1.0, double offsetX = 0.0, double offsetY = 0.0, double offsetZ = 0.0, double persistence = 0.5)
    {
        double sampledX = (x + offsetX) * scale;
        double sampledY = (y + offsetY) * scale;
        double sampledZ = (z + offsetZ) * scale;

        double total = 0;
        double frequency = 1;
        double amplitude = 1;
        double maxValue = 0;

        for (int i = 0; i < octaves; i++)
        {
            double normalizedNoise = (RawNoise3d(sampledX * frequency, sampledY * frequency, sampledZ * frequency) + 1) / 2.0;
            total += normalizedNoise * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;
        }

        return total / maxValue;
    }
}