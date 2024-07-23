using BackendBlog.DTO.Image;
using FluentValidation;

namespace BackendBlog.Validators.Image
{
    public class ImageUploadValidator : AbstractValidator<ImageUploadDto>
    {
        private static readonly HashSet<string> AllowedExtensions = new HashSet<string>
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp"
        };

            private static readonly HashSet<string> AllowedMimeTypes = new HashSet<string>
        {
            "image/jpeg", "image/png", "image/gif", "image/bmp", "image/tiff", "image/webp"
        };
        public ImageUploadValidator()
        {
            RuleFor(x => x.File)
            .NotNull().WithMessage("Image is required.")
            .Must(BeAValidFileType).WithMessage("Only image files are allowed.")
            .Must(file => file.Length > 0).WithMessage("File cannot be empty.");
        }
        private bool BeAValidFileType(IFormFile file)
        {
            if (file == null) return false;

            var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
            var mimeType = file.ContentType.ToLower();

            return AllowedExtensions.Contains(fileExtension) && AllowedMimeTypes.Contains(mimeType);
        }
    }
}
