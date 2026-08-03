using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SopaLetrasManager : MonoBehaviour
{
    [Header("Grid")]
    public GameObject celdaPrefab;
    public GridLayoutGroup gridLayout;

    [Header("Nivel 1 - Grilla reducida (12 columnas x 6 filas)")]
    public GameObject gridDimensiones;
    public float anchoGrid1;
    public float alturaGrid1;
    public float anchoGrid2;
    public float alturaGrid2;

    public GameObject contornoDimensiones;
    public float anchoContorno1;
    public float alturaContorno1;
    public float anchoContorno2;
    public float alturaContorno2;
    [Tooltip("Palabras a encontrar en Nivel 1. Deben caber dentro de la grilla de Nivel 1.")]
    public string[] palabrasNivel1 = {
        "COSMOS","POLVO","ESPIRAL","GALAXIAS"
    };
    [Tooltip("Cada string es una fila. Deben ser 6 filas de máximo 12 caracteres cada una.")]
    private readonly string[] gridRawNivel1 = {
        "COSMOSRVREES",
        "UNIVERLLORLS",
        "NIVERSOV P H",
        "IXCRGALAXIAS",
        "VENDDOA  LI ",
        "EEXBPESPIRAL"
    };

    [Header("Nivel 2 - Grilla completa (configuración actual)")]
    [Tooltip("Palabras a encontrar en Nivel 2.")]
    public string[] palabrasNivel2 = {
        "COSMOS","UNIVERSO","GALAXIAS","ESTRELLAS",
        "POLVO","ESPIRAL","OVALADA","ELIPTICA"
    };
    private readonly string[] gridRawNivel2 = {
        "COSMOSRVREESVFSDRTY",
        "UNIVERLLORLS VADTXHE",
        "NIVERSOV P HTEPZSIX",
        "IXCRGALAXIASCRLTCBU",
        "VENDDOA  LI UAEUCOH",
        "EEXBPFSPOJUO OFLXEOA",
        "RRKEACTFCTRES PILKVL",
        "SRESPIRALESYCCHBDAR",
        "OOVALADAEDELIPTICAS"
    };

    [Header("Colores")]
    public Color colorNormal = Color.white;
    public Color colorSeleccion = new Color(0.8f, 0.9f, 1f);
    public Color colorEncontrada = new Color(0.78f, 0.85f, 0.27f);

    [Header("UI")]
    public Transform panelPalabras;
    public GameObject palabraTagPrefab;
    public TextMeshProUGUI textoFeedback;
    public Sprite[] fondosTag;
    public Color colorFondoTag = Color.white;
    public Vector2 tamanoTag = new Vector2(120, 30);

    [Header("Fin de juego")]
    public float tiempoAntesDeFinalizar = 1.2f;

    // Datos activos de la partida actual (dependen del nivel elegido)
    private string[] palabrasActuales;
    private string[] gridRawActual;

    private int ROWS;
    private int COLS;
    private char[,] grid;
    private SopaLetrasCell[,] celdas;
    private Dictionary<string, List<Vector2Int>> ubicaciones = new();
    private Dictionary<string, TextMeshProUGUI> tags = new();
    private HashSet<string> halladas = new();

    private SopaLetrasCell celdaInicio;
    private SopaLetrasCell celdaActual;
    private bool arrastrando = false;
    private List<SopaLetrasCell> seleccionActual = new();

    // Referencias a lo instanciado, para poder destruirlo al reconstruir
    private List<GameObject> celdasInstanciadas = new();
    private List<GameObject> tagsInstanciados = new();

    private bool jugando = false;

    void Update()
    {
        if (jugando && arrastrando && Input.GetMouseButtonUp(0))
        {
            arrastrando = false;
            Verificar();
        }
    }

    // Llamado explícitamente por JuegosManager al iniciar o reintentar el nivel.
    public void ReiniciarJuego(int nivel)
    {
        StopAllCoroutines();

        // Elegir configuración según nivel
        if (nivel == 1)
        {
            palabrasActuales = palabrasNivel1;
            gridRawActual = gridRawNivel1;
            gridDimensiones.GetComponent<RectTransform>().sizeDelta = new Vector2(anchoGrid1, alturaGrid1);
            contornoDimensiones.GetComponent<RectTransform>().sizeDelta = new Vector2(anchoContorno1, alturaContorno1);
        }
        else
        {
            palabrasActuales = palabrasNivel2;
            gridRawActual = gridRawNivel2;
            gridDimensiones.GetComponent<RectTransform>().sizeDelta = new Vector2(anchoGrid2, alturaGrid2);
            contornoDimensiones.GetComponent<RectTransform>().sizeDelta = new Vector2(anchoContorno2, alturaContorno2);
        }

        LimpiarInstancias();

        arrastrando = false;
        celdaInicio = null;
        celdaActual = null;
        seleccionActual.Clear();
        halladas.Clear();
        ubicaciones.Clear();
        tags.Clear();

        if (textoFeedback)
            textoFeedback.text = "";

        ConstruirGrid();
        BuscarPalabras();
        GenerarCeldas();
        GenerarTags();

        jugando = true;
    }

    // Destruye celdas y tags de la partida/nivel anterior antes de reconstruir
    void LimpiarInstancias()
    {
        foreach (var go in celdasInstanciadas)
            if (go != null) Destroy(go);
        celdasInstanciadas.Clear();

        foreach (var go in tagsInstanciados)
            if (go != null) Destroy(go);
        tagsInstanciados.Clear();
    }

    void ConstruirGrid()
    {
        ROWS = gridRawActual.Length;
        COLS = 0;
        foreach (var row in gridRawActual)
            if (row.Length > COLS) COLS = row.Length;

        grid = new char[ROWS, COLS];
        string letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        for (int r = 0; r < ROWS; r++)
            for (int c = 0; c < COLS; c++)
            {
                char ch = c < gridRawActual[r].Length ? gridRawActual[r][c] : ' ';
                grid[r, c] = (ch == ' ') ? letras[Random.Range(0, letras.Length)] : ch;
            }
    }

    void BuscarPalabras()
    {
        int[] drs = { 0, 1, 0, -1, 1, 1, -1, -1 };
        int[] dcs = { 1, 0, -1, 0, 1, -1, 1, -1 };

        foreach (string pal in palabrasActuales)
        {
            for (int r = 0; r < ROWS && !ubicaciones.ContainsKey(pal); r++)
                for (int c = 0; c < COLS && !ubicaciones.ContainsKey(pal); c++)
                    for (int d = 0; d < 8; d++)
                    {
                        bool ok = true;
                        var celdsList = new List<Vector2Int>();
                        for (int i = 0; i < pal.Length; i++)
                        {
                            int nr = r + drs[d] * i;
                            int nc = c + dcs[d] * i;
                            if (nr < 0 || nr >= ROWS || nc < 0 || nc >= COLS || grid[nr, nc] != pal[i])
                            { ok = false; break; }
                            celdsList.Add(new Vector2Int(nr, nc));
                        }
                        if (ok) ubicaciones[pal] = celdsList;
                    }

            if (!ubicaciones.ContainsKey(pal))
                Debug.LogWarning($"[SopaLetras] La palabra \"{pal}\" no entró en la grilla actual. Revisá gridRawNivel1/2.");
        }
    }

    void GenerarCeldas()
    {
        gridLayout.constraintCount = COLS;
        celdas = new SopaLetrasCell[ROWS, COLS];

        for (int r = 0; r < ROWS; r++)
            for (int c = 0; c < COLS; c++)
            {
                GameObject go = Instantiate(celdaPrefab, gridLayout.transform);
                SopaLetrasCell cell = go.GetComponent<SopaLetrasCell>();
                cell.Init(r, c, grid[r, c], this);
                celdas[r, c] = cell;
                go.GetComponent<Image>().color = colorNormal;

                celdasInstanciadas.Add(go);
            }
    }

    void GenerarTags()
    {
        for (int i = 0; i < palabrasActuales.Length; i++)
        {
            string pal = palabrasActuales[i];
            GameObject go = Instantiate(palabraTagPrefab, panelPalabras);
            go.GetComponent<RectTransform>().sizeDelta = tamanoTag;

            if (fondosTag != null && fondosTag.Length > 0)
            {
                Image imgFondo = go.GetComponent<Image>();
                if (imgFondo != null)
                {
                    imgFondo.sprite = fondosTag[i % fondosTag.Length];
                    imgFondo.color = colorFondoTag;
                }
            }

            TextMeshProUGUI tmp = go.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = pal;
            tags[pal] = tmp;

            tagsInstanciados.Add(go);
        }
    }

    // ── Selección ─────────────────────────────────────────────

    public void IniciarSeleccion(SopaLetrasCell celda)
    {
        if (!jugando) return;
        arrastrando = true;
        celdaInicio = celda;
        celdaActual = celda;
        ActualizarVisualSeleccion();
    }

    public void ActualizarSeleccion(SopaLetrasCell celda)
    {
        if (!arrastrando) return;
        celdaActual = celda;
        ActualizarVisualSeleccion();
    }

    List<Vector2Int> ObtenerLinea(Vector2Int desde, Vector2Int hasta)
    {
        var resultado = new List<Vector2Int>();

        int diffR = hasta.x - desde.x;
        int diffC = hasta.y - desde.y;

        int dr = diffR == 0 ? 0 : (diffR > 0 ? 1 : -1);
        int dc = diffC == 0 ? 0 : (diffC > 0 ? 1 : -1);

        if (dr == 0 && dc == 0)
        {
            resultado.Add(desde);
            return resultado;
        }

        int distR = Mathf.Abs(diffR);
        int distC = Mathf.Abs(diffC);

        if (dr != 0 && dc != 0 && distR != distC)
            return resultado;

        int r = desde.x, c = desde.y;
        while (true)
        {
            resultado.Add(new Vector2Int(r, c));
            if (r == hasta.x && c == hasta.y) break;
            r += dr;
            c += dc;
        }

        return resultado;
    }

    void ActualizarVisualSeleccion()
    {
        foreach (var cel in seleccionActual)
            if (!EstaEncontrada(cel))
                cel.GetComponent<Image>().color = colorNormal;

        seleccionActual.Clear();

        var linea = ObtenerLinea(
            new Vector2Int(celdaInicio.fila, celdaInicio.columna),
            new Vector2Int(celdaActual.fila, celdaActual.columna)
        );

        foreach (var pos in linea)
        {
            var cel = celdas[pos.x, pos.y];
            seleccionActual.Add(cel);
            if (!EstaEncontrada(cel))
                cel.GetComponent<Image>().color = colorSeleccion;
        }
    }

    void Verificar()
    {
        var linea = ObtenerLinea(
            new Vector2Int(celdaInicio.fila, celdaInicio.columna),
            new Vector2Int(celdaActual.fila, celdaActual.columna)
        );

        foreach (var pal in palabrasActuales)
        {
            if (halladas.Contains(pal)) continue;
            if (!ubicaciones.ContainsKey(pal)) continue;

            var ubs = ubicaciones[pal];
            if (ubs.Count != linea.Count) continue;

            bool match = true;
            bool matchRev = true;
            for (int i = 0; i < linea.Count; i++)
            {
                if (linea[i] != ubs[i]) match = false;
                if (linea[i] != ubs[ubs.Count - 1 - i]) matchRev = false;
            }

            if (match || matchRev)
            {
                halladas.Add(pal);
                foreach (var pos in ubs)
                    celdas[pos.x, pos.y].GetComponent<Image>().color = colorEncontrada;

                if (tags.ContainsKey(pal))
                    tags[pal].fontStyle = FontStyles.Strikethrough;

                if (textoFeedback)
                    textoFeedback.text = $"¡Encontraste \"{pal}\"!";

                if (halladas.Count == palabrasActuales.Length)
                {
                    jugando = false;

                    if (textoFeedback)
                        textoFeedback.text = "¡Encontraste todas las palabras!";

                    StartCoroutine(FinalizarJuego());
                }

                seleccionActual.Clear();
                return;
            }
        }

        foreach (var pos in linea)
            if (!EstaEncontrada(celdas[pos.x, pos.y]))
                celdas[pos.x, pos.y].GetComponent<Image>().color = colorNormal;

        seleccionActual.Clear();
    }

    IEnumerator FinalizarJuego()
    {
        yield return new WaitForSeconds(tiempoAntesDeFinalizar);

        int puntaje = 100; // criterio fijo, ver mensaje anterior sobre cronómetro descartado

        if (JuegosManager.Instance != null)
            JuegosManager.Instance.MostrarPuntaje(puntaje);
    }

    bool EstaEncontrada(SopaLetrasCell cel)
    {
        return cel.GetComponent<Image>().color == colorEncontrada;
    }
}