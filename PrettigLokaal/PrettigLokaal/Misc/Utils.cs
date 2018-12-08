using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace PrettigLokaal.Misc
{
    class Utils
    {
        public static async void InfoBox(string text, string title, Action onComplete = null)
        {
            var dialog = new MessageDialog(text, title);
            await dialog.ShowAsync();
            if(onComplete != null)
                onComplete.Invoke();
        }

        public static async void InfoBox(string text, Action onComplete = null)
        {
            var dialog = new MessageDialog(text);
            await dialog.ShowAsync();
            if (onComplete != null)
                onComplete.Invoke();
        }


        public static async void YesNoPrompt(string text, string title, Action onConfirm, Action onCancel = null)
        {
            var yesCommand = new UICommand("Yes");
            var noCommand = new UICommand("No");

            var dialog = new MessageDialog(text, title);
            dialog.Options = MessageDialogOptions.None;
            dialog.Commands.Add(yesCommand);
            dialog.Commands.Add(noCommand);

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var command = await dialog.ShowAsync();
            if (command == yesCommand)
                onConfirm.Invoke();
            else if (onCancel != null)
                onCancel.Invoke();
        }

        public static void YesNoPrompt(string text, Action onConfirm, Action onCancel = null)
        {
            YesNoPrompt(text, "Bericht", onConfirm, onCancel);
        }

    }
}
