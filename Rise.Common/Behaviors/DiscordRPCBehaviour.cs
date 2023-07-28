namespace Rise.Common.Behaviours;

class DiscordRPCBehaviour {
  private DiscordRPCState currentRawState;
  private string stateCaption;

  private string interpretState(DiscordRPCState d, string s) {
    switch (d) {
      case DiscordRPCState.LISTENING:
        return 'Listening to ' + s;
        break;
      case DiscordRPCState.WATCHING:
        return 'Watching ' + s;
        break;
      case DiscordRPCState.OBSERVING:
        return 'Looking at ' + s;
        break;
      case DiscordRPCState.IDLE:
        return 'Idle';
        break;
    }
  }
}

public enum DiscordRPCState {
  LISTENING,
  WATCHING,
  OBSERVING,
  IDLE;
}
