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
    static TcpListener servidorTcp = new TcpListener(IPAddress.Parse("127.0.0.1"), 10001);
    static Carretera carretera = new Carretera();
    static List<TcpClient> listaClientes = new List<TcpClient>();
    static object lockObj = new object();
    static Vehiculo? VehiculoEnPuente = null;
    static Queue<Vehiculo> ColaNorte = new Queue<Vehiculo>();
    static Queue<Vehiculo> ColaSur = new Queue<Vehiculo>();
    static string DireccionActual = "Norte";

    static void Main()
    {
        servidorTcp.Start();
        Console.WriteLine("🚦 Servidor iniciado. Esperando conexiones...");

        while (true)
        {
            TcpClient cliente = servidorTcp.AcceptTcpClient();
            lock (lockObj) listaClientes.Add(cliente);
            Console.WriteLine("✅ Cliente conectado.");
            new Thread(() => GestionarCliente(cliente)).Start();
        }
    }

    static void GestionarCliente(TcpClient cliente)
    {
        try
        {
            NetworkStream stream = cliente.GetStream();
            Vehiculo vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);

            lock (lockObj)
            {
                vehiculo.Id = carretera.NumVehiculosEnCarrera + 1;
                carretera.AñadirVehiculo(vehiculo);

                if (vehiculo.Direccion == "Norte")
                    ColaNorte.Enqueue(vehiculo);
                else
                    ColaSur.Enqueue(vehiculo);
            }

            NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);
            Console.WriteLine($"🚗 Vehículo {vehiculo.Id} asignado. Dirección: {vehiculo.Direccion}");

            while (!vehiculo.Acabado)
            {
                vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(stream);

                lock (lockObj)
                {
                    Console.WriteLine($"🔍 Estado del puente: Ocupado por #{VehiculoEnPuente?.Id ?? 0}");

                    if (EstaCercaDelPuente(vehiculo))
                    {
                        while (VehiculoEnPuente != null || vehiculo.Direccion != DireccionActual)
                        {
                            Console.WriteLine($"⛔ Vehículo #{vehiculo.Id} esperando turno. Puente ocupado por #{VehiculoEnPuente?.Id}.");
                            vehiculo.Parado = true;
                            NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);
                            Monitor.Wait(lockObj);
                        }

                        // 📌 Ahora el puente está realmente ocupado
                        VehiculoEnPuente = vehiculo;  
                        Console.WriteLine($"🚗 Vehículo #{vehiculo.Id} entra al puente → VehiculoEnPuente = #{vehiculo.Id}");
                        vehiculo.Parado = false;
                        NetworkStreamClass.EscribirDatosVehiculoNS(stream, vehiculo);
                    }

                    if (HaSalidoDelPuente(vehiculo))
                    {
                        VehiculoEnPuente = null;
                        Console.WriteLine($"✅ Vehículo #{vehiculo.Id} sale del puente");

                        CambiarDireccionSiEsNecesario();
                        Monitor.PulseAll(lockObj);
                    }

                    carretera.ActualizarVehiculo(vehiculo);
                }

                EnviarDatosACtodosLosClientes();
            }

            Console.WriteLine($"🏁 Vehículo #{vehiculo.Id} terminó su recorrido.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error con cliente: {ex.Message}");
        }
    }

    static bool EstaCercaDelPuente(Vehiculo vehiculo)
    {
        return vehiculo.Pos >= 25 && vehiculo.Pos < 30;
    }

    static bool HaSalidoDelPuente(Vehiculo vehiculo)
    {
        return vehiculo.Pos >= 50;
    }

    static void CambiarDireccionSiEsNecesario()
    {
        lock (lockObj)
        {
            if (VehiculoEnPuente == null) 
            {
                DireccionActual = DireccionActual == "Norte" ? "Sur" : "Norte";
                Console.WriteLine($"🔄 Cambio de dirección: Ahora los vehículos de {DireccionActual} pueden pasar");
            }
        }
    }

    static void EnviarDatosACtodosLosClientes()
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
                catch { }
            }
        }
    }
}
