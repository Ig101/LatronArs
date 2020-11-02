using System;
using LatronArs.Engine.Scene;
using LatronArs.Models.Enums;

namespace LatronArs.WebClient.Models
{
    public class KeyEventHandler<T>
    {
        public string[] Codes { get; set; }

        public Direction? Direction { get; set; }

        public bool Hold { get; set; }

        public Action<T> Action { get; set; }

        public bool ShiftHold { get; set; }

        public Action<T> ShiftAction { get; set; }

        public bool AltHold { get; set; }

        public Action<T> AltAction { get; set; }

        public bool CtrlHold { get; set; }

        public Action<T> CtrlAction { get; set; }
    }
}