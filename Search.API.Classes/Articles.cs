using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.API.Classes
{
    public class Articles
    {
        public string Id { get; set; }
        public string Site { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Relatedcontentwords { get; set; }
        public string[] Cmskeywords { get; set; }
        public string[] PartNumbers { get; set; }
        public DateTime Pubdate { get; set; }
        public double? Score { get; set; }

        public void Sync(Articles existingArticle)
        {
            if (string.IsNullOrEmpty(this.Site) == true)
            {
                this.Site = existingArticle.Site;
            }

            if (string.IsNullOrEmpty(this.Title) == true)
            {
                this.Title = existingArticle.Title;
            }

            if (string.IsNullOrEmpty(this.Url) == true)
            {
                this.Url = existingArticle.Url;
            }

            if (string.IsNullOrEmpty(this.ImageUrl) == true)
            {
                this.ImageUrl = existingArticle.ImageUrl;
            }

            if (string.IsNullOrEmpty(this.Content) == true)
            {
                this.Content = existingArticle.Content;
            }

            if (string.IsNullOrEmpty(this.Description) == true)
            {
                this.Description = existingArticle.Description;
            }

            if (string.IsNullOrEmpty(this.Relatedcontentwords) == true)
            {
                this.Relatedcontentwords = existingArticle.Relatedcontentwords;
            }

            if (this.Cmskeywords == null)
            {
                this.Cmskeywords = existingArticle.Cmskeywords;
            }

            if (this.PartNumbers == null)
            {
                this.PartNumbers = existingArticle.PartNumbers;
            }

            if (this.Pubdate != null)
            {
                this.Pubdate = existingArticle.Pubdate;
            }

        }

        public override string ToString()
        {
            string result = $"*** Pub Date {this.Pubdate}, Title: {this.Title}, Url: {this.Url}\n";

            result += $"Image URL: {this.ImageUrl}\n";

            result += "CMS Keywords: ";
            foreach (string word in Cmskeywords)
            {
                result += $"{word} ";
            }
            result += "\n";

            result += "Part Numbers: ";
            foreach (string part in PartNumbers)
            {
                result += $"{part} ";
            }
            result += "\n";

            result += $"Content: {this.Content}\n";

            return result;
        }
    }
}
