/**
 * @file BaseModel.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
namespace HDK.Models
{
    public class BaseModel
    {
        public int ID { get; set; }

        public ErrorCollection Errors { get; set; }

        public bool IsOk => Errors == null;
    }
}
