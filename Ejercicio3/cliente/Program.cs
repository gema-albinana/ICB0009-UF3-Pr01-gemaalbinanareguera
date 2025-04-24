﻿using System;
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

                // Creación del vehículo con dirección aleatoria y posición inicial correcta
                Vehiculo vehiculo = new Vehiculo()
                {
                    Direccion = new Random().Next(2) == 0 ? "Norte" : "Sur",
                    Pos = 0,
                    Velocidad = new Random().Next(60, 120)
                };

                vehiculo.Pos = (vehiculo.Direccion == "Norte") ? 0 : 100;

                // ✅ Enviar vehículo al servidor antes de imprimir su información
                NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);

                // ✅ Recibir la versión actualizada del vehículo con ID asignado por el servidor
                vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);
                if (vehiculo != null)
                {
                    Console.WriteLine($"🚗 Vehículo asignado: ID {vehiculo.Id}, Dirección: {vehiculo.Direccion}, Velocidad: {vehiculo.Velocidad} km/h");
                }
                else
                {
                    Console.WriteLine("🚦 Vehículo esperando autorización del servidor...");
                    return;
                }

                // 🚗 **Bucle de movimiento con actualizaciones**
                while ((vehiculo.Direccion == "Norte" && vehiculo.Pos < 100) ||
                       (vehiculo.Direccion == "Sur" && vehiculo.Pos > 1))
                {
                    vehiculo.Pos += (vehiculo.Direccion == "Norte") ? 1 : -1;
                    Thread.Sleep((int)(1000 / (vehiculo.Velocidad / 3.6)));

                    // ✅ Enviar actualización al servidor
                    NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);

                    Console.WriteLine($"🚗 Vehículo {vehiculo.Id} avanzando. Posición: {vehiculo.Pos}");
                }

                vehiculo.Acabado = true;
                NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);
                Console.WriteLine($"🏁 Vehículo {vehiculo.Id} completó su recorrido.");

                stream.Close();
                client.Close();
                Console.WriteLine("🔌 Cliente desconectado correctamente.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al conectar con el servidor: {ex.Message}");
        }
    }
}
