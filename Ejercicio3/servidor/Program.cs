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
    private static TcpListener ServidorTcp = new TcpListener(IPAddress.Parse("127.0.0.1"), 10001);
    private static Carretera carretera = new Carretera();
    private static List<TcpClient> listaClientes = new List<TcpClient>();
    private static object lockObj = new object();

    private static SemaphoreSlim semaforo = new SemaphoreSlim(1, 1);
    private static Vehiculo? vehiculoEnTunel = null;
    private static Queue<Vehiculo> vehiculosNorte = new Queue<Vehiculo>();
    private static Queue<Vehiculo> vehiculosSur = new Queue<Vehiculo>();
    private static int contadorVehiculos = 1;

    static void Main()
    {
        ServidorTcp.Start();
        Console.WriteLine("🚦 Servidor iniciado. Esperando conexiones...");

        while (true)
        {
            TcpClient cliente = ServidorTcp.AcceptTcpClient();
            lock (lockObj)
            {
                listaClientes.Add(cliente);
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

            Console.WriteLine("📥 Esperando datos del vehículo...");
            Vehiculo vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);

            if (vehiculo == null)
            {
                Console.WriteLine("❌ Error: Vehículo recibido es NULL.");
                return;
            }

            lock (lockObj)
            {
                vehiculo.Id = contadorVehiculos++;
                carretera.AñadirVehiculo(vehiculo);
            }

            NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);
            Console.WriteLine($"🚗 Vehículo {vehiculo.Id} asignado. Dirección: {vehiculo.Direccion}");

            lock (lockObj)
            {
                if (vehiculo.Direccion == "Norte")
                {
                    vehiculosNorte.Enqueue(vehiculo);
                }
                else
                {
                    vehiculosSur.Enqueue(vehiculo);
                }
            }

            semaforo.Wait();

            lock (lockObj)
            {
                if (vehiculoEnTunel == null || vehiculoEnTunel.Direccion == vehiculo.Direccion)
                {
                    vehiculoEnTunel = vehiculo;
                    Console.WriteLine($"🚗 Vehículo {vehiculo.Id} ({vehiculo.Direccion}) CRUZANDO túnel...");
                    Thread.Sleep(10000);
                    vehiculoEnTunel = null;

                    Console.WriteLine($"✅ Vehículo {vehiculo.Id} ha cruzado el túnel.");

                    if (vehiculo.Direccion == "Norte" && vehiculosNorte.Count > 0)
                    {
                        Vehiculo siguienteVehiculo = vehiculosNorte.Dequeue();
                        Console.WriteLine($"🚗 {siguienteVehiculo.Id} ahora puede cruzar el túnel.");
                    }
                    else if (vehiculo.Direccion == "Sur" && vehiculosSur.Count > 0)
                    {
                        Vehiculo siguienteVehiculo = vehiculosSur.Dequeue();
                        Console.WriteLine($"🚗 {siguienteVehiculo.Id} ahora puede cruzar el túnel.");
                    }
                }
            }
            semaforo.Release();

            while (!vehiculo.Acabado)
            {
                Vehiculo vehiculoActualizado = NetworkStreamClass.LeerDatosVehiculoNS(stream);

                if (vehiculoActualizado == null)
                {
                    Console.WriteLine("❌ Error: Datos de vehículo recibidos son NULL.");
                    return;
                }

                carretera.ActualizarVehiculo(vehiculoActualizado);
                Console.WriteLine($"🚦 Vehículo {vehiculoActualizado.Id} en carretera. Posición: {vehiculoActualizado.Pos}");

                EnviarDatosATodosLosClientes();
            }

            Console.WriteLine($"🏁 Vehículo {vehiculo.Id} completó su recorrido.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error con cliente: {ex.Message}");
        }
    }

    static void EnviarDatosATodosLosClientes()
    {
        lock (lockObj)
        {
            foreach (TcpClient cliente in listaClientes)
            {
                try
                {
                    NetworkStream stream = cliente.GetStream();
                    NetworkStreamClass.EscribirDatosCarreteraNS(stream, carretera);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error al enviar datos a un cliente: {ex.Message}");
                }
            }
        }
    }
}
