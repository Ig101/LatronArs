namespace LatronArs.WebClient.Models
{
    public class KeyboardEvent
    {
        public string Code { get; set; }

        public bool ShiftKey { get; set; }

        public bool AltKey { get; set; }

        public bool CtrlKey { get; set; }
    }
}