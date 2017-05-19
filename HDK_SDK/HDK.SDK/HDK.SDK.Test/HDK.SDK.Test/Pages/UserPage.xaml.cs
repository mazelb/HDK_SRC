/**
 * @file UserPage.xaml.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HDK.SDK.Interfaces;
using HDK.Models;
using HDK.Models.Activity;
using HDK.Models.Enum;
using HDK.Models.Requests;
using HDK.Models.Streaming;
using HeddokoSdkXamarin.Test.Interfaces;
using HeddokoSdkXamarin.Test.Models;
using Microsoft.AspNet.SignalR.Client;
using Xamarin.Forms;
using HDK.SDK;

namespace HeddokoSdkXamarin.Test.Pages
{
    public partial class UserPage
    {
        private static string StreamDataFileName => "MovementLog.dat";

        private readonly HeddokoClient _client;

        private readonly User _user;
        private readonly string _deviceToken;
        private readonly IFileManager _fileManager;

        private static readonly object LockObj = new object();
        private static int _recievedPartsCount;

        public UserPage(HeddokoClient client, User user, IFileManager fileManager, string deviceToken)
        {
            InitializeComponent();

            _client = client;
            _user = user;
            _fileManager = fileManager;
            _deviceToken = deviceToken;

            _client.ReconnectionAttemptsExpired += ClientOnReconnectionAttemptsExpired;
            _client.AddStateChangeListener(ClientOnStateChange);
        }

        private void ClientOnReconnectionAttemptsExpired()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                DisplayAlert("Reconnection Attempts Expired", "Reconnection Attempts Expired. Client is disconnected", "Ok");

                DisableButtonsOnCloseConnection();
            });
        }

        private void ClientOnStateChange(StateChange stateChange)
        {
            Device.BeginInvokeOnMainThread(() => DisplayAlert("Connection state changed", $"Connection state changed to {stateChange.NewState}", "Ok"));
        }

        private void EnableButtonsOnOpenConnection()
        {
            OpenConnectionButton.IsEnabled = false;
            SendStreamButton.IsEnabled = true;
            ReceiveStreamButton.IsEnabled = true;
            SubscribeOnNotificationsButton.IsEnabled = true;
            CloseConnectionButton.IsEnabled = true;
        }

        private void DisableButtonsOnCloseConnection()
        {
            OpenConnectionButton.IsEnabled = true;
            SendStreamButton.IsEnabled = false;
            ReceiveStreamButton.IsEnabled = false;
            SubscribeOnNotificationsButton.IsEnabled = false;
            CloseConnectionButton.IsEnabled = false;
        }

        private async void TestProfile(object sender, EventArgs e)
        {
            try
            {
                User profile = await _client.Profile();

                DisplayDone(profile, "TestProfile");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private async void TestUploadRecord(object sender, EventArgs e)
        {
            try
            {
                if (_user.Kit != null)
                {
                    var request = new RecordRequest
                    {
                        //Label = "SN0001", //optional should be set Kit Label or Kit ID or Brainpack Label - that settings should be useful when you parse files from sd cards mostly only for Data analyst uploading
                        KitID = _user.Kit.ID, //optional
                        Files = new List<AssetFile>
                        {
                            new AssetFile { FileName = "tes t.txt", Type = AssetType.ProcessedFrameData },
                            new AssetFile { FileName = "test.txt", Type = AssetType.AnalysisFrameData },
                            new AssetFile { FileName = "test.txt", Type = AssetType.RawFrameData }
                        }
                    };

                    Record record = await _client.UploadRecord(request);

                    DisplayDone(record, "TestUploadRecord");
                }
                else
                {
                    await DisplayAlert("TestUploadRecord Done", "Kit is null", "Ok");
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private async void TestRecordCollection(object sender, EventArgs e)
        {
            try
            {
                if (_user.RoleType == UserRoleType.Analyst)
                {
                    ListCollection<User> users = await _client.UsersCollection(new ListRequest { Take = 10, Skip = 0 });
                    DisplayDone(users, "TestUserCollection");

                    ListCollection<Record> records = await _client.RecordsCollection(new UserRecordListRequest { UserID = _user.ID, Take = 10, Skip = 0 });
                    DisplayDone(records, "TestRecordCollection");
                }
                else
                {
                    await DisplayAlert("Unauthorized", "User role type must be Analyst", "Ok");
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private async void TestFirmware(object sender, EventArgs e)
        {
            try
            {
                Firmware software = await _client.CheckLastSofware();

                if (software != null && !software.IsOk)
                {
                    DisplayError(software);
                }

                Firmware firmware = await _client.CheckLastFirmware(new FirmwareRequest { Type = FirmwareType.Databoard });

                if (firmware != null && !firmware.IsOk)
                {
                    DisplayError(software);
                }

                if (!string.IsNullOrEmpty(firmware?.Name))
                {
                    await _client.DownloadFile(firmware.Url, firmware.Name);

                    byte[] downloadedBytes = _fileManager.ReadAllBytes(firmware.Name);
                    string result = Encoding.UTF8.GetString(downloadedBytes, 0, downloadedBytes.Length);

                    await DisplayAlert("Downloaded last firmware", $"Content: {result.Substring(0, result.Length > 100 ? 100 : result.Length)}", "Ok");

                    Firmware updatedFirmware = await _client.UpdateFirmware(new FirmwareRequest
                    {
                        ID = 1, //id of brainpack or powerboard or databoard or sensor, 
                                //Label = "BP0001", //label of brainpack or powerboard or databoard or sensor, 
                        FirmwareID = firmware.ID
                        // make sure firmware id has the same type as ID of brainpack or powerboard or databoard or sensor
                    });

                    DisplayDone(updatedFirmware, "TestFirmware");
                }
                else
                {
                    await DisplayAlert("Last firmware", "Last firmware or it's name is null", "Ok");
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private async void TestStreamSending(object sender, EventArgs e)
        {
            try
            {
                await _client.CreateStreamConnection();

                var assestReader = DependencyService.Get<IAssestReader>();

                byte[] stream = assestReader.ReadAllBytes(StreamDataFileName);

                int partsCount = (int)Math.Ceiling((decimal)stream.Length / 100);

                var model = new TextViewModel { Text = "Sent stream parts:", Title = "Stream sending" };
                var page = new TextViewPage { BindingContext = model };

                await Navigation.PushAsync(page);

                for (int i = 0; i < partsCount; i++)
                {
                    byte[] streamPart = stream.Skip(i).Take(100).ToArray();
                    _client.SendStream(new StreamMessage { Message = streamPart, MessageType = StreamMessageType.Stream });

                    model.Text += $" {++i}";
                    page.ChangeText(model.Text);
                }

                await _client.RemoveStreamConnection();

                await DisplayAlert("Stream sent", $"{partsCount} parts of the stream was sent", "Ok");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private async void OnStreamReceiving(object sender, EventArgs e)
        {
            try
            {
                string channelName = await TestStreamChannels(_client);
                await TestStreamReceiving(_client, channelName);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private async Task<string> TestStreamChannels(HeddokoClient client)
        {
            List<Channel> connections = await client.CurrentStreamConnections();
            if (connections.Count > 0)
            {
                return connections.First().Name;
            }

            return null;
        }

        private async Task TestStreamReceiving(HeddokoClient client, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            await DisplayAlert("Subscribing to channel", $"Subscribing to channel: {name}", "Ok");

            var model = new TextViewModel { Text = "Receving stream parts:", Title = "Stream receiving" };
            var page = new TextViewPage { BindingContext = model };

            _recievedPartsCount = 0;
            await client.SubscribeOnGettingStream(name,
                    data =>
                    {
                        lock (LockObj)
                        {
                            model.Text += $" {++_recievedPartsCount}";
                            page.ChangeText(model.Text);
                        }
                    });

            await Navigation.PushAsync(page);
        }

        private async void TestSubscribeOnGettingNotifications(object sender, EventArgs e)
        {
            try
            {
                await _client.SubscribeOnGettingNotification(_deviceToken, OnNotificationReceived);

                DisplayDone("Subscribition");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void OnNotificationReceived(NotificationMessage message)
        {
            Device.BeginInvokeOnMainThread(() => DisplayAlert(message.Type.ToString(), message.Text, "Ok"));
        }

        private async void TestGetUnreadActivity(object sender, EventArgs e)
        {
            try
            {
                ListCollection<UserEvent> activity = await _client.GetUnreadActivity(new ListRequest { Take = 20, Skip = 0 });

                await ShowCollection(activity.Collection, x => $"{x.Created} {x.Type} {x.Message}", "Unread Activity");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private async void TestGetLatestActivity(object sender, EventArgs e)
        {
            try
            {
                ListCollection<UserEvent> activity = await _client.GetLatestActivity(new ListRequest { Take = 20, Skip = 0 });

                await ShowCollection(activity.Collection, x => $"{x.Created} {x.Type} {x.Message}", "Latest Activity");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private async void TestDefaultFirmwareRecords(object sender, EventArgs e)
        {
            try
            {
                ListCollection<Record> defaultRecords = await _client.GetDefaultRecords(new ListRequest { Take = 10, Skip = 0 });

                var results = new List<string>();

                for (int i = 0; i < defaultRecords.Collection.Count; i++)
                {
                    Record defaultRecord = defaultRecords.Collection[i];
                    if (defaultRecord.Assets == null || defaultRecord.Assets.Count == 0)
                    {
                        results.Add($"    {i}. There are no assets for this default record");
                        continue;
                    }

                    results.Add($"    {i}. Default record files:");

                    foreach (Asset asset in defaultRecord.Assets)
                    {
                        string fileName = Path.GetFileName(asset.Url);
                        await _client.DownloadFile(asset.Url, fileName);

                        byte[] downloadedBytes = _fileManager.ReadAllBytes(fileName);
                        string result = Encoding.UTF8.GetString(downloadedBytes, 0, downloadedBytes.Length);

                        results.Add($"{asset.Url} downloaded");
                    }
                }

                await ShowCollection(results, r => r, "Default records");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private async void OnOpenConnection(object sender, EventArgs e)
        {
            try
            {
                await _client.OpenConnection();
                EnableButtonsOnOpenConnection();

                DisplayDone("Open connection");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void OnCloseConnection(object sender, EventArgs e)
        {
            try
            {
                _client.CloseConnection();
                DisableButtonsOnCloseConnection();

                DisplayDone("Close connection");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private async void TestErgoScore(object sender, EventArgs e)
        {
            var sb = new StringBuilder();

            ErgoScore score = await _client.GetErgoScore();
            sb.AppendLine($"Your score :{score.Score}");

            ErgoScore orgScore = await _client.GetCurrentOrganizationScore();
            sb.AppendLine($"Your organization score :{orgScore.Score}");

            ErgoScore teamScore = await _client.GetTeamScore();
            sb.AppendLine($"Your team score :{teamScore.Score}");

            List<ErgoScore> teamErgoScores = await _client.GetTeamErgoScores(_user.Team.ID);

            sb.AppendLine("Team scores:");
            foreach (ErgoScore teamErgoScore in teamErgoScores)
            {
                sb.AppendLine($"User: {teamErgoScore.Id}, score :{teamErgoScore.Score}");
            }

            var model = new TextViewModel { Text = sb.ToString(), Title = "Ergo score" };
            var page = new TextViewPage { BindingContext = model };

            await Navigation.PushAsync(page);
        }
    }
}
