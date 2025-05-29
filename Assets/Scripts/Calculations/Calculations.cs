using System;
using System.Linq;

public static class Calculations
{
    public const int hexWidth = 26;
    public const int hexHeight = 23;
    public static string CalcNewSeed(string seed, int level)
    {
        seed = seed.PadLeft(12, '0');
        string tempSeed, newSeed;

        Random rand = new Random(level);
        tempSeed = new string(Enumerable.Repeat(0, 12).Select(_ => (char)(rand.Next(10) + '0')).ToArray());

        newSeed = new string(tempSeed.Zip(seed, (a, b) => (char)(((a - '0') ^ (b - '0')) % 10 + '0')).ToArray());

        return newSeed;
    }
    public static void CalcCoord(int n, ref int x, ref int y) // Вычисление координат центра комнаты по её номеру
    {
        if (n == 0)
        {
            x = 0; y = 0; return;
        }
        int distanceFromCenter = (int)Math.Floor(0.5 + (Math.Sqrt(12 * n - 3) / 6));
        int numInLayer = n - (1 + 3 * distanceFromCenter * (distanceFromCenter - 1));
        if (numInLayer == 0 || numInLayer == distanceFromCenter * 3)
        {
            y = 0;
            x = (numInLayer == 0) ? hexWidth * distanceFromCenter : (-hexWidth * distanceFromCenter);
        }
        else if (distanceFromCenter * 2 + 1 > numInLayer % (distanceFromCenter * 3)
            && numInLayer % (distanceFromCenter * 3) > (distanceFromCenter - 1))
        {
            double temp = numInLayer % (distanceFromCenter * 3) - 1.5 * distanceFromCenter;
            if (numInLayer / (distanceFromCenter * 3) == 0)
            {
                y = distanceFromCenter * hexHeight;
                x = (int)(-hexWidth * temp);
            }
            else
            {
                y = -hexHeight * distanceFromCenter;
                x = (int)(hexWidth * temp);
            }
        }
        else
        {
            int temp2 = numInLayer % (distanceFromCenter * 3);
            if (temp2 <= distanceFromCenter - 1)
            {
                x = hexWidth * distanceFromCenter - hexWidth / 2 * temp2;
                y = hexHeight * temp2;
            }
            else
            {
                x = hexWidth / 2 * distanceFromCenter - hexWidth / 2 * temp2;
                y = hexHeight * (-temp2 + distanceFromCenter * 3);
            }
            if (numInLayer / (distanceFromCenter * 3) != 0)
            {
                x *= -1;
                y *= -1;
            }
        }
    }
    public static int CalcNumOfNeighbour(int n, int k) // Вычисление комнаты, соседней от комнаты с номером n в направлении k 
    {
        // n - номер комнаты, k - направление движения от текущей комнаты.
        // l - номер слоя, n_l - номер в слое.

        if (n == 0)
            return k + 1;

        int l = (int)Math.Floor(0.5 + (Math.Sqrt(12 * n - 3) / 6));
        int n_l = n - (1 + 3 * l * (l - 1));

        if (n_l == 0 && (k == 4 || k == 5))
            return 3 * (l + k - 4) * (l + k - 3);
        else if ((n_l == 6 * l - 1) && (k == 1 || k == 2))
            return (l > 1 || k == 1) ? (1 + 3 * (l - k) * (l + 1 - k)) : 0;
        else if (l * (k - 1) <= n_l && n_l <= l * (k + 1))
            return n + 6 * l + k;
        else if (n_l >= l * 5 && k == 0)
            return n + 6 * (l + 1);
        else if (n_l > l * 5 && k == 3)
            return n - 6 * l;
        else if (l * ((k + 1) % 6) < n_l && (n_l <= ((k != 4) ? (l * ((k + 2) % 6)) : l * (k + 2))))
            return n - 1;
        else if (k < 2 && (l * (k + 4) <= n_l && n_l < l * (k + 5)) ||
            l * ((k + 4) % 6) <= n_l && n_l < l * ((k + 5) % 6))
            return n + 1;
        else if (l > 1)
            return n - 6 * (l - 1) - (3 + k) % 6;
        else
            return 0;
    }
    public static int CalcNumOfExtraRooms(string seed) // Вычисление количества дополнительных комнат по сиду уровня
    {
        int sum = seed.Sum(c => c - '0');

        while (sum >= 10)
        {
            sum = sum.ToString().Sum(c => c - '0');
        }

        return sum;
    }
}
