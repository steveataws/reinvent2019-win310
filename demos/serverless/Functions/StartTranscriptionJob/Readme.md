# StartTranscriptionJob Project

This Lambda function is used to start a Amazon Transcribe job against an audio
or video file uploaded to an Amazon S3 bucket. The bucket must be configured as
an event source for the function.

Transcription is an asynchronous task, so once the job is queued, the Lambda function
exits. A second Lambda function, in the NotifyTranscriptionJobComplete project, is
triggered when Amazon Transcribe writes the transcribed text to the output bucket.

To deploy the stack and related Lambda functions, use the build_and_deploy.ps1 script
in the CDK project folder. You will need to have the Amazon.Lambda.Tools extensions for
the dotnet CLI installed, as well as the CDK CLI.
