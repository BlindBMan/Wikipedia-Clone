using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Wikipedia_Clone.Models
{
    public class Article
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        // [Required(ErrorMessage = "Category is mandatory")]
        // public int CategoryId { get; set; }


    }
}