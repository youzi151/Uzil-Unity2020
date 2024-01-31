using Uzil;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TestJson : MonoBehaviour
{

    public string json = "";
    public bool isGo = false;
    protected Stopwatch watch = new Stopwatch();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isGo == false) return;
        this.isGo = false;

        this.watch.Reset();

        this.watch.Start();
        
        DictSO data = DictSO.Json(this.json);

        this.watch.Stop();

        UnityEngine.Debug.Log(this.watch.Elapsed);
    }
}
