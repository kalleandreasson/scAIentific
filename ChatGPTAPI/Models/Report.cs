namespace ChatGPTAPI.Models;
public class Report
{
    public string ArticleID { get; set; }
    public string Authors { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public string Abstract { get; set; }
    public string FullReference { get; set; }
    public string Notes { get; set; }
}