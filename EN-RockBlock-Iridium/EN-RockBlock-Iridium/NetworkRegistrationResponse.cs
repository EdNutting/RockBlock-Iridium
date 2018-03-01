using System;
using System.Collections.Generic;

namespace EN.RockBlockIridium
{
    public class NetworkRegistrationResponse
    {
        public NetworkRegistrationStatuses Status { get; private set; }
        public int RegistrationError { get; private set; }

        public NetworkRegistrationResponse(NetworkRegistrationStatuses status, int err)
        {
            Status = status;
            RegistrationError = err;
        }
    }
}
