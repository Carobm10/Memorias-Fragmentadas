# Memorias Fragmentadas — Manual de Usuario

## 1. Descripción del Juego

**Memorias Fragmentadas** es un videojuego narrativo en primera persona con soporte para Realidad Virtual (Google Cardboard). El jugador asume el rol de un niño llamado Joselito que despierta en su casa familiar en Colombia durante los años 50-60. A través de misiones cotidianas y la interacción con su familia, el jugador descubre fragmentos de la memoria y la vida doméstica de la época.

El juego combina exploración, interacción con objetos, diálogos con personajes (NPCs) y pequeños rompecabezas.

---

## 2. Requisitos del Sistema

- Teléfono Android (versión 7.0 o superior recomendada)
- Visor Google Cardboard (opcional pero recomendado)
- Controlador/Joystick Bluetooth compatible con Android
- Espacio de almacenamiento: aproximadamente 500 MB

---

## 3. Instalación

### Paso 1: Obtener el APK
El archivo APK se distribuye a través de Google Drive u otro servicio de almacenamiento en la nube. Descargue el archivo `.apk` en su teléfono Android.

### Paso 2: Permitir instalación de fuentes desconocidas
1. Vaya a **Configuración** del teléfono.
2. Busque **Seguridad** o **Privacidad**.
3. Active la opción **Instalar aplicaciones de fuentes desconocidas** (o permita al navegador/administrador de archivos instalar apps).

### Paso 3: Instalar el APK
1. Abra el archivo `.apk` descargado desde su administrador de archivos o desde la notificación de descarga.
2. Pulse **Instalar**.
3. Espere a que se complete la instalación.

### Paso 4: Configurar el visor (opcional)
1. Inserte el teléfono en el visor Google Cardboard.
2. Conecte el joystick Bluetooth al teléfono (Configuración → Bluetooth → Emparejar dispositivo nuevo).
3. Abra la aplicación **Memorias Fragmentadas**.

---

## 4. Controles

### Joystick Bluetooth (Control principal)

| Botón | Función |
|-------|---------|
| **Joystick analógico** | Moverse por el escenario |
| **B** | Interactuar: abrir/cerrar puertas y cajones, presionar teclas, seleccionar objetos, inspeccionar |
| **A** | Hablar con personajes, iniciar misiones |
| **X** | Salir, cerrar diálogos, cancelar, devolver objetos |
| **Y** | Opciones adicionales en diálogos, avanzar texto |
| **Gatillo** | Acción alternativa (equivalente al tap en Cardboard) |

### Teclado (Solo en modo desarrollo/Editor)

| Tecla | Función |
|-------|---------|
| **WASD** | Moverse |
| **B** | Interactuar |
| **A** | Hablar / iniciar misión |
| **X** | Salir / cancelar |
| **Y** | Opciones adicionales |
| **Click izquierdo** | Seleccionar (en menú) |

### Google Cardboard (Sin joystick)

- **Mover la cabeza**: Mirar alrededor
- **Tap magnético / botón del visor**: Equivale al botón B (interactuar)

---

## 5. Flujo del Juego

### 5.1 Menú Principal (Máquina de Escribir)

Al iniciar el juego se presenta una habitación con una **máquina de escribir antigua**.

**Para comenzar a jugar:**
1. Mire el botón verde **"OP"** (Opciones) frente a la máquina de escribir.
2. Aparecerá un mensaje: **"Presiona B"**.
3. Presione **B** → Aparecen dos globos: **JUGAR** y **AJUSTES**.
4. Mire el globo **JUGAR** y presione **B** → Inicia el juego.

**Método alternativo (escribiendo):**
1. Mire las teclas de la máquina y presione **B** en cada letra para escribir "JUGAR".
2. Presione la tecla **ENVIAR** con **B**.
3. El juego cargará la siguiente escena.

### 5.2 Tutorial del Joystick

Antes de entrar al juego principal, hay un tutorial interactivo:
1. Se muestran indicadores para cada botón del mando.
2. Presione cada botón (A, B, X, Y, joystick) para confirmar que funcionan.
3. Presione **Y** para continuar al video introductorio.

### 5.3 Video Introductorio

Se reproduce un video contextual sobre la historia.
- **Y** → Continuar / Saltar al juego
- **X** → Volver al menú

### 5.4 Escena Principal (La Casa)

Al cargar la escena principal se reproduce una animación de **despertar** (efecto de abrir los ojos). Después de esto, el jugador tiene control total.

---

## 6. Jugabilidad

### 6.1 Exploración

- Use el **joystick analógico** para caminar por la casa.
- Mire los objetos. Cuando el puntero central detecta algo interactuable, se muestra un texto indicando la acción disponible.
- Los pasos del jugador suenan diferente según la superficie (madera o piso).

### 6.2 Interacción con Objetos

| Acción | Cómo hacerlo |
|--------|--------------|
| Abrir/cerrar puerta | Mire la puerta → Presione **B** |
| Abrir/cerrar cajón | Mire el cajón → Presione **B** |
| Sacar objeto del cajón | Abra el cajón → Mire el objeto dentro → Presione **B** |
| Rotar objeto (inspección 360°) | Con el objeto sacado, use el **joystick** para rotarlo |
| Devolver objeto al cajón | Presione **X** mientras inspecciona |
| Inspeccionar objeto fijo | Mire el objeto → Presione **B** → Rote con joystick → **X** para soltar |

### 6.3 Diálogos con NPCs

1. Acérquese a un personaje y mírelo.
2. Aparecerá: **"Presiona A para hablar"**.
3. Presione **A** para iniciar el diálogo.
4. El texto se escribe letra por letra.
5. Opciones de respuesta: **A**, **B**, **Y** según se indique.
6. Presione **X** para cerrar el diálogo.

---

## 7. Misiones

### 7.1 Misión de la Radio (Rosa, la empleada)

1. Hable con Rosa en la cocina presionando **A**.
2. Rosa pide poner música en la radio.
3. Busque la radio y presione **B** para revisar.
4. La radio necesita pilas. Busque las pilas en los cajones cercanos.
5. Presione **B** sobre las pilas para recogerlas.
6. Vuelva a la radio e inserte las pilas presionando **B**.
7. Gire la perilla para sintonizar la emisora.

### 7.2 Misión del Periódico (Mamá)

1. Hable con Mamá presionando **A**.
2. Mamá pide que traiga el monedero para pagar al del periódico.
3. Busque el monedero (cerca de la mesita del televisor).
4. Presione **B** para recogerlo.
5. Lleve el monedero a la puerta de entrada y presione **B** para pagar.
6. Tome el periódico y lléveselo a Mamá.

### 7.3 Misión del Uniforme (Clóset)

1. Acérquese al clóset e inicie la misión presionando **A**.
2. El jugador es movido frente al clóset.
3. Mire las prendas y presione **B** para seleccionar una.
4. Si es incorrecta: se muestra un mensaje. Puede probar otra.
5. Si es correcta: misión completada, vuelve a su posición.
6. Presione **X** para cerrar los mensajes.

### 7.4 Misión de la Carta (Papá, máquina de escribir)

1. Busque la máquina de escribir en la casa.
2. Mírela y presione **B** para sentarse a escribir.
3. Siga las instrucciones en pantalla para escribir la carta letra por letra.
4. Mire cada tecla y presione **B** para escribir la letra correspondiente.
5. Complete el texto de la carta.
6. Presione **X** para levantarse.

### 7.5 Misión del Televisor (Papá)

1. El papá pide arreglar el televisor.
2. Encienda la TV presionando **B**.
3. Se activa la interferencia. Ajuste las antenas.
4. Mire cada antena y use el joystick para ajustar su posición.
5. Cuando ambas antenas estén en la posición correcta, la misión se completa.

---

## 8. Consejos

- **El puntero verde central** indica hacia dónde está mirando. Los objetos se resaltan en verde cuando son interactuables.
- Si se siente perdido, explore las habitaciones y hable con los personajes. Ellos le darán pistas.
- Los cajones pueden contener objetos importantes para las misiones.
- Cuando inspeccione un objeto en 360°, el cajón se bloquea hasta que devuelva el objeto con **X**.
- Las puertas y cajones tienen sonido al abrir/cerrar.

---

## 9. Solución de Problemas

| Problema | Solución |
|----------|----------|
| El joystick no responde | Reconecte el Bluetooth. Cierre y reabra la app. |
| La pantalla está dividida (modo VR) pero no tengo visor | En ajustes del teléfono, desactive el modo Cardboard, o presione el icono de engranaje dentro de la app. |
| El juego va lento | Cierre otras aplicaciones. Asegúrese de tener al menos 2GB de RAM disponible. |
| No puedo instalar el APK | Verifique que activó "Fuentes desconocidas" en Seguridad. |
| Los textos no se ven | Reinicie la aplicación. Si persiste, reporte al equipo de desarrollo. |
| Me caí del mapa | El juego lo reposicionará automáticamente. Si no, reinicie la escena. |

---

## 10. Créditos

**Memorias Fragmentadas**
Proyecto académico — Universidad Militar Nueva Granada

---

*Versión del manual: 1.0 — Junio 2026*
