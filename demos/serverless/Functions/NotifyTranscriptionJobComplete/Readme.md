# NotifyTranscriptionJobComplete Project

This Lambda function is triggered when the output from an Amazon Transcribe job, 
started as a result of the StartTranscriptionJob function, is written to an Amazon S3
bucket. The bucket must be configured as an event source for the function.

The function sends a notification message to an Amazon SNS topic that the transcription
output is available. The message contains a presigned url to the output file that is valid
for one week.

You can have an email subscription configured to this topic by supplying 
an email address to the stack when deploying. Pass the parameter -SubscriberEmail to the
PowerShell build script noted below. By default the transcription job uses the 'en-us' 
language code but this can be changed by passing a value to the -LanguageCode parameter 
of the build script. Parameters passed to the build script are supplied as context values
to the resulting CDK deployment command.

To deploy the stack and related Lambda functions, use the build_and_deploy.ps1 script
in the CDK project folder. You will need to have the Amazon.Lambda.Tools extensions for
the dotnet CLI installed, as well as the CDK CLI.
