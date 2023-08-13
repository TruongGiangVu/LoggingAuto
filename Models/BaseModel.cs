using Newtonsoft.Json;

namespace LoggingAuto.Models;

public class BaseModel<T>
    {
    public override string? ToString() => JsonConvert.SerializeObject(this);
    public T ShallowCopy() => (T)MemberwiseClone();
}