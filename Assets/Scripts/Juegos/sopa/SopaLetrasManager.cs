// ── SopaLetrasManager.cs ──────────────────────────────────────
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SopaLetrasManager : MonoBehaviour
{
    [Header("Grid")]
    public GameObject celdaPrefab;
    public GridLayoutGroup gridLayout;

    [Header("Palabras a encontrar")]
    public string[] palabras = {
        "COSMOS","UNIVERSO","GALAXIAS","ESTRELLAS",
        "POLVO","ESPIRAL","OVALADA","ELIPTICA"
    };

    [Header("Colores")]
    public Color colorNormal = Color.white;
    public Color colorSeleccion = new Color(0.8f, 0.9f, 1f);
    public Color colorEncontrada = new Color(0.78f, 0.85f, 0.27f);

    [Header("UI")]
    public Transform panelPalabras;
    public GameObject palabraTagPrefab;
    public TextMeshProUGUI textoFeedback;

    // Cuadrícula raw
    private readonly string[] GRID_RAW = {
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

    void Start()
    {
        ConstruirGrid();
        BuscarPalabras();
        GenerarCeldas();
        GenerarTags();
    }

    void Update()
    {
        if (arrastrando && Input.GetMouseButtonUp(0))
        {
            arrastrando = false;
            Verificar();
        }
    }

    void ConstruirGrid()
    {
        ROWS = GRID_RAW.Length;
        COLS = 0;
        foreach (var row in GRID_RAW)
            if (row.Length > COLS) COLS = row.Length;

        grid = new char[ROWS, COLS];
        string letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        for (int r = 0; r < ROWS; r++)
            for (int c = 0; c < COLS; c++)
            {
                char ch = c < GRID_RAW[r].Length ? GRID_RAW[r][c] : ' ';
                grid[r, c] = (ch == ' ') ? letras[Random.Range(0, letras.Length)] : ch;
            }
    }

    void BuscarPalabras()
    {
        int[] drs = { 0, 1, 0, -1, 1, 1, -1, -1 };
        int[] dcs = { 1, 0, -1, 0, 1, -1, 1, -1 };

        foreach (string pal in palabras)
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
            }
    }

    void GenerarTags()
    {
        foreach (string pal in palabras)
        {
            GameObject go = Instantiate(palabraTagPrefab, panelPalabras);
            TextMeshProUGUI tmp = go.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = pal;
            tags[pal] = tmp;
        }
    }

    // ── Selección ─────────────────────────────────────────────

    public void IniciarSeleccion(SopaLetrasCell celda)
    {
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
        // Limpiar selección anterior
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

        foreach (var pal in palabras)
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

                if (halladas.Count == palabras.Length && textoFeedback)
                    textoFeedback.text = "¡Encontraste todas las palabras!";

                seleccionActual.Clear();
                return;
            }
        }

        // No encontrada — limpiar selección
        foreach (var pos in linea)
            if (!EstaEncontrada(celdas[pos.x, pos.y]))
                celdas[pos.x, pos.y].GetComponent<Image>().color = colorNormal;

        seleccionActual.Clear();
    }

    bool EstaEncontrada(SopaLetrasCell cel)
    {
        return cel.GetComponent<Image>().color == colorEncontrada;
    }
}