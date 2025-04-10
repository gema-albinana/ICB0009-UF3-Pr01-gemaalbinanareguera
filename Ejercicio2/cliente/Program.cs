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

                // 📥 Recibir vehículo asignado desde el servidor
                Vehiculo vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);
                Console.WriteLine($"🚗 Vehículo {vehiculo.Id} asignado. Dirección: {vehiculo.Direccion}");

                while (!vehiculo.Acabado)
                {
                    // 🔄 Simular movimiento del vehículo
                    vehiculo.Pos += 1;
                    Thread.Sleep(vehiculo.Velocidad);

                    if (vehiculo.Pos >= 100)
                    {
                        vehiculo.Acabado = true;
                    }

                    Console.WriteLine($"📡 Vehículo {vehiculo.Id} avanzando. Posición: {vehiculo.Pos}");

                    // 📤 Enviar datos actualizados al servidor
                    NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);

                    // 📥 Recibir estado de la carretera
                    Carretera carretera = NetworkStreamClass.LeerDatosCarreteraNS(stream);
                    MostrarEstadoCarretera(carretera);
                }

                Console.WriteLine($"🏁 Vehículo {vehiculo.Id} completó su recorrido.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al conectar con el servidor: {ex.Message}");
        }

        Console.ReadLine();
    }

    static void MostrarEstadoCarretera(Carretera carretera)
    {
        Console.WriteLine("\n🚦 Estado de la carretera:");
        foreach (var veh in carretera.VehiculosEnCarretera)
        {
            string estado = veh.Acabado ? "🏁 Finalizado" : veh.Parado ? "⏸️ Esperando" : "🚗 En movimiento";
            Console.WriteLine($"[{veh.Direccion}] Vehículo {veh.Id}: Posición {veh.Pos} - {estado}");
        }
    }
}
