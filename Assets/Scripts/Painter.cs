using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painter : MonoBehaviour {
    private Texture2D drawTexture;
    private Color[] buffer;

    private int texSize = 256;

    [SerializeField]
    private int paintSize = 5;

    void Start()
    {
        Texture2D mainTexture = (Texture2D)GetComponent<MeshRenderer>().material.mainTexture;
        Color[] pixels = mainTexture.GetPixels();

        buffer = new Color[pixels.Length];
        pixels.CopyTo(buffer, 0);

        drawTexture = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);
        drawTexture.filterMode = FilterMode.Point;
    }

    public void Draw(Vector2 p)
    {
        for (int x = 0; x < texSize; x++)
        {
            for (int y = 0; y < texSize; y++)
            {
                if ((p - new Vector2(x, y)).magnitude < paintSize)
                {
                    buffer.SetValue(Color.white, x + texSize * y);
                }
            }
        }
    }

    public void Clear()
    {
        for (int x = 0; x < texSize; x++)
        {
            for (int y = 0; y < texSize; y++)
            {
                buffer.SetValue(Color.black, x + texSize * y);
            }
        }
        Apply();
    }

    public Texture2D GetTexture()
    {
        return drawTexture;
    }

    private void Apply()
    {
        drawTexture.SetPixels(buffer);
        drawTexture.Apply();
        GetComponent<MeshRenderer>().material.mainTexture = drawTexture;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                Draw(hit.textureCoord * texSize);
            }

            Apply();
        }
    }
}
