using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Reelset : MonoBehaviour
{
    public bool isSpining = false;
    public List<Reel> reels;

    public static Reelset Instance;
    float delayBetweenReelStart = 0.2f;
    float delayBetweenReelStop = 0.2f;

    List<int> stops = new List<int>();

    void Start()
    {
        Instance = this;
    }

    public void StartSpin()
    {
        if (isSpining) return;
        isSpining = true;

        stops.Clear();
        BroadcastMessage("Idle");   //Symbols move to Idle state. Hide win highlights
        StartCoroutine(StartSpinCoroutine());
    }

    IEnumerator StartSpinCoroutine()
    {
        foreach (var reel in reels)
        {
            reel.StartSpin();
            yield return new WaitForSeconds(delayBetweenReelStart);
        }
    }

    public void GenerateStops()
    {
        Random rand = new Random();

        foreach (var reel in reels)
        {
            stops.Add(rand.Next(reel.reelStrip.Count));
        }
    }

    public void SetStops()
    {
        StartCoroutine(SetStopsCoroutine());
    }

    IEnumerator SetStopsCoroutine()
    {
        for (int i = 0; i < reels.Count; i++)
        {
            reels[i].SetStopIndex(stops[i]);
            yield return new WaitForSeconds(delayBetweenReelStop);
        }
    }


    public void SpinDone()
    {
        StartCoroutine(ProcessSpinDone());
    }

    IEnumerator ProcessSpinDone()
    {
        MainGameMB.Instance.SpinComplete();

        yield return new WaitForSeconds(0.3f); // Delay to show wins

        isSpining = false;
    }
}
