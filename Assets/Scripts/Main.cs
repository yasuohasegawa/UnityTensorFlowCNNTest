using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TensorFlow;

public class Main : MonoBehaviour {
    public TextAsset model;

    // Use this for initialization
    void Start () {
        TFGraph graph = new TFGraph();
        graph.Import(model.bytes);
        TFSession sess = new TFSession(graph);
        float[] x = new float[] { 0f, 1f, 2f, 3f, 4f, 5f };
        float[] y = sess.GetRunner()
            .AddInput(graph["x"][0], x)
            .Fetch(graph["y"][0])
            .Run()[0]
            .GetValue() as float[];
        foreach (float value in y) print(value);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
