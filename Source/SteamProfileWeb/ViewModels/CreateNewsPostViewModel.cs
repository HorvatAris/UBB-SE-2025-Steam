using BusinessLayer.Models;

namespace SteamProfileWeb.ViewModels
{
    public class CreateNewsPostViewModel
    {
        public int? PostId {  get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsEditMode => PostId.HasValue;
        public string FormTitle => IsEditMode ? "Edit Post" : "Create Post";
        public string SubmitButtonText => IsEditMode ? "Save" : "Post";
    }
}
