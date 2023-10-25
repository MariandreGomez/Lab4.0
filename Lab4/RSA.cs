using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public class RSA
    {
        public BigInteger N { get; private set; }
        public BigInteger E { get; private set; }
        public BigInteger D { get; private set; }

        public RSA(int bytesize)
        {
            BigInteger p = GenerateRandomPrime(bytesize);
            BigInteger q = GenerateRandomPrime(bytesize);
            N = p * q;
            BigInteger z = (p - BigInteger.One) * (q - BigInteger.One);
            E = ObtenerCoprimo(z, bytesize);
            D = CalcularD(E, z);
        }

        public static BigInteger GenerateRandomPrime(int bitLength)
        {
            while (true)
            {
                BigInteger candidate = GenerateRandomBigInteger(bitLength);
                if (IsPrime(candidate))
                    return candidate;
            }
        }

        public static BigInteger GenerateRandomBigInteger(int bitLength)
        {
            byte[] data = new byte[bitLength / 8];
            Random random = new Random();
            random.NextBytes(data);
            BigInteger value = new BigInteger(data);
            return BigInteger.Abs(value);
        }

        public static bool IsPrime(BigInteger number)
        {
            if (number <= 1)
                return false;
            if (number <= 3)
                return true;
            if (number % 2 == 0 || number % 3 == 0)
                return false;

            for (BigInteger i = 5; i * i <= number; i += 6)
            {
                if (number % i == 0 || number % (i + 2) == 0)
                    return false;
            }

            return true;
        }

        public static BigInteger ObtenerCoprimo(BigInteger z, int keySize)
        {
            Random rand = new Random();
            BigInteger e;
            BigInteger one = BigInteger.One;

            do
            {
                e = GenerateRandomBigInteger(keySize, rand);
                while (BigInteger.Min(e, z) == z)
                {
                    e = GenerateRandomBigInteger(keySize, rand);
                }
            }
            while (!CalcularMCD(e, z).Equals(one));

            return e;
        }

        public static BigInteger GenerateRandomBigInteger(int bitLength, Random rand)
        {
            byte[] data = new byte[bitLength / 8];
            rand.NextBytes(data);
            data[data.Length - 1] &= 0x7F; // Asegúrate de que el bit más alto sea 0 para que el número sea positivo.
            BigInteger value = new BigInteger(data);
            return BigInteger.Abs(value);
        }

        public static BigInteger CalcularMCD(BigInteger a, BigInteger b)
        {
            while (b != BigInteger.Zero)
            {
                BigInteger temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static BigInteger CalcularD(BigInteger e, BigInteger z)
        {
            BigInteger d = ExtendedEuclidean(e, z);
            if (d < 0)
            {
                d = d + z; // Asegurarse de que D sea positivo
            }
            return d;
        }

        public static BigInteger ExtendedEuclidean(BigInteger a, BigInteger b)
        {
            BigInteger x0 = 1, x1 = 0, y0 = 0, y1 = 1;

            while (b != 0)
            {
                BigInteger q = a / b;
                BigInteger temp = a;
                a = b;
                b = temp % b;

                temp = x0;
                x0 = x1;
                x1 = temp - q * x1;

                temp = y0;
                y0 = y1;
                y1 = temp - q * y1;
            }

            return x0;
        }



    }
}
