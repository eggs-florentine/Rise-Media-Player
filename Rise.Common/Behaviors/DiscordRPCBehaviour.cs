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

  public void invoke(DiscordRPCState d, string s, string clientId) {
    // interpret state and then set action
    client = new DiscordRpcClient(clientId);

    client.Initialize();
    
    client.SetPresence(new RichPresence()
    { 
      Details = interpretState(d, s),
      State = "",
    });
  }

  public void initialise(string clientId) {
    // initialise client and invoke as idle, check initialisation
    client = new DiscordRpcClient(clientId);          

    // subscribe to events
    client.OnReady += (sender, e) =>
    {
        Console.WriteLine("Received ready from user {0}", e.User.Username);
    };

    // connect to the RPC
    client.Initialize();

    // set the rich presence
    client.SetPresence(new RichPresence()
    {
        Details = "Idle",
        State = ""
    }); 
  } 
}

public enum DiscordRPCState {
  LISTENING,
  WATCHING,
  OBSERVING,
  IDLE;
}
