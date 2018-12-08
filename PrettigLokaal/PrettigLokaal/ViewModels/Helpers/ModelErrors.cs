using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PrettigLokaal.ViewModels.Helpers
{
    public class ModelErrors : INotifyPropertyChanged
    {
        private readonly Dictionary<string, string> modelErrors = new Dictionary<string, string>();

        public bool IsValid
        {
            get { return modelErrors.Count < 1; }
        }

        public void Clear()
        {
            modelErrors.Clear();
        }

        public string this[string fieldName]
        {
            get
            {
                return modelErrors.ContainsKey(fieldName) ?
                    modelErrors[fieldName] : string.Empty;
            }

            set
            {
                if (modelErrors.ContainsKey(fieldName))
                {
                    if (string.IsNullOrWhiteSpace(value))
                        modelErrors.Remove(fieldName);
                    else
                        modelErrors[fieldName] = value;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(value))
                        modelErrors.Add(fieldName, value);
                }
                RaisePropertyChanged("ModelErrors");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
