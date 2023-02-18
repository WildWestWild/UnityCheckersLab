using System.Collections.Generic;
using Checkers;
using JetBrains.Annotations;

namespace DefaultNamespace
{
    public interface IObserver
    {
        bool IsNeedBlockEventUI { get; }
        void CheckValidMode([CanBeNull] ref IObserver observer);
        void TryActiveRecordMode(IGameManager gameManager);
        void TryActiveReplayMode(IClickHandler clickHandler, Dictionary<string, IBaseClickComponent> coordinateDictionary);
        void SaveClick(IBaseClickComponent baseClickComponent, Coordinates coordinates);
        void CheckReplayMethod();
    }
}