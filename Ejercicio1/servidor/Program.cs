using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetworkStreamNS;

class Servidor
{
    static TcpListener ServidorTcp = new TcpListener(IPAddress.Parse("127.0.0.1"), 10001);
    static int contadorID = 0; // ID único para cada bicicleta
    static object lockObj = new object(); // Proteger el contador en hilos

    static void Main(string[] args)
    {
        ServidorTcp.Start();
        Console.WriteLine("🚦 Servidor iniciado. Esperando conexiones...");

        while (true) // Permitir múltiples conexiones
        {
            TcpClient cliente = ServidorTcp.AcceptTcpClient();
            Console.WriteLine("✅ Cliente conectado.");

            // Crear un nuevo hilo para gestionar al cliente
            Thread clienteThread = new Thread(() => GestionarCliente(cliente));
            clienteThread.Start();
        }
    }

    static void GestionarCliente(TcpClient cliente)
    {
        int idVehiculo;
        string direccionAleatoria;

        // Proteger la asignación de ID con lock
        lock (lockObj)
        {
            idVehiculo = ++contadorID;
        }

        // Generar dirección aleatoria (Norte o Sur)
        direccionAleatoria = (new Random().Next(2) == 0) ? "Norte" : "Sur";

        Console.WriteLine($"🚲 Bicicleta {idVehiculo} asignada. Dirección: {direccionAleatoria}");

        // 📡 Obtener el flujo de comunicación con el cliente
        NetworkStream stream = cliente.GetStream();

        // 📥 Esperar mensaje de inicio del cliente
        string mensajeInicio = NetworkStreamClass.LeerMensajeNetworkStream(stream);
        if (mensajeInicio == "INICIO")
        {
            Console.WriteLine("🔄 Handshake iniciado por el cliente.");

            // 📤 Enviar ID del vehículo al cliente
            NetworkStreamClass.EscribirMensajeNetworkStream(stream, idVehiculo.ToString());

            // 📥 Esperar confirmación del cliente con el mismo ID
            string confirmacionCliente = NetworkStreamClass.LeerMensajeNetworkStream(stream);
            if (confirmacionCliente == idVehiculo.ToString())
            {
                Console.WriteLine($"✅ Cliente confirmó recepción del ID {idVehiculo}. Handshake completado.");
            }
            else
            {
                Console.WriteLine($"❌ Cliente envió una confirmación incorrecta: {confirmacionCliente}. Se cerrará la conexión.");
                cliente.Close();
            }
        }
        else
        {
            Console.WriteLine("❌ Cliente no envió 'INICIO'. Se cerrará la conexión.");
            cliente.Close();
        }
    }
}
