namespace fireMCG.PathOfLayouts.Messaging
{
	public delegate void MessageListener<T>(T message) where T : IMessage;

	public interface IMessageBus
	{
		void Subscribe<T>(MessageListener<T> listener) where T : IMessage;

		void Unsubscribe<T>(MessageListener<T> listener) where T : IMessage;

		void Publish(IMessage message);
	}
}