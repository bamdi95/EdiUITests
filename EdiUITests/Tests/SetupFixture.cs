namespace EdiUITests.Tests
{
    [SetUpFixture]
    public static class SetupFixture
    {
        public static string EdiPath;

        [OneTimeSetUp]
        public static void GetEdiPath()
        {
            EdiPath = TestContext.Parameters.Get("EdiPath") ?? throw new ArgumentNullException("EdiPath");
        } 
    }
}