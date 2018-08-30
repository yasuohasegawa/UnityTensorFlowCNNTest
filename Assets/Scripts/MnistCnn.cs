using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TensorFlow;


public class MnistCnn : MonoBehaviour
{
    public UI ui;
    public Painter painter;
    public TextAsset model;
    public Texture2D targetTex;

    private TFGraph graph;
    private TFSession session;

    private static int img_width = 28;
    private static int img_height = 28;
    private float[,,,] inputImg = new float[1, img_width, img_height, 1];

    private Texture2D nTex;

    // Use this for initialization
    void Start()
    {
        graph = new TFGraph();
        graph.Import(model.bytes);
        session = new TFSession(graph);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ProcessImage()
    {
        targetTex = Resize(painter.GetTexture(), img_width, img_height);// resize 28 x 28 texture
        ui.rimg.texture = targetTex; // Make sure that the resize works ok.

        for (int i = 0; i < img_width; i++)
        {
            for (int j = 0; j < img_height; j++)
            {
                inputImg[0, img_width - i - 1, j, 0] = targetTex.GetPixel(j, i).r;
            }
        }

        // run CNN
        var runner = session.GetRunner();
        runner.AddInput(graph["conv2d_1_input"][0], inputImg).Fetch(graph["dense_2/Softmax"][0]);

        // Run the model
        float[,] recurrent_tensor = runner.Run()[0].GetValue() as float[,];

        // Find the answer the model is most confident in
        float highest_val = 0;
        int highest_ind = -1;
        float sum = 0;
        float currTime = Time.time;

        for (int j = 0; j < 10; j++)
        {
            float confidence = recurrent_tensor[0, j];
            if (highest_ind > -1)
            {
                if (recurrent_tensor[0, j] > highest_val)
                {
                    highest_val = confidence;
                    highest_ind = j;
                }
            }
            else
            {
                highest_val = confidence;
                highest_ind = j;
            }

            // sum should total 1 in the end
            sum += confidence;
        }

        // Display the answer to the screen
        Debug.Log("Answer: " + highest_ind + "\n Confidence: " + highest_val + "\nLatency: " + (Time.time - currTime) * 1000000 + " us");

        ui.resText.text = "Answer: " + highest_ind.ToString();
    }

    private Texture2D Resize(Texture2D source, int newWidth, int newHeight)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Point;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        nTex = new Texture2D(newWidth, newHeight);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newWidth), 0, 0);

        nTex = FlipImage(nTex);

        nTex.Apply();
        RenderTexture.active = null;
        return nTex;

    }

    private Texture2D FlipImage(Texture2D source)
    {
        var srcPixels = source.GetPixels();
        var outPixels = new Color[srcPixels.Length];

        var currentIndex = 0;

        var startX = source.width - 1;
        var startY = source.height - 1;

        for (var y = startY; y >= 0; y--)
        {
            for (var x = startX; x >= 0; x--)
            {
                outPixels[currentIndex++] = srcPixels[y * source.width + x];
            }
        }

        source.SetPixels(outPixels);

        return source;
    }
}
