using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Lab4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Operaciones operaciones1 = new Operaciones();
            while (true)
            {
                Console.WriteLine("1. Cargar Datos");
                Console.WriteLine("2. Generar firmas y cifrar conversaciones");
                Console.WriteLine("3. Decifrar conversaciones y validar firmas");
                Console.WriteLine("4. salir del programa");
                Console.WriteLine("Escoga una opcion");
                int opcion = Convert.ToInt16(Console.ReadLine());

                switch (opcion)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("Cargar Datos");
                        operaciones1.CargarDatos();
                        break;

                    case 2:
                        Console.Clear();
                        Console.WriteLine("Generar Firma y cifrar conversaciones");
                        operaciones1.GenerarFirmasYCodificar();
                        break;
                    case 3:
                        Console.Clear();
                        Console.WriteLine("Decifrar conversaciones");
                        operaciones1.DecifrarConversaciones();
                        break;
                    case 4:
                        Console.WriteLine("Salir del programa");
                        Console.ReadKey();
                        break;
                }
            }          
        }
    }
}
