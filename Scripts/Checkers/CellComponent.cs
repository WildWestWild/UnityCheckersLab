﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class CellComponent : BaseClickComponent
    {
        [field: SerializeField]
        public Material WhiteMaterial { get; set; }
        
        [field: SerializeField]
        public Material BlackMaterial { get; set; }

        private Dictionary<NeighborType, CellComponent> _neighbors;


        /// <summary>
        /// Возвращает соседа клетки по указанному направлению
        /// </summary>
        /// <param name="type">Перечисление направления</param>
        /// <returns>Клетка-сосед или null</returns>
        public CellComponent GetNeighbors(NeighborType type) => _neighbors[type];


        public override void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"X = {Coordinates.X}, Y = {Coordinates.Y}");
            CallBackEvent(this, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent(this, false);
        }

        /// <summary>
        /// Конфигурирование связей клеток
        /// </summary>
        public void Configuration(Dictionary<NeighborType, CellComponent> neighbors)
        {
            if (_neighbors != null) return;
            _neighbors = neighbors;
        }

    }

    /// <summary>
    /// Тип соседа клетки
    /// </summary>
    public enum NeighborType : byte
    {
        /// <summary>
        /// Клетка сверху и слева от данной
        /// </summary>
        TopLeft,
        /// <summary>
        /// Клетка сверху и справа от данной
        /// </summary>
        TopRight,
        /// <summary>
        /// Клетка снизу и слева от данной
        /// </summary>
        BottomLeft,
        /// <summary>
        /// Клетка снизу и справа от данной
        /// </summary>
        BottomRight
    }
}