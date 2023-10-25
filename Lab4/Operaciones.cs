using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
//1016156267944
namespace Lab4
{
    public class CSVData //clase para leer el archivo csv
    {
        public CSVData() { }

        public CSVData(string v1, string v2)
        {
            operacion = v1;
            JSONData = v2;
        }

        public string operacion { get; set; }
        public string JSONData { get; set; }

    }

    public class Persona
    {
        public Persona(string nameC, string dpiC, string datebirthC, string adrressC)
        {
            name = nameC;
            dpi = dpiC;
            datebirth = datebirthC;
            address = adrressC;
        }
        public Persona() { }
        public string name { get; set; }
        public string dpi { get; set; }
        public string datebirth { get; set; }
        public string address { get; set; }
        public List<string> companies { get; set; }
    }

    internal class Operaciones
    {
        TreeNode InfoPersona = new TreeNode(); //arbol
        public void CargarDatos()
        {

            string[] CsvLines = System.IO.File.ReadAllLines(@"C:\Users\maria\Desktop\inputs (3)\input(1).csv");

            //Listas auxiliares
            List<CSVData> insert = new List<CSVData>();
            List<CSVData> patch = new List<CSVData>();
            List<CSVData> eliminar = new List<CSVData>();

            for (int i = 0; i < CsvLines.Length; i++)
            {
                string[] rowdata = CsvLines[i].Split(';'); // lee el separador ";" 
                CSVData record = new CSVData(rowdata[0], rowdata[1]); //se inserta en la clase que contiene el jsondata y la operacion

                if (rowdata[0] == "INSERT")
                {
                    insert.Add(record);
                }
                else if (rowdata[0] == "PATCH")
                {
                    patch.Add(record);
                }
                else if (rowdata[0] == "DELETE")
                {
                    eliminar.Add(record);
                }
            }

            //Insertar en arbol 
            foreach (CSVData item in insert)
            {
                Persona person = JsonConvert.DeserializeObject<Persona>(item.JSONData);
                InfoPersona.Insertar(person);
            }

            //Actualizar informacion 
            string dpi = "";
            foreach (var item in patch)
            {
                Persona personaPatch = JsonConvert.DeserializeObject<Persona>(item.JSONData);
                dpi = personaPatch.dpi;
                InfoPersona.ActualizarNodo(dpi, personaPatch);
            }

            //Eliminar informacion          
            foreach (var item in eliminar)
            {
                Persona personaDelete = JsonConvert.DeserializeObject<Persona>(item.JSONData);
                InfoPersona.Eliminar(personaDelete);
            }
        }

        //public static BigInteger publica, N, privada;
            Dictionary<string, byte[]> Firmasporarchivos = new Dictionary<string, byte[]>();
        Dictionary<string, byte[]> hashdefirmas = new Dictionary<string, byte[]>();
        public string clave = "SECRET"; //Clave para transpocision simple 

        //PRUEBA
        //BigInteger privateKey = new BigInteger(123456);
        //BigInteger publicKey = new BigInteger(789012);

        //BigInteger n = 33;
        //BigInteger j = 7;
        //BigInteger k = 3;

        

        public void GenerarFirmasYCodificar()
        {
            //Lee la carpeta donde se encuentran los archivos
            string carpeta = @"C:\Users\maria\Desktop\inputs (3)\inputs";
            string Cifrado = @"C:\Users\maria\Desktop\inputs (3)\Cifrado";
            string nombre, dpi;

            //Crear una instancia
            //RSAGenerator rsaGenerator = new RSAGenerator(5);
            //publica = rsaGenerator.E; 
            //N = rsaGenerator.N;
            //privada = rsaGenerator.D;
            // byte[] firma;


            if (Directory.Exists(carpeta))
            {
                string[] archivos = Directory.GetFiles(carpeta, "CONV-*.txt");
                foreach (var item in archivos)
                {
                    // Obtener el contenido del archivo
                    string contenido = File.ReadAllText(item);
                    //Cifrar
                    //obtener nombre completo del txt
                    nombre = Path.GetFileNameWithoutExtension(item);
                    //separamos el nombre del archivo para obtener el dpi
                    string[] partesNombre = nombre.Split('-');
                    dpi = partesNombre[1];
                    //Cifrar contenido
                    string contenidoCifrado = Cifrar(contenido, clave);
                    // Crear una nueva ruta para el archivo cifrado en la carpeta de salida
                    string rutaArchivoCifrado = Path.Combine(Cifrado, $"{nombre}-Cifrado.txt");
                    string nombreaux = $"{nombre}-Cifrado";
                    ///Guardar nuevo archivo en una nueva carpeta
                    File.WriteAllText(rutaArchivoCifrado, contenidoCifrado);
                    byte[] messageBytes = CalcularHash(contenido);
                    //Generar firma
                    // Firma el hash del mensaje con la clave privada
                    Firmasporarchivos[nombreaux] = Firmar(contenido, messageBytes);   //CalcularFirma(contenidoCifrado, j, n);
                    hashdefirmas[nombreaux] = messageBytes;
                }
            }

        }

        public void DecifrarConversaciones()
        {
            string ArchivoCifrado = @"C:\Users\maria\Desktop\inputs (3)\Cifrado";
            string nombreArchivo, dpi, DPIBuscar;
            string contenidoCifrado;
            string contenidoDescifrado = "";

            Console.WriteLine("Ingrese DPI de la persona que desea buscar");
            DPIBuscar = Console.ReadLine();

            Persona personaEncontrada = InfoPersona.BuscarPorDPI(DPIBuscar);
            if (personaEncontrada != null)
            {
                Console.WriteLine("Persona encontrada(Nombre): " + personaEncontrada.name);
                Console.WriteLine("Persona encontrada(DPI): " + personaEncontrada.dpi);
                Console.WriteLine("Persona encontrada(Direccion): " + personaEncontrada.address);
                Console.WriteLine();

                if (Directory.Exists(ArchivoCifrado))
                {
                    string[] archivosCifrados = Directory.GetFiles(ArchivoCifrado, "CONV-*.txt");
                    foreach (var item in archivosCifrados)
                    {
                        // Obtener nombre completo del archivo cifrado
                        nombreArchivo = Path.GetFileNameWithoutExtension(item);
                        
                        // Separar el nombre del archivo para obtener el DPI
                        string[] partesNombre = nombreArchivo.Split('-');
                        dpi = partesNombre[1];

                        if (dpi == DPIBuscar)
                        {
                            // Obtener el contenido cifrado del archivo
                            contenidoCifrado = File.ReadAllText(item);
                         
                           // Descifrar el contenido
                            contenidoDescifrado = Descifrar(contenidoCifrado, clave);
                            // Mostrar el contenido descifrado
                            Console.WriteLine($"Contenido descifrado de {nombreArchivo}:");
                            Console.WriteLine();
                            Console.WriteLine(contenidoDescifrado);
                            Console.WriteLine("-------------------------------------");

                            byte[] hashcant = CalcularHash(contenidoDescifrado);
                            //verificar firma 
                            byte[] firmaRecuperada;
                            byte[] hashhhhh;
                            if (Firmasporarchivos.ContainsKey(nombreArchivo))
                            {
                                // Acceder a la firma asociada al nombre del archivo
                                firmaRecuperada = Firmasporarchivos[nombreArchivo];
                                hashhhhh = hashdefirmas[nombreArchivo];
                                bool sonIguales = CompararArreglos(hashcant, hashhhhh);

                                if (sonIguales)
                                {
                                    Console.WriteLine("Son iguales");
                                }
                                //CalcularFirma(contenidoCifrado, j, n);
                                Validar(hashcant, firmaRecuperada);

                            }

                        }
                    }

                    
                }

            }
            else
            {
                Console.WriteLine("Persona no encontrada.");
            }
        }

        //Calcualr Hash
        public static byte[] CalcularHash(string input)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);
                return hashBytes;
            }
        }


        //Cifrar contenido carta
        public string Cifrar(string mensaje, string clave)
        {
            int numRows = (int)Math.Ceiling((double)mensaje.Length / clave.Length);
            char[,] grid = new char[numRows, clave.Length];

            int messageIndex = 0;

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < clave.Length; j++)
                {
                    if (messageIndex < mensaje.Length)
                    {
                        grid[i, j] = mensaje[messageIndex];
                        messageIndex++;
                    }
                    else
                    {
                        grid[i, j] = ' ';
                    }
                }
            }

            int[] order = new int[clave.Length];
            for (int i = 0; i < clave.Length; i++)
            {
                order[i] = i;
            }

            for (int i = 0; i < clave.Length - 1; i++)
            {
                for (int j = i + 1; j < clave.Length; j++)
                {
                    if (clave[i] > clave[j])
                    {
                        char temp = clave[i];
                        clave = clave.Remove(i, 1).Insert(i, clave[j].ToString());
                        clave = clave.Remove(j, 1).Insert(j, temp.ToString());

                        int tempOrder = order[i];
                        order[i] = order[j];
                        order[j] = tempOrder;
                    }
                }
            }

            string mensajeCifrado = "";

            for (int i = 0; i < clave.Length; i++)
            {
                int col = Array.IndexOf(order, i);

                for (int j = 0; j < numRows; j++)
                {
                    mensajeCifrado += grid[j, col];
                }
            }

            return mensajeCifrado;
        }

        //decifrar conversaciones
        public string Descifrar(string mensajeCifrado, string clave)
        {
            int numRows = (int)Math.Ceiling((double)mensajeCifrado.Length / clave.Length);
            char[,] grid = new char[numRows, clave.Length];

            int[] order = new int[clave.Length];
            for (int i = 0; i < clave.Length; i++)
            {
                order[i] = i;
            }

            for (int i = 0; i < clave.Length - 1; i++)
            {
                for (int j = i + 1; j < clave.Length; j++)
                {
                    if (clave[i] > clave[j])
                    {
                        char temp = clave[i];
                        clave = clave.Remove(i, 1).Insert(i, clave[j].ToString());
                        clave = clave.Remove(j, 1).Insert(j, temp.ToString());

                        int tempOrder = order[i];
                        order[i] = order[j];
                        order[j] = tempOrder;
                    }
                }
            }

            int colLength = mensajeCifrado.Length / numRows;
            int index = 0;

            for (int i = 0; i < clave.Length; i++)
            {
                int col = Array.IndexOf(order, i);

                for (int j = 0; j < numRows; j++)
                {
                    grid[j, col] = mensajeCifrado[index];
                    index++;
                }
            }

            string mensajeDescifrado = "";

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < clave.Length; j++)
                {
                    mensajeDescifrado += grid[i, j];
                }
            }

            return mensajeDescifrado.Trim();
        }

        //Generar firma digital
        //public static RSAGenerator rsaGenerator = new RSAGenerator(50);
        //BigInteger publica = rsaGenerator.E;
        //BigInteger privada = rsaGenerator.D;
        //BigInteger N = rsaGenerator.N;

        public static RSA rsaG = new RSA(20);
        BigInteger n = rsaG.N;
        BigInteger publica = rsaG.E;
        BigInteger privada = rsaG.D;
        BigInteger o = rsaG.E;
        

        public byte[] Firmar(string message, byte[] canthash)
        {   
            BigInteger messageHashBigInt = new BigInteger(canthash);
            BigInteger digitalSignature = BigInteger.ModPow(messageHashBigInt, privada, n);

            return digitalSignature.ToByteArray();            
        }

        //validar firma
        public void Validar(byte[] messaghash, byte[] digitalSignature)
        {

            byte[] messageBytes = messaghash;
           
            BigInteger signature = new BigInteger(digitalSignature);
            BigInteger hash = new BigInteger(messaghash);
            BigInteger decryptedSignature = BigInteger.ModPow(signature, publica, n);
            byte[] decryptedBytes = decryptedSignature.ToByteArray();   
            // Comparar el resumen del mensaje con la firma desencriptada
            bool sonIguales = CompararArreglos(messageBytes, decryptedBytes);
            if (sonIguales)
            {
                Console.WriteLine("Es valida");
            }
        }

        public static bool CompararArreglos(byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length)
            {
                return false;
            }

            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                {
                    return false;
                }
            }

            return true;
        }

    }
}
