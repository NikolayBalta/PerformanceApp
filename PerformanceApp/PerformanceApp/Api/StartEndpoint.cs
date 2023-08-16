using PerformanceApp.Utilites;

namespace PerformanceApp.Api
{
    public class StartEndpoint
    {
        public IResult GetChallenge()
        {
            var responseObject = new { message = "The challenge accepted!!!" };
            return Results.Json(responseObject);
        }
    }
}
