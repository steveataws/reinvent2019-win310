using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.TranscribeService;
using Amazon.TranscribeService.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace StartTranscriptionJob
{
    public class Function
    {
        IAmazonS3 S3Client { get; set; }
        IAmazonTranscribeService TranscribeClient { get; set; }

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
            S3Client = new AmazonS3Client();
            TranscribeClient = new AmazonTranscribeServiceClient();
        }

        /// <summary>
        /// Constructs an instance with a preconfigured clients. This can be used for testing the outside of the Lambda environment.
        /// </summary>
        /// <param name="s3Client"></param>
        public Function(IAmazonS3 s3Client, IAmazonTranscribeService transcribeClient)
        {
            S3Client = s3Client;
            TranscribeClient = transcribeClient;
        }

        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used
        /// to respond to S3 notifications.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(S3Event evnt, ILambdaContext context)
        {
            if(!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LAMBDA_TASK_ROOT")))
            {
                Amazon.XRay.Recorder.Handlers.AwsSdk.AWSSDKHandler.RegisterXRayForAllServices();
            }

            var region = Environment.GetEnvironmentVariable("AWS_REGION");
            var outputBucket = Environment.GetEnvironmentVariable("TRANSCRIBE_OUTPUT_BUCKET");
            var languageCode = Environment.GetEnvironmentVariable("TRANSCRIBE_LANGUAGE_CODE");
            if (string.IsNullOrEmpty(languageCode))
            {
                languageCode = "en-us";
            }

            foreach (var record in evnt.Records)
            {
                var bucketName = record.S3.Bucket.Name;
                var objectKey =  System.Net.WebUtility.UrlDecode(record.S3.Object.Key);

                context.Logger.LogLine($"Starting transcription for object {objectKey} in bucket {bucketName}");

                var s3Uri = $"https://{bucketName}.s3.{region}.amazonaws.com/{objectKey}";

                // job name has to be unique, also note that any spaces in the object key
                // come through as +, which is not valid in job name
                var jobName = $"{objectKey.Replace("+", "")}_{DateTime.UtcNow.Ticks}";
                var response = await TranscribeClient.StartTranscriptionJobAsync(new StartTranscriptionJobRequest
                {
                    TranscriptionJobName = jobName,
                    LanguageCode = languageCode,
                    OutputBucketName = outputBucket,
                    MediaFormat = Path.GetExtension(objectKey).Trim('.'),
                    Media = new Media
                    {
                        MediaFileUri = s3Uri
                    }
                });
            }
        }
    }
}
