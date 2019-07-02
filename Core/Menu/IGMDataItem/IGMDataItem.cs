﻿using Microsoft.Xna.Framework;

namespace OpenVIII
{
   
        #region Classes

        public abstract class IGMDataItem//<T>
        {
            //protected T _data;
            protected Rectangle _pos;
            public bool Enabled { get; private set; } = true;
            public Vector2 Scale { get; set; }

            public IGMDataItem(Rectangle? pos = null, Vector2? scale = null)
            {
                _pos = pos ?? Rectangle.Empty;
                Scale = scale ?? TextScale;
            }
        public static float Fade => Menu.Fade;
        public static Vector2 TextScale => Menu.TextScale;
        public static float Blink_Amount => Menu.Blink_Amount;
        public static void DrawPointer(Point cursor, Vector2? offset = null, bool blink = false) => Menu.DrawPointer(cursor, offset, blink);
        public virtual void Show() => Enabled = true;
            public virtual void Hide() => Enabled = false;

            /// <summary>
            /// Where to draw this item.
            /// </summary>
            public virtual Rectangle Pos { get => _pos; set => _pos = value; }

            public Color Color { get; set; } = Color.White;

            //public virtual object Data { get; public set; }
            //public virtual FF8String Data { get; public set; }
            public abstract void Draw();

            public static implicit operator Rectangle(IGMDataItem v) => v.Pos;

            public static implicit operator Color(IGMDataItem v) => v.Color;

            public static implicit operator IGMDataItem(IGMData v)
            {
                return new IGMDataItem_IGMData(v);
            }

            public virtual void ReInit()
            { }

            public virtual bool Update()
            { return false; }

            public virtual bool Inputs()
            { return false; }
        }
        #endregion Classes
    
}