namespace Newsy.Persistence.Models;

public class DbObject<Tid>
    where Tid : notnull
{
    public Tid Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public bool IsDeleted() => DeletedAt.HasValue;

    public DateTimeOffset PrepareUpdate(string? updatedBy = default)
    {
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = updatedBy;

        if (CreatedAt == DateTimeOffset.MinValue)
        {
            CreatedAt = UpdatedAt;
            CreatedBy = updatedBy;
        }

        return UpdatedAt;
    }

    public DateTimeOffset PrepareDelete(string? deletedBy = default)
    {
        DeletedAt = PrepareUpdate(deletedBy);
        DeletedBy = deletedBy;
        return DeletedAt.Value;
    }
}
