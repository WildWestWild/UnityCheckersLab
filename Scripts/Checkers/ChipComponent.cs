using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        [field: SerializeField]
        public Material SelectMaterial { get; set; }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            SetMaterial(SelectMaterial);
            CallBackEvent((CellComponent) Pair, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            SetMaterial();
            CallBackEvent((CellComponent) Pair, false);
        }
    }
}