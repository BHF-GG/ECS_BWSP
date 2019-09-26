using System;
using System.Configuration;
using NSubstitute;
using NUnit.Framework;


namespace ECS.Test.Unit
{
    [TestFixture]
    public class EcsUnitTests
    {
        // member variables to hold uut and fakes
        private FakeTempSensor _fakeTempSensor;
        private FakeHeater _fakeHeater;
        private ECS _uut;
        private FakeWindow _fakeWindow;

        //For substituding
        private ITempSensor _subTempSensor;
        private IHeater _subHeater;
        private IWindow _subWindow;
        private ECS _uutsub;

        [SetUp]
        public void Setup()
        {
            // Create the fake stubs and mocks
            _fakeHeater = new FakeHeater();
            _fakeTempSensor = new FakeTempSensor();
            _fakeWindow = new FakeWindow();

            _subTempSensor = Substitute.For<ITempSensor>();
            _subHeater = Substitute.For<IHeater>();
            _subWindow = Substitute.For<IWindow>();

            // Inject them into the uut via the constructor
            _uut = new ECS(_fakeTempSensor, _fakeHeater, _fakeWindow, 25, 28);
            _uutsub = new ECS(_subTempSensor, _subHeater, _subWindow, 25, 28);
        }


        #region Threshold tests

        [Test]
        public void Thresholds_ValidUpperTemperatureThresholdSet_NoExceptionsThrown()
        {
            // Check that it doesn't throw
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.UpperTemperatureThreshold = 27; }, Throws.Nothing);
        }

        [Test]
        public void Thresholds_ValidLowerTemperatureThresholdSet_NoExceptionsThrown()
        {
            // Check that it doesn't throw 
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.LowerTemperatureThreshold = 26; }, Throws.Nothing);
        }

        [Test]
        public void Thresholds_UpperSetToLower_NoExceptionsThrown()
        {
            // Check that it doesn't throw when they are equal
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.UpperTemperatureThreshold = _uut.LowerTemperatureThreshold; }, Throws.Nothing);
        }

        [Test]
        public void Thresholds_LowerSetToUpper_NoExceptionsThrown()
        {
            // Check that it doesn't throw when they are equal
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.LowerTemperatureThreshold = _uut.UpperTemperatureThreshold; }, Throws.Nothing);
        }
        #endregion

        #region Regulation tests

        #region T < Tlow

        [Test]
        public void Regulate_TempIsLow_HeaterIsTurnedOn()
        {
            // Setup stub with desired response
            _fakeTempSensor.Temp = 20;
            // Act
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            Assert.That(_fakeHeater.TurnOnCalledTimes, Is.EqualTo(1));
        }


        [Test]
        public void Regulate_TempIsLow_WindowIsClosed()
        {
            // Setup stub with desired response
            _fakeTempSensor.Temp = 20;
            // Act
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            Assert.That(_fakeWindow.CloseCalledTimes, Is.EqualTo(1));
        }

        #endregion

        #region T == Tlow

        [Test]
        public void Regulate_TempIsAtLowerThreshold_HeaterIsTurnedOff()
        {
            // Setup the stub with desired response
            _fakeTempSensor.Temp = 25;
            // Act
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            Assert.That(_fakeHeater.TurnOffCalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void Regulate_TempIsAtLowerThreshold_WindowIsClosed()
        {
            // Setup the stub with desired response
            _fakeTempSensor.Temp = 25;
            // Act
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            Assert.That(_fakeWindow.CloseCalledTimes, Is.EqualTo(1));
        }

        #endregion

        #region Tlow < T < Thigh

        [Test]
        public void Regulate_TempIsBetweenLowerAndUpperThresholds_HeaterIsTurnedOff()
        {
            // Setup the stub with desired response
            _fakeTempSensor.Temp = 27;
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            Assert.That(_fakeHeater.TurnOnCalledTimes, Is.EqualTo(0));
        }

        [Test]
        public void Regulate_TempIsBetweenLowerAndUpperThresholds_WindowIsClosed()
        {
            // Setup the stub with desired response
            _fakeTempSensor.Temp = 27;
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            Assert.That(_fakeWindow.CloseCalledTimes, Is.EqualTo(1));
        }

        #endregion

        #region T == Thigh

        [Test]
        public void Regulate_TempIsAtUpperThreshold_HeaterIsTurnedOff()
        {
            // Setup the stub with desired response
            _fakeTempSensor.Temp = 27;
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            Assert.That(_fakeHeater.TurnOffCalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void Regulate_TempIsAtUpperThreshold_WindowIsClosed()
        {
            // Setup the stub with desired response
            _fakeTempSensor.Temp = 27;
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            Assert.That(_fakeWindow.CloseCalledTimes, Is.EqualTo(1));
        }

        #endregion

        #region T > Thigh

        [Test]
        public void Regulate_TempIsAboveUpperThreshold_HeaterIsTurnedOff()
        {
            // Setup the stub with desired response
            _fakeTempSensor.Temp = 27;
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            Assert.That(_fakeHeater.TurnOffCalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void Regulate_TempIsAboveUpperThreshold_WindowIsOpened()
        {
            // Setup the stub with desired response
            _fakeTempSensor.Temp = 29;
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            Assert.That(_fakeWindow.OpenCalledTimes, Is.EqualTo(1));
        }

        #endregion

        #endregion

        #region Regulation tests with subs

        [Test]
        public void RegulateSub_TempIsLow_HeaterIsTurnedOn()
        {
            // Setup substitude
            _subTempSensor.GetTemp().Returns(20);
            // Act
            _uutsub.Regulate();
            // Check if turnOn is called once
            _subHeater.Received(1).TurnOn();
            //_subWindow.Received(1).Close();
        }

        [Test]
        public void RegulateSub_TempIsLow_WindowIsClosed()
        {
            // Setup substitude
            _subTempSensor.GetTemp().Returns(20);
            // Act
            _uutsub.Regulate();
            // Check if turnOn is called once
            _subWindow.Received(1).Close();
        }

        #region T == Tlow

        [Test]
        public void RegulateSub_TempIsAtLowerThreshold_HeaterIsTurnedOff()
        {
            //Setting temp to lower
            _subTempSensor.GetTemp().Returns(25);
            _uutsub.Regulate();
            _subHeater.Received(0).TurnOn();

        }

        [Test]
        public void RegulateSub_TempIsAtLowerThreshold_WindowIsClosed()
        {
            //Setting temp to lower
            _subTempSensor.GetTemp().Returns(25);
            _uutsub.Regulate();
            _subWindow.Received(0).Open();
        }

        #endregion

        #region Tlow < T < Thigh

        [Test]
        public void RegulateSub_TempIsBetweenLowerAndUpperThresholds_HeaterIsTurnedOff()
        {
            //Setting temp between lower and upper threshhold
            _subTempSensor.GetTemp().Returns(27);
            _uutsub.Regulate();
            _subHeater.Received(0).TurnOn();
        }

        [Test]
        public void RegulateSub_TempIsBetweenLowerAndUpperThresholds_WindowIsClosed()
        {
            // Setup the stub with desired response
            _fakeTempSensor.Temp = 27;
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            Assert.That(_fakeWindow.CloseCalledTimes, Is.EqualTo(1));
        }

        #endregion

        #region T == Thigh

        [Test]
        public void RegulateSub_TempIsAtUpperThreshold_HeaterIsTurnedOff()
        {
            // Setup the stub with desired response
            _fakeTempSensor.Temp = 27;
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            Assert.That(_fakeHeater.TurnOffCalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void RegulateSub_TempIsAtUpperThreshold_WindowIsClosed()
        {
            // Setup the stub with desired response
            _fakeTempSensor.Temp = 27;
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            Assert.That(_fakeWindow.CloseCalledTimes, Is.EqualTo(1));
        }

        #endregion

        #region T > Thigh

        [Test]
        public void RegulateSub_TempIsAboveUpperThreshold_HeaterIsTurnedOff()
        {
            // Setup the stub with desired response
            _fakeTempSensor.Temp = 27;
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            Assert.That(_fakeHeater.TurnOffCalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void RegulateSub_TempIsAboveUpperThreshold_WindowIsOpened()
        {
            // Setup the stub with desired response
            _fakeTempSensor.Temp = 29;
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            Assert.That(_fakeWindow.OpenCalledTimes, Is.EqualTo(1));
        }
        #endregion
        #endregion

        #region Threshold tests with subs
        [Test]
        public void ThresholdsSub_ValidUpperTemperatureThresholdSet_NoExceptionsThrown()
        {
            // Check that it doesn't throw
            _uutsub.LowerTemperatureThreshold = 20;
            Assert.That(() => { _uutsub.UpperTemperatureThreshold = 27; }, Throws.Nothing);
        }

        [Test]
        public void ThresholdsSub_ValidLowerTemperatureThresholdSet_NoExceptionsThrown()
        {
            // Check that it doesn't throw 
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.LowerTemperatureThreshold = 26; }, Throws.Nothing);
        }

        [Test]
        public void ThresholdsSub_UpperSetToLower_NoExceptionsThrown()
        {
            // Check that it doesn't throw when they are equal
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.UpperTemperatureThreshold = _uut.LowerTemperatureThreshold; }, Throws.Nothing);
        }

        [Test]
        public void ThresholdsSub_LowerSetToUpper_NoExceptionsThrown()
        {
            // Check that it doesn't throw when they are equal
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.LowerTemperatureThreshold = _uut.UpperTemperatureThreshold; }, Throws.Nothing);
        }


        [Test]
        public void ThresholdsSub_InvalidUpperTemperatureThresholdSet_ExceptionsThrown()
        {
            // Check that it throws
            Assert.That(() => { _uutsub.UpperTemperatureThreshold = 10; }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ThresholdsSub_InvalidLowerTemperatureThresholdSet_ExceptionsThrown()
        {
            // Check that it throws
            Assert.That(() => { _uutsub.LowerTemperatureThreshold = 45; }, Throws.TypeOf<ArgumentException>());
        }
        #endregion
    }
}
