using System;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // Crear el cliente TCP
            TcpClient client = new TcpClient();

            try
            {
                // Conectar al servidor
                client.Connect("127.0.0.1", 10001);
                if (client.Connected)
                {
                    Console.WriteLine("✅ Cliente conectado al servidor.");

                    // Obtener el flujo de red para la comunicación con el servidor
                    NetworkStream stream = client.GetStream();
                    Console.WriteLine("📡 NetworkStream obtenido para la conexión con el servidor.");
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
