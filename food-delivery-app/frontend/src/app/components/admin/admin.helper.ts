export function getStatusColor(status: string): string {
  switch (status) {
    case 'Pending': return '#ff9800';
    case 'Confirmed': return '#2196f3';
    case 'Preparing': return '#9c27b0';
    case 'Ready': return '#4caf50';
    case 'Delivered': return '#2e7d32';
    case 'Cancelled': return '#f44336';
    default: return '#999';
  }
}
