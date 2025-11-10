using System.Collections.Generic;
using UnityEngine;
using Renderer = VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem.Renderer;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.Console
{
    public class PrototypeConsole
    {
        public readonly List<PrototypeLog> PrototypeLogList = new();

        public static void Log(Prototype prototype, PrototypeLog prototypeLog)
        {
            foreach (Renderer customRenderer in RendererStackManager.Instance.RendererStack.ElementList)
            {
                if (customRenderer is PrototypeRenderer prototypeRenderer)
                {
                    foreach (Prototype proto in prototypeRenderer.SelectionData.PrototypeList)
                    {
                        if (prototype == proto)
                        {
                            if (!HasLog(prototype, prototypeLog))
                            {
                                prototype.PrototypeConsole.PrototypeLogList.Add(prototypeLog);
                            }

                            if (prototypeLog.Error)
                            {
                                Debug.LogError(prototypeLog.Header + ". " + prototypeLog.Text);
                            }
                            else
                            {
                                Debug.LogWarning(prototypeLog.Header + ". " + prototypeLog.Text);
                            }
                        }
                    }
                }
            }
        }

        private static bool HasLog(Prototype prototype, PrototypeLog prototypeLog)
        {
            foreach (PrototypeLog log in prototype.PrototypeConsole.PrototypeLogList)
            {
                if (log.Header == prototypeLog.Header && log.Text == prototypeLog.Text)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasError()
        {
            foreach (PrototypeLog prototypeLog in PrototypeLogList)
            {
                if (prototypeLog.Error)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasWarning()
        {
            foreach (PrototypeLog prototypeLog in PrototypeLogList)
            {
                if (!prototypeLog.Error)
                {
                    return true;
                }
            }

            return false;
        }

        public void Clear() => PrototypeLogList.Clear();
    }
}
