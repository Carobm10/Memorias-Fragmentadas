# Visual Loading Screen - Referencia Rápida

## Cómo se ve en APK/Dispositivo

### Estado 1: Cargando...

```
┌─────────────────────────────────────────┐
│                                         │
│                                         │
│          Cargando.                      │
│          (animando puntos)              │
│                                         │
│      ┌──────────────────────────┐       │
│      │████░░░░░░░░░░░░░░░░░░░░│       │
│      └──────────────────────────┘       │
│                                         │
│                 25%                     │
│                                         │
│                                         │
└─────────────────────────────────────────┘
```

### Estado 2: Progreso avanzado

```
┌─────────────────────────────────────────┐
│                                         │
│                                         │
│          Cargando..                     │
│          (cambio de puntos)             │
│                                         │
│      ┌──────────────────────────┐       │
│      │████████████████░░░░░░░░│       │
│      └──────────────────────────┘       │
│                                         │
│                 68%                     │
│                                         │
│                                         │
└─────────────────────────────────────────┘
```

### Estado 3: Completado

```
┌─────────────────────────────────────────┐
│                                         │
│                                         │
│          ¡Listo!                        │
│          (transición completada)        │
│                                         │
│      ┌──────────────────────────┐       │
│      │████████████████████████│       │
│      └──────────────────────────┘       │
│                                         │
│                100%                     │
│                                         │
│                                         │
└─────────────────────────────────────────┘
(desaparece automáticamente)
```

## Detalles Técnicos

### Colores

- **Fondo**: Negro muy oscuro (0, 0, 0) con 95% transparencia (casi opaco)
- **Panel**: Gris oscuro (0.1, 0.1, 0.15) con 80% opacidad
- **Barra (fondo)**: Gris claro (0.2, 0.2, 0.25)
- **Barra (relleno)**: Azul claro (0.2, 0.8, 1.0) - muy visible
- **Texto**: Blanco puro

### Dimensiones

- **Panel**: 600x400 píxeles centrado en pantalla
- **Barra de progreso**: 500x40 píxeles
- **Texto "Cargando"**: Tamaño de fuente 48pt, Bold
- **Porcentaje**: Tamaño de fuente 32pt, Bold

### Animaciones

- **Texto**: Cambia cada 0.4 segundos
  - Frame 1: "Cargando."
  - Frame 2: "Cargando.."
  - Frame 3: "Cargando..."
- **Barra**: Se llena suavemente con progreso simulado
  - Comienza en 20%
  - Avanza gradualmente con pequeños saltos
  - Llega a 100% cuando la escena está lista
- **Porcentaje**: Se actualiza en tiempo real

### Tiempo Total

- Mínimo: ~1.5 segundos (1 segundo de precarga + 0.5 segundos para mostrar "¡Listo!")
- Máximo: Depende de la complejidad de la escena y poder del dispositivo
- En dispositivos rápidos: 1.5-2 segundos
- En dispositivos lentos: 2-3 segundos

## Interacción del Usuario

- El usuario **NO puede interactuar** con la pantalla de carga
- La pantalla **bloquea raycast** (toca todo el área)
- Se destruye automáticamente cuando la escena está lista
- No requiere ninguna entrada del usuario

## Logs en Consola

Mientras se carga, verás estos mensajes:

```
Pantalla de carga mostrada
Contenido multimedia precargado (sin reproducción automática)
SceneDebugNavigator UI creado en la esquina derecha
Pantalla de carga ocultada
```

## Optimización para Diferentes Dispositivos

### Móviles potentes (Galaxy S20, iPhone 12+)

- Pantalla visible: ~1.5 segundos
- La barra llena suavemente
- El progreso parece lineal

### Móviles medianos (Galaxy S10, iPhone XS)

- Pantalla visible: ~2-2.5 segundos
- La barra tiene pequeños saltos visibles
- El progreso es más variable

### Móviles antiguos (Galaxy S5, iPhone 6)

- Pantalla visible: ~3-5 segundos
- La barra tiene más saltos
- El progreso es más lento y notable

## Personalización Recomendada

Para mejorar la experiencia visual:

1. **Agregar logo del proyecto**:
   - Añade una imagen en el centro del panel
   - Encima del texto "Cargando"

2. **Cambiar mensaje**:
   - De "Cargando." a "Iniciando..." o "Preparando..."
   - Diferentes mensajes para cada escena

3. **Agregar tips**:
   - Muestra consejos mientras carga
   - Cada 2-3 segundos cambia el tip

4. **Efectos visuales**:
   - Animación de giro/pulso en el logo
   - Cambio de color de la barra (rojo → naranja → verde)
   - Brillo del texto
