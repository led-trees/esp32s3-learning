using System.Device.Gpio;

namespace spiled
{
    public class LedIndicator
    {
        readonly GpioController controller;
        readonly GpioPin led1;
        readonly GpioPin led2;

        bool led1State = false;
        bool led2State = false;

        public LedIndicator(GpioController controller)
        {
            this.controller = controller;

            led1 = controller.OpenPin(1, PinMode.Output);
            led2 = controller.OpenPin(2, PinMode.Output);
        }

        public bool Led1
        {
            get => led1State;
            set
            {
                led1State = value;
                Update();
            }
        }

        public bool Led2
        {
            get => led2State;
            set
            {
                led2State = value;
                Update();
            }
        }

        void Update()
        {
            led1.Write(led1State ? PinValue.High : PinValue.Low);
            led2.Write(led2State ? PinValue.High : PinValue.Low);
        }
    }
}