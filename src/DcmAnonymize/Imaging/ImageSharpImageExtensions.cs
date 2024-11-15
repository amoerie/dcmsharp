using FellowOakDicom.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace DcmAnonymize.Imaging;

/// <summary>
/// Convenience class for non-generic access to <see cref="ImageSharpImage"/> image objects.
/// </summary>
public static class ImageSharpImageExtensions
{

    /// <summary>
    /// Convenience method to access ImageSharpImage <see cref="IImage"/> instance as ImageSharp image.
    /// </summary>
    /// <param name="image"><see cref="IImage"/> object.</param>
    /// <returns><see cref="Image"/> contents of <paramref name="image"/>.</returns>
    public static Image<Bgra32> AsSharpImage(this IImage image)
    {
        return (image as ImageSharpImage)!.RenderedImage;
    }

}
