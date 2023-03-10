using DefaultNamespace;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public abstract class BaseClickComponent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBaseClickComponent
    {
        //Меш игрового объекта
        private MeshRenderer _mesh;

        private Material _startMaterial;

        [Tooltip("Цветовая сторона игрового объекта"), SerializeField]
        private ColorType _color;
        
        protected Coordinates Coordinates;

        public void SetCoordinate(Coordinates coordinates) => Coordinates = coordinates;
        public Coordinates GetCoordinate() => Coordinates;
        /// <summary>
        /// Возвращает цветовую сторону игрового объекта
        /// </summary>
        public ColorType GetColor => _color;

        public void SetColor(ColorType colorType) => _color = colorType;

        /// <summary>
        /// Выбран ли данный объект, для передвижения на его место, либо для уничтожения
        /// </summary>
        public bool IsMovePicked { get; set; }

        /// <summary>
        /// Возвращает или устанавливает пару игровому объекту
        /// </summary>
        /// <remarks>У клеток пара - фишка, у фишек - клетка</remarks>
        [CanBeNull]
        public BaseClickComponent Pair { get; set; }

        public void SetMaterial([CanBeNull] Material material = null)
        {
            _mesh.sharedMaterial = material ? material : _startMaterial;
        }
        public void SaveBaseMaterial(Material material) => _startMaterial = material;

        /// <summary>
        /// Событие клика на игровом объекте
        /// </summary>
        public event ClickEventHandler OnClickEventHandler;

        /// <summary>
        /// Событие наведения и сброса наведения на объект
        /// </summary>
        public event FocusEventHandler OnFocusEventHandler;


        //При навадении на объект мышки, вызывается данный метод
        //При наведении на фишку, должна подсвечиваться клетка под ней
        //При наведении на клетку - подсвечиваться сама клетка
        public abstract void OnPointerEnter(PointerEventData eventData);

        //Аналогично методу OnPointerEnter(), но срабатывает когда мышка перестает
        //указывать на объект, соответственно нужно снимать подсветку с клетки
        public abstract void OnPointerExit(PointerEventData eventData);

        //При нажатии мышкой по объекту, вызывается данный метод
        public void OnPointerClick(PointerEventData eventData)
		{
            OnClickEventHandler?.Invoke(this);
        }

        //Этот метод можно вызвать в дочерних классах (если они есть) и тем самым пробросить вызов
        //события из дочернего класса в родительский
        protected void CallBackEvent(CellComponent target, bool isSelect)
        {
            OnFocusEventHandler?.Invoke(target, isSelect);
		}

		protected virtual void Awake()
        {
            _mesh = GetComponent<MeshRenderer>();
            //Этот список будет использоваться для набора материалов у меша,
            //в данном ДЗ достаточно массива из 3 элементов
            //1 элемент - родной материал меша, он не меняется
            //2 элемент - материал при наведении курсора на клетку/выборе фишки
            //3 элемент - материал клетки, на которую можно передвинуть фишку
            _startMaterial = _mesh.sharedMaterial;
        }

        /// <summary>
        /// Отвязка Pair
        /// </summary>
        public void DestroyPair()
        {
            if (Pair != null) Pair.Pair = null;
            Pair = null;
        }

        /// <summary>
        /// Привязка Pair
        /// </summary>
        /// <param name="newPair">Тот объект, который хотим связать с текущим</param>
        public void CreatePair(BaseClickComponent newPair)
        {
            newPair.Pair = this;
            Pair = newPair;
        }
	}

    public enum ColorType
    {
        White,
        Black
    }

    public delegate void ClickEventHandler(BaseClickComponent component);
    public delegate void FocusEventHandler(CellComponent component, bool isSelect);
}