namespace Client
{
    public class Command
    {
        public Operacao Op;
        public string? Key;
        public string? Value;

    }

     public enum Operacao
        {
        Insert,
        Update,
        Remove,
        Search
        }
}