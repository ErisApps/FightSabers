using System;
using FightSabers.Models.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FightSabers.Models.Converters
{
    public class QuestConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IQuest);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObj = JObject.Load(reader);
            return JsonConvert.DeserializeObject(jObj.ToString(), Type.GetType(jObj["questType"].ToString()));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
