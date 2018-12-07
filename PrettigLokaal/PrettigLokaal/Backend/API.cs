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

namespace PrettigLokaal.Backend
{
    // This class is responsible for calling the backend API server, translating ViewModels into request models and vice-versa.
    class API
    {
        private static API singleton = null;
        private const string PWVAULT_RES  = "AUTH";
        private const string PWVAULT_USER = "API";
        private const string ACCOUNTCACHE = "account.json";

#if DEBUG
        private const string ENDPOINT     = "http://localhost:3000";
#else
        private const string ENDPOINT     = "http://localhost:3000";
#endif 

        private PasswordVault passwordVault = new PasswordVault();
        private StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        private HttpClient client = new HttpClient();
        private string token = ""; // JWT auth token, empty if not logged in.
        private Account accountData = null; // Cached account data

        public delegate void Callback<T>(T result, ErrorModel error);
        public delegate void VoidCallback(ErrorModel error);

        private API()
        {  
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
            StorageFile file = await storageFolder.GetFileAsync(ACCOUNTCACHE);
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
            StorageFile file = await storageFolder.CreateFileAsync(ACCOUNTCACHE, CreationCollisionOption.ReplaceExisting);
            if(accountData != null && file != null)
                await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(accountData));
            callback.Invoke(null);
        }

        private void LoadToken()
        {
            var credentials = passwordVault.Retrieve(PWVAULT_RES, PWVAULT_USER);
            credentials.RetrievePassword();
            string _token = credentials.Password;
            if (_token == null)
                SetToken("");
            SetToken(credentials.Password);
        }

        private void SetToken(string _token)
        {
            token = _token;
            passwordVault.Add(new PasswordCredential(PWVAULT_RES, PWVAULT_USER, token));
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

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
                callback.Invoke(JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync()), null);
            else
            {
                if((int)response.StatusCode == 422) // Http Status 422: Unprocessable Entity (Backend uses this to throw errors)
                    callback.Invoke(default(T), JsonConvert.DeserializeObject<ErrorModel>(await response.Content.ReadAsStringAsync()));
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

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
                callback.Invoke(null);
            else
            {
                if ((int)response.StatusCode == 422) // Http Status 422: Unprocessable Entity (Backend uses this to throw errors)
                    callback.Invoke(JsonConvert.DeserializeObject<ErrorModel>(await response.Content.ReadAsStringAsync()));
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
            return false;
        }

        public Account GetAccountInfo()
        {
            return accountData;
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
    }
}
