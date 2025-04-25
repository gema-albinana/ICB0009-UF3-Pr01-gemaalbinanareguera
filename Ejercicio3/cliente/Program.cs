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

                // Crear vehículo con dirección aleatoria y velocidad asignada correctamente
                string direccion = new Random().Next(2) == 0 ? "Norte" : "Sur";

                Vehiculo vehiculo = new Vehiculo
                {
                    Direccion = direccion,
                    Pos = (direccion == "Norte") ? 0 : 100,
                    Velocidad = new Random().Next(60, 101) // 🔥 Ahora la velocidad es entre 60 y 100 km/h
                };

                // Enviar vehículo al servidor
                NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);

                // Recibir vehículo con ID asignado por el servidor
                vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);
                if (vehiculo != null)
                {
                    Console.WriteLine($"🚗 Vehículo asignado: ID {vehiculo.Id}, Dirección: {vehiculo.Direccion}, Velocidad: {vehiculo.Velocidad} km/h");
                }
                else
                {
                    Console.WriteLine("❌ Error al recibir datos del servidor.");
                    return;
                }

                // Bucle de movimiento
                while ((vehiculo.Direccion == "Norte" && vehiculo.Pos < 100) ||
                       (vehiculo.Direccion == "Sur" && vehiculo.Pos > 0))
                {
                    vehiculo.Pos += (vehiculo.Direccion == "Norte") ? 1 : -1;

                    // Simula tiempo según velocidad (km/h -> m/s)
                    Thread.Sleep((int)(1000 / (vehiculo.Velocidad / 3.6)));

                    // Enviar actualización al servidor
                    NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);

                    Console.WriteLine($"🚗 Vehículo {vehiculo.Id} avanzando. Posición: {vehiculo.Pos} km");
                }

                // Marcar como completado
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
