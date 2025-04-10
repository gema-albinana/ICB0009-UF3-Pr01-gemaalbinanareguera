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
    static Carretera carretera = new Carretera();
    static object lockObj = new object();

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

        lock (lockObj)
        {
            // 📌 Asignar un ID secuencial
            vehiculo.Id = carretera.NumVehiculosEnCarrera + 1;
            carretera.AñadirVehiculo(vehiculo);
        }

        Console.WriteLine($"🚗 Vehículo {vehiculo.Id} asignado. Dirección: {vehiculo.Direccion}");

        // 📤 Enviar el vehículo con ID asignado al cliente
        NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);

        // 📌 Mostrar los vehículos en la carretera
        Console.WriteLine("🚦 Vehículos en carretera:");
        carretera.MostrarBicicletas();
    }
}
