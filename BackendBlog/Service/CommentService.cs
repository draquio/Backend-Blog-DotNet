using AutoMapper;
using BackendBlog.DTO.Comment;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service.Interface;
using BackendBlog.Validators;
using BackendBlog.Validators.Pagination;

namespace BackendBlog.Service
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public CommentService(ICommentRepository commentRepository, IPostRepository postRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _mapper = mapper;
        }

        public async Task<List<CommentDetailDto>> GetPagedComments(int page, int pageSize)
        {
            try
            {
                PaginationValidator.ValidatePage(page);
                PaginationValidator.ValidatePageSize(pageSize);
                List<Comment> comments = await _commentRepository.GetPagedComments(page, pageSize);
                if (comments == null) return new List<CommentDetailDto>();
                List<CommentDetailDto> commentDetailDtos = _mapper.Map<List<CommentDetailDto>>(comments);
                return commentDetailDtos;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the list of Comments: {ex.Message}", ex);
            }
        }
        public async Task<List<CommentDetailDto>> GetPagedCommentsByPostId(int id, int page, int pageSize, bool? isApproved)
        {
            try
            {
                PaginationValidator.ValidatePage(page);
                PaginationValidator.ValidatePageSize(pageSize);
                GenericValidator.ValidateId(id);
                Post post = await _postRepository.GetById(id);
                if(post == null) throw new KeyNotFoundException($"Post not found");
                List<Comment> comments = await _commentRepository.GetPagedCommentsByPostId(id, page, pageSize, isApproved);
                if (comments == null) return new List<CommentDetailDto>();
                List<CommentDetailDto> commentDetailDtos = _mapper.Map<List<CommentDetailDto>>(comments);
                return commentDetailDtos;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the list of Comments: {ex.Message}", ex);
            }
        }
        public async Task<CommentDetailDto> Create(CommentCreateDto commentCreateDto)
        {
            try
            {
                Comment comment = _mapper.Map<Comment>(commentCreateDto);
                Comment commentCreated = await _commentRepository.Create(comment);
                if(commentCreated == null || commentCreated.Id == 0) throw new InvalidOperationException("Comment couldn't be created");
                CommentDetailDto commentDetailDto = _mapper.Map<CommentDetailDto>(comment);
                return commentDetailDto;
            }
            catch(InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the list of Comments: {ex.Message}", ex);
            }
        }
        public async Task<bool> Update(CommentUpdateDto commentUpdateDto)
        {
            try
            {
                Comment comment = await _commentRepository.GetById(commentUpdateDto.Id);
                if (comment == null) throw new KeyNotFoundException($"Comment with ID {commentUpdateDto.Id} not found");
                _mapper.Map(commentUpdateDto, comment);
                bool response = await _commentRepository.Update(comment);
                if (!response) throw new InvalidOperationException("Category couldn't be updated");
                return response;
            }
            catch (KeyNotFoundException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating the comment: {ex.Message}", ex);
            }
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                GenericValidator.ValidateId(id);
                Comment comment = await _commentRepository.GetById(id);
                if (comment == null) throw new KeyNotFoundException($"Comment with ID {id} not found");
                bool response = await _commentRepository.Delete(comment);
                if (!response) throw new InvalidOperationException("Comment couldn't be updated");
                return response;
            }
            catch (KeyNotFoundException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting the comment: {ex.Message}", ex);
            }
        }

        public async Task<bool> ApproveComment(int id)
        {
            try
            {
                GenericValidator.ValidateId(id);
                Comment comment = await _commentRepository.GetById(id);
                if (comment == null) throw new KeyNotFoundException($"Comment with ID {id} not found");
                comment.IsApproved = true;
                bool response = await _commentRepository.Update(comment);
                if (!response) throw new InvalidOperationException("Comment couldn't be updated");
                return response;
            }
            catch (KeyNotFoundException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred: {ex.Message}", ex);
            }
        }
    }
}
