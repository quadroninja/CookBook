namespace CookBookBackend.Core.Exceptions
{
    [Serializable]
    internal class PhotoNotFoundException : Exception
    {
        public string PhotoName { get; }
        
        public PhotoNotFoundException(string photoName) : base($"Photo by name of {photoName} not found!")
        {
            PhotoName = photoName;
        }
    }
}