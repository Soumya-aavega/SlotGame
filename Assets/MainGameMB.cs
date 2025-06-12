using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainGameMB : MonoBehaviour
{
    public static MainGameMB Instance;

    [Header("Easy Win")]
    //Only 2 symbols instead of 7
    //For win testing purpose
    [SerializeField] bool easyWin = false;
    [SerializeField] GameObject easyWinWarning;

    [Header("Reelset")]
    public Reelset reelset;

    [SerializeField] List<Symbol> symbols;
    List<string> _symbolNames = new List<string>();
    public List<string> symbolNames 
    {
        get { return _symbolNames; }
    }

    [Header("Win Meter")]
    [SerializeField]TextMeshPro winMeter;
    int totalWinAmount = 0;
    
    float spinStopDelay = 0.5f;

    private void Awake()
    {
        Instance = this;

        if(easyWin)
        {
            symbols.RemoveRange(0, 5);
            easyWinWarning.SetActive(true);
        }
        else
        {
            easyWinWarning.SetActive(false);
        }

        PopulateSymbolNAmes();
    }

    void PopulateSymbolNAmes()
    {
        foreach (Symbol symbol in symbols) 
        {
            _symbolNames.Add(symbol.name);
        }
    }

    public GameObject GetSymbolObject(string symbolName)
    {
        foreach (Symbol symbol in symbols)
        {
            if (symbol.name == symbolName)
            {
                return symbol.symbol;
            }
        }

        return null;
    }

    IEnumerator ProcessBet()
    {
        reelset.StartSpin();
        reelset.GenerateStops();

        yield return new WaitForSeconds(spinStopDelay);

        reelset.SetStops();
    }

    public void SpinComplete()
    {
        ProcessWinEvaluation();
    }



    //****************************************************************************************
    ///
    /// BUTTON CLICKS
    ///

    public void BetButtonPressed()
    {
        //Spin processes
        if (reelset.isSpining) return;

        LinesManager.Instance.Reset();
        totalWinAmount = 0;
        winMeter.text = totalWinAmount.ToString();

        StartCoroutine(ProcessBet());
    }

    public void ChangeSpeed(int _speed)
    {
        Time.timeScale = _speed;
    }



    //****************************************************************************************
    ///
    /// WIN LOGIC
    ///

    List<(int, int)> line1 = new List<(int, int)>() { (0,1), (1,1), (2,1), (3,1), (4,1) };
    List<(int, int)> line2 = new List<(int, int)>() { (0,1), (1,1), (2,2), (3,1), (4,1) };
    List<(int, int)> line3 = new List<(int, int)>() { (0,1), (1,2), (2,3), (3,2), (4,1) };
    List<(int, int)> line4 = new List<(int, int)>() { (0,2), (1,1), (2,2), (3,1), (4,2) };
    List<(int, int)> line5 = new List<(int, int)>() { (0,2), (1,2), (2,2), (3,2), (4,2) };
    List<(int, int)> line6 = new List<(int, int)>() { (0,2), (1,3), (2,3), (3,3), (4,2) };
    List<(int, int)> line7 = new List<(int, int)>() { (0,3), (1,2), (2,1), (3,2), (4,3) };
    List<(int, int)> line8 = new List<(int, int)>() { (0,3), (1,3), (2,2), (3,3), (4,3) };
    List<(int, int)> line9 = new List<(int, int)>() { (0,3), (1,3), (2,3), (3,3), (4,3) };

    List<List<(int, int)>> lines = new List<List<(int, int)>>() { };


    private void Start()
    {
        AddLines();
    }

    void AddLines()
    {
        lines.Add(line1);
        lines.Add(line2);
        lines.Add(line3);
        lines.Add(line4);
        lines.Add(line5);
        lines.Add(line6);
        lines.Add(line7);
        lines.Add(line8);
        lines.Add(line9);
    }

    void ProcessWinEvaluation()
    {
        for (int i = 0; i < lines.Count; i++) 
        {
            ProcessLineWin(lines[i], i+1 );
        }
    }

    void ProcessLineWin(List<(int, int)> line, int lineNum)
    {
        var symbolName = reelset.reels[line[0].Item1].VisibleSymbols[line[0].Item2].name;
        var symbolsList = new List<GameObject>();

        int winAmount = 50;

        for (int i = 0; i < line.Count; i++) 
        {
            var _symbol = reelset.reels[line[i].Item1].VisibleSymbols[line[i].Item2];
            symbolsList.Add(_symbol);
            if (_symbol.name != symbolName)
            {
                return; //Didn't make a line
            }
        }

        if(lineNum == 1 || lineNum == 5 || lineNum == 9)
        {
            winAmount = 100;
        }

        foreach (var symbol in symbolsList) 
        {
            symbol.GetComponent<SymbolMB>().Highlight();
        }

        LinesManager.Instance.HighlightNumber(lineNum);

        totalWinAmount += winAmount;
        winMeter.text = totalWinAmount.ToString();
    }
}
