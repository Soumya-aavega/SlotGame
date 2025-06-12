using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using DG.Tweening;
using Random = System.Random;
using System.Linq;

[Serializable]
public class Reel : MonoBehaviour
{

    [Range(25, 100)] [SerializeField] int reelStripCount = 50;
    public List<string> reelStrip;

    //pool
    int poolSize = 5;   // 3 + 1 top + 1 bitton buffer
    public Dictionary<string, List<GameObject>> pools = new Dictionary<string, List<GameObject>> ();

    [SerializeField] int currentReelIndex = 0;
    public int stopIndex = -1;

    [Header("Reel")]
    [SerializeField] Transform inactiveParent;
    [SerializeField] Transform activeParent;
    float symbolSpacing = 1.6f;
    
    public List<GameObject> VisibleSymbols = new List<GameObject> ();

    //Speed
    float DispalcementTime = 0.5f;
    float minDispalcementTime = 0.02f;
    float maxDispalcementTime = 0.5f;
    float stopDispalcementTime = 0.15f;

    public void GenerateRandonReelStrip()
    {
        var symbolNames = MainGameMB.Instance.symbolNames;
        Random rand = new Random();
        for (int i = 0; i < reelStripCount; i++) 
        {
            var randSymbolName = MainGameMB.Instance.symbolNames[rand.Next(symbolNames.Count)];
            reelStrip.Add(randSymbolName);
        }
    }

    void Start()
    {
        GenerateRandonReelStrip();
        CreatePoolObjects();
        ShowSymbolsInReel();
    }

    void CreatePoolObjects()
    {
        foreach(var symbolName in MainGameMB.Instance.symbolNames)
        {
            CreatePoolObject(symbolName);
        }
    }

    void CreatePoolObject(string symbolName)
    {
        var _symbol = MainGameMB.Instance.GetSymbolObject(symbolName);
        var pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i ++)
        {
            var obj = GameObject.Instantiate(_symbol, inactiveParent);
            obj.name = symbolName;
            obj.transform.position  = Vector3.one * 1000;
            obj.SetActive(false);
            pool.Add(obj);
        }
        pools.Add(symbolName, pool);
    }

    void ShowSymbolsInReel()
    {
        VisibleSymbols.Clear();

        for (int i = 0; i < 5; i++)
        {
            int j = currentReelIndex + i;
            if(j > reelStripCount - 1)
            {
                j = j - reelStripCount;
            }

            var symbolname = reelStrip[j];
            var symbol = pools[symbolname][0];
            pools[symbolname].RemoveAt(0);

            symbol.SetActive(true);
            symbol.transform.localPosition = new Vector3(0, i * symbolSpacing * -1f, 0);
            symbol.transform.parent = activeParent;

            VisibleSymbols.Add(symbol);
        }
    }

    public void StartSpin()
    {
        stopIndex = -1; //Spin till stopIndex is set

        ChangeReelSpeedTime(maxDispalcementTime, minDispalcementTime, 1f);  //Increasing speed
        
        Spin();
    }

    void Spin()
    {
        StartCoroutine(DoOneStepCoroutine());
    }

    void ChangeReelSpeedTime(float startValue, float endValue, float time)
    {
        DispalcementTime = startValue;
        DOTween.To(() => DispalcementTime, x => DispalcementTime = x, endValue, time);
    }

    IEnumerator DoOneStepCoroutine()
    {
        float counter = 0;
        float yDisplacement = 0;

        while (counter < DispalcementTime)
        {
            counter += Time.deltaTime;
            yDisplacement = Mathf.Lerp(0, symbolSpacing * -1f, counter / DispalcementTime);

            int symbolIndex = 0;
            foreach (Transform obj in activeParent.transform)
            {
                var pos = obj.transform.localPosition;
                obj.transform.localPosition = new Vector3(pos.x, (symbolIndex * symbolSpacing * -1) + yDisplacement, pos.z);

                symbolIndex++;
            }
            yield return null;
        }

        OneStepCompleted();
    }

    void OneStepCompleted()
    {
        DecrementCurrentReelIndex();
        UpdateReel();

        if(stopIndex != currentReelIndex)
        {
            Spin();
        }
        else  //Spin Stops
        {
            var reelset = MainGameMB.Instance.reelset;
            if (reelset.reels.Last() == this)
            {
                reelset.SpinDone();
            }

            //Bounce on Stop
            transform.DOMoveY(transform.position.y - 0.5f, 0.25f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                transform.DOMoveY(transform.position.y + 0.5f, 0.25f).SetEase(Ease.OutCubic);
            });
        }
    }

    void UpdateReel()
    {
        //Remove one from bottom
        var obj = VisibleSymbols.Last();
        VisibleSymbols.RemoveAt(4);
        var objName = obj.name;

        obj.SetActive(false);
        obj.transform.parent = inactiveParent;
        obj.transform.position = Vector3.one * 1000;
        pools[objName].Add(obj);


        //Add one to Top
        int j = currentReelIndex;
        if (j > reelStripCount - 1)
        {
            j = j - reelStripCount;
        }

        var symbolname = reelStrip[j];
        var symbol = pools[symbolname][0];
        pools[symbolname].RemoveAt(0);

        symbol.SetActive(true);
        symbol.transform.localPosition = new Vector3(0, 0, 0);
        symbol.transform.parent = activeParent;
        symbol.transform.SetAsFirstSibling();


        //Update VisibleSymbols
        VisibleSymbols.Insert(0, symbol);
    }

    void DecrementCurrentReelIndex()
    {
        currentReelIndex--;
        if (currentReelIndex < 0)
        {
            currentReelIndex = reelStripCount - 1;
        }
    }

    public void SetStopIndex(int value)
    {
        stopIndex = value;
        currentReelIndex = stopIndex - 1;   //Changing currentReelIndex to control reel stop order

        if(currentReelIndex < 0)
        {
            currentReelIndex = reelStrip.Count - 1;
        }

        ChangeReelSpeedTime(minDispalcementTime, stopDispalcementTime, 10f);  //Decrease speed
    }
}
