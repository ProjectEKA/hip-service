using System;
using System.Collections.Generic;

namespace In.ProjectEKA.HipService.Link.Model
{
    public class AuthModeFetch
    {
        public AuthModeFetch(string purpose, List<Mode> modes)
        {
            Purpose = purpose;
            Modes = modes;
        }

        public string Purpose { get; }
        public List<Mode> Modes { get; }
    }
}