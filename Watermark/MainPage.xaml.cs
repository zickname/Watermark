using System.Diagnostics;
using System.Drawing;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Image = System.Drawing.Image;

namespace Watermark;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnClicked(object sender, EventArgs e)
    {
        await AddWatermarkToImageAsync();
    }
    
    public static async Task AddWatermarkToImageAsync()
    {
        try
        {
            var options = new PickOptions { FileTypes = FilePickerFileType.Images };
            var result = await FilePicker.PickAsync(options);
            string textWatermark = "aidar.kz";
            
            if (result == null)
            {
                return;
            }

            if (!result.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
                !result.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            using (var stream = await result.OpenReadAsync())
            {
                using (var image = Image.FromStream(stream))
                {
                    var watermarkedImage = new Bitmap(image.Width, image.Height);
                    using (var graphics = Graphics.FromImage(watermarkedImage))
                    {
                        graphics.DrawImage(image, 0, 0);

                        var font = new Font("Arial", 18, FontStyle.Bold);
                        var brush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
                        var textSize = graphics.MeasureString(textWatermark, font);
                        var x = (watermarkedImage.Width - textSize.Width) / 2;
                        var y = (watermarkedImage.Height - textSize.Height) / 2;

                        graphics.DrawString(textWatermark, font, brush, x, y);
                    }

                    watermarkedImage.Save(GetWatermarkedFileName(result.FullPath));
                }
            }

            return;
        }
        catch (Exception ex)
        {
           Debug.WriteLine(ex.Message);
        }
    }
    private static string GetWatermarkedFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var path = Path.GetDirectoryName(fileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var fullPath = Path.Combine(path, $"{fileNameWithoutExtension}_watermark{extension}");

        return fullPath;
    }
}