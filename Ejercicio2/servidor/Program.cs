using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using NetworkStreamNS;
using CarreteraClass;
using VehiculoClass;

class Servidor
{
    static TcpListener ServidorTcp = new TcpListener(IPAddress.Parse("127.0.0.1"), 10001);
    static Carretera carretera = new Carretera(); // Simulación de la carretera

    static void Main(string[] args)
    {
        ServidorTcp.Start();
        Console.WriteLine("🚦 Servidor iniciado. Esperando conexiones...");

        while (true)
        {
            TcpClient cliente = ServidorTcp.AcceptTcpClient();
            Console.WriteLine("✅ Cliente conectado.");

            Thread clienteThread = new Thread(() => GestionarCliente(cliente));
            clienteThread.Start();
        }
    }

    static void GestionarCliente(TcpClient cliente)
    {
        NetworkStream stream = cliente.GetStream();

        // 📥 Recibir vehículo sin ID del cliente
        Vehiculo vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);

        vehiculo.Id = carretera.NumVehiculosEnCarrera + 1; // 📌 Asignar ID
        carretera.AñadirVehiculo(vehiculo);
        NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo); // 📤 Enviar vehículo con ID al cliente

        Console.WriteLine($"🚗 Vehículo {vehiculo.Id} asignado. Dirección: {vehiculo.Direccion}");

        // 🚗 **Bucle para actualizar la carretera con el avance del vehículo**
        while (!vehiculo.Acabado)
        {
            vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream); // 📥 Recibir datos del vehículo
            carretera.ActualizarVehiculo(vehiculo); // 🚗 Actualizar posición en la carretera

            Console.WriteLine($"🚦 Vehículo {vehiculo.Id} en carretera. Posición: {vehiculo.Pos}");

            if (vehiculo.Acabado)
            {
                Console.WriteLine($"🏁 Vehículo {vehiculo.Id} completó su recorrido.");
                break; // ⏹ Detener actualización al completar recorrido
            }
        }

        Console.WriteLine("🚦 Estado final de la carretera:");
        carretera.MostrarBicicletas(); // 📌 Mostrar vehículos en carretera
    }
}
