using Ecommerce.Models;

namespace Ecommerce.Helpers
{
	public class FileHelpers
	{
		public static string GetImageUrl(string folderPath, IFormFile file, IWebHostEnvironment webHostEnvironment)
		{
			folderPath += Guid.NewGuid() + file.FileName;
			string serverFolderPath = Path.Combine(webHostEnvironment!.WebRootPath, folderPath);

			using (var newFile = new FileStream(serverFolderPath, FileMode.Create))
			{
				file.CopyTo(newFile);
			}

			return "/" + folderPath;
		}

		public static bool DeleteImageFromWWWRoot(MyImage image, IWebHostEnvironment webHostEnvironment)
		{
			string path = image.ImageURL!.Remove(0, 1);
			path = Path.Combine(webHostEnvironment!.WebRootPath, path);

			if (System.IO.File.Exists(path))
			{
				System.IO.File.Delete(path);
				return true;
			}

			return false;
		}
	}
}
