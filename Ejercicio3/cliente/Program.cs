using System;
using System.Net.Sockets;
using System.Threading;
using NetworkStreamNS;
using VehiculoClass;
using CarreteraClass;

class Cliente
{
    static void Main()
    {
        TcpClient client = new TcpClient();
        try
        {
            client.Connect("127.0.0.1", 10001);
            if (client.Connected)
            {
                Console.WriteLine("✅ Cliente conectado al servidor.");
                NetworkStream stream = client.GetStream();

                // ✅ Creación del vehículo con dirección aleatoria y posición inicial correcta
                Vehiculo vehiculo = new Vehiculo();
                vehiculo.Direccion = new Random().Next(0, 2) == 0 ? "Norte" : "Sur";
                vehiculo.Pos = (vehiculo.Direccion == "Norte") ? 0 : 100;
                vehiculo.Velocidad = new Random().Next(60, 120);

                // 📤 Convertir a XML y enviarlo al servidor
                string xmlEnviado = System.Text.Encoding.UTF8.GetString(vehiculo.VehiculoaBytes());
                Console.WriteLine($"📤 XML enviado al servidor:\n{xmlEnviado}");
                NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);

                // 📥 Esperar que el servidor asigne un ID antes de continuar
                try
                {
                    Vehiculo vehiculoRecibido = NetworkStreamClass.LeerDatosVehiculoNS(stream);
                    if (vehiculoRecibido == null)
                    {
                        Console.WriteLine("❌ Error al recibir datos del servidor.");
                        return;
                    }
                    vehiculo.Id = vehiculoRecibido.Id;
                    Console.WriteLine($"✅ Vehículo recibido con ID: {vehiculo.Id}, Dirección: {vehiculo.Direccion}, Velocidad: {vehiculo.Velocidad} km/h");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error en la recepción de datos: {ex.Message}");
                    return;
                }

                // 🔹 Crear hilo para escuchar actualizaciones del servidor
                Thread hiloRecepcion = new Thread(() => RecibirDatosCarretera(stream));
                hiloRecepcion.Start();

                // 🚗 **Bucle de movimiento del vehículo**
                while ((vehiculo.Direccion == "Norte" && vehiculo.Pos < 100) ||
                       (vehiculo.Direccion == "Sur" && vehiculo.Pos > 0))
                {
                    vehiculo.Pos += (vehiculo.Direccion == "Norte") ? 1 : -1;

                    // 📌 Simulación de avance basado en velocidad
                    int tiempoEspera = (int)(1000 / (vehiculo.Velocidad / 3.6));
                    Thread.Sleep(tiempoEspera);

                    Console.WriteLine($"🚗 Vehículo {vehiculo.Id} avanzando. Posición: {vehiculo.Pos}");
                    NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);
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
                carretera.MostrarVehiculos();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al recibir datos del servidor: {ex.Message}");
        }
    }
}
