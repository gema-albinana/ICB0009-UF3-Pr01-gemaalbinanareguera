using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Servidor
{
    static TcpListener ServidorTcp = new TcpListener(IPAddress.Parse("127.0.0.1"), 10001);

    static void Main(string[] args)
    {
        ServidorTcp.Start();
        Console.WriteLine("🚦 Servidor iniciado. Esperando conexiones...");

        while (true) // Bucle infinito para aceptar múltiples clientes
        {
            TcpClient cliente = ServidorTcp.AcceptTcpClient(); // Espera un nuevo cliente
            Console.WriteLine("✅ Cliente conectado.");

            // Crear un nuevo hilo para gestionar al cliente
            Thread clienteThread = new Thread(() => GestionarCliente(cliente));
            clienteThread.Start();
        }
    }

    static void GestionarCliente(TcpClient cliente)
    {
        Console.WriteLine("🚗 Gestionando nuevo vehículo...");
        
    }
}
