using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace AP
{
    public enum APEventType : uint
    {
        All = 0,
        LeftPanelChange = 1
    }

    public interface IEventLauncher { }

    public class APEventMgr : MonoBehaviour
    {
        public void Register(APEventType type, Action<object[]> act, IEventLauncher launcher)
        {
            if (type == APEventType.All || launcher == null || act == null)
                return;

            var actIn = new APEvent()
            {
                destroyed = false,
                act = act,
            };

            if (_eventLaunchers.TryGetValue(launcher, out var ev))
            {
                ev ??= new LinkedList<APEvent>();
                ev.AddLast(actIn);
                if (!_eventTypes.TryGetValue(type, out var ev2)) return;
                ev2 ??= new LinkedList<APEvent>();
                ev2.AddLast(actIn);
            }
            else
            {
                _eventLaunchers[launcher] = new LinkedList<APEvent>();
                _eventLaunchers[launcher].AddLast(actIn);

                _eventTypes[type] = new LinkedList<APEvent>();
                _eventTypes[type].AddLast(actIn);
            }
        }
        public void Unregister(APEventType type)
        {
            if (type == APEventType.All)
            {
                _eventLaunchers.Clear();
                _eventTypes.Clear();
                return;
            }

            if (!_eventTypes.TryGetValue(type, out var ev)) return;
            if (ev == null) return;
            foreach (var e in ev)
            {
                e.destroyed = true;
                e.act = null;
            }
        }
        public void Unregister(IEventLauncher launcher)
        {
            if (launcher == null) return;
            if (!_eventLaunchers.TryGetValue(launcher, out var ev)) return;
            if (ev == null) return;

            foreach (var e in ev)
            {
                e.destroyed = true;
                e.act = null;
            }
        }
        public void Fire(APEventType type, object[] obj = null)
        {
            if (type == APEventType.All)
            {
                foreach (var evs in _eventTypes)
                {
                    if (evs.Value == null) continue;
                    foreach (var e in evs.Value)
                    {
                        if (e == null) continue;
                        if (!e.destroyed)
                            e.act?.Invoke(obj);
                    }
                }
                return;
            }

            if (!_eventTypes.TryGetValue(type, out var ev)) return;
            if (ev == null) return;

            foreach (var e in ev) if (e is { destroyed: false }) e.act?.Invoke(obj);
        }

        public void Fire(IEventLauncher launcher, object[] obj = null)
        {
            if (launcher == null) return;
            if (!_eventLaunchers.TryGetValue(launcher, out var ev)) return;
            if (ev == null) return;
            foreach (var e in ev) e?.act?.Invoke(obj);
        }

        public float ClearInterval
        {
            get => _interval;
            set => _interval = value;
        }

        // -----------------------------------------------------------
        private class APEvent
        {
            public bool destroyed;
            public Action<object[]> act;
        }

        private void Clear()
        {
            foreach (var ev in _eventLaunchers)
            {
                if (ev.Value == null)
                {
                    _clearLauncherTemp.AddLast(ev.Key);
                    continue;
                }

                var node = ev.Value.First;
                while (node != null)
                {
                    var tmp = node;
                    node = node.Next;
                    if (tmp.Value.destroyed)
                        ev.Value.Remove(tmp);
                }

                if (ev.Value.Count == 0)
                {
                    _clearLauncherTemp.AddLast(ev.Key);
                }
            }
            foreach (var ev in _eventTypes)
            {
                if (ev.Value == null)
                {
                    _clearTypeTemp.AddLast(ev.Key);
                    continue;
                }

                var node = ev.Value.First;
                while (node != null)
                {
                    var tmp = node;
                    node = node.Next;
                    if (tmp.Value.destroyed)
                        ev.Value.Remove(tmp);
                }

                if (ev.Value.Count == 0)
                {
                    _clearTypeTemp.AddLast(ev.Key);
                }
            }
            foreach (var el in _clearLauncherTemp)
            {
                _eventLaunchers.Remove(el);
            }
            foreach (var et in _clearTypeTemp)
            {
                _eventTypes.Remove(et);
            }

            _clearLauncherTemp.Clear();
            _clearTypeTemp.Clear();

            _isClean = true;
        }

        private readonly Dictionary<APEventType, LinkedList<APEvent>> _eventTypes = new();
        private readonly Dictionary<IEventLauncher, LinkedList<APEvent>> _eventLaunchers = new();

        private bool _isClean;
        private float _hasIntervalTime;
        private float _interval = 300f;
        private readonly LinkedList<APEventType> _clearTypeTemp = new();
        private readonly LinkedList<IEventLauncher> _clearLauncherTemp = new();

        #region Inst

        public static APEventMgr I => _i;

        private void Awake()
        {
            if (_i == null)
            {
                _i = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }

        private void Update()
        {
            // 每 interval 间隔检查一次垃圾
            if (_hasIntervalTime > _interval)
            {
                _hasIntervalTime = 0;
                if (!_isClean) Clear();
            }
            _hasIntervalTime += Time.deltaTime;
        }

        private static APEventMgr _i;

        #endregion
    }
}

