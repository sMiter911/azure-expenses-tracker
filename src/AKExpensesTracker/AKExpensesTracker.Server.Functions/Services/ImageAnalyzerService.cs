using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AKExpensesTracker.Server.Functions.Services
{
    public class ImageAnalyzerService : IImageAnalyzerService
    {
        private readonly ComputerVisionClient _computerVisionClient;

        public ImageAnalyzerService(ComputerVisionClient computerVisionClient)
        {
            _computerVisionClient = computerVisionClient;
        }

        public async Task<IEnumerable<string>> ExtractImageCategoriesAsync(Stream imageStream)
        {
            var requiredFeatures = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Categories
            };
            var imageAnalysis = await _computerVisionClient.AnalyzeImageInStreamAsync(imageStream, requiredFeatures);
            return imageAnalysis.Categories.Select(c => c.Name);
        }
    }

    
}
