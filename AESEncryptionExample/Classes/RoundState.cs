using System.Collections.Generic;

namespace AESEncryptionExample.Classes
{
    public class RoundState
    {
        public RoundState(string header, List<List<string>> matrix)
        {
            Header = header;
            Matrix = matrix;
        }
        public string Header { get; set; }
        public List<List<string>> Matrix { get; set; }
    }
}
