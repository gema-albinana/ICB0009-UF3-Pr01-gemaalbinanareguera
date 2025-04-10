using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetworkStreamNS;
using CarreteraClass;
using VehiculoClass;

class Servidor
{
    static TcpListener ServidorTcp = new TcpListener(IPAddress.Parse("127.0.0.1"), 10001);
    static Carretera carretera = new Carretera(); // 📌 Simulación de la carretera
    static object lockObj = new object(); // 🔒 Protección de datos compartidos

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
        int idVehiculo;
        string direccion;
        Vehiculo vehiculo; // 📌 Definir aquí para que tenga alcance en todo el método

        lock (lockObj)
        {
            idVehiculo = carretera.NumVehiculosEnCarrera + 1;
            direccion = (new Random().Next(2) == 0) ? "Norte" : "Sur";

            // 📌 Crear y añadir vehículo a la carretera
            vehiculo = new Vehiculo() { Id = idVehiculo, Direccion = direccion };
            carretera.AñadirVehiculo(vehiculo);
        } // 🔄 Se cerró correctamente el bloque `lock`

        Console.WriteLine($"🚗 Vehículo {idVehiculo} asignado. Dirección: {direccion}");

        NetworkStream stream = cliente.GetStream();
        NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);

        while (!vehiculo.Acabado)
        {
            Vehiculo vehiculoActualizado = NetworkStreamClass.LeerDatosVehiculoNS(stream);
            if (vehiculoActualizado != null)
            {
                lock (lockObj)
                {
                    carretera.ActualizarVehiculo(vehiculoActualizado);
                } // 🔄 Se cerró correctamente el `lock`
                
                Console.WriteLine($"📡 Vehículo {idVehiculo} actualizado a posición {vehiculoActualizado.Pos}.");

                // 📤 Enviar estado de la carretera a los clientes
                NetworkStreamClass.EscribirDatosCarreteraNS(stream, carretera);
            }

            Thread.Sleep(500); // 🔄 Simular la espera antes de recibir nueva información
        }

        Console.WriteLine($"🏁 Vehículo {idVehiculo} completó su recorrido.");
        cliente.Close();
    }
}
