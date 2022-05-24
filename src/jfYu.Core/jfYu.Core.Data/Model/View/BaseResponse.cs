using System.Text.Json;

namespace jfYu.Core.Data.Model.View
{
    public class BaseResponse<T>
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public BaseResponse()
        {
            Code = "200";
            Message = string.Empty; 
        }


        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                DefaultIgnoreCondition= System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });;
        }
    }
}
