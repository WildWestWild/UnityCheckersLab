#nullable enable
using System.Collections;
using System.Collections.Generic;
using Checkers;
using DefaultNamespace;
using UnityEngine;

public class DeskGenerator: MonoBehaviour
{
    [SerializeField] private int _rows;
    [SerializeField] private int _cols;

    [SerializeField] private CellComponent _cellPrefab;
    [SerializeField] private ChipComponent _chipPrefab;
    [SerializeField] private ClickHandler _clickHandler;

    private CellComponent[,] Cells;
    private IObserver? _observer;
    private IGameManager _IGameManager;
    private Dictionary<string, IBaseClickComponent> _coordinatesDictionary = new Dictionary<string, IBaseClickComponent>();
    private int tunrPoints;

    public void InitiateTurnDesk() => tunrPoints = 180;

    private void Awake()
    {
        Cells = new CellComponent[_rows, _cols];
        _clickHandler = GetComponent<ClickHandler>();
        _observer = GetComponent<Observer>();
        var gameManager = GetComponent<GameManager>();
        _IGameManager = gameManager;

        var color = ColorType.White;

        for (int i = 0; i < _rows; i++)
        {
            var previousPosition = 0f;
            color = (color == ColorType.White) ? ColorType.Black : ColorType.White;

            for (int j = 0; j < _cols; j++)
            {
                var cell = Instantiate(_cellPrefab, new Vector3(i * 10, 0, previousPosition), Quaternion.identity, transform);

                previousPosition += cell.transform.localScale.x * 10;
                
                color = (color == ColorType.White) ? ColorType.Black : ColorType.White;
                
                var material = color == ColorType.Black ? cell.BlackMaterial : cell.WhiteMaterial;
                
                cell.SetMaterial(material);
                cell.SaveBaseMaterial(material);
                var coordinate = new Coordinates(j, i);
                cell.SetCoordinate(coordinate);
                _coordinatesDictionary.Add(coordinate.GetCoordinateKey(), cell);

                Cells[i, j] = cell;

                if (color == ColorType.Black && i < 3)
                {
                    CreateChip(cell, cell.WhiteMaterial, ColorType.White);
                }
                
                if (color == ColorType.Black && i > 4)  
                {
                    CreateChip(cell, cell.BlackMaterial, ColorType.Black);
                }

                Debug.Log($"Position: Row = {i}, Cell = {j}. Color = {material.name}");
            }
        }
        
        gameManager.Init(this, _rows);
        _clickHandler.Init(Cells, _coordinatesDictionary, gameManager, _observer);
        _observer.CheckValidMode(ref _observer);
        _observer?.TryActiveRecordMode(_IGameManager);
        _observer?.TryActiveReplayMode(_clickHandler, _coordinatesDictionary);
    }

    private void Start()
    {
        StartCoroutine(TurnDesk());
    }

    private void FixedUpdate()
    {
        _observer?.CheckReplayMethod();
    }

    private void CreateChip(CellComponent cell, Material material, ColorType colorType)
    {
        var transformCell = cell.transform.position;
        ChipComponent chip = Instantiate(_chipPrefab, new Vector3(transformCell.x,transformCell.y + 1 , transformCell.z), Quaternion.identity, transform);
        chip.SetMaterial(material);
        chip.SaveBaseMaterial(material);
        chip.Pair = cell;
        cell.Pair = chip;
        chip.SetColor(colorType);
    }
    
    private IEnumerator TurnDesk()
    {
        while (true)
        {
            if (tunrPoints > 0)
            {
                transform.Rotate(0, 1, 0);
                tunrPoints--;
            }
            yield return null;
        }
    }

    private void OnDisable()
    {
        StopCoroutine(TurnDesk());
    }
}