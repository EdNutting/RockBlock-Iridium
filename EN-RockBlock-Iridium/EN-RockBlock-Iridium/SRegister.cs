using System;
using System.Collections.Generic;

namespace EN.RockBlockIridium
{
    public class SRegister
    {
        public readonly static int[] ValidRegisterNumbers =
        {
            2, 3, 4, 5, 13, 14, 21, 23, 39, 112, 113, 121, 122
        };

        public int Index { get; private set; }
        public int Value { get; private set; }

        internal SRegister(int index, int value)
        {
            Index = index;
            Value = value;
        }
    }
}
