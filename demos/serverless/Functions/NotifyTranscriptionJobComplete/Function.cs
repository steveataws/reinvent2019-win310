using System;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.S3;
using Amazon.S3.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace NotifyTranscriptionJobComplete
{
    public class Function
    {
        IAmazonSimpleNotificationService snsClient { get; set; }

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
            snsClient = new AmazonSimpleNotificationServiceClient();
        }

        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SNS event object and can be used 
        /// to respond to SNS messages.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(S3Event evnt, ILambdaContext context)
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LAMBDA_TASK_ROOT")))
            {
                Amazon.XRay.Recorder.Handlers.AwsSdk.AWSSDKHandler.RegisterXRayForAllServices();
            }

            var topicArn = Environment.GetEnvironmentVariable("TRANSCRIBE_TOPIC_ARN");

            foreach (var record in evnt.Records)
            {
                var bucketName = record.S3.Bucket.Name;
                var objectKey = System.Net.WebUtility.UrlDecode(record.S3.Object.Key);

                // if the object was Amazon Transcribe tempfile used to probe write permissions, skip it
                if (objectKey.Equals(".write_access_check_file.temp", StringComparison.OrdinalIgnoreCase)) 
                {
                    context.Logger.LogLine("Skipping processing of Amazon Transcribe write-access test file");
                    continue;
                }

                context.Logger.LogLine($"Sending job complete notification for {objectKey} in bucket {bucketName}");

                // this does not make an actual call to S3...
                var s3Uri = new AmazonS3Client().GetPreSignedURL(new Amazon.S3.Model.GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    Expires = DateTime.Now.AddDays(7),
                    Verb = HttpVerb.GET,
                    Protocol = Protocol.HTTPS,
                    ResponseHeaderOverrides = new ResponseHeaderOverrides
                    {
                        ContentType = "text/json"
                    }
                });

                await snsClient.PublishAsync(new PublishRequest
                {
                    TopicArn = topicArn,
                    Subject = "Transcription complete!",
                    Message = $"Transcription of file {objectKey} is now available at {s3Uri}"
                });
            }
        }
    }
}
