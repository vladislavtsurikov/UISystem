using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core;
#if UNITY_EDITOR

#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings
{
    [Serializable]
    public class CustomMasks
    {
        private static readonly StringBuilder _builder = new();
        public List<Texture2D> Masks = new();
        public int SelectedCustomBrush;

        public void GetBuiltinBrushes()
        {
#if UNITY_EDITOR
            GetBuiltinTextures(EditorResources.brushesPath, "builtin_brush_");
            SelectedCustomBrush = Mathf.Min(Masks.Count - 1, SelectedCustomBrush);
#endif
        }

        public void GetPolarisBrushes()
        {
            Masks = new List<Texture2D>(Resources.LoadAll<Texture2D>(MegaWorldPath.PolarisBrushes));
            SelectedCustomBrush = Mathf.Min(Masks.Count - 1, SelectedCustomBrush);
        }

        private void GetBuiltinTextures(string path, string prefix)
        {
#if UNITY_EDITOR
            Texture2D texture;
            var index = 1;
            do // begin from ../Brush_1 to ../Brush_n until there is a file not found
            {
                // Build file path
                _builder.Length = 0;
                _builder.Append(path);
                _builder.Append(prefix);
                _builder.Append(index);
                _builder.Append(".png");

                // Increase index for next texture
                index++;

                // Add texture to list
                texture = (Texture2D)EditorGUIUtility.Load(_builder.ToString());
                if (texture != null)
                {
                    Masks.Add(texture);
                }
            } while (texture != null);
#endif
        }

        public Texture2D GetSelectedBrush()
        {
            if (Masks.Count == 0)
            {
                return null;
            }

            if (SelectedCustomBrush > Masks.Count - 1)
            {
                return null;
            }

            return Masks[SelectedCustomBrush];
        }
    }
}
