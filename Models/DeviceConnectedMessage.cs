using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SmartAsthmaAssistane.Models;

public class DeviceConnectedMessage : ValueChangedMessage<string>
{
    public DeviceConnectedMessage(string value) : base(value)
    {
    }
}
