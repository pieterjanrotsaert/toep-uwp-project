using PrettigLokaalBackend.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Newtonsoft.Json;
using PrettigLokaalBackend.Models.Domain;
using Windows.Storage;
using System.Security.Authentication;

namespace PrettigLokaal.Backend
{
    // This class is responsible for calling the backend API server, translating ViewModels into request models and vice-versa.
    class API
    {
        private static API singleton = null;
        private const string PWVAULT_TOKENRES        = "AUTH";
        private const string PWVAULT_TOKENUSER       = "TOKEN";
        private const string PWVAULT_ACCOUNTRES      = "ACCOUNT";
        private const string PWVAULT_ACCOUNTNAME     = "NAME";
        private const string PWVAULT_ACCOUNTPASSWORD = "PASSWORD";

        private const string ACCOUNTCACHE = "account.json";

#if DEBUG
        private const string ENDPOINT     = "https://localhost:3001";
#else
        private const string ENDPOINT     = "http://localhost:3000";
#endif 

        private PasswordVault passwordVault = new PasswordVault();
        private StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        private HttpClient client;
        private HttpClientHandler clientHandler = new HttpClientHandler();

        private string token = ""; // JWT auth token, empty if not logged in.
        private Account accountData = null; // Cached account data

        public delegate void Callback<T>(T result, ErrorModel error);
        public delegate void VoidCallback(ErrorModel error);

        private API()
        {
            // Prevents exceptions being thrown because of https signing errors.
            clientHandler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            clientHandler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
            client = new HttpClient(clientHandler);
        }

        public static API Get() // Lazily instantiates the singleton and returns it.
        {
            if (singleton == null)
                singleton = new API();
            return singleton;
        }

        public void Init(VoidCallback callback)
        {
            LoadToken();
            if (IsLoggedIn())
                LoadAccountData(callback);
            else
                callback(null);
        }

        private async void LoadAccountData(VoidCallback callback)
        {
            StorageFile file = null;
            try
            {
                file = await storageFolder.GetFileAsync(ACCOUNTCACHE);
            }
            catch (Exception) { }
            if (file != null)
            {
                accountData = JsonConvert.DeserializeObject<Account>(await FileIO.ReadTextAsync(file));
                callback.Invoke(null);
            }
            else
                callback.Invoke(new ErrorModel(ErrorModel.NOT_FOUND, ACCOUNTCACHE));
        }

        private async void SaveAccountData(VoidCallback callback)
        {
            StorageFile file = null;
            try
            {
                file = await storageFolder.CreateFileAsync(ACCOUNTCACHE, CreationCollisionOption.ReplaceExisting);
            }
            catch (Exception) { }
            if(accountData != null && file != null)
                await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(accountData));
            callback.Invoke(null);
        }

        private void LoadToken()
        {
            string _token = null;
            try
            {
                var credentials = passwordVault.Retrieve(PWVAULT_TOKENRES, PWVAULT_TOKENUSER);
                credentials.RetrievePassword();
                _token = credentials.Password;
            }
            catch (Exception) { }
            if (_token == null)
                SetToken("");
            else 
                SetToken(_token);
        }

        private void SetToken(string _token)
        {
            token = _token;
            if(!string.IsNullOrEmpty(token))
                passwordVault.Add(new PasswordCredential(PWVAULT_TOKENRES, PWVAULT_TOKENUSER, token));
            else
            {
                try
                {
                    var credentials = passwordVault.Retrieve(PWVAULT_TOKENRES, PWVAULT_TOKENUSER);
                    passwordVault.Remove(credentials);
                }
                catch (Exception) { }
            }
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        }

        private void ClearToken()
        {
            SetToken("");
        }

        // Sends a request to the Backend, and parses the result into a model of type T.
        private async void Send<T>(HttpMethod method, string path, object requestModel, Callback<T> callback)
        {
            var request = new HttpRequestMessage(method, ENDPOINT + path);
            if(requestModel != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request);
            }
            catch(HttpRequestException ex)
            {
                callback.Invoke(default(T), new ErrorModel(ErrorModel.NETWORK_ERROR, ex.Message));
                return; 
            }

            if (response.IsSuccessStatusCode)
                callback.Invoke(JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync()), null);
            else
            {
                if((int)response.StatusCode == 422) // Http Status 422: Unprocessable Entity (Backend uses this to throw errors)
                    callback.Invoke(default(T), JsonConvert.DeserializeObject<ErrorModel>(await response.Content.ReadAsStringAsync()));
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) // Happens when the token expires or is invalid.
                {
                    ClearToken();
                    callback.Invoke(default(T), new ErrorModel(ErrorModel.NOT_LOGGED_IN));
                }
                else
                {
                    ErrorModel err = new ErrorModel(ErrorModel.HTTP_ERROR, 
                                                    "Error " + response.StatusCode.ToString() + ": " + response.ReasonPhrase);
                    callback.Invoke(default(T), err);
                }
            }
        }

        // Sends a request to the backend and ignores the resulting content. (Use this for Posts etc. that don't return anything.)
        private async void SendVoid(HttpMethod method, string path, object requestModel, VoidCallback callback)
        {
            var request = new HttpRequestMessage(method, ENDPOINT + path);
            if (requestModel != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request);
            }
            catch (HttpRequestException ex)
            {
                callback.Invoke(new ErrorModel(ErrorModel.NETWORK_ERROR, ex.Message));
                return;
            }

            if (response.IsSuccessStatusCode)
                callback.Invoke(null);
            else
            {
                if ((int)response.StatusCode == 422) // Http Status 422: Unprocessable Entity (Backend uses this to throw errors)
                    callback.Invoke(JsonConvert.DeserializeObject<ErrorModel>(await response.Content.ReadAsStringAsync()));
                else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ClearToken();
                    callback.Invoke(new ErrorModel(ErrorModel.NOT_LOGGED_IN));
                }
                else
                {
                    ErrorModel err = new ErrorModel(ErrorModel.HTTP_ERROR,
                                                    "Error " + response.StatusCode.ToString() + ": " + response.ReasonPhrase);
                    callback.Invoke(err);
                }
            }
        }

        // Convenience functions
        public void SendPost<T>(string path, object requestModel, Callback<T> callback)
        {
            Send(HttpMethod.Post, path, requestModel, callback);
        }

        public void SendPostVoid(string path, object requestModel, VoidCallback callback)
        {
            SendVoid(HttpMethod.Post, path, requestModel, callback);
        }

        public void SendGet<T>(string path, Callback<T> callback)
        {
            Send(HttpMethod.Get, path, null, callback);
        }

        public bool IsLoggedIn()
        {
            return token.Length > 0;
        }

        public bool IsMerchant()
        {
            return accountData != null ? accountData.Merchant != null : false;
        }

        public Account GetAccountInfo()
        {
            return accountData;
        }

        public void RememberLoginInfo(string username, string password)
        {
            if (!string.IsNullOrEmpty(username))
                passwordVault.Add(new PasswordCredential(PWVAULT_ACCOUNTRES, PWVAULT_ACCOUNTNAME, username));
            if (!string.IsNullOrEmpty(password))
                passwordVault.Add(new PasswordCredential(PWVAULT_ACCOUNTRES, PWVAULT_ACCOUNTPASSWORD, password));
        }

        public string GetRememberedLoginName()
        {
            try
            {
                var credentials = passwordVault.Retrieve(PWVAULT_ACCOUNTRES, PWVAULT_ACCOUNTNAME);
                credentials.RetrievePassword();
                return credentials.Password;
            }
            catch (Exception) { }
            return "";
        }

        public string GetRememberedLoginPassword()
        {
            try
            {
                var credentials = passwordVault.Retrieve(PWVAULT_ACCOUNTRES, PWVAULT_ACCOUNTPASSWORD);
                credentials.RetrievePassword();
                return credentials.Password;
            }
            catch (Exception) { }
            return "";
        }

        public bool HasRememberedLoginInfo()
        {
            return (GetRememberedLoginName().Length > 0 || GetRememberedLoginPassword().Length > 0);
        }

        public void ClearRememberedLoginInfo()
        {
            try
            {
                var credentials = passwordVault.Retrieve(PWVAULT_TOKENRES, PWVAULT_ACCOUNTNAME);
                passwordVault.Remove(credentials);
            }
            catch (Exception) { }
            try
            {
                var credentials = passwordVault.Retrieve(PWVAULT_TOKENRES, PWVAULT_ACCOUNTPASSWORD);
                passwordVault.Remove(credentials);
            }
            catch (Exception) { }
        }

        // This function retrieves and stores account info, it is automatically called by Login() and CreateAccount().
        public void RetrieveAccountInfo(VoidCallback callback)
        {
            if (!IsLoggedIn())
                callback(new ErrorModel(ErrorModel.NOT_LOGGED_IN));

            SendGet<Account>("/api/account", (response, err) =>
            {
                if (response == null)
                    callback(err);
                else
                {
                    accountData = response;
                    SaveAccountData(callback);
                }
            });
        }

        public void Login(string email, string password, VoidCallback callback)
        {
            LoginModel model = new LoginModel()
            {
                Email = email,
                Password = password 
            };

            SendPost<LoginResponse>("/api/account/login", model, (response, err) => 
            {
                if (err == null)
                {
                    SetToken(response.Token);
                    RetrieveAccountInfo(callback);
                }
                else
                    callback.Invoke(err);
            });
        }

        public void Logout(VoidCallback callback)
        {
            // TODO: Contact server and ask it to invalidate the token first.
            ClearToken();
            accountData = null;
            callback.Invoke(null);
        }

        public void CreateAccount(string email, string password, string fullname, DateTime birthDay, VoidCallback callback)
        {
            CreateUserModel model = new CreateUserModel()
            {
                Email = email,
                Password = password,
                Fullname = fullname,
                BirthDate = birthDay
            };

            SendPost<LoginResponse>("/api/account/create", model, (response, err) =>
            {
                if (err == null)
                {
                    SetToken(response.Token);
                    RetrieveAccountInfo(callback);
                }
                else
                    callback.Invoke(err);
            });
        }

        public void ChangePassword(string oldPassword, string newPassword, VoidCallback callback)
        {
            UpdatePasswordModel model = new UpdatePasswordModel()
            {
                OldPassword = oldPassword,
                NewPassword = newPassword
            };

            SendPostVoid("/api/account/updatepassword", model, callback);
        }

        public void RegisterAsMerchant(MerchantRegisterModel model, VoidCallback callback)
        {
            SendPostVoid("/api/merchant/register", model, err =>
            {
                if (err != null)
                    callback(err);
                else
                    RetrieveAccountInfo(callback); // Retrieve & store updated account info.
            });
        }

        public void UpdateMerchantDetails(MerchantRegisterModel model, VoidCallback callback)
        {
            SendPostVoid("/api/merchant/updatedetails", model, err =>
            {
                if (err != null)
                    callback(err);
                else
                    RetrieveAccountInfo(callback); // Retrieve & store updated account info.
            });
        }

        // Warning: this deletes your merchant account.
        public void TerminateMerchantAccount(VoidCallback callback)
        {
            SendPostVoid("/api/merchant/terminate", null, err =>
            {
                if (err != null)
                    callback(err);
                else
                    RetrieveAccountInfo(callback); // Retrieve & store updated account info.
            });
        }
        
        // Retrieves all merchant data of the currently logged in account, including images etc.
        public void GetAccountMerchantData(Callback<Merchant> callback)
        {
            SendGet("/api/merchant/myaccount", callback);
        }

        // Upload multiple images, each string represents an image encoded in base64.
        public void UploadMerchantImages(List<string> imageData, VoidCallback callback)
        {
            SendPostVoid("/api/merchant/addimages", imageData, callback);
        }

        public void RemoveImage(int id, VoidCallback callback)
        {
            SendPostVoid("/api/merchant/removeimage/" + id, null, callback);
        }

        public void GetImage(int id, Callback<Image> callback)
        {
            SendGet("/api/file/image/" + id, callback);
        }

        public void RemovePromotion(int id, VoidCallback callback)
        {
            SendPostVoid("/api/merchant/removepromotion/" + id, null, callback);
        }

        public void RemoveEvent(int id, VoidCallback callback)
        {
            SendPostVoid("/api/merchant/removeevent/" + id, null, callback);
        }

        public void AddPromotion(MerchantAddPromotionModel model, VoidCallback callback)
        {
            SendPostVoid("/api/merchant/addpromotion", model, callback);
        }

        public void UpdatePromotion(MerchantAddPromotionModel model, VoidCallback callback)
        {
            SendPostVoid("/api/merchant/updatepromotion", model, callback);
        }

        public void AddEvent(MerchantAddEventModel model, VoidCallback callback)
        {
            SendPostVoid("/api/merchant/addevent", model, callback);
        }

        public void UpdateEvent(MerchantAddEventModel model, VoidCallback callback)
        {
            SendPostVoid("/api/merchant/updateevent", model, callback);
        }

    }
}
