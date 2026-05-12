namespace CookBookBackend.Core.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public string EntityName { get; }
        public int Id { get; }
     
        public EntityNotFoundException(string entityName, int id) : base($"Entity {entityName} with id {id} was not found")
        {
            EntityName = entityName;
            Id = id;
        }
    }
}
