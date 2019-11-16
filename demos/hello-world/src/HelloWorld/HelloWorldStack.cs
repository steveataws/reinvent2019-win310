using Amazon.CDK;
using Amazon.CDK.AWS.S3;

namespace HelloWorld
{
    public class HelloWorldStack : Stack
    {
        public HelloWorldStack(Construct parent, string id, IStackProps props) : base(parent, id, props)
        {
            // The code that defines your stack goes here
            //code to create a public s3 bucket
            BucketProps bucketProps = new BucketProps();
            bucketProps.AccessControl = BucketAccessControl.PUBLIC_READ_WRITE;
            bucketProps.BucketName = "reinvent2019cdkdemo";
            Bucket bucket = new Bucket(this, "ReinventBucket", bucketProps);
        }
    }
}
