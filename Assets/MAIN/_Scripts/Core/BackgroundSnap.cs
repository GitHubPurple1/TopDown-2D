using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundSnap : MonoBehaviour
{
    [SerializeField] private Transform target;          //  Main Camera
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private int gridCells = 40;        //  Tiles per side
    [SerializeField] private Color baseColor = new Color(0.12f, 0.12f, 0.14f);
    [SerializeField] private Color lineColor = new Color(0.22f, 0.22f, 0.26f);

    private const int TexSize = 64;
    private Texture2D texture;

    private void Awake()
    {
        Color[] pixels = new Color[TexSize * TexSize];
        for (int y = 0; y < TexSize; y++)
        {
            for (int x = 0; x < TexSize; x++)
            {
                bool isLine = x == 0 || y == 0;          // Left column and bottom row
                pixels[y * TexSize + x] = isLine ? lineColor : baseColor;
            }
        }

        texture = new Texture2D(TexSize, TexSize);
        texture.filterMode = FilterMode.Point;          // Crisp lines, no blur
        texture.SetPixels(pixels);
        texture.Apply();                                // Upload the pixels

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, TexSize, TexSize),
                                     new Vector2(0.5f, 0.5f), TexSize / tileSize, 0, SpriteMeshType.FullRect);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprite;                            // sprite first, then draw mode
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(gridCells * tileSize, gridCells * tileSize);
        sr.sortingOrder = -100;
    }

    private void LateUpdate()
    {
        if (target == null) return;
        float snappedX = Mathf.Round(target.position.x / tileSize) * tileSize;
        float snappedY = Mathf.Round(target.position.y / tileSize) * tileSize;
        transform.position = new Vector3(snappedX, snappedY, 0);

    }

    private void OnDestroy()
    {
        Destroy(texture);
    }
}