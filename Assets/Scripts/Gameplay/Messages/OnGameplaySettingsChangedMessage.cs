using fireMCG.PathOfLayouts.Messaging;

namespace fireMCG.PathOfLayouts.Gameplay
{
    public class OnGameplaySettingsChangedMessage : IMessage
    {
        public readonly int MovementSpeedPercent;
        public readonly int LightRadiusPercent;

        public OnGameplaySettingsChangedMessage(int movementSpeedPercent, int lightRadiusPercent)
        {
            MovementSpeedPercent = movementSpeedPercent;
            LightRadiusPercent = lightRadiusPercent;
        }
    }
}