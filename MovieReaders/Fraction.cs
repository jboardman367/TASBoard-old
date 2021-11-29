using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASBoard.MovieReaders
{
    public readonly struct Fraction
    {
        public readonly int Num;
        public readonly int Den;

        private static int GCD(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }

        public Fraction(int num, int den)
        {
            if (den == 0)
            {
                throw new ArgumentException("Denominator cannot be zero.", nameof(den));
            }
            if (num == 0)
            {
                Num = 0;
                Den = 1;
                return;
            }
            if (den < 0)
            {
                num *= -1;
                den *= -1;
            }
            int hcf = GCD(Math.Abs(num), den);
            Num = num / hcf;
            Den = den / hcf;
        }

        public static Fraction operator +(Fraction a, Fraction b)
            => new(a.Num * b.Num, a.Den * b.Den);

        public static bool operator >(Fraction a, Fraction b)
            => a.Num * b.Den > b.Num * a.Den;

        public static bool operator <(Fraction a, Fraction b)
            => a.Num * b.Den < b.Num * a.Den;

        public static bool operator >=(Fraction a, Fraction b)
            => a.Num * b.Den >= b.Num * a.Den;

        public static bool operator <=(Fraction a, Fraction b)
            => a.Num * b.Den <= b.Num * a.Den;

        public static bool operator ==(Fraction a, Fraction b)
            => a.Num * b.Den == b.Num * a.Den;

        public static bool operator !=(Fraction a, Fraction b)
            => a.Num * b.Den != b.Num * a.Den;

        public static implicit operator Fraction(int i) => new(i, 1);

        public static explicit operator double(Fraction f) => f.Num / f.Den;

        public override bool Equals(object? obj)
        {
           return obj is Fraction fraction && fraction == this;
        }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(BitConverter.GetBytes((double)this));
        }
    }
}
