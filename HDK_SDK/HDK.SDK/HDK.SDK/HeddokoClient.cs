/**
 * @file HeddokoClient.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using HDK.SDK.Extentions;
using HDK.SDK.Interfaces;
using HDK.Models;
using HDK.Models.Activity;
using HDK.Models.Enum;
using HDK.Models.Requests;
using HDK.Models.Streaming;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;

namespace HDK.SDK
{
    public class HeddokoClient
    {
        private const string ApplicationJson = "application/json";
        private const string StreamingHubName = "StreamingHub";
        private const string NotificationsHubName = "NotificationsHub";
        private const string TokenHeaderName = "Authorization";
        private const string Bearer = "Bearer ";

        private readonly HubConnection _hubConnection;
        private readonly IHubProxy _streamindHubProxy;
        private readonly IHubProxy _notificationsHubProxy;

        private readonly IFileManager _fileManager;

        private int _reconnectionAttempts;

        public event Action<Exception> OnError;
        public event Action ReconnectionAttemptsExpired;

        public HeddokoClient(HeddokoConfig config, IFileManager fileManager, string token = null)
        {
            Config = config;
            Token = token;

            _hubConnection = new HubConnection(Config.ServerUrl);
            _streamindHubProxy = _hubConnection.CreateHubProxy(StreamingHubName);
            _notificationsHubProxy = _hubConnection.CreateHubProxy(NotificationsHubName);

            _reconnectionAttempts = 0;
            _hubConnection.StateChanged += HubConnectionOnStateChanged;

            _fileManager = fileManager;

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                OnError?.Invoke(e.Exception.GetBaseException());

                e.SetObserved();
            };
        }

        private void HubConnectionOnStateChanged(StateChange stateChange)
        {
            if ((stateChange.OldState == ConnectionState.Reconnecting || stateChange.OldState == ConnectionState.Connecting) && stateChange.NewState == ConnectionState.Disconnected)
            {
                if (_reconnectionAttempts >= Config.ReconnectionAttempts)
                {
                    ReconnectionAttemptsExpired?.Invoke();
                    return;
                }

                _reconnectionAttempts++;

                Task.Delay(Config.ReconnectionDelay).ContinueWith(_ => ReopenConnection());
            }
        }

        private HeddokoConfig Config { get; }

        private string Token { get; set; }

        #region Private methods

        private async Task<T> SendPostXWwwForm<T>(UserRequest request)
        {
            using (var client = new HttpClient())
            {
                var postData = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "username", Uri.EscapeUriString(request.Username) },
                    { "password", Uri.EscapeUriString(request.Password) }
                };

                var content = new FormUrlEncodedContent(postData);
                HttpResponseMessage response = await client.PostAsync(Config.OauthUrl, content).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>();
                }

                ErrorOauth error = await response.Content.ReadAsAsync<ErrorOauth>();

                throw new Exception(error.ErrorDescription);
            }
        }

        private static StringContent GetJsonStringContent(object request)
        {
            var json = JsonConvert.SerializeObject(request);
            var stringContent = new StringContent(json, Encoding.UTF8, ApplicationJson);
            return stringContent;
        }

        private async Task<T> Send<T>(string url, HttpType type = HttpType.GET, object request = null, bool allowToken = true, string[] filenames = null)
        {
            try
            {
                url = Config.APIUrl + url;
                using (var client = new HttpClient())
                {
                    if (allowToken && !string.IsNullOrEmpty(Token))
                    {
                        client.DefaultRequestHeaders.Add(TokenHeaderName, Bearer + Token);
                    }

                    HttpResponseMessage response = null;
                    switch (type)
                    {
                        case HttpType.GET:
                            response = await client.GetAsync(url).ConfigureAwait(false);

                            break;
                        case HttpType.POST:
                            if (filenames != null)
                            {
                                response = await Upload(client, filenames, request, url);
                            }
                            else
                            {
                                StringContent stringContent = GetJsonStringContent(request);

                                response = await client.PostAsync(url, stringContent).ConfigureAwait(false);
                            }
                            break;
                        case HttpType.PUT:
                            StringContent content = GetJsonStringContent(request);

                            response = await client.PutAsync(url, content).ConfigureAwait(false);
                            break;
                        case HttpType.DELETE:
                            response = await client.DeleteAsync(url).ConfigureAwait(false);
                            break;
                    }

                    if (response == null)
                    {
                        throw new Exception("Response is null");
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsAsync<T>();
                    }

                    throw new Exception($"Error {response.StatusCode}: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                //TODO Add error log
                throw ex;
            }
        }

        private Dictionary<string, string> ConvertObject(object obj)
        {
            var formData = new Dictionary<string, string>();

            TypeInfo typeInfo = obj.GetType().GetTypeInfo();
            foreach (PropertyInfo pi in typeInfo.DeclaredProperties)
            {
                formData.Add(pi.Name, JsonConvert.SerializeObject(pi.GetValue(obj, null) ?? string.Empty));
            }

            return formData;
        }

        private async Task<HttpResponseMessage> Upload(HttpClient client, string[] fileNames, object form, string url)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            using (var content = new MultipartFormDataContent(boundary))
            {
                foreach (string fileName in fileNames)
                {
                    byte[] file = _fileManager.ReadAllBytes(fileName);
                    content.Add(new StreamContent(new MemoryStream(file)), "file", fileName);
                }

                Dictionary<string, string> formData = ConvertObject(form);

                foreach (var keyValuePair in formData)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }

                return await client.PostAsync(url, content);
            }
        }

        private string BuildParameters(string url, ListRequest request)
        {
            url += $"/{request.Take}";

            if (request.Skip.HasValue)
            {
                url += $"/{request.Skip.Value}";
            }

            return url;
        }

        #endregion

        public void SetToken(string token)
        {
            Token = token;
        }

        public async Task<OauthResponse> OauthToken(UserRequest request)
        {
            OauthResponse token = await SendPostXWwwForm<OauthResponse>(request);

            return token;
        }

        public async Task<User> SignIn(UserRequest request)
        {
            OauthResponse token = await OauthToken(request);

            if (token != null)
            {
                SetToken(token.AccessToken);
            }

            var f = await CheckLastSofware();

            User user = await Profile();

            return user;
        }

        public async Task<User> Profile()
        {
            User user = await Send<User>("/users/profile");

            return user;
        }

        public async Task<Firmware> CheckLastSofware()
        {
            Firmware firmware = await Send<Firmware>("/firmwares/software");

            return firmware;
        }

        public async Task<Firmware> CheckLastFirmware(FirmwareRequest request)
        {
            Firmware firmware = await Send<Firmware>("/firmwares/check", HttpType.POST, request);

            return firmware;
        }

        public async Task<Firmware> UpdateFirmware(FirmwareRequest request)
        {
            Firmware firmware = await Send<Firmware>("/firmwares/update", HttpType.POST, request);

            return firmware;
        }

        [Obsolete("Upload is obsolete, please use UploadRecord instead.")]
        public async Task<Asset> Upload(AssetRequest request, string filename)
        {
            Asset upload = await Send<Asset>("/assets/upload", HttpType.POST, request, true, new[] { filename });

            return upload;
        }

        public async Task DownloadFile(string url, string filename)
        {
            using (var client = new HttpClient())
            {
                byte[] data = await client.GetByteArrayAsync(url);
                _fileManager.CreateAndWriteFile(filename, data);
            }
        }

        public async Task<ListCollection<User>> UsersCollection(ListRequest request)
        {
            ListCollection<User> users = await Send<ListCollection<User>>(BuildParameters("/users/list", request), HttpType.GET, request);

            return users;
        }

        [Obsolete("AssetsCollection is obsolete, please use RecordsCollection instead.")]
        public async Task<ListCollection<Asset>> AssetsCollection(AssetListRequest request)
        {
            string url = "/assets/list";
            if (request.UserID.HasValue)
            {
                url += $"/{request.UserID.Value}";
            }
            ListCollection<Asset> users = await Send<ListCollection<Asset>>(BuildParameters(url, request), HttpType.GET, request);
            return users;
        }

        public async Task<ListCollection<Firmware>> FirmwareCollection(FirmwareListRequest request)
        {
            string url = $"/firmwares/list/{(int)request.Type}";

            ListCollection<Firmware> records = await Send<ListCollection<Firmware>>(BuildParameters(url, request), HttpType.GET, request);

            return records;
        }

        public async Task<ListCollection<Record>> GetDefaultRecords(ListRequest request)
        {
            return await Send<ListCollection<Record>>(BuildParameters("/records/default", request));
        }

        public async Task OpenConnection()
        {
            try
            {
                _hubConnection.Headers[TokenHeaderName] = Bearer + Token;
                await ReopenConnection();
            }
            catch (Exception e)
            {
                throw e.GetBaseException();
            }
        }

        private async Task ReopenConnection()
        {
            await _hubConnection.Start();
            _reconnectionAttempts = 0;
        }

        public void SendStream(StreamMessage stream)
        {
            try
            {
                _streamindHubProxy.Invoke("Send", stream).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        //TODO Add error log
                        OnError?.Invoke(task.Exception.GetBaseException());
                    }
                });
            }
            catch (Exception e)
            {
                throw e.GetBaseException();
            }
        }

        public async Task SubscribeOnGettingStream(string groupName, Action<StreamMessage> action)
        {
            try
            {
                await _streamindHubProxy.Invoke("JoinGroupByTeam", groupName);
                _streamindHubProxy.On("GetData", action);
            }
            catch (Exception e)
            {
                //TODO Add error log
                throw e.GetBaseException();
            }
        }

        public async Task<List<Channel>> CurrentStreamConnections()
        {
            return await Send<List<Channel>>("/stream/connections");
        }

        public async Task RemoveStreamConnection()
        {
            await Send<bool>("/stream/connections/delete");
        }

        public void CloseConnection()
        {
            _hubConnection.Stop();
        }

        public async Task<bool> CreateStreamConnection()
        {
            return await Send<bool>("/stream/connections/add", HttpType.POST);
        }

        public async Task SubscribeOnGettingNotification(string deviceId, Action<NotificationMessage> action)
        {
            try
            {
                await _notificationsHubProxy.Invoke("Subscribe", deviceId);
                _notificationsHubProxy.On("ShowNotification", action);
            }
            catch (Exception e)
            {
                //TODO Add error log
                throw e.GetBaseException();
            }
        }

        public async Task<bool> AddDevice(string deviceId)
        {
            var data = new SubscribeTokenModel { Token = deviceId, Type = DeviceType.Desktop };

            return await Send<bool>("/users/subscribe", HttpType.POST, data);
        }

        public async Task<bool> RemoveDevice(string deviceId)
        {
            var data = new UnsubscribeTokenModel { Token = deviceId };

            return await Send<bool>("/users/unsubscribe", HttpType.POST, data);
        }

        public async Task<ListCollection<UserEvent>> GetLatestActivity(ListRequest request)
        {
            return await Send<ListCollection<UserEvent>>(BuildParameters("/users/activity", request), HttpType.GET, request);
        }

        public async Task<ListCollection<UserEvent>> GetUnreadActivity(ListRequest request)
        {
            return await Send<ListCollection<UserEvent>>(BuildParameters("/users/activity/unread", request), HttpType.GET, request);
        }

        public string GenerateDeviceToken()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<ListCollection<Organization>> GetAllOrganizations(ListRequest request)
        {
            return await Send<ListCollection<Organization>>(BuildParameters("/universal/organizations", request), HttpType.GET, request);
        }

        public async Task<ListCollection<Team>> GetAllTeams(TeamListRequest request)
        {
            string url = $"/universal/organizations/{request.OrganizationId}/teams";

            return await Send<ListCollection<Team>>(BuildParameters(url, request), HttpType.GET, request);
        }

        public async Task<ListCollection<Record>> GetAllRecords(RecordListRequest request)
        {
            string url = $"/universal/teams/{request.TeamId}/records";

            return await Send<ListCollection<Record>>(BuildParameters(url, request), HttpType.GET, request);
        }

        public async Task<Record> UploadRecord(RecordRequest request)
        {
            string[] filenames = request.Files.Select(f => f.FileName).ToArray();

            foreach (AssetFile file in request.Files)
            {
                file.FileName = Path.GetFileName(file.FileName);
            }

            Record upload = await Send<Record>("/users/records/upload", HttpType.POST, request, true, filenames);

            return upload;
        }

        public async Task<ListCollection<Record>> RecordsCollection(UserRecordListRequest request)
        {
            var url = request.UserID.HasValue ? $"/users/{request.UserID.Value}/records" : "/users/records";

            ListCollection<Record> users = await Send<ListCollection<Record>>(BuildParameters(url, request), HttpType.GET, request);

            return users;
        }

        public void AddStateChangeListener(Action<StateChange> stateChangeListener)
        {
            _hubConnection.StateChanged += stateChangeListener;
        }

        public void RemoveStateChangeListener(Action<StateChange> stateChangeListener)
        {
            _hubConnection.StateChanged -= stateChangeListener;
        }

        public async Task<ErgoScore> GetErgoScore(int? userId = null)
        {
            string url = $"/ergoscore/{userId}";

            return await Send<ErgoScore>(url);
        }

        public async Task<List<ErgoScore>> GetOrganizationErgoScores(int orgId)
        {
            string url = $"/ergoscore/org/{orgId}";

            return await Send<List<ErgoScore>>(url);
        }

        public async Task<List<ErgoScore>> GetTeamErgoScores(int teamId)
        {
            string url = $"/ergoscore/team/{teamId}";

            return await Send<List<ErgoScore>>(url);
        }

        public async Task<ErgoScore> GetCurrentOrganizationScore()
        {
            string url = "/ergoscore/orgScore";

            return await Send<ErgoScore>(url);
        }

        public async Task<ErgoScore> GetTeamScore()
        {
            string url = "/ergoscore/teamScore";

            return await Send<ErgoScore>(url);
        }
    }
}