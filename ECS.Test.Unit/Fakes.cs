namespace ECS.Test.Unit
{
    internal class FakeTempSensor : ITempSensor
    {
        public int Temp { get; set; }

        public FakeTempSensor()
        {
            Temp = 0;
        }

        public int GetTemp()
        {
            return Temp;
        }
    }

    internal class FakeHeater : IHeater
    {
        public int TurnOffCalledTimes { get; set; }
        public int TurnOnCalledTimes { get; set; }

        public FakeHeater()
        {
            TurnOffCalledTimes = 0;
            TurnOnCalledTimes = 0;
        }

        public void TurnOn()
        {
            ++TurnOnCalledTimes;
        }



        public void TurnOff()
        {
            ++TurnOffCalledTimes;
        }
    }


    internal class FakeWindow : IWindow
    {
        public int OpenCalledTimes { get; set; }
        public int CloseCalledTimes { get; set; }

        public FakeWindow()
        {
            OpenCalledTimes = 0;
            CloseCalledTimes = 0;
        }

        public void Close()
        {
            ++CloseCalledTimes;
        }

        public void Open()
        {
            ++OpenCalledTimes;
        }
    }

}
