using Bonsai.Harp;
using System.Threading;
using System.Threading.Tasks;

namespace AllenNeuralDynamics.Cuttlefish
{
    /// <inheritdoc/>
    public partial class Device
    {
        /// <summary>
        /// Initializes a new instance of the asynchronous API to configure and interface
        /// with Cuttlefish devices on the specified serial port.
        /// </summary>
        /// <param name="portName">
        /// The name of the serial port used to communicate with the Harp device.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous initialization operation. The value of
        /// the <see cref="Task{TResult}.Result"/> parameter contains a new instance of
        /// the <see cref="AsyncDevice"/> class.
        /// </returns>
        public static async Task<AsyncDevice> CreateAsync(string portName, CancellationToken cancellationToken = default)
        {
            var device = new AsyncDevice(portName);
            var whoAmI = await device.ReadWhoAmIAsync(cancellationToken);
            if (whoAmI != Device.WhoAmI)
            {
                var errorMessage = string.Format(
                    "The device ID {1} on {0} was unexpected. Check whether a Cuttlefish device is connected to the specified serial port.",
                    portName, whoAmI);
                throw new HarpException(errorMessage);
            }

            return device;
        }
    }

    /// <summary>
    /// Represents an asynchronous API to configure and interface with Cuttlefish devices.
    /// </summary>
    public partial class AsyncDevice : Bonsai.Harp.AsyncDevice
    {
        internal AsyncDevice(string portName)
            : base(portName)
        {
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="PinDirection"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<Pins> ReadPinDirectionAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PinDirection.Address), cancellationToken);
            return PinDirection.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="PinDirection"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<Pins>> ReadTimestampedPinDirectionAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PinDirection.Address), cancellationToken);
            return PinDirection.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the <see cref="PinDirection"/> register.
        /// </summary>
        /// <param name="value">The value to write in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePinDirectionAsync(Pins value, CancellationToken cancellationToken = default)
        {
            var request = PinDirection.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="PinState"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<Pins> ReadPinStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PinState.Address), cancellationToken);
            return PinState.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="PinState"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<Pins>> ReadTimestampedPinStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PinState.Address), cancellationToken);
            return PinState.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the <see cref="PinState"/> register.
        /// </summary>
        /// <param name="value">The value to write in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePinStateAsync(Pins value, CancellationToken cancellationToken = default)
        {
            var request = PinState.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="PinSet"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<Pins> ReadPinSetAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PinSet.Address), cancellationToken);
            return PinSet.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="PinSet"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<Pins>> ReadTimestampedPinSetAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PinSet.Address), cancellationToken);
            return PinSet.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the <see cref="PinSet"/> register.
        /// </summary>
        /// <param name="value">The value to write in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePinSetAsync(Pins value, CancellationToken cancellationToken = default)
        {
            var request = PinSet.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="PinClear"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<Pins> ReadPinClearAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PinClear.Address), cancellationToken);
            return PinClear.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="PinClear"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<Pins>> ReadTimestampedPinClearAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PinClear.Address), cancellationToken);
            return PinClear.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the <see cref="PinClear"/> register.
        /// </summary>
        /// <param name="value">The value to write in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePinClearAsync(Pins value, CancellationToken cancellationToken = default)
        {
            var request = PinClear.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="EnableRisingEdgeEvents"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<Pins> ReadEnableRisingEdgeEventsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(EnableRisingEdgeEvents.Address), cancellationToken);
            return EnableRisingEdgeEvents.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="EnableRisingEdgeEvents"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<Pins>> ReadTimestampedEnableRisingEdgeEventsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(EnableRisingEdgeEvents.Address), cancellationToken);
            return EnableRisingEdgeEvents.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the <see cref="EnableRisingEdgeEvents"/> register.
        /// </summary>
        /// <param name="value">The value to write in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteEnableRisingEdgeEventsAsync(Pins value, CancellationToken cancellationToken = default)
        {
            var request = EnableRisingEdgeEvents.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="RisingEdgeEvents"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<Pins> ReadRisingEdgeEventsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(RisingEdgeEvents.Address), cancellationToken);
            return RisingEdgeEvents.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="RisingEdgeEvents"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<Pins>> ReadTimestampedRisingEdgeEventsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(RisingEdgeEvents.Address), cancellationToken);
            return RisingEdgeEvents.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="EnableFallingEdgeEvents"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<Pins> ReadEnableFallingEdgeEventsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(EnableFallingEdgeEvents.Address), cancellationToken);
            return EnableFallingEdgeEvents.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="EnableFallingEdgeEvents"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<Pins>> ReadTimestampedEnableFallingEdgeEventsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(EnableFallingEdgeEvents.Address), cancellationToken);
            return EnableFallingEdgeEvents.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the <see cref="EnableFallingEdgeEvents"/> register.
        /// </summary>
        /// <param name="value">The value to write in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteEnableFallingEdgeEventsAsync(Pins value, CancellationToken cancellationToken = default)
        {
            var request = EnableFallingEdgeEvents.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="FallingEdgeEvents"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<Pins> ReadFallingEdgeEventsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(FallingEdgeEvents.Address), cancellationToken);
            return FallingEdgeEvents.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="FallingEdgeEvents"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<Pins>> ReadTimestampedFallingEdgeEventsAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(FallingEdgeEvents.Address), cancellationToken);
            return FallingEdgeEvents.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="PwmState"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<EnableFlag> ReadPwmStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmState.Address), cancellationToken);
            return PwmState.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="PwmState"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<EnableFlag>> ReadTimestampedPwmStateAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmState.Address), cancellationToken);
            return PwmState.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the <see cref="PwmState"/> register.
        /// </summary>
        /// <param name="value">The value to write in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePwmStateAsync(EnableFlag value, CancellationToken cancellationToken = default)
        {
            var request = PwmState.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="PwmSettings0"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<byte[]> ReadPwmSettings0Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings0.Address), cancellationToken);
            return PwmSettings0.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="PwmSettings0"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedPwmSettings0Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings0.Address), cancellationToken);
            return PwmSettings0.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the <see cref="PwmSettings0"/> register.
        /// </summary>
        /// <param name="value">The value to write in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePwmSettings0Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = PwmSettings0.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="PwmSettings1"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<byte[]> ReadPwmSettings1Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings1.Address), cancellationToken);
            return PwmSettings1.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="PwmSettings1"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedPwmSettings1Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings1.Address), cancellationToken);
            return PwmSettings1.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the <see cref="PwmSettings1"/> register.
        /// </summary>
        /// <param name="value">The value to write in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePwmSettings1Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = PwmSettings1.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="PwmSettings2"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<byte[]> ReadPwmSettings2Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings2.Address), cancellationToken);
            return PwmSettings2.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="PwmSettings2"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedPwmSettings2Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings2.Address), cancellationToken);
            return PwmSettings2.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the <see cref="PwmSettings2"/> register.
        /// </summary>
        /// <param name="value">The value to write in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePwmSettings2Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = PwmSettings2.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="PwmSettings3"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<byte[]> ReadPwmSettings3Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings3.Address), cancellationToken);
            return PwmSettings3.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="PwmSettings3"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedPwmSettings3Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings3.Address), cancellationToken);
            return PwmSettings3.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the <see cref="PwmSettings3"/> register.
        /// </summary>
        /// <param name="value">The value to write in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePwmSettings3Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = PwmSettings3.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="PwmSettings4"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<byte[]> ReadPwmSettings4Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings4.Address), cancellationToken);
            return PwmSettings4.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="PwmSettings4"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedPwmSettings4Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings4.Address), cancellationToken);
            return PwmSettings4.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the <see cref="PwmSettings4"/> register.
        /// </summary>
        /// <param name="value">The value to write in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePwmSettings4Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = PwmSettings4.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="PwmSettings5"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<byte[]> ReadPwmSettings5Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings5.Address), cancellationToken);
            return PwmSettings5.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="PwmSettings5"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedPwmSettings5Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings5.Address), cancellationToken);
            return PwmSettings5.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the <see cref="PwmSettings5"/> register.
        /// </summary>
        /// <param name="value">The value to write in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePwmSettings5Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = PwmSettings5.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="PwmSettings6"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<byte[]> ReadPwmSettings6Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings6.Address), cancellationToken);
            return PwmSettings6.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="PwmSettings6"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedPwmSettings6Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings6.Address), cancellationToken);
            return PwmSettings6.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the <see cref="PwmSettings6"/> register.
        /// </summary>
        /// <param name="value">The value to write in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePwmSettings6Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = PwmSettings6.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the <see cref="PwmSettings7"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the register payload.
        /// </returns>
        public async Task<byte[]> ReadPwmSettings7Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings7.Address), cancellationToken);
            return PwmSettings7.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the timestamped contents of the <see cref="PwmSettings7"/> register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The task result contains
        /// the timestamped register payload.
        /// </returns>
        public async Task<Timestamped<byte[]>> ReadTimestampedPwmSettings7Async(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(PwmSettings7.Address), cancellationToken);
            return PwmSettings7.GetTimestampedPayload(reply);
        }

        /// <summary>
        /// Asynchronously writes a value to the <see cref="PwmSettings7"/> register.
        /// </summary>
        /// <param name="value">The value to write in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WritePwmSettings7Async(byte[] value, CancellationToken cancellationToken = default)
        {
            var request = PwmSettings7.FromPayload(MessageType.Write, value);
            await CommandAsync(request, cancellationToken);
        }
    }
}
