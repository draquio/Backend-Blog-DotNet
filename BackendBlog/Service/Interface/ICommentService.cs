using BackendBlog.DTO.Comment;

namespace BackendBlog.Service.Interface
{
    public interface ICommentService
    {
        Task<List<CommentDetailDto>> GetPagedComments(int page, int pageSize);
        Task<List<CommentDetailDto>> GetPagedCommentsByPostId(int id, int page, int pageSize, bool? isApproved);
        Task<CommentDetailDto> Create(CommentCreateDto commentCreateDto);
        Task<bool> Update(CommentUpdateDto commentUpdateDto);
        Task<bool> Delete(int id);
        Task<bool> ApproveComment(int id);
    }
}
