# Guía de Configuración - Sistema de Transición de Escenas

## Descripción General

Se ha implementado un sistema completo de transición entre escenas que incluye:

- **SceneTransitionManager**: Gestor centralizado de transiciones con precarga asincrónica
- **SceneDebugNavigator**: UI de debug en cada escena para cambiar entre ellas
- **ProjectInitializer**: Inicializador que configura el sistema en la primera carga
- **LoadingScreenAnimator**: Pantalla de carga visual con barra de progreso animada

## Pantalla de Carga Visual (APK)

Cuando cambias entre escenas, verás una pantalla visual clara:

```
┌─────────────────────────────┐
│                             │
│     Cargando.              │
│     (anima: . → .. → ...)  │
│                             │
│  ┌─────────────────────┐    │
│  │████████░░░░░░░░    │ 45% │
│  └─────────────────────┘    │
│                             │
│     (cuando termina)        │
│     ¡Listo! - 100%          │
│                             │
└─────────────────────────────┘
```

**Características:**

- Fondo oscuro semi-transparente para mejor visibilidad
- Texto "Cargando" con animación de puntos (., .., ...)
- Barra de progreso azul claro que se llena gradualmente
- Porcentaje visible en tiempo real
- Al completar, muestra "¡Listo!" y barra al 100%
- Desaparece automáticamente cuando la escena está lista
- Diseño optimizado para pantallas móviles

## Archivos Creados

### Scripts Principales

1. **SceneTransitionManager.cs** - Gestor central de transiciones
   - Precarga asincrónica de escenas
   - Pantalla de carga durante las transiciones
   - Manejo automático de cámaras
   - Preload de contenido multimedia sin reproducción automática

2. **SceneDebugNavigator.cs** - Navegador de debug
   - Crea botones en la esquina derecha de pantalla
   - Permite navegar entre escenas en secuencia
   - Botones: "← Anterior" y "Siguiente →"
   - Crea la pantalla de carga visual mejorada

3. **LoadingScreenAnimator.cs** - Animador visual de carga
   - Anima el texto "Cargando" con puntos dinámicos
   - Anima la barra de progreso
   - Muestra porcentaje en tiempo real
   - Muestra "¡Listo!" cuando se completa

4. **SceneInitializer.cs** - Inicializador de escenas
   - Puede añadirse a escenas individuales para crear UI de debug

5. **ProjectInitializer.cs** - Inicializador global
   - Se debe colocar en la escena Menu.unity
   - Crea el SceneTransitionManager en la primera carga

## Scripts Modificados

- **VideoSceneController.cs** - Actualizado para usar SceneTransitionManager
- **Tecla.cs** - Actualizado para usar SceneTransitionManager

## Configuración Requerida en Unity Editor

### Paso 1: Configura el Menu.unity

1. Abre la escena `Assets/PROYECTO OFICIAL/Scenes/Menu.unity`
2. Crea un GameObject vacío llamado "ProjectInitializer"
3. Añádele el script `ProjectInitializer.cs`
4. En el Inspector, asegúrate de que los checkboxes estén marcados:
   - ✓ Create Transition Manager
   - ✓ Create Loading Canvas Prefab

### Paso 2: Configura Escena_VideoIntro.unity

1. Abre `Assets/PROYECTO OFICIAL/Scenes/Escena_VideoIntro.unity`
2. Crea un GameObject vacío llamado "SceneInitializer"
3. Añádele el script `SceneInitializer.cs`
4. En el Inspector:
   - ✓ Create Debug Navigator
   - ✓ Debug Mode

### Paso 3: Configura BASE.unity

1. Abre `Assets/PROYECTO OFICIAL/Scenes/BASE/BASE.unity`
2. Crea un GameObject vacío llamado "SceneInitializer"
3. Añádele el script `SceneInitializer.cs`
4. En el Inspector:
   - ✓ Create Debug Navigator
   - ✓ Debug Mode

### Paso 4: Verifica los Settings de Escenas

1. Ve a File > Build Settings
2. Asegúrate de que las escenas estén en este orden:
   - Index 0: Assets/PROYECTO OFICIAL/Scenes/Menu.unity
   - Index 1: Assets/PROYECTO OFICIAL/Scenes/Escena_VideoIntro.unity
   - Index 2: Assets/PROYECTO OFICIAL/Scenes/BASE/BASE.unity

## Flujo de Transiciones

```
Menu (Escena 0)
    ↓ (Siguiente)
Escena_VideoIntro (Escena 1)
    ↓ (Siguiente)
BASE (Escena 2)
    ↓ (Siguiente)
Menu (vuelve al inicio)
```

También puedes usar el botón "← Anterior" para ir hacia atrás.

## Características Principales

### Sistema de Precarga

- Las escenas se precargan de forma asincrónica
- **Pantalla de carga visual mejorada:**
  - Fondo oscuro semi-transparente para máxima visibilidad
  - Texto "Cargando" con animación de puntos dinámicos
  - Barra de progreso azul claro que se llena gradualmente
  - Porcentaje visible en tiempo real
  - Mensaje "¡Listo!" cuando se completa la carga
  - Tiempo mínimo: 1 segundo (configurable en SceneTransitionManager)
- El contenido multimedia (videos, audios) se precarga pero NO se reproduce automáticamente

### Manejo de Cámaras

- Todas las cámaras de la escena anterior se desactivan antes de cargar la nueva
- La nueva escena puede activar sus propias cámaras sin conflictos

### Botones de Debug

- Aparecen en la esquina superior derecha de la pantalla
- Dos botones: "← Anterior" y "Siguiente →"
- Fondo gris semi-transparente
- Tamaño: 150x100 píxeles (configurable)

### Soporte Multimedia

- **Escena 2 (VideoIntro)**: Los videos se precargan pero no inician automáticamente
  - Puedes iniciarlos manualmente presionando el botón de Play
- **Escena 3 (BASE)**: Los audios se precargan pero no se reproducen automáticamente
  - Se reproducen solo cuando interactúas con los elementos

## Debugging y Registro

Los scripts registran información importante en la consola:

- Inicio del manager
- Carga de escenas
- Precarga de contenido multimedia
- Creación de UI de debug

Para ver esta información, abre la Consola en Unity (Window > General > Console)

## Notas de Implementación

1. **Singleton**: El SceneTransitionManager usa el patrón Singleton y persiste entre escenas
2. **DontDestroyOnLoad**: El manager no se destruye al cambiar de escena
3. **Async Loading**: Por defecto usa carga asincrónica para evitar freezes
4. **Camera Management**: Desactiva todas las cámaras antes de cargar para evitar conflictos de renderizado

## Posibles Ajustes Futuros

Si necesitas:

- **Cambiar el tiempo mínimo de carga**: Modifica `minLoadingScreenTime` en el Inspector
- **Usar carga sincrónica**: Desactiva `useAsyncLoading` en el Inspector
- **Cambiar el tamaño de los botones de debug**: Modifica los valores en `CreateDebugNavigatorUI()`
- **Personalizar colores de la pantalla de carga**:
  - Fondo: Modifica `new Color(0.05f, 0.05f, 0.08f, 0.95f)` en CreateLoadingScreen()
  - Barra de progreso: Modifica `new Color(0.2f, 0.8f, 1f, 1f)` (azul claro)
  - Panel: Modifica `new Color(0.1f, 0.1f, 0.15f, 0.8f)` (gris oscuro)
- **Cambiar velocidad de animación de texto**: Modifica `yield return new WaitForSeconds(0.4f)` en LoadingScreenAnimator
- **Cambiar velocidad de la barra**: Modifica `Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime * 2f)`
- **Usar una imagen de fondo personalizada**: Reemplaza el Color por una Texture en el Background Image

## Soporte de Cardboard VR

El VideoSceneController mantiene su soporte para Cardboard VR y se integra correctamente con el nuevo sistema de transiciones.
