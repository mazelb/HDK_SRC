/**
 * @file TextViewPage.xaml.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using Xamarin.Forms;

namespace HeddokoSdkXamarin.Test.Pages
{
    public partial class TextViewPage : ContentPage
    {
        public TextViewPage()
        {
            InitializeComponent();
        }

        public void ChangeText(string text)
        {
            Device.BeginInvokeOnMainThread(() => TextLabel.Text = text);
        }
    }
}
