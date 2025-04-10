using System;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace NetworkStreamNS
{
    public class NetworkStreamClass
    {
        // Método para escribir un mensaje en el NetworkStream
        public static void EscribirMensajeNetworkStream(NetworkStream NS, string mensaje)
        {
            try
            {
                byte[] mensajeBytes = Encoding.UTF8.GetBytes(mensaje);
                NS.Write(mensajeBytes, 0, mensajeBytes.Length);
                Console.WriteLine($"📤 Mensaje enviado: {mensaje}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al escribir en NetworkStream: {ex.Message}");
            }
        }

        // Método para leer un mensaje desde el NetworkStream
        public static string LeerMensajeNetworkStream(NetworkStream NS)
        {
            try
            {
                byte[] bufferLectura = new byte[1024];
                int bytesLeidos = 0;
                var tmpStream = new MemoryStream();

                do
                {
                    int bytesLectura = NS.Read(bufferLectura, 0, bufferLectura.Length);
                    tmpStream.Write(bufferLectura, 0, bytesLectura);
                    bytesLeidos += bytesLectura;
                } while (NS.DataAvailable);

                byte[] bytesTotales = tmpStream.ToArray();
                string mensajeRecibido = Encoding.UTF8.GetString(bytesTotales, 0, bytesLeidos);

                Console.WriteLine($"📥 Mensaje recibido: {mensajeRecibido}");
                return mensajeRecibido;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al leer desde NetworkStream: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
