/**
 * @file Kit.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
namespace HeddokoSdkXamarin.Models
{
    public class Kit : BaseModel
    {
        public string IDView { get; set; }

        public Brainpack Brainpack { get; set; }
    }
}
