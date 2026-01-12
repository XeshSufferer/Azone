namespace Azone.Catalog.Models;

public class LogoUrl
{
    public string Url { get; set; }
    public int ShopId { get; set; }
    public LogoUrl(string url, int shopId)
    {
        Url = url;
    }
    
    public LogoUrl() { }
}