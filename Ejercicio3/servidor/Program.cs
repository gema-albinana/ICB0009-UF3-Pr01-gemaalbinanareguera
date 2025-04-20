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
    private static SemaphoreSlim semaforo = new SemaphoreSlim(1, 1); // Control del puente
    private static Vehiculo? vehiculoEnPuente = null; // Vehículo actual en el puente
    private static Carretera carretera = new Carretera(); // Registro de vehículos en carretera
    private static Queue<Vehiculo> colaNorte = new Queue<Vehiculo>(); // Cola de vehículos dirección Norte
    private static Queue<Vehiculo> colaSur = new Queue<Vehiculo>(); // Cola de vehículos dirección Sur
    private static int contadorVehiculos = 1; // Contador global de vehículos en orden de llegada

    static void Main()
    {
        TcpListener servidor = new TcpListener(IPAddress.Parse("127.0.0.1"), 12345);
        servidor.Start();
        Console.WriteLine("🚦 Servidor iniciado. Esperando conexiones...");

        while (true)
        {
            TcpClient cliente = servidor.AcceptTcpClient();
            Console.WriteLine("✅ Cliente conectado.");
            ThreadPool.QueueUserWorkItem(AtenderCliente, cliente);
        }
    }

    static void AtenderCliente(object? obj)
    {
        if (obj is not TcpClient cliente) return;
        NetworkStream stream = cliente.GetStream();

        try
        {
            // Leer los datos del vehículo desde el cliente
            Vehiculo? vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);
            if (vehiculo == null)
            {
                Console.WriteLine("❌ Error: Vehículo recibido es NULL.");
                return;
            }

            // Asignar un ID secuencial al vehículo
            vehiculo.Id = contadorVehiculos++;
            Console.WriteLine($"📥 Vehículo {vehiculo.Id} ({vehiculo.Direccion}) recibido.");

            carretera.AñadirVehiculo(vehiculo);
            semaforo.Wait();

            // Si el puente está libre, el vehículo puede cruzar
            if (vehiculoEnPuente == null)
            {
                vehiculoEnPuente = vehiculo;
                NetworkStreamClass.EscribirMensajeNetworkStream(stream, $"🚗 {vehiculo.Id} ({vehiculo.Direccion}) CRUZANDO puente.");
                Console.WriteLine($"🚗 Vehículo {vehiculo.Id} ({vehiculo.Direccion}) CRUZANDO puente...");
                Thread.Sleep(2000);
                vehiculoEnPuente = null;
                NetworkStreamClass.EscribirMensajeNetworkStream(stream, $"✅ {vehiculo.Id} ha cruzado el puente.");
                Console.WriteLine($"✅ Vehículo {vehiculo.Id} ha cruzado el puente.");

                // Determinar qué vehículo tiene prioridad para cruzar
                if (vehiculo.Direccion == "Norte" && colaSur.Count > 0)
                {
                    Vehiculo siguienteVehiculo = colaSur.Dequeue();
                    Console.WriteLine($"🚗 {siguienteVehiculo.Id} ahora puede cruzar el puente.");
                    NetworkStreamClass.EscribirMensajeNetworkStream(stream, $"📢 {siguienteVehiculo.Id} puede avanzar.");
                }
                else if (vehiculo.Direccion == "Sur" && colaNorte.Count > 0)
                {
                    Vehiculo siguienteVehiculo = colaNorte.Dequeue();
                    Console.WriteLine($"🚗 {siguienteVehiculo.Id} ahora puede cruzar el puente.");
                    NetworkStreamClass.EscribirMensajeNetworkStream(stream, $"📢 {siguienteVehiculo.Id} puede avanzar.");
                }
            }
            else
            {
                // Si el puente está ocupado, el vehículo entra en la cola correspondiente
                if (vehiculo.Direccion == "Norte")
                {
                    colaNorte.Enqueue(vehiculo);
                }
                else
                {
                    colaSur.Enqueue(vehiculo);
                }
                NetworkStreamClass.EscribirMensajeNetworkStream(stream, $"❌ {vehiculo.Id} ({vehiculo.Direccion}) esperando: Puente ocupado por {vehiculoEnPuente.Id}");
                Console.WriteLine($"🚗 Vehículo {vehiculo.Id} ({vehiculo.Direccion}) esperando: Puente ocupado por {vehiculoEnPuente.Id}");
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"❌ Error en la conexión: {ex.Message}");
        }
        finally
        {
            if (semaforo.CurrentCount == 0)
            {
                semaforo.Release();
            }
            cliente.Close();
        }
    }
}
