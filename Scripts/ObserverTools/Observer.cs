using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Checkers;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace DefaultNamespace
{
    public class Observer : MonoBehaviour, IObserver
    {
        private const string SELECT_CHIP = "select chip - ";
        private const string SELECT_CELL = "click to - ";
        [SerializeField] private bool IsRecord;
        [SerializeField] private bool IsReplay;
        private Dictionary<string, IBaseClickComponent> _coordinateDictionary;
        private IGameManager _gameManager;
        private IClickHandler _clickHandler;
        private string _fileName;
        private readonly Queue<Action> _actionQueueForMainThread = new Queue<Action>();

        private string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                {
                    _fileName = $"RecordGame-[{DateTime.Now:d}].txt";
                }

                return _fileName;
            }
        }

        public void TryActiveReplayMode(IClickHandler clickHandler, Dictionary<string, IBaseClickComponent> coordinateDictionary)
        {
            if (IsReplay)
            {
                _coordinateDictionary = coordinateDictionary;
                _clickHandler = clickHandler;
                Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(1500);
                        StartReplay();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                });
            }
        }

        public void TryActiveRecordMode(IGameManager gameManager)
        {
            if (IsRecord)
            {
                if (File.Exists(FileName))
                {
                    File.Delete(FileName);
                }

                File.Create(FileName);
                _gameManager = gameManager;
            }
        }

        /// <summary>
        /// Нужно ли блокировать инициализацию обработчиков событий для отключения управления пользователя
        /// </summary>
        public bool IsNeedBlockEventUI => IsReplay;

        public void CheckValidMode(ref IObserver observer)
        {
            if (IsReplay && IsRecord)
            {
                throw new Exception("Пожалуйста выберете один из режимов");
            }

            if (!IsReplay && !IsRecord)
            {
                observer = null;
            }
        }

        public void SaveClick(IBaseClickComponent baseClickComponent, Coordinates coordinates)
        {
            if (IsRecord)
            {
                var colorToString = _gameManager.IsWhiteMove ? ColorType.White.ToString() : ColorType.Black.ToString();
                string resultReplayLog = $"Player {colorToString} {SelectPhase(baseClickComponent)} XY = {coordinates.GetCoordinateKey()} ";
                File.AppendAllText(FileName, resultReplayLog + Environment.NewLine);
            }
        }

        public void CheckReplayMethod()
        {
            if (_actionQueueForMainThread.Any())
            {
                var method = _actionQueueForMainThread.Dequeue();
                method.Invoke();
            }
        }

        private void StartReplay()
        {
            string[] replayLogs = File.ReadAllLines(FileName);

            foreach (var replayLog in replayLogs)
            {
                if (replayLog.Contains(SELECT_CHIP))
                {
                    if (replayLog.Contains(ColorType.White.ToString()))
                    {
                        var component = GetIBaseComponent(replayLog);
                        _actionQueueForMainThread.Enqueue(() => _clickHandler.OnWhiteChipClicked(component.Pair));
                    }
                    else
                    {
                        var component = GetIBaseComponent(replayLog);
                        _actionQueueForMainThread.Enqueue(() =>_clickHandler.OnBlackChipClicked(component.Pair));
                    }
                }

                if (replayLog.Contains(SELECT_CELL))
                {
                    var component = GetIBaseComponent(replayLog);
                    _actionQueueForMainThread.Enqueue(() =>_clickHandler.OnCheckClicked(component));
                }

                Thread.Sleep(1000);
            }
        }

        private IBaseClickComponent GetIBaseComponent(string replayLog)
        {
            string coordinateKey = replayLog[replayLog.Length - 3].ToString() + replayLog[replayLog.Length - 2];
            var component = _coordinateDictionary[coordinateKey];
            return component;
        }

        private string SelectPhase(IBaseClickComponent baseClickComponent) => baseClickComponent switch
        {
            ChipComponent _ => SELECT_CHIP,
            CellComponent _ => SELECT_CELL,
            _ => throw new ArgumentOutOfRangeException(nameof(baseClickComponent), baseClickComponent, null)
        };
    }
}