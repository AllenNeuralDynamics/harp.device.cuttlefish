using Bonsai;
using Bonsai.Harp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Xml.Serialization;

namespace AllenNeuralDynamics.Cuttlefish
{
    /// <summary>
    /// Generates events and processes commands for the Cuttlefish device connected
    /// at the specified serial port.
    /// </summary>
    [Combinator(MethodName = nameof(Generate))]
    [WorkflowElementCategory(ElementCategory.Source)]
    [Description("Generates events and processes commands for the Cuttlefish device.")]
    public partial class Device : Bonsai.Harp.Device, INamedElement
    {
        /// <summary>
        /// Represents the unique identity class of the <see cref="Cuttlefish"/> device.
        /// This field is constant.
        /// </summary>
        public const int WhoAmI = 1403;

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        public Device() : base(WhoAmI) { }

        string INamedElement.Name => nameof(Cuttlefish);

        /// <summary>
        /// Gets a read-only mapping from address to register type.
        /// </summary>
        public static new IReadOnlyDictionary<int, Type> RegisterMap { get; } = new Dictionary<int, Type>
            (Bonsai.Harp.Device.RegisterMap.ToDictionary(entry => entry.Key, entry => entry.Value))
        {
            { 32, typeof(PinDirection) },
            { 33, typeof(PinState) },
            { 34, typeof(PinSet) },
            { 35, typeof(PinClear) },
            { 36, typeof(EnableRisingEdgeEvents) },
            { 37, typeof(RisingEdgeEvents) },
            { 38, typeof(EnableFallingEdgeEvents) },
            { 39, typeof(FallingEdgeEvents) },
            { 40, typeof(PwmStart) },
            { 41, typeof(PwmStop) },
            { 42, typeof(PwmSettings0) },
            { 43, typeof(PwmSettings1) },
            { 44, typeof(PwmSettings2) },
            { 45, typeof(PwmSettings3) },
            { 46, typeof(PwmSettings4) },
            { 47, typeof(PwmSettings5) },
            { 48, typeof(PwmSettings6) },
            { 49, typeof(PwmSettings7) }
        };

        /// <summary>
        /// Gets the contents of the metadata file describing the <see cref="Cuttlefish"/>
        /// device registers.
        /// </summary>
        public static readonly string Metadata = GetDeviceMetadata();

        static string GetDeviceMetadata()
        {
            var deviceType = typeof(Device);
            using var metadataStream = deviceType.Assembly.GetManifestResourceStream($"{deviceType.Namespace}.device.yml");
            using var streamReader = new System.IO.StreamReader(metadataStream);
            return streamReader.ReadToEnd();
        }
    }

    /// <summary>
    /// Represents an operator that returns the contents of the metadata file
    /// describing the <see cref="Cuttlefish"/> device registers.
    /// </summary>
    [Description("Returns the contents of the metadata file describing the Cuttlefish device registers.")]
    public partial class GetDeviceMetadata : Source<string>
    {
        /// <summary>
        /// Returns an observable sequence with the contents of the metadata file
        /// describing the <see cref="Cuttlefish"/> device registers.
        /// </summary>
        /// <returns>
        /// A sequence with a single <see cref="string"/> object representing the
        /// contents of the metadata file.
        /// </returns>
        public override IObservable<string> Generate()
        {
            return Observable.Return(Device.Metadata);
        }
    }

    /// <summary>
    /// Represents an operator that groups the sequence of <see cref="Cuttlefish"/>" messages by register type.
    /// </summary>
    [Description("Groups the sequence of Cuttlefish messages by register type.")]
    public partial class GroupByRegister : Combinator<HarpMessage, IGroupedObservable<Type, HarpMessage>>
    {
        /// <summary>
        /// Groups an observable sequence of <see cref="Cuttlefish"/> messages
        /// by register type.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of observable groups, each of which corresponds to a unique
        /// <see cref="Cuttlefish"/> register.
        /// </returns>
        public override IObservable<IGroupedObservable<Type, HarpMessage>> Process(IObservable<HarpMessage> source)
        {
            return source.GroupBy(message => Device.RegisterMap[message.Address]);
        }
    }

    /// <summary>
    /// Represents an operator that writes the sequence of <see cref="Cuttlefish"/>" messages
    /// to the standard Harp storage format.
    /// </summary>
    [Description("Writes the sequence of Cuttlefish messages to the standard Harp storage format.")]
    public partial class DeviceDataWriter : Sink<HarpMessage>, INamedElement
    {
        const string BinaryExtension = ".bin";
        const string MetadataFileName = "device.yml";
        readonly Bonsai.Harp.MessageWriter writer = new();

        string INamedElement.Name => nameof(Cuttlefish) + "DataWriter";

        /// <summary>
        /// Gets or sets the relative or absolute path on which to save the message data.
        /// </summary>
        [Description("The relative or absolute path of the directory on which to save the message data.")]
        [Editor("Bonsai.Design.SaveFileNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string Path
        {
            get => System.IO.Path.GetDirectoryName(writer.FileName);
            set => writer.FileName = System.IO.Path.Combine(value, nameof(Cuttlefish) + BinaryExtension);
        }

        /// <summary>
        /// Gets or sets a value indicating whether element writing should be buffered. If <see langword="true"/>,
        /// the write commands will be queued in memory as fast as possible and will be processed
        /// by the writer in a different thread. Otherwise, writing will be done in the same
        /// thread in which notifications arrive.
        /// </summary>
        [Description("Indicates whether writing should be buffered.")]
        public bool Buffered
        {
            get => writer.Buffered;
            set => writer.Buffered = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to overwrite the output file if it already exists.
        /// </summary>
        [Description("Indicates whether to overwrite the output file if it already exists.")]
        public bool Overwrite
        {
            get => writer.Overwrite;
            set => writer.Overwrite = value;
        }

        /// <summary>
        /// Gets or sets a value specifying how the message filter will use the matching criteria.
        /// </summary>
        [Description("Specifies how the message filter will use the matching criteria.")]
        public FilterType FilterType
        {
            get => writer.FilterType;
            set => writer.FilterType = value;
        }

        /// <summary>
        /// Gets or sets a value specifying the expected message type. If no value is
        /// specified, all messages will be accepted.
        /// </summary>
        [Description("Specifies the expected message type. If no value is specified, all messages will be accepted.")]
        public MessageType? MessageType
        {
            get => writer.MessageType;
            set => writer.MessageType = value;
        }

        private IObservable<TSource> WriteDeviceMetadata<TSource>(IObservable<TSource> source)
        {
            var basePath = Path;
            if (string.IsNullOrEmpty(basePath))
                return source;

            var metadataPath = System.IO.Path.Combine(basePath, MetadataFileName);
            return Observable.Create<TSource>(observer =>
            {
                Bonsai.IO.PathHelper.EnsureDirectory(metadataPath);
                if (System.IO.File.Exists(metadataPath) && !Overwrite)
                {
                    throw new System.IO.IOException(string.Format("The file '{0}' already exists.", metadataPath));
                }

                System.IO.File.WriteAllText(metadataPath, Device.Metadata);
                return source.SubscribeSafe(observer);
            });
        }

        /// <summary>
        /// Writes each Harp message in the sequence to the specified binary file, and the
        /// contents of the device metadata file to a separate text file.
        /// </summary>
        /// <param name="source">The sequence of messages to write to the file.</param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing the
        /// messages to a raw binary file, and the contents of the device metadata file
        /// to a separate text file.
        /// </returns>
        public override IObservable<HarpMessage> Process(IObservable<HarpMessage> source)
        {
            return source.Publish(ps => ps.Merge(
                WriteDeviceMetadata(writer.Process(ps.GroupBy(message => message.Address)))
                .IgnoreElements()
                .Cast<HarpMessage>()));
        }

        /// <summary>
        /// Writes each Harp message in the sequence of observable groups to the
        /// corresponding binary file, where the name of each file is generated from
        /// the common group register address. The contents of the device metadata file are
        /// written to a separate text file.
        /// </summary>
        /// <param name="source">
        /// A sequence of observable groups, each of which corresponds to a unique register
        /// address.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing the Harp
        /// messages in each group to the corresponding file, and the contents of the device
        /// metadata file to a separate text file.
        /// </returns>
        public IObservable<IGroupedObservable<int, HarpMessage>> Process(IObservable<IGroupedObservable<int, HarpMessage>> source)
        {
            return WriteDeviceMetadata(writer.Process(source));
        }

        /// <summary>
        /// Writes each Harp message in the sequence of observable groups to the
        /// corresponding binary file, where the name of each file is generated from
        /// the common group register name. The contents of the device metadata file are
        /// written to a separate text file.
        /// </summary>
        /// <param name="source">
        /// A sequence of observable groups, each of which corresponds to a unique register
        /// type.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing the Harp
        /// messages in each group to the corresponding file, and the contents of the device
        /// metadata file to a separate text file.
        /// </returns>
        public IObservable<IGroupedObservable<Type, HarpMessage>> Process(IObservable<IGroupedObservable<Type, HarpMessage>> source)
        {
            return WriteDeviceMetadata(writer.Process(source));
        }
    }

    /// <summary>
    /// Represents an operator that filters register-specific messages
    /// reported by the <see cref="Cuttlefish"/> device.
    /// </summary>
    /// <seealso cref="PinDirection"/>
    /// <seealso cref="PinState"/>
    /// <seealso cref="PinSet"/>
    /// <seealso cref="PinClear"/>
    /// <seealso cref="EnableRisingEdgeEvents"/>
    /// <seealso cref="RisingEdgeEvents"/>
    /// <seealso cref="EnableFallingEdgeEvents"/>
    /// <seealso cref="FallingEdgeEvents"/>
    /// <seealso cref="PwmStart"/>
    /// <seealso cref="PwmStop"/>
    /// <seealso cref="PwmSettings0"/>
    /// <seealso cref="PwmSettings1"/>
    /// <seealso cref="PwmSettings2"/>
    /// <seealso cref="PwmSettings3"/>
    /// <seealso cref="PwmSettings4"/>
    /// <seealso cref="PwmSettings5"/>
    /// <seealso cref="PwmSettings6"/>
    /// <seealso cref="PwmSettings7"/>
    [XmlInclude(typeof(PinDirection))]
    [XmlInclude(typeof(PinState))]
    [XmlInclude(typeof(PinSet))]
    [XmlInclude(typeof(PinClear))]
    [XmlInclude(typeof(EnableRisingEdgeEvents))]
    [XmlInclude(typeof(RisingEdgeEvents))]
    [XmlInclude(typeof(EnableFallingEdgeEvents))]
    [XmlInclude(typeof(FallingEdgeEvents))]
    [XmlInclude(typeof(PwmStart))]
    [XmlInclude(typeof(PwmStop))]
    [XmlInclude(typeof(PwmSettings0))]
    [XmlInclude(typeof(PwmSettings1))]
    [XmlInclude(typeof(PwmSettings2))]
    [XmlInclude(typeof(PwmSettings3))]
    [XmlInclude(typeof(PwmSettings4))]
    [XmlInclude(typeof(PwmSettings5))]
    [XmlInclude(typeof(PwmSettings6))]
    [XmlInclude(typeof(PwmSettings7))]
    [Description("Filters register-specific messages reported by the Cuttlefish device.")]
    public class FilterRegister : FilterRegisterBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegister"/> class.
        /// </summary>
        public FilterRegister()
        {
            Register = new PinDirection();
        }

        string INamedElement.Name
        {
            get => $"{nameof(Cuttlefish)}.{GetElementDisplayName(Register)}";
        }
    }

    /// <summary>
    /// Represents an operator which filters and selects specific messages
    /// reported by the Cuttlefish device.
    /// </summary>
    /// <seealso cref="PinDirection"/>
    /// <seealso cref="PinState"/>
    /// <seealso cref="PinSet"/>
    /// <seealso cref="PinClear"/>
    /// <seealso cref="EnableRisingEdgeEvents"/>
    /// <seealso cref="RisingEdgeEvents"/>
    /// <seealso cref="EnableFallingEdgeEvents"/>
    /// <seealso cref="FallingEdgeEvents"/>
    /// <seealso cref="PwmStart"/>
    /// <seealso cref="PwmStop"/>
    /// <seealso cref="PwmSettings0"/>
    /// <seealso cref="PwmSettings1"/>
    /// <seealso cref="PwmSettings2"/>
    /// <seealso cref="PwmSettings3"/>
    /// <seealso cref="PwmSettings4"/>
    /// <seealso cref="PwmSettings5"/>
    /// <seealso cref="PwmSettings6"/>
    /// <seealso cref="PwmSettings7"/>
    [XmlInclude(typeof(PinDirection))]
    [XmlInclude(typeof(PinState))]
    [XmlInclude(typeof(PinSet))]
    [XmlInclude(typeof(PinClear))]
    [XmlInclude(typeof(EnableRisingEdgeEvents))]
    [XmlInclude(typeof(RisingEdgeEvents))]
    [XmlInclude(typeof(EnableFallingEdgeEvents))]
    [XmlInclude(typeof(FallingEdgeEvents))]
    [XmlInclude(typeof(PwmStart))]
    [XmlInclude(typeof(PwmStop))]
    [XmlInclude(typeof(PwmSettings0))]
    [XmlInclude(typeof(PwmSettings1))]
    [XmlInclude(typeof(PwmSettings2))]
    [XmlInclude(typeof(PwmSettings3))]
    [XmlInclude(typeof(PwmSettings4))]
    [XmlInclude(typeof(PwmSettings5))]
    [XmlInclude(typeof(PwmSettings6))]
    [XmlInclude(typeof(PwmSettings7))]
    [XmlInclude(typeof(TimestampedPinDirection))]
    [XmlInclude(typeof(TimestampedPinState))]
    [XmlInclude(typeof(TimestampedPinSet))]
    [XmlInclude(typeof(TimestampedPinClear))]
    [XmlInclude(typeof(TimestampedEnableRisingEdgeEvents))]
    [XmlInclude(typeof(TimestampedRisingEdgeEvents))]
    [XmlInclude(typeof(TimestampedEnableFallingEdgeEvents))]
    [XmlInclude(typeof(TimestampedFallingEdgeEvents))]
    [XmlInclude(typeof(TimestampedPwmStart))]
    [XmlInclude(typeof(TimestampedPwmStop))]
    [XmlInclude(typeof(TimestampedPwmSettings0))]
    [XmlInclude(typeof(TimestampedPwmSettings1))]
    [XmlInclude(typeof(TimestampedPwmSettings2))]
    [XmlInclude(typeof(TimestampedPwmSettings3))]
    [XmlInclude(typeof(TimestampedPwmSettings4))]
    [XmlInclude(typeof(TimestampedPwmSettings5))]
    [XmlInclude(typeof(TimestampedPwmSettings6))]
    [XmlInclude(typeof(TimestampedPwmSettings7))]
    [Description("Filters and selects specific messages reported by the Cuttlefish device.")]
    public partial class Parse : ParseBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parse"/> class.
        /// </summary>
        public Parse()
        {
            Register = new PinDirection();
        }

        string INamedElement.Name => $"{nameof(Cuttlefish)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents an operator which formats a sequence of values as specific
    /// Cuttlefish register messages.
    /// </summary>
    /// <seealso cref="PinDirection"/>
    /// <seealso cref="PinState"/>
    /// <seealso cref="PinSet"/>
    /// <seealso cref="PinClear"/>
    /// <seealso cref="EnableRisingEdgeEvents"/>
    /// <seealso cref="RisingEdgeEvents"/>
    /// <seealso cref="EnableFallingEdgeEvents"/>
    /// <seealso cref="FallingEdgeEvents"/>
    /// <seealso cref="PwmStart"/>
    /// <seealso cref="PwmStop"/>
    /// <seealso cref="PwmSettings0"/>
    /// <seealso cref="PwmSettings1"/>
    /// <seealso cref="PwmSettings2"/>
    /// <seealso cref="PwmSettings3"/>
    /// <seealso cref="PwmSettings4"/>
    /// <seealso cref="PwmSettings5"/>
    /// <seealso cref="PwmSettings6"/>
    /// <seealso cref="PwmSettings7"/>
    [XmlInclude(typeof(PinDirection))]
    [XmlInclude(typeof(PinState))]
    [XmlInclude(typeof(PinSet))]
    [XmlInclude(typeof(PinClear))]
    [XmlInclude(typeof(EnableRisingEdgeEvents))]
    [XmlInclude(typeof(RisingEdgeEvents))]
    [XmlInclude(typeof(EnableFallingEdgeEvents))]
    [XmlInclude(typeof(FallingEdgeEvents))]
    [XmlInclude(typeof(PwmStart))]
    [XmlInclude(typeof(PwmStop))]
    [XmlInclude(typeof(PwmSettings0))]
    [XmlInclude(typeof(PwmSettings1))]
    [XmlInclude(typeof(PwmSettings2))]
    [XmlInclude(typeof(PwmSettings3))]
    [XmlInclude(typeof(PwmSettings4))]
    [XmlInclude(typeof(PwmSettings5))]
    [XmlInclude(typeof(PwmSettings6))]
    [XmlInclude(typeof(PwmSettings7))]
    [Description("Formats a sequence of values as specific Cuttlefish register messages.")]
    public partial class Format : FormatBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format"/> class.
        /// </summary>
        public Format()
        {
            Register = new PinDirection();
        }

        string INamedElement.Name => $"{nameof(Cuttlefish)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents a register that set the direction of the pins. 0 = input; 1 = output.
    /// </summary>
    [Description("Set the direction of the pins. 0 = input; 1 = output")]
    public partial class PinDirection
    {
        /// <summary>
        /// Represents the address of the <see cref="PinDirection"/> register. This field is constant.
        /// </summary>
        public const int Address = 32;

        /// <summary>
        /// Represents the payload type of the <see cref="PinDirection"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PinDirection"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PinDirection"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Pins GetPayload(HarpMessage message)
        {
            return (Pins)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PinDirection"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Pins)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PinDirection"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PinDirection"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PinDirection"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PinDirection"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PinDirection register.
    /// </summary>
    /// <seealso cref="PinDirection"/>
    [Description("Filters and selects timestamped messages from the PinDirection register.")]
    public partial class TimestampedPinDirection
    {
        /// <summary>
        /// Represents the address of the <see cref="PinDirection"/> register. This field is constant.
        /// </summary>
        public const int Address = PinDirection.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PinDirection"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetPayload(HarpMessage message)
        {
            return PinDirection.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that read or write the state of the pins.
    /// </summary>
    [Description("Read or write the state of the pins.")]
    public partial class PinState
    {
        /// <summary>
        /// Represents the address of the <see cref="PinState"/> register. This field is constant.
        /// </summary>
        public const int Address = 33;

        /// <summary>
        /// Represents the payload type of the <see cref="PinState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PinState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PinState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Pins GetPayload(HarpMessage message)
        {
            return (Pins)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PinState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Pins)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PinState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PinState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PinState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PinState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PinState register.
    /// </summary>
    /// <seealso cref="PinState"/>
    [Description("Filters and selects timestamped messages from the PinState register.")]
    public partial class TimestampedPinState
    {
        /// <summary>
        /// Represents the address of the <see cref="PinState"/> register. This field is constant.
        /// </summary>
        public const int Address = PinState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PinState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetPayload(HarpMessage message)
        {
            return PinState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set pins specified in the mask to logic HIGH by setting the corresponding bit.
    /// </summary>
    [Description("Set pins specified in the mask to logic HIGH by setting the corresponding bit.")]
    public partial class PinSet
    {
        /// <summary>
        /// Represents the address of the <see cref="PinSet"/> register. This field is constant.
        /// </summary>
        public const int Address = 34;

        /// <summary>
        /// Represents the payload type of the <see cref="PinSet"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PinSet"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PinSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Pins GetPayload(HarpMessage message)
        {
            return (Pins)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PinSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Pins)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PinSet"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PinSet"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PinSet"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PinSet"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PinSet register.
    /// </summary>
    /// <seealso cref="PinSet"/>
    [Description("Filters and selects timestamped messages from the PinSet register.")]
    public partial class TimestampedPinSet
    {
        /// <summary>
        /// Represents the address of the <see cref="PinSet"/> register. This field is constant.
        /// </summary>
        public const int Address = PinSet.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PinSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetPayload(HarpMessage message)
        {
            return PinSet.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set specified pins in the mask to logic LOW by setting the corresponding bit.
    /// </summary>
    [Description("Set specified pins in the mask to logic LOW by setting the corresponding bit.")]
    public partial class PinClear
    {
        /// <summary>
        /// Represents the address of the <see cref="PinClear"/> register. This field is constant.
        /// </summary>
        public const int Address = 35;

        /// <summary>
        /// Represents the payload type of the <see cref="PinClear"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PinClear"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PinClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Pins GetPayload(HarpMessage message)
        {
            return (Pins)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PinClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Pins)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PinClear"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PinClear"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PinClear"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PinClear"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PinClear register.
    /// </summary>
    /// <seealso cref="PinClear"/>
    [Description("Filters and selects timestamped messages from the PinClear register.")]
    public partial class TimestampedPinClear
    {
        /// <summary>
        /// Represents the address of the <see cref="PinClear"/> register. This field is constant.
        /// </summary>
        public const int Address = PinClear.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PinClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetPayload(HarpMessage message)
        {
            return PinClear.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enable Events from the RisingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic LOW to logic HIGH.
    /// </summary>
    [Description("Enable Events from the RisingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic LOW to logic HIGH.")]
    public partial class EnableRisingEdgeEvents
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableRisingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const int Address = 36;

        /// <summary>
        /// Represents the payload type of the <see cref="EnableRisingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="EnableRisingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EnableRisingEdgeEvents"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Pins GetPayload(HarpMessage message)
        {
            return (Pins)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EnableRisingEdgeEvents"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Pins)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EnableRisingEdgeEvents"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableRisingEdgeEvents"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EnableRisingEdgeEvents"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableRisingEdgeEvents"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EnableRisingEdgeEvents register.
    /// </summary>
    /// <seealso cref="EnableRisingEdgeEvents"/>
    [Description("Filters and selects timestamped messages from the EnableRisingEdgeEvents register.")]
    public partial class TimestampedEnableRisingEdgeEvents
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableRisingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const int Address = EnableRisingEdgeEvents.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EnableRisingEdgeEvents"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetPayload(HarpMessage message)
        {
            return EnableRisingEdgeEvents.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic LOW to logic HIGH.
    /// </summary>
    [Description("Event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic LOW to logic HIGH.")]
    public partial class RisingEdgeEvents
    {
        /// <summary>
        /// Represents the address of the <see cref="RisingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const int Address = 37;

        /// <summary>
        /// Represents the payload type of the <see cref="RisingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="RisingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="RisingEdgeEvents"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Pins GetPayload(HarpMessage message)
        {
            return (Pins)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="RisingEdgeEvents"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Pins)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="RisingEdgeEvents"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RisingEdgeEvents"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="RisingEdgeEvents"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RisingEdgeEvents"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// RisingEdgeEvents register.
    /// </summary>
    /// <seealso cref="RisingEdgeEvents"/>
    [Description("Filters and selects timestamped messages from the RisingEdgeEvents register.")]
    public partial class TimestampedRisingEdgeEvents
    {
        /// <summary>
        /// Represents the address of the <see cref="RisingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const int Address = RisingEdgeEvents.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="RisingEdgeEvents"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetPayload(HarpMessage message)
        {
            return RisingEdgeEvents.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enable Events from the FallingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic HIGH to logic LOW.
    /// </summary>
    [Description("Enable Events from the FallingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic HIGH to logic LOW.")]
    public partial class EnableFallingEdgeEvents
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableFallingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const int Address = 38;

        /// <summary>
        /// Represents the payload type of the <see cref="EnableFallingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="EnableFallingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EnableFallingEdgeEvents"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Pins GetPayload(HarpMessage message)
        {
            return (Pins)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EnableFallingEdgeEvents"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Pins)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EnableFallingEdgeEvents"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableFallingEdgeEvents"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EnableFallingEdgeEvents"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableFallingEdgeEvents"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EnableFallingEdgeEvents register.
    /// </summary>
    /// <seealso cref="EnableFallingEdgeEvents"/>
    [Description("Filters and selects timestamped messages from the EnableFallingEdgeEvents register.")]
    public partial class TimestampedEnableFallingEdgeEvents
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableFallingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const int Address = EnableFallingEdgeEvents.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EnableFallingEdgeEvents"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetPayload(HarpMessage message)
        {
            return EnableFallingEdgeEvents.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic HIGH to logic LOW.
    /// </summary>
    [Description("Event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic HIGH to logic LOW.")]
    public partial class FallingEdgeEvents
    {
        /// <summary>
        /// Represents the address of the <see cref="FallingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const int Address = 39;

        /// <summary>
        /// Represents the payload type of the <see cref="FallingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="FallingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="FallingEdgeEvents"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Pins GetPayload(HarpMessage message)
        {
            return (Pins)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="FallingEdgeEvents"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Pins)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="FallingEdgeEvents"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FallingEdgeEvents"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="FallingEdgeEvents"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FallingEdgeEvents"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// FallingEdgeEvents register.
    /// </summary>
    /// <seealso cref="FallingEdgeEvents"/>
    [Description("Filters and selects timestamped messages from the FallingEdgeEvents register.")]
    public partial class TimestampedFallingEdgeEvents
    {
        /// <summary>
        /// Represents the address of the <see cref="FallingEdgeEvents"/> register. This field is constant.
        /// </summary>
        public const int Address = FallingEdgeEvents.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="FallingEdgeEvents"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetPayload(HarpMessage message)
        {
            return FallingEdgeEvents.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that start the associated PWM channel for the pins specified in the mask.
    /// </summary>
    [Description("Start the associated PWM channel for the pins specified in the mask.")]
    public partial class PwmStart
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmStart"/> register. This field is constant.
        /// </summary>
        public const int Address = 40;

        /// <summary>
        /// Represents the payload type of the <see cref="PwmStart"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PwmStart"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PwmStart"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Pins GetPayload(HarpMessage message)
        {
            return (Pins)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PwmStart"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Pins)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PwmStart"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmStart"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PwmStart"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmStart"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PwmStart register.
    /// </summary>
    /// <seealso cref="PwmStart"/>
    [Description("Filters and selects timestamped messages from the PwmStart register.")]
    public partial class TimestampedPwmStart
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmStart"/> register. This field is constant.
        /// </summary>
        public const int Address = PwmStart.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PwmStart"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetPayload(HarpMessage message)
        {
            return PwmStart.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that stop the associated PWM channel for the pins specified in the mask.
    /// </summary>
    [Description("Stop the associated PWM channel for the pins specified in the mask.")]
    public partial class PwmStop
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmStop"/> register. This field is constant.
        /// </summary>
        public const int Address = 41;

        /// <summary>
        /// Represents the payload type of the <see cref="PwmStop"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PwmStop"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PwmStop"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Pins GetPayload(HarpMessage message)
        {
            return (Pins)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PwmStop"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Pins)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PwmStop"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmStop"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PwmStop"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmStop"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Pins value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PwmStop register.
    /// </summary>
    /// <seealso cref="PwmStop"/>
    [Description("Filters and selects timestamped messages from the PwmStop register.")]
    public partial class TimestampedPwmStop
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmStop"/> register. This field is constant.
        /// </summary>
        public const int Address = PwmStop.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PwmStop"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Pins> GetPayload(HarpMessage message)
        {
            return PwmStop.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that struct to configure PWM0 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [Description("Struct to configure PWM0 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)")]
    public partial class PwmSettings0
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings0"/> register. This field is constant.
        /// </summary>
        public const int Address = 42;

        /// <summary>
        /// Represents the payload type of the <see cref="PwmSettings0"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PwmSettings0"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 17;

        /// <summary>
        /// Returns the payload data for <see cref="PwmSettings0"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PwmSettings0"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PwmSettings0"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings0"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PwmSettings0"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings0"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PwmSettings0 register.
    /// </summary>
    /// <seealso cref="PwmSettings0"/>
    [Description("Filters and selects timestamped messages from the PwmSettings0 register.")]
    public partial class TimestampedPwmSettings0
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings0"/> register. This field is constant.
        /// </summary>
        public const int Address = PwmSettings0.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PwmSettings0"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return PwmSettings0.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that struct to configure PWM1 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [Description("Struct to configure PWM1 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)")]
    public partial class PwmSettings1
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings1"/> register. This field is constant.
        /// </summary>
        public const int Address = 43;

        /// <summary>
        /// Represents the payload type of the <see cref="PwmSettings1"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PwmSettings1"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 17;

        /// <summary>
        /// Returns the payload data for <see cref="PwmSettings1"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PwmSettings1"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PwmSettings1"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings1"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PwmSettings1"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings1"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PwmSettings1 register.
    /// </summary>
    /// <seealso cref="PwmSettings1"/>
    [Description("Filters and selects timestamped messages from the PwmSettings1 register.")]
    public partial class TimestampedPwmSettings1
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings1"/> register. This field is constant.
        /// </summary>
        public const int Address = PwmSettings1.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PwmSettings1"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return PwmSettings1.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that struct to configure PWM2 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [Description("Struct to configure PWM2 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)")]
    public partial class PwmSettings2
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings2"/> register. This field is constant.
        /// </summary>
        public const int Address = 44;

        /// <summary>
        /// Represents the payload type of the <see cref="PwmSettings2"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PwmSettings2"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 17;

        /// <summary>
        /// Returns the payload data for <see cref="PwmSettings2"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PwmSettings2"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PwmSettings2"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings2"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PwmSettings2"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings2"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PwmSettings2 register.
    /// </summary>
    /// <seealso cref="PwmSettings2"/>
    [Description("Filters and selects timestamped messages from the PwmSettings2 register.")]
    public partial class TimestampedPwmSettings2
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings2"/> register. This field is constant.
        /// </summary>
        public const int Address = PwmSettings2.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PwmSettings2"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return PwmSettings2.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that struct to configure PWM3 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [Description("Struct to configure PWM3 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)")]
    public partial class PwmSettings3
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings3"/> register. This field is constant.
        /// </summary>
        public const int Address = 45;

        /// <summary>
        /// Represents the payload type of the <see cref="PwmSettings3"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PwmSettings3"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 17;

        /// <summary>
        /// Returns the payload data for <see cref="PwmSettings3"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PwmSettings3"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PwmSettings3"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings3"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PwmSettings3"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings3"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PwmSettings3 register.
    /// </summary>
    /// <seealso cref="PwmSettings3"/>
    [Description("Filters and selects timestamped messages from the PwmSettings3 register.")]
    public partial class TimestampedPwmSettings3
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings3"/> register. This field is constant.
        /// </summary>
        public const int Address = PwmSettings3.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PwmSettings3"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return PwmSettings3.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that struct to configure PWM4 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [Description("Struct to configure PWM4 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)")]
    public partial class PwmSettings4
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings4"/> register. This field is constant.
        /// </summary>
        public const int Address = 46;

        /// <summary>
        /// Represents the payload type of the <see cref="PwmSettings4"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PwmSettings4"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 17;

        /// <summary>
        /// Returns the payload data for <see cref="PwmSettings4"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PwmSettings4"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PwmSettings4"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings4"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PwmSettings4"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings4"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PwmSettings4 register.
    /// </summary>
    /// <seealso cref="PwmSettings4"/>
    [Description("Filters and selects timestamped messages from the PwmSettings4 register.")]
    public partial class TimestampedPwmSettings4
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings4"/> register. This field is constant.
        /// </summary>
        public const int Address = PwmSettings4.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PwmSettings4"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return PwmSettings4.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that struct to configure PWM5 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [Description("Struct to configure PWM5 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)")]
    public partial class PwmSettings5
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings5"/> register. This field is constant.
        /// </summary>
        public const int Address = 47;

        /// <summary>
        /// Represents the payload type of the <see cref="PwmSettings5"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PwmSettings5"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 17;

        /// <summary>
        /// Returns the payload data for <see cref="PwmSettings5"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PwmSettings5"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PwmSettings5"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings5"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PwmSettings5"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings5"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PwmSettings5 register.
    /// </summary>
    /// <seealso cref="PwmSettings5"/>
    [Description("Filters and selects timestamped messages from the PwmSettings5 register.")]
    public partial class TimestampedPwmSettings5
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings5"/> register. This field is constant.
        /// </summary>
        public const int Address = PwmSettings5.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PwmSettings5"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return PwmSettings5.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that struct to configure PWM6 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [Description("Struct to configure PWM6 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)")]
    public partial class PwmSettings6
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings6"/> register. This field is constant.
        /// </summary>
        public const int Address = 48;

        /// <summary>
        /// Represents the payload type of the <see cref="PwmSettings6"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PwmSettings6"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 17;

        /// <summary>
        /// Returns the payload data for <see cref="PwmSettings6"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PwmSettings6"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PwmSettings6"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings6"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PwmSettings6"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings6"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PwmSettings6 register.
    /// </summary>
    /// <seealso cref="PwmSettings6"/>
    [Description("Filters and selects timestamped messages from the PwmSettings6 register.")]
    public partial class TimestampedPwmSettings6
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings6"/> register. This field is constant.
        /// </summary>
        public const int Address = PwmSettings6.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PwmSettings6"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return PwmSettings6.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that struct to configure PWM7 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [Description("Struct to configure PWM7 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)")]
    public partial class PwmSettings7
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings7"/> register. This field is constant.
        /// </summary>
        public const int Address = 49;

        /// <summary>
        /// Represents the payload type of the <see cref="PwmSettings7"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PwmSettings7"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 17;

        /// <summary>
        /// Returns the payload data for <see cref="PwmSettings7"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte[] GetPayload(HarpMessage message)
        {
            return message.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PwmSettings7"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadArray<byte>();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PwmSettings7"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings7"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PwmSettings7"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PwmSettings7"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte[] value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PwmSettings7 register.
    /// </summary>
    /// <seealso cref="PwmSettings7"/>
    [Description("Filters and selects timestamped messages from the PwmSettings7 register.")]
    public partial class TimestampedPwmSettings7
    {
        /// <summary>
        /// Represents the address of the <see cref="PwmSettings7"/> register. This field is constant.
        /// </summary>
        public const int Address = PwmSettings7.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PwmSettings7"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte[]> GetPayload(HarpMessage message)
        {
            return PwmSettings7.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents an operator which creates standard message payloads for the
    /// Cuttlefish device.
    /// </summary>
    /// <seealso cref="CreatePinDirectionPayload"/>
    /// <seealso cref="CreatePinStatePayload"/>
    /// <seealso cref="CreatePinSetPayload"/>
    /// <seealso cref="CreatePinClearPayload"/>
    /// <seealso cref="CreateEnableRisingEdgeEventsPayload"/>
    /// <seealso cref="CreateRisingEdgeEventsPayload"/>
    /// <seealso cref="CreateEnableFallingEdgeEventsPayload"/>
    /// <seealso cref="CreateFallingEdgeEventsPayload"/>
    /// <seealso cref="CreatePwmStartPayload"/>
    /// <seealso cref="CreatePwmStopPayload"/>
    /// <seealso cref="CreatePwmSettings0Payload"/>
    /// <seealso cref="CreatePwmSettings1Payload"/>
    /// <seealso cref="CreatePwmSettings2Payload"/>
    /// <seealso cref="CreatePwmSettings3Payload"/>
    /// <seealso cref="CreatePwmSettings4Payload"/>
    /// <seealso cref="CreatePwmSettings5Payload"/>
    /// <seealso cref="CreatePwmSettings6Payload"/>
    /// <seealso cref="CreatePwmSettings7Payload"/>
    [XmlInclude(typeof(CreatePinDirectionPayload))]
    [XmlInclude(typeof(CreatePinStatePayload))]
    [XmlInclude(typeof(CreatePinSetPayload))]
    [XmlInclude(typeof(CreatePinClearPayload))]
    [XmlInclude(typeof(CreateEnableRisingEdgeEventsPayload))]
    [XmlInclude(typeof(CreateRisingEdgeEventsPayload))]
    [XmlInclude(typeof(CreateEnableFallingEdgeEventsPayload))]
    [XmlInclude(typeof(CreateFallingEdgeEventsPayload))]
    [XmlInclude(typeof(CreatePwmStartPayload))]
    [XmlInclude(typeof(CreatePwmStopPayload))]
    [XmlInclude(typeof(CreatePwmSettings0Payload))]
    [XmlInclude(typeof(CreatePwmSettings1Payload))]
    [XmlInclude(typeof(CreatePwmSettings2Payload))]
    [XmlInclude(typeof(CreatePwmSettings3Payload))]
    [XmlInclude(typeof(CreatePwmSettings4Payload))]
    [XmlInclude(typeof(CreatePwmSettings5Payload))]
    [XmlInclude(typeof(CreatePwmSettings6Payload))]
    [XmlInclude(typeof(CreatePwmSettings7Payload))]
    [XmlInclude(typeof(CreateTimestampedPinDirectionPayload))]
    [XmlInclude(typeof(CreateTimestampedPinStatePayload))]
    [XmlInclude(typeof(CreateTimestampedPinSetPayload))]
    [XmlInclude(typeof(CreateTimestampedPinClearPayload))]
    [XmlInclude(typeof(CreateTimestampedEnableRisingEdgeEventsPayload))]
    [XmlInclude(typeof(CreateTimestampedRisingEdgeEventsPayload))]
    [XmlInclude(typeof(CreateTimestampedEnableFallingEdgeEventsPayload))]
    [XmlInclude(typeof(CreateTimestampedFallingEdgeEventsPayload))]
    [XmlInclude(typeof(CreateTimestampedPwmStartPayload))]
    [XmlInclude(typeof(CreateTimestampedPwmStopPayload))]
    [XmlInclude(typeof(CreateTimestampedPwmSettings0Payload))]
    [XmlInclude(typeof(CreateTimestampedPwmSettings1Payload))]
    [XmlInclude(typeof(CreateTimestampedPwmSettings2Payload))]
    [XmlInclude(typeof(CreateTimestampedPwmSettings3Payload))]
    [XmlInclude(typeof(CreateTimestampedPwmSettings4Payload))]
    [XmlInclude(typeof(CreateTimestampedPwmSettings5Payload))]
    [XmlInclude(typeof(CreateTimestampedPwmSettings6Payload))]
    [XmlInclude(typeof(CreateTimestampedPwmSettings7Payload))]
    [Description("Creates standard message payloads for the Cuttlefish device.")]
    public partial class CreateMessage : CreateMessageBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMessage"/> class.
        /// </summary>
        public CreateMessage()
        {
            Payload = new CreatePinDirectionPayload();
        }

        string INamedElement.Name => $"{nameof(Cuttlefish)}.{GetElementDisplayName(Payload)}";
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the direction of the pins. 0 = input; 1 = output.
    /// </summary>
    [DisplayName("PinDirectionPayload")]
    [Description("Creates a message payload that set the direction of the pins. 0 = input; 1 = output.")]
    public partial class CreatePinDirectionPayload
    {
        /// <summary>
        /// Gets or sets the value that set the direction of the pins. 0 = input; 1 = output.
        /// </summary>
        [Description("The value that set the direction of the pins. 0 = input; 1 = output.")]
        public Pins PinDirection { get; set; }

        /// <summary>
        /// Creates a message payload for the PinDirection register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Pins GetPayload()
        {
            return PinDirection;
        }

        /// <summary>
        /// Creates a message that set the direction of the pins. 0 = input; 1 = output.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PinDirection register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PinDirection.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the direction of the pins. 0 = input; 1 = output.
    /// </summary>
    [DisplayName("TimestampedPinDirectionPayload")]
    [Description("Creates a timestamped message payload that set the direction of the pins. 0 = input; 1 = output.")]
    public partial class CreateTimestampedPinDirectionPayload : CreatePinDirectionPayload
    {
        /// <summary>
        /// Creates a timestamped message that set the direction of the pins. 0 = input; 1 = output.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PinDirection register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PinDirection.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that read or write the state of the pins.
    /// </summary>
    [DisplayName("PinStatePayload")]
    [Description("Creates a message payload that read or write the state of the pins.")]
    public partial class CreatePinStatePayload
    {
        /// <summary>
        /// Gets or sets the value that read or write the state of the pins.
        /// </summary>
        [Description("The value that read or write the state of the pins.")]
        public Pins PinState { get; set; }

        /// <summary>
        /// Creates a message payload for the PinState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Pins GetPayload()
        {
            return PinState;
        }

        /// <summary>
        /// Creates a message that read or write the state of the pins.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PinState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PinState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that read or write the state of the pins.
    /// </summary>
    [DisplayName("TimestampedPinStatePayload")]
    [Description("Creates a timestamped message payload that read or write the state of the pins.")]
    public partial class CreateTimestampedPinStatePayload : CreatePinStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that read or write the state of the pins.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PinState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PinState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set pins specified in the mask to logic HIGH by setting the corresponding bit.
    /// </summary>
    [DisplayName("PinSetPayload")]
    [Description("Creates a message payload that set pins specified in the mask to logic HIGH by setting the corresponding bit.")]
    public partial class CreatePinSetPayload
    {
        /// <summary>
        /// Gets or sets the value that set pins specified in the mask to logic HIGH by setting the corresponding bit.
        /// </summary>
        [Description("The value that set pins specified in the mask to logic HIGH by setting the corresponding bit.")]
        public Pins PinSet { get; set; }

        /// <summary>
        /// Creates a message payload for the PinSet register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Pins GetPayload()
        {
            return PinSet;
        }

        /// <summary>
        /// Creates a message that set pins specified in the mask to logic HIGH by setting the corresponding bit.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PinSet register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PinSet.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set pins specified in the mask to logic HIGH by setting the corresponding bit.
    /// </summary>
    [DisplayName("TimestampedPinSetPayload")]
    [Description("Creates a timestamped message payload that set pins specified in the mask to logic HIGH by setting the corresponding bit.")]
    public partial class CreateTimestampedPinSetPayload : CreatePinSetPayload
    {
        /// <summary>
        /// Creates a timestamped message that set pins specified in the mask to logic HIGH by setting the corresponding bit.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PinSet register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PinSet.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set specified pins in the mask to logic LOW by setting the corresponding bit.
    /// </summary>
    [DisplayName("PinClearPayload")]
    [Description("Creates a message payload that set specified pins in the mask to logic LOW by setting the corresponding bit.")]
    public partial class CreatePinClearPayload
    {
        /// <summary>
        /// Gets or sets the value that set specified pins in the mask to logic LOW by setting the corresponding bit.
        /// </summary>
        [Description("The value that set specified pins in the mask to logic LOW by setting the corresponding bit.")]
        public Pins PinClear { get; set; }

        /// <summary>
        /// Creates a message payload for the PinClear register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Pins GetPayload()
        {
            return PinClear;
        }

        /// <summary>
        /// Creates a message that set specified pins in the mask to logic LOW by setting the corresponding bit.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PinClear register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PinClear.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set specified pins in the mask to logic LOW by setting the corresponding bit.
    /// </summary>
    [DisplayName("TimestampedPinClearPayload")]
    [Description("Creates a timestamped message payload that set specified pins in the mask to logic LOW by setting the corresponding bit.")]
    public partial class CreateTimestampedPinClearPayload : CreatePinClearPayload
    {
        /// <summary>
        /// Creates a timestamped message that set specified pins in the mask to logic LOW by setting the corresponding bit.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PinClear register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PinClear.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enable Events from the RisingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic LOW to logic HIGH.
    /// </summary>
    [DisplayName("EnableRisingEdgeEventsPayload")]
    [Description("Creates a message payload that enable Events from the RisingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic LOW to logic HIGH.")]
    public partial class CreateEnableRisingEdgeEventsPayload
    {
        /// <summary>
        /// Gets or sets the value that enable Events from the RisingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic LOW to logic HIGH.
        /// </summary>
        [Description("The value that enable Events from the RisingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic LOW to logic HIGH.")]
        public Pins EnableRisingEdgeEvents { get; set; }

        /// <summary>
        /// Creates a message payload for the EnableRisingEdgeEvents register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Pins GetPayload()
        {
            return EnableRisingEdgeEvents;
        }

        /// <summary>
        /// Creates a message that enable Events from the RisingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic LOW to logic HIGH.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EnableRisingEdgeEvents register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.EnableRisingEdgeEvents.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enable Events from the RisingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic LOW to logic HIGH.
    /// </summary>
    [DisplayName("TimestampedEnableRisingEdgeEventsPayload")]
    [Description("Creates a timestamped message payload that enable Events from the RisingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic LOW to logic HIGH.")]
    public partial class CreateTimestampedEnableRisingEdgeEventsPayload : CreateEnableRisingEdgeEventsPayload
    {
        /// <summary>
        /// Creates a timestamped message that enable Events from the RisingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic LOW to logic HIGH.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EnableRisingEdgeEvents register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.EnableRisingEdgeEvents.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic LOW to logic HIGH.
    /// </summary>
    [DisplayName("RisingEdgeEventsPayload")]
    [Description("Creates a message payload that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic LOW to logic HIGH.")]
    public partial class CreateRisingEdgeEventsPayload
    {
        /// <summary>
        /// Gets or sets the value that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic LOW to logic HIGH.
        /// </summary>
        [Description("The value that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic LOW to logic HIGH.")]
        public Pins RisingEdgeEvents { get; set; }

        /// <summary>
        /// Creates a message payload for the RisingEdgeEvents register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Pins GetPayload()
        {
            return RisingEdgeEvents;
        }

        /// <summary>
        /// Creates a message that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic LOW to logic HIGH.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the RisingEdgeEvents register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.RisingEdgeEvents.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic LOW to logic HIGH.
    /// </summary>
    [DisplayName("TimestampedRisingEdgeEventsPayload")]
    [Description("Creates a timestamped message payload that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic LOW to logic HIGH.")]
    public partial class CreateTimestampedRisingEdgeEventsPayload : CreateRisingEdgeEventsPayload
    {
        /// <summary>
        /// Creates a timestamped message that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic LOW to logic HIGH.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the RisingEdgeEvents register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.RisingEdgeEvents.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enable Events from the FallingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic HIGH to logic LOW.
    /// </summary>
    [DisplayName("EnableFallingEdgeEventsPayload")]
    [Description("Creates a message payload that enable Events from the FallingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic HIGH to logic LOW.")]
    public partial class CreateEnableFallingEdgeEventsPayload
    {
        /// <summary>
        /// Gets or sets the value that enable Events from the FallingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic HIGH to logic LOW.
        /// </summary>
        [Description("The value that enable Events from the FallingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic HIGH to logic LOW.")]
        public Pins EnableFallingEdgeEvents { get; set; }

        /// <summary>
        /// Creates a message payload for the EnableFallingEdgeEvents register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Pins GetPayload()
        {
            return EnableFallingEdgeEvents;
        }

        /// <summary>
        /// Creates a message that enable Events from the FallingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic HIGH to logic LOW.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EnableFallingEdgeEvents register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.EnableFallingEdgeEvents.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enable Events from the FallingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic HIGH to logic LOW.
    /// </summary>
    [DisplayName("TimestampedEnableFallingEdgeEventsPayload")]
    [Description("Creates a timestamped message payload that enable Events from the FallingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic HIGH to logic LOW.")]
    public partial class CreateTimestampedEnableFallingEdgeEventsPayload : CreateEnableFallingEdgeEventsPayload
    {
        /// <summary>
        /// Creates a timestamped message that enable Events from the FallingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic HIGH to logic LOW.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EnableFallingEdgeEvents register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.EnableFallingEdgeEvents.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic HIGH to logic LOW.
    /// </summary>
    [DisplayName("FallingEdgeEventsPayload")]
    [Description("Creates a message payload that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic HIGH to logic LOW.")]
    public partial class CreateFallingEdgeEventsPayload
    {
        /// <summary>
        /// Gets or sets the value that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic HIGH to logic LOW.
        /// </summary>
        [Description("The value that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic HIGH to logic LOW.")]
        public Pins FallingEdgeEvents { get; set; }

        /// <summary>
        /// Creates a message payload for the FallingEdgeEvents register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Pins GetPayload()
        {
            return FallingEdgeEvents;
        }

        /// <summary>
        /// Creates a message that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic HIGH to logic LOW.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the FallingEdgeEvents register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.FallingEdgeEvents.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic HIGH to logic LOW.
    /// </summary>
    [DisplayName("TimestampedFallingEdgeEventsPayload")]
    [Description("Creates a timestamped message payload that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic HIGH to logic LOW.")]
    public partial class CreateTimestampedFallingEdgeEventsPayload : CreateFallingEdgeEventsPayload
    {
        /// <summary>
        /// Creates a timestamped message that event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic HIGH to logic LOW.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the FallingEdgeEvents register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.FallingEdgeEvents.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that start the associated PWM channel for the pins specified in the mask.
    /// </summary>
    [DisplayName("PwmStartPayload")]
    [Description("Creates a message payload that start the associated PWM channel for the pins specified in the mask.")]
    public partial class CreatePwmStartPayload
    {
        /// <summary>
        /// Gets or sets the value that start the associated PWM channel for the pins specified in the mask.
        /// </summary>
        [Description("The value that start the associated PWM channel for the pins specified in the mask.")]
        public Pins PwmStart { get; set; }

        /// <summary>
        /// Creates a message payload for the PwmStart register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Pins GetPayload()
        {
            return PwmStart;
        }

        /// <summary>
        /// Creates a message that start the associated PWM channel for the pins specified in the mask.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PwmStart register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmStart.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that start the associated PWM channel for the pins specified in the mask.
    /// </summary>
    [DisplayName("TimestampedPwmStartPayload")]
    [Description("Creates a timestamped message payload that start the associated PWM channel for the pins specified in the mask.")]
    public partial class CreateTimestampedPwmStartPayload : CreatePwmStartPayload
    {
        /// <summary>
        /// Creates a timestamped message that start the associated PWM channel for the pins specified in the mask.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PwmStart register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmStart.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that stop the associated PWM channel for the pins specified in the mask.
    /// </summary>
    [DisplayName("PwmStopPayload")]
    [Description("Creates a message payload that stop the associated PWM channel for the pins specified in the mask.")]
    public partial class CreatePwmStopPayload
    {
        /// <summary>
        /// Gets or sets the value that stop the associated PWM channel for the pins specified in the mask.
        /// </summary>
        [Description("The value that stop the associated PWM channel for the pins specified in the mask.")]
        public Pins PwmStop { get; set; }

        /// <summary>
        /// Creates a message payload for the PwmStop register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Pins GetPayload()
        {
            return PwmStop;
        }

        /// <summary>
        /// Creates a message that stop the associated PWM channel for the pins specified in the mask.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PwmStop register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmStop.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that stop the associated PWM channel for the pins specified in the mask.
    /// </summary>
    [DisplayName("TimestampedPwmStopPayload")]
    [Description("Creates a timestamped message payload that stop the associated PWM channel for the pins specified in the mask.")]
    public partial class CreateTimestampedPwmStopPayload : CreatePwmStopPayload
    {
        /// <summary>
        /// Creates a timestamped message that stop the associated PWM channel for the pins specified in the mask.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PwmStop register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmStop.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that struct to configure PWM0 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("PwmSettings0Payload")]
    [Description("Creates a message payload that struct to configure PWM0 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreatePwmSettings0Payload
    {
        /// <summary>
        /// Gets or sets the value that struct to configure PWM0 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        [Description("The value that struct to configure PWM0 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
        public byte[] PwmSettings0 { get; set; }

        /// <summary>
        /// Creates a message payload for the PwmSettings0 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return PwmSettings0;
        }

        /// <summary>
        /// Creates a message that struct to configure PWM0 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PwmSettings0 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings0.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that struct to configure PWM0 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("TimestampedPwmSettings0Payload")]
    [Description("Creates a timestamped message payload that struct to configure PWM0 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreateTimestampedPwmSettings0Payload : CreatePwmSettings0Payload
    {
        /// <summary>
        /// Creates a timestamped message that struct to configure PWM0 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PwmSettings0 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings0.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that struct to configure PWM1 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("PwmSettings1Payload")]
    [Description("Creates a message payload that struct to configure PWM1 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreatePwmSettings1Payload
    {
        /// <summary>
        /// Gets or sets the value that struct to configure PWM1 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        [Description("The value that struct to configure PWM1 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
        public byte[] PwmSettings1 { get; set; }

        /// <summary>
        /// Creates a message payload for the PwmSettings1 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return PwmSettings1;
        }

        /// <summary>
        /// Creates a message that struct to configure PWM1 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PwmSettings1 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings1.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that struct to configure PWM1 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("TimestampedPwmSettings1Payload")]
    [Description("Creates a timestamped message payload that struct to configure PWM1 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreateTimestampedPwmSettings1Payload : CreatePwmSettings1Payload
    {
        /// <summary>
        /// Creates a timestamped message that struct to configure PWM1 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PwmSettings1 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings1.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that struct to configure PWM2 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("PwmSettings2Payload")]
    [Description("Creates a message payload that struct to configure PWM2 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreatePwmSettings2Payload
    {
        /// <summary>
        /// Gets or sets the value that struct to configure PWM2 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        [Description("The value that struct to configure PWM2 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
        public byte[] PwmSettings2 { get; set; }

        /// <summary>
        /// Creates a message payload for the PwmSettings2 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return PwmSettings2;
        }

        /// <summary>
        /// Creates a message that struct to configure PWM2 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PwmSettings2 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings2.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that struct to configure PWM2 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("TimestampedPwmSettings2Payload")]
    [Description("Creates a timestamped message payload that struct to configure PWM2 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreateTimestampedPwmSettings2Payload : CreatePwmSettings2Payload
    {
        /// <summary>
        /// Creates a timestamped message that struct to configure PWM2 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PwmSettings2 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings2.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that struct to configure PWM3 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("PwmSettings3Payload")]
    [Description("Creates a message payload that struct to configure PWM3 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreatePwmSettings3Payload
    {
        /// <summary>
        /// Gets or sets the value that struct to configure PWM3 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        [Description("The value that struct to configure PWM3 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
        public byte[] PwmSettings3 { get; set; }

        /// <summary>
        /// Creates a message payload for the PwmSettings3 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return PwmSettings3;
        }

        /// <summary>
        /// Creates a message that struct to configure PWM3 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PwmSettings3 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings3.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that struct to configure PWM3 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("TimestampedPwmSettings3Payload")]
    [Description("Creates a timestamped message payload that struct to configure PWM3 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreateTimestampedPwmSettings3Payload : CreatePwmSettings3Payload
    {
        /// <summary>
        /// Creates a timestamped message that struct to configure PWM3 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PwmSettings3 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings3.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that struct to configure PWM4 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("PwmSettings4Payload")]
    [Description("Creates a message payload that struct to configure PWM4 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreatePwmSettings4Payload
    {
        /// <summary>
        /// Gets or sets the value that struct to configure PWM4 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        [Description("The value that struct to configure PWM4 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
        public byte[] PwmSettings4 { get; set; }

        /// <summary>
        /// Creates a message payload for the PwmSettings4 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return PwmSettings4;
        }

        /// <summary>
        /// Creates a message that struct to configure PWM4 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PwmSettings4 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings4.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that struct to configure PWM4 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("TimestampedPwmSettings4Payload")]
    [Description("Creates a timestamped message payload that struct to configure PWM4 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreateTimestampedPwmSettings4Payload : CreatePwmSettings4Payload
    {
        /// <summary>
        /// Creates a timestamped message that struct to configure PWM4 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PwmSettings4 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings4.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that struct to configure PWM5 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("PwmSettings5Payload")]
    [Description("Creates a message payload that struct to configure PWM5 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreatePwmSettings5Payload
    {
        /// <summary>
        /// Gets or sets the value that struct to configure PWM5 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        [Description("The value that struct to configure PWM5 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
        public byte[] PwmSettings5 { get; set; }

        /// <summary>
        /// Creates a message payload for the PwmSettings5 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return PwmSettings5;
        }

        /// <summary>
        /// Creates a message that struct to configure PWM5 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PwmSettings5 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings5.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that struct to configure PWM5 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("TimestampedPwmSettings5Payload")]
    [Description("Creates a timestamped message payload that struct to configure PWM5 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreateTimestampedPwmSettings5Payload : CreatePwmSettings5Payload
    {
        /// <summary>
        /// Creates a timestamped message that struct to configure PWM5 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PwmSettings5 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings5.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that struct to configure PWM6 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("PwmSettings6Payload")]
    [Description("Creates a message payload that struct to configure PWM6 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreatePwmSettings6Payload
    {
        /// <summary>
        /// Gets or sets the value that struct to configure PWM6 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        [Description("The value that struct to configure PWM6 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
        public byte[] PwmSettings6 { get; set; }

        /// <summary>
        /// Creates a message payload for the PwmSettings6 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return PwmSettings6;
        }

        /// <summary>
        /// Creates a message that struct to configure PWM6 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PwmSettings6 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings6.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that struct to configure PWM6 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("TimestampedPwmSettings6Payload")]
    [Description("Creates a timestamped message payload that struct to configure PWM6 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreateTimestampedPwmSettings6Payload : CreatePwmSettings6Payload
    {
        /// <summary>
        /// Creates a timestamped message that struct to configure PWM6 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PwmSettings6 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings6.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that struct to configure PWM7 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("PwmSettings7Payload")]
    [Description("Creates a message payload that struct to configure PWM7 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreatePwmSettings7Payload
    {
        /// <summary>
        /// Gets or sets the value that struct to configure PWM7 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        [Description("The value that struct to configure PWM7 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
        public byte[] PwmSettings7 { get; set; }

        /// <summary>
        /// Creates a message payload for the PwmSettings7 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte[] GetPayload()
        {
            return PwmSettings7;
        }

        /// <summary>
        /// Creates a message that struct to configure PWM7 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PwmSettings7 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings7.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that struct to configure PWM7 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
    /// </summary>
    [DisplayName("TimestampedPwmSettings7Payload")]
    [Description("Creates a timestamped message payload that struct to configure PWM7 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).")]
    public partial class CreateTimestampedPwmSettings7Payload : CreatePwmSettings7Payload
    {
        /// <summary>
        /// Creates a timestamped message that struct to configure PWM7 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8).
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PwmSettings7 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Cuttlefish.PwmSettings7.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Available pins on the device
    /// </summary>
    [Flags]
    public enum Pins : byte
    {
        None = 0x0,
        Pin0 = 0x1,
        Pin1 = 0x2,
        Pin2 = 0x4,
        Pin3 = 0x8,
        Pin4 = 0x10,
        Pin5 = 0x20,
        Pin6 = 0x40,
        Pin7 = 0x80
    }
}
