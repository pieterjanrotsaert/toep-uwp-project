using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PrettigLokaal.ViewModels.Helpers
{
    // This class provides helpers for validation and raising events.
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [NotMapped][JsonIgnore] public ModelErrors ModelErrors { get; set; } = new ModelErrors();
        [NotMapped][JsonIgnore] public bool IsValid { get { return ModelErrors.IsValid; } }

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
