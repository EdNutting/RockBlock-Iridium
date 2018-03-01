using System;
using System.Collections.Generic;

namespace EN.RockBlockIridium
{
    public class ATCommandResponse<T>
    {
        public T Data { get; private set; }
        public bool Error { get; private set; }

        public List<string> RawData { get; private set; }

        internal ATCommandResponse(T data, bool error, List<string> rawData)
        {
            Data = data;
            Error = error;
            RawData = rawData;
        }
    }
}
