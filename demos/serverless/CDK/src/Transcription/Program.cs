using Amazon.CDK;

namespace Transcription
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new TranscriptionStack(app, "TranscriptionStack");
            app.Synth();
        }
    }
}
