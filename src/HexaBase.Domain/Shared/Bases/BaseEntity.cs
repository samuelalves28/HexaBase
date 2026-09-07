namespace HexaBase.Domain.Shared.Bases;

public abstract class BaseEntity
{
    #region Builders

    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }

    #endregion Builders

    #region Properties

    public int Id { get; private set; }
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; } = null;

    #endregion Properties

    #region Public Methods

    public void SetId(int id)
    {
        Id = id;
    }

    public void ChangeLastUpdate()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    #endregion Public Methods
}