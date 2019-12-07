﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wikipedia_Clone.Models
{
    public class Article
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        // previous contents
        [AllowHtml]
        public string LastContent { get; set; }

        public DateTime Date { get; set; }

        // Protects against unregistered editing
        public bool Protected { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

    }
}