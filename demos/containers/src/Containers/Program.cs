using Amazon.CDK;

namespace Containers
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new App(null);
            new ContainersStack(app, "ContainersStack", new StackProps());
            app.Synth();
        }
    }
}
