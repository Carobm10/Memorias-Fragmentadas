using UnityEngine;

[System.Serializable]
public class AudioClipData
{
    [Header("Datos del Audio")]
    [Tooltip("Nombre visible del audio en el Inspector.")]
    public string nombre = "Nuevo Audio";

    [Tooltip("Archivo de audio a reproducir. Acepta WAV, MP3 y formatos compatibles con Unity.")]
    public AudioClip clip;

    [Tooltip("Volumen del audio. Rango de 0 a 1.")]
    [Range(0f, 1f)]
    public float volumen = 1f;

    [Tooltip("Velocidad de reproducción. 1 = normal. Menor a 1 = más lento, mayor a 1 = más rápido.")]
    [Range(0.5f, 2f)]
    public float pitch = 1f;

    [Tooltip("Si está activo, el audio se repite al terminar.")]
    public bool loop = false;

    [Tooltip("Número total de repeticiones del clip. 1 = una vez, 2 = dos veces. Se ignora si Loop está activo.")]
    [Min(1)]
    public int repeticiones = 1;
    
    [Header("Tiempo")]
    [Tooltip("Espera antes de reproducir este audio, en segundos.")]
    [Min(0f)]
    public float delay = 0f;
    
    [Header("Opciones")]
    [Tooltip("Si está activo, este audio se reproduce al iniciar la escena.")]
    public bool reproducirAlInicio = false;

    [Header("Audio Espacial (3D)")]
    [Tooltip("Activa audio espacial para que el sonido se atenúe con la distancia.")]
    public bool usar3D = false;

    [Tooltip("Distancia máxima en unidades de Unity a la que el audio deja de escucharse. Ejemplo: 20 = fuera de 20 unidades ya no se oye.")]
    [Min(0.1f)]
    public float distanciaMaxima = 20f;

    [Tooltip("Intensidad de cercanía para audio 3D. 1 = caída media. >1 cae mucho más rápido (ej: 2.5 para voz muy local). <1 cae más suave (ej: 0.5 para ambiente). Ejemplo con distanciaMaxima=20: a 10 unidades, 0.5 se oye alto, 1 medio, 2.5 bajo. En Play Mode se actualiza en tiempo real.")]
    [Range(0.25f, 4f)]
    public float intensidadCercania = 1f;

    [Tooltip("Porcentaje final del radio máximo que se usa para el fade out. 0.25 = los últimos 25% del radio se desvanecen suavemente.")]
    [Range(0.05f, 0.5f)]
    public float fadeOutZona = 0.25f;

    [Header("Punto de Emisión (Opcional)")]
    [Tooltip("Empty/Transform opcional para usar como origen de este audio. Si está vacío, se usa el origen que defina el manager o la secuencia.")]
    public Transform puntoEmisionPersonalizado;
    
    [HideInInspector]
    public bool yaReproducido = false;
}
