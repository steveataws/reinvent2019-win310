# Demo scenario

This simple 'Hello World' app deploys content for a static website to an Amazon S3 bucket, and configures the bucket and its content for public read-only access.

## Useful commands

* `dotnet build src` compile this app
* `cdk bootstrap`   creates artifacts to permit the CDK to upload assets associated with the sample (the static website files)
* `cdk deploy`      deploy this stack to your default AWS account/region
* `cdk diff`        compare deployed stack with current state
* `cdk synth`       emits the synthesized CloudFormation template
