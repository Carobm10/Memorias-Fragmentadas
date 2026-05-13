# Reparar Errores de Menu.unity - Guía Rápida

## El Problema

Tu escena Menu.unity tiene:

- ❌ TextMeshPro sin fuentes asignadas (J, Borrar, G, E, Enviar, T, U, S, A, R)
- ❌ Prefab faltante "Hoja"
- ❌ RectTransform faltante en algunos GameObjects
- ❌ Renderer abstracto causando conflictos

Estos son errores **pre-existentes** en tu escena, no causados por el sistema de transición.

---

## Opción 1: Reparación Automática (Recomendado)

### Paso 1: Usa el Script de Reparación

1. Abre `Menu.unity` en el editor
2. En la Jerarquía, selecciona cualquier GameObject
3. En el Inspector → **Add Component**
4. Busca y añade: `MenuSceneFixerScript`

### Paso 2: Ejecuta la Reparación

1. En el Inspector, encontrarás el script `MenuSceneFixerScript`
2. Verás dos botones (métodos contextuales):
   - **"Arreglar todos los TextMeshPro"** ← Click aquí primero
   - **"Limpiar Renderer Abstracto"** ← Click aquí segundo

3. Observa la Consola para ver qué se reparó

### Paso 3: Limpia

1. Delete el GameObject con `MenuSceneFixerScript`
2. Save la escena (Ctrl+S)

**¡Listo!** Los errores de TextMeshPro deberían desaparecer.

---

## Opción 2: Reparación Manual

Si prefieres hacerlo manualmente:

### Para cada GameObject (J, Borrar, G, E, Enviar, T, U, S, A, R):

1. Selecciona el GameObject en la Jerarquía
2. En el Inspector, busca **TextMeshPro** component
3. En el campo **"Font Asset"**:
   - Click en el círculo/selector
   - Busca **"LiberationSans SDF"** (o cualquier fuente disponible)
   - Selecciona y asigna
4. Si falta **RectTransform**:
   - Click en **"Add Component"**
   - Busca **"RectTransform"**
   - Añade

---

## Opción 3: Reinstalar TextMesh Pro

Si los errores persisten:

1. Ve a **Window → TextMesh Pro → Import TMP Essential Resources**
2. Haz click en **Import**
3. Espera a que termine
4. Abre `Menu.unity` de nuevo

Esto reinstala todas las fuentes de TextMesh Pro.

---

## Para el Prefab Faltante "Hoja"

```
Error: "Prefab instance problem. Missing Prefab Asset: 'Hoja (Missing Prefab with guid: fda12a8f0d9720c4cae230bbdd3a2920)'"
```

Soluciones:

### Opción A: Restaurar desde Backup

Si tienes un backup del proyecto, restaura el prefab "Hoja".

### Opción B: Eliminar la Instancia

1. En la Jerarquía, busca un GameObject que diga "Missing"
2. Click derecho → Delete
3. Si lo necesitas, crea un nuevo GameObject vacío

### Opción C: Recrear el Prefab

Si sabes qué es "Hoja":

1. Crea un nuevo GameObject con la estructura que necesitas
2. Arrastralo a una carpeta (Assets/...) para crear un prefab
3. Asigna el GUID si es necesario

---

## Checklist de Reparación

Después de ejecutar la reparación:

- [ ] No hay errores rojos en la Consola
- [ ] No hay advertencias sobre "LiberationSans SDF"
- [ ] No hay "Missing Renderer"
- [ ] La escena Menu.unity abre sin problemas
- [ ] Los GameObjects de letras (J, Borrar, etc.) se ven correctamente
- [ ] Puedes hacer Play sin crashes

---

## Próximo Paso

Una vez reparada la escena Menu.unity:

1. Sigue los 4 pasos del **QUICK_START_GUIDE.md**
2. Crea los GameObjects ProjectInitializer y SceneInitializer
3. Haz Play y prueba el sistema de transición

---

## Si Aún Hay Errores

Después de la reparación, si aún ves advertencias:

```
✓ Las advertencias de fuentes pueden ignorarse (no afectan al juego)
✓ El juego funciona aunque haya advertencias
✓ Solo los errores rojos bloquean la ejecución
```

Para completar la eliminación de advertencias:

1. Selecciona cada TextMeshPro
2. Asegúrate de que "Font Asset" NO esté vacío
3. Si está vacío, assign cualquier fuente disponible

---

## Nota sobre el Sistema de Transición

Los errores de Menu.unity **NO afectan** al sistema de transición que implementé.

El loading screen y los botones de debug funcionarán correctamente aunque Menu.unity tenga estos errores. Pero es mejor repararlos para tener una escena limpia.

---

## Resumen

```
Problema: TextMeshPro sin fuentes en Menu.unity
Solución: Script automático MenuSceneFixerScript.cs
Tiempo: 2-3 minutos
Dificultad: ⭐ Muy fácil
```

¿Necesitas ayuda con algún paso?
