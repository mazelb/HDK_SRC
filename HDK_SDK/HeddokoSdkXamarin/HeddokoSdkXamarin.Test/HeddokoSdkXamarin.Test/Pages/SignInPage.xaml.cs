/**
 * @file SignInPage.xaml.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System;
using System.Threading.Tasks;
using HeddokoSdkXamarin.Interfaces;
using HeddokoSdkXamarin.Models;
using HeddokoSdkXamarin.Models.Enum;
using HeddokoSdkXamarin.Models.Requests;
using Xamarin.Forms;

namespace HeddokoSdkXamarin.Test.Pages
{
    public partial class SignInPage
    {
        private static string ServerUrl => "http://dev.app.heddoko.com/";
        private static string TestUserName => "tester";
        private static string TestUserPassword => "adg$1gda";
        private static string LicenseUniversalUserName => "heddoko.universal";
        private static string LicenseUniversalPassword => "H3dd0k0_universal$";
        private static int ReconnectionAttempts => 10;
        private static int ReconnectionDelay => 2000;

        public SignInPage()
        {
            InitializeComponent();
        }

        private async Task OnSignIn(UserRequest request)
        {
            try
            {
                HeddokoConfig config = new HeddokoConfig(ServerUrl, "HEDFstcKsx0NHjPSsjfSDJdsDkvdfdkFJPRGldfgdfgvVBrk", ReconnectionAttempts, ReconnectionDelay);
                IFileManager fileManager = DependencyService.Get<IFileManager>();
                HeddokoClient client = new HeddokoClient(config, fileManager);
                User user = await client.SignIn(request);

                if (user.IsOk)
                {
                    await TestUser(user);

                    Page page;

                    if (user.RoleType == UserRoleType.LicenseUniversal)
                    {
                        page = new LicenseUniversalPage(client);
                    }
                    else
                    {
                        // You should generate token at the first signin to desktop app and add it to available user's devices
                        string deviceToken = client.GenerateDeviceToken();
                        await TestAddDevice(client, deviceToken);

                        //If you don't want to receive notifications you can remove your desktop app from available user's devices
                        //_client.RemoveDevice(token)

                        page = new UserPage(client, user, fileManager, deviceToken);
                    }

                    await Navigation.PushAsync(page);
                }
                else
                {
                    DisplayError(user);
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private async Task TestUser(User user)
        {
            LicenseInfo license = user.LicenseInfo;

            await DisplayAlert("Sign in", $"You signed in successfully. License: {license.Name}", "Ok");

            if (user.RoleType != UserRoleType.LicenseUniversal)
            {
                Kit kit = user.Kit;

                Brainpack brainpack = user.Kit.Brainpack;
            }
        }

        private async Task TestAddDevice(HeddokoClient client, string deviceToken)
        {
            string message = await client.AddDevice(deviceToken)
                ? "Device was added successfully"
                : "Something went wrong on adding device";

            await DisplayAlert("AddDevice", message, "Ok");
        }

        private async void OnSignInUser(object sender, EventArgs e)
        {
            await OnSignIn(new UserRequest
            {
                Username = TestUserName,
                Password = TestUserPassword
            });
        }

        private async void OnSignInLicenseUniversal(object sender, EventArgs e)
        {
            await OnSignIn(new UserRequest
            {
                Username = LicenseUniversalUserName,
                Password = LicenseUniversalPassword
            });
        }
    }
}
