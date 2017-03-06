/**
 * @file HttpExtentions.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HeddokoSdkXamarin.Extentions
{
    public static class HttpExtentions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            string str = await content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
