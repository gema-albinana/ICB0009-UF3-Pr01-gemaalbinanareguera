using System;
using System.Net.Sockets;
using System.IO;
using VehiculoClass;
using NetworkStreamNS;

class Cliente
{
    static void Main()
    {
        try
        {
            var cliente = new TcpClient("127.0.0.1", 12345);
            var stream = cliente.GetStream();

            // Crear y enviar el vehículo sin asignar un ID aleatorio
            Vehiculo vehiculo = new Vehiculo()
            {
                Direccion = new Random().Next(0, 2) == 0 ? "Norte" : "Sur"
            };

            NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);
            Console.WriteLine($"📤 Enviando datos del vehículo al servidor.");

            // Leer la respuesta del servidor
            string respuesta = NetworkStreamClass.LeerMensajeNetworkStream(stream);
            Console.WriteLine($"📥 {respuesta}");

            if (respuesta.Contains("esperando"))
            {
                Console.WriteLine("🚗 El vehículo está esperando para entrar al puente...");
            }
            else if (respuesta.Contains("CRUZANDO"))
            {
                Console.WriteLine("🚗 El vehículo está cruzando el puente...");
            }

            // Esperar la confirmación final
            string finalResponse = NetworkStreamClass.LeerMensajeNetworkStream(stream);
            Console.WriteLine($"📥 {finalResponse}");

            cliente.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en la conexión: {ex.Message}");
        }
    }
}
