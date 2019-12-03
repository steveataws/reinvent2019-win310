param (
	# optional email to subscribe to completion notifications. If specified, check the email after
	# deployment to confirm the subscription
    [string]$subscriberEmail,
	
	# optional language code for Amazon Transcribe. If not specified, the stack will default to
	# 'en-us'
	[string]$languageCode
)
	
# Build the two Lambda functions and place the zip bundles into the assets folder for the
# CDK deployment process to pick up. Note that these commands assume you have the 
# Amazon.Lambda.Tools global tools package installed (dotnet tool install -g Amazon.Lambda.Tools).

Write-Host "...Building Lambda function packages"

dotnet lambda package --project-location ../Functions/StartTranscriptionJob --output-package ./assets/StartTranscriptionJob.zip
dotnet lambda package --project-location ../Functions/NotifyTranscriptionJobComplete --output-package ./assets/NotifyTranscriptionJobComplete.zip

# Deploy the stack using the CDK CLI

$command = "cdk deploy"
if ($subscriberEmail) {
	$command += " -c SubscriberEmail=$subscriberEmail"
}
if ($languageCode) {
	$command += " -c LanguageCode=$languageCode"
}

Write-Host "...Deploying stack with command: $command"
Invoke-Expression $command
