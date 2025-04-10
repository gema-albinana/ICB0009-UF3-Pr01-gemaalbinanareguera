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

                    // 📤 Enviar mensaje de inicio al servidor
                    NetworkStreamClass.EscribirMensajeNetworkStream(stream, "INICIO");

                    // 📥 Recibir ID asignado del servidor
                    string idRecibido = NetworkStreamClass.LeerMensajeNetworkStream(stream);
                    Console.WriteLine($"🔹 ID recibido del servidor: {idRecibido}");

                    // 📤 Confirmar recepción enviando el mismo ID de vuelta
                    NetworkStreamClass.EscribirMensajeNetworkStream(stream, idRecibido);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al conectar con el servidor: {ex.Message}");
            }

            Console.ReadLine(); // Mantener la consola abierta
        }
    }
}
