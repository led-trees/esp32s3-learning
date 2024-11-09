using System.Device.Gpio;

namespace spiled
{
    public class Togglers
    {
        readonly GpioController controller;
        readonly GpioPin toggler1;
        readonly GpioPin toggler2;
        readonly GpioPin toggler3;
        readonly GpioPin toggler4;
        readonly GpioPin toggler5;
        readonly GpioPin toggler6;
        readonly GpioPin toggler7;
        readonly GpioPin toggler8;

        public Togglers(GpioController controller)
        {
            this.controller = controller;

            toggler1 = controller.OpenPin(4, PinMode.Input);
            toggler2 = controller.OpenPin(5, PinMode.Input);
            toggler3 = controller.OpenPin(6, PinMode.Input);
            toggler4 = controller.OpenPin(7, PinMode.Input);
            toggler5 = controller.OpenPin(15, PinMode.Input);
            toggler6 = controller.OpenPin(16, PinMode.Input);
            toggler7 = controller.OpenPin(17, PinMode.Input);
            toggler8 = controller.OpenPin(18, PinMode.Input);
        }

        public bool Value1 => toggler1.Read() == PinValue.Low;
        public bool Value2 => toggler2.Read() == PinValue.Low;
        public bool Value3 => toggler3.Read() == PinValue.Low;
        public bool Value4 => toggler4.Read() == PinValue.Low;
        public bool Value5 => toggler5.Read() == PinValue.Low;
        public bool Value6 => toggler6.Read() == PinValue.Low;
        public bool Value7 => toggler7.Read() == PinValue.Low;
        public bool Value8 => toggler8.Read() == PinValue.Low;

        public byte Byte
        {
            get
            {
                byte b = 0b_0000_0000;

                if (Value1)
                    b = b |= 0b_0000_0001;
                if (Value2)
                    b = b |= 0b_0000_0010;
                if (Value3)
                    b = b |= 0b_0000_0100;
                if (Value4)
                    b = b |= 0b_0000_1000;
                if (Value5)
                    b = b |= 0b_0001_0000;
                if (Value6)
                    b = b |= 0b_0010_0000;
                if (Value7)
                    b = b |= 0b_0100_0000;
                if (Value8)
                    b = b |= 0b_1000_0000;

                return b;
            }
        }
    }
}