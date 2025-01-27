﻿using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Devon4Net.Infrastructure.AWS.CDK.Helpers
{
    public class JsonHelper : IJsonHelper
    {
        private const string BuiltInTypes = "String, DateTime, DateTimeKind, DateTimeOffset, AsyncCallback, AttributeTargets, AttributeUsageAttribute, Boolean, Byte, Char, CharEnumerator, Base64FormattingOptions, DayOfWeek, DBNull, Decimal, Double, EnvironmentVariableTarget, EventHandler, GCCollectionMode, Guid, Int16, Int32, Int64, IntPtr, SByte, Single, TimeSpan, TimeZoneInfo, TypeCode, UInt16, UInt32, UInt64, UIntPtr";
        private JsonSerializerOptions CamelJsonSerializerOptions { get; }

        private JsonSerializerOptions JsonSerializerOptions { get; }

        public JsonHelper()
        {
            JsonSerializerOptions = null;
            CamelJsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
        }

        public JsonHelper(JsonSerializerOptions jsonSerializerOptions)
        {
            JsonSerializerOptions = jsonSerializerOptions;
        }

        public T Deserialize<T>(string input, bool useCamelCase= false)
        {
            if (string.IsNullOrEmpty(input))
            {
                return default;
            }

            if (BuiltInTypes.Contains(typeof(T).Name))
            {
                return (T)Convert.ChangeType(input, typeof(T));
            }

            return JsonSerializer.Deserialize<T>(input, useCamelCase ? CamelJsonSerializerOptions : null);
        }

        public async Task<string> Serialize<T>(T input)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync<T>(stream, input, JsonSerializerOptions).ConfigureAwait(false);

            stream.Position = 0;
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }

        public string Serialize(object toPrint, bool useCamelCase = false)
        {
            return JsonSerializer.Serialize(toPrint, useCamelCase ? CamelJsonSerializerOptions : null);
        }
    }
}
