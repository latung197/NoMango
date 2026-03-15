import React from 'react';
import { Badge } from '../ui/badge';

export const getEfficiencyBadge = (efficiency: number) => {
  if (efficiency >= 95) return <Badge className="bg-green-100 text-green-800">Excellent</Badge>;
  if (efficiency >= 85) return <Badge className="bg-blue-100 text-blue-800">Good</Badge>;
  if (efficiency >= 75) return <Badge className="bg-yellow-100 text-yellow-800">Average</Badge>;
  return <Badge className="bg-red-100 text-red-800">Poor</Badge>;
};

export const getQCStatusBadge = (status: string) => {
  switch (status) {
    case 'Passed':
      return <Badge className="bg-green-100 text-green-800">Passed / ဖြတ်သန်းပြီး</Badge>;
    case 'Failed':
      return <Badge className="bg-red-100 text-red-800">Failed / မအောင်မြင်</Badge>;
    case 'Pending':
      return <Badge className="bg-yellow-100 text-yellow-800">Pending / စောင့်ဆိုင်း</Badge>;
    case 'Completed':
      return <Badge className="bg-green-100 text-green-800">Completed / ပြီးစီး</Badge>;
    default:
      return <Badge variant="secondary">Unknown</Badge>;
  }
};

export const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
  }).format(value);
};

export const formatNumber = (value: number) => {
  return value.toLocaleString();
};

export const calculatePassRate = (passed: number, total: number) => {
  return total > 0 ? Math.round((passed / total) * 100) : 0;
};

export const handleExport = (reportType: string, format: string) => {
  // Mock export functionality
  alert(`Exporting ${reportType} report as ${format}`);
};