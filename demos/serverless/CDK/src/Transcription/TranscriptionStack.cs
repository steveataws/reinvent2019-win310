using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SNS.Subscriptions;

namespace Transcription
{
    public class TranscriptionStack : Stack
    {
        internal TranscriptionStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var inputBucket = new Bucket(this, "Input");
            var outputBucket = new Bucket(this, "Output");

            var languageCode = this.Node.TryGetContext("LanguageCode")?.ToString();
            if (string.IsNullOrEmpty(languageCode))
            {
                languageCode = "en-us";
            }

            var startJobFunction = new Function(this, "StartJobFunction", new FunctionProps
            {
                Tracing = Tracing.ACTIVE,
                Runtime = Runtime.DOTNET_CORE_2_1,
                Timeout = Duration.Seconds(30),
                MemorySize = 256,
                Environment = new Dictionary<string, string>()
                {
                    { "TRANSCRIBE_OUTPUT_BUCKET", outputBucket.BucketName },
                    { "TRANSCRIBE_LANGUAGE_CODE", languageCode }
                },
                FunctionName = "StartTranscriptionJob",
                Code = Code.FromAsset("./assets/StartTranscriptionJob.zip"),
                Handler = "StartTranscriptionJob::StartTranscriptionJob.Function::FunctionHandler",
                Events = new[] 
                {
                    new S3EventSource(inputBucket, new S3EventSourceProps
                    {
                        Events = new [] { EventType.OBJECT_CREATED }
                    })
                }
            });

            startJobFunction.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps
            {
                Actions = new[] { "transcribe:StartTranscriptionJob" },
                Effect = Effect.ALLOW,
                Resources = new[] { "*" }
            }));
            // this is passed onto Transcribe so it can write to the output bucket
            startJobFunction.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps
            {
                Actions = new[] { "s3:Put*" },
                Effect = Effect.ALLOW,
                Resources = new[] { outputBucket.ArnForObjects("*") }
            }));

            inputBucket.GrantRead(startJobFunction);

            var notificationTopic = new Topic(this, "TranscriptionNotificationTopic", new TopicProps
            {
                TopicName = "TranscriptionCompletedTopic",
            });

            var subscriberEmail = this.Node.TryGetContext("SubscriberEmail")?.ToString();
            if (!string.IsNullOrEmpty(subscriberEmail))
            {
                notificationTopic.AddSubscription(new EmailSubscription(subscriberEmail));
            }

            var notifyCompletionFunction = new Function(this, "NotifyCompleteFunction", new FunctionProps
            {
                Tracing = Tracing.ACTIVE,
                Runtime = Runtime.DOTNET_CORE_2_1,
                Timeout = Duration.Seconds(30),
                MemorySize = 256,
                Environment = new Dictionary<string, string>()
                {
                    { "TRANSCRIBE_TOPIC_ARN", notificationTopic.TopicArn }
                },
                FunctionName = "NotifyTranscriptionJobComplete",
                Code = Code.FromAsset("./assets/NotifyTranscriptionJobComplete.zip"),
                Handler = "NotifyTranscriptionJobComplete::NotifyTranscriptionJobComplete.Function::FunctionHandler",
                Events = new[]
                {
                    new S3EventSource(outputBucket, new S3EventSourceProps
                    {
                        Events = new [] { EventType.OBJECT_CREATED }
                    })
                }
            });

            notifyCompletionFunction.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps
            {
                Actions = new[] { "sns:Publish" },
                Effect = Effect.ALLOW,
                Resources = new[] { notificationTopic.TopicArn }
            }));
            notifyCompletionFunction.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps
            {
                // this permission is needed to allow the presigned url that the Lambda generates
                // to work and not yield an access-denied error
                Actions = new[] { "s3:GetObject" },
                Effect = Effect.ALLOW,
                Resources = new[] { outputBucket.ArnForObjects("*") }
            }));

            outputBucket.GrantWrite(notifyCompletionFunction);

            // emit the name of the input bucket so we know where to upload content!
            new CfnOutput(this, "InputBucket", new CfnOutputProps
            {
                Value = inputBucket.BucketName
            });
        }
    }
}
