using Bonsai;
using Bonsai.Harp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Xml.Serialization;

namespace AllenNeuralDynamics.CuttlefishCamTrigger
{
    /// <summary>
    /// Generates events and processes commands for the Cuttlefish-Cam-Trigger device connected
    /// at the specified serial port.
    /// </summary>
    [Combinator(MethodName = nameof(Generate))]
    [WorkflowElementCategory(ElementCategory.Source)]
    [Description("Generates events and processes commands for the Cuttlefish-Cam-Trigger device.")]
    public partial class Device : Bonsai.Harp.Device, INamedElement
    {
        /// <summary>
        /// Represents the unique identity class of the <see cref="Cuttlefish-Cam-Trigger"/> device.
        /// This field is constant.
        /// </summary>
        public const int WhoAmI = 1408;

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        public Device() : base(WhoAmI) { }

        string INamedElement.Name => nameof(Cuttlefish-Cam-Trigger);

        /// <summary>
        /// Gets a read-only mapping from address to register type.
        /// </summary>
        public static new IReadOnlyDictionary<int, Type> RegisterMap { get; } = new Dictionary<int, Type>
            (Bonsai.Harp.Device.RegisterMap.ToDictionary(entry => entry.Key, entry => entry.Value))
        {
            { 32, typeof(EnableOutput) },
            { 33, typeof(InvertOutputMask) },
            { 34, typeof(EnableRisingEdgeEvent) },
            { 35, typeof(RisingEdgeEvent) },
            { 36, typeof(Port0FrequencyHz) },
            { 38, typeof(Port1FrequencyHz) },
            { 40, typeof(Port2FrequencyHz) },
            { 42, typeof(Port3FrequencyHz) },
            { 44, typeof(Port4FrequencyHz) },
            { 46, typeof(Port5FrequencyHz) },
            { 48, typeof(Port6FrequencyHz) },
            { 50, typeof(Port7FrequencyHz) }
        };

        /// <summary>
        /// Gets the contents of the metadata file describing the <see cref="Cuttlefish-Cam-Trigger"/>
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
    /// describing the <see cref="Cuttlefish-Cam-Trigger"/> device registers.
    /// </summary>
    [Description("Returns the contents of the metadata file describing the Cuttlefish-Cam-Trigger device registers.")]
    public partial class GetDeviceMetadata : Source<string>
    {
        /// <summary>
        /// Returns an observable sequence with the contents of the metadata file
        /// describing the <see cref="Cuttlefish-Cam-Trigger"/> device registers.
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
    /// Represents an operator that groups the sequence of <see cref="Cuttlefish-Cam-Trigger"/>" messages by register type.
    /// </summary>
    [Description("Groups the sequence of Cuttlefish-Cam-Trigger messages by register type.")]
    public partial class GroupByRegister : Combinator<HarpMessage, IGroupedObservable<Type, HarpMessage>>
    {
        /// <summary>
        /// Groups an observable sequence of <see cref="Cuttlefish-Cam-Trigger"/> messages
        /// by register type.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of observable groups, each of which corresponds to a unique
        /// <see cref="Cuttlefish-Cam-Trigger"/> register.
        /// </returns>
        public override IObservable<IGroupedObservable<Type, HarpMessage>> Process(IObservable<HarpMessage> source)
        {
            return source.GroupBy(message => Device.RegisterMap[message.Address]);
        }
    }

    /// <summary>
    /// Represents an operator that writes the sequence of <see cref="Cuttlefish-Cam-Trigger"/>" messages
    /// to the standard Harp storage format.
    /// </summary>
    [Description("Writes the sequence of Cuttlefish-Cam-Trigger messages to the standard Harp storage format.")]
    public partial class DeviceDataWriter : Sink<HarpMessage>, INamedElement
    {
        const string BinaryExtension = ".bin";
        const string MetadataFileName = "device.yml";
        readonly Bonsai.Harp.MessageWriter writer = new();

        string INamedElement.Name => nameof(Cuttlefish-Cam-Trigger) + "DataWriter";

        /// <summary>
        /// Gets or sets the relative or absolute path on which to save the message data.
        /// </summary>
        [Description("The relative or absolute path of the directory on which to save the message data.")]
        [Editor("Bonsai.Design.SaveFileNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string Path
        {
            get => System.IO.Path.GetDirectoryName(writer.FileName);
            set => writer.FileName = System.IO.Path.Combine(value, nameof(Cuttlefish-Cam-Trigger) + BinaryExtension);
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
    /// reported by the <see cref="Cuttlefish-Cam-Trigger"/> device.
    /// </summary>
    /// <seealso cref="EnableOutput"/>
    /// <seealso cref="InvertOutputMask"/>
    /// <seealso cref="EnableRisingEdgeEvent"/>
    /// <seealso cref="RisingEdgeEvent"/>
    /// <seealso cref="Port0FrequencyHz"/>
    /// <seealso cref="Port1FrequencyHz"/>
    /// <seealso cref="Port2FrequencyHz"/>
    /// <seealso cref="Port3FrequencyHz"/>
    /// <seealso cref="Port4FrequencyHz"/>
    /// <seealso cref="Port5FrequencyHz"/>
    /// <seealso cref="Port6FrequencyHz"/>
    /// <seealso cref="Port7FrequencyHz"/>
    [XmlInclude(typeof(EnableOutput))]
    [XmlInclude(typeof(InvertOutputMask))]
    [XmlInclude(typeof(EnableRisingEdgeEvent))]
    [XmlInclude(typeof(RisingEdgeEvent))]
    [XmlInclude(typeof(Port0FrequencyHz))]
    [XmlInclude(typeof(Port1FrequencyHz))]
    [XmlInclude(typeof(Port2FrequencyHz))]
    [XmlInclude(typeof(Port3FrequencyHz))]
    [XmlInclude(typeof(Port4FrequencyHz))]
    [XmlInclude(typeof(Port5FrequencyHz))]
    [XmlInclude(typeof(Port6FrequencyHz))]
    [XmlInclude(typeof(Port7FrequencyHz))]
    [Description("Filters register-specific messages reported by the Cuttlefish-Cam-Trigger device.")]
    public class FilterRegister : FilterRegisterBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegister"/> class.
        /// </summary>
        public FilterRegister()
        {
            Register = new EnableOutput();
        }

        string INamedElement.Name
        {
            get => $"{nameof(Cuttlefish-Cam-Trigger)}.{GetElementDisplayName(Register)}";
        }
    }

    /// <summary>
    /// Represents an operator which filters and selects specific messages
    /// reported by the Cuttlefish-Cam-Trigger device.
    /// </summary>
    /// <seealso cref="EnableOutput"/>
    /// <seealso cref="InvertOutputMask"/>
    /// <seealso cref="EnableRisingEdgeEvent"/>
    /// <seealso cref="RisingEdgeEvent"/>
    /// <seealso cref="Port0FrequencyHz"/>
    /// <seealso cref="Port1FrequencyHz"/>
    /// <seealso cref="Port2FrequencyHz"/>
    /// <seealso cref="Port3FrequencyHz"/>
    /// <seealso cref="Port4FrequencyHz"/>
    /// <seealso cref="Port5FrequencyHz"/>
    /// <seealso cref="Port6FrequencyHz"/>
    /// <seealso cref="Port7FrequencyHz"/>
    [XmlInclude(typeof(EnableOutput))]
    [XmlInclude(typeof(InvertOutputMask))]
    [XmlInclude(typeof(EnableRisingEdgeEvent))]
    [XmlInclude(typeof(RisingEdgeEvent))]
    [XmlInclude(typeof(Port0FrequencyHz))]
    [XmlInclude(typeof(Port1FrequencyHz))]
    [XmlInclude(typeof(Port2FrequencyHz))]
    [XmlInclude(typeof(Port3FrequencyHz))]
    [XmlInclude(typeof(Port4FrequencyHz))]
    [XmlInclude(typeof(Port5FrequencyHz))]
    [XmlInclude(typeof(Port6FrequencyHz))]
    [XmlInclude(typeof(Port7FrequencyHz))]
    [XmlInclude(typeof(TimestampedEnableOutput))]
    [XmlInclude(typeof(TimestampedInvertOutputMask))]
    [XmlInclude(typeof(TimestampedEnableRisingEdgeEvent))]
    [XmlInclude(typeof(TimestampedRisingEdgeEvent))]
    [XmlInclude(typeof(TimestampedPort0FrequencyHz))]
    [XmlInclude(typeof(TimestampedPort1FrequencyHz))]
    [XmlInclude(typeof(TimestampedPort2FrequencyHz))]
    [XmlInclude(typeof(TimestampedPort3FrequencyHz))]
    [XmlInclude(typeof(TimestampedPort4FrequencyHz))]
    [XmlInclude(typeof(TimestampedPort5FrequencyHz))]
    [XmlInclude(typeof(TimestampedPort6FrequencyHz))]
    [XmlInclude(typeof(TimestampedPort7FrequencyHz))]
    [Description("Filters and selects specific messages reported by the Cuttlefish-Cam-Trigger device.")]
    public partial class Parse : ParseBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parse"/> class.
        /// </summary>
        public Parse()
        {
            Register = new EnableOutput();
        }

        string INamedElement.Name => $"{nameof(Cuttlefish-Cam-Trigger)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents an operator which formats a sequence of values as specific
    /// Cuttlefish-Cam-Trigger register messages.
    /// </summary>
    /// <seealso cref="EnableOutput"/>
    /// <seealso cref="InvertOutputMask"/>
    /// <seealso cref="EnableRisingEdgeEvent"/>
    /// <seealso cref="RisingEdgeEvent"/>
    /// <seealso cref="Port0FrequencyHz"/>
    /// <seealso cref="Port1FrequencyHz"/>
    /// <seealso cref="Port2FrequencyHz"/>
    /// <seealso cref="Port3FrequencyHz"/>
    /// <seealso cref="Port4FrequencyHz"/>
    /// <seealso cref="Port5FrequencyHz"/>
    /// <seealso cref="Port6FrequencyHz"/>
    /// <seealso cref="Port7FrequencyHz"/>
    [XmlInclude(typeof(EnableOutput))]
    [XmlInclude(typeof(InvertOutputMask))]
    [XmlInclude(typeof(EnableRisingEdgeEvent))]
    [XmlInclude(typeof(RisingEdgeEvent))]
    [XmlInclude(typeof(Port0FrequencyHz))]
    [XmlInclude(typeof(Port1FrequencyHz))]
    [XmlInclude(typeof(Port2FrequencyHz))]
    [XmlInclude(typeof(Port3FrequencyHz))]
    [XmlInclude(typeof(Port4FrequencyHz))]
    [XmlInclude(typeof(Port5FrequencyHz))]
    [XmlInclude(typeof(Port6FrequencyHz))]
    [XmlInclude(typeof(Port7FrequencyHz))]
    [Description("Formats a sequence of values as specific Cuttlefish-Cam-Trigger register messages.")]
    public partial class Format : FormatBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format"/> class.
        /// </summary>
        public Format()
        {
            Register = new EnableOutput();
        }

        string INamedElement.Name => $"{nameof(Cuttlefish-Cam-Trigger)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents a register that bitMask to enable/disable each of generated outputs.
    /// </summary>
    [Description("BitMask to enable/disable each of generated outputs.")]
    public partial class EnableOutput
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableOutput"/> register. This field is constant.
        /// </summary>
        public const int Address = 32;

        /// <summary>
        /// Represents the payload type of the <see cref="EnableOutput"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="EnableOutput"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EnableOutput"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Ports GetPayload(HarpMessage message)
        {
            return (Ports)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EnableOutput"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Ports> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Ports)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EnableOutput"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableOutput"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Ports value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EnableOutput"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableOutput"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Ports value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EnableOutput register.
    /// </summary>
    /// <seealso cref="EnableOutput"/>
    [Description("Filters and selects timestamped messages from the EnableOutput register.")]
    public partial class TimestampedEnableOutput
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableOutput"/> register. This field is constant.
        /// </summary>
        public const int Address = EnableOutput.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EnableOutput"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Ports> GetPayload(HarpMessage message)
        {
            return EnableOutput.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that bitMask to invert each of the output lines. If a bit is set, the corresponding output will be inverted.
    /// </summary>
    [Description("BitMask to invert each of the output lines. If a bit is set, the corresponding output will be inverted.")]
    public partial class InvertOutputMask
    {
        /// <summary>
        /// Represents the address of the <see cref="InvertOutputMask"/> register. This field is constant.
        /// </summary>
        public const int Address = 33;

        /// <summary>
        /// Represents the payload type of the <see cref="InvertOutputMask"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="InvertOutputMask"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="InvertOutputMask"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Ports GetPayload(HarpMessage message)
        {
            return (Ports)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="InvertOutputMask"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Ports> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Ports)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="InvertOutputMask"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="InvertOutputMask"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Ports value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="InvertOutputMask"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="InvertOutputMask"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Ports value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// InvertOutputMask register.
    /// </summary>
    /// <seealso cref="InvertOutputMask"/>
    [Description("Filters and selects timestamped messages from the InvertOutputMask register.")]
    public partial class TimestampedInvertOutputMask
    {
        /// <summary>
        /// Represents the address of the <see cref="InvertOutputMask"/> register. This field is constant.
        /// </summary>
        public const int Address = InvertOutputMask.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="InvertOutputMask"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Ports> GetPayload(HarpMessage message)
        {
            return InvertOutputMask.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that if enabled, an event will be dispatched from register RisingEdgeEvent if any of the specified outputs produces a rising edge.
    /// </summary>
    [Description("If enabled, an event will be dispatched from register RisingEdgeEvent if any of the specified outputs produces a rising edge.")]
    public partial class EnableRisingEdgeEvent
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableRisingEdgeEvent"/> register. This field is constant.
        /// </summary>
        public const int Address = 34;

        /// <summary>
        /// Represents the payload type of the <see cref="EnableRisingEdgeEvent"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="EnableRisingEdgeEvent"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EnableRisingEdgeEvent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EnableRisingEdgeEvent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EnableRisingEdgeEvent"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableRisingEdgeEvent"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EnableRisingEdgeEvent"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableRisingEdgeEvent"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EnableRisingEdgeEvent register.
    /// </summary>
    /// <seealso cref="EnableRisingEdgeEvent"/>
    [Description("Filters and selects timestamped messages from the EnableRisingEdgeEvent register.")]
    public partial class TimestampedEnableRisingEdgeEvent
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableRisingEdgeEvent"/> register. This field is constant.
        /// </summary>
        public const int Address = EnableRisingEdgeEvent.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EnableRisingEdgeEvent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return EnableRisingEdgeEvent.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that an event that is dispatched when any of the enabled outputs produces a rising edge. The event will be dispatched only if EnableRisingEdgeEvent is set.
    /// </summary>
    [Description("An event that is dispatched when any of the enabled outputs produces a rising edge. The event will be dispatched only if EnableRisingEdgeEvent is set.")]
    public partial class RisingEdgeEvent
    {
        /// <summary>
        /// Represents the address of the <see cref="RisingEdgeEvent"/> register. This field is constant.
        /// </summary>
        public const int Address = 35;

        /// <summary>
        /// Represents the payload type of the <see cref="RisingEdgeEvent"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="RisingEdgeEvent"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="RisingEdgeEvent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="RisingEdgeEvent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="RisingEdgeEvent"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RisingEdgeEvent"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="RisingEdgeEvent"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RisingEdgeEvent"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// RisingEdgeEvent register.
    /// </summary>
    /// <seealso cref="RisingEdgeEvent"/>
    [Description("Filters and selects timestamped messages from the RisingEdgeEvent register.")]
    public partial class TimestampedRisingEdgeEvent
    {
        /// <summary>
        /// Represents the address of the <see cref="RisingEdgeEvent"/> register. This field is constant.
        /// </summary>
        public const int Address = RisingEdgeEvent.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="RisingEdgeEvent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return RisingEdgeEvent.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that frequency of the pulse train generated on Port0. The value is in Hz.
    /// </summary>
    [Description("Frequency of the pulse train generated on Port0. The value is in Hz.")]
    public partial class Port0FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port0FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = 36;

        /// <summary>
        /// Represents the payload type of the <see cref="Port0FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Port0FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Port0FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Port0FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Port0FrequencyHz"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port0FrequencyHz"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Port0FrequencyHz"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port0FrequencyHz"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Port0FrequencyHz register.
    /// </summary>
    /// <seealso cref="Port0FrequencyHz"/>
    [Description("Filters and selects timestamped messages from the Port0FrequencyHz register.")]
    public partial class TimestampedPort0FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port0FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = Port0FrequencyHz.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Port0FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Port0FrequencyHz.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that frequency of the pulse train generated on Port1. The value is in Hz.
    /// </summary>
    [Description("Frequency of the pulse train generated on Port1. The value is in Hz.")]
    public partial class Port1FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port1FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = 38;

        /// <summary>
        /// Represents the payload type of the <see cref="Port1FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Port1FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Port1FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Port1FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Port1FrequencyHz"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port1FrequencyHz"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Port1FrequencyHz"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port1FrequencyHz"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Port1FrequencyHz register.
    /// </summary>
    /// <seealso cref="Port1FrequencyHz"/>
    [Description("Filters and selects timestamped messages from the Port1FrequencyHz register.")]
    public partial class TimestampedPort1FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port1FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = Port1FrequencyHz.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Port1FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Port1FrequencyHz.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that frequency of the pulse train generated on Port2. The value is in Hz.
    /// </summary>
    [Description("Frequency of the pulse train generated on Port2. The value is in Hz.")]
    public partial class Port2FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port2FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = 40;

        /// <summary>
        /// Represents the payload type of the <see cref="Port2FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Port2FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Port2FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Port2FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Port2FrequencyHz"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port2FrequencyHz"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Port2FrequencyHz"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port2FrequencyHz"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Port2FrequencyHz register.
    /// </summary>
    /// <seealso cref="Port2FrequencyHz"/>
    [Description("Filters and selects timestamped messages from the Port2FrequencyHz register.")]
    public partial class TimestampedPort2FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port2FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = Port2FrequencyHz.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Port2FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Port2FrequencyHz.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that frequency of the pulse train generated on Port3. The value is in Hz.
    /// </summary>
    [Description("Frequency of the pulse train generated on Port3. The value is in Hz.")]
    public partial class Port3FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port3FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = 42;

        /// <summary>
        /// Represents the payload type of the <see cref="Port3FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Port3FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Port3FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Port3FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Port3FrequencyHz"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port3FrequencyHz"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Port3FrequencyHz"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port3FrequencyHz"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Port3FrequencyHz register.
    /// </summary>
    /// <seealso cref="Port3FrequencyHz"/>
    [Description("Filters and selects timestamped messages from the Port3FrequencyHz register.")]
    public partial class TimestampedPort3FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port3FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = Port3FrequencyHz.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Port3FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Port3FrequencyHz.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that frequency of the pulse train generated on Port4. The value is in Hz.
    /// </summary>
    [Description("Frequency of the pulse train generated on Port4. The value is in Hz.")]
    public partial class Port4FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port4FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = 44;

        /// <summary>
        /// Represents the payload type of the <see cref="Port4FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Port4FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Port4FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Port4FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Port4FrequencyHz"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port4FrequencyHz"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Port4FrequencyHz"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port4FrequencyHz"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Port4FrequencyHz register.
    /// </summary>
    /// <seealso cref="Port4FrequencyHz"/>
    [Description("Filters and selects timestamped messages from the Port4FrequencyHz register.")]
    public partial class TimestampedPort4FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port4FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = Port4FrequencyHz.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Port4FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Port4FrequencyHz.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that frequency of the pulse train generated on Port5. The value is in Hz.
    /// </summary>
    [Description("Frequency of the pulse train generated on Port5. The value is in Hz.")]
    public partial class Port5FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port5FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = 46;

        /// <summary>
        /// Represents the payload type of the <see cref="Port5FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Port5FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Port5FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Port5FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Port5FrequencyHz"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port5FrequencyHz"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Port5FrequencyHz"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port5FrequencyHz"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Port5FrequencyHz register.
    /// </summary>
    /// <seealso cref="Port5FrequencyHz"/>
    [Description("Filters and selects timestamped messages from the Port5FrequencyHz register.")]
    public partial class TimestampedPort5FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port5FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = Port5FrequencyHz.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Port5FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Port5FrequencyHz.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that frequency of the pulse train generated on Port6. The value is in Hz.
    /// </summary>
    [Description("Frequency of the pulse train generated on Port6. The value is in Hz.")]
    public partial class Port6FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port6FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = 48;

        /// <summary>
        /// Represents the payload type of the <see cref="Port6FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Port6FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Port6FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Port6FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Port6FrequencyHz"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port6FrequencyHz"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Port6FrequencyHz"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port6FrequencyHz"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Port6FrequencyHz register.
    /// </summary>
    /// <seealso cref="Port6FrequencyHz"/>
    [Description("Filters and selects timestamped messages from the Port6FrequencyHz register.")]
    public partial class TimestampedPort6FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port6FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = Port6FrequencyHz.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Port6FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Port6FrequencyHz.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that frequency of the pulse train generated on Port7. The value is in Hz.
    /// </summary>
    [Description("Frequency of the pulse train generated on Port7. The value is in Hz.")]
    public partial class Port7FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port7FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = 50;

        /// <summary>
        /// Represents the payload type of the <see cref="Port7FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Port7FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Port7FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Port7FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Port7FrequencyHz"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port7FrequencyHz"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Port7FrequencyHz"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Port7FrequencyHz"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Port7FrequencyHz register.
    /// </summary>
    /// <seealso cref="Port7FrequencyHz"/>
    [Description("Filters and selects timestamped messages from the Port7FrequencyHz register.")]
    public partial class TimestampedPort7FrequencyHz
    {
        /// <summary>
        /// Represents the address of the <see cref="Port7FrequencyHz"/> register. This field is constant.
        /// </summary>
        public const int Address = Port7FrequencyHz.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Port7FrequencyHz"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Port7FrequencyHz.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents an operator which creates standard message payloads for the
    /// Cuttlefish-Cam-Trigger device.
    /// </summary>
    /// <seealso cref="CreateEnableOutputPayload"/>
    /// <seealso cref="CreateInvertOutputMaskPayload"/>
    /// <seealso cref="CreateEnableRisingEdgeEventPayload"/>
    /// <seealso cref="CreateRisingEdgeEventPayload"/>
    /// <seealso cref="CreatePort0FrequencyHzPayload"/>
    /// <seealso cref="CreatePort1FrequencyHzPayload"/>
    /// <seealso cref="CreatePort2FrequencyHzPayload"/>
    /// <seealso cref="CreatePort3FrequencyHzPayload"/>
    /// <seealso cref="CreatePort4FrequencyHzPayload"/>
    /// <seealso cref="CreatePort5FrequencyHzPayload"/>
    /// <seealso cref="CreatePort6FrequencyHzPayload"/>
    /// <seealso cref="CreatePort7FrequencyHzPayload"/>
    [XmlInclude(typeof(CreateEnableOutputPayload))]
    [XmlInclude(typeof(CreateInvertOutputMaskPayload))]
    [XmlInclude(typeof(CreateEnableRisingEdgeEventPayload))]
    [XmlInclude(typeof(CreateRisingEdgeEventPayload))]
    [XmlInclude(typeof(CreatePort0FrequencyHzPayload))]
    [XmlInclude(typeof(CreatePort1FrequencyHzPayload))]
    [XmlInclude(typeof(CreatePort2FrequencyHzPayload))]
    [XmlInclude(typeof(CreatePort3FrequencyHzPayload))]
    [XmlInclude(typeof(CreatePort4FrequencyHzPayload))]
    [XmlInclude(typeof(CreatePort5FrequencyHzPayload))]
    [XmlInclude(typeof(CreatePort6FrequencyHzPayload))]
    [XmlInclude(typeof(CreatePort7FrequencyHzPayload))]
    [XmlInclude(typeof(CreateTimestampedEnableOutputPayload))]
    [XmlInclude(typeof(CreateTimestampedInvertOutputMaskPayload))]
    [XmlInclude(typeof(CreateTimestampedEnableRisingEdgeEventPayload))]
    [XmlInclude(typeof(CreateTimestampedRisingEdgeEventPayload))]
    [XmlInclude(typeof(CreateTimestampedPort0FrequencyHzPayload))]
    [XmlInclude(typeof(CreateTimestampedPort1FrequencyHzPayload))]
    [XmlInclude(typeof(CreateTimestampedPort2FrequencyHzPayload))]
    [XmlInclude(typeof(CreateTimestampedPort3FrequencyHzPayload))]
    [XmlInclude(typeof(CreateTimestampedPort4FrequencyHzPayload))]
    [XmlInclude(typeof(CreateTimestampedPort5FrequencyHzPayload))]
    [XmlInclude(typeof(CreateTimestampedPort6FrequencyHzPayload))]
    [XmlInclude(typeof(CreateTimestampedPort7FrequencyHzPayload))]
    [Description("Creates standard message payloads for the Cuttlefish-Cam-Trigger device.")]
    public partial class CreateMessage : CreateMessageBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMessage"/> class.
        /// </summary>
        public CreateMessage()
        {
            Payload = new CreateEnableOutputPayload();
        }

        string INamedElement.Name => $"{nameof(Cuttlefish-Cam-Trigger)}.{GetElementDisplayName(Payload)}";
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that bitMask to enable/disable each of generated outputs.
    /// </summary>
    [DisplayName("EnableOutputPayload")]
    [Description("Creates a message payload that bitMask to enable/disable each of generated outputs.")]
    public partial class CreateEnableOutputPayload
    {
        /// <summary>
        /// Gets or sets the value that bitMask to enable/disable each of generated outputs.
        /// </summary>
        [Description("The value that bitMask to enable/disable each of generated outputs.")]
        public Ports EnableOutput { get; set; }

        /// <summary>
        /// Creates a message payload for the EnableOutput register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Ports GetPayload()
        {
            return EnableOutput;
        }

        /// <summary>
        /// Creates a message that bitMask to enable/disable each of generated outputs.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EnableOutput register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.EnableOutput.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that bitMask to enable/disable each of generated outputs.
    /// </summary>
    [DisplayName("TimestampedEnableOutputPayload")]
    [Description("Creates a timestamped message payload that bitMask to enable/disable each of generated outputs.")]
    public partial class CreateTimestampedEnableOutputPayload : CreateEnableOutputPayload
    {
        /// <summary>
        /// Creates a timestamped message that bitMask to enable/disable each of generated outputs.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EnableOutput register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.EnableOutput.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that bitMask to invert each of the output lines. If a bit is set, the corresponding output will be inverted.
    /// </summary>
    [DisplayName("InvertOutputMaskPayload")]
    [Description("Creates a message payload that bitMask to invert each of the output lines. If a bit is set, the corresponding output will be inverted.")]
    public partial class CreateInvertOutputMaskPayload
    {
        /// <summary>
        /// Gets or sets the value that bitMask to invert each of the output lines. If a bit is set, the corresponding output will be inverted.
        /// </summary>
        [Description("The value that bitMask to invert each of the output lines. If a bit is set, the corresponding output will be inverted.")]
        public Ports InvertOutputMask { get; set; }

        /// <summary>
        /// Creates a message payload for the InvertOutputMask register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Ports GetPayload()
        {
            return InvertOutputMask;
        }

        /// <summary>
        /// Creates a message that bitMask to invert each of the output lines. If a bit is set, the corresponding output will be inverted.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the InvertOutputMask register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.InvertOutputMask.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that bitMask to invert each of the output lines. If a bit is set, the corresponding output will be inverted.
    /// </summary>
    [DisplayName("TimestampedInvertOutputMaskPayload")]
    [Description("Creates a timestamped message payload that bitMask to invert each of the output lines. If a bit is set, the corresponding output will be inverted.")]
    public partial class CreateTimestampedInvertOutputMaskPayload : CreateInvertOutputMaskPayload
    {
        /// <summary>
        /// Creates a timestamped message that bitMask to invert each of the output lines. If a bit is set, the corresponding output will be inverted.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the InvertOutputMask register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.InvertOutputMask.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that if enabled, an event will be dispatched from register RisingEdgeEvent if any of the specified outputs produces a rising edge.
    /// </summary>
    [DisplayName("EnableRisingEdgeEventPayload")]
    [Description("Creates a message payload that if enabled, an event will be dispatched from register RisingEdgeEvent if any of the specified outputs produces a rising edge.")]
    public partial class CreateEnableRisingEdgeEventPayload
    {
        /// <summary>
        /// Gets or sets the value that if enabled, an event will be dispatched from register RisingEdgeEvent if any of the specified outputs produces a rising edge.
        /// </summary>
        [Description("The value that if enabled, an event will be dispatched from register RisingEdgeEvent if any of the specified outputs produces a rising edge.")]
        public byte EnableRisingEdgeEvent { get; set; }

        /// <summary>
        /// Creates a message payload for the EnableRisingEdgeEvent register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return EnableRisingEdgeEvent;
        }

        /// <summary>
        /// Creates a message that if enabled, an event will be dispatched from register RisingEdgeEvent if any of the specified outputs produces a rising edge.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EnableRisingEdgeEvent register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.EnableRisingEdgeEvent.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that if enabled, an event will be dispatched from register RisingEdgeEvent if any of the specified outputs produces a rising edge.
    /// </summary>
    [DisplayName("TimestampedEnableRisingEdgeEventPayload")]
    [Description("Creates a timestamped message payload that if enabled, an event will be dispatched from register RisingEdgeEvent if any of the specified outputs produces a rising edge.")]
    public partial class CreateTimestampedEnableRisingEdgeEventPayload : CreateEnableRisingEdgeEventPayload
    {
        /// <summary>
        /// Creates a timestamped message that if enabled, an event will be dispatched from register RisingEdgeEvent if any of the specified outputs produces a rising edge.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EnableRisingEdgeEvent register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.EnableRisingEdgeEvent.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that an event that is dispatched when any of the enabled outputs produces a rising edge. The event will be dispatched only if EnableRisingEdgeEvent is set.
    /// </summary>
    [DisplayName("RisingEdgeEventPayload")]
    [Description("Creates a message payload that an event that is dispatched when any of the enabled outputs produces a rising edge. The event will be dispatched only if EnableRisingEdgeEvent is set.")]
    public partial class CreateRisingEdgeEventPayload
    {
        /// <summary>
        /// Gets or sets the value that an event that is dispatched when any of the enabled outputs produces a rising edge. The event will be dispatched only if EnableRisingEdgeEvent is set.
        /// </summary>
        [Description("The value that an event that is dispatched when any of the enabled outputs produces a rising edge. The event will be dispatched only if EnableRisingEdgeEvent is set.")]
        public byte RisingEdgeEvent { get; set; }

        /// <summary>
        /// Creates a message payload for the RisingEdgeEvent register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return RisingEdgeEvent;
        }

        /// <summary>
        /// Creates a message that an event that is dispatched when any of the enabled outputs produces a rising edge. The event will be dispatched only if EnableRisingEdgeEvent is set.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the RisingEdgeEvent register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.RisingEdgeEvent.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that an event that is dispatched when any of the enabled outputs produces a rising edge. The event will be dispatched only if EnableRisingEdgeEvent is set.
    /// </summary>
    [DisplayName("TimestampedRisingEdgeEventPayload")]
    [Description("Creates a timestamped message payload that an event that is dispatched when any of the enabled outputs produces a rising edge. The event will be dispatched only if EnableRisingEdgeEvent is set.")]
    public partial class CreateTimestampedRisingEdgeEventPayload : CreateRisingEdgeEventPayload
    {
        /// <summary>
        /// Creates a timestamped message that an event that is dispatched when any of the enabled outputs produces a rising edge. The event will be dispatched only if EnableRisingEdgeEvent is set.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the RisingEdgeEvent register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.RisingEdgeEvent.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that frequency of the pulse train generated on Port0. The value is in Hz.
    /// </summary>
    [DisplayName("Port0FrequencyHzPayload")]
    [Description("Creates a message payload that frequency of the pulse train generated on Port0. The value is in Hz.")]
    public partial class CreatePort0FrequencyHzPayload
    {
        /// <summary>
        /// Gets or sets the value that frequency of the pulse train generated on Port0. The value is in Hz.
        /// </summary>
        [Description("The value that frequency of the pulse train generated on Port0. The value is in Hz.")]
        public float Port0FrequencyHz { get; set; }

        /// <summary>
        /// Creates a message payload for the Port0FrequencyHz register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Port0FrequencyHz;
        }

        /// <summary>
        /// Creates a message that frequency of the pulse train generated on Port0. The value is in Hz.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Port0FrequencyHz register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port0FrequencyHz.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that frequency of the pulse train generated on Port0. The value is in Hz.
    /// </summary>
    [DisplayName("TimestampedPort0FrequencyHzPayload")]
    [Description("Creates a timestamped message payload that frequency of the pulse train generated on Port0. The value is in Hz.")]
    public partial class CreateTimestampedPort0FrequencyHzPayload : CreatePort0FrequencyHzPayload
    {
        /// <summary>
        /// Creates a timestamped message that frequency of the pulse train generated on Port0. The value is in Hz.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Port0FrequencyHz register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port0FrequencyHz.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that frequency of the pulse train generated on Port1. The value is in Hz.
    /// </summary>
    [DisplayName("Port1FrequencyHzPayload")]
    [Description("Creates a message payload that frequency of the pulse train generated on Port1. The value is in Hz.")]
    public partial class CreatePort1FrequencyHzPayload
    {
        /// <summary>
        /// Gets or sets the value that frequency of the pulse train generated on Port1. The value is in Hz.
        /// </summary>
        [Description("The value that frequency of the pulse train generated on Port1. The value is in Hz.")]
        public float Port1FrequencyHz { get; set; }

        /// <summary>
        /// Creates a message payload for the Port1FrequencyHz register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Port1FrequencyHz;
        }

        /// <summary>
        /// Creates a message that frequency of the pulse train generated on Port1. The value is in Hz.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Port1FrequencyHz register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port1FrequencyHz.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that frequency of the pulse train generated on Port1. The value is in Hz.
    /// </summary>
    [DisplayName("TimestampedPort1FrequencyHzPayload")]
    [Description("Creates a timestamped message payload that frequency of the pulse train generated on Port1. The value is in Hz.")]
    public partial class CreateTimestampedPort1FrequencyHzPayload : CreatePort1FrequencyHzPayload
    {
        /// <summary>
        /// Creates a timestamped message that frequency of the pulse train generated on Port1. The value is in Hz.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Port1FrequencyHz register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port1FrequencyHz.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that frequency of the pulse train generated on Port2. The value is in Hz.
    /// </summary>
    [DisplayName("Port2FrequencyHzPayload")]
    [Description("Creates a message payload that frequency of the pulse train generated on Port2. The value is in Hz.")]
    public partial class CreatePort2FrequencyHzPayload
    {
        /// <summary>
        /// Gets or sets the value that frequency of the pulse train generated on Port2. The value is in Hz.
        /// </summary>
        [Description("The value that frequency of the pulse train generated on Port2. The value is in Hz.")]
        public float Port2FrequencyHz { get; set; }

        /// <summary>
        /// Creates a message payload for the Port2FrequencyHz register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Port2FrequencyHz;
        }

        /// <summary>
        /// Creates a message that frequency of the pulse train generated on Port2. The value is in Hz.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Port2FrequencyHz register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port2FrequencyHz.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that frequency of the pulse train generated on Port2. The value is in Hz.
    /// </summary>
    [DisplayName("TimestampedPort2FrequencyHzPayload")]
    [Description("Creates a timestamped message payload that frequency of the pulse train generated on Port2. The value is in Hz.")]
    public partial class CreateTimestampedPort2FrequencyHzPayload : CreatePort2FrequencyHzPayload
    {
        /// <summary>
        /// Creates a timestamped message that frequency of the pulse train generated on Port2. The value is in Hz.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Port2FrequencyHz register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port2FrequencyHz.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that frequency of the pulse train generated on Port3. The value is in Hz.
    /// </summary>
    [DisplayName("Port3FrequencyHzPayload")]
    [Description("Creates a message payload that frequency of the pulse train generated on Port3. The value is in Hz.")]
    public partial class CreatePort3FrequencyHzPayload
    {
        /// <summary>
        /// Gets or sets the value that frequency of the pulse train generated on Port3. The value is in Hz.
        /// </summary>
        [Description("The value that frequency of the pulse train generated on Port3. The value is in Hz.")]
        public float Port3FrequencyHz { get; set; }

        /// <summary>
        /// Creates a message payload for the Port3FrequencyHz register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Port3FrequencyHz;
        }

        /// <summary>
        /// Creates a message that frequency of the pulse train generated on Port3. The value is in Hz.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Port3FrequencyHz register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port3FrequencyHz.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that frequency of the pulse train generated on Port3. The value is in Hz.
    /// </summary>
    [DisplayName("TimestampedPort3FrequencyHzPayload")]
    [Description("Creates a timestamped message payload that frequency of the pulse train generated on Port3. The value is in Hz.")]
    public partial class CreateTimestampedPort3FrequencyHzPayload : CreatePort3FrequencyHzPayload
    {
        /// <summary>
        /// Creates a timestamped message that frequency of the pulse train generated on Port3. The value is in Hz.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Port3FrequencyHz register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port3FrequencyHz.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that frequency of the pulse train generated on Port4. The value is in Hz.
    /// </summary>
    [DisplayName("Port4FrequencyHzPayload")]
    [Description("Creates a message payload that frequency of the pulse train generated on Port4. The value is in Hz.")]
    public partial class CreatePort4FrequencyHzPayload
    {
        /// <summary>
        /// Gets or sets the value that frequency of the pulse train generated on Port4. The value is in Hz.
        /// </summary>
        [Description("The value that frequency of the pulse train generated on Port4. The value is in Hz.")]
        public float Port4FrequencyHz { get; set; }

        /// <summary>
        /// Creates a message payload for the Port4FrequencyHz register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Port4FrequencyHz;
        }

        /// <summary>
        /// Creates a message that frequency of the pulse train generated on Port4. The value is in Hz.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Port4FrequencyHz register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port4FrequencyHz.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that frequency of the pulse train generated on Port4. The value is in Hz.
    /// </summary>
    [DisplayName("TimestampedPort4FrequencyHzPayload")]
    [Description("Creates a timestamped message payload that frequency of the pulse train generated on Port4. The value is in Hz.")]
    public partial class CreateTimestampedPort4FrequencyHzPayload : CreatePort4FrequencyHzPayload
    {
        /// <summary>
        /// Creates a timestamped message that frequency of the pulse train generated on Port4. The value is in Hz.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Port4FrequencyHz register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port4FrequencyHz.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that frequency of the pulse train generated on Port5. The value is in Hz.
    /// </summary>
    [DisplayName("Port5FrequencyHzPayload")]
    [Description("Creates a message payload that frequency of the pulse train generated on Port5. The value is in Hz.")]
    public partial class CreatePort5FrequencyHzPayload
    {
        /// <summary>
        /// Gets or sets the value that frequency of the pulse train generated on Port5. The value is in Hz.
        /// </summary>
        [Description("The value that frequency of the pulse train generated on Port5. The value is in Hz.")]
        public float Port5FrequencyHz { get; set; }

        /// <summary>
        /// Creates a message payload for the Port5FrequencyHz register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Port5FrequencyHz;
        }

        /// <summary>
        /// Creates a message that frequency of the pulse train generated on Port5. The value is in Hz.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Port5FrequencyHz register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port5FrequencyHz.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that frequency of the pulse train generated on Port5. The value is in Hz.
    /// </summary>
    [DisplayName("TimestampedPort5FrequencyHzPayload")]
    [Description("Creates a timestamped message payload that frequency of the pulse train generated on Port5. The value is in Hz.")]
    public partial class CreateTimestampedPort5FrequencyHzPayload : CreatePort5FrequencyHzPayload
    {
        /// <summary>
        /// Creates a timestamped message that frequency of the pulse train generated on Port5. The value is in Hz.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Port5FrequencyHz register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port5FrequencyHz.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that frequency of the pulse train generated on Port6. The value is in Hz.
    /// </summary>
    [DisplayName("Port6FrequencyHzPayload")]
    [Description("Creates a message payload that frequency of the pulse train generated on Port6. The value is in Hz.")]
    public partial class CreatePort6FrequencyHzPayload
    {
        /// <summary>
        /// Gets or sets the value that frequency of the pulse train generated on Port6. The value is in Hz.
        /// </summary>
        [Description("The value that frequency of the pulse train generated on Port6. The value is in Hz.")]
        public float Port6FrequencyHz { get; set; }

        /// <summary>
        /// Creates a message payload for the Port6FrequencyHz register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Port6FrequencyHz;
        }

        /// <summary>
        /// Creates a message that frequency of the pulse train generated on Port6. The value is in Hz.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Port6FrequencyHz register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port6FrequencyHz.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that frequency of the pulse train generated on Port6. The value is in Hz.
    /// </summary>
    [DisplayName("TimestampedPort6FrequencyHzPayload")]
    [Description("Creates a timestamped message payload that frequency of the pulse train generated on Port6. The value is in Hz.")]
    public partial class CreateTimestampedPort6FrequencyHzPayload : CreatePort6FrequencyHzPayload
    {
        /// <summary>
        /// Creates a timestamped message that frequency of the pulse train generated on Port6. The value is in Hz.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Port6FrequencyHz register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port6FrequencyHz.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that frequency of the pulse train generated on Port7. The value is in Hz.
    /// </summary>
    [DisplayName("Port7FrequencyHzPayload")]
    [Description("Creates a message payload that frequency of the pulse train generated on Port7. The value is in Hz.")]
    public partial class CreatePort7FrequencyHzPayload
    {
        /// <summary>
        /// Gets or sets the value that frequency of the pulse train generated on Port7. The value is in Hz.
        /// </summary>
        [Description("The value that frequency of the pulse train generated on Port7. The value is in Hz.")]
        public float Port7FrequencyHz { get; set; }

        /// <summary>
        /// Creates a message payload for the Port7FrequencyHz register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Port7FrequencyHz;
        }

        /// <summary>
        /// Creates a message that frequency of the pulse train generated on Port7. The value is in Hz.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Port7FrequencyHz register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port7FrequencyHz.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that frequency of the pulse train generated on Port7. The value is in Hz.
    /// </summary>
    [DisplayName("TimestampedPort7FrequencyHzPayload")]
    [Description("Creates a timestamped message payload that frequency of the pulse train generated on Port7. The value is in Hz.")]
    public partial class CreateTimestampedPort7FrequencyHzPayload : CreatePort7FrequencyHzPayload
    {
        /// <summary>
        /// Creates a timestamped message that frequency of the pulse train generated on Port7. The value is in Hz.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Port7FrequencyHz register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.CuttlefishCamTrigger.Port7FrequencyHz.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Available ports on the device
    /// </summary>
    [Flags]
    public enum Ports : byte
    {
        None = 0x0,
        Port0 = 0x1,
        Port1 = 0x2,
        Port2 = 0x4,
        Port3 = 0x8,
        Port4 = 0x10,
        Port5 = 0x20,
        Port6 = 0x40,
        Port7 = 0x80
    }
}
