using System;
using System.Collections.Generic;
using UnityEngine;

namespace AP
{
    public interface IAnimaLauncher
    {
        public float Factor { get; }
    }

    public class APAnimaMgr : MonoBehaviour
    {
        public delegate float Curve(float t);
        public void Register(IAnimaLauncher launcher, Action<float> setter, Curve curve = null)
        {
            if (launcher == null || setter == null) return;

            var anim = new APAnima()
            {
                enable = true,

                launcher = launcher,
                curve = curve,
                setter = setter,
                t = curve?.Invoke(launcher.Factor) ?? launcher.Factor,
                lastFactor = launcher.Factor,
            };

            if (_animaLauncher.TryGetValue(launcher, out var an))
            {
                an ??= new LinkedList<APAnima>();
                an.AddLast(anim);
            }
            else
            {
                _animaLauncher[launcher] = new LinkedList<APAnima>();
                _animaLauncher[launcher].AddLast(anim);
            }
        }

        public void Unregister(IAnimaLauncher launcher)
        {
            if (_animaLauncher.TryGetValue(launcher, out var an))
            {
                foreach (var a in an)
                {
                    a.enable = false;
                }
            }
        }

        private class APAnima
        {
            public bool enable;

            public IAnimaLauncher launcher;
            public Curve curve;
            public Action<float> setter;
            public float t;
            public float lastFactor;
        }

        private void UpdateAnima(APAnima an)
        {
            if (Mathf.Approximately(an.launcher.Factor, an.lastFactor))
            {
                if ((!(an.launcher.Factor < 0) && !(an.launcher.Factor > 1)) ||
                    !(an.t > 0) || !(an.t < 1)) return;
                an.t = Mathf.Clamp01(an.launcher.Factor);
                an.setter.Invoke(an.curve?.Invoke(an.t) ?? an.t);
                return;
            }

            an.lastFactor = an.launcher.Factor;

            if (an.lastFactor is > 0 and < 1)
            {
                an.t = an.lastFactor;
                an.setter.Invoke(an.curve?.Invoke(an.t) ?? an.t);
            }
            else if (an.t is > 0 and < 1)
            {
                an.t = Mathf.Clamp01(an.launcher.Factor);
                an.setter.Invoke(an.curve?.Invoke(an.t) ?? an.t);
                an.t = -1;
            }
        }

        private readonly LinkedList<IAnimaLauncher> _clearLauncherTemp = new ();
        private readonly Dictionary<IAnimaLauncher, LinkedList<APAnima>> _animaLauncher = new();

        #region Inst

        public static APAnimaMgr I => _i;

        private void Awake()
        {
            if (_i == null)
            {
                _i = this;
                LeanTween.init();
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }

        private void Update()
        {
            foreach (var ans in _animaLauncher)
            {
                var an = ans.Value.First;
                while (an != null)
                {
                    if (an.Value.enable)
                    {
                        UpdateAnima(an.Value);
                    }
                    else
                    {
                        ans.Value.Remove(an);
                    }

                    an = an.Next;
                }

                if (ans.Value.Count == 0)
                {
                    _clearLauncherTemp.AddLast(ans.Key);
                }
            }

            // 清垃圾
            if (_clearLauncherTemp.Count == 0) return;
            foreach (var launcher in _clearLauncherTemp)
            {
                _animaLauncher.Remove(launcher);
            }
            _clearLauncherTemp.Clear();
        }

        private static APAnimaMgr _i;

        #endregion
    }
}