using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Detecta qué continente se tocó sobre una esfera (globo), usando una textura
/// "máscara" donde cada continente está pintado con un color sólido distinto
/// (en vez de la textura visual real del mapa).
///
/// Requiere que el GameObject tenga un MeshCollider (no SphereCollider),
/// porque solo MeshCollider devuelve coordenadas UV en el RaycastHit.
/// </summary>
[RequireComponent(typeof(MeshCollider))]
public class GlobeContinentPicker : MonoBehaviour
{
    [System.Serializable]
    public class Continente
    {
        public string id;
        public Color colorEnMascara = Color.white;
        [Tooltip("Opcional: panel a activar directamente al tocar este continente.")]
        public GameObject panel;
    }

    [Header("Textura máscara (Read/Write Enabled, Compression: None, Filter: Point)")]
    [SerializeField] private Texture2D texturaMascara;

    [Header("Continentes")]
    [SerializeField] private List<Continente> continentes;
    [SerializeField] private float toleranciaColor = 0.05f;

    [Header("Detección de tap (vs. arrastre para rotar)")]
    [Tooltip("Si el dedo/mouse se movió más que esto (en píxeles) entre el inicio y el final del toque, se considera arrastre/rotación y NO se abre ningún panel.")]
    [SerializeField] private float umbralArrastre = 20f;

    [Header("Diagnóstico en pantalla (opcional, para ver en el build real)")]
    [Tooltip("Si lo asignás, muestra ahí toda la info de diagnóstico, visible directamente en el dispositivo.")]
    //[SerializeField] private TextMeshProUGUI textoDiagnostico;

    private Camera camaraPrincipal;
    private MeshCollider meshCollider;
    private Vector2 posicionInicioToque;
    private bool siguiendoToque = false;

    void Awake()
    {
        camaraPrincipal = Camera.main;
        meshCollider = GetComponent<MeshCollider>();

        if (camaraPrincipal == null)
            Debug.LogError("[GlobeContinentPicker] No se encontró una cámara con el tag 'MainCamera'. Asignale ese tag a tu cámara.");


        Debug.Log($"[Chequeo UV] La malla del collider tiene UVs: {meshCollider.sharedMesh.uv.Length > 0} (cantidad: {meshCollider.sharedMesh.uv.Length}, vertices: {meshCollider.sharedMesh.vertexCount})");
    }

    void Update()
    {
        Vector2? posicionActual = null;
        bool comenzo = false;
        bool termino = false;

        if (Input.GetMouseButtonDown(0)) { posicionActual = Input.mousePosition; comenzo = true; }
        else if (Input.GetMouseButtonUp(0)) { posicionActual = Input.mousePosition; termino = true; }
        else if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            posicionActual = t.position;
            comenzo = t.phase == TouchPhase.Began;
            termino = t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled;
        }

        if (posicionActual == null) return;

        if (comenzo)
        {
            posicionInicioToque = posicionActual.Value;
            siguiendoToque = true;
            return;
        }

        if (termino && siguiendoToque)
        {
            siguiendoToque = false;

            float distancia = Vector2.Distance(posicionActual.Value, posicionInicioToque);
            if (distancia > umbralArrastre) return; // fue un gesto de rotación, no un tap

            ProcesarTap(posicionActual.Value);
        }
    }

    void ProcesarTap(Vector2 posicionPantalla)
    {
        if (camaraPrincipal == null) return;

        Ray ray = camaraPrincipal.ScreenPointToRay(posicionPantalla);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            bool esMeshColliderEsperado = hit.collider == meshCollider;
            Vector2 uv = hit.textureCoord;
            Color colorTocado = MuestrearColorExacto(uv);
            string colorHex = ColorUtility.ToHtmlStringRGB(colorTocado);

            string diagnostico =
                $"Collider: {hit.collider.name} (esperado: {esMeshColliderEsperado})\n" +
                $"UV: {uv}\n" +
                $"Textura: {texturaMascara.width}x{texturaMascara.height} | isReadable: {texturaMascara.isReadable} | formato: {texturaMascara.format}\n" +
                $"Color leído: #{colorHex}  RGBA: {colorTocado}";

            Debug.Log($"[GlobeContinentPicker] {diagnostico}");
            //if (textoDiagnostico != null) textoDiagnostico.text = diagnostico;

            if (!esMeshColliderEsperado) return;

            Continente continente = EncontrarContinentePorColor(colorTocado);
            if (continente != null)
            {
                Debug.Log($"[GlobeContinentPicker] Tocaste: {continente.id}");
                if (continente.panel != null)
                    continente.panel.SetActive(true);
            }
        }
        
    }

    Color MuestrearColorExacto(Vector2 uv)
    {
        int x = Mathf.Clamp(Mathf.FloorToInt(uv.x * texturaMascara.width), 0, texturaMascara.width - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(uv.y * texturaMascara.height), 0, texturaMascara.height - 1);
        return texturaMascara.GetPixel(x, y);
    }

    Continente EncontrarContinentePorColor(Color color)
    {
        foreach (var c in continentes)
        {
            if (ColoresSimilares(color, c.colorEnMascara))
                return c;
        }
        return null;
    }

    bool ColoresSimilares(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < toleranciaColor &&
               Mathf.Abs(a.g - b.g) < toleranciaColor &&
               Mathf.Abs(a.b - b.b) < toleranciaColor;
    }
}