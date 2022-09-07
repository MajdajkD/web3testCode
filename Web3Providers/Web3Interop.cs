using Microsoft.JSInterop;

namespace Web3Providers
{
  public class Web3Interop
  {
    private readonly IJSRuntime _jsRuntime;

    public static Web3Interop Instance { get; private set; }
    public bool Available { get; set; } = false;
    public string? SelectedAccount { get; set; } = null;
    public string? ChainId { get; set; } = null;

    public event Func<bool, Task> AvailabilityChangedEvent;

    public event Func<string, Task> SelectedAccountChangedEvent;

    public event Func<string, Task> NetworkChangedEvent;

    public Web3Interop(IJSRuntime jsRuntime)
    {
      _jsRuntime = jsRuntime;
      Instance = this;
    }

    public async Task EnableWeb3Provider()
    {
      var modalOpened = await _jsRuntime.InvokeAsync<bool>("NethereumWeb3Interop.SetupWeb3");
      Available = await _jsRuntime.InvokeAsync<bool>("NethereumWeb3Interop.SetupWeb3Provider");

      if (AvailabilityChangedEvent != null)
        await AvailabilityChangedEvent.Invoke(Available);

      if (Available)
      {
        SelectedAccount = await _jsRuntime.InvokeAsync<string>("NethereumWeb3Interop.GetAccount");
        ChainId = await _jsRuntime.InvokeAsync<string>("NethereumWeb3Interop.GetNetwork");

        if (SelectedAccountChangedEvent != null)
          await SelectedAccountChangedEvent.Invoke(SelectedAccount);

        if (NetworkChangedEvent != null)
          await NetworkChangedEvent.Invoke(ChainId);
      }
    }

    [JSInvokable()]
    public static async Task Web3AvailableChanged(bool available)
    {
      Web3Interop.Instance.Available = available;
      if (Instance.AvailabilityChangedEvent != null)
        await Instance.AvailabilityChangedEvent.Invoke(available);
    }

    [JSInvokable()]
    public static async Task SelectedAccountChanged(string selectedAccount)
    {
      Web3Interop.Instance.SelectedAccount = selectedAccount;
      if (Instance.SelectedAccountChangedEvent != null)
        await Instance.SelectedAccountChangedEvent.Invoke(selectedAccount);
    }

    [JSInvokable()]
    public static async Task SelectedNetworkChanged(string chainId)
    {
      Web3Interop.Instance.ChainId = chainId;
      if (Instance.NetworkChangedEvent != null)
        await Instance.NetworkChangedEvent.Invoke(chainId);
    }
  }
}