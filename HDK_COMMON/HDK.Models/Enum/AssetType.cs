/**
 * @file AssetType.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
namespace HDK.Models.Enum
{
    public enum AssetType
    {
        Log = 5,
        SystemLog = 6,
        Setting = 7,
        Record = 8,
        DefaultRecords = 9,
        ProcessedFrameData = 11,
        AnalysisFrameData = 12,
        RawFrameData = 13
    }
}
