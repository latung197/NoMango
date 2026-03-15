import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell, LineChart, Line } from 'recharts';
import { Avatar, AvatarFallback, AvatarImage } from '../ui/avatar';
import { 
  FileText,
  Download,
  Calendar,
  Users,
  Clock,
  TrendingUp,
  AlertTriangle,
  CheckCircle,
  BarChart3,
  PieChart as PieChartIcon
} from 'lucide-react';

// Mock data for reports
const attendanceSummary = [
  { date: '2025-09-01', present: 89, late: 5, absent: 2, total: 96 },
  { date: '2025-09-02', present: 92, late: 3, absent: 1, total: 96 },
  { date: '2025-09-03', present: 87, late: 7, absent: 2, total: 96 },
  { date: '2025-09-04', present: 90, late: 4, absent: 2, total: 96 },
  { date: '2025-09-05', present: 88, late: 6, absent: 2, total: 96 },
  { date: '2025-09-06', present: 91, late: 4, absent: 1, total: 96 },
  { date: '2025-09-07', present: 89, late: 5, absent: 2, total: 96 }
];

const departmentData = [
  { name: 'Production', present: 42, late: 3, absent: 1, total: 46 },
  { name: 'Engineering', present: 11, late: 1, absent: 0, total: 12 },
  { name: 'Quality Control', present: 7, late: 1, absent: 0, total: 8 },
  { name: 'Administration', present: 6, late: 0, absent: 0, total: 6 },
  { name: 'Finance', present: 4, late: 0, absent: 0, total: 4 },
  { name: 'Maintenance', present: 8, late: 0, absent: 1, total: 9 }
];

const shiftAssignmentData = [
  { shift: 'Morning Shift', assigned: 45, present: 42, efficiency: 93.3 },
  { shift: 'Evening Shift', assigned: 32, present: 30, efficiency: 93.8 },
  { shift: 'Night Shift', assigned: 0, present: 0, efficiency: 0 },
  { shift: 'Flexible', assigned: 19, present: 17, efficiency: 89.5 }
];

const flexibleHoursData = [
  { name: 'Ko Zaw', target: 40, actual: 35, compliance: 87.5, status: 'underworked' },
  { name: 'Ma Su', target: 35, actual: 42, compliance: 120, status: 'overworked' },
  { name: 'Ma Aye', target: 40, actual: 39, compliance: 97.5, status: 'on-track' },
  { name: 'Ko Min', target: 40, actual: 38, compliance: 95, status: 'on-track' },
  { name: 'Ma Khin', target: 35, actual: 34, compliance: 97.1, status: 'on-track' },
  { name: 'Ko Htun', target: 40, actual: 43, compliance: 107.5, status: 'overworked' }
];

const productivityData = [
  { name: 'Ko Thant', hoursWorked: 48, outputUnits: 1250, productivity: 26.04, department: 'Production' },
  { name: 'Ma Hla', hoursWorked: 47, outputUnits: 1180, productivity: 25.11, department: 'Production' },
  { name: 'Ko Aung', hoursWorked: 46, outputUnits: 1200, productivity: 26.09, department: 'Production' },
  { name: 'Ma Thin', hoursWorked: 44, outputUnits: 1100, productivity: 25.00, department: 'Production' },
  { name: 'Ko Zaw', hoursWorked: 35, outputUnits: 0, productivity: 0, department: 'Engineering' },
  { name: 'Ma Su', hoursWorked: 42, outputUnits: 0, productivity: 0, department: 'Quality Control' }
];

const lateAbsentData = [
  { name: 'Ma Hla', department: 'Production', lateCount: 3, absentCount: 0, totalDays: 7, lateReason: 'Traffic' },
  { name: 'Ko Min', department: 'Engineering', lateCount: 2, absentCount: 1, totalDays: 7, lateReason: 'Family issue' },
  { name: 'Ma Khin', department: 'Quality Control', lateCount: 1, absentCount: 0, totalDays: 7, lateReason: 'Transport' },
  { name: 'Ko Htun', department: 'Production', lateCount: 1, absentCount: 1, totalDays: 7, lateReason: 'Medical' }
];

const reportTypes = [
  {
    id: 'daily-attendance',
    name: 'Daily Attendance Sheet',
    nameLocal: 'နေ့စဉ်တက်ရောက်မှုစာရွက်',
    description: 'Combined shift and flexible staff attendance',
    automated: true
  },
  {
    id: 'working-hours',
    name: 'Working Hours Summary',
    nameLocal: 'အလုပ်ချိန်အကျဉ်းချုပ်',
    description: 'Per day/week/month working hours analysis',
    automated: true
  },
  {
    id: 'shift-assignment',
    name: 'Shift Assignment Summary',
    nameLocal: 'အလုပ်ပတ်ခန့်အပ်မှုအကျဉ်းချုပ်',
    description: 'Who is assigned where and efficiency metrics',
    automated: false
  },
  {
    id: 'flexible-compliance',
    name: 'Flexible Hours Compliance',
    nameLocal: 'လိုက်လျောညီထွေချိန်လိုက်နာမှု',
    description: 'Target vs actual hours for flexible staff',
    automated: true
  },
  {
    id: 'absence-late',
    name: 'Absence & Late Report',
    nameLocal: 'မရှိခြင်းနှင့် နောက်ကျခြင်းအစီရင်ခံစာ',
    description: 'Staff attendance issues and patterns',
    automated: false
  },
  {
    id: 'productivity',
    name: 'Operator Productivity Report',
    nameLocal: 'အော်ပရေတာကုန်ထုတ်စွမ်းအားအစီရင်ခံစာ',
    description: 'Cross-linked with production logs (read-only)',
    automated: true
  }
];

export function HRReports() {
  const [selectedReport, setSelectedReport] = useState('daily-attendance');
  const [dateRange, setDateRange] = useState('week');
  const [startDate, setStartDate] = useState('2025-09-01');
  const [endDate, setEndDate] = useState('2025-09-07');
  const [selectedDepartment, setSelectedDepartment] = useState('all');

  const currentReport = reportTypes.find(r => r.id === selectedReport);

  const generateReport = () => {
    // Here you would typically generate the actual report
    window.print(); // Simple print for demo
  };

  const exportReport = (format: 'pdf' | 'excel') => {
    // Here you would export the report in the specified format
    alert(`Exporting ${currentReport?.name} as ${format.toUpperCase()}`);
  };

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-pink-100 rounded-lg">
              <FileText className="h-6 w-6 text-pink-600" />
            </div>
            <div>
              <h1 className="text-3xl font-semibold text-slate-900">
                HR Reports | HR အစီရင်ခံစာများ
              </h1>
              <p className="text-slate-600 mt-1">
                Comprehensive HR analytics and reporting system
              </p>
              <p className="text-sm text-slate-500">
                ပြည့်စုံသော HR ခွဲခြမ်းစိတ်ဖြာမှုနှင့် အစီရင်ခံစာစနစ်
              </p>
            </div>
          </div>
          
          <div className="flex items-center gap-3">
            <Button variant="outline" onClick={() => exportReport('pdf')} className="gap-2">
              <Download className="h-4 w-4" />
              Export PDF
            </Button>
            <Button variant="outline" onClick={() => exportReport('excel')} className="gap-2">
              <Download className="h-4 w-4" />
              Export Excel
            </Button>
            <Button onClick={generateReport} className="gap-2 bg-pink-600 hover:bg-pink-700">
              <FileText className="h-4 w-4" />
              Generate Report
            </Button>
          </div>
        </div>

        {/* Report Selection and Filters */}
        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
              <div>
                <Label className="text-sm">Report Type | အစီရင်ခံစာအမျိုးအစား</Label>
                <Select value={selectedReport} onValueChange={setSelectedReport}>
                  <SelectTrigger className="mt-1">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    {reportTypes.map(report => (
                      <SelectItem key={report.id} value={report.id}>
                        <div>
                          <div className="font-medium">{report.name}</div>
                          <div className="text-xs text-slate-500">{report.nameLocal}</div>
                        </div>
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              
              <div>
                <Label className="text-sm">Date Range | ရက်စွဲအပိုင်းအခြား</Label>
                <Select value={dateRange} onValueChange={setDateRange}>
                  <SelectTrigger className="mt-1">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="today">Today | ယနေ့</SelectItem>
                    <SelectItem value="week">This Week | ဤအပတ်</SelectItem>
                    <SelectItem value="month">This Month | ဤလ</SelectItem>
                    <SelectItem value="custom">Custom | စိတ်ကြိုက်</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              
              <div>
                <Label className="text-sm">Start Date | စတင်ရက်စွဲ</Label>
                <Input
                  type="date"
                  value={startDate}
                  onChange={(e) => setStartDate(e.target.value)}
                  className="mt-1"
                />
              </div>
              
              <div>
                <Label className="text-sm">End Date | ကုန်ဆုံးရက်စွဲ</Label>
                <Input
                  type="date"
                  value={endDate}
                  onChange={(e) => setEndDate(e.target.value)}
                  className="mt-1"
                />
              </div>
              
              <div>
                <Label className="text-sm">Department | ဌာန</Label>
                <Select value={selectedDepartment} onValueChange={setSelectedDepartment}>
                  <SelectTrigger className="mt-1">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Departments</SelectItem>
                    <SelectItem value="production">Production</SelectItem>
                    <SelectItem value="engineering">Engineering</SelectItem>
                    <SelectItem value="quality-control">Quality Control</SelectItem>
                    <SelectItem value="administration">Administration</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Report Description */}
        {currentReport && (
          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center justify-between">
                <div>
                  <h3 className="text-lg font-medium text-slate-900">{currentReport.name}</h3>
                  <p className="text-base text-slate-600">{currentReport.nameLocal}</p>
                  <p className="text-sm text-slate-500 mt-1">{currentReport.description}</p>
                </div>
                <div className="flex items-center gap-2">
                  <Badge className={currentReport.automated ? 'bg-green-100 text-green-800' : 'bg-blue-100 text-blue-800'}>
                    {currentReport.automated ? 'Automated' : 'Manual'}
                  </Badge>
                </div>
              </div>
            </CardContent>
          </Card>
        )}

        {/* Report Content Based on Selection */}
        {selectedReport === 'daily-attendance' && (
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Attendance Chart */}
            <Card className="border-0 shadow-sm">
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <BarChart3 className="h-5 w-5 text-blue-600" />
                  <div>
                    <div>Weekly Attendance Trend</div>
                    <div className="text-sm text-slate-500">အပတ်စဉ်တက်ရောက်မှုလမ်းကြောင်း</div>
                  </div>
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="h-64">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={attendanceSummary}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="date" tickFormatter={(date) => new Date(date).toLocaleDateString('en-US', { weekday: 'short' })} />
                      <YAxis />
                      <Tooltip />
                      <Bar dataKey="present" fill="#10b981" name="Present" />
                      <Bar dataKey="late" fill="#f59e0b" name="Late" />
                      <Bar dataKey="absent" fill="#ef4444" name="Absent" />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </CardContent>
            </Card>

            {/* Department Breakdown */}
            <Card className="border-0 shadow-sm">
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Users className="h-5 w-5 text-green-600" />
                  <div>
                    <div>Department Attendance</div>
                    <div className="text-sm text-slate-500">ဌာနအလိုက်တက်ရောက်မှု</div>
                  </div>
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-3">
                  {departmentData.map((dept, index) => {
                    const presentRate = (dept.present / dept.total) * 100;
                    
                    return (
                      <div key={index} className="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
                        <div>
                          <div className="font-medium">{dept.name}</div>
                          <div className="text-sm text-slate-500">
                            {dept.present}/{dept.total} present
                          </div>
                        </div>
                        <div className="flex items-center gap-3">
                          <div className="w-24 bg-slate-200 rounded-full h-2">
                            <div 
                              className="bg-green-500 h-2 rounded-full" 
                              style={{ width: `${presentRate}%` }}
                            ></div>
                          </div>
                          <span className="text-sm font-medium w-12 text-right">
                            {presentRate.toFixed(0)}%
                          </span>
                        </div>
                      </div>
                    );
                  })}
                </div>
              </CardContent>
            </Card>
          </div>
        )}

        {selectedReport === 'shift-assignment' && (
          <Card className="border-0 shadow-sm">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Clock className="h-5 w-5 text-orange-600" />
                <div>
                  <div>Shift Assignment Summary</div>
                  <div className="text-sm text-slate-500">အလုပ်ပတ်ခန့်အပ်မှုအကျဉ်းချုပ်</div>
                </div>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Shift | အလုပ်ပတ်</TableHead>
                    <TableHead>Assigned | ခန့်အပ်ထားသော</TableHead>
                    <TableHead>Present | တက်ရောက်နေသော</TableHead>
                    <TableHead>Efficiency | ထိရောက်မှု</TableHead>
                    <TableHead>Status | အခြေအနေ</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {shiftAssignmentData.map((shift, index) => (
                    <TableRow key={index}>
                      <TableCell className="font-medium">{shift.shift}</TableCell>
                      <TableCell>{shift.assigned}</TableCell>
                      <TableCell>{shift.present}</TableCell>
                      <TableCell>{shift.efficiency.toFixed(1)}%</TableCell>
                      <TableCell>
                        <Badge className={
                          shift.efficiency >= 95 ? 'bg-green-100 text-green-800' :
                          shift.efficiency >= 85 ? 'bg-yellow-100 text-yellow-800' :
                          'bg-red-100 text-red-800'
                        }>
                          {shift.efficiency >= 95 ? 'Excellent' :
                           shift.efficiency >= 85 ? 'Good' : 'Needs Attention'}
                        </Badge>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </CardContent>
          </Card>
        )}

        {selectedReport === 'flexible-compliance' && (
          <Card className="border-0 shadow-sm">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <TrendingUp className="h-5 w-5 text-purple-600" />
                <div>
                  <div>Flexible Hours Compliance</div>
                  <div className="text-sm text-slate-500">လိုက်လျောညီထွေချိန်လိုက်နာမှု</div>
                </div>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Staff | ဝန်ထမ်း</TableHead>
                    <TableHead>Target Hours | ပစ်မှတ်ချိန်</TableHead>
                    <TableHead>Actual Hours | လက်ရှိချိန်</TableHead>
                    <TableHead>Compliance | လိုက်နာမှု</TableHead>
                    <TableHead>Status | အခြေအနေ</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {flexibleHoursData.map((staff, index) => (
                    <TableRow key={index}>
                      <TableCell className="font-medium">{staff.name}</TableCell>
                      <TableCell>{staff.target} hrs</TableCell>
                      <TableCell>{staff.actual} hrs</TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <div className="w-16 bg-slate-200 rounded-full h-2">
                            <div 
                              className={`h-2 rounded-full ${
                                staff.compliance >= 90 ? 'bg-green-500' : 
                                staff.compliance >= 70 ? 'bg-yellow-500' : 'bg-red-500'
                              }`}
                              style={{ width: `${Math.min(100, staff.compliance)}%` }}
                            ></div>
                          </div>
                          <span className="text-sm font-medium">{staff.compliance.toFixed(1)}%</span>
                        </div>
                      </TableCell>
                      <TableCell>
                        <Badge className={
                          staff.status === 'on-track' ? 'bg-green-100 text-green-800' :
                          staff.status === 'overworked' ? 'bg-orange-100 text-orange-800' :
                          'bg-red-100 text-red-800'
                        }>
                          {staff.status === 'on-track' ? 'On Track' :
                           staff.status === 'overworked' ? 'Overworked' : 'Underworked'}
                        </Badge>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </CardContent>
          </Card>
        )}

        {selectedReport === 'absence-late' && (
          <Card className="border-0 shadow-sm">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <AlertTriangle className="h-5 w-5 text-red-600" />
                <div>
                  <div>Absence & Late Report</div>
                  <div className="text-sm text-slate-500">မရှိခြင်းနှင့် နောက်ကျခြင်းအစီရင်ခံစာ</div>
                </div>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Staff | ဝန်ထမ်း</TableHead>
                    <TableHead>Department | ဌာန</TableHead>
                    <TableHead>Late Count | နောက်ကျအကြိမ်</TableHead>
                    <TableHead>Absent Count | မရှိအကြိမ်</TableHead>
                    <TableHead>Attendance Rate | တက်ရောက်မှုနှုန်း</TableHead>
                    <TableHead>Primary Reason | အဓိကအကြောင်းရင်း</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {lateAbsentData.map((staff, index) => {
                    const attendanceRate = ((staff.totalDays - staff.absentCount) / staff.totalDays) * 100;
                    
                    return (
                      <TableRow key={index}>
                        <TableCell className="font-medium">{staff.name}</TableCell>
                        <TableCell>{staff.department}</TableCell>
                        <TableCell>
                          <Badge variant="outline" className="text-yellow-700">
                            {staff.lateCount}
                          </Badge>
                        </TableCell>
                        <TableCell>
                          <Badge variant="outline" className="text-red-700">
                            {staff.absentCount}
                          </Badge>
                        </TableCell>
                        <TableCell>
                          <div className="flex items-center gap-2">
                            <div className="w-16 bg-slate-200 rounded-full h-2">
                              <div 
                                className={`h-2 rounded-full ${
                                  attendanceRate >= 95 ? 'bg-green-500' : 
                                  attendanceRate >= 85 ? 'bg-yellow-500' : 'bg-red-500'
                                }`}
                                style={{ width: `${attendanceRate}%` }}
                              ></div>
                            </div>
                            <span className="text-sm font-medium">{attendanceRate.toFixed(0)}%</span>
                          </div>
                        </TableCell>
                        <TableCell className="text-sm">{staff.lateReason}</TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
            </CardContent>
          </Card>
        )}

        {selectedReport === 'productivity' && (
          <Card className="border-0 shadow-sm">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <TrendingUp className="h-5 w-5 text-blue-600" />
                <div>
                  <div>Operator Productivity Report</div>
                  <div className="text-sm text-slate-500">အော်ပရေတာကုန်ထုတ်စွမ်းအားအစီရင်ခံစာ</div>
                </div>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="mb-4 p-3 bg-blue-50 border border-blue-200 rounded-lg">
                <div className="flex items-center gap-2 text-blue-800 text-sm">
                  <div className="w-2 h-2 rounded-full bg-blue-400"></div>
                  <span>This report is cross-linked with Production logs (read-only view)</span>
                </div>
              </div>
              
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Operator | အော်ပရေတာ</TableHead>
                    <TableHead>Department | ဌာန</TableHead>
                    <TableHead>Hours Worked | အလုပ်ချိန်</TableHead>
                    <TableHead>Output Units | ထုတ်လုပ်အရေအတွက်</TableHead>
                    <TableHead>Productivity | ကုန်ထုတ်စွမ်းအား</TableHead>
                    <TableHead>Performance | စွမ်းဆောင်ရည်</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {productivityData.map((operator, index) => (
                    <TableRow key={index}>
                      <TableCell>
                        <div className="flex items-center gap-3">
                          <Avatar className="h-8 w-8">
                            <AvatarFallback className="text-xs">
                              {operator.name.split(' ').map(n => n[0]).join('')}
                            </AvatarFallback>
                          </Avatar>
                          <span className="font-medium">{operator.name}</span>
                        </div>
                      </TableCell>
                      <TableCell>{operator.department}</TableCell>
                      <TableCell>{operator.hoursWorked}h</TableCell>
                      <TableCell>
                        {operator.outputUnits > 0 ? operator.outputUnits.toLocaleString() : 'N/A'}
                      </TableCell>
                      <TableCell>
                        {operator.productivity > 0 ? `${operator.productivity.toFixed(2)} units/hr` : 'N/A'}
                      </TableCell>
                      <TableCell>
                        {operator.department === 'Production' ? (
                          <Badge className={
                            operator.productivity >= 25 ? 'bg-green-100 text-green-800' :
                            operator.productivity >= 20 ? 'bg-yellow-100 text-yellow-800' :
                            'bg-red-100 text-red-800'
                          }>
                            {operator.productivity >= 25 ? 'High' :
                             operator.productivity >= 20 ? 'Average' : 'Low'}
                          </Badge>
                        ) : (
                          <Badge className="bg-slate-100 text-slate-600">
                            Non-Production
                          </Badge>
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </CardContent>
          </Card>
        )}

        {/* Available Reports List */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <FileText className="h-5 w-5 text-pink-600" />
              <div>
                <div>Available HR Reports | ရရှိနိုင်သော HR အစီရင်ခံစာများ</div>
                <div className="text-sm text-slate-500">Complete list of HR reporting capabilities</div>
              </div>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {reportTypes.map((report) => (
                <div 
                  key={report.id} 
                  className={`p-4 border-2 rounded-lg cursor-pointer transition-colors ${
                    selectedReport === report.id 
                      ? 'border-pink-300 bg-pink-50' 
                      : 'border-slate-200 hover:border-slate-300'
                  }`}
                  onClick={() => setSelectedReport(report.id)}
                >
                  <div className="flex items-center justify-between mb-2">
                    <h4 className="font-medium text-slate-900">{report.name}</h4>
                    <Badge className={report.automated ? 'bg-green-100 text-green-800' : 'bg-blue-100 text-blue-800'}>
                      {report.automated ? 'Auto' : 'Manual'}
                    </Badge>
                  </div>
                  <p className="text-base text-slate-600 mb-1">{report.nameLocal}</p>
                  <p className="text-sm text-slate-500">{report.description}</p>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}