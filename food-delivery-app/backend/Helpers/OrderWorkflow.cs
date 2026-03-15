namespace FoodDeliveryAPI.Helpers
{
    public static class OrderWorkflow
    {
        private static readonly string[] OrderedStatuses =
        {
            "Pending", "Accepted", "Preparing", "OutForDelivery", "Delivered"
        };

        public static bool IsValid(string status)
            => OrderedStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);

        public static bool CanTransition(string current, string target)
        {
            var currentIndex = Array.FindIndex(OrderedStatuses, s => s.Equals(current, StringComparison.OrdinalIgnoreCase));
            var targetIndex = Array.FindIndex(OrderedStatuses, s => s.Equals(target, StringComparison.OrdinalIgnoreCase));
            return currentIndex >= 0 && targetIndex >= 0 && targetIndex >= currentIndex;
        }

        public static IReadOnlyCollection<string> All => OrderedStatuses;
    }
}
