import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Textarea } from './ui/textarea';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from './ui/dialog';
import { Alert, AlertDescription } from './ui/alert';
import { Separator } from './ui/separator';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { toast } from 'sonner@2.0.3';
import { 
  Package,
  ArrowDown,
  ArrowUp,
  ArrowLeftRight,
  QrCode,
  Search,
  Clock,
  CheckCircle,
  XCircle,
  User,
  Calendar,
  Building,
  BarChart3,
  Settings,
  LogOut,
  Plus,
  Minus,
  Send,
  AlertCircle,
  Eye
} from 'lucide-react';

interface WarehouseManagementProps {
  warehouse: {
    id: string;
    name: string;
    code: string;
    type: string;
    manager: {
      name: string;
      initials: string;
    };
  };
  onBack: () => void;
}

export function WarehouseManagement({ warehouse, onBack }: WarehouseManagementProps) {
  const [activeTab, setActiveTab] = useState('stock-balance');
  const [scanInput, setScanInput] = useState('');
  const [selectedItem, setSelectedItem] = useState('');
  const [quantity, setQuantity] = useState('');
  const [batchJob, setBatchJob] = useState('');
  const [fromWarehouse, setFromWarehouse] = useState('');
  const [toWarehouse, setToWarehouse] = useState('');
  const [reason, setReason] = useState('');
  const [notes, setNotes] = useState('');
  const [showAdjustmentModal, setShowAdjustmentModal] = useState(false);
  const [adjustmentData, setAdjustmentData] = useState({
    item: '',
    quantity: '',
    type: 'add',
    reason: '',
    note: '',
    password: ''
  });

  // Mock data
  const mockItems = [
    { id: 'ITM-001', name: 'Blue Pellet Grade A', code: 'BP-A-001', unit: 'kg' },
    { id: 'ITM-002', name: 'Red Pellet Grade B', code: 'RP-B-002', unit: 'kg' },
    { id: 'ITM-003', name: 'Green Pellet Premium', code: 'GP-P-003', unit: 'kg' },
    { id: 'ITM-004', name: 'Plastic Bottle 500ml', code: 'PB-500-001', unit: 'pcs' },
    { id: 'ITM-005', name: 'Plastic Container 1L', code: 'PC-1L-001', unit: 'pcs' }
  ];

  const mockWarehouses = [
    { id: 'RM-WH', name: 'Raw Material Warehouse' },
    { id: 'PROD-WH', name: 'Production Warehouse' },
    { id: 'FG-WH', name: 'Finished Goods Warehouse' },
    { id: 'PACK-WH', name: 'Packaging Warehouse' }
  ];

  const mockReasons = [
    'Production Request',
    'Quality Control',
    'Transfer to Production',
    'Customer Order',
    'Internal Use',
    'Maintenance',
    'Emergency Stock',
    'Quality Sample'
  ];

  const mockPendingRequests = [
    {
      id: 'REQ-001',
      item: 'Blue Pellet Grade A',
      quantity: 500,
      unit: 'kg',
      from: 'RM-WH',
      requestedBy: 'Ko Aung',
      time: '10:30 AM',
      type: 'stock-in'
    },
    {
      id: 'REQ-002',
      item: 'Red Pellet Grade B',
      quantity: 250,
      unit: 'kg',
      from: 'RM-WH',
      requestedBy: 'Ma Su',
      time: '09:45 AM',
      type: 'stock-in'
    }
  ];

  const mockOutgoingRequests = [
    {
      id: 'OUT-001',
      item: 'Plastic Bottle 500ml',
      quantity: 1000,
      unit: 'pcs',
      destination: 'PROD-WH',
      status: 'Pending Approval',
      submittedAt: '2025-09-25 11:30',
      approver: 'Production Manager'
    },
    {
      id: 'OUT-002',
      item: 'Green Pellet Premium',
      quantity: 300,
      unit: 'kg',
      destination: 'FG-WH',
      status: 'Auto-Approved',
      submittedAt: '2025-09-25 10:15',
      approver: null
    }
  ];

  const mockRecentMovements = [
    {
      id: 'MOV-001',
      type: 'Stock In',
      item: 'Blue Pellet Grade A',
      quantity: 500,
      user: 'Ko Aung',
      time: '11:45 AM',
      from: 'RM-WH'
    },
    {
      id: 'MOV-002',
      type: 'Stock Out',
      item: 'Red Pellet Grade B',
      quantity: 200,
      user: 'Ma Su',
      time: '11:30 AM',
      to: 'PROD-WH'
    },
    {
      id: 'MOV-003',
      type: 'Transfer',
      item: 'Plastic Container 1L',
      quantity: 150,
      user: 'Ko Win',
      time: '11:15 AM',
      to: 'FG-WH'
    }
  ];

  // Comprehensive stock balance data for each warehouse
  const getStockBalanceData = () => {
    if (warehouse.type === 'RM') {
      return [
        {
          id: 'STK-001',
          itemCode: 'PE-HD-001',
          itemName: 'HDPE Natural Grade A',
          category: 'Primary Resin',
          currentStock: 8500,
          reservedQty: 1200,
          availableQty: 7300,
          reorderLevel: 2000,
          maxLevel: 15000,
          unit: 'kg',
          lastMovement: '2024-01-15 14:30',
          batch: 'BATCH-2024-001',
          expiryDate: '2025-06-15',
          location: 'A-01-B-03',
          status: 'normal',
          costPerUnit: 1850
        },
        {
          id: 'STK-002',
          itemCode: 'PP-RED-002',
          itemName: 'Polypropylene Red Masterbatch',
          category: 'Masterbatch',
          currentStock: 450,
          reservedQty: 200,
          availableQty: 250,
          reorderLevel: 500,
          maxLevel: 2000,
          unit: 'kg',
          lastMovement: '2024-01-15 13:45',
          batch: 'BATCH-2024-002',
          expiryDate: '2025-03-20',
          location: 'B-02-C-01',
          status: 'low',
          costPerUnit: 2450
        },
        {
          id: 'STK-003',
          itemCode: 'ADD-UV-001',
          itemName: 'UV Stabilizer Premium',
          category: 'Additives',
          currentStock: 85,
          reservedQty: 50,
          availableQty: 35,
          reorderLevel: 100,
          maxLevel: 500,
          unit: 'kg',
          lastMovement: '2024-01-15 12:20',
          batch: 'BATCH-2024-003',
          expiryDate: '2025-01-10',
          location: 'C-01-A-02',
          status: 'critical',
          costPerUnit: 4200
        },
        {
          id: 'STK-004',
          itemCode: 'PE-BL-003',
          itemName: 'Polyethylene Blue Grade',
          category: 'Primary Resin',
          currentStock: 6385,
          reservedQty: 1350,
          availableQty: 5035,
          reorderLevel: 1500,
          maxLevel: 10000,
          unit: 'kg',
          lastMovement: '2024-01-15 15:10',
          batch: 'BATCH-2024-004',
          expiryDate: '2025-08-25',
          location: 'A-02-B-01',
          status: 'normal',
          costPerUnit: 1920
        }
      ];
    } else if (warehouse.type === 'FG') {
      return [
        {
          id: 'FG-001',
          itemCode: 'BTL-500-CLR',
          itemName: 'Plastic Bottle 500ml Clear',
          category: 'Bottles',
          currentStock: 15400,
          reservedQty: 4500,
          availableQty: 10900,
          reorderLevel: 5000,
          maxLevel: 30000,
          unit: 'pcs',
          lastMovement: '2024-01-15 16:00',
          batch: 'JOB-2024-001',
          expiryDate: null,
          location: 'FG-A-01',
          status: 'normal',
          costPerUnit: 45
        },
        {
          id: 'FG-002',
          itemCode: 'CONT-1L-WHT',
          itemName: 'Food Container 1L White',
          category: 'Containers',
          currentStock: 8930,
          reservedQty: 3200,
          availableQty: 5730,
          reorderLevel: 3000,
          maxLevel: 20000,
          unit: 'pcs',
          lastMovement: '2024-01-15 15:30',
          batch: 'JOB-2024-002',
          expiryDate: null,
          location: 'FG-B-02',
          status: 'normal',
          costPerUnit: 78
        },
        {
          id: 'FG-003',
          itemCode: 'CUP-200-DIS',
          itemName: 'Disposable Cup 200ml',
          category: 'Cups',
          currentStock: 1300,
          reservedQty: 1200,
          availableQty: 100,
          reorderLevel: 2000,
          maxLevel: 15000,
          unit: 'pcs',
          lastMovement: '2024-01-15 13:15',
          batch: 'JOB-2024-003',
          expiryDate: null,
          location: 'FG-C-01',
          status: 'low',
          costPerUnit: 25
        }
      ];
    } else if (warehouse.type === 'PROD') {
      return [
        {
          id: 'WIP-001',
          itemCode: 'WIP-BTL-500',
          itemName: 'Bottle 500ml - In Progress',
          category: 'Work in Progress',
          currentStock: 3200,
          reservedQty: 500,
          availableQty: 2700,
          reorderLevel: 1000,
          maxLevel: 8000,
          unit: 'pcs',
          lastMovement: '2024-01-15 15:45',
          batch: 'WIP-2024-001',
          expiryDate: null,
          location: 'PROD-A-01',
          status: 'normal',
          costPerUnit: 35
        },
        {
          id: 'WIP-002',
          itemCode: 'WIP-CONT-1L',
          itemName: 'Food Container 1L - Semi-Finished',
          category: 'Work in Progress',
          currentStock: 2150,
          reservedQty: 400,
          availableQty: 1750,
          reorderLevel: 2000,
          maxLevel: 5000,
          unit: 'pcs',
          lastMovement: '2024-01-15 14:20',
          batch: 'WIP-2024-002',
          expiryDate: null,
          location: 'PROD-B-01',
          status: 'low',
          costPerUnit: 58
        }
      ];
    } else {
      return [
        {
          id: 'PACK-001',
          itemCode: 'BOX-CARD-L',
          itemName: 'Cardboard Boxes Large',
          category: 'Primary Packaging',
          currentStock: 5200,
          reservedQty: 1500,
          availableQty: 3700,
          reorderLevel: 2000,
          maxLevel: 10000,
          unit: 'pcs',
          lastMovement: '2024-01-15 15:20',
          batch: 'PACK-2024-001',
          expiryDate: null,
          location: 'PACK-A-01',
          status: 'normal',
          costPerUnit: 85
        },
        {
          id: 'PACK-002',
          itemCode: 'LBL-BTL-AST',
          itemName: 'Bottle Labels Assorted',
          category: 'Labels',
          currentStock: 4850,
          reservedQty: 1200,
          availableQty: 3650,
          reorderLevel: 3000,
          maxLevel: 15000,
          unit: 'pcs',
          lastMovement: '2024-01-15 14:45',
          batch: 'PACK-2024-002',
          expiryDate: '2025-12-31',
          location: 'PACK-B-01',
          status: 'normal',
          costPerUnit: 12
        }
      ];
    }
  };

  const [stockBalanceData] = useState(getStockBalanceData());
  const [stockSearchTerm, setStockSearchTerm] = useState('');
  const [categoryFilter, setCategoryFilter] = useState('all');
  const [statusFilter, setStatusFilter] = useState('all');

  const filteredStockData = stockBalanceData.filter(item => {
    const matchesSearch = item.itemName.toLowerCase().includes(stockSearchTerm.toLowerCase()) ||
                         item.itemCode.toLowerCase().includes(stockSearchTerm.toLowerCase()) ||
                         item.batch.toLowerCase().includes(stockSearchTerm.toLowerCase());
    const matchesCategory = categoryFilter === 'all' || item.category === categoryFilter;
    const matchesStatus = statusFilter === 'all' || item.status === statusFilter;
    
    return matchesSearch && matchesCategory && matchesStatus;
  });

  const categories = [...new Set(stockBalanceData.map(item => item.category))];

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'normal': return 'bg-green-100 text-green-800';
      case 'low': return 'bg-yellow-100 text-yellow-800';
      case 'critical': return 'bg-red-100 text-red-800';
      case 'overstock': return 'bg-blue-100 text-blue-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const kpis = {
    inStock: stockBalanceData.reduce((sum, item) => sum + item.currentStock, 0),
    reserved: stockBalanceData.reduce((sum, item) => sum + item.reservedQty, 0),
    available: stockBalanceData.reduce((sum, item) => sum + item.availableQty, 0),
    pendingIn: 850,
    pendingOut: 320
  };

  const handleQrScan = () => {
    // Mock QR scan result
    const mockScanResult = {
      item: 'Blue Pellet Grade A',
      batch: 'BATCH-2025-001',
      quantity: 500,
      from: 'RM-WH'
    };
    
    setSelectedItem(mockScanResult.item);
    setBatchJob(mockScanResult.batch);
    setQuantity(mockScanResult.quantity.toString());
    setFromWarehouse(mockScanResult.from);
    
    toast.success('QR Code scanned successfully');
  };

  const handleStockIn = () => {
    if (!selectedItem || !quantity) {
      toast.error('Please select item and quantity');
      return;
    }
    
    toast.success(`Stock In confirmed: ${quantity} units of ${selectedItem}`);
    // Reset form
    setSelectedItem('');
    setQuantity('');
    setBatchJob('');
    setNotes('');
  };

  const handleStockOut = () => {
    if (!selectedItem || !quantity || !toWarehouse || !reason) {
      toast.error('Please fill in all required fields');
      return;
    }
    
    // Mock route validation
    const hasRoute = Math.random() > 0.3; // 70% chance of valid route
    const autoApprove = Math.random() > 0.5; // 50% chance of auto-approve
    
    if (!hasRoute) {
      toast.error(`No route from ${warehouse.code} to ${toWarehouse}. Contact Admin.`);
      return;
    }
    
    if (autoApprove) {
      toast.success('Auto-approved; Destination notified.');
    } else {
      toast.warning('Approval required; sent to Production Manager.');
    }
    
    // Reset form
    setSelectedItem('');
    setQuantity('');
    setToWarehouse('');
    setReason('');
    setNotes('');
  };

  const handleTransfer = () => {
    if (!selectedItem || !quantity || !toWarehouse) {
      toast.error('Please fill in all required fields');
      return;
    }
    
    toast.success(`Transfer submitted: ${quantity} units of ${selectedItem} to ${toWarehouse}`);
    // Reset form
    setSelectedItem('');
    setQuantity('');
    setToWarehouse('');
    setNotes('');
  };

  const handleAdjustment = () => {
    if (!adjustmentData.item || !adjustmentData.quantity || !adjustmentData.reason || !adjustmentData.password) {
      toast.error('Please fill in all required fields');
      return;
    }
    
    if (adjustmentData.password !== 'supervisor123') {
      toast.error('Invalid supervisor password');
      return;
    }
    
    toast.success(`Stock adjustment confirmed: ${adjustmentData.type === 'add' ? '+' : '-'}${adjustmentData.quantity} ${adjustmentData.item}`);
    setShowAdjustmentModal(false);
    setAdjustmentData({
      item: '',
      quantity: '',
      type: 'add',
      reason: '',
      note: '',
      password: ''
    });
  };

  const validateRoute = (destination: string) => {
    // Mock route validation logic
    if (!destination) return null;
    
    const hasRoute = Math.random() > 0.2; // 80% chance of valid route
    const autoApprove = Math.random() > 0.4; // 60% chance of auto-approve
    
    if (!hasRoute) {
      return {
        type: 'error',
        message: `No route from ${warehouse.code} to ${destination}. Contact Admin.`
      };
    }
    
    if (autoApprove) {
      return {
        type: 'success',
        message: 'Auto-approved; Destination notified.'
      };
    }
    
    return {
      type: 'warning',
      message: 'Approval required; sent to Production Manager.'
    };
  };

  const routeValidation = validateRoute(toWarehouse);

  return (
    <div className="h-full bg-slate-50">
      <div className="h-full overflow-y-auto">
        <div className="p-6">
          {/* Breadcrumb & Header */}
          <div className="mb-6">
            <div className="flex items-center gap-2 text-sm text-slate-500 mb-4">
              <button onClick={onBack} className="hover:text-slate-700">Inventory</button>
              <span>/</span>
              <button onClick={onBack} className="hover:text-slate-700">Warehouse Dashboard</button>
              <span>/</span>
              <span className="text-slate-900">{warehouse.name}</span>
            </div>
            
            <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-4">
              <div className="flex items-center gap-4">
                <div>
                  <h1 className="text-2xl font-semibold text-slate-900">{warehouse.name}</h1>
                  <div className="flex items-center gap-3 mt-1">
                    <Badge className="bg-blue-100 text-blue-800">{warehouse.type}</Badge>
                    <span className="text-slate-600">{warehouse.code}</span>
                    <span className="text-slate-400">•</span>
                    <span className="text-slate-600">{warehouse.manager.name}</span>
                  </div>
                </div>
              </div>
              
              <div className="flex gap-2">
                <Button variant="outline" size="sm" onClick={() => setShowAdjustmentModal(true)}>
                  <Settings className="h-4 w-4 mr-2" />
                  Adjustment
                </Button>
                <Button variant="outline" size="sm" onClick={onBack}>
                  <LogOut className="h-4 w-4 mr-2" />
                  Exit
                </Button>
              </div>
            </div>
            
            {/* Live Stock Summary */}
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mt-4">
              <Card className="border-0 shadow-sm">
                <CardContent className="p-4 text-center">
                  <div className="text-lg font-semibold text-slate-900">{kpis.inStock.toLocaleString()}</div>
                  <div className="text-sm text-slate-600">Total Stock</div>
                </CardContent>
              </Card>
              <Card className="border-0 shadow-sm">
                <CardContent className="p-4 text-center">
                  <div className="text-lg font-semibold text-green-700">{kpis.available.toLocaleString()}</div>
                  <div className="text-sm text-slate-600">Available</div>
                </CardContent>
              </Card>
              <Card className="border-0 shadow-sm">
                <CardContent className="p-4 text-center">
                  <div className="text-lg font-semibold text-amber-700">{kpis.reserved.toLocaleString()}</div>
                  <div className="text-sm text-slate-600">Reserved</div>
                </CardContent>
              </Card>
              <Card className="border-0 shadow-sm">
                <CardContent className="p-4 text-center">
                  <div className="text-lg font-semibold text-blue-700">{kpis.pendingIn.toLocaleString()}</div>
                  <div className="text-sm text-slate-600">Pending In</div>
                </CardContent>
              </Card>
            </div>
          </div>

          {/* Main Tabs */}
          <Tabs value={activeTab} onValueChange={setActiveTab} className="space-y-6">
            <TabsList className="grid w-full grid-cols-4">
              <TabsTrigger value="stock-balance" className="flex items-center gap-2">
                <BarChart3 className="h-4 w-4" />
                STOCK BALANCE
              </TabsTrigger>
              <TabsTrigger value="stock-in" className="flex items-center gap-2">
                <ArrowDown className="h-4 w-4" />
                STOCK IN (Receive)
              </TabsTrigger>
              <TabsTrigger value="stock-out" className="flex items-center gap-2">
                <ArrowUp className="h-4 w-4" />
                STOCK OUT (Issue/Request)
              </TabsTrigger>
              <TabsTrigger value="transfer" className="flex items-center gap-2">
                <ArrowLeftRight className="h-4 w-4" />
                TRANSFER (Internal move)
              </TabsTrigger>
            </TabsList>

            {/* STOCK BALANCE Tab */}
            <TabsContent value="stock-balance" className="space-y-6">
              {/* Filters */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <BarChart3 className="h-5 w-5 text-blue-600" />
                    Stock Balance Filters
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                    <div>
                      <Label>Search Items</Label>
                      <div className="relative">
                        <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-slate-400" />
                        <Input
                          placeholder="Search by name, code, or batch..."
                          value={stockSearchTerm}
                          onChange={(e) => setStockSearchTerm(e.target.value)}
                          className="pl-10"
                        />
                      </div>
                    </div>
                    <div>
                      <Label>Category</Label>
                      <Select value={categoryFilter} onValueChange={setCategoryFilter}>
                        <SelectTrigger>
                          <SelectValue placeholder="All Categories" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="all">All Categories</SelectItem>
                          {categories.map(category => (
                            <SelectItem key={category} value={category}>{category}</SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                    <div>
                      <Label>Status</Label>
                      <Select value={statusFilter} onValueChange={setStatusFilter}>
                        <SelectTrigger>
                          <SelectValue placeholder="All Status" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="all">All Status</SelectItem>
                          <SelectItem value="normal">Normal</SelectItem>
                          <SelectItem value="low">Low Stock</SelectItem>
                          <SelectItem value="critical">Critical</SelectItem>
                          <SelectItem value="overstock">Overstock</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                    <div className="flex items-end">
                      <div className="text-sm text-slate-600">
                        Showing {filteredStockData.length} of {stockBalanceData.length} items
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Stock Balance Table */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Package className="h-5 w-5 text-green-600" />
                    Detailed Stock Balance - {warehouse.name}
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="overflow-x-auto">
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Item Code</TableHead>
                          <TableHead>Item Name</TableHead>
                          <TableHead>Category</TableHead>
                          <TableHead>Current Stock</TableHead>
                          <TableHead>Reserved</TableHead>
                          <TableHead>Available</TableHead>
                          <TableHead>Reorder Level</TableHead>
                          <TableHead>Batch/Job</TableHead>
                          <TableHead>Location</TableHead>
                          <TableHead>Last Movement</TableHead>
                          <TableHead>Status</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {filteredStockData.map((item) => (
                          <TableRow key={item.id} className="hover:bg-slate-50">
                            <TableCell className="font-mono text-sm">{item.itemCode}</TableCell>
                            <TableCell>
                              <div>
                                <div className="font-medium">{item.itemName}</div>
                                {item.expiryDate && (
                                  <div className="text-xs text-slate-500">
                                    Exp: {item.expiryDate}
                                  </div>
                                )}
                              </div>
                            </TableCell>
                            <TableCell>
                              <Badge variant="outline" className="text-xs">
                                {item.category}
                              </Badge>
                            </TableCell>
                            <TableCell>
                              <div className="text-right">
                                <div className="font-semibold">{item.currentStock.toLocaleString()}</div>
                                <div className="text-xs text-slate-500">{item.unit}</div>
                              </div>
                            </TableCell>
                            <TableCell>
                              <div className="text-right">
                                <div className="font-medium text-amber-700">{item.reservedQty.toLocaleString()}</div>
                                <div className="text-xs text-slate-500">{item.unit}</div>
                              </div>
                            </TableCell>
                            <TableCell>
                              <div className="text-right">
                                <div className="font-medium text-green-700">{item.availableQty.toLocaleString()}</div>
                                <div className="text-xs text-slate-500">{item.unit}</div>
                              </div>
                            </TableCell>
                            <TableCell>
                              <div className="text-right">
                                <div className="text-slate-600">{item.reorderLevel.toLocaleString()}</div>
                                <div className="text-xs text-slate-500">{item.unit}</div>
                              </div>
                            </TableCell>
                            <TableCell className="font-mono text-sm">{item.batch}</TableCell>
                            <TableCell className="font-mono text-sm">{item.location}</TableCell>
                            <TableCell className="text-sm text-slate-600">{item.lastMovement}</TableCell>
                            <TableCell>
                              <Badge className={getStatusColor(item.status)}>
                                {item.status === 'normal' ? '✅ Normal' :
                                 item.status === 'low' ? '⚠️ Low' :
                                 item.status === 'critical' ? '🚨 Critical' : '📦 Overstock'}
                              </Badge>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </div>
                  
                  {filteredStockData.length === 0 && (
                    <div className="text-center py-8 text-slate-500">
                      <Package className="h-12 w-12 mx-auto mb-4 text-slate-300" />
                      <p>No items found matching your criteria</p>
                    </div>
                  )}
                </CardContent>
              </Card>
            </TabsContent>

            {/* STOCK IN Tab */}
            <TabsContent value="stock-in" className="space-y-6">
              <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                {/* Stock In Form */}
                <div className="lg:col-span-2">
                  <Card>
                    <CardHeader>
                      <CardTitle className="flex items-center gap-2">
                        <Package className="h-5 w-5 text-green-600" />
                        Stock In Form
                      </CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-4">
                      {/* QR Scanner */}
                      <div className="flex gap-2">
                        <Input
                          placeholder="Scan QR code or search item..."
                          value={scanInput}
                          onChange={(e) => setScanInput(e.target.value)}
                          className="flex-1"
                        />
                        <Button onClick={handleQrScan} className="bg-green-600 hover:bg-green-700">
                          <QrCode className="h-4 w-4" />
                        </Button>
                        <Button variant="outline">
                          <Search className="h-4 w-4" />
                        </Button>
                      </div>

                      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div>
                          <Label>Item</Label>
                          <Select value={selectedItem} onValueChange={setSelectedItem}>
                            <SelectTrigger>
                              <SelectValue placeholder="Select item" />
                            </SelectTrigger>
                            <SelectContent>
                              {mockItems.map(item => (
                                <SelectItem key={item.id} value={item.name}>
                                  {item.name} ({item.code})
                                </SelectItem>
                              ))}
                            </SelectContent>
                          </Select>
                        </div>

                        <div>
                          <Label>Batch/Job (Optional)</Label>
                          <Input
                            value={batchJob}
                            onChange={(e) => setBatchJob(e.target.value)}
                            placeholder="Enter batch or job number"
                          />
                        </div>

                        <div>
                          <Label>Quantity + UOM</Label>
                          <Input
                            type="number"
                            value={quantity}
                            onChange={(e) => setQuantity(e.target.value)}
                            placeholder="Enter quantity"
                          />
                        </div>

                        <div>
                          <Label>From Warehouse</Label>
                          <Select value={fromWarehouse} onValueChange={setFromWarehouse}>
                            <SelectTrigger>
                              <SelectValue placeholder="Auto-filled if pending request" />
                            </SelectTrigger>
                            <SelectContent>
                              {mockWarehouses.map(wh => (
                                <SelectItem key={wh.id} value={wh.id}>
                                  {wh.name}
                                </SelectItem>
                              ))}
                            </SelectContent>
                          </Select>
                        </div>
                      </div>

                      <div>
                        <Label>Notes</Label>
                        <Textarea
                          value={notes}
                          onChange={(e) => setNotes(e.target.value)}
                          placeholder="Additional notes..."
                        />
                      </div>

                      <Button onClick={handleStockIn} className="w-full bg-green-600 hover:bg-green-700" size="lg">
                        <CheckCircle className="h-4 w-4 mr-2" />
                        Confirm Stock In
                      </Button>
                    </CardContent>
                  </Card>
                </div>

                {/* Pending Stock In Requests */}
                <div>
                  <Card>
                    <CardHeader>
                      <CardTitle className="flex items-center gap-2">
                        <Clock className="h-5 w-5 text-blue-600" />
                        Pending Stock In Requests
                      </CardTitle>
                    </CardHeader>
                    <CardContent>
                      <div className="space-y-3">
                        {mockPendingRequests.map(request => (
                          <div key={request.id} className="p-3 bg-slate-50 rounded-lg">
                            <div className="flex justify-between items-start mb-2">
                              <div className="text-sm font-medium">{request.item}</div>
                              <Badge className="bg-blue-100 text-blue-800 text-xs">
                                {request.id}
                              </Badge>
                            </div>
                            <div className="text-sm text-slate-600 space-y-1">
                              <div>Qty: {request.quantity} {request.unit}</div>
                              <div>From: {request.from}</div>
                              <div>By: {request.requestedBy} at {request.time}</div>
                            </div>
                            <div className="flex gap-2 mt-3">
                              <Button size="sm" className="flex-1 bg-green-600 hover:bg-green-700">
                                <CheckCircle className="h-3 w-3 mr-1" />
                                Approve
                              </Button>
                              <Button size="sm" variant="outline" className="flex-1">
                                <XCircle className="h-3 w-3 mr-1" />
                                Reject
                              </Button>
                            </div>
                          </div>
                        ))}
                      </div>
                    </CardContent>
                  </Card>
                </div>
              </div>
            </TabsContent>

            {/* STOCK OUT Tab */}
            <TabsContent value="stock-out" className="space-y-6">
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Stock Out Form */}
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <ArrowUp className="h-5 w-5 text-orange-600" />
                      Stock Out Form
                    </CardTitle>
                  </CardHeader>
                  <CardContent className="space-y-4">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div>
                        <Label>Item</Label>
                        <Select value={selectedItem} onValueChange={setSelectedItem}>
                          <SelectTrigger>
                            <SelectValue placeholder="Select item" />
                          </SelectTrigger>
                          <SelectContent>
                            {mockItems.map(item => (
                              <SelectItem key={item.id} value={item.name}>
                                {item.name} ({item.code})
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </div>

                      <div>
                        <Label>Quantity + UOM</Label>
                        <Input
                          type="number"
                          value={quantity}
                          onChange={(e) => setQuantity(e.target.value)}
                          placeholder="Enter quantity"
                        />
                      </div>

                      <div>
                        <Label>Destination Warehouse</Label>
                        <Select value={toWarehouse} onValueChange={setToWarehouse}>
                          <SelectTrigger>
                            <SelectValue placeholder="Select destination" />
                          </SelectTrigger>
                          <SelectContent>
                            {mockWarehouses.filter(wh => wh.id !== warehouse.id).map(wh => (
                              <SelectItem key={wh.id} value={wh.id}>
                                {wh.name}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </div>

                      <div>
                        <Label>Job/Batch (Optional)</Label>
                        <Input
                          value={batchJob}
                          onChange={(e) => setBatchJob(e.target.value)}
                          placeholder="Enter job or batch"
                        />
                      </div>

                      <div className="md:col-span-2">
                        <Label>Reason</Label>
                        <Select value={reason} onValueChange={setReason}>
                          <SelectTrigger>
                            <SelectValue placeholder="Select reason" />
                          </SelectTrigger>
                          <SelectContent>
                            {mockReasons.map(r => (
                              <SelectItem key={r} value={r}>{r}</SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </div>
                    </div>

                    <div>
                      <Label>Notes</Label>
                      <Textarea
                        value={notes}
                        onChange={(e) => setNotes(e.target.value)}
                        placeholder="Additional notes..."
                      />
                    </div>

                    {/* Route Validation */}
                    {routeValidation && (
                      <Alert className={
                        routeValidation.type === 'success' ? 'border-green-200 bg-green-50' :
                        routeValidation.type === 'warning' ? 'border-yellow-200 bg-yellow-50' :
                        'border-red-200 bg-red-50'
                      }>
                        <AlertCircle className={`h-4 w-4 ${
                          routeValidation.type === 'success' ? 'text-green-600' :
                          routeValidation.type === 'warning' ? 'text-yellow-600' :
                          'text-red-600'
                        }`} />
                        <AlertDescription className={
                          routeValidation.type === 'success' ? 'text-green-800' :
                          routeValidation.type === 'warning' ? 'text-yellow-800' :
                          'text-red-800'
                        }>
                          {routeValidation.message}
                        </AlertDescription>
                      </Alert>
                    )}

                    <Button 
                      onClick={handleStockOut} 
                      className="w-full bg-orange-600 hover:bg-orange-700" 
                      size="lg"
                      disabled={routeValidation?.type === 'error'}
                    >
                      <Send className="h-4 w-4 mr-2" />
                      Create Stock Out Request
                    </Button>
                  </CardContent>
                </Card>

                {/* Outgoing Requests */}
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <Eye className="h-5 w-5 text-purple-600" />
                      Outgoing Requests
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="space-y-3">
                      {mockOutgoingRequests.map(request => (
                        <div key={request.id} className="p-3 bg-slate-50 rounded-lg">
                          <div className="flex justify-between items-start mb-2">
                            <div className="text-sm font-medium">{request.item}</div>
                            <Badge className={
                              request.status === 'Auto-Approved' ? 'bg-green-100 text-green-800' :
                              'bg-yellow-100 text-yellow-800'
                            }>
                              {request.status}
                            </Badge>
                          </div>
                          <div className="text-sm text-slate-600 space-y-1">
                            <div>Qty: {request.quantity} {request.unit}</div>
                            <div>To: {request.destination}</div>
                            <div>Submitted: {request.submittedAt}</div>
                            {request.approver && (
                              <div>Approver: {request.approver}</div>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </CardContent>
                </Card>
              </div>
            </TabsContent>

            {/* TRANSFER Tab */}
            <TabsContent value="transfer" className="space-y-6">
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Transfer Form */}
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <ArrowLeftRight className="h-5 w-5 text-blue-600" />
                      Transfer Form
                    </CardTitle>
                  </CardHeader>
                  <CardContent className="space-y-4">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div>
                        <Label>From</Label>
                        <Input value={warehouse.name} disabled className="bg-slate-100" />
                      </div>

                      <div>
                        <Label>To</Label>
                        <Select value={toWarehouse} onValueChange={setToWarehouse}>
                          <SelectTrigger>
                            <SelectValue placeholder="Select destination" />
                          </SelectTrigger>
                          <SelectContent>
                            {mockWarehouses.filter(wh => wh.id !== warehouse.id).map(wh => (
                              <SelectItem key={wh.id} value={wh.id}>
                                {wh.name}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </div>

                      <div>
                        <Label>Item</Label>
                        <Select value={selectedItem} onValueChange={setSelectedItem}>
                          <SelectTrigger>
                            <SelectValue placeholder="Select item" />
                          </SelectTrigger>
                          <SelectContent>
                            {mockItems.map(item => (
                              <SelectItem key={item.id} value={item.name}>
                                {item.name} ({item.code})
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </div>

                      <div>
                        <Label>Quantity + UOM</Label>
                        <Input
                          type="number"
                          value={quantity}
                          onChange={(e) => setQuantity(e.target.value)}
                          placeholder="Enter quantity"
                        />
                      </div>
                    </div>

                    <div>
                      <Label>Notes</Label>
                      <Textarea
                        value={notes}
                        onChange={(e) => setNotes(e.target.value)}
                        placeholder="Transfer notes..."
                      />
                    </div>

                    {/* Route Validation */}
                    {routeValidation && (
                      <Alert className={
                        routeValidation.type === 'success' ? 'border-green-200 bg-green-50' :
                        routeValidation.type === 'warning' ? 'border-yellow-200 bg-yellow-50' :
                        'border-red-200 bg-red-50'
                      }>
                        <AlertCircle className={`h-4 w-4 ${
                          routeValidation.type === 'success' ? 'text-green-600' :
                          routeValidation.type === 'warning' ? 'text-yellow-600' :
                          'text-red-600'
                        }`} />
                        <AlertDescription className={
                          routeValidation.type === 'success' ? 'text-green-800' :
                          routeValidation.type === 'warning' ? 'text-yellow-800' :
                          'text-red-800'
                        }>
                          {routeValidation.message}
                        </AlertDescription>
                      </Alert>
                    )}

                    <Button 
                      onClick={handleTransfer} 
                      className="w-full bg-blue-600 hover:bg-blue-700" 
                      size="lg"
                      disabled={routeValidation?.type === 'error'}
                    >
                      <Send className="h-4 w-4 mr-2" />
                      Submit Transfer
                    </Button>
                  </CardContent>
                </Card>

                {/* Recent Movements Timeline */}
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <Clock className="h-5 w-5 text-slate-600" />
                      Recent Movements (Today)
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="space-y-3">
                      {mockRecentMovements.map(movement => (
                        <div key={movement.id} className="flex items-start gap-3 p-3 bg-slate-50 rounded-lg">
                          <div className={`p-1 rounded-full ${
                            movement.type === 'Stock In' ? 'bg-green-100' :
                            movement.type === 'Stock Out' ? 'bg-orange-100' :
                            'bg-blue-100'
                          }`}>
                            {movement.type === 'Stock In' ? (
                              <ArrowDown className="h-3 w-3 text-green-600" />
                            ) : movement.type === 'Stock Out' ? (
                              <ArrowUp className="h-3 w-3 text-orange-600" />
                            ) : (
                              <ArrowLeftRight className="h-3 w-3 text-blue-600" />
                            )}
                          </div>
                          <div className="flex-1">
                            <div className="text-sm font-medium">{movement.type}</div>
                            <div className="text-sm text-slate-600">{movement.item}</div>
                            <div className="text-xs text-slate-500">
                              {movement.quantity} units • {movement.user} • {movement.time}
                              {movement.from && ` • From: ${movement.from}`}
                              {movement.to && ` • To: ${movement.to}`}
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  </CardContent>
                </Card>
              </div>
            </TabsContent>
          </Tabs>

          {/* Audit Footer */}
          <Card className="mt-6">
            <CardHeader>
              <CardTitle className="text-sm">Audit Trail - Last 10 Actions</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-xs text-slate-500 space-y-1">
                <div>2025-09-25 11:45:23 - Ko Aung - Stock In: Blue Pellet Grade A (500kg)</div>
                <div>2025-09-25 11:30:15 - Ma Su - Stock Out Request: Red Pellet Grade B (200kg) to PROD-WH</div>
                <div>2025-09-25 11:15:08 - Ko Win - Transfer: Plastic Container 1L (150pcs) to FG-WH</div>
                <div>2025-09-25 10:45:32 - Admin - Stock Adjustment: +50kg Blue Pellet Grade A (Correction)</div>
                <div>2025-09-25 10:30:18 - Ma Thin - Stock In: Green Pellet Premium (300kg)</div>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>

      {/* Adjustment Modal */}
      <Dialog open={showAdjustmentModal} onOpenChange={setShowAdjustmentModal}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <Settings className="h-5 w-5" />
              Stock Adjustment (Supervisor Only)
            </DialogTitle>
            <DialogDescription>
              Make stock adjustments for inventory corrections. Supervisor credentials required.
            </DialogDescription>
          </DialogHeader>
          
          <div className="space-y-4">
            <div>
              <Label>Item</Label>
              <Select value={adjustmentData.item} onValueChange={(value) => 
                setAdjustmentData(prev => ({ ...prev, item: value }))
              }>
                <SelectTrigger>
                  <SelectValue placeholder="Select item" />
                </SelectTrigger>
                <SelectContent>
                  {mockItems.map(item => (
                    <SelectItem key={item.id} value={item.name}>
                      {item.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label>Type</Label>
                <Select value={adjustmentData.type} onValueChange={(value) => 
                  setAdjustmentData(prev => ({ ...prev, type: value }))
                }>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="add">Add (+)</SelectItem>
                    <SelectItem value="subtract">Subtract (-)</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label>Quantity</Label>
                <Input
                  type="number"
                  value={adjustmentData.quantity}
                  onChange={(e) => setAdjustmentData(prev => ({ ...prev, quantity: e.target.value }))}
                  placeholder="Enter quantity"
                />
              </div>
            </div>

            <div>
              <Label>Reason Code</Label>
              <Select value={adjustmentData.reason} onValueChange={(value) => 
                setAdjustmentData(prev => ({ ...prev, reason: value }))
              }>
                <SelectTrigger>
                  <SelectValue placeholder="Select reason" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="correction">Stock Correction</SelectItem>
                  <SelectItem value="damage">Damaged Goods</SelectItem>
                  <SelectItem value="expired">Expired Items</SelectItem>
                  <SelectItem value="found">Found Stock</SelectItem>
                  <SelectItem value="lost">Lost/Missing</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label>Note</Label>
              <Textarea
                value={adjustmentData.note}
                onChange={(e) => setAdjustmentData(prev => ({ ...prev, note: e.target.value }))}
                placeholder="Additional notes..."
              />
            </div>

            <div>
              <Label>Supervisor Password</Label>
              <Input
                type="password"
                value={adjustmentData.password}
                onChange={(e) => setAdjustmentData(prev => ({ ...prev, password: e.target.value }))}
                placeholder="Enter supervisor password"
              />
              <div className="text-xs text-slate-500 mt-1">
                Demo password: supervisor123
              </div>
            </div>

            <div className="flex gap-3 pt-4">
              <Button onClick={handleAdjustment} className="flex-1" size="lg">
                <CheckCircle className="h-4 w-4 mr-2" />
                Confirm Adjustment
              </Button>
              <Button 
                variant="outline" 
                onClick={() => setShowAdjustmentModal(false)}
                size="lg"
              >
                Cancel
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}