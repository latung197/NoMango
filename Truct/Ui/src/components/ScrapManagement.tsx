import React, { useState } from 'react';
import {
  Recycle,
  TrendingUp,
  AlertTriangle,
  AlertCircle,
  Plus,
  Eye,
  Download,
  Search,
  Filter,
  Calendar,
  User
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Badge } from './ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Textarea } from './ui/textarea';

// Enhanced mock data for Reject Management
const mockRejectRecords = [
  {
    rejectId: 'REJ-20250904-001',
    jobId: 'JOB20250904-001',
    product: '2011 - Plastic Bottle 500ml',
    rejectQty: 180,
    rejectReason: 'Short shot',
    dateLogged: '2025-09-04 14:30',
    loggedBy: 'QC Officer 1',
    status: 'logged',
    notes: 'Incomplete filling, not suitable for rework'
  },
  {
    rejectId: 'REJ-20250906-001',
    jobId: 'JOB20250906-003',
    product: '2012 - Plastic Container 1L',
    rejectQty: 200,
    rejectReason: 'Dimension',
    dateLogged: '2025-09-06 19:00',
    loggedBy: 'QC Officer 3',
    status: 'reviewed',
    notes: 'Major dimensional deviation beyond rework capability'
  },
  {
    rejectId: 'REJ-20250907-001',
    jobId: 'JOB20250907-003',
    product: '2012 - Plastic Container 1L',
    rejectQty: 1800,
    rejectReason: 'Dimension',
    dateLogged: '2025-09-07 15:20',
    loggedBy: 'QC Officer 1',
    status: 'logged',
    notes: 'Major dimensional issues beyond repair'
  },
  {
    rejectId: 'REJ-20250903-001',
    jobId: 'JOB20250903-004',
    product: '2013 - Plastic Cup 250ml',
    rejectQty: 125,
    rejectReason: 'Contamination',
    dateLogged: '2025-09-03 11:15',
    loggedBy: 'QC Officer 2',
    status: 'reviewed',
    notes: 'Material contamination detected during production'
  },
  {
    rejectId: 'REJ-20250902-001',
    jobId: 'JOB20250902-003',
    product: '2011 - Plastic Bottle 500ml',
    rejectQty: 95,
    rejectReason: 'Broken',
    dateLogged: '2025-09-02 16:45',
    loggedBy: 'Operator B (OP002)',
    status: 'logged',
    notes: 'Broken parts during ejection process'
  },
  {
    rejectId: 'REJ-20250901-001',
    jobId: 'JOB20250901-005',
    product: '2012 - Plastic Container 1L',
    rejectQty: 350,
    rejectReason: 'Overheated',
    dateLogged: '2025-09-01 13:30',
    loggedBy: 'QC Officer 1',
    status: 'reviewed',
    notes: 'Material overheated, structural integrity compromised'
  },
  {
    rejectId: 'REJ-20250831-001',
    jobId: 'JOB20250831-002',
    product: '2013 - Plastic Cup 250ml',
    rejectQty: 80,
    rejectReason: 'Color variation',
    dateLogged: '2025-08-31 14:00',
    loggedBy: 'QC Officer 2',
    status: 'logged',
    notes: 'Severe color inconsistency outside tolerance'
  },
  {
    rejectId: 'REJ-20250830-001',
    jobId: 'JOB20250830-001',
    product: '2011 - Plastic Bottle 500ml',
    rejectQty: 220,
    rejectReason: 'Material defect',
    dateLogged: '2025-08-30 10:20',
    loggedBy: 'QC Officer 3',
    status: 'reviewed',
    notes: 'Raw material quality issue'
  }
];

export function ScrapManagement() {
  const [searchTerm, setSearchTerm] = useState('');
  const [logRejectDialog, setLogRejectDialog] = useState(false);
  const [viewHistoryDialog, setViewHistoryDialog] = useState(false);
  const [selectedJobForHistory, setSelectedJobForHistory] = useState<any>(null);
  const [rejectFormData, setRejectFormData] = useState({
    jobId: '',
    product: '',
    rejectQty: '',
    rejectReason: '',
    notes: ''
  });
  const [rejectDateFilter, setRejectDateFilter] = useState('all');
  const [rejectProductFilter, setRejectProductFilter] = useState('all');
  const [rejectReasonFilter, setRejectReasonFilter] = useState('all');

  // Calculate summary metrics
  const todayRejectQty = mockRejectRecords
    .filter(s => s.dateLogged.startsWith('2025-09-04'))
    .reduce((sum, s) => sum + s.rejectQty, 0);
  
  const thisMonthRejectQty = mockRejectRecords
    .filter(s => s.dateLogged.startsWith('2025-09'))
    .reduce((sum, s) => sum + s.rejectQty, 0);
  
  const totalProductionQty = 50000; // Mock total production for percentage calculation
  const rejectPercentage = ((thisMonthRejectQty / totalProductionQty) * 100).toFixed(1);
  
  const topRejectReasons = Object.entries(
    mockRejectRecords.reduce((acc: any, record) => {
      acc[record.rejectReason] = (acc[record.rejectReason] || 0) + record.rejectQty;
      return acc;
    }, {})
  ).sort(([,a], [,b]) => (b as number) - (a as number))[0];

  // Filter reject records
  const filteredRecords = mockRejectRecords.filter(record => {
    const matchesSearch = record.rejectId.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         record.jobId.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         record.product.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesDate = rejectDateFilter === 'all' || 
                       (rejectDateFilter === 'today' && record.dateLogged.startsWith('2025-09-04')) ||
                       (rejectDateFilter === 'week' && record.dateLogged >= '2025-09-01') ||
                       (rejectDateFilter === 'month' && record.dateLogged.startsWith('2025-09'));
    
    const matchesProduct = rejectProductFilter === 'all' || record.product.includes(rejectProductFilter);
    const matchesReason = rejectReasonFilter === 'all' || record.rejectReason === rejectReasonFilter;
    
    return matchesSearch && matchesDate && matchesProduct && matchesReason;
  });

  const getStatusBadge = (status: string) => {
    const config = status === 'logged' 
      ? { variant: 'secondary' as const, color: 'text-slate-600', label: 'Logged' }
      : { variant: 'default' as const, color: 'text-green-600', label: 'Reviewed' };
    
    return <Badge variant={config.variant} className={config.color}>{config.label}</Badge>;
  };

  const handleLogReject = () => {
    console.log('Logging reject:', rejectFormData);
    setLogRejectDialog(false);
    setRejectFormData({
      jobId: '',
      product: '',
      rejectQty: '',
      rejectReason: '',
      notes: ''
    });
  };

  const handleViewHistory = (jobId: string) => {
    const historyRecords = mockRejectRecords.filter(record => record.jobId === jobId);
    setSelectedJobForHistory({ jobId, records: historyRecords });
    setViewHistoryDialog(true);
  };

  const handleExport = (format: 'excel' | 'pdf') => {
    console.log(`Exporting reject data to ${format.toUpperCase()}`);
    // Mock export functionality
  };

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto">
        {/* Page Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-slate-900 mb-2">
            Reject Management | အမှိုက် စီမံခန့်ခွဲမှု
          </h1>
          <p className="text-slate-600">
            Log and track irreparable products that cannot be reworked.
          </p>
        </div>

        {/* Summary Cards */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-slate-600">Total Reject Qty Today</p>
                  <p className="text-2xl font-bold text-red-600">{todayRejectQty}</p>
                </div>
                <Recycle className="h-8 w-8 text-red-600" />
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-slate-600">Total Reject Qty This Month</p>
                  <p className="text-2xl font-bold text-orange-600">{thisMonthRejectQty}</p>
                </div>
                <TrendingUp className="h-8 w-8 text-orange-600" />
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-slate-600">Reject % of Production</p>
                  <p className="text-2xl font-bold text-purple-600">{rejectPercentage}%</p>
                </div>
                <AlertTriangle className="h-8 w-8 text-purple-600" />
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-slate-600">Top Reject Reason</p>
                  <p className="text-lg font-bold text-blue-600">{topRejectReasons ? topRejectReasons[0] : 'N/A'}</p>
                  <p className="text-sm text-slate-500">Qty: {topRejectReasons ? topRejectReasons[1] : 0}</p>
                </div>
                <AlertCircle className="h-8 w-8 text-blue-600" />
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Controls */}
        <Card className="mb-6">
          <CardHeader>
            <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
              <CardTitle>Reject Records</CardTitle>
              <div className="flex flex-wrap gap-2">
                <Dialog open={logRejectDialog} onOpenChange={setLogRejectDialog}>
                  <DialogTrigger asChild>
                    <Button className="bg-blue-600 hover:bg-blue-700">
                      <Plus className="w-4 h-4 mr-2" />
                      Log Reject
                    </Button>
                  </DialogTrigger>
                  <DialogContent>
                    <DialogHeader>
                      <DialogTitle>Log New Reject</DialogTitle>
                    </DialogHeader>
                    <div className="space-y-4 pt-4">
                      <div className="grid grid-cols-2 gap-4">
                        <div className="space-y-2">
                          <Label htmlFor="jobId">Job ID</Label>
                          <Input
                            id="jobId"
                            value={rejectFormData.jobId}
                            onChange={(e) => setRejectFormData({...rejectFormData, jobId: e.target.value})}
                            placeholder="JOB20250904-001"
                          />
                        </div>
                        <div className="space-y-2">
                          <Label htmlFor="product">Product</Label>
                          <Input
                            id="product"
                            value={rejectFormData.product}
                            onChange={(e) => setRejectFormData({...rejectFormData, product: e.target.value})}
                            placeholder="Product name"
                          />
                        </div>
                      </div>
                      <div className="grid grid-cols-2 gap-4">
                        <div className="space-y-2">
                          <Label htmlFor="rejectQty">Reject Quantity</Label>
                          <Input
                            id="rejectQty"
                            type="number"
                            value={rejectFormData.rejectQty}
                            onChange={(e) => setRejectFormData({...rejectFormData, rejectQty: e.target.value})}
                            placeholder="0"
                          />
                        </div>
                        <div className="space-y-2">
                          <Label htmlFor="rejectReason">Reject Reason</Label>
                          <Select 
                            value={rejectFormData.rejectReason} 
                            onValueChange={(value) => setRejectFormData({...rejectFormData, rejectReason: value})}
                          >
                            <SelectTrigger>
                              <SelectValue placeholder="Select reason" />
                            </SelectTrigger>
                            <SelectContent>
                              <SelectItem value="Contamination">Contamination</SelectItem>
                              <SelectItem value="Broken">Broken</SelectItem>
                              <SelectItem value="Overheated">Overheated</SelectItem>
                              <SelectItem value="Dimension">Dimension</SelectItem>
                              <SelectItem value="Short shot">Short shot</SelectItem>
                              <SelectItem value="Color variation">Color variation</SelectItem>
                              <SelectItem value="Material defect">Material defect</SelectItem>
                            </SelectContent>
                          </Select>
                        </div>
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="notes">Notes</Label>
                        <Textarea
                          id="notes"
                          value={rejectFormData.notes}
                          onChange={(e) => setRejectFormData({...rejectFormData, notes: e.target.value})}
                          placeholder="Additional notes about the reject..."
                          rows={3}
                        />
                      </div>
                      <div className="flex justify-end gap-2 pt-4">
                        <Button variant="outline" onClick={() => setLogRejectDialog(false)}>
                          Cancel
                        </Button>
                        <Button onClick={handleLogReject}>
                          Log Reject
                        </Button>
                      </div>
                    </div>
                  </DialogContent>
                </Dialog>

                <Button variant="outline" onClick={() => handleExport('excel')}>
                  <Download className="w-4 h-4 mr-2" />
                  Export Excel
                </Button>
                <Button variant="outline" onClick={() => handleExport('pdf')}>
                  <Download className="w-4 h-4 mr-2" />
                  Export PDF
                </Button>
              </div>
            </div>
          </CardHeader>
          
          <CardContent>
            {/* Filters */}
            <div className="flex flex-col sm:flex-row gap-4 mb-6">
              <div className="flex-1">
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400 w-4 h-4" />
                  <Input
                    placeholder="Search by Reject ID, Job ID, or Product..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="pl-10"
                  />
                </div>
              </div>
              
              <Select value={rejectDateFilter} onValueChange={setRejectDateFilter}>
                <SelectTrigger className="w-40">
                  <Calendar className="w-4 h-4 mr-2" />
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Dates</SelectItem>
                  <SelectItem value="today">Today</SelectItem>
                  <SelectItem value="week">This Week</SelectItem>
                  <SelectItem value="month">This Month</SelectItem>
                </SelectContent>
              </Select>

              <Select value={rejectProductFilter} onValueChange={setRejectProductFilter}>
                <SelectTrigger className="w-48">
                  <Filter className="w-4 h-4 mr-2" />
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Products</SelectItem>
                  <SelectItem value="Plastic Bottle">Plastic Bottle</SelectItem>
                  <SelectItem value="Plastic Container">Plastic Container</SelectItem>
                  <SelectItem value="Plastic Cup">Plastic Cup</SelectItem>
                </SelectContent>
              </Select>

              <Select value={rejectReasonFilter} onValueChange={setRejectReasonFilter}>
                <SelectTrigger className="w-48">
                  <Filter className="w-4 h-4 mr-2" />
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Reasons</SelectItem>
                  <SelectItem value="Contamination">Contamination</SelectItem>
                  <SelectItem value="Broken">Broken</SelectItem>
                  <SelectItem value="Overheated">Overheated</SelectItem>
                  <SelectItem value="Dimension">Dimension</SelectItem>
                  <SelectItem value="Short shot">Short shot</SelectItem>
                  <SelectItem value="Color variation">Color variation</SelectItem>
                  <SelectItem value="Material defect">Material defect</SelectItem>
                </SelectContent>
              </Select>
            </div>

            {/* Reject Records Table */}
            <div className="border rounded-lg overflow-hidden">
              <Table>
                <TableHeader>
                  <TableRow className="bg-slate-50">
                    <TableHead>Reject ID</TableHead>
                    <TableHead>Job ID</TableHead>
                    <TableHead>Product</TableHead>
                    <TableHead className="text-red-600">Reject Qty</TableHead>
                    <TableHead>Reject Reason</TableHead>
                    <TableHead>Date Logged</TableHead>
                    <TableHead>Logged By</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredRecords.map((record) => (
                    <TableRow key={record.rejectId} className="hover:bg-slate-50">
                      <TableCell className="font-medium">{record.rejectId}</TableCell>
                      <TableCell>
                        <Button 
                          variant="link" 
                          className="p-0 h-auto text-blue-600"
                          onClick={() => handleViewHistory(record.jobId)}
                        >
                          {record.jobId}
                        </Button>
                      </TableCell>
                      <TableCell>{record.product}</TableCell>
                      <TableCell className="text-red-600 font-semibold">
                        {record.rejectQty.toLocaleString()}
                      </TableCell>
                      <TableCell>
                        <Badge variant="outline" className="bg-red-50 text-red-700 border-red-200">
                          {record.rejectReason}
                        </Badge>
                      </TableCell>
                      <TableCell>{record.dateLogged}</TableCell>
                      <TableCell>
                        <div className="flex items-center">
                          <User className="w-4 h-4 mr-1 text-slate-400" />
                          {record.loggedBy}
                        </div>
                      </TableCell>
                      <TableCell>{getStatusBadge(record.status)}</TableCell>
                      <TableCell>
                        <Button 
                          variant="outline" 
                          size="sm"
                          onClick={() => handleViewHistory(record.jobId)}
                        >
                          <Eye className="w-4 h-4 mr-1" />
                          View History
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>

            {filteredRecords.length === 0 && (
              <div className="text-center py-8 text-slate-500">
                No reject records found matching your criteria.
              </div>
            )}
          </CardContent>
        </Card>

        {/* View History Dialog */}
        <Dialog open={viewHistoryDialog} onOpenChange={setViewHistoryDialog}>
          <DialogContent className="max-w-4xl">
            <DialogHeader>
              <DialogTitle>
                Reject History for Job: {selectedJobForHistory?.jobId}
              </DialogTitle>
            </DialogHeader>
            <div className="pt-4">
              {selectedJobForHistory?.records?.length > 0 ? (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Reject ID</TableHead>
                      <TableHead>Reject Qty</TableHead>
                      <TableHead>Reason</TableHead>
                      <TableHead>Date</TableHead>
                      <TableHead>Logged By</TableHead>
                      <TableHead>Notes</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {selectedJobForHistory.records.map((record: any) => (
                      <TableRow key={record.rejectId}>
                        <TableCell>{record.rejectId}</TableCell>
                        <TableCell className="text-red-600 font-semibold">
                          {record.rejectQty}
                        </TableCell>
                        <TableCell>
                          <Badge variant="outline" className="bg-red-50 text-red-700">
                            {record.rejectReason}
                          </Badge>
                        </TableCell>
                        <TableCell>{record.dateLogged}</TableCell>
                        <TableCell>{record.loggedBy}</TableCell>
                        <TableCell className="max-w-xs truncate">{record.notes}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              ) : (
                <div className="text-center py-8 text-slate-500">
                  No reject history found for this job.
                </div>
              )}
            </div>
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}