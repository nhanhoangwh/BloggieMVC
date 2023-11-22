using System.ComponentModel.DataAnnotations;

namespace Bloggie.Web.Models.ViewModels
{
    public class AddTagRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Displayname { get; set; } 
    }
}
