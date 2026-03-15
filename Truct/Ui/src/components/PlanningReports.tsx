import React, { useState } from 'react';
import {
  FileText,
  ClipboardList,
  BarChart3,
  Package,
  Search,
  Filter,
  Download,
  Calendar,
  TrendingUp,
  AlertCircle,
  CheckCircle,
  Clock,
  User,
  Boxes,
  ChevronRight
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Badge } from './ui/badge';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';

// Mock data for reports
const mockPlanList = [
  {
    planId: 'PL20250903-001',
    parentProduct: '2011 - Plastic Bottle 500ml',
    totalQty: 1000,
    status: 'final',
    createdDate: '2025-09-03',
    planner: 'Ko Aye',
    completedJobs: 2,
    totalJobs: 3
  },
  {
    planId: 'PL20250903-002',
    parentProduct: '2012 - Plastic Container 1L',
    totalQty: 500,
    status: 'in-progress',
    createdDate: '2025-09-03',
    planner: 'Ko Aung',
    completedJobs: 1,
    totalJobs: 2
  },
  {
    planId: 'PL20250902-001',
    parentProduct: '2013 - Plastic Cup 250ml',
    totalQty: 2000,
    status: 'completed',
    createdDate: '2025-09-02',
    planner: 'Ko Mya',
    completedJobs: 4,
    totalJobs: 4
  },
  {
    planId: 'PL20250902-002',
    parentProduct: '2011 - Plastic Bottle 500ml',
    totalQty: 1500,
    status: 'draft',
    createdDate: '2025-09-02',
    planner: 'Ko Tun',
    completedJobs: 0,
    totalJobs: 3
  }
];

const mockPlanVsActual = [
  {
    planId: 'PL20250903-001',
    jobId: 'PL20250903-001-J1',
    childProduct: '2011-01 - Bottle Body',
    plannedQty: 1000,
    actualQty: 950,
    variance: -50,
    variancePercent: -5.0,
    status: 'completed',
    startDate: '2025-09-03 08:00',
    endDate: '2025-09-03 16:00',
    operator: 'Operator A'
  },
  {
    planId: 'PL20250903-001',
    jobId: 'PL20250903-001-J2',
    childProduct: '2011-02 - Bottle Cap',
    plannedQty: 1000,
    actualQty: 1020,
    variance: 20,
    variancePercent: 2.0,
    status: 'completed',
    startDate: '2025-09-03 09:00',
    endDate: '2025-09-03 13:00',
    operator: 'Operator B'
  },
  {
    planId: 'PL20250903-001',
    jobId: 'PL20250903-001-J3',
    childProduct: '2011-03 - Product Label',
    plannedQty: 1000,
    actualQty: 0,
    variance: -1000,
    variancePercent: -100.0,
    status: 'pending',
    startDate: null,
    endDate: null,
    operator: null
  },
  {
    planId: 'PL20250903-002',
    jobId: 'PL20250903-002-J1',
    childProduct: '2012-01 - Container Body',
    plannedQty: 500,
    actualQty: 480,
    variance: -20,
    variancePercent: -4.0,
    status: 'completed',
    startDate: '2025-09-03 10:00',
    endDate: '2025-09-03 18:00',
    operator: 'Operator C'
  }
];

const mockMaterialUsage = [
  {
    planId: 'PL20250903-001',
    jobId: 'PL20250903-001-J1',
    childProduct: '2011-01 - Bottle Body',
    materialCode: 'PET001',
    materialName: 'PET Resin Clear',
    plannedUsage: 20.0,
    actualUsage: 19.5,
    unit: 'kg',
    variance: -0.5,
    variancePercent: -2.5,
    wasteQty: 0.3
  },
  {
    planId: 'PL20250903-001',
    jobId: 'PL20250903-001-J2',
    childProduct: '2011-02 - Bottle Cap',
    materialCode: 'PP002',
    materialName: 'PP Blue Compound',
    plannedUsage: 3.5,
    actualUsage: 3.7,
    unit: 'kg',
    variance: 0.2,
    variancePercent: 5.7,
    wasteQty: 0.1
  },
  {
    planId: 'PL20250903-002',
    jobId: 'PL20250903-002-J1',
    childProduct: '2012-01 - Container Body',
    materialCode: 'HDPE003',
    materialName: 'HDPE Natural',
    plannedUsage: 15.0,
    actualUsage: 14.8,
    unit: 'kg',
    variance: -0.2,
    variancePercent: -1.3,
    wasteQty: 0.4
  }
];

const mockJobTraceability = [
  {
    planId: 'PL20250903-001',
    jobId: 'PL20250903-001-J1',
    childProduct: '2011-01 - Bottle Body',
    machine: 'INJ-M001',
    operator: 'Operator A (ID: OP001)',
    shift: 'Day Shift A',
    startTime: '2025-09-03 08:00:00',
    endTime: '2025-09-03 16:00:00',
    duration: '8h 0m',
    outputQty: 950,
    qualityOk: 920,
    qualityReject: 30,
    moldNo: 'MD001',
    cycleTime: 30.5,
    totalCycles: 1000,
    materialBatch: 'PET001-B240901'
  },
  {
    planId: 'PL20250903-001',
    jobId: 'PL20250903-001-J2',
    childProduct: '2011-02 - Bottle Cap',
    machine: 'INJ-M002',
    operator: 'Operator B (ID: OP002)',
    shift: 'Day Shift A',
    startTime: '2025-09-03 09:00:00',
    endTime: '2025-09-03 13:00:00',
    duration: '4h 0m',
    outputQty: 1020,
    qualityOk: 1015,
    qualityReject: 5,
    moldNo: 'MD002',
    cycleTime: 14.1,
    totalCycles: 1025,
    materialBatch: 'PP002-B240902'
  }
];

export function PlanningReports() {
  const [activeTab, setActiveTab] = useState('plan-list');
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedStatus, setSelectedStatus] = useState('all');
  const [selectedPlanner, setSelectedPlanner] = useState('all');
  const [dateRange, setDateRange] = useState('week');

  const renderPlanList = () => (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <ClipboardList className="h-5 w-5 text-blue-600" />
            Plan List Report
            <span className="text-sm font-normal text-slate-600">အစီအစဉ်စာရင်းအစီရင်ခံစာ</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          {/* Filters */}
          <div className="flex gap-4 mb-6">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-3 h-4 w-4 text-slate-400" />
              <Input
                placeholder="Search plans..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-10"
              />
            </div>
            <Select value={selectedStatus} onValueChange={setSelectedStatus}>
              <SelectTrigger className="w-48">
                <SelectValue placeholder="Filter by Status" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Status</SelectItem>
                <SelectItem value="draft">Draft</SelectItem>
                <SelectItem value="final">Final</SelectItem>
                <SelectItem value="in-progress">In Progress</SelectItem>
                <SelectItem value="completed">Completed</SelectItem>
              </SelectContent>
            </Select>
            <Select value={selectedPlanner} onValueChange={setSelectedPlanner}>
              <SelectTrigger className="w-48">
                <SelectValue placeholder="Filter by Planner" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Planners</SelectItem>
                <SelectItem value="Ko Tun">Ko Tun</SelectItem>
                <SelectItem value="Ko Aye">Ko Aye</SelectItem>
                <SelectItem value="Ko Mya">Ko Mya</SelectItem>
                <SelectItem value="Ko Aung">Ko Aung</SelectItem>
              </SelectContent>
            </Select>
            <Button variant="outline" size="sm">
              <Download className="h-4 w-4 mr-2" />
              Export
            </Button>
          </div>

          {/* Plan List Table */}
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Plan ID</TableHead>
                  <TableHead>Parent Product</TableHead>
                  <TableHead>Total Qty</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Created Date</TableHead>
                  <TableHead>Planner</TableHead>
                  <TableHead>Job Progress</TableHead>
                  <TableHead>Completion %</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockPlanList.map((plan) => (
                  <TableRow key={plan.planId}>
                    <TableCell className="font-mono">{plan.planId}</TableCell>
                    <TableCell>{plan.parentProduct}</TableCell>
                    <TableCell>{plan.totalQty.toLocaleString()}</TableCell>
                    <TableCell>
                      <Badge 
                        variant={
                          plan.status === 'completed' ? 'default' : 
                          plan.status === 'in-progress' ? 'secondary' : 
                          plan.status === 'final' ? 'outline' : 
                          'outline'
                        }
                      >
                        {plan.status}
                      </Badge>
                    </TableCell>
                    <TableCell>{plan.createdDate}</TableCell>
                    <TableCell>{plan.planner}</TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <span className="text-sm">
                          {plan.completedJobs}/{plan.totalJobs}
                        </span>
                        <div className="w-20 h-2 bg-slate-200 rounded-full">
                          <div 
                            className="h-full bg-blue-600 rounded-full"
                            style={{ width: `${(plan.completedJobs / plan.totalJobs) * 100}%` }}
                          ></div>
                        </div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <span className="font-medium">
                        {Math.round((plan.completedJobs / plan.totalJobs) * 100)}%
                      </span>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const renderPlanVsActual = () => (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <BarChart3 className="h-5 w-5 text-green-600" />
            Plan vs Actual Report
            <span className="text-sm font-normal text-slate-600">အစီအစဉ်နှင့်အမှန်တကယ်နှိုင်းယှဉ်ချက်</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex gap-4 mb-6">
            <Button variant="outline" size="sm">
              <Filter className="h-4 w-4 mr-2" />
              Filter by Date
            </Button>
            <Button variant="outline" size="sm">
              <Download className="h-4 w-4 mr-2" />
              Export
            </Button>
          </div>

          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Plan ID</TableHead>
                  <TableHead>Job ID</TableHead>
                  <TableHead>Child Product</TableHead>
                  <TableHead>Planned Qty</TableHead>
                  <TableHead>Actual Qty</TableHead>
                  <TableHead>Variance</TableHead>
                  <TableHead>Variance %</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Operator</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockPlanVsActual.map((item, index) => (
                  <TableRow key={index}>
                    <TableCell className="font-mono text-sm">{item.planId}</TableCell>
                    <TableCell className="font-mono text-sm">{item.jobId}</TableCell>
                    <TableCell>{item.childProduct}</TableCell>
                    <TableCell>{item.plannedQty.toLocaleString()}</TableCell>
                    <TableCell>{item.actualQty.toLocaleString()}</TableCell>
                    <TableCell>
                      <span className={`font-medium ${item.variance >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                        {item.variance >= 0 ? '+' : ''}{item.variance}
                      </span>
                    </TableCell>
                    <TableCell>
                      <span className={`font-medium ${item.variancePercent >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                        {item.variancePercent >= 0 ? '+' : ''}{item.variancePercent.toFixed(1)}%
                      </span>
                    </TableCell>
                    <TableCell>
                      <Badge 
                        variant={
                          item.status === 'completed' ? 'default' : 
                          item.status === 'in-progress' ? 'secondary' : 
                          'outline'
                        }
                      >
                        {item.status}
                      </Badge>
                    </TableCell>
                    <TableCell>{item.operator || 'Not assigned'}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const renderMaterialUsage = () => (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Package className="h-5 w-5 text-purple-600" />
            Material Usage Report
            <span className="text-sm font-normal text-slate-600">ပစ္စည်းအသုံးပြုမှုအစီရင်ခံစာ</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex gap-4 mb-6">
            <Select>
              <SelectTrigger className="w-48">
                <SelectValue placeholder="Select Plan ID" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Plans</SelectItem>
                <SelectItem value="PL20250903-001">PL20250903-001</SelectItem>
                <SelectItem value="PL20250903-002">PL20250903-002</SelectItem>
              </SelectContent>
            </Select>
            <Button variant="outline" size="sm">
              <Download className="h-4 w-4 mr-2" />
              Export
            </Button>
          </div>

          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Plan ID</TableHead>
                  <TableHead>Job ID</TableHead>
                  <TableHead>Child Product</TableHead>
                  <TableHead>Material Code</TableHead>
                  <TableHead>Material Name</TableHead>
                  <TableHead>Planned Usage</TableHead>
                  <TableHead>Actual Usage</TableHead>
                  <TableHead>Variance</TableHead>
                  <TableHead>Variance %</TableHead>
                  <TableHead>Waste Qty</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockMaterialUsage.map((item, index) => (
                  <TableRow key={index}>
                    <TableCell className="font-mono text-sm">{item.planId}</TableCell>
                    <TableCell className="font-mono text-sm">{item.jobId}</TableCell>
                    <TableCell>{item.childProduct}</TableCell>
                    <TableCell className="font-mono">{item.materialCode}</TableCell>
                    <TableCell>{item.materialName}</TableCell>
                    <TableCell>{item.plannedUsage} {item.unit}</TableCell>
                    <TableCell>{item.actualUsage} {item.unit}</TableCell>
                    <TableCell>
                      <span className={`font-medium ${item.variance >= 0 ? 'text-red-600' : 'text-green-600'}`}>
                        {item.variance >= 0 ? '+' : ''}{item.variance} {item.unit}
                      </span>
                    </TableCell>
                    <TableCell>
                      <span className={`font-medium ${item.variancePercent >= 0 ? 'text-red-600' : 'text-green-600'}`}>
                        {item.variancePercent >= 0 ? '+' : ''}{item.variancePercent.toFixed(1)}%
                      </span>
                    </TableCell>
                    <TableCell>{item.wasteQty} {item.unit}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const renderJobTraceability = () => (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <TrendingUp className="h-5 w-5 text-orange-600" />
            Job Traceability Report
            <span className="text-sm font-normal text-slate-600">အလုပ်ခြေရာခံအစီရင်ခံစာ</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex gap-4 mb-6">
            <div className="relative">
              <Search className="absolute left-3 top-3 h-4 w-4 text-slate-400" />
              <Input
                placeholder="Search by Plan ID or Job ID..."
                className="pl-10 w-64"
              />
            </div>
            <Button variant="outline" size="sm">
              <Download className="h-4 w-4 mr-2" />
              Export
            </Button>
          </div>

          <div className="space-y-4">
            {mockJobTraceability.map((job, index) => (
              <Card key={index} className="border">
                <CardHeader className="pb-3">
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                      <div className="flex items-center gap-2">
                        <Badge variant="outline" className="font-mono text-xs">
                          {job.planId}
                        </Badge>
                        <ChevronRight className="h-4 w-4 text-slate-400" />
                        <Badge variant="secondary" className="font-mono text-xs">
                          {job.jobId}
                        </Badge>
                      </div>
                      <div className="text-sm font-medium text-slate-900">
                        {job.childProduct}
                      </div>
                    </div>
                    <div className="flex items-center gap-2">
                      <CheckCircle className="h-4 w-4 text-green-600" />
                      <span className="text-sm text-green-600">Completed</span>
                    </div>
                  </div>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <div>
                      <Label className="text-xs text-slate-500">Machine</Label>
                      <div className="font-medium">{job.machine}</div>
                    </div>
                    <div>
                      <Label className="text-xs text-slate-500">Operator</Label>
                      <div className="font-medium">{job.operator}</div>
                    </div>
                    <div>
                      <Label className="text-xs text-slate-500">Shift</Label>
                      <div className="font-medium">{job.shift}</div>
                    </div>
                    <div>
                      <Label className="text-xs text-slate-500">Duration</Label>
                      <div className="font-medium">{job.duration}</div>
                    </div>
                  </div>

                  <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <div>
                      <Label className="text-xs text-slate-500">Output Qty</Label>
                      <div className="font-medium text-blue-600">{job.outputQty.toLocaleString()}</div>
                    </div>
                    <div>
                      <Label className="text-xs text-slate-500">Quality OK</Label>
                      <div className="font-medium text-green-600">{job.qualityOk.toLocaleString()}</div>
                    </div>
                    <div>
                      <Label className="text-xs text-slate-500">Quality Reject</Label>
                      <div className="font-medium text-red-600">{job.qualityReject.toLocaleString()}</div>
                    </div>
                    <div>
                      <Label className="text-xs text-slate-500">Reject Rate</Label>
                      <div className="font-medium text-red-600">
                        {((job.qualityReject / job.outputQty) * 100).toFixed(1)}%
                      </div>
                    </div>
                  </div>

                  <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <div>
                      <Label className="text-xs text-slate-500">Mold No.</Label>
                      <div className="font-medium">{job.moldNo}</div>
                    </div>
                    <div>
                      <Label className="text-xs text-slate-500">Cycle Time</Label>
                      <div className="font-medium">{job.cycleTime}s</div>
                    </div>
                    <div>
                      <Label className="text-xs text-slate-500">Total Cycles</Label>
                      <div className="font-medium">{job.totalCycles.toLocaleString()}</div>
                    </div>
                    <div>
                      <Label className="text-xs text-slate-500">Material Batch</Label>
                      <div className="font-mono text-sm">{job.materialBatch}</div>
                    </div>
                  </div>

                  {job.qualityReject > 0 && (
                    <div className="bg-red-50 border border-red-200 rounded-lg p-3">
                      <div className="flex items-center gap-2">
                        <AlertCircle className="h-4 w-4 text-red-600" />
                        <span className="text-sm font-medium text-red-700">Reject Reason</span>
                      </div>
                      <div className="text-sm text-red-600 mt-1">{job.rejectReason}</div>
                    </div>
                  )}
                </CardContent>
              </Card>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-slate-900">Planning Reports</h1>
            <p className="text-sm text-slate-600">စီမံခန့်ခွဲမှုအစီရင်ခံစာများ</p>
          </div>
          <div className="flex gap-2">
            <Button variant="outline" className="gap-2">
              <Calendar className="h-4 w-4" />
              Date Range
            </Button>
            <Button className="bg-blue-600 hover:bg-blue-700 gap-2">
              <Download className="h-4 w-4" />
              Export All
            </Button>
          </div>
        </div>

        {/* Reports Summary Cards */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-slate-600">Total Plans</p>
                  <p className="text-2xl font-bold text-slate-900">24</p>
                </div>
                <ClipboardList className="h-8 w-8 text-blue-600" />
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-slate-600">Completed Jobs</p>
                  <p className="text-2xl font-bold text-green-600">87</p>
                </div>
                <CheckCircle className="h-8 w-8 text-green-600" />
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-slate-600">Avg. Efficiency</p>
                  <p className="text-2xl font-bold text-orange-600">94.2%</p>
                </div>
                <TrendingUp className="h-8 w-8 text-orange-600" />
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-slate-600">Material Waste</p>
                  <p className="text-2xl font-bold text-red-600">2.8%</p>
                </div>
                <AlertCircle className="h-8 w-8 text-red-600" />
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Reports Tabs */}
        <Card>
          <CardContent className="pt-6">
            <Tabs value={activeTab} onValueChange={setActiveTab}>
              <TabsList className="grid w-full grid-cols-4">
                <TabsTrigger value="plan-list" className="flex items-center gap-2">
                  <ClipboardList className="h-4 w-4" />
                  Plan List
                </TabsTrigger>
                <TabsTrigger value="plan-vs-actual" className="flex items-center gap-2">
                  <BarChart3 className="h-4 w-4" />
                  Plan vs Actual
                </TabsTrigger>
                <TabsTrigger value="material-usage" className="flex items-center gap-2">
                  <Package className="h-4 w-4" />
                  Material Usage
                </TabsTrigger>
                <TabsTrigger value="job-traceability" className="flex items-center gap-2">
                  <TrendingUp className="h-4 w-4" />
                  Job Traceability
                </TabsTrigger>
              </TabsList>

              <div className="mt-6">
                <TabsContent value="plan-list" className="space-y-4">
                  {renderPlanList()}
                </TabsContent>

                <TabsContent value="plan-vs-actual" className="space-y-4">
                  {renderPlanVsActual()}
                </TabsContent>

                <TabsContent value="material-usage" className="space-y-4">
                  {renderMaterialUsage()}
                </TabsContent>

                <TabsContent value="job-traceability" className="space-y-4">
                  {renderJobTraceability()}
                </TabsContent>
              </div>
            </Tabs>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}