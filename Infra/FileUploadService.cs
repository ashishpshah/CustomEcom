using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using static System.Net.Mime.MediaTypeNames;

namespace JewelryStore.Infra
{
	public static class FileUploadService
	{
		public static async Task<string> UploadImageAsync(IFormFile file, string folderPath, int thumbWidth = 0, int thumbHeight = 0)
		{
			if (file == null || file.Length == 0) return null;

			if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

			string extension = Path.GetExtension(file.FileName);
			string fileName = Guid.NewGuid().ToString() + extension;
			string filePath = Path.Combine(folderPath, fileName);

			DeleteOldImage(filePath);

			using (var image = await SixLabors.ImageSharp.Image.LoadAsync(file.OpenReadStream()))
			{
				if (thumbWidth > 0 && thumbHeight > 0)
					image.Mutate(x => x.Resize(new ResizeOptions
					{
						Mode = ResizeMode.Max,
						Size = new Size(thumbWidth, thumbHeight)
					}));

				await image.SaveAsync(filePath);
			}

			int index = folderPath.IndexOf("wwwroot", StringComparison.OrdinalIgnoreCase);

			string webRoot = folderPath.Substring(0, index + 7);

			string relativePath = filePath.Replace(webRoot, "").Replace("\\", "/");

			return relativePath;
		}

		public static void DeleteOldImage(string imagePath)
		{
			if (string.IsNullOrEmpty(imagePath)) return;

			string fullPath = Path.Combine(AppHttpContextAccessor.WebRootPath, imagePath.TrimStart('/'));

			if (File.Exists(fullPath)) File.Delete(fullPath);
		}
	}
}
