using System;
using System.Net.Sockets;
using NetworkStreamNS;
using VehiculoClass;

class Cliente
{
    static void Main(string[] args)
    {
        TcpClient client = new TcpClient();
        try
        {
            client.Connect("127.0.0.1", 10001);
            if (client.Connected)
            {
                Console.WriteLine("✅ Cliente conectado al servidor.");
                NetworkStream stream = client.GetStream();

                // 🚗 Crear un vehículo sin ID (será asignado por el servidor)
                Vehiculo vehiculo = new Vehiculo() { Pos = 0, Direccion = "Norte" };

                // 📤 Enviar vehículo sin ID al servidor
                NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);
                Console.WriteLine("📤 Vehículo enviado sin ID, el servidor asignará uno.");

                // 📥 Recibir el vehículo con ID asignado por el servidor
                vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);
                Console.WriteLine($"✅ Vehículo recibido con ID: {vehiculo.Id}");

                Console.ReadLine(); // Mantener la consola abierta
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al conectar con el servidor: {ex.Message}");
        }
    }
}
