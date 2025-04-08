using System;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        static TcpClient client = new TcpClient();


        static void Main(string[] args)
        {
            client = new TcpClient();

            try
            {
                client.Connect("127.0.0.1", 10001);
                if (client.Connected)
                {
                    Console.WriteLine("Cliente:✅ Cliente conectado ");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al conectar con el servidor: {ex.Message}");
            }

            Console.ReadLine();  // Mantener la consola abierta
        }
    }
}
