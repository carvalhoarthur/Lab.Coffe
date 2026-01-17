namespace Lab.Coffe.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    protected BaseEntity(Guid id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
