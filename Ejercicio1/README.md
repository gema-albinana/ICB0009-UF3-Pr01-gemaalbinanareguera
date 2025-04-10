## 📌 **Etapa 1: Conexión Servidor - Cliente**    
En esta etapa hemos implementado la conexión entre un **servidor TCP** y un **cliente TCP** en C#.  
El servidor escucha en `127.0.0.1:10001`, mientras que el cliente se conecta, verificando que la comunicación es exitosa.    
Para probar la conexión, primero ejecutamos el servidor y luego el cliente. Ambos mostrarán mensajes en la consola confirmando que la conexión se ha establecido correctamente.   
Esta etapa demuestra cómo un servidor TCP y un cliente TCP pueden establecer comunicación en C#. El servidor permanece activo esperando conexiones, mientras que el cliente se conecta para verificar la comunicación. El propósito de esta etapa es comprobar que el cliente se conecta al servidor sin errores. No se envían mensajes entre ellos, solo se confirma que la conexión funciona correctamente.  
🎯 Resultado esperado en el servidor  
  ![alt text](image.png)    
🎯 Resultado esperado en el cliente   
  ![alt text](image-1.png)  

## 📌 **Etapa 2: Aceptación de clientes**  
En esta etapa, el servidor ha sido mejorado para aceptar varios clientes de forma concurrente.  
Cada nuevo cliente que se conecta es gestionado por un **hilo independiente**, permitiendo que el servidor siga aceptando nuevas conexiones mientras atiende clientes existentes.    
En esta etapa se permite que el servidor acepte y gestiones múltiples clientes al mismo tiempo. Para ello se le asigna un hilo a cada cliente para evitar bloqueos en la conexión.  
Se muestra “Gestionando nuevo vehículo…” cuando un cliente se conecta.  
🎯 Resultado esperado  
  ![alt text](image-2.png)

## 📌 **Etapa3: Asignar un ID único a cada bicicleta**   
En esta etapa, el servidor identifica cada bicicleta conectada asignándole un **ID único** y una **dirección aleatoria**.  
Para evitar problemas de concurrencia, se protege la asignación de IDs mediante **bloqueo (`lock`)**.   
Se genera un ID único para cada bicicleta que se conecta. Se le asigna una dirección aleatoria (Norte/Sur) a cada bicicleta. Se usa programación concurrente para garantizar que varios clientes se manejen sin errores.   
🎯 Resultado esperado   
 ![alt text](image-3.png)   

## 📌 **Etapa4: Obtener el NetworkStream**  
En esta etapa, cada vez que un **cliente** se conecta al   **servidor**, ambos obtienen el NetworkStream.    
Esto permitirá **enviar y recibir datos** en futuras etapas.   
Obtenemos el NetworkStream en cliente y servidor tras la conexión. Preparando asi la comunicación entre ambas partes para el intercambio de datos.      
🎯 Resultado esperado en el servidor   
 ![alt text](image-4.png)   
🎯 Resultado esperado en el cliente  
 ![alt text](image-5.png)  

## 📌 **Etapa5: Programar métodos EscribirMensajeNetworkStream y LeerMensajeNetworkStream**    
En esta etapa, creamos métodos para **enviar y recibir datos** entre **cliente y servidor** a través del NetworkStream, lo que prepara la aplicación para el **handshake** en la siguiente fase.  
Para mantener el código organizado y reutilizable, se utiliza la clase NetworkStreamClass proporcionada por el profesor, la cual centraliza los métodos de comunicación entre servidor y cliente."   
Se implementa un método para enviar datos “EscribirMensajeNetworStream”.  
Se implementa un método para recibir datos “LeerMensajeNetworkStream”.   
Uso una clase independiente “NetworkStreamClass” para compartir los métodos entre cliente y servidor.  
🎯 Resultado esperado en el servidor   
 ![alt text](image-6.png)    
🎯 Resultado esperado en el cliente  
 ![alt text](image-7.png)  
 ![alt text](image-8.png)  

## 📌 **Etapa6: Handshake**    
Antes de que el cliente pueda comenzar a operar, debe establecerse un **handshake** con el servidor. Esto permite que **ambas partes intercambien información esencial** para la ejecución correcta.  
En nuestro programa, el único dato necesario para el cliente es el “ID” que le asigna el servidor, por lo tanto, el handshake sigue este proceso:  
-Cliente envía un mensaje al servidor para iniciar el handshake (“INICIO”).  
-Servidor responde con el ID asignado al vehículo.  
-Cliente confirma la recepción enviando el mismo ID de vuelta.  
-Si el ID recibido y enviado coinciden, el handshake se completa y el cliente puede continuar.  
🎯 Resultado esperado en el servidor.     
 ![alt text](image-9.png)   
🎯 Resultado esperado en el cliente     
![alt text](image-10.png)   
![alt text](image-11.png)  

## 📌 **Etapa7: Almacenar información de clientes conectados**  
En esta etapa, el servidor debe **mantener un registro de los clientes conectados**, lo que le permitirá **gestionar múltiples conexiones y enviar mensajes a cada cliente**.    
Para conseguirlo:    
-Creamos una clase "cliente" que almacena el ID del cliente y su NetworStream para la comunicación.   
-Usamos una lista "List<Cliente>" ene l servidor para guardar la información de cada cliente conectado.    
-Cada vez que un cliente se conecta se añade a la lista y se muestra el número total de clientes conectados.  
🎯 Resultado esperado en el servidor   
![alt text](image-12.png)      
🎯 Resultado esperado en el cliente  
![alt text](image-13.png)  
![alt text](image-14.png)