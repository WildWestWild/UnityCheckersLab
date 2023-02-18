using DefaultNamespace;
using JetBrains.Annotations;
using UnityEngine;

namespace Checkers
{
    public interface IBaseClickComponent
    {
        /// <summary>
        /// Возвращает или устанавливает пару игровому объекту
        /// </summary>
        /// <remarks>У клеток пара - фишка, у фишек - клетка</remarks>
        [CanBeNull]
        BaseClickComponent Pair { get; }
        
        /// <summary>
        /// Выбран ли данный объект, для передвижения на его место, либо для уничтожения
        /// </summary>
        bool IsMovePicked { get; set; }

        void SetMaterial([CanBeNull] Material material = null);
        ColorType GetColor { get; }
        Coordinates GetCoordinate();
    }
}