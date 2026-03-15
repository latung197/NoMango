import React, { useState } from 'react';
import {
  Package,
  TrendingUp,
  AlertTriangle,
  AlertCircle,
  Plus,
  Eye,
  Download,
  Search,
  Filter,
  Calendar,
  User,
  Send,
  ArrowRight,
  Clock,
  CheckCircle,
  Warehouse
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
import { toast } from 'sonner@2.0.3';

// Enhanced mock data for Reject Management
const mockRejectRecords = [
  {
    rejectId: 'REJ-20250904-001',
    jobId: 'JOB20250904-001',
    product: '2011 - Plastic Bottle 500ml',
    rejectQty: 180,
    rejectReason: 'Appearance - Scratches',
    dateLogged: '2025-09-04 14:30',
    loggedBy: 'QC Officer 1',
    status: 'pending-warehouse',
    notes: 'Surface scratches, not suitable for quality standards',
    selectedWarehouse: null,
    warehouseMovedDate: null
  },
  {
    rejectId: 'REJ-20250906-001',
    jobId: 'JOB20250906-003',
    product: '2012 - Plastic Container 1L',
    rejectQty: 200,
    rejectReason: 'Deforming - Warped',
    dateLogged: '2025-09-06 19:00',
    loggedBy: 'QC Officer 3',
    status: 'sent-to-warehouse',
    notes: 'Major dimensional deviation beyond rework capability',
    selectedWarehouse: 'Reject-WH1',
    warehouseMovedDate: '2025-09-06 20:15'
  },
  {
    rejectId: 'REJ-20250907-001',
    jobId: 'JOB20250907-003',
    product: '2013 - Plastic Cup 250ml',
    rejectQty: 95,
    rejectReason: 'Others - Material Mix',
    dateLogged: '2025-09-07 11:15',
    loggedBy: 'QC Officer 2',
    status: 'sent-to-warehouse',
    notes: 'Wrong material mix, complete reject',
    selectedWarehouse: 'Reject-WH2',
    warehouseMovedDate: '2025-09-07 12:30'
  },
  {
    rejectId: 'REJ-20250908-001',
    jobId: 'JOB20250908-002',
    product: '2011 - Plastic Bottle 500ml',
    rejectQty: 150,
    rejectReason: 'Reject Codes - R001',
    dateLogged: '2025-09-08 16:45',
    loggedBy: 'QC Officer 1',
    status: 'pending-warehouse',
    notes: 'Standard reject code R001 - Color variation',
    selectedWarehouse: null,
    warehouseMovedDate: null
  }
];

// Reject warehouses
const mockRejectWarehouses = [
  { code: 'Reject-WH1', name: 'Reject Warehouse 1', location: 'Building A', capacity: '80%' },
  { code: 'Reject-WH2', name: 'Reject Warehouse 2', location: 'Building B', capacity: '65%' },
  { code: 'Reject-WH3', name: 'Reject Warehouse 3', location: 'Building C', capacity: '45%' },
  { code: 'Reject-WH4', name: 'Reject Warehouse 4', location: 'Building D', capacity: '30%' }
];

// Movement history mock data
const mockMovementHistory = [
  {
    id: 'MOV-001',
    rejectId: 'REJ-20250906-001',
    jobId: 'JOB20250906-003',
    fromLocation: 'Production Line',
    toWarehouse: 'Reject-WH1',
    quantity: 200,
    movedDate: '2025-09-06 20:15',
    movedBy: 'Warehouse Staff 1'
  },
  {
    id: 'MOV-002',
    rejectId: 'REJ-20250907-001',
    jobId: 'JOB20250907-003',
    fromLocation: 'Production Line',
    toWarehouse: 'Reject-WH2',
    quantity: 95,
    movedDate: '2025-09-07 12:30',
    movedBy: 'Warehouse Staff 2'
  }
];

export function RejectManagement() {
  const [rejectRecords, setRejectRecords] = useState(mockRejectRecords);
  const [movementHistory, setMovementHistory] = useState(mockMovementHistory);
  const [selectedWarehouse, setSelectedWarehouse] = useState<string>('');
  const [selectedRejects, setSelectedRejects] = useState<string[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');

  // Filter reject records
  const filteredRejects = rejectRecords.filter(record => {
    const matchesSearch = record.jobId.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         record.product.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         record.rejectReason.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = statusFilter === 'all' || record.status === statusFilter;
    return matchesSearch && matchesStatus;
  });

  // Calculate statistics
  const totalRejects = rejectRecords.length;
  const pendingWarehouse = rejectRecords.filter(r => r.status === 'pending-warehouse').length;
  const sentToWarehouse = rejectRecords.filter(r => r.status === 'sent-to-warehouse').length;
  const totalRejectQty = rejectRecords.reduce((sum, r) => sum + r.rejectQty, 0);

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'pending-warehouse': return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'sent-to-warehouse': return 'bg-green-100 text-green-800 border-green-200';
      default: return 'bg-slate-100 text-slate-800 border-slate-200';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'pending-warehouse': return 'Pending Warehouse';
      case 'sent-to-warehouse': return 'Sent to Warehouse';
      default: return 'Unknown';
    }
  };

  const handleSendToWarehouse = (rejectIds: string[]) => {
    if (!selectedWarehouse) {
      toast.error('Please select a warehouse first');
      return;
    }

    if (rejectIds.length === 0) {
      toast.error('Please select reject records to send');
      return;
    }

    // Update reject records
    const updatedRejects = rejectRecords.map(record => {
      if (rejectIds.includes(record.rejectId) && record.status === 'pending-warehouse') {
        return {
          ...record,
          status: 'sent-to-warehouse',
          selectedWarehouse,
          warehouseMovedDate: new Date().toISOString().slice(0, 16).replace('T', ' ')
        };
      }
      return record;
    });

    // Add movement history entries
    const newMovements = rejectIds
      .filter(id => rejectRecords.find(r => r.rejectId === id)?.status === 'pending-warehouse')
      .map(id => {
        const record = rejectRecords.find(r => r.rejectId === id);
        return {
          id: `MOV-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`,
          rejectId: id,
          jobId: record?.jobId || '',
          fromLocation: 'Production Line',
          toWarehouse: selectedWarehouse,
          quantity: record?.rejectQty || 0,
          movedDate: new Date().toISOString().slice(0, 16).replace('T', ' '),
          movedBy: 'Production Control User'
        };
      });

    setRejectRecords(updatedRejects);
    setMovementHistory([...movementHistory, ...newMovements]);
    setSelectedRejects([]);
    setSelectedWarehouse('');

    toast.success(`✅ ${rejectIds.length} reject record(s) sent to ${mockRejectWarehouses.find(w => w.code === selectedWarehouse)?.name || selectedWarehouse} successfully!`);
  };

  const handleSelectReject = (rejectId: string) => {
    setSelectedRejects(prev => 
      prev.includes(rejectId) 
        ? prev.filter(id => id !== rejectId)
        : [...prev, rejectId]
    );
  };

  const handleSelectAll = () => {
    const pendingRejects = filteredRejects.filter(r => r.status === 'pending-warehouse').map(r => r.rejectId);
    setSelectedRejects(pendingRejects.length === selectedRejects.length ? [] : pendingRejects);
  };

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="mb-6">
        <div className="flex items-center gap-3 mb-2">
          <div className="flex items-center justify-center w-8 h-8 bg-orange-100 rounded-full">
            <Package className="h-5 w-5 text-orange-600" />
          </div>
          <h1 className="text-xl font-semibold text-slate-900">
            Reject Management | ပယ်ဖျက်ပစ္စည်း စီမံ
          </h1>
        </div>
        <div className="flex items-center gap-2 text-sm text-slate-600 ml-11">
          <Package className="h-4 w-4" />
          Manage and track rejected products from production lines to reject warehouses
        </div>
      </div>

      {/* Summary Dashboard */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-2xl font-bold text-slate-700">{totalRejects}</div>
                <div className="text-sm text-slate-600">Total Rejects</div>
              </div>
              <Package className="h-8 w-8 text-slate-600" />
            </div>
          </CardContent>
        </Card>

        <Card className="border-yellow-200 bg-yellow-50">
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-2xl font-bold text-yellow-600">{pendingWarehouse}</div>
                <div className="text-sm text-yellow-700">Pending Warehouse</div>
              </div>
              <Clock className="h-8 w-8 text-yellow-600" />
            </div>
          </CardContent>
        </Card>

        <Card className="border-green-200 bg-green-50">
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-2xl font-bold text-green-600">{sentToWarehouse}</div>
                <div className="text-sm text-green-700">Sent to Warehouse</div>
              </div>
              <CheckCircle className="h-8 w-8 text-green-600" />
            </div>
          </CardContent>
        </Card>

        <Card className="border-red-200 bg-red-50">
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-2xl font-bold text-red-600">{totalRejectQty.toLocaleString()}</div>
                <div className="text-sm text-red-700">Total Reject Qty</div>
              </div>
              <AlertTriangle className="h-8 w-8 text-red-600" />
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Warehouse Selection & Action Panel */}
      <Card className="border-blue-200 bg-blue-50">
        <CardHeader>
          <CardTitle className="text-lg text-blue-900">Warehouse Assignment</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex items-center gap-4">
            <div className="flex-1">
              <Label htmlFor="warehouse-select" className="text-sm font-medium text-blue-900 mb-2 block">
                Select Warehouse | ဂိုဒေါင်ရွေးချယ်ပါ
              </Label>
              <Select value={selectedWarehouse} onValueChange={setSelectedWarehouse}>
                <SelectTrigger className="bg-white border-blue-200">
                  <SelectValue placeholder="Choose reject warehouse..." />
                </SelectTrigger>
                <SelectContent>
                  {mockRejectWarehouses.map((warehouse) => (
                    <SelectItem key={warehouse.code} value={warehouse.code}>
                      <div className="flex items-center justify-between w-full">
                        <span>{warehouse.name}</span>
                        <Badge variant="outline" className="ml-2">
                          {warehouse.capacity}
                        </Badge>
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="flex gap-2 pt-6">
              <Button
                onClick={() => handleSendToWarehouse(selectedRejects)}
                disabled={!selectedWarehouse || selectedRejects.length === 0}
                className="bg-blue-600 hover:bg-blue-700"
              >
                <Send className="h-4 w-4 mr-2" />
                Send to Warehouse
              </Button>
              <Button
                variant="outline"
                onClick={() => setSelectedRejects([])}
                disabled={selectedRejects.length === 0}
              >
                Clear Selection
              </Button>
            </div>
          </div>
          {selectedRejects.length > 0 && (
            <div className="mt-4 p-3 bg-blue-100 rounded-lg">
              <div className="text-sm text-blue-800">
                <strong>{selectedRejects.length}</strong> reject record(s) selected for warehouse transfer
              </div>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Reject Records Table */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5 text-orange-600" />
              Job Rejects List | အလုပ်ပယ်ဖျက်စာရင်း
              <Badge className="bg-orange-100 text-orange-800 ml-auto">
                {filteredRejects.length} records
              </Badge>
            </CardTitle>
            <div className="flex items-center gap-4">
              <div className="flex items-center gap-2">
                <Search className="h-4 w-4 text-slate-400" />
                <Input
                  placeholder="Search by Job ID, Product, or Reason..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="w-64"
                />
              </div>
              <Select value={statusFilter} onValueChange={setStatusFilter}>
                <SelectTrigger className="w-48">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Status</SelectItem>
                  <SelectItem value="pending-warehouse">Pending Warehouse</SelectItem>
                  <SelectItem value="sent-to-warehouse">Sent to Warehouse</SelectItem>
                </SelectContent>
              </Select>
              <Button variant="outline" size="sm">
                <Download className="h-4 w-4 mr-2" />
                Export
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow className="bg-slate-50">
                  <TableHead className="w-12">
                    <input
                      type="checkbox"
                      checked={selectedRejects.length === filteredRejects.filter(r => r.status === 'pending-warehouse').length && filteredRejects.filter(r => r.status === 'pending-warehouse').length > 0}
                      onChange={handleSelectAll}
                      className="rounded"
                    />
                  </TableHead>
                  <TableHead>Job ID</TableHead>
                  <TableHead>Product</TableHead>
                  <TableHead>Reject Qty</TableHead>
                  <TableHead>Reason</TableHead>
                  <TableHead>Selected Warehouse</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Date Logged</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredRejects.map((record) => (
                  <TableRow key={record.rejectId} className="hover:bg-slate-50">
                    <TableCell>
                      <input
                        type="checkbox"
                        checked={selectedRejects.includes(record.rejectId)}
                        onChange={() => handleSelectReject(record.rejectId)}
                        disabled={record.status !== 'pending-warehouse'}
                        className="rounded"
                      />
                    </TableCell>
                    <TableCell className="font-mono font-medium text-blue-600">
                      {record.jobId}
                    </TableCell>
                    <TableCell>
                      <div className="font-medium">{record.product}</div>
                    </TableCell>
                    <TableCell className="text-right font-mono">
                      <span className="text-red-600 font-medium">
                        {record.rejectQty.toLocaleString()} pcs
                      </span>
                    </TableCell>
                    <TableCell>
                      <div className="max-w-32 truncate">{record.rejectReason}</div>
                    </TableCell>
                    <TableCell>
                      {record.selectedWarehouse ? (
                        <Badge variant="outline" className="text-blue-700 border-blue-200 bg-blue-50">
                          {record.selectedWarehouse}
                        </Badge>
                      ) : (
                        <span className="text-slate-400">-</span>
                      )}
                    </TableCell>
                    <TableCell>
                      <Badge className={getStatusColor(record.status)}>
                        {getStatusText(record.status)}
                      </Badge>
                    </TableCell>
                    <TableCell className="font-mono text-sm">
                      {record.dateLogged.split(' ')[0]}
                    </TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <Button variant="outline" size="sm">
                          <Eye className="h-4 w-4" />
                        </Button>
                        {record.status === 'pending-warehouse' && (
                          <Button
                            size="sm"
                            onClick={() => {
                              setSelectedRejects([record.rejectId]);
                              if (selectedWarehouse) {
                                handleSendToWarehouse([record.rejectId]);
                              }
                            }}
                            disabled={!selectedWarehouse}
                            className="bg-blue-600 hover:bg-blue-700"
                          >
                            <Send className="h-4 w-4" />
                          </Button>
                        )}
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>

      {/* Movement History */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <ArrowRight className="h-5 w-5 text-green-600" />
            Movement History | ရွေ့လျားမှုသမိုင်း
            <Badge className="bg-green-100 text-green-800 ml-auto">
              {movementHistory.length} movements
            </Badge>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow className="bg-slate-50">
                  <TableHead>Movement ID</TableHead>
                  <TableHead>Job ID</TableHead>
                  <TableHead>From → To</TableHead>
                  <TableHead>Quantity</TableHead>
                  <TableHead>Moved Date</TableHead>
                  <TableHead>Moved By</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {movementHistory.map((movement) => (
                  <TableRow key={movement.id} className="hover:bg-slate-50">
                    <TableCell className="font-mono text-sm">{movement.id}</TableCell>
                    <TableCell className="font-mono font-medium text-blue-600">
                      {movement.jobId}
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <span className="text-slate-600">{movement.fromLocation}</span>
                        <ArrowRight className="h-4 w-4 text-slate-400" />
                        <Badge variant="outline" className="text-blue-700 border-blue-200 bg-blue-50">
                          {movement.toWarehouse}
                        </Badge>
                      </div>
                    </TableCell>
                    <TableCell className="text-right font-mono">
                      {movement.quantity.toLocaleString()} pcs
                    </TableCell>
                    <TableCell className="font-mono text-sm">
                      {movement.movedDate}
                    </TableCell>
                    <TableCell className="text-slate-600">
                      {movement.movedBy}
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
}