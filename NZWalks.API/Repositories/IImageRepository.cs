using NZWalks.API.Models.Entites;

namespace NZWalks.API.Repositories
{
    public interface IImageRepository
    {
        Task<Image> UploadImageAsync(Image image);
    }
}
