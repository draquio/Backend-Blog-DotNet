﻿namespace BackendBlog.DTO.Post
{
    public class PostUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public int UserId { get; set; }
        public int? ImageId { get; set; }
        public List<int>? CategoryIds { get; set; }
        public List<string>? Tags { get; set; }
    }
}

