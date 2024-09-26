namespace OrderDeliverySystem.Client.Infrastructure.Services
{
    public class LayoutUpdateService
    {
        public event Action OnLayoutUpdated;

        public void NotifyLayoutUpdate()
        {
            OnLayoutUpdated?.Invoke();
        }
    }

}
