using System;
using System.Collections.Generic;

namespace DealingAdmin.Abstractions.Extensions;

public static class NumericExtension
{
    public static DateTime JsUnixTimeToDateTime(this ulong javaTimeStamp )
    {
        DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddMilliseconds( javaTimeStamp ).ToLocalTime();
        return dateTime;
    }

}