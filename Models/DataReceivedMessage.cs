using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SmartAsthmaAssistane.Models;

public class DataReceivedMessage : ValueChangedMessage<Item>
{
    public DataReceivedMessage(Item value) : base(value)
    {
    }
}
