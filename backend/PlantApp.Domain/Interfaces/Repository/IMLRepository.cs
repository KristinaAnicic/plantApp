public interface IMLRepository
{
    public Task<List<HealthPredictionRecord>> GetHealthPredictionTrainingData();
    public Task<List<HealthPredictionRecord>> GetUserHealthPredictionInputData(int userId);
}
