using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PrettigLokaal.ViewModels.Helpers
{
    // This class provides helpers for validation and raising events.
    abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ModelErrors ModelErrors { get; set; } = new ModelErrors();
        public bool IsValid { get { return ModelErrors.IsValid; } }

        public void Validate()
        {
            ModelErrors.Clear();
            ValidateSelf();
            RaisePropertyChanged("ModelErrors");
        }

        public void AddModelError(string fieldName, string error)
        {
            ModelErrors[fieldName] = error;
            RaisePropertyChanged("ModelErrors");
        }

        protected abstract void ValidateSelf();

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
