using System;
using System.Net.Sockets;
using NetworkStreamNS; 

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

                    // 📡 Obtener el flujo de comunicación con el servidor
                    NetworkStream stream = client.GetStream();

                    // 📥 Recibir mensaje del servidor
                    string mensaje = NetworkStreamClass.LeerMensajeNetworkStream(stream);
                    Console.WriteLine($"🔹 Mensaje desde el servidor: {mensaje}");
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
