import React from 'react';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { ProductionReport } from '../data/reportsData';
import { getEfficiencyBadge, getQCStatusBadge } from '../utils/reportUtils';

interface ProductionReportTableProps {
  reports: ProductionReport[];
}

export function ProductionReportTable({ reports }: ProductionReportTableProps) {
  return (
    <div className="overflow-x-auto">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Date / ရက်စွဲ</TableHead>
            <TableHead>Shift / ပေါ်တီ</TableHead>
            <TableHead>Machine / စက်</TableHead>
            <TableHead>Product / ထုတ်ကုန်</TableHead>
            <TableHead>Target / ပစ်မှတ်</TableHead>
            <TableHead>Actual / အမှန်တကယ်</TableHead>
            <TableHead>Efficiency / ထိရောက်မှု</TableHead>
            <TableHead>Downtime / ရပ်နားချိန်</TableHead>
            <TableHead>Operator / အော်ပရေတာ</TableHead>
            <TableHead>QC Status</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {reports.map((report) => (
            <TableRow key={report.id}>
              <TableCell>{report.date}</TableCell>
              <TableCell>{report.shift}</TableCell>
              <TableCell>{report.machine}</TableCell>
              <TableCell>{report.product}</TableCell>
              <TableCell>{report.targetQuantity.toLocaleString()}</TableCell>
              <TableCell>{report.actualQuantity.toLocaleString()}</TableCell>
              <TableCell>
                <div className="flex items-center gap-2">
                  <span>{report.efficiency}%</span>
                  {getEfficiencyBadge(report.efficiency)}
                </div>
              </TableCell>
              <TableCell>{report.downtime} min</TableCell>
              <TableCell>{report.operator}</TableCell>
              <TableCell>{getQCStatusBadge(report.qcStatus)}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}