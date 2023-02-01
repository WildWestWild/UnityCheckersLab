using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace Checkers
{
    public class ClickHandler : MonoBehaviour
    {
        private CellComponent[,] _cells;
        private Dictionary<string, BaseClickComponent> _coordinateDictionary;
        private static ChipComponent _savePickedChip;
        private static CellComponent _savePickedCell;
        private static ChipComponent _saveDestroyChip;
        private GameManager _gameManager;

        public void Start()
        {
            StartCoroutine(MoveChip());
        }

        /// <summary>
        /// Иницализируем привязку событий к шашкам
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="coordinateDictionary"></param>
        /// <param name="gameManager"></param>
        public void Init(CellComponent[,] cells, Dictionary<string, BaseClickComponent> coordinateDictionary, GameManager gameManager)
        {
            _cells = cells;
            _coordinateDictionary = coordinateDictionary;
            _gameManager = gameManager;

            foreach (var cell in cells)
            {
                cell.OnClickEventHandler += OnCheckClicked;

                if (cell.Pair is null)
                {
                    continue;
                }

                var chip = cell.Pair;
                if (chip.GetColor == ColorType.White)
                    chip.OnClickEventHandler += OnWhiteChipClicked;
                else
                    chip.OnClickEventHandler += OnBlackChipClicked;
            }
        }

        private void OnCheckClicked(BaseClickComponent component)
        {
            if ((component is CellComponent && component.IsMovePicked) ||
                (component.Pair is CellComponent && component.Pair.IsMovePicked))
            {
                _savePickedCell = component as CellComponent;
            }

            AllResetDefaultMaterial();
        }

        private void OnWhiteChipClicked(BaseClickComponent component)
        {
            AllResetDefaultMaterial();
            if (!_gameManager.IsStopGame && _gameManager.IsWhiteMove)
            {
                var cell = component.Pair;
                var coordinateCellUnderChip = cell!.GetCoordinate();
                var chip = component as ChipComponent;
                chip!.IsMovePicked = true;
                _savePickedChip = chip;
                CellHighlight(new Coordinates(coordinateCellUnderChip.X + 1, coordinateCellUnderChip.Y + 1), new Coordinates(coordinateCellUnderChip.X + 2, coordinateCellUnderChip.Y + 2), chip);
                CellHighlight(new Coordinates(coordinateCellUnderChip.X - 1, coordinateCellUnderChip.Y + 1), new Coordinates(coordinateCellUnderChip.X - 2, coordinateCellUnderChip.Y + 2), chip);
            }
        }

        private void OnBlackChipClicked(BaseClickComponent component)
        {
            AllResetDefaultMaterial();
            if (!_gameManager.IsStopGame && !_gameManager.IsWhiteMove)
            {
                var cell = component.Pair;
                var coordinateCellUnderChip = cell!.GetCoordinate();
                var chip = component as ChipComponent;
                chip!.IsMovePicked = true;
                _savePickedChip = chip;
                CellHighlight(new Coordinates(coordinateCellUnderChip.X + 1, coordinateCellUnderChip.Y - 1), new Coordinates(coordinateCellUnderChip.X + 2, coordinateCellUnderChip.Y - 2), chip);
                CellHighlight(new Coordinates(coordinateCellUnderChip.X - 1, coordinateCellUnderChip.Y - 1), new Coordinates(coordinateCellUnderChip.X - 2, coordinateCellUnderChip.Y - 2), chip);
            }
        }

        private void CellHighlight(Coordinates coordinateCell, Coordinates coordinateDestroy, ChipComponent chipComponent)
        {
            var selectMaterial = chipComponent.SelectMaterial;
            string keyCell = coordinateCell.GetCoordinateKey();
            if (_coordinateDictionary.ContainsKey(keyCell))
            {
                var findCell = _coordinateDictionary[keyCell];
                if (findCell.Pair is null && !(selectMaterial is null))
                {
                    findCell.SetMaterial(selectMaterial);
                    findCell.IsMovePicked = true;
                    return;
                }

                if (!(findCell.Pair is null) && chipComponent.GetColor != findCell.Pair.GetColor && !(selectMaterial is null))
                {
                    string keyChipDestroy = coordinateDestroy.GetCoordinateKey();
                    if (_coordinateDictionary.ContainsKey(keyChipDestroy))
                    {
                        var findCellMoveToDestroyChip = _coordinateDictionary[keyChipDestroy];
                        if (findCellMoveToDestroyChip.Pair is null)
                        {
                            findCellMoveToDestroyChip.SetMaterial(selectMaterial);
                            findCellMoveToDestroyChip.IsMovePicked = true;
                            _saveDestroyChip = findCell.Pair as ChipComponent;
                            Debug.Log($"KeyChipDestroy = [ X = {coordinateDestroy.X}, Y = {coordinateDestroy.Y}]; CoordinateMoveCell = [ X = {coordinateCell.X}, Y = {coordinateCell.Y} ]");
                        }
                    }
                }
            }
        }

        private void AllResetDefaultMaterial()
        {
            foreach (var cell in _cells)
            {
                if (cell.IsMovePicked)
                {
                    cell.SetMaterial();
                    cell.IsMovePicked = false;
                }

                if (!(cell.Pair is null) && cell.Pair.IsMovePicked)
                {
                    cell.SetMaterial();
                    cell.IsMovePicked = false;
                }
            }
        }

        /// <summary>
        /// Извершение  хода
        /// </summary>
        private void CompleteMove()
        {
            // Отвязываем Chip от старого Cell
            _savePickedChip.DestroyPair();
            // Привязываем Chip к новому Cell
            _savePickedChip.CreatePair(_savePickedCell);
            // Удаляем сохраненные ссылки на Chip и Cell
            _gameManager.CheckVictory(_savePickedChip);
            _gameManager.PassMove();
            _savePickedChip = null;
            _savePickedCell = null;
            _saveDestroyChip = null;
        }

        private IEnumerator MoveChip()
        {
            while (true)
            {
                if (_savePickedCell != null && _savePickedChip != null
                                            && Vector3.Distance(_savePickedChip.transform.position, _savePickedCell.transform.position) > 0f)
                {
                    LerpChip();
                }

                if (_savePickedCell != null && _savePickedChip != null
                                            && Vector3.Distance(_savePickedChip.transform.position - Vector3.up, _savePickedCell.transform.position) <= 0.1f)
                {
                    CompleteMove();
                }

                if (_savePickedChip != null && _saveDestroyChip != null
                                            && Vector3.Distance(_savePickedChip.transform.position, _saveDestroyChip.transform.position) <= 10f)
                {
                    DestroyChip();
                }

                yield return null;
            }
        }

        private static void LerpChip()
        {
            var lerpPosition = Vector3.Lerp(_savePickedChip.transform.position, _savePickedCell.transform.position, 0.05f);
            _savePickedChip.transform.position = new Vector3(lerpPosition.x, _savePickedChip.transform.position.y, lerpPosition.z);
        }

        private static void DestroyChip()
        {
            _saveDestroyChip.DestroyPair();
            Destroy(_saveDestroyChip.gameObject);
            _saveDestroyChip = null;
        }

        public void OnDisable()
        {
            StopCoroutine(MoveChip());
        }
    }
}