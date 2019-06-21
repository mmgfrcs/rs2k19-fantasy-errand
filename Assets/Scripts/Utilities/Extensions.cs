using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyErrand
{
    public static class Extensions
    {
        public static Texture2D ToTexture2D(this Texture texture)
        {
            return Texture2D.CreateExternalTexture(
                texture.width,
                texture.height,
                TextureFormat.RGB24,
                false, false,
                texture.GetNativeTexturePtr());
        }
        public static List<GameObject> GetAllChildren(this Transform t)
        {
            List<GameObject> go = new List<GameObject>();
            foreach(Transform trans in t)
            {
                go.Add(trans.gameObject);
            }
            return go;
        }
        public static GameObject FindByName(this GameObject[] arr, string name)
        {
            foreach(var i in arr)
            {
                if (i.name.ToLower().Contains(name.ToLower()))
                {
                    return i;
                }
            }

            return null;
        }

        public static void SetActiveAll(this GameObject[] arr, bool active)
        {
            foreach(var i in arr)
            {
                i.SetActive(active);
            }
        }
    }
}
