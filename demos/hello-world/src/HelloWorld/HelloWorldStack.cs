using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.S3.Deployment;
using Amazon.CDK.AWS.IAM;

namespace HelloWorld
{
    public class HelloWorldStack : Stack
    {
        public HelloWorldStack(Construct parent, string id, IStackProps props) : base(parent, id, props)
        {
            // Create a S3 bucket to host a static website, with global read access
            var bucket = new Bucket(this, "ReinventBucket", new BucketProps
            {
                BucketName = "reinvent2019cdkdemo",
                AccessControl = BucketAccessControl.PUBLIC_READ,
                WebsiteIndexDocument = "index.html"
            });

            // grant access to the pages and resources in the website (setting
            // Public Read Only on the bucket is not sufficient)
            bucket.AddToResourcePolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new [] { "s3:GetObject" },
                Resources = new [] { bucket.ArnForObjects("*") },
                Principals = new IPrincipal[]
                {
                    new AnyPrincipal()
                }
            }));

            // deploy the site
            new BucketDeployment(this, "ReinventBucketDeployment", new BucketDeploymentProps
            {
                DestinationBucket = bucket,
                Sources = new [] { Source.Asset("./site-contents") }
            });

            // emit the url of the website for convenience
            new CfnOutput(this, "BucketWebsiteUrl", new CfnOutputProps
            {
                Value = bucket.BucketWebsiteUrl
            });
        }
    }
}
