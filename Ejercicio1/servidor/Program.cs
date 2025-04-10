using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Servidor
{
    static TcpListener ServidorTcp = new TcpListener(IPAddress.Parse("127.0.0.1"), 10001);
    static int contadorID = 0; // ID único para cada bicicleta
    static object lockObj = new object(); // Objeto para proteger el contador en hilos

    static void Main(string[] args)
    {
        ServidorTcp.Start();
        Console.WriteLine("🚦 Servidor iniciado. Esperando conexiones...");

        while (true) // Permitir múltiples conexiones
        {
            TcpClient cliente = ServidorTcp.AcceptTcpClient();
            Console.WriteLine("✅ Cliente conectado.");

            // Crear un nuevo hilo para gestionar al cliente
            Thread clienteThread = new Thread(() => GestionarCliente(cliente));
            clienteThread.Start();
        }
    }

    static void GestionarCliente(TcpClient cliente)
    {
        int idVehiculo;
        string direccionAleatoria;

        // Proteger la asignación de ID con lock
        lock (lockObj)
        {
            idVehiculo = ++contadorID;
        }

        // Generar dirección aleatoria (norte o sur)
        direccionAleatoria = (new Random().Next(2) == 0) ? "Norte" : "Sur";

        Console.WriteLine($"🚲 Bicicleta {idVehiculo} asignada. Dirección: {direccionAleatoria}");

        // Aquí podríamos agregar más lógica en el futuro (como enviar el ID al cliente)
    }
}
