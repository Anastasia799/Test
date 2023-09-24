using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ApplicationDbContext = BlogApi.Database.ApplicationDbContext;

namespace BlogApi.Services;

public class BlogService : PostService.PostServiceBase {
    private readonly ApplicationDbContext _dbContext;

    public BlogService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<Post> CreatePost(CreatePostRequest request, ServerCallContext context)
    {
        var post = new Post
        {
            Title = request.Title,
            Body = request.Body,
            UserId = request.UserId
        };
        _dbContext.Posts.Add(post);
        await _dbContext.SaveChangesAsync();
        return post;
    }

    public override async Task<Post?> UpdatePost(UpdatePostRequest request, ServerCallContext context)
    {
        var post = await _dbContext.Posts.FindAsync(request.Id);
        if (post != null)
        {
            post.Title = request.Title;
            post.Body = request.Body;
            await _dbContext.SaveChangesAsync();
        }
        return post;
    }

    public override async Task<GetPostResponse> GetPost(GetPostRequest request, ServerCallContext context)
    {
        var post = await _dbContext.Posts.FindAsync(request.Id);
        return new GetPostResponse { Post = post };
    }

    public override async Task<ListPostsResponse> ListPosts(ListPostsRequest request, ServerCallContext context) {
        var posts = await _dbContext.Posts.ToListAsync();
        return new ListPostsResponse { Posts = { posts } };
    }

    public override async Task<DeletePostResponse> DeletePost(DeletePostRequest request, ServerCallContext context) {
        var post = await _dbContext.Posts.FindAsync(request.PostId);
        if (post == null) return new DeletePostResponse();
        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync();
        return new DeletePostResponse();
    }

    public override async Task<CreateCommentResponse> CreateComment(CreateCommentRequest request, ServerCallContext context)
    {
        var comment = new Comment() {
            PostId = request.PostId,
            UserId = request.UserId
        };
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
        return new CreateCommentResponse { CommentId = comment.Id };
    }
}