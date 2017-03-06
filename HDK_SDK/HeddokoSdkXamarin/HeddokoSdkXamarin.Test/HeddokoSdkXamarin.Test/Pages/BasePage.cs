/**
 * @file BasePage.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HeddokoSdkXamarin.Models;
using HeddokoSdkXamarin.Test.Models;
using Xamarin.Forms;

namespace HeddokoSdkXamarin.Test.Pages
{
    public class BasePage : ContentPage
    {
        protected void DisplayError(Exception ex)
        {
            DisplayAlert("Error", $"{ex.GetBaseException().Message}", "Ok");
        }

        protected void DisplayError(BaseModel model)
        {
            if (model.Errors.Errors != null && model.Errors.Errors.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (Error error in model.Errors.Errors)
                {
                    sb.AppendLine(error.Message);
                }

                DisplayAlert("Error", sb.ToString(), "Ok");
            }
            else
            {
                DisplayAlert("Error", "Unknown", "Ok");
            }
        }

        protected void DisplayDone(BaseModel model, string method)
        {
            if (model.IsOk)
            {
                DisplayDone(method);
            }
            else
            {
                DisplayError(model);
            }
        }

        protected void DisplayDone(string method)
        {
            DisplayAlert("Done", $"{method} done", "Ok");
        }

        protected async Task ShowCollection<T>(IEnumerable<T> collection, Func<T, string> display, string title)
        {
            StringBuilder sb = new StringBuilder();

            foreach (T item in collection)
            {
                sb.AppendLine(display(item));
            }

            var textViewPage = new TextViewPage { BindingContext = new TextViewModel { Text = sb.ToString(), Title = title } };

            await Navigation.PushAsync(textViewPage);
        }


        protected async Task ShowCollectionAsyncDisplay<T>(IEnumerable<T> collection, Func<T, Task<string>> display, string title)
        {
            StringBuilder sb = new StringBuilder();

            foreach (T item in collection)
            {
                sb.AppendLine(await display(item));
            }

            var textViewPage = new TextViewPage { BindingContext = new TextViewModel { Text = sb.ToString(), Title = title } };

            await Navigation.PushAsync(textViewPage);
        }
    }
}
