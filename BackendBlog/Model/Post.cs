﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BackendBlog.Model
{
    public partial class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public int UserId { get; set; }
        public int? ImageId {  get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Published { get; set; } = true;
        public virtual List<string>? Tags { get; set; } = new List<string>();
        public virtual User User { get; set; }
        public virtual Image Image { get; set; }
        public virtual ICollection<PostCategory>? PostCategories { get; set; } = new List<PostCategory>();
        public virtual ICollection<Comment>? Comments { get; set; } = new List<Comment>();

    }
}
