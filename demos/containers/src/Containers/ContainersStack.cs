using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;

namespace Containers
{
    public class ContainersStack : Stack
    {
        public ContainersStack(Construct parent, string id, IStackProps props) : base(parent, id, props)
        {
            // The code that defines your stack goes here
            Vpc vpc = new Vpc(this, "VPC");
            ClusterProps clusterProps = new ClusterProps();
            clusterProps.Vpc = vpc;
            Cluster cluster = new Cluster(this, "Cluster", clusterProps);
            FargateTaskDefinition taskDef = new FargateTaskDefinition(this, "FargateTaskDefinition");
            ContainerDefinitionOptions containerOptions = new ContainerDefinitionOptions() {
                Image = ContainerImage.FromAsset("app/")
            };

            taskDef.AddContainer("Container", containerOptions).AddPortMappings(new PortMapping() {
                ContainerPort = 80,
                HostPort = 80
            });

            ApplicationLoadBalancedFargateService service = new ApplicationLoadBalancedFargateService(this, "DotnetFargateApp", new ApplicationLoadBalancedFargateServiceProps() {
                MemoryLimitMiB = 512,
                Cpu = 256,
                TaskDefinition = taskDef
            });
            
        }
    }
}
