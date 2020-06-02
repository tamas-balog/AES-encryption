using System.Collections.Generic;

namespace AESEncryptionExample.Classes
{
    public class Round
    {
        public List<RoundState> Matrixes { get; set; }
        public Round()
        {
            Matrixes = new List<RoundState>();
        }
    }
}
