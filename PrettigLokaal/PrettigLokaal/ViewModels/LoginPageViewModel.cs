using PrettigLokaal.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettigLokaal.ViewModels
{
    class LoginPageViewModel : ViewModelBase 
    {
        string Email { get; set; }
        string Password { get; set; }
        bool Remember { get; set; }
    }
}
