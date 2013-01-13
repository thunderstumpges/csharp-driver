﻿using System;

namespace Cassandra
{
    internal partial class TypeInterpreter
    {
        public static object ConvertFromTimestamp(TableMetadata.ColumnInfo type_info, byte[] value)
        {
            Array.Reverse(value);
            return ConversionHelper.FromUnixTime(BitConverter.ToInt64(value, 0));
        }

        public static Type GetTypeFromTimestamp(TableMetadata.ColumnInfo type_info)
        {
            return typeof(DateTimeOffset);
        }

        public static byte[] InvConvertFromTimestamp(TableMetadata.ColumnInfo type_info, object value)
        {
            return ConversionHelper.ToBytesFromInt64(ConversionHelper.ToUnixTime((DateTimeOffset)value));
        }
    }
}
