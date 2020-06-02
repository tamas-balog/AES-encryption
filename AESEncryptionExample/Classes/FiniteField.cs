using System;
using System.Text;

namespace AESEncryptionExample.Classes
{
    public class FiniteField
    {
        public byte[] Coefficients { get; set; }

        public static FiniteField IrreduciblePolynomial
        {
            get
            {
                return new FiniteField(new byte[] { 1, 0, 0, 0, 1, 1, 0, 1, 1 });
            }
        }

        public static FiniteField C
        {
            get
            {
                return new FiniteField(new byte[] { 0, 1, 1, 0, 0, 0, 1, 1 });
            }
        }

        public static FiniteField InvC
        {
            get
            {
                return new FiniteField(new byte[] { 0, 0, 0, 0, 0, 1, 0, 1 });
            }
        }

        public int Count
        {
            get
            {
                return Coefficients.Length;
            }
        }
        public byte this[int i]
        {
            get { return Coefficients[i]; }
            set { Coefficients[i] = value; }
        }

        public int FirstNonZero
        {
            get
            {
                for (int i = 0; i < Count; i++)
                {
                    if (Coefficients[i] == 1)
                    {
                        return i;
                    }
                }
                return Count;
            }
        }

        public FiniteField()
        {
            Coefficients = new byte[8];
        }

        public FiniteField(int length)
        {
            Coefficients = new byte[length];
        }
        public FiniteField(byte[] coefficients)
        {
            Coefficients = coefficients;
        }
        public FiniteField(string coefficients)
        {
            Coefficients = new byte[coefficients.Length];
            for (int i = 0; i < coefficients.Length; i++)
            {
                Coefficients[i] = Convert.ToByte(coefficients[i].ToString());
            }
        }
        public static FiniteField operator *(FiniteField a, FiniteField b)
        {
            FiniteField product = new FiniteField(a.Count + b.Count - 1);
            for (int i = 0; i < a.Count; i++)
            {
                for (int j = 0; j < b.Count; j++)
                {
                    product[i + j] = Convert.ToByte(a[i] & b[j] ^ product[i + j]);
                }
            }
            product.Cut();
            return product;
        }

        public void Cut()
        {
            while (Coefficients.Length - FirstNonZero >= IrreduciblePolynomial.Count)
            {
                FiniteField reminder = this - IrreduciblePolynomial;
                Coefficients = reminder.Coefficients;
            }

            byte[] reduced = new byte[8];

            int length = Coefficients.Length - 8;
            for (int i = 0; i < 8; i++)
            {
                reduced[i] = Coefficients[length + i];
            }
            Coefficients = reduced;
        }

        public static FiniteField operator -(FiniteField a, FiniteField b)
        {
            if (a < b)
            {
                FiniteField c = new FiniteField(a.Count);
                c = a;
                a = b;
                b = c;
            }

            FiniteField temp = a.Clone();
            int aStart = a.FirstNonZero;
            int bStart = b.FirstNonZero;

            if (b == new FiniteField())
            {
                return a;
            }

            for (int i = 0; i < b.Count - bStart; i++)
            {
                temp[i + aStart] = Convert.ToByte(a[i + aStart] ^ b[i + bStart]);
            }

            return temp;
        }

        public static FiniteField operator %(FiniteField a, FiniteField b)
        {
            FiniteField reminder = a.Clone();

            while (reminder >= b)
            {
                reminder -= b;
            }

            return reminder;
        }
        static int Compare(FiniteField a, FiniteField b)
        {
            int indexA = 0;
            int indexB = 0;
            if (a.Count > b.Count)
            {
                while (indexA < a.Count - b.Count)
                {
                    if (a[indexA] == 1)
                    {
                        return 1;
                    }
                    indexA++;
                }
            }

            if (b.Count > a.Count)
            {
                while (indexB < b.Count - a.Count)
                {
                    if (b[indexB] == 1)
                    {
                        return -1;
                    }
                    indexB++;
                }
            }

            for (int i = 0; i < Math.Min(a.Count, b.Count); i++)
            {
                if (a[i + indexA] < b[i + indexB])
                {
                    return -1;
                }
                else if (a[i + indexA] > b[i + indexB])
                {
                    return 1;
                }
            }
            return 0;
        }

        public static bool operator ==(FiniteField lhs, FiniteField rhs)
        {
            return Compare(lhs, rhs) == 0;
        }

        public static bool operator !=(FiniteField lhs, FiniteField rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator >(FiniteField lhs, FiniteField rhs)
        {
            return Compare(lhs, rhs) == 1;
        }

        public static bool operator <(FiniteField lhs, FiniteField rhs)
        {
            return Compare(lhs, rhs) == -1;
        }

        public static bool operator >=(FiniteField lhs, FiniteField rhs)
        {
            return Compare(lhs, rhs) != -1;
        }

        public static bool operator <=(FiniteField lhs, FiniteField rhs)
        {
            return Compare(lhs, rhs) != 1;
        }

        public static FiniteField operator <<(FiniteField a, int count)
        {
            FiniteField temp = new FiniteField(a.Count);
            int index = temp.Count - count;

            for (int i = 0; i < a.Count; i++)
            {
                temp[index] = a[i];
                index++;
                index %= a.Count;
            }

            return temp;
        }

        public static FiniteField operator ~(FiniteField a)
        {
            if (a == new FiniteField())
            {
                return new FiniteField();
            }
            FiniteField t = new FiniteField();
            FiniteField newt = new FiniteField("00000001");
            FiniteField r = IrreduciblePolynomial;
            FiniteField newr = a;

            while (newr != new FiniteField())
            {
                FiniteField quotient = r / newr;

                FiniteField prov2 = newr;
                newr = r + quotient * prov2;
                r = prov2;

                FiniteField prov = newt;
                newt = t + quotient * prov;
                t = prov;
            }

            return t;
        }

        public static FiniteField operator +(FiniteField a, FiniteField b)
        {
            FiniteField temp;
            int shift;
            if (a.Count < b.Count)
            {
                temp = new FiniteField(b.Count);
                shift = b.Count - a.Count;
                for (int i = 0; i < shift; i++)
                {
                    temp[i] = b[i];
                }
                for (int i = 0; i < a.Count; i++)
                {
                    temp[i + shift] = Convert.ToByte(a[i] ^ b[i + shift]);
                }
            }
            else if (b.Count < a.Count)
            {
                temp = new FiniteField(a.Count);
                shift = a.Count - b.Count;
                for (int i = 0; i < shift; i++)
                {
                    temp[i] = a[i];
                }
                for (int i = 0; i < b.Count; i++)
                {
                    temp[i + shift] = Convert.ToByte(a[i + shift] ^ b[i]);
                }
            }
            else
            {
                temp = new FiniteField(a.Count);
                for (int i = 0; i < a.Count; i++)
                {
                    temp[i] = Convert.ToByte(a[i] ^ b[i]);
                }
            }
            temp.Cut();

            return temp;
        }

        public static FiniteField operator /(FiniteField a, FiniteField b)
        {
            FiniteField quotient = new FiniteField(a.Count);
            FiniteField temp = a.Clone();
            while (temp.Count - temp.FirstNonZero >= b.Count - b.FirstNonZero)
            {
                int aFirst = temp.Count - temp.FirstNonZero;
                int bFirst = b.Count - b.FirstNonZero;
                int shift = Math.Abs(aFirst - bFirst);
                quotient[quotient.Count - 1 - shift] = 1;
                temp -= b;
            }

            return quotient;
        }
        public static FiniteField SBox(FiniteField b)
        {
            FiniteField s = ~b;
            return s + (s << 1) + (s << 2) + (s << 3) + (s << 4) + C;
        }

        public static FiniteField InvSBox(FiniteField s)
        {
            FiniteField b = (s << 1) + (s << 3) + (s << 6) + InvC;
            return ~b;
        }

        public FiniteField Clone()
        {
            return new FiniteField((byte[])Coefficients.Clone());
        }

        public override bool Equals(object obj)
        {
            return Compare(this, (FiniteField)obj) == 0;
        }

        public override int GetHashCode()
        {
            return Coefficients.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in Coefficients)
            {
                stringBuilder.Append(item);
            }

            return stringBuilder.ToString();
        }

        public string ToHexaString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            double count = 0;
            for (int i = 0; i < 4; i++)
            {
                count += Math.Pow(2, 3 - i) * Coefficients[i];
            }

            char c;
            switch (count)
            {
                case 10:
                    c = 'A';
                    break;
                case 11:
                    c = 'B';
                    break;
                case 12:
                    c = 'C';
                    break;
                case 13:
                    c = 'D';
                    break;
                case 14:
                    c = 'E';
                    break;
                case 15:
                    c = 'F';
                    break;
                default:
                    c = Convert.ToChar(count.ToString());
                    break;
            }
            stringBuilder.Append(c);

            count = 0;
            for (int i = 0; i < 4; i++)
            {
                count += Math.Pow(2, 3 - i) * Coefficients[i + 4];
            }
            switch (count)
            {
                case 10:
                    c = 'A';
                    break;
                case 11:
                    c = 'B';
                    break;
                case 12:
                    c = 'C';
                    break;
                case 13:
                    c = 'D';
                    break;
                case 14:
                    c = 'E';
                    break;
                case 15:
                    c = 'F';
                    break;
                default:
                    c = Convert.ToChar(count.ToString());
                    break;
            }
            stringBuilder.Append(c);
            return stringBuilder.ToString();
        }
    }
}
