using System;
using System.Globalization;

namespace TapVerse.Core
{
    /// <summary>
    /// Lightweight scientific-notation number to keep idle-game ranges readable.
    /// Mantissa is kept in double range, exponent is integer base 10.
    /// </summary>
    [Serializable]
    public struct BigDouble : IComparable<BigDouble>, IEquatable<BigDouble>
    {
        private const double Tolerance = 1e-9;
        public double Mantissa;
        public int Exponent;

        public static readonly BigDouble Zero = new BigDouble(0d, 0);
        public static readonly BigDouble One = new BigDouble(1d, 0);

        public BigDouble(double mantissa, int exponent)
        {
            Mantissa = mantissa;
            Exponent = exponent;
            Normalize();
        }

        public static BigDouble FromDouble(double value)
        {
            if (Math.Abs(value) < Tolerance)
            {
                return Zero;
            }

            var exponent = (int)Math.Floor(Math.Log10(Math.Abs(value)));
            var mantissa = value / Math.Pow(10, exponent);
            return new BigDouble(mantissa, exponent);
        }

        public double ToDouble() => Mantissa * Math.Pow(10, Exponent);

        public static BigDouble operator +(BigDouble a, BigDouble b)
        {
            if (a.IsZero()) return b;
            if (b.IsZero()) return a;

            if (a.Exponent == b.Exponent)
            {
                return new BigDouble(a.Mantissa + b.Mantissa, a.Exponent);
            }

            var diff = a.Exponent - b.Exponent;
            if (diff > 15)
            {
                return a;
            }
            if (diff < -15)
            {
                return b;
            }

            if (diff > 0)
            {
                var mantissa = a.Mantissa + b.Mantissa / Math.Pow(10, diff);
                return new BigDouble(mantissa, a.Exponent);
            }
            else
            {
                var mantissa = b.Mantissa + a.Mantissa / Math.Pow(10, -diff);
                return new BigDouble(mantissa, b.Exponent);
            }
        }

        public static BigDouble operator -(BigDouble a, BigDouble b)
        {
            return a + (-b);
        }

        public static BigDouble operator -(BigDouble value)
        {
            return new BigDouble(-value.Mantissa, value.Exponent);
        }

        public static BigDouble operator *(BigDouble a, BigDouble b)
        {
            if (a.IsZero() || b.IsZero()) return Zero;
            return new BigDouble(a.Mantissa * b.Mantissa, a.Exponent + b.Exponent);
        }

        public static BigDouble operator *(BigDouble a, double scalar)
        {
            return a * FromDouble(scalar);
        }

        public static BigDouble operator /(BigDouble a, BigDouble b)
        {
            if (b.IsZero()) throw new DivideByZeroException();
            if (a.IsZero()) return Zero;
            return new BigDouble(a.Mantissa / b.Mantissa, a.Exponent - b.Exponent);
        }

        public bool IsZero() => Math.Abs(Mantissa) < Tolerance;

        private void Normalize()
        {
            if (IsZero())
            {
                Mantissa = 0d;
                Exponent = 0;
                return;
            }

            var log10 = Math.Log10(Math.Abs(Mantissa));
            var floor = Math.Floor(log10);
            Mantissa /= Math.Pow(10, floor);
            Exponent += (int)floor;

            if (Math.Abs(Mantissa) >= 10d)
            {
                Mantissa /= 10d;
                Exponent += 1;
            }
        }

        public override string ToString()
        {
            if (IsZero())
            {
                return "0";
            }

            if (Exponent < 6)
            {
                return ToDouble().ToString("N0", CultureInfo.InvariantCulture);
            }

            return $"{Mantissa:F2}e{Exponent}";
        }

        public string ToShortString()
        {
            if (IsZero()) return "0";

            var exp = Exponent;
            if (exp < 3)
            {
                return ToDouble().ToString("0.##", CultureInfo.InvariantCulture);
            }

            var suffixIndex = exp / 3;
            string[] suffixes = {"", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc"};
            if (suffixIndex < suffixes.Length)
            {
                var value = Mantissa * Math.Pow(10, exp % 3);
                return $"{value:0.##}{suffixes[suffixIndex]}";
            }

            return $"{Mantissa:0.##}e{exp}";
        }

        public int CompareTo(BigDouble other)
        {
            if (Exponent == other.Exponent)
            {
                return Mantissa.CompareTo(other.Mantissa);
            }

            return Exponent.CompareTo(other.Exponent);
        }

        public bool Equals(BigDouble other)
        {
            return Math.Abs(Mantissa - other.Mantissa) < Tolerance && Exponent == other.Exponent;
        }

        public override bool Equals(object obj)
        {
            return obj is BigDouble other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Mantissa.GetHashCode() * 397) ^ Exponent;
            }
        }

        public static bool operator >(BigDouble a, BigDouble b) => a.CompareTo(b) > 0;
        public static bool operator <(BigDouble a, BigDouble b) => a.CompareTo(b) < 0;
        public static bool operator >=(BigDouble a, BigDouble b) => a.CompareTo(b) >= 0;
        public static bool operator <=(BigDouble a, BigDouble b) => a.CompareTo(b) <= 0;
    }
}
