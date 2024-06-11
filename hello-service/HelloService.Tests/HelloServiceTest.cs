using NSubstitute;
using NUnit.Framework;

namespace Hello.Tests
{
    public class HelloServiceTest
    {
        private HelloService _helloService;
        private OutputService? _outputService;
        private DateTimeProvider? _dateTimeProvider;

        [SetUp]
        public void SetUp()
        {
            _outputService = Substitute.For<OutputService>();
            _dateTimeProvider = Substitute.For<DateTimeProvider>();
            _helloService = new HelloService(_outputService, _dateTimeProvider);
        }

        [Test]
        public void Say_Buenos_dias_at_6AM()
        {
            Get(new TimeOnly(6, 00));

            _helloService.Hello();

            Assert("Buenos dias!");
        }

        [Test]
        public void Say_Buenos_dias_at_11_59_AM() {
            Get(new TimeOnly(11, 59));

            _helloService.Hello();

            Assert("Buenos dias!");
        }

        [Test]
        public void Say_Buenas_tardes_between_12AM_to_07_59_PM() {
           Get(new TimeOnly(12, 00));

           _helloService.Hello();

           Assert("Buenas tardes!");
        }

        [Test]
        public void Say_Buenas_noches_between_08PM_to_05_59_AM() {
            Get(new TimeOnly(20, 00));

            _helloService.Hello();

            Assert("Buenas noches!");
        }

        private void Get(TimeOnly time)
        {
            _dateTimeProvider.Get().Returns(time);
        }

        private void Assert(string message)
        {
            _outputService.Received(1).Write(message);
        }
    }
}


/* Lista de Test
 * Te saluda con "Buenos d�as!" desde las 6:00 AM hasta las 11:59AM OK
 * Te saluda con "Buenas tardes!" desde las 12:00 PM hasta las 07:59 PM OK
 * Te saluda con "Buenas noches!" desde las 08:00 PM hasta las 5:59 AM
 * */