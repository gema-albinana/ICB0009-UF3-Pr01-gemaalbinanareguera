## 📌 **Etapa1:Programación de los métodos de la clase NetworkStreamClass**  
En esta etapa, hemos programado los métodos necesarios para **intercambiar datos de tipo "Vehiculo" y "Carretera"** entre el servidor y los clientes mediante "NetworkStream".  
Estos métodos serán fundamentales para la comunicación en las siguientes etapas del ejercicio.  

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
