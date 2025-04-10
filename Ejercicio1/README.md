## 📌 **Etapa 1: Conexión Servidor - Cliente**    
En esta etapa hemos implementado la conexión entre un **servidor TCP** y un **cliente TCP** en C#.  
El servidor escucha en `127.0.0.1:10001`, mientras que el cliente se conecta, verificando que la comunicación es exitosa.    
Para probar la conexión, primero ejecutamos el servidor y luego el cliente. Ambos mostrarán mensajes en la consola confirmando que la conexión se ha establecido correctamente.   
🎯 Resultado esperado en el servidor  
  ![alt text](image.png)    
🎯 Resultado esperado en el cliente   
  ![alt text](image-1.png)  

## 📌 **Etapa 2: Aceptación de clientes**  
En esta etapa, el servidor ha sido mejorado para aceptar varios clientes de forma concurrente.  
Cada nuevo cliente que se conecta es gestionado por un **hilo independiente**, permitiendo que el servidor siga aceptando nuevas conexiones mientras atiende clientes existentes.  
🎯 Resultado esperado  
  ![alt text](image-2.png)

## 📌 **Etapa3: Asignar un ID único a cada bicicleta**   
En esta etapa, el servidor identifica cada bicicleta conectada asignándole un **ID único** y una **dirección aleatoria**.  
Para evitar problemas de concurrencia, se protege la asignación de IDs mediante **bloqueo (`lock`)**.    
🎯 Resultado esperado   
 ![alt text](image-3.png) 