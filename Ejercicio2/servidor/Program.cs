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
    static List<TcpClient> listaClientes = new List<TcpClient>(); // 📌 Lista de clientes conectados
    static object lockObj = new object(); // 🔒 Protección de datos compartidos

    static void Main(string[] args)
    {
        ServidorTcp.Start();
        Console.WriteLine("🚦 Servidor iniciado. Esperando conexiones...");

        while (true)
        {
            TcpClient cliente = ServidorTcp.AcceptTcpClient();
            
            lock (lockObj)
            {
                listaClientes.Add(cliente); // 📌 Guardar el cliente en la lista
            }

            Console.WriteLine("✅ Cliente conectado.");

            Thread clienteThread = new Thread(() => GestionarCliente(cliente));
            clienteThread.Start();
        }
    }

    static void GestionarCliente(TcpClient cliente)
    {
        try
        {
            NetworkStream stream = cliente.GetStream();

            // 📥 Recibir vehículo sin ID del cliente
            Vehiculo vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);

            lock (lockObj)
            {
                vehiculo.Id = carretera.NumVehiculosEnCarrera + 1; // 📌 Asignar ID secuencial
                carretera.AñadirVehiculo(vehiculo);
            }

            NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo); // 📤 Enviar vehículo con ID al cliente
            Console.WriteLine($"🚗 Vehículo {vehiculo.Id} asignado. Dirección: {vehiculo.Direccion}");

            // 🚗 **Bucle para actualizar la carretera con el avance del vehículo**
            while (!vehiculo.Acabado)
            {
                vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream); // 📥 Recibir actualización del cliente
                carretera.ActualizarVehiculo(vehiculo); // 🚗 Actualizar posición en la carretera

                Console.WriteLine($"🚦 Vehículo {vehiculo.Id} en carretera. Posición: {vehiculo.Pos}");

                EnviarDatosACtodosLosClientes(); // 📤 Notificar a todos los clientes
            }

            Console.WriteLine($"🏁 Vehículo {vehiculo.Id} completó su recorrido.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error con cliente: {ex.Message}");
        }
    }

    // 📤 **Método para enviar datos de la carretera a todos los clientes conectados**
    static void EnviarDatosACtodosLosClientes()
    {
        lock (lockObj)
        {
            foreach (TcpClient cliente in listaClientes)
            {
                try
                {
                    NetworkStream stream = cliente.GetStream();
                    NetworkStreamClass.EscribirDatosCarreteraNS(stream, carretera); // 📤 Enviar datos
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error al enviar datos a un cliente: {ex.Message}");
                }
            }
        }
    }
}
