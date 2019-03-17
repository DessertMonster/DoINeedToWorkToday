using Amazon;
using Amazon.CodePipeline;
using Amazon.Lambda.Core;
using System.Linq;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DoINeedToWork.Lambda
{
    public class Function
    {
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(ILambdaContext context)
        {
            LambdaLogger.Log($"{context.FunctionName} was invoked as a CodePipeline stage.");

            var client = new AmazonCodePipelineClient(RegionEndpoint.USWest2);

            var poll = await client.PollForJobsAsync(new Amazon.CodePipeline.Model.PollForJobsRequest {
                ActionTypeId = new Amazon.CodePipeline.Model.ActionTypeId
                {
                    Category = ActionCategory.Invoke,
                    Owner = ActionOwner.AWS,
                    Provider = "Lambda",
                    Version = "1"
                }
            });

            var jobId = poll.Jobs.First().Id;

            LambdaLogger.Log($"Job id is {jobId}");

            await client.PutJobSuccessResultAsync(new Amazon.CodePipeline.Model.PutJobSuccessResultRequest {
                JobId = jobId
            });
        }
    }
}
