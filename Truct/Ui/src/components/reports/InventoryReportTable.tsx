import React from 'react';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { InventoryReport } from '../data/reportsData';
import { formatCurrency, formatNumber } from '../utils/reportUtils';

interface InventoryReportTableProps {
  reports: InventoryReport[];
}

export function InventoryReportTable({ reports }: InventoryReportTableProps) {
  return (
    <div className="overflow-x-auto">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Date / ရက်စွဲ</TableHead>
            <TableHead>Warehouse / ကုန်လှောင်ရုံ</TableHead>
            <TableHead>Item / ပစ္စည်း</TableHead>
            <TableHead>Category / အမျိုးအစား</TableHead>
            <TableHead>Opening Stock / စတင်ရာစတော့ခ်</TableHead>
            <TableHead>Received / လက်ခံရရှိ</TableHead>
            <TableHead>Issued / ထုတ်ပေး</TableHead>
            <TableHead>Closing Stock / ပိတ်သိမ်းစတော့ခ်</TableHead>
            <TableHead>Value / တန်ဖိုး</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {reports.map((report) => (
            <TableRow key={report.id}>
              <TableCell>{report.date}</TableCell>
              <TableCell>{report.warehouse}</TableCell>
              <TableCell>{report.item}</TableCell>
              <TableCell>{report.category}</TableCell>
              <TableCell>{formatNumber(report.openingStock)}</TableCell>
              <TableCell>{formatNumber(report.received)}</TableCell>
              <TableCell>{formatNumber(report.issued)}</TableCell>
              <TableCell>{formatNumber(report.closingStock)}</TableCell>
              <TableCell>{formatCurrency(report.value)}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}