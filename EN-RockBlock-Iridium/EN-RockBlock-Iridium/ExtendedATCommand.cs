using System;
using System.Collections.Generic;

namespace EN.RockBlockIridium
{
    internal class ExtendedATCommand : IATCommand
    {
        public enum Groups
        {
            None,
            Cellular,
            DataCompression,
            Generic,
            InterfaceControl,
            ShortBurstData,
            MotorolaSatelliteProductProprietary
        }

        private static readonly string[] Prefixes =
        {
            "",
            "+C",
            "+D",
            "+G",
            "+I",
            "+SBD",
            "-MS",
        };

        public static string GetPrefix(Groups group)
        {
            return Prefixes[(int)group];
        }

        private Groups Group;
        private string Command;
        private bool Test;
        private int[] Parameters;

        internal ExtendedATCommand(Groups group, string command, bool test, params int[] parameters)
        {
            Group = group;
            Command = command;
            Test = test;
            Parameters = parameters;
        }

        public string GetCommandString()
        {
            if (Test)
            {
                return String.Format("{0}{1}=?", GetPrefix(Group), Command);
            }
            else if (Parameters.Length > 0)
            {
                return String.Format("{0}{1}={2}", GetPrefix(Group), Command, String.Join(",", Parameters));
            }
            else
            {
                return String.Format("{0}{1}", GetPrefix(Group), Command);
            }
        }
    }
}
