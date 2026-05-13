# Paso a Paso - Configuración Final del Sistema de Transición

## Respuesta a tu pregunta: "¿Creo un GameObject? ¿Un Text?"

**Respuesta: NO necesitas crear nada manualmente en las escenas.**

El loading screen se crea automáticamente en **tiempo de ejecución**. Los scripts lo generan dinámicamente cuando cambias de escena. Solo necesitas:

1. ✓ Los scripts ya están creados
2. ✓ El loading screen se crea automáticamente
3. ✓ Los botones de debug se crean automáticamente

Solo necesitas configurar 4 cosas simples en las escenas.

---

## Paso 1: Abre Menu.unity

1. En el Project, navega a: `Assets/PROYECTO OFICIAL/Scenes/`
2. **Double-click** en `Menu.unity` para abrir la escena

---

## Paso 2: Crea un GameObject en Menu.unity

1. Click derecho en la Jerarquía → **Create Empty**
2. Renómbralo a: `ProjectInitializer`
3. En el Inspector, arrastra el script `ProjectInitializer.cs` al componente (o Add Component → ProjectInitializer)

**Resultado en Inspector:**

```
ProjectInitializer
├─ ProjectInitializer (Script)
│  ├─ Create Transition Manager: ✓ (debe estar marcado)
```

Este GameObject se auto-destruirá después de inicializar el sistema. ✓

---

## Paso 3: Abre Escena_VideoIntro.unity

1. En el Project, navega a: `Assets/PROYECTO OFICIAL/Scenes/`
2. **Double-click** en `Escena_VideoIntro.unity`

---

## Paso 4: Crea un GameObject en Escena_VideoIntro.unity

1. Click derecho en la Jerarquía → **Create Empty**
2. Renómbralo a: `SceneInitializer`
3. Arrastra el script `SceneInitializer.cs` (o Add Component → SceneInitializer)

**Resultado en Inspector:**

```
SceneInitializer
├─ SceneInitializer (Script)
│  ├─ Create Debug Navigator: ✓
│  ├─ Debug Mode: ✓
```

---

## Paso 5: Abre BASE.unity

1. En el Project, navega a: `Assets/PROYECTO OFICIAL/Scenes/BASE/`
2. **Double-click** en `BASE.unity`

---

## Paso 6: Crea un GameObject en BASE.unity

1. Click derecho en la Jerarquía → **Create Empty**
2. Renómbralo a: `SceneInitializer`
3. Arrastra el script `SceneInitializer.cs`

**Resultado en Inspector:**

```
SceneInitializer
├─ SceneInitializer (Script)
│  ├─ Create Debug Navigator: ✓
│  ├─ Debug Mode: ✓
```

---

## Paso 7: Verifica Build Settings

1. Ve a **File → Build Settings**
2. Verifica que las escenas estén listadas en este orden:

```
Scenes In Build:
├─ [0] Assets/PROYECTO OFICIAL/Scenes/Menu.unity
├─ [1] Assets/PROYECTO OFICIAL/Scenes/Escena_VideoIntro.unity
├─ [2] Assets/PROYECTO OFICIAL/Scenes/BASE/BASE.unity
```

Si no están en ese orden:

- Arrastra las escenas a las posiciones correctas
- O añádelas manualmente (Drag and drop desde Project a la lista)

---

## Listo! ¿Qué sucede ahora?

Cuando ejecutes el juego:

### En el Editor (Play)

1. Presiona **Play** desde Menu.unity
2. Verás dos botones en la esquina derecha: `← Anterior` y `Siguiente →`
3. Presiona `Siguiente →`
4. Se mostrará la pantalla de carga con:
   - Texto "Cargando." (anima los puntos)
   - Barra azul llenándose
   - Porcentaje actualizado en vivo
5. Se carga Escena_VideoIntro automáticamente
6. Vuelven a aparecer los botones de debug
7. Repite el proceso para las otras escenas

### En APK (Teléfono)

Exactamente lo mismo, pero sin los botones de debug (puedes desactivarlos antes de hacer build)

---

## Ejemplo Visual de lo que ves

### Estado 1: Menu.unity cargado

```
┌─────────────────────────┐
│                         │
│   (Tu contenido)        │
│                         │
│    ┌─────────────────┐  │
│    │ ← Anterior      │  │
│    │ Siguiente →     │  │
│    └─────────────────┘  │
└─────────────────────────┘
     (botones en esquina derecha)
```

### Estado 2: Presionas "Siguiente →"

```
┌─────────────────────────┐
│                         │
│   Cargando.             │
│                         │
│ ┌────────────────────┐  │
│ │████░░░░░░░░░░░░░░│ 25%│
│ └────────────────────┘  │
│                         │
└─────────────────────────┘
```

### Estado 3: Cuando termina

```
┌─────────────────────────┐
│                         │
│   ¡Listo!               │
│                         │
│ ┌────────────────────┐  │
│ │████████████████████│100%│
│ └────────────────────┘  │
│                         │
└─────────────────────────┘
(desaparece automáticamente)
```

### Estado 4: Escena_VideoIntro cargada

```
┌─────────────────────────┐
│                         │
│   (Tu contenido)        │
│                         │
│    ┌─────────────────┐  │
│    │ ← Anterior      │  │
│    │ Siguiente →     │  │
│    └─────────────────┘  │
└─────────────────────────┘
```

---

## Resumen de Pasos

| Paso | Acción                             | Ubicación               |
| ---- | ---------------------------------- | ----------------------- |
| 1    | Create Empty GameObject            | Menu.unity              |
| 2    | Renombra a "ProjectInitializer"    | Jerarquía               |
| 3    | Añade script ProjectInitializer.cs | Inspector               |
| 4    | Create Empty GameObject            | Escena_VideoIntro.unity |
| 5    | Renombra a "SceneInitializer"      | Jerarquía               |
| 6    | Añade script SceneInitializer.cs   | Inspector               |
| 7    | Create Empty GameObject            | BASE.unity              |
| 8    | Renombra a "SceneInitializer"      | Jerarquía               |
| 9    | Añade script SceneInitializer.cs   | Inspector               |
| 10   | Verifica Build Settings            | File → Build Settings   |

---

## Verificación

Después de hacer todo, ejecuta estas pruebas:

- [ ] Play desde Menu.unity
- [ ] Ves botones en esquina derecha
- [ ] Presionas "Siguiente →"
- [ ] Se muestra pantalla de carga con progreso
- [ ] Se carga Escena_VideoIntro
- [ ] Los botones aparecen de nuevo
- [ ] No hay errores rojos en la Consola
- [ ] Continúas a BASE.unity sin problemas

---

## Si hay Errores

Abre **Window → General → Console** y busca:

✓ "ProjectInitializer: SceneTransitionManager creado"
✓ "SceneDebugNavigator UI creado en la esquina derecha"
✓ "Pantalla de carga mostrada"
✓ "Pantalla de carga ocultada"

Si ves estos mensajes = **¡Todo funciona! ✓**

Si ves errores rojos, revisa:

1. ¿Los GameObjects se llaman exactamente "ProjectInitializer" y "SceneInitializer"?
2. ¿Los scripts están asignados en el Inspector?
3. ¿Las escenas están en Build Settings en el orden correcto?

---

## Resultado Final

✓ Sistema de transición completo
✓ Pantalla de carga visual automática
✓ Botones de debug para testing
✓ Sin freezes o pantallas negras
✓ Listo para APK

¡Ahora puedes testear el flujo completo de escenas!
