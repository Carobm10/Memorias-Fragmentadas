# SISTEMA DE GUION DE AUDIO - INSTRUCCIONES DE USO

## 📋 Componentes del Sistema

### 1. **AudioClipData** (AudioClipData.cs)

- Estructura serializable que contiene los datos de cada audio
- Campos:
  - `nombre`: Nombre identificativo del audio
  - `clip`: AudioClip (archivo .wav o .mp3)
  - `volumen`: Nivel de volumen (0-1)
  - `pitch`: Velocidad de reproducción (0.5-2.0)
  - `loop`: Si debe repetirse
  - `delay`: Tiempo de espera antes de reproducir
  - `reproducirAlInicio`: Si se reproduce automáticamente al iniciar

### 2. **AudioScriptManager** (AudioScriptManager.cs)

- Manager central para audios **por TIEMPO**
- Características:
  - Maneja múltiples audios con delays configurables
  - Reproduce audios automáticamente según el tiempo transcurrido
  - Permite reproducir audios puntuales o configurados

### 3. **AudioTrigger** (AudioTrigger.cs)

- Script para audios activados **por POSICIÓN del jugador**
- Características:
  - Detecta cuando el jugador entra en un área
  - Soporta volumen dinámico (según distancia)
  - Puede reproducirse una sola vez o múltiples veces
  - Se reinicia con ResetTrigger()

### 4. **AudioScriptAdvanced** (AudioScriptAdvanced.cs)

- Manager avanzado para secuencias complejas
- Características:
  - Maneja múltiples eventos de audio
  - Controla delays entre audios de una secuencia
  - Ideal para diálogos o narraciones

---

## ⚙️ SETUP EN LA ESCENA PruebasEsteban.unity

### OPCIÓN 1: Audios por TIEMPO (AudioScriptManager)

**Paso 1: Crear GameObject vacío**

1. En la escena, crea un nuevo GameObject vacío (Ctrl+Shift+N)
2. Nómbralo: `AudioScriptManager`

**Paso 2: Añadir el script**

1. Selecciona el GameObject
2. Añade componente: `AudioScriptManager`
3. Se creará automáticamente un AudioSource

**Paso 3: Configurar audios por tiempo**

1. En el Inspector, expande "Audios por Tiempo"
2. Haz clic en el + para añadir audios
3. Para cada audio:
   - **Nombre**: (ej: "Voz intro", "Efecto puerta")
   - **Clip**: Arrastra tu archivo de audio aquí
   - **Volumen**: 0.0 - 1.0
   - **Pitch**: 0.5 - 2.0
   - **Loop**: Marca si debe repetirse
   - **Delay**: Tiempo en segundos antes de reproducir
   - **Reproducir Al Inicio**: Marca si debe sonar apenas inicia la escena

**Ejemplo:**

```
Audio 1: Nombre = "Sonido Ambiente"
         Clip = ambient.mp3
         Delay = 0s
         Reproducir Al Inicio = true

Audio 2: Nombre = "Voz Personaje"
         Clip = voice.wav
         Delay = 3s (Suena después de 3 segundos)
         Volumen = 0.8
```

### OPCIÓN 2: Audios por POSICIÓN (AudioTrigger)

**Paso 1: Crear un área de detección**

1. Crea un nuevo GameObject vacío: `AudioTriggerArea_Puerta` (ej)
2. Añade un Collider (Box Collider, Sphere Collider, etc)
3. **IMPORTANTE**: Marca `Is Trigger` = true

**Paso 2: Añadir el script AudioTrigger**

1. Selecciona el GameObject
2. Añade componente: `AudioTrigger`
3. Se creará automáticamente un AudioSource

**Paso 3: Configurar el trigger**
En el Inspector, configura:

**Configuración Trigger:**

- **Audio Data**: Crea un nuevo AudioClipData (o selecciona uno existente)
  - Click derecho en el campo → Create > AudioClipData
  - Llena: nombre, clip, volumen, pitch, etc
- **Reproducir Al Entrar**: true (suena cuando el jugador entra)
- **Reproducir Una Vez Solo**: true (solo una vez) o false (cada vez que entra)
- **Tag Jugador**: "Player" (asegúrate que el jugador tenga este tag)

**Volumen Dinámico (Opcional):**

- **Usar Volumen Dinámico**: true
- **Distancia Máxima**: 20 (el volumen decrece con la distancia)

**Ejemplo de Setup:**

```
GameObject: AudioTriggerArea_Puerta
├─ Box Collider (Is Trigger = true, posicionado en la puerta)
├─ AudioTrigger
│  ├─ Audio Data → AudioClipData
│  │  ├─ Nombre = "Crujido de Puerta"
│  │  ├─ Clip = door_sound.wav
│  │  ├─ Volumen = 0.7
│  │  └─ Reproducir Al Entrar = true
│  ├─ Reproducir Una Vez Solo = false
│  └─ Usar Volumen Dinámico = true
```

### OPCIÓN 3: Secuencias Complejas (AudioScriptAdvanced)

**Paso 1: Crear GameObject**

1. Crea un GameObject: `AudioSequencer`
2. Añade componente: `AudioScriptAdvanced`

**Paso 2: Configurar eventos de audio**

1. En "Audio Events", haz clic en + para añadir un evento
2. Para cada evento:
   - **Nombre**: (ej: "Diálogo Intro")
   - **Audios**: Lista de AudioClipData en orden
   - **Delay Entre Los**: Tiempo entre cada audio

**Paso 3: Llamar desde código**

```csharp
AudioScriptAdvanced audioSequencer = GetComponent<AudioScriptAdvanced>();
audioSequencer.StartAudioSequence();  // Inicia la reproducción
audioSequencer.StopAudioSequence();   // Detiene
```

---

## 🎮 MANEJO POR CÓDIGO (Script personalizado)

Si necesitas controlar audios desde otro script:

```csharp
// Obtener referencias
AudioScriptManager audioManager = GetComponent<AudioScriptManager>();
AudioTrigger audioTrigger = GetComponent<AudioTrigger>();

// Reproducir audio inmediatamente
audioManager.PlayAudioImmediate(miAudioClip, volumen: 0.8f);

// Detener
audioManager.StopAudio();

// Pausar/Reanudar
audioManager.PauseAudio();
audioManager.ResumeAudio();

// Reiniciar todo
audioManager.ResetScript();

// Reiniciar trigger
audioTrigger.ResetTrigger();

// Verificar estado
bool isPlaying = audioManager.IsPlaying();
float tiempoTranscurrido = audioManager.GetElapsedTime();
```

---

## 📁 ESTRUCTURA DE ARCHIVOS

```
Assets/PROYECTO OFICIAL/Scripts/Audio/
├─ AudioClipData.cs           (Estructura de datos)
├─ AudioScriptManager.cs       (Manager por tiempo)
├─ AudioTrigger.cs            (Trigger por posición)
└─ AudioScriptAdvanced.cs     (Secuencias avanzadas)
```

---

## ✅ CHECKLIST DE SETUP

- [ ] Importaste los AudioClips (wav/mp3) a Assets
- [ ] Creaste un GameObject con AudioScriptManager
- [ ] Configuraste al menos un audio con delay
- [ ] El jugador tiene el tag "Player"
- [ ] Creaste áreas de trigger con Is Trigger = true
- [ ] Cada AudioTrigger tiene un AudioClipData configurado
- [ ] Probaste la escena

---

## 🐛 SOLUCIÓN DE PROBLEMAS

**P: El audio no suena**
R:

- Verifica que el clip no sea nulo
- Comprueba el volumen en AudioScriptManager y el clip
- Asegúrate de que el AudioSource esté enabled

**P: Los triggers no funcionan**
R:

- El Collider debe tener `Is Trigger = true`
- El jugador debe tener el tag "Player"
- Verifica que el jugador tenga un Collider (para física)

**P: El audio se corta**
R:

- Asegúrate de que `loop` esté bien configurado
- Si es una secuencia, aumenta el delay

**P: El volumen dinámico no funciona**
R:

- Marca `Usar Volumen Dinámico = true`
- Aumenta `Distancia Máxima` si es necesario

---

## 🚀 PRÓXIMOS PASOS OPCIONALES

1. Crear una UI para controlar la reproducción
2. Añadir fade in/out
3. Sistema de subtítulos sincronizado con audio
4. Guardar/cargar estado de audios reproducidos

---

**Creado para: Memorias Fragmentadas**
**Versión: 1.0**
