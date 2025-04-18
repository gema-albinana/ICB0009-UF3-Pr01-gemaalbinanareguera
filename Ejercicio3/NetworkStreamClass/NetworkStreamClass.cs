using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using CarreteraClass;
using VehiculoClass;

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

        // 📌 Método para escribir en NetworkStream los datos de tipo Carretera
        public static void EscribirDatosCarreteraNS(NetworkStream NS, Carretera C)
        {            
            try
            {
                byte[] datosCarretera = C.CarreteraABytes();
                NS.Write(datosCarretera, 0, datosCarretera.Length);
                Console.WriteLine($"📤 Datos de carretera enviados.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al enviar datos de carretera: {ex.Message}");
            }
        }

        // 📥 Método para leer desde NetworkStream los datos de un objeto Carretera
        public static Carretera LeerDatosCarreteraNS(NetworkStream NS)
        {
            try
            {
                byte[] buffer = new byte[8192]; // Aumentar tamaño de buffer
                MemoryStream ms = new MemoryStream();
                int bytesLeidos = NS.Read(buffer, 0, buffer.Length);
                ms.Write(buffer, 0, bytesLeidos);

                Console.WriteLine($"📥 Datos de carretera recibidos.");
                return Carretera.BytesACarretera(ms.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al recibir datos de carretera: {ex.Message}");
                return null;
            }
        }

        // 🚗 Método para enviar datos de Vehiculo a NetworkStream
        public static void EscribirDatosVehiculoNS(NetworkStream NS, Vehiculo V)
        {            
            try
            {
                byte[] datosVehiculo = V.VehiculoaBytes();
                NS.Write(datosVehiculo, 0, datosVehiculo.Length);
                Console.WriteLine($"📤 Datos del vehículo {V.Id} enviados.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al enviar datos del vehículo: {ex.Message}");
            }
        }

        // 📥 Método para leer desde NetworkStream los datos de un objeto Vehiculo
        public static Vehiculo LeerDatosVehiculoNS(NetworkStream NS)
        {
            try
            {
                byte[] buffer = new byte[4096];
                MemoryStream ms = new MemoryStream();
                int bytesLeidos = NS.Read(buffer, 0, buffer.Length);
                ms.Write(buffer, 0, bytesLeidos);

                Console.WriteLine($"📥 Datos del vehículo recibidos.");
                return Vehiculo.BytesAVehiculo(ms.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al recibir datos del vehículo: {ex.Message}");
                return null;
            }
        }
    }
}
