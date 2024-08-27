namespace Shortly.Client.Data.ViewModels
{
    public class GetUrlVM
    {
        public int Id { get; set; }
        public string OriginalLink { get; set; }
        public string ShortLink { get; set; }
        public int NoOfClicks { get; set; }
        public string? UserId { get; set; }

        public GetUserVM UserVM { get; set; }
    }
}
