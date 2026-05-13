# Cómo Funciona el Loading Screen Automático

## Tu Pregunta: "¿Dónde coloco el loading screen en la escena?"

**Respuesta: NO lo colocas en la escena. Se crea automáticamente en tiempo de ejecución.**

---

## Cómo Funciona (Internamente)

### Flujo de Transición

```
Usuario presiona botón
        ↓
SceneTransitionManager.LoadScene() se ejecuta
        ↓
ShowLoadingScreen() crea el Canvas
        ↓
SceneDebugNavigator.CreateLoadingScreen() genera:
  • Canvas (contenedor)
  • Panel (fondo gris)
  • Text "Cargando"
  • Image (barra de progreso)
  • Text (porcentaje)
  • LoadingScreenAnimator (animación)
        ↓
Canvas aparece en pantalla
        ↓
Mientras carga la escena:
  • Texto anima los puntos
  • Barra avanza
  • Porcentaje se actualiza
        ↓
Escena cargada
        ↓
HideLoadingScreen() destruye el Canvas
        ↓
Nueva escena aparece
```

---

## El Canvas se Crea Automáticamente

Cuando ejecutes el juego y presiones un botón:

```csharp
// En SceneTransitionManager.cs
private void ShowLoadingScreen()
{
    currentLoadingCanvas = SceneDebugNavigator.CreateLoadingScreen();
    // ^ Esto genera toda la UI automáticamente
    DontDestroyOnLoad(currentLoadingCanvas.gameObject);
}
```

**Resultado:**

- Canvas aparece automáticamente
- No necesitas haberlo creado manualmente
- Se destruye automáticamente cuando termina

---

## Jerarquía en Tiempo de Ejecución

Mientras está cargando, verías esto en la Jerarquía (si pausaras):

```
Hierarchy
├─ Menu (o la escena actual)
│  ├─ [tu contenido]
│  └─ SceneInitializer
│
└─ LoadingCanvas (CREADO AUTOMÁTICAMENTE)
   ├─ Background
   │  └─ LoadingPanel
   │     ├─ LoadingText
   │     ├─ ProgressBarContainer
   │     │  └─ ProgressBarFill
   │     └─ PercentText
   ├─ LoadingScreenAnimator (script)
   └─ PercentageUpdater (script)
```

**Cuando termina la carga, el LoadingCanvas se destruye automáticamente.**

---

## Ubicación en Pantalla

El loading screen siempre aparece en el **centro de la pantalla** porque se crea con:

```csharp
RectTransform panelRect = panelGO.AddComponent<RectTransform>();
panelRect.anchoredPosition = Vector2.zero;  // Centro
```

**Resultado:** Siempre centrado, sin importar la resolución de pantalla.

---

## ¿Puedo Personalizarlo?

**Sí, editando SceneDebugNavigator.cs método `CreateLoadingScreen()`:**

### Cambiar posición

```csharp
// Izquierda
panelRect.anchorMin = new Vector2(0, 0.5f);
panelRect.anchorMax = new Vector2(0, 0.5f);
panelRect.anchoredPosition = new Vector2(50, 0);

// Arriba
panelRect.anchorMin = new Vector2(0.5f, 1);
panelRect.anchorMax = new Vector2(0.5f, 1);
panelRect.anchoredPosition = new Vector2(0, -50);
```

### Cambiar colores

```csharp
// Fondo más claro
bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

// Barra de progreso roja
barFillImage.color = new Color(1f, 0f, 0f, 1f);

// Texto amarillo
text.color = Color.yellow;
```

### Cambiar tamaño

```csharp
panelRect.sizeDelta = new Vector2(800, 600); // Más grande
```

### Cambiar mensaje

```csharp
text.text = "Iniciando escena...";
// En LoadingScreenAnimator.cs, modifica el array:
string[] frames = new string[] { "Iniciando.", "Iniciando..", "Iniciando..." };
```

---

## Why Automatic?

Esta aproximación es mejor porque:

1. **No contaminates las escenas** - No necesitas un GameObject en cada escena
2. **Es reutilizable** - El mismo código funciona en todas las transiciones
3. **Es dinámico** - Se adapta a cualquier resolución de pantalla
4. **Es temporal** - Se destruye automáticamente cuando termina
5. **Es singleton** - El manager persiste entre escenas

---

## En la APK

Cuando hagas build de la APK y ejecutes en teléfono, pasará exactamente lo mismo:

1. Presionas un botón (o evento de tu juego)
2. Se muestra el loading screen automáticamente
3. Vuelve a desaparecer cuando carga

**El usuario nunca verá que se está creando dinámicamente - solo verá la pantalla de carga bonita. ✓**

---

## Resumen

```
¿Necesito crear un GameObject/Canvas en la escena?
→ NO, se crea automáticamente

¿Dónde aparece?
→ Centro de pantalla

¿Se destruye automáticamente?
→ SÍ, cuando termina la carga

¿Puedo personalizarlo?
→ SÍ, editando SceneDebugNavigator.cs

¿Funciona en APK?
→ SÍ, igual que en editor
```

---

## Diagrama de Generación

```
CreateLoadingScreen() (estático)
        ↓
    Canvas
        ↓
    RectTransform
        ↓
    CanvasScaler
        ↓
    Background (Image)
        ↓
    LoadingPanel (Image)
        ├─ LoadingText (Text)
        ├─ ProgressBarContainer (Image)
        │  └─ ProgressBarFill (Image - Filled)
        └─ PercentText (Text)
        ↓
    LoadingScreenAnimator (MonoBehaviour)
        ↓
    PercentageUpdater (MonoBehaviour)
        ↓
    Devuelve Canvas → DontDestroyOnLoad
```

**Todo esto se genera en una línea de código:**

```csharp
Canvas myLoadingScreen = SceneDebugNavigator.CreateLoadingScreen();
```

¡Simple y efectivo! ✓
