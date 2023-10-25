using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Security.Cryptography;
namespace Lab4
{
   
    public class RSAGenerator
    {
        public BigInteger N { get; private set; }
        public BigInteger E { get; private set; }
        public BigInteger D { get; private set; }

        public RSAGenerator(int bitSize)
        {
            // Genera dos números primos grandes p y q
            BigInteger p = GenerarNumeroPrimo(bitSize);
            BigInteger q = GenerarNumeroPrimo(bitSize);

            // Calcula el módulo N
            N = p * q;

            // Calcula la función totient de Euler (phi)
            BigInteger phi = (p - 1) * (q - 1);

            // Encuentra un número 'e' que sea coprimo con phi (usualmente, un número primo pequeño)
            E = EncontrarCoPrimo(phi);

            // Encuentra el inverso modular 'd' de 'e' (d * e ≡ 1 (mod phi))
            D = CalcularInversoModular(E, phi);
        }

        public static BigInteger GenerarNumeroPrimo(int bitSize)
        {
            Random random = new Random();
            BigInteger numeroPrimo;

            do
            {
                numeroPrimo = GenerarNumeroPositivo(bitSize);
            } while (!EsPrimo(numeroPrimo));

            return numeroPrimo;
        }

        public static BigInteger GenerarNumeroPositivo(int bitSize)
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[bitSize / 8];
                rng.GetBytes(bytes);
                BigInteger numero = new BigInteger(bytes);
                numero = BigInteger.Abs(numero);

                if (numero < 2)
                {
                    numero += 2; // Asegurarse de que sea mayor o igual a 2
                }

                return numero;
            }
        }


        public static bool EsPrimo(BigInteger n)
        {
            if (n <= 1)
                return false;
            if (n <= 3)
                return true;
            if (n % 2 == 0 || n % 3 == 0)
                return false;

            for (BigInteger i = 5; i * i <= n; i += 6)
            {
                if (n % i == 0 || n % (i + 2) == 0)
                    return false;
            }
            return true;
        }

        public static BigInteger EncontrarCoPrimo(BigInteger phi)
        {
            // Devuelve un número primo pequeño que sea coprimo con phi
            BigInteger e = 3; // Comúnmente se usa 3 como valor inicial
            while (BigInteger.GreatestCommonDivisor(e, phi) != 1)
            {
                e += 2; // Incrementa en 2 para garantizar que sea impar
            }
            return e;
        }

        public static BigInteger CalcularInversoModular(BigInteger a, BigInteger m)
        {
            BigInteger m0 = m;
            BigInteger x0 = 0;
            BigInteger x1 = 1;

            while (a > 1)
            {
                BigInteger q = a / m;
                BigInteger t = m;

                m = a % m;
                a = t;
                t = x0;
                x0 = x1 - q * x0;
                x1 = t;
            }

            if (x1 < 0)
            {
                x1 += m0;
            }

            return x1;
        }

      
    }
}
