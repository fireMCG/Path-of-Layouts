using System;

namespace fireMCG.PathOfLayouts.Messaging
{
	internal static class Validation
	{
		internal static Type IsMessageType(Type type)
		{
			if(type == null)
			{
				throw new ArgumentNullException(nameof(type), "Message type is null.");
			}
			if(!typeof(IMessage).IsAssignableFrom(type))
			{
				throw new ArgumentException($"Message type '{type?.FullName ?? "null"}' is not of type '{typeof(IMessage).FullName}'.");
			}

			return type;
		}
	}
}