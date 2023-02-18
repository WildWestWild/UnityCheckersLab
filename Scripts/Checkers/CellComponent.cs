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

        public override void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"X = {Coordinates.X}, Y = {Coordinates.Y}");
            CallBackEvent(this, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent(this, false);
        }
    }
}