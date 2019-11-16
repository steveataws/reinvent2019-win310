using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new App(null);
            new HelloWorldStack(app, "HelloWorldStack", new StackProps());
            app.Synth();
        }
    }
}
