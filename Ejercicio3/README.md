## 🚦 Simulación de Tráfico en el Puente 🚗**  
Pregunta teórica 1:    
[Explica las ventajas e inconvenientes de programar el control de paso por el túnel, en el cliente o en el servidor]    
📌 Control en el servidor  
✅ Ventajas:  
-Centralización de la lógica: El servidor gestiona el tráfico de forma global, evitando inconsistencias entre clientes.  
-Sincronización garantizada: Como todos los clientes dependen del mismo servidor, se evita que múltiples vehículos intenten cruzar simultáneamente sin control.  
-Mayor seguridad: El servidor puede implementar reglas estrictas sin depender de la confiabilidad del código del cliente.  
❌ Inconvenientes:  
-Mayor carga para el servidor: Con muchos clientes conectados, el servidor puede volverse un cuello de botella.  
-Retraso en la comunicación: La latencia en la conexión puede hacer que los clientes tengan una pequeña demora antes de recibir autorización para moverse.  

📌 Control en el cliente  
✅ Ventajas:  
-Menos carga para el servidor: Cada cliente decide cuándo moverse, reduciendo la cantidad de mensajes que deben enviarse.  
-Mayor independencia: Si el servidor tiene problemas, los clientes pueden seguir funcionando localmente sin colapsar el sistema.  
-Menos retraso: La decisión se toma instantáneamente sin depender de mensajes del servidor.   
❌ Inconvenientes:  
-Posibles inconsistencias: Si los clientes no están bien sincronizados, podrían intentar cruzar el túnel al mismo tiempo, causando errores.  
-Menor seguridad: Un cliente mal programado o con errores podría ignorar las reglas y causar problemas en la simulación.    
📌 ¿Cuál es la mejor opción?  
💡 Yo creo que la mejor opcion es controlar el tráfico en el servidor para evitar conflictos y desincronización, asegurando que solo un vehículo cruce el túnel.  

Pregunta teórica 2:  
Yo he hecho  dos colas separadas (Queue<Vehiculo>), una para cada dirección (Norte y Sur).  
Cuando un vehículo llega al puente, si esta libre, puede cruzarlo de inmediato. Si está ocupado, se coloca en la cola correspondiente según su dirección.  
Cuando el puente se desocupa y había vehículos esperando, se otorga prioridad a los vehículos en dirección opuesta al que acaba de cruzar.


