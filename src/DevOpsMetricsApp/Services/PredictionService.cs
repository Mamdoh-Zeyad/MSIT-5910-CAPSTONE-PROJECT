using DevOpsMetricsApp.Data;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.EntityFrameworkCore;

namespace DevOpsMetricsApp.Services
{
    // 1. Define the input data model
    public class MLDeploymentData
    {
        public bool IsSuccessful { get; set; }
        public float BuildDurationSeconds { get; set; }
    }

    // 2. Define the output prediction model
    public class BuildPrediction
    {
        [ColumnName("Score")]
        public float PredictedDuration { get; set; }
    }

    public class PredictionService
    {
        private readonly ApplicationDbContext _context;
        private readonly MLContext _mlContext;

        public PredictionService(ApplicationDbContext context)
        {
            _context = context;
            _mlContext = new MLContext(seed: 0); // Deterministic seed
        }

        public async Task<float> PredictNextBuildDurationAsync()
        {
            // Fetch historical data from your database
            var historicalData = await _context.DeploymentRecords
                .Select(d => new MLDeploymentData
                {
                    IsSuccessful = d.IsSuccessful,
                    BuildDurationSeconds = (float)d.BuildDurationSeconds
                }).ToListAsync();

            if (historicalData.Count < 5) return 0f; // Need minimum data to train

            // Load data into ML.NET
            IDataView trainingData = _mlContext.Data.LoadFromEnumerable(historicalData);

            // Build the Machine Learning Pipeline (Linear Regression)
            var pipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "BuildDurationSeconds")
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "IsSuccessfulEncoded", inputColumnName: "IsSuccessful"))
                .Append(_mlContext.Transforms.Concatenate("Features", "IsSuccessfulEncoded"))
                .Append(_mlContext.Regression.Trainers.Sdca());

            // Train the Model
            var model = pipeline.Fit(trainingData);

            // Create a prediction engine
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<MLDeploymentData, BuildPrediction>(model);

            // Predict duration for a typical "successful" build
            var prediction = predictionEngine.Predict(new MLDeploymentData { IsSuccessful = true });

            return prediction.PredictedDuration;
        }
    }
}