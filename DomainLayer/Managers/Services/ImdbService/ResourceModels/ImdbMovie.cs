namespace DomainLayer.Managers.Services.ImdbService.ResourceModels
{
    internal sealed class ImdbMovie
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Category { get; set; }
        public int Year { get; set; }
    }
}
