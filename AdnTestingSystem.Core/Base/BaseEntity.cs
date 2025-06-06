using System.ComponentModel.DataAnnotations;
using AdnTestingSystem.Core.Utils;

namespace AdnTestingSystem.Core.Base;

public abstract class BaseEntity
{
    protected BaseEntity()
    {
        Id = Guid.NewGuid().ToString("N");
        CreatedTime = LastUpdatedTime = CoreHelper.SystemTimeNow;
    }

    [Key]
    public string Id { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset LastUpdatedTime { get; set; }
    public DateTimeOffset? DeletedTime { get; set; }
}