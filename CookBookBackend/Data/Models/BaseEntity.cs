namespace CookBookBackend.Data.Models
{
    public class BaseEntity
    {
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }
    }

}
