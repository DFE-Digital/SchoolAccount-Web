using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Features.Feedback;

public class FeedbackController : Controller
{
    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> Send(string message)
    // {
    //     await _container.CreateIfNotExistsAsync();
    //     var blob = _container.GetBlobClient($"{Guid.NewGuid()}.txt");
    //     await blob.UploadAsync(BinaryData.FromString(message));
    //     return Ok(new { success = true });
    // }
}
