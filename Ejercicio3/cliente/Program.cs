using System;
using System.Net.Sockets;
using System.Threading;
using NetworkStreamNS;
using VehiculoClass;
using CarreteraClass;
using System.Text;
using System.IO;
using System;
using System.IO;
using System.Net.Sockets;

class Cliente
{
    static void Main()
    {
        try
        {
            var cliente = new TcpClient("127.0.0.1", 12345);
            var stream = cliente.GetStream();
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream) { AutoFlush = true };

            // Solicitar información del vehículo
            string vehiculo = "Vehículo " + new Random().Next(1, 100); // Genera un nombre aleatorio para el vehículo
            string direccion = new Random().Next(0, 2) == 0 ? "Norte" : "Sur"; // Dirección aleatoria

            writer.WriteLine($"{vehiculo} - {direccion}");

            // Leer respuesta del servidor
            string respuesta = reader.ReadLine();
            Console.WriteLine($"📥 {respuesta}");

            // Aquí comprobamos si la respuesta contiene 'esperando'
            if (respuesta.Contains("esperando"))
            {
                Console.WriteLine("🚗 El vehículo está esperando para entrar al túnel...");
            }
            else if (respuesta.Contains("CRUZANDO"))
            {
                Console.WriteLine("🚗 El vehículo está cruzando el túnel...");
            }

            // Mantén la conexión abierta hasta que el servidor termine su tarea
            Console.WriteLine("Esperando que el servidor termine de procesar...");
            string finalResponse = reader.ReadLine(); // Asegura que el servidor complete la transacción
            Console.WriteLine($"📥 {finalResponse}");

            // Solo cerramos la conexión después de que se haya procesado todo
            cliente.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Error en la conexión: " + ex.Message);
        }
    }
}
