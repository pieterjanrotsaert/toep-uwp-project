using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Models.Requests
{
    public class SimpleBoolModel
    {
        public bool State { get; set; }

        public SimpleBoolModel() { }
        public SimpleBoolModel(bool state) { State = state; }
    }
}
