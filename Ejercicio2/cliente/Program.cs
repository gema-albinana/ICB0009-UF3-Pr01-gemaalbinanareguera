using System;
using System.Net.Sockets;
using System.Threading;
using NetworkStreamNS;
using VehiculoClass;
using CarreteraClass;


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

                Vehiculo vehiculo = new Vehiculo() { Pos = 0, Direccion = "Norte", Velocidad = 500 };
                NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);

                vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);
                Console.WriteLine($"✅ Vehículo recibido con ID: {vehiculo.Id}");

                // 🔹 Crear hilo para escuchar actualizaciones del servidor
                Thread hiloRecepcion = new Thread(() => RecibirDatosCarretera(stream));
                hiloRecepcion.Start();

                // 🚗 Bucle de movimiento del vehículo
                while (vehiculo.Pos < 100)
                {
                    vehiculo.Pos++;
                    Console.WriteLine($"🚗 Vehículo {vehiculo.Id} avanzando. Posición: {vehiculo.Pos}");
                    NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);
                    Thread.Sleep(vehiculo.Velocidad);
                }

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

    static void RecibirDatosCarretera(NetworkStream stream)
    {
        try
        {
            while (true)
            {
                Carretera carretera = NetworkStreamClass.LeerDatosCarreteraNS(stream);
                Console.WriteLine("🚦 Estado de la carretera:");
                carretera.MostrarBicicletas();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al recibir datos del servidor: {ex.Message}");
        }
    }
}
