# Troubleshooting - Sistema de Transición de Escenas

## Problemas Comunes y Soluciones

### 1. Pantalla negra o congelada durante transición

**Causa**: Las cámaras no se están activando correctamente en la nueva escena.

**Solución**:

1. Asegúrate de que tu escena tenga al menos una cámara activa
2. Verifica en la Consola si hay errores de referencia
3. Si la cámara está en un Canvas, asegúrate de que está configurada correctamente

### 2. El video/audio se reproduce automáticamente

**Causa**: El VideoPlayer o AudioSource tienen `playOnAwake` activado.

**Solución**:

1. Ve a tu VideoPlayer/AudioSource en la escena
2. Desactiva la opción "Play On Awake"
3. El SceneTransitionManager lo hará automáticamente, pero es buena práctica desactivarlo

### 3. Los botones de debug no aparecen

**Causa**: El SceneDebugNavigator no se está creando o hay un error en la creación de UI.

**Solución**:

1. Abre la Consola (Window > General > Console)
2. Busca mensajes de error relacionados con "SceneDebugNavigator"
3. Verifica que Canvas está siendo creado correctamente
4. Si el error es sobre "Text" o fuentes, intenta usar TextMeshPro en lugar de Text legacy

### 4. El SceneTransitionManager no se encuentra

**Causa**: No hay un ProjectInitializer en la escena inicial.

**Solución**:

1. Asegúrate de seguir el Paso 1 de la configuración
2. Crea un GameObject "ProjectInitializer" en Menu.unity
3. Añádele el script ProjectInitializer.cs

### 5. Las transiciones son muy lentas

**Causa**: El tiempo mínimo de carga es muy alto.

**Solución**:

1. Selecciona el SceneTransitionManager en la escena
2. En el Inspector, reduce `Min Loading Screen Time`
3. Valor recomendado: 0.5 - 1 segundo

### 6. El contenido multimedia se sigue reproduciendo de la escena anterior

**Causa**: Los AudioSources/VideoPlayers no se están deteniendo correctamente.

**Solución**:

1. Verifica que los GameObject con AudioSource/VideoPlayer estén siendo destruidos al cambiar de escena
2. O marca "Don't Destroy On Load" solo para los que necesites mantener entre escenas
3. El SceneTransitionManager debería detener todos automáticamente

### 7. Los botones no funcionan

**Causa**: El SceneTransitionManager no se encontró en FindFirstObjectByType.

**Solución**:

1. Verifica que el ProjectInitializer se ejecutó (revisa la consola)
2. Intenta crear manualmente un GameObject "SceneTransitionManager" en la primera escena
3. Asegúrate de que DontDestroyOnLoad esté activado

### 8. La pantalla de carga no aparece

**Causa**: El método CreateLoadingScreen no se está llamando correctamente.

**Solución**:

1. Verifica en la Consola que "Pantalla de carga mostrada" aparece
2. Si no aparece, revisa que ShowLoadingScreen se llama en TransitionToScene
3. Intenta crear manualmente una Canvas llamada "LoadingCanvas" para test

### 9. La barra de progreso no se anima

**Causa**: El LoadingScreenAnimator no está siendo asignado correctamente.

**Solución**:

1. En el GameObject "LoadingCanvas", verifica que el componente LoadingScreenAnimator esté asignado
2. En la Consola, busca errores sobre "LoadingScreenAnimator"
3. Asegúrate de que el Image del Fill tiene `Type: Filled` y `Fill Method: Horizontal`

### 10. El texto "Cargando" no parpadea

**Causa**: La corutina de animación no se está ejecutando.

**Solución**:

1. Verifica que el Canvas es un GameObject activo
2. Revisa que el componente Text existe y es visible
3. Comprueba que el LoadingScreenAnimator tiene acceso al componente Text

## Verificación de Configuración

Para asegurar que todo esté correctamente configurado:

### Checklist de Configuración

- [ ] Menu.unity tiene un GameObject "ProjectInitializer" con el script ProjectInitializer.cs
- [ ] Escena_VideoIntro.unity tiene un GameObject "SceneInitializer" con el script SceneInitializer.cs
- [ ] BASE.unity tiene un GameObject "SceneInitializer" con el script SceneInitializer.cs
- [ ] Las escenas están en Build Settings en el orden correcto
- [ ] VideoPlayers tienen "Play On Awake" desactivado
- [ ] AudioSources tienen "Play On Awake" desactivado
- [ ] Cada escena tiene al menos una cámara configurada
- [ ] No hay errores en la Consola al cargar las escenas

### Verificación de Transición

1. Inicia el juego desde Menu.unity
2. Deberías ver los botones de debug en la esquina derecha
3. Presiona "Siguiente →" y observa:
   - [ ] Aparece pantalla de carga con fondo oscuro
   - [ ] Texto "Cargando." con animación de puntos
   - [ ] Barra de progreso azul que se llena
   - [ ] Porcentaje aumenta de 0% a 100%
   - [ ] Cuando termina, muestra "¡Listo!" en 100%
   - [ ] Pantalla desaparece después de ~1.5 segundos
   - [ ] Se carga Escena_VideoIntro correctamente
   - [ ] Los botones de debug aparecen de nuevo
4. Continúa hasta BASE.unity
5. Verifica que todo funciona correctamente

## Logs Esperados

Cuando todo funciona correctamente, deberías ver en la Consola:

```
ProjectInitializer: SceneTransitionManager creado
ProjectInitializer: UI de debug creado para la escena inicial
VideoSceneController iniciado
Contenido multimedia precargado (sin reproducción automática)
SceneDebugNavigator UI creado en la esquina derecha
```

## Optimizaciones Avanzadas

### Para mejorar rendimiento:

1. **Preload de Assets**: Carga modelos 3D antes de mostrar la escena
2. **Pool de GameObjects**: Reutiliza GameObjects entre escenas
3. **Async Asset Loading**: Carga assets de forma asincrónica

### Para mejorar UX:

1. **Pantalla de carga personalizada**: Ya incluida con animaciones
   - Puedes modificar colores, velocidades y mensajes
   - Modifica `CreateLoadingScreen()` en SceneDebugNavigator.cs
   - Modifica `LoadingScreenAnimator.cs` para cambiar animaciones
2. **Barra de progreso**: Ya incluida y funcional
3. **Transiciones suaves**: Usa `minLoadingScreenTime` para ajustar duración

## Preguntas Frecuentes

**P: ¿Cómo se ve la pantalla de carga en la APK?**
R: Muestra un fondo oscuro con "Cargando." (puntos animados), una barra de progreso azul claro y porcentaje. Al completar, muestra "¡Listo!" y desaparece.

**P: ¿Puedo personalizar la pantalla de carga?**
R: Sí, modifica colores en `CreateLoadingScreen()`, velocidades en `LoadingScreenAnimator.cs`, o el contenido según necesites

**P: ¿Los botones de debug desaparecen en producción?**
R: Puedes desactivar la creación de UI de debug en el Inspector, o eliminar los GameObjects ProjectInitializer/SceneInitializer antes de hacer build

**P: ¿Soporta múltiples escenas cargadas simultáneamente?**
R: Actualmente no, pero se puede modificar el SceneTransitionManager para LoadSceneAsync con LoadSceneMode.Additive

**P: ¿Puedo saltar directamente a una escena específica?**
R: Sí, usa `transitionManager.LoadScene("NombreEscena")`

**P: ¿Por qué se demora 1 segundo mínimo?**
R: Es para dar tiempo a que el usuario vea que algo está pasando. Puedes reducirlo modificando `minLoadingScreenTime`
