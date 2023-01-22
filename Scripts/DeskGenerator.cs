using Checkers;
using UnityEngine;

public class DeskGenerator: MonoBehaviour
{
    [SerializeField] private int _rows;
    [SerializeField] private int _cols;

    [SerializeField] private CellComponent _cellPrefab;
    [SerializeField] private ChipComponent _chipPrefab;

    private void Awake()
    {
        var cells = new CellComponent[_rows, _cols];
        var previousColor = ColorType.White;

        for (int i = 0; i < _rows; i++)
        {
            var previousPosition = 0f;
            previousColor = (previousColor == ColorType.White) ? ColorType.Black : ColorType.White;

            for (int j = 0; j < _cols; j++)
            {
                var cell = Instantiate(_cellPrefab, new Vector3(i, 0, previousPosition), Quaternion.identity, transform);

                previousPosition += cell.transform.localScale.x;

                var material = previousColor == ColorType.Black ? cell.BlackMaterial : cell.WhiteMaterial;
                
                cell.SetMaterial(material);

                cells[i, j] = cell;
                
                Debug.Log($"Position: Row = {i}, Cell = {j}. Color = {material.name}");
            }
        }
    }
}