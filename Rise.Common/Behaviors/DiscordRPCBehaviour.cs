namespace Rise.Common.Behaviours;

class DiscordRPCBehaviour {
  private DiscordRPCState currentRawState;
  private string stateCaption = interpretState(currentRawState);
  private bool initialised = false;

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

  public void invoke(DiscordRPCState d, string s) {
    // interpret state and then set action
  }

  public void initialise(string clientId) {
    // initialise client and invoke as idle, check initialisation
  } 
}

public enum DiscordRPCState {
  LISTENING,
  WATCHING,
  OBSERVING,
  IDLE;
}
