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

        // ENTRADA AL TÚNEL CONTROLADA
        semaforo.Wait();

        lock (lockObj)
        {
            // Esperar si hay un vehículo en túnel y es de dirección opuesta
            while (vehiculoEnTunel != null && vehiculoEnTunel.Direccion != vehiculo.Direccion)
            {
                Monitor.Wait(lockObj); // Espera hasta que pueda entrar
            }

            // Ya puede entrar al túnel
            vehiculoEnTunel = vehiculo;
        }

        Console.WriteLine($"🚗 Vehículo {vehiculo.Id} ({vehiculo.Direccion}) CRUZANDO túnel...");
        Thread.Sleep(10000); // Simula el cruce
        Console.WriteLine($"✅ Vehículo {vehiculo.Id} ha cruzado el túnel.");

        lock (lockObj)
        {
            vehiculoEnTunel = null;
            Monitor.PulseAll(lockObj); // Avisar a los que estaban esperando
        }

        semaforo.Release();

        // Bucle para manejar actualizaciones
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
