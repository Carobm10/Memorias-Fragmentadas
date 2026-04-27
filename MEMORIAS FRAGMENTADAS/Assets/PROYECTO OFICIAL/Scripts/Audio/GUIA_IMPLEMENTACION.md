# IMPLEMENTACIÓN PASO A PASO - SISTEMA DE AUDIO

## 🎯 OBJETIVO

Crear un sistema flexible que permita:

- Reproducir audios en momentos específicos (por tiempo)
- Activar audios cuando el jugador entra en áreas (por posición)
- Manejar múltiples audios y secuencias complejas

---

## 📦 ARCHIVOS CREADOS

```
Assets/PROYECTO OFICIAL/Scripts/Audio/
├─ AudioClipData.cs                  ✅ Estructura de datos
├─ AudioScriptManager.cs             ✅ Manager por tiempo
├─ AudioTrigger.cs                   ✅ Activador por posición
├─ AudioScriptAdvanced.cs            ✅ Secuencias complejas
├─ AudioControlExample.cs            ✅ Script de ejemplo
└─ README_AUDIO_SYSTEM.md            ✅ Documentación completa
```

---

## 🚀 INICIO RÁPIDO

### CASO 1: AUDIOS SECUENCIADOS POR TIEMPO

**Ideal para:** Narraciones, cinemáticas, eventos temporales

1. En PruebasEsteban.unity, crea un GameObject vacío
2. Nómbralo: `AudioManager`
3. Añade componente: `AudioScriptManager`
4. Configura los audios en el Inspector:

   ```
   - Nombre: "Intro"
   - Clip: [Tu archivo de audio]
   - Delay: 0s
   - Reproducir Al Inicio: ✓

   - Nombre: "Diálogo"
   - Clip: [Tu archivo de audio]
   - Delay: 3s
   - Volumen: 0.8
   ```

**Result:** Los audios se reproducen automáticamente según el delay

---

### CASO 2: AUDIOS POR POSICIÓN DEL JUGADOR

**Ideal para:** Efectos de ambientación, alertas de zona, diálogos contextuales

1. Crea un GameObject: `TriggerZona_PuertaEntrada`
2. Añade un Collider (Box, Sphere, Capsule - lo que prefieras)
3. **CRÍTICO:** Marca `Is Trigger = true`
4. Añade componente: `AudioTrigger`
5. En el Inspector:

   ```
   Audio Data → Crear nuevo
   └─ Clip: [Tu archivo]
   └─ Nombre: "Puerta crujiendo"
   └─ Volumen: 0.6

   Reproducir Al Entrar: ✓
   Reproducir Una Vez Solo: ✓
   Tag Jugador: "Player"

   (Opcional) Volumen Dinámico: ✓
   (Opcional) Distancia Máxima: 15
   ```

**Result:** El audio suena cuando el jugador entra en el área

---

### CASO 3: SECUENCIAS COMPLEJAS

**Ideal para:** Diálogos entre personajes, narraciones largas

1. Crea: `Narrator` (GameObject)
2. Añade: `AudioScriptAdvanced`
3. Configura eventos:
   ```
   Audio Events → +
   └─ Nombre: "Diálogo Principal"
   └─ Audios:
      ├─ Audio 1: [voz personaje A]
      ├─ Audio 2: [voz personaje B]
      ├─ Audio 3: [efecto transición]
   └─ Delay Entre Los: 0.5s
   ```
4. Desde código o UI, llama:
   ```csharp
   audioScriptAdvanced.StartAudioSequence();
   ```

---

## 🎛️ CONTROLES DESDE CÓDIGO

```csharp
// En cualquier script
AudioScriptManager manager = FindObjectOfType<AudioScriptManager>();

// Reproducir ahora
manager.PlayAudioImmediate(clip, volumen: 0.8f, pitch: 1f);

// Controlar reproducción
manager.PauseAudio();
manager.ResumeAudio();
manager.StopAudio();

// Estado
bool isPlaying = manager.IsPlaying();
float time = manager.GetElapsedTime();

// Reiniciar
manager.ResetScript();
```

---

## 🔧 CONFIGURACIÓN RECOMENDADA

### Para voces/diálogos:

- Volumen: 0.7 - 1.0
- Pitch: 1.0 (normal)
- Loop: false

### Para efectos de sonido:

- Volumen: 0.5 - 0.8
- Pitch: 0.8 - 1.2 (variar)
- Loop: false

### Para ambientación:

- Volumen: 0.3 - 0.5
- Pitch: 1.0
- Loop: true

---

## ✅ CHECKLIST FINAL

- [ ] Importé todos los AudioClips (wav/mp3) a Assets
- [ ] Creé un GameObject con AudioScriptManager (opcional si solo uso triggers)
- [ ] Configuré al menos 2 audios con delays diferentes
- [ ] El jugador tiene el tag "Player" (Window > Tags and Layers)
- [ ] Creé áreas de trigger con Collider + Is Trigger = true
- [ ] Cada AudioTrigger tiene su AudioClipData
- [ ] Probé la escena con Play (F5)
- [ ] Los audios se reproducen en el momento correcto
- [ ] Los volúmenes son los esperados

---

## 🎮 PRUEBAS

En PruebasEsteban.unity:

1. **Prueba audios por tiempo:**
   - Presiona Play
   - Verifica que los audios suenen en el momento correcto

2. **Prueba audios por posición:**
   - Mueve el jugador hacia las áreas de trigger
   - Verifica que el audio suene al entrar

3. **Prueba controles (si usas AudioControlExample):**
   - Presiona 1, 2, 3, 4, 5, 6 para probar diferentes acciones
   - Presiona I para ver información

---

## 🐛 DEBUG

Si algo no funciona:

```csharp
// En el console de Unity, verifica logs
// Los scripts imprimen información útil:
Debug.Log("Reproduciendo audio: ...");
Debug.Log("AudioTrigger: Reproduciendo ...");
Debug.Log("AudioScriptManager: Guion reiniciado");
```

---

## 📝 NOTAS

- Los AudioClips deben estar en Assets para que se serialicen
- El sistema es completamente modular: úsalos por separado o combinados
- Puedes crear múltiples managers o triggers en la misma escena
- Los audios se reproducen en AudioSource, puedes ajustar parámetros allí también

---

**¡Listo para usar! 🎵**
