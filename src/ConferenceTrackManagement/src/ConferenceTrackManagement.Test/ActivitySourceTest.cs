using Xunit;

namespace ConferenceTrackManagement.Test
{
    using ConferenceTrackManagement.Entity;
    using ConferenceTrackManagement.Common;
    using ConferenceTrackManagement.Exceptions;

    public class ActivitySourceTest
    {
        private ActivityParser _parser;
        public ActivitySourceTest()
        {
            _parser = new ActivityParser();
        }

        [Fact]
        public void ParseActivityWithMin()
        {
            //Arrange 
            var text = "Common Ruby Errors 45min";

            //Act
            var activity = _parser.Parse(text);

            //Assert
            var expected = new Activity("Common Ruby Errors", new ActivityDuration(TimeUnit.Min, 45));
            Assert.NotNull(activity);
            Assert.Equal(activity.Subject, expected.Subject);
            Assert.Equal(activity.Duration.Unit, expected.Duration.Unit);
            Assert.Equal(activity.Duration.Value, expected.Duration.Value);
        }

        [Fact]
        public void ParseActivityWithLightning()
        {
            //Arrange 
            var text = "Rails for Python Developers lightning";

            //Act
            var activity = _parser.Parse(text);

            //Assert
            var expected = new Activity("Rails for Python Developers lightning", new ActivityDuration(TimeUnit.Lightning, 1));
            Assert.NotNull(activity);
            Assert.Equal(activity.Subject, expected.Subject);
            Assert.Equal(activity.Duration.Unit, expected.Duration.Unit);
            Assert.Equal(activity.Duration.Value, expected.Duration.Value);
        }
        
        [Fact]
        public void ParseActivityWithUnSupportedPattern()
        {
            //Arrange 
            var text = "Common Ruby Errors 1Hour";

            //Act & Assert
            Assert.Throws<AcitivityParseException>(()=>_parser.Parse(text));
        }
    }
}
