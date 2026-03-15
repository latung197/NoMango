import React from 'react';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { QCReport } from '../data/reportsData';
import { getQCStatusBadge, calculatePassRate } from '../utils/reportUtils';
import { Badge } from '../ui/badge';

interface QCReportTableProps {
  reports: QCReport[];
}

export function QCReportTable({ reports }: QCReportTableProps) {
  return (
    <div className="overflow-x-auto">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Date / ရက်စွဲ</TableHead>
            <TableHead>Product / ထုတ်ကုန်</TableHead>
            <TableHead>Batch / အစုအဖွဲ့</TableHead>
            <TableHead>Inspector / စစ်ဆေးသူ</TableHead>
            <TableHead>Tested / စစ်ဆေးပြီး</TableHead>
            <TableHead>Passed / ဖြတ်သန်းပြီး</TableHead>
            <TableHead>Failed / မအောင်မြင်</TableHead>
            <TableHead>Pass Rate / ဖြတ်သန်းနှုန်း</TableHead>
            <TableHead>Defect Type / ချို့ယွင်းမှုအမျိုးအစား</TableHead>
            <TableHead>Status / အခြေအနေ</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {reports.map((report) => {
            const passRate = calculatePassRate(report.passed, report.tested);
            return (
              <TableRow key={report.id}>
                <TableCell>{report.date}</TableCell>
                <TableCell>{report.product}</TableCell>
                <TableCell>{report.batch}</TableCell>
                <TableCell>{report.inspector}</TableCell>
                <TableCell>{report.tested}</TableCell>
                <TableCell>{report.passed}</TableCell>
                <TableCell>{report.failed}</TableCell>
                <TableCell>
                  <div className="flex items-center gap-2">
                    <span>{passRate}%</span>
                    {passRate >= 95 ? (
                      <Badge className="bg-green-100 text-green-800">Excellent</Badge>
                    ) : passRate >= 85 ? (
                      <Badge className="bg-blue-100 text-blue-800">Good</Badge>
                    ) : (
                      <Badge className="bg-yellow-100 text-yellow-800">Needs Improvement</Badge>
                    )}
                  </div>
                </TableCell>
                <TableCell>{report.defectType}</TableCell>
                <TableCell>{getQCStatusBadge(report.status)}</TableCell>
              </TableRow>
            );
          })}
        </TableBody>
      </Table>
    </div>
  );
}