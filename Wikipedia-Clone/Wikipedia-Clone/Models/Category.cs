using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Wikipedia_Clone.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Title required")]
        public string CategoryTitle { get; set; }

        // TODO: ICollection to be added
    }
}