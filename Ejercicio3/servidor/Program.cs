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
    private static SemaphoreSlim semaforo = new SemaphoreSlim(1, 1); // Control del túnel
    private static Vehiculo? vehiculoEnTunel = null; // Vehículo actual en el túnel
    private static Carretera carretera = new Carretera(); // Registro de vehículos en la carretera
    private static Queue<Vehiculo> vehiculosEsperando = new Queue<Vehiculo>(); // Cola de espera
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

            if (vehiculoEnTunel == null || vehiculoEnTunel.Direccion == vehiculo.Direccion)
            {
                vehiculoEnTunel = vehiculo;
                NetworkStreamClass.EscribirMensajeNetworkStream(stream, $"🚗 {vehiculo.Id} ({vehiculo.Direccion}) CRUZANDO túnel.");
                Console.WriteLine($"🚗 Vehículo {vehiculo.Id} ({vehiculo.Direccion}) CRUZANDO túnel...");

                // Simula el cruce
                Thread.Sleep(2000);
                
                vehiculoEnTunel = null;
                NetworkStreamClass.EscribirMensajeNetworkStream(stream, $"✅ {vehiculo.Id} ha cruzado el túnel.");
                Console.WriteLine($"✅ Vehículo {vehiculo.Id} ha cruzado el túnel.");

                // Verificar si hay vehículos esperando
                if (vehiculosEsperando.Count > 0)
                {
                    Vehiculo siguienteVehiculo = vehiculosEsperando.Dequeue();
                    Console.WriteLine($"🚗 {siguienteVehiculo.Id} ahora puede cruzar el túnel.");
                }
            }
            else
            {
                vehiculosEsperando.Enqueue(vehiculo);
                NetworkStreamClass.EscribirMensajeNetworkStream(stream, $"❌ {vehiculo.Id} ({vehiculo.Direccion}) esperando. Túnel ocupado.");
                Console.WriteLine($"🚗 Vehículo {vehiculo.Id} ({vehiculo.Direccion}) esperando. Túnel ocupado.");
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
