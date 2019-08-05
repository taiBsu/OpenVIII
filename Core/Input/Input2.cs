﻿using Microsoft.Xna.Framework.Input;
using OpenVIII.Encoding.Tags;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenVIII
{
    public class Input2
    {
        #region Fields

        private static Dictionary<Button_Flags, FF8TextTagKey> Convert_Button { get; set; }
        protected static bool bLimitInput;
        public static InputKeyboard Keyboard { get; private set; }
        public static InputGamePad GamePad { get; private set; }
        public static InputMouse Mouse { get; private set; }
        protected static double msDelay;
        private static readonly int msDelayLimit = 100;

        #endregion Fields

        #region Constructors

        public Input2(bool skip = false)
        {
            if (!skip)
            {
                if (Keyboard == null)
                    Keyboard = new InputKeyboard();
                if (Mouse == null)
                    Mouse = new InputMouse();
                if (GamePad == null)
                    GamePad = new InputGamePad();
                if (InputList == null)
                    InputList = new List<Inputs>
                {
                    new Inputs_OpenVIII(),
                    new Inputs_FF8PSX(),
                    new Inputs_FF8Steam(),
                    new Inputs_FF82000()
                };
                if (main == null)
                    main = new Input2(true);
                if (Convert_Button == null)
                {
                    Convert_Button = new Dictionary<Button_Flags, FF8TextTagKey>()
                    {
                        //Buttons is
                        //finisher = 0x0001
                        //up = 0x0010
                        //-> = 0x0020
                        //do = 0x0040
                        //< - = 0x0080
                        //L2 = 0x0100
                        //R2 = 0x0200
                        //L1 = 0x0400
                        //R1 = 0x0800
                        // /\ = 0x1000
                        //O = 0x2000
                        //X = 0x4000
                        //| _ |= 0x8000
                        //None = 0xFFFF

                        {Button_Flags.Up, FF8TextTagKey.Up },
                        {Button_Flags.Right, FF8TextTagKey.Right },
                        {Button_Flags.Down, FF8TextTagKey.Down },
                        {Button_Flags.Left, FF8TextTagKey.Left },
                        {Button_Flags.L2, FF8TextTagKey.EscapeLeft },
                        {Button_Flags.R2, FF8TextTagKey.EscapeRight },
                        {Button_Flags.L1, FF8TextTagKey.RotateLeft },
                        {Button_Flags.R1, FF8TextTagKey.RotateRight },
                        {Button_Flags.Triangle, FF8TextTagKey.Cancel },
                        {Button_Flags.Circle, FF8TextTagKey.Menu },
                        {Button_Flags.Cross, FF8TextTagKey.Confirm },
                        {Button_Flags.Square, FF8TextTagKey.Cards }
                    };
                }
            }
        }

        private static Input2 main;

        #endregion Constructors

        #region Methods

        public static bool Update()
        {
            CheckInputLimit();
            Keyboard?.UpdateOnce();
            GamePad?.UpdateOnce();
            Mouse?.UpdateOnce();
            return false;
        }

        protected bool ButtonTriggered(FF8TextTagKey key)
        {
            foreach (Inputs list in InputList.Where(x => x.Data.Any(y => y.Key.Contains(key))))
            {
                foreach (KeyValuePair<List<FF8TextTagKey>, List<InputButton>> kvp in list.Data.Where(y => y.Key.Contains(key)))
                {
                    foreach (InputButton test in kvp.Value)
                    {
                        if(ButtonTriggered(test))
                        return true;
                    }
                }
            }

            return false;
        }

        protected virtual bool ButtonTriggered(InputButton test)
        {
            if (!bLimitInput || (test.Trigger & ButtonTrigger.IgnoreDelay) != 0)
            {
                if (Keyboard.ButtonTriggered(test))
                    return true;
                if (Mouse.ButtonTriggered(test))
                    return true;
                if (GamePad.ButtonTriggered(test))
                    return true;
            }
            return false;
        }

        protected virtual bool UpdateOnce() => false;

        private static void CheckInputLimit()
        {
            //issue here if CheckInputLimit is checked more than once per update cycle this will be wrong.
            if (Memory.gameTime != null)
                bLimitInput = (msDelay += Memory.gameTime.ElapsedGameTime.TotalMilliseconds) < msDelayLimit;
            if (!bLimitInput) msDelay = 0;
        }

        public static List<Inputs> InputList { get; private set; }

        public static bool Button(FF8TextTagKey k) => main?.ButtonTriggered(k) ?? false;

        public static bool Button(InputButton k) => main?.ButtonTriggered(k) ?? false;

        public static bool Button(Keys k) => main?.ButtonTriggered(new InputButton() { Key = k }) ?? false;

        public static bool Button(GamePadButtons k) => main?.ButtonTriggered(new InputButton() { GamePadButton = k }) ?? false;

        public static bool Button(MouseButtons k) => main?.ButtonTriggered(new InputButton() { MouseButton = k }) ?? false;

        public static bool DelayedButton(FF8TextTagKey k)
        {
            bool ret = Button(k);
            if (ret)
                ResetInputLimit();
            return ret;
        }

        public static bool DelayedButton(InputButton k)
        {
            bool ret = Button(k);
            if (ret)
                ResetInputLimit();
            return ret;
        }

        public static bool DelayedButton(Keys k)
        {
            bool ret = Button(k);
            if (ret)
                ResetInputLimit();
            return ret;
        }

        public static bool DelayedButton(MouseButtons k)
        {
            bool ret = Button(k);
            if (ret)
                ResetInputLimit();
            return ret;
        }

        public static bool DelayedButton(GamePadButtons k)
        {
            bool ret = Button(k);
            if (ret)
                ResetInputLimit();
            return ret;
        }

        public static bool Button(Button_Flags k) => Convert_Button.ContainsKey(k) && Button(Convert_Button[k]);
        public static bool DelayedButton(Button_Flags k) => Convert_Button.ContainsKey(k) && Button(Convert_Button[k]);
        public static IReadOnlyList<FF8TextTagKey> Convert_Flags(Button_Flags k)
        {
            List<FF8TextTagKey> ret = new List<FF8TextTagKey>(1);
            foreach (Button_Flags x in Enum.GetValues(typeof(Button_Flags)))
            {
                if (k.HasFlag(x) && Convert_Button.ContainsKey(k))
                {
                    Debug.WriteLine("{0} set", x);
                    ret.Add(Convert_Button[k]);
                }
            }
            return ret;
        }
        public static double Distance(float speed) =>
            // no input throttle but still take the max speed * time; for non analog controls
            speed * Memory.gameTime.ElapsedGameTime.TotalMilliseconds;

        public static void ResetInputLimit()
        {
            msDelay = 0;
            bLimitInput = false;
        }

        #endregion Methods
    }
}