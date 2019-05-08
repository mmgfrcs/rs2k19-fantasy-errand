using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FantasyErrand
{
    public static class ImageRotator
    {
        public static Texture2D RotateImage(this Texture2D t)
        {
            Texture2D newTexture = new Texture2D(t.height, t.width, t.format, false);

            for (int i = 0; i < t.width; i++)
            {
                for (int j = 0; j < t.height; j++)
                {
                    newTexture.SetPixel(j, i, t.GetPixel(t.width - i, j));
                }
            }
            newTexture.Apply();
            return newTexture;
        }
    }
}
