using System;
using System.Net.Sockets;
using System.Threading;
using NetworkStreamNS;
using VehiculoClass;
using CarreteraClass;

class Cliente
{
    static Vehiculo vehiculo = new Vehiculo();

    static void Main()
    {
        try
        {
            TcpClient client = new TcpClient("127.0.0.1", 10001);
            NetworkStream stream = client.GetStream();

            vehiculo.Direccion = new Random().Next(0, 2) == 0 ? "Norte" : "Sur";
            vehiculo.Velocidad = new Random().Next(100, 300);

            NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);
            vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);

            Console.WriteLine($"🚗 Vehículo #{vehiculo.Id}, Dirección: {vehiculo.Direccion}, Velocidad: {vehiculo.Velocidad}ms");

            new Thread(() => EscucharEstadoCarretera(stream)).Start();

            while (vehiculo.Pos < 100)
            {
                if (!vehiculo.Parado)
                {
                    vehiculo.Pos++;

                    if (EstaCercaDelPuente(vehiculo))
                        Console.WriteLine("⛔ Intentando entrar al puente...");
                    else if (HaSalidoDelPuente(vehiculo))
                        Console.WriteLine("➡️ Saliendo del puente...");
                }
                else
                {
                    Console.WriteLine($"🛑 Vehículo #{vehiculo.Id} detenido esperando turno...");
                    
                    while (vehiculo.Parado) 
                    {
                        vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);
                        Thread.Sleep(100);
                    }
                }

                NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);
                Thread.Sleep(vehiculo.Velocidad);
                vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);
            }

            vehiculo.Acabado = true;
            NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);
            Console.WriteLine("🏁 Vehículo terminó el recorrido.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    static void EscucharEstadoCarretera(NetworkStream stream)
    {
        try
        {
            while (true)
            {
                Carretera carretera = NetworkStreamClass.LeerDatosCarreteraNS(stream);
                var actual = carretera.VehiculosEnCarretera.Find(v => v.Id == vehiculo.Id);
                if (actual != null)
                {
                    vehiculo.Parado = actual.Parado;
                }
            }
        }
        catch
        {
            Console.WriteLine("🔌 Se perdió la conexión con el servidor.");
        }
    }

    static bool EstaCercaDelPuente(Vehiculo vehiculo)
    {
        return vehiculo.Pos >= 25 && vehiculo.Pos < 30;
    }

    static bool HaSalidoDelPuente(Vehiculo vehiculo)
    {
        return vehiculo.Pos >= 50;
    }
}
