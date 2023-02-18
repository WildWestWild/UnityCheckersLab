namespace Checkers
{
    public interface IClickHandler
    {
        void OnCheckClicked(IBaseClickComponent component);
        void OnWhiteChipClicked(IBaseClickComponent component);
        void OnBlackChipClicked(IBaseClickComponent component);
    }
}