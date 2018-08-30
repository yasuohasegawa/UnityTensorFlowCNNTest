using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TensorFlow;


public class Mnist : MonoBehaviour {
    private const int INPUT_SIZE = 28;
    private const int IMAGE_MEAN = 117;
    private const float IMAGE_STD = 1;
    private const string INPUT_TENSOR = "input";
    private const string OUTPUT_TENSOR = "output";

    public TextAsset model;
    public Texture2D targetTex;

    private TFGraph graph;
    private TFSession session;

    private static int img_width = 28;
    private static int img_height = 28;
    //private float[,,,] inputImg = new float[1, img_width, img_height, 1];
    private float[,] inputImg = new float[1, img_width* img_height];

    // Use this for initialization
    void Start () {
        graph = new TFGraph();
        graph.Import(model.bytes);
        session = new TFSession(graph);
        ProcessImage();
    }

    // Update is called once per frame
    void Update () {
		
	}

    void ProcessImage()
    {
        int index = 0;
        for (int i = 0; i < img_width; i++)
        {
            for (int j = 0; j < img_height; j++)
            {
                inputImg[0, index] = targetTex.GetPixel(j, i).r;
                index++;
            }
        }

        var runner = session.GetRunner();
        runner.AddInput(graph[INPUT_TENSOR][0], inputImg).Fetch(graph[OUTPUT_TENSOR][0]);

        /*
        var output = runner.Run();
        //put results into one dimensional array
        float[] probs = ((float[][])output[0].GetValue(jagged: true))[0];

        for(int i = 0; i< probs.Length; i++)
        {
            Debug.Log(i+":"+probs[i].ToString("f3"));
        }
        */

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

    }
}
