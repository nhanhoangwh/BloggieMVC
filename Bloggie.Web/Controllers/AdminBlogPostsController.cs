using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminBlogPostsController : Controller
    {
        private readonly ITagRepository tagRepositories;
        private readonly IBlogPostRepository blogPostRepository;

        public AdminBlogPostsController(ITagRepository tagRepositories, IBlogPostRepository blogPostRepository)
        {
            this.tagRepositories = tagRepositories;
            this.blogPostRepository = blogPostRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            // get tags from repository
            var tags = await tagRepositories.GetAllAsync();

            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {
            //Map view model to domain model
            var blogPost = new BlogPost
            {
                Heading = addBlogPostRequest.Heading,
                PageTitle = addBlogPostRequest.PageTitle,
                Content = addBlogPostRequest.Content,
                ShortDescription = addBlogPostRequest.ShortDescription,
                FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
                UrlHandle = addBlogPostRequest.UrlHandle,
                PublishedDate = addBlogPostRequest.PublishedDate,
                Author = addBlogPostRequest.Author,
                Visible = addBlogPostRequest.Visible
            };

            //Map Tags from selected Tags
            var selectedTags = new List<Tag>();
            foreach (var selectedTagId in addBlogPostRequest.SelectedTags)
            {
                var selectedTagIdAsGuid = Guid.Parse(selectedTagId);
                var existingTag = await tagRepositories.GetAsync(selectedTagIdAsGuid);

                if (existingTag != null)
                {
                    selectedTags.Add(existingTag);
                }
            }

            //Mapping tags back to the Domain Model
            blogPost.Tags = selectedTags;

            await blogPostRepository.AddAsync(blogPost);

            return RedirectToAction("Add");
        }

        public async Task<IActionResult> List()
        {
            // Call the repository
            var blogPosts = await blogPostRepository.GetAllAsync();

            return View(blogPosts);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            // Retrieve the result from the repository
            var blogPost = await blogPostRepository.GetAsync(id);
            var tagsDomainModel = await tagRepositories.GetAllAsync();

            //Map the domain model into the view model

            if (blogPost != null)
            {
                var model = new EditBlogPostRequest
                {
                    Id = blogPost.Id,
                    Heading = blogPost.Heading,
                    PageTitle = blogPost.PageTitle,
                    Content = blogPost.Content,
                    Author = blogPost.Author,
                    Visible = blogPost.Visible,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    PublishedDate = blogPost.PublishedDate,
                    ShortDescription = blogPost.ShortDescription,
                    UrlHandle = blogPost.UrlHandle,
                    Tags = tagsDomainModel.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }),
                    SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray()
                };

                return View(model);
            }

            // Pass data to View
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
            // Map view model back to domain model
            var blogPostDomainModel = new BlogPost
            {
                Id = editBlogPostRequest.Id,
                Heading = editBlogPostRequest.Heading,
                Author = editBlogPostRequest.Author,
                Content = editBlogPostRequest.Content,
                FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
                UrlHandle= editBlogPostRequest.UrlHandle,
                PageTitle=editBlogPostRequest.PageTitle,
                PublishedDate=editBlogPostRequest.PublishedDate,   
                ShortDescription=editBlogPostRequest.ShortDescription,
                Visible = editBlogPostRequest.Visible
            };

            // Map tags into domain model
            var selectedTags = new List<Tag>();
            foreach( var selectedTagId in editBlogPostRequest.SelectedTags )
            {
                var selectedTagIdAsGuid = Guid.Parse( selectedTagId );
                var existingTag = await tagRepositories.GetAsync(selectedTagIdAsGuid);
                if ( existingTag != null )
                {
                    selectedTags.Add( existingTag );
                }
            }

            blogPostDomainModel.Tags = selectedTags;

            // Submit information to repository to update
            var updatedBLogPost = await blogPostRepository.UpdateAsync(blogPostDomainModel);

            if (updatedBLogPost != null)
            {
                // Show success notification
            }
            else
            {
                // Show failure notification
            }

            // Redirect to Get

            return RedirectToAction("Edit", new { id = editBlogPostRequest.Id});
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {   
            // Talk to repository to delete this blog post and tags
            var deletedBlogPost = await blogPostRepository.DeleteAsync(editBlogPostRequest.Id);

            if (deletedBlogPost != null)
            {
                // Show success notificaation
                return RedirectToAction("List");
            }

            // Show failure notification
            return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });
        }
    }
}
