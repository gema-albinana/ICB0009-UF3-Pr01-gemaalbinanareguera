using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetworkStreamNS;
using CarreteraClass;
using VehiculoClass;


using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

class Servidor
{
    private static SemaphoreSlim semaforo = new SemaphoreSlim(1, 1); // Controla el túnel
    private static string vehiculoEnTunel = ""; // Almacena el vehículo que está cruzando
    private static int contadorVehiculos = 1; // Contador de vehículos
    private static Queue<string> vehiculosEsperando = new Queue<string>(); // Cola de vehículos esperando

    static void Main()
    {
        TcpListener servidor = new TcpListener(IPAddress.Parse("127.0.0.1"), 12345);
        servidor.Start();
        Console.WriteLine("🚦 Servidor iniciado. Esperando conexiones...");

        while (true)
        {
            TcpClient cliente = servidor.AcceptTcpClient();
            Console.WriteLine("✅ Cliente conectado.");
            ThreadPool.QueueUserWorkItem(AtenderCliente, cliente);
        }
    }

    static void AtenderCliente(object obj)
    {
        TcpClient cliente = (TcpClient)obj;
        NetworkStream stream = cliente.GetStream();
        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

        try
        {
            // Leer los datos del vehículo
            string datosVehiculo = reader.ReadLine();
            Console.WriteLine($"📥 Datos del vehículo recibidos.");

            // Asignar un número secuencial a cada vehículo y obtener su dirección
            string vehiculoID = $"Vehículo {contadorVehiculos++}";
            string direccion = datosVehiculo.Contains("Norte") ? "Norte" : "Sur";

            // Si el túnel está libre o el vehículo está en la misma dirección
            semaforo.Wait(); // Asegura que solo un vehículo pueda cruzar el túnel a la vez

            if (string.IsNullOrEmpty(vehiculoEnTunel) || vehiculoEnTunel == direccion)
            {
                vehiculoEnTunel = direccion;  // Asignar la dirección del vehículo que está cruzando
                writer.WriteLine($"📥 🚗 {vehiculoID} ({direccion}) CRUZANDO túnel.");
                Console.WriteLine($"🚗 {vehiculoID} ({direccion}) CRUZANDO túnel...");

                // Simula el cruce del túnel
                Thread.Sleep(2000); // Simula que el vehículo está cruzando el túnel

                writer.WriteLine($"✅ {vehiculoID} ha cruzado el túnel.");
                Console.WriteLine($"✅ {vehiculoID} ha cruzado el túnel.");
                
                // Verificar si hay vehículos esperando
                if (vehiculosEsperando.Count > 0)
                {
                    string siguienteVehiculo = vehiculosEsperando.Dequeue();
                    Console.WriteLine($"🚗 {siguienteVehiculo} ahora puede cruzar el túnel.");
                }
                vehiculoEnTunel = ""; // Libera el túnel para el siguiente vehículo
            }
            else
            {
                vehiculosEsperando.Enqueue($"{vehiculoID} ({direccion})");
                writer.WriteLine($"❌ {vehiculoID} ({direccion}) esperando en dirección {direccion}. Túnel ocupado.");
                Console.WriteLine($"🚗 {vehiculoID} ({direccion}) esperando en dirección {direccion}. Túnel ocupado.");
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"❌ Error en la conexión: {ex.Message}");
        }
        finally
        {
            semaforo.Release(); // Libera el semáforo para que otro vehículo pueda cruzar
            cliente.Close(); // Cierra la conexión del cliente
        }
    }
}
