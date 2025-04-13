using System;
using System.Net.Sockets;
using System.Threading;
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

                // 🚗 Crear vehículo sin ID (lo asigna el servidor)
                Vehiculo vehiculo = new Vehiculo() { Pos = 0, Direccion = "Norte", Velocidad = 500 }; // Velocidad en milisegundos

                // 📤 Enviar el vehículo sin ID al servidor
                NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);
                Console.WriteLine("📤 Vehículo enviado sin ID, el servidor asignará uno.");

                // 📥 Recibir vehículo con ID asignado por el servidor
                vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);
                Console.WriteLine($"✅ Vehículo recibido con ID: {vehiculo.Id}");

                // 🚗 **Mover el vehículo**
                while (vehiculo.Pos < 100)
                {
                    vehiculo.Pos += 1; // 🚗 Avanzar una posición
                    Console.WriteLine($"🚗 Vehículo {vehiculo.Id} avanzando. Posición: {vehiculo.Pos}");

                    NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo); // 📤 Enviar actualización al servidor
                    Thread.Sleep(vehiculo.Velocidad); // ⏳ Control de velocidad
                }

                // 🏁 Marcar vehículo como "Acabado"
                vehiculo.Acabado = true;
                NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);
                Console.WriteLine($"🏁 Vehículo {vehiculo.Id} completó su recorrido.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al conectar con el servidor: {ex.Message}");
        }
    }
}
