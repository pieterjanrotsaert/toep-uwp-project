using PrettigLokaalBackend.Models.Requests;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
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

        public static void ErrorBox(ErrorModel err)
        {
            InfoBox("Er is een fout opgetreden: " + err.GetDescription(), "Fout");
        }


        public static async void YesNoPrompt(string text, string title, Action onConfirm, Action onCancel = null)
        {
            var yesCommand = new UICommand("Ja");
            var noCommand = new UICommand("Nee");

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

        public static async void PickImageFile(Action<StorageFile> callback)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");
            StorageFile file = await picker.PickSingleFileAsync();
            if(file != null)
                callback.Invoke(file);
        }

        public static async void PickImageFiles(Action<IReadOnlyList<StorageFile>> callback)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");
            var files = await picker.PickMultipleFilesAsync();
            if (files != null)
                callback.Invoke(files);
        }


        public static TimeSpan? DateTimeToTimeSpan(DateTime dt)
        {
            TimeSpan FResult;
            try
            {
                FResult = dt - dt.Date;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }

            return FResult;
        }
       
        public static DateTime? TimeSpanToDateTime(TimeSpan ts)
        {
            DateTime? FResult = null;
            try
            {
                string year = string.Format("{0:0000}", DateTime.MinValue.Date.Year);
                string month = string.Format("{0:00}", DateTime.MinValue.Date.Month);
                string day = string.Format("{0:00}", DateTime.MinValue.Date.Day);

                string hours = string.Format("{0:00}", ts.Hours);
                string minutes = string.Format("{0:00}", ts.Minutes);
                string seconds = string.Format("{0:00}", ts.Seconds);

                string dSep = "-"; string tSep = ":"; string dtSep = "T";

                // yyyy-mm-ddTHH:mm:ss
                string dtStr = string.Concat(year, dSep, month, dSep, day, dtSep, hours, tSep, minutes, tSep, seconds);

                DateTime dt;
                if (DateTime.TryParseExact(dtStr, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
                {
                    FResult = dt;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }

            return FResult;
        }
    }
}
