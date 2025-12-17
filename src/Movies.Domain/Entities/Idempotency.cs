namespace Movies.Domain.Entities;

public class Idempotency
{
    public Guid Key { get; set; }
    public long CreatedAt { get; set; } 
}
