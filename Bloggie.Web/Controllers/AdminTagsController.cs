using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminTagsController : Controller
    {
        private readonly ITagRepository tagRepositories;

        public AdminTagsController(ITagRepository tagRepositories)
        {
            this.tagRepositories = tagRepositories;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {
            ValidateAddTagRequest(addTagRequest);

            if (!ModelState.IsValid)
            {
                return View();
            }

            //Mapping AddTagRequest to Tag domain model
            var tag = new Tag()
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.Displayname,
            };

            await tagRepositories.AddAsync(tag);

            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            //use dbContext to read the tags
            var tags = await tagRepositories.GetAllAsync();

            return View(tags);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {   
            var tag = await tagRepositories.GetAsync(id);

            if (tag != null)
            {
                var editTagRequest = new EditTagRequest()
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName
                };

                return View(editTagRequest);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
        {
            var tag = new Tag()
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };

            var updatedTag = await tagRepositories.UpdateAsync(tag);

            if (updatedTag != null)
            {
                // Show success notification
            }
            else
            {
                // Show failure notification
            }
            return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {
            var deletedTag = await tagRepositories.DeleteAsync(editTagRequest.Id);

            if (deletedTag != null)
            {   
                // Show success notification
                return RedirectToAction("List");
            }
                
            // Show failure notification
            return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }

        private void ValidateAddTagRequest(AddTagRequest addTagRequest)
        {
            if(addTagRequest != null && addTagRequest.Displayname != null)
            {
                if(addTagRequest.Name == addTagRequest.Displayname)
                {
                    ModelState.AddModelError("Displayname", "Displayname cannot be the same as name");
                }
            }
        }
    }
}
