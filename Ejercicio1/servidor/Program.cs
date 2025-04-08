using System;
using System.Net;
using System.Net.Sockets;

class Servidor
{
    static TcpListener ServidorTcp = new TcpListener(IPAddress.Parse("127.0.0.1"), 10001); // Inicializado aquí

    static string HostName = "localhost";

    static void Main(string[] args)
    {
        byte[] bufferLectura = new byte[1024];

        // Obtener y mostrar las IP del servidor
        IPAddress[] addresses = Dns.GetHostAddresses(HostName);
        foreach (IPAddress IP in addresses)
        {
            Console.WriteLine("Dirección IP: {0}", IP.ToString());
        }

        // Inicializar y arrancar el servidor
        ServidorTcp = new TcpListener(IPAddress.Parse("127.0.0.1"), 10001);
        ServidorTcp.Start();
        Console.WriteLine("🚦 Servidor iniciado");

        // Esperar y aceptar conexión de un cliente
        TcpClient Cliente = ServidorTcp.AcceptTcpClient();

        if (Cliente.Connected)
        {
            Console.WriteLine("✅ Cliente conectado");
        }

        // Mantener la ventana abierta
        Console.ReadLine();
    }
}
