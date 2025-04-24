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
                    Direccion = new Random().Next(0, 2) == 0 ? "Norte" : "Sur",
                    Pos = 0, // Se inicializa en 0 y luego se ajusta según la dirección
                    Velocidad = new Random().Next(60, 120) // Velocidad entre 60 y 120 km/h
                };

                // Ajuste correcto de posición según dirección
                vehiculo.Pos = (vehiculo.Direccion == "Norte") ? 0 : 100;

                // Enviar vehículo al servidor
                NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);

                // Esperar notificación para cruzar el puente
                vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);
                if (vehiculo != null)
                {
                    Console.WriteLine($"✅ Vehículo {vehiculo.Id} autorizado para cruzar el puente.");
                }
                else
                {
                    Console.WriteLine("🚦 Vehículo esperando para cruzar el puente...");
                }

                // 🚗 Bucle de movimiento del vehículo
                while ((vehiculo.Direccion == "Norte" && vehiculo.Pos < 100) ||
                       (vehiculo.Direccion == "Sur" && vehiculo.Pos > 1))
                {
                    vehiculo.Pos += (vehiculo.Direccion == "Norte") ? 1 : -1; // Dirección correcta

                    // Convertir km/h en milisegundos para simular avance realista
                    int tiempoEspera = (int)(1000 / (vehiculo.Velocidad / 3.6)); // Ajuste basado en m/s
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
}
