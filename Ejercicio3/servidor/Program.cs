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

    private static Vehiculo? vehiculoEnTunel = null;
    private static int contadorVehiculos = 1;
    private static int contadorActualizaciones = 0;

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

            bool dentroDelTunel = false;

            while (!vehiculo.Acabado)
            {
                Vehiculo vehiculoActualizado = NetworkStreamClass.LeerDatosVehiculoNS(stream);

                if (vehiculoActualizado == null)
                {
                    Console.WriteLine("❌ Error: Datos de vehículo recibidos son NULL.");
                    return;
                }

                lock (lockObj)
                {
                    carretera.ActualizarVehiculo(vehiculoActualizado);
                }

                // **Mostrar posición y velocidad en cada iteración**
                Console.WriteLine($"🚗 Vehículo {vehiculoActualizado.Id} - Posición: {vehiculoActualizado.Pos} km, Velocidad: {vehiculoActualizado.Velocidad} km/h.");

                // Verificar si el vehículo entra al túnel en km 30 (Norte) o km 50 (Sur)
                if (!dentroDelTunel &&
                    ((vehiculoActualizado.Direccion == "Norte" && vehiculoActualizado.Pos == 30) ||
                     (vehiculoActualizado.Direccion == "Sur" && vehiculoActualizado.Pos == 50)))
                {
                    lock (lockObj)
                    {
                        while (vehiculoEnTunel != null && vehiculoEnTunel.Direccion != vehiculoActualizado.Direccion)
                        {
                            Console.WriteLine($"⛔ Vehículo {vehiculoActualizado.Id} esperando túnel ocupado...");
                            Monitor.Wait(lockObj);
                        }

                        vehiculoEnTunel = vehiculoActualizado;
                        dentroDelTunel = true;

                        Console.WriteLine($"🚦 Vehículo {vehiculoActualizado.Id} ENTRA al túnel en km {vehiculoActualizado.Pos}.");
                    }
                }

                // Verificar si ha salido del túnel en km 50 (Norte) o km 30 (Sur)
                if (dentroDelTunel &&
                    ((vehiculoActualizado.Direccion == "Norte" && vehiculoActualizado.Pos >= 50) ||
                     (vehiculoActualizado.Direccion == "Sur" && vehiculoActualizado.Pos <= 30)))
                {
                    lock (lockObj)
                    {
                        Console.WriteLine($"✅ Vehículo {vehiculoActualizado.Id} SALE del túnel.");
                        vehiculoEnTunel = null;
                        dentroDelTunel = false;
                        Monitor.PulseAll(lockObj);
                    }
                }

                contadorActualizaciones++;

                if (vehiculoActualizado.Acabado)
                {
                    Console.WriteLine($"🏁 Vehículo {vehiculoActualizado.Id} completó su recorrido.");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error con cliente: {ex.Message}");
        }
    }
}
