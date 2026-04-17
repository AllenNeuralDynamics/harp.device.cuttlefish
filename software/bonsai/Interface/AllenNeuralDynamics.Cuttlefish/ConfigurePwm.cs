using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Harp;
using Bonsai;

namespace AllenNeuralDynamics.Cuttlefish
{
    /// <summary>
    /// Represents an operator that generates a sequence of Harp messages to
    /// configure the PWM feature.
    /// </summary>
    [Description("Generates a sequence of Harp messages to configure the PWM feature.")]
    public class ConfigurePwm : Source<HarpMessage>
    {
        /// <summary>
        /// Gets or sets the type of the Harp Message
        /// </summary>
        [Description("The type of the Harp message.")]
        public MessageType MessageType { get; set; } = MessageType.Write;

        /// <summary>
        /// Gets or sets the PWM protocol delay.
        /// </summary>
        [Description("The delay to start the PWM protocol after the trigger is activated.")]
        public uint Delay { get; set; } = 0;

        /// <summary>
        /// Gets or sets the on-time of the PWM pulse. Defined in microseconds.
        /// </summary>
        [Description("The time the pulse spends on the High state. Defined in microseconds.")]
        public uint OnTime { get; set; } = 500000;

        /// <summary>
        /// Gets or sets the off-time of the PWM pulse. Defined in microseconds.
        /// </summary>
        [Description("The off-time of the PWM pulse. Defined in microseconds.")]
        public uint OffDuration { get; set; } = 1000000;

        /// <summary>
        /// Gets or sets the number of pulses to trigger on the specified PWM.
        /// If the default value of zero is specified, the PWM will be infinite.
        /// </summary>
        [Description("The PWM output pin.")]
        public PwmPin Pin { get; set; } = PwmPin.Pwm0;

        /// <summary>
        /// Gets or sets the number of times the PWM protocol will be repeated.
        /// </summary>
        [Description("The number of times the PWM protocol will be repeated.")]
        public uint RepeatCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value specifying whether generation of the PWM should be inverted.
        /// </summary>
        [Description("Specifies whether the pulse should be inverted.")]
        public bool Invert { get; set; } = false;


        /// <summary>
        /// Generates an observable sequence of Harp messages to configure a
        /// PWM task.
        /// </summary>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects representing a command
        /// to configure a PWM task.
        /// </returns>
        public override IObservable<HarpMessage> Generate()
        {
            return Observable.Return(BuildMessage(Pin, MessageType, null));
        }

        /// <summary>
        /// Generates an observable sequence of Harp messages to configure the
        /// PWM feature whenever the source sequence emits a notification.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used to emit new configuration
        /// messages.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects representing the commands
        /// needed to fully configure the PWM feature.
        /// </returns>
        public IObservable<HarpMessage> Generate<TSource>(IObservable<TSource> source)
        {
            return source.Select(input => BuildMessage(Pin, MessageType, null));
        }

        /// <summary>
        /// Builds a message to configure the PWM task.
        /// </summary>
        public HarpMessage BuildMessage(PwmPin outputPin, MessageType messageType, double? timestamp = null)
        {
            var payload = new HelperMethods.PwmTaskPayload()
            {
                offset_us = Delay,
                on_duration_us = OnTime,
                off_duration_us = OffDuration,
                cycles = RepeatCount,
                invert = Invert ? (byte)1 : (byte)0
            };
            var bytes = HelperMethods.StructToByteArray(payload);
            if (timestamp.HasValue)
            {
                return HarpMessage.FromPayload((int)outputPin + offset, timestamp.Value, messageType, PayloadType.U8, bytes);
            }
            else {
                return HarpMessage.FromPayload((int)outputPin + offset, messageType, PayloadType.U8, bytes);
            }
        }

            private static int offset = Device.RegisterMap.First(kv => kv.Value == typeof(PwmSettings0)).Key;
            
            /// <summary>
            /// Specifies the PWM output pin.
            /// </summary>
            public enum PwmPin : uint
            {
                Pwm0 = Pins.Pin0,
                Pwm1 = Pins.Pin1,
                Pwm2 = Pins.Pin2,
                Pwm3 = Pins.Pin3,

                Pwm4 = Pins.Pin4,
                Pwm5 = Pins.Pin5,
                Pwm6 = Pins.Pin6,
                Pwm7 = Pins.Pin7

            }

    }
}