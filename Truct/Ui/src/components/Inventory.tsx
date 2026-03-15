import React, { useState } from 'react';
import { MovementConfirmationModal } from './MovementConfirmationModal';
import { toast } from 'sonner';
import {
  BarChart3,
  FileText,
  Package,
  QrCode,
  ArrowRightLeft,
  AlertTriangle,
  RefreshCw,
  UserCheck,
  Truck,
  Building2,
  Search,
  Filter,
  Download,
  Eye,
  Edit,
  Plus,
  Minus,
  CheckCircle,
  XCircle,
  Clock,
  User,
  Calendar,
  Scan,
  Weight,
  MoreHorizontal,
  Settings,
  Trash2,
  Send,
  Save,
  Check,
  X,
  Camera,
  Printer,
  History,
  TrendingUp,
  TrendingDown,
  Activity,
  MapPin,
  Info,
  ExternalLink,
  Shuffle,
  HandCoins,
  PackageOpen,
  Clipboard,
  Scale,
  Timer,
  Users
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Label } from './ui/label';
import { Textarea } from './ui/textarea';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Switch } from './ui/switch';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { Checkbox } from './ui/checkbox';
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger } from './ui/alert-dialog';

interface InventoryProps {
  currentPage: string;
  onPageChange: (page: string) => void;
}

export function Inventory({ currentPage, onPageChange }: InventoryProps) {
  const [selectedWarehouse, setSelectedWarehouse] = useState<string>('all');
  const [unitToggle, setUnitToggle] = useState<{ [key: string]: 'kg' | 'pcs' }>({});
  const [showQRDialog, setShowQRDialog] = useState(false);
  const [showPermissionsDialog, setShowPermissionsDialog] = useState(false);
  const [showBorrowDialog, setShowBorrowDialog] = useState(false);
  const [showReceivingDialog, setShowReceivingDialog] = useState(false);
  const [showTransferDialog, setShowTransferDialog] = useState(false);
  const [selectedTransferFrom, setSelectedTransferFrom] = useState<string>('');
  const [showAdjustDialog, setShowAdjustDialog] = useState(false);
  
  // Movement confirmation modal states
  const [showConfirmationModal, setShowConfirmationModal] = useState(false);
  const [confirmationModalConfig, setConfirmationModalConfig] = useState({
    title: '',
    titleMM: '',
    onConfirm: () => {}
  });
  
  // Finished Goods Receiving state (moved from inside render function)
  const [searchFilters, setSearchFilters] = useState({
    releaseId: '',
    jobId: '',
    productName: '',
    qcStatus: 'all',
    dateFrom: '2025-09-01',
    dateTo: '2025-09-07'
  });
  const [showAcceptDialog, setShowAcceptDialog] = useState(false);
  const [showLabelDialog, setShowLabelDialog] = useState(false);
  const [showPartialAcceptDialog, setShowPartialAcceptDialog] = useState(false);
  const [showRejectDialog, setShowRejectDialog] = useState(false);
  const [selectedBatch, setSelectedBatch] = useState<any>(null);
  const [partialAcceptQty, setPartialAcceptQty] = useState('');
  const [rejectReason, setRejectReason] = useState('');
  const [selectedLabelType, setSelectedLabelType] = useState('QR');

  // Mock data for warehouses with color coding and transfer rules
  const mockWarehouses = [
    { 
      id: 'RM-WH', 
      name: 'Raw Material Warehouse', 
      nameMM: 'ကုန်ကြမ်းဂိုဒေါင်', 
      type: 'Physical', 
      color: '🟦', 
      stock: '2,500 kg', 
      status: 'in-stock', 
      capacity: '85%',
      inputMethod: 'Manual',
      allowedTo: ['PROD-WH', 'GLUE-WH'],
      description: 'Raw materials storage'
    },
    { 
      id: 'GLUE-WH', 
      name: 'Glue Warehouse', 
      nameMM: 'ကော်စာ/ကော်ခဲဂိုဒေါင်', 
      type: 'Physical', 
      color: '🟩', 
      stock: '450 kg', 
      status: 'low-stock', 
      capacity: '25%',
      inputMethod: 'Manual',
      allowedTo: ['PROD-WH'],
      description: 'Adhesives and glue materials'
    },
    { 
      id: 'PROD-WH', 
      name: 'Production Warehouse', 
      nameMM: 'ထုတ်လုပ်မှုဂိုဒေါင်', 
      type: 'Virtual', 
      color: '🟨', 
      stock: '1,200 pcs', 
      status: 'in-progress', 
      capacity: '60%',
      inputMethod: 'QR/RFID',
      allowedTo: ['FG-WH', 'CUT-WH', 'RM-WH'],
      description: 'Production line storage'
    },
    { 
      id: 'FG-WH', 
      name: 'Finished Goods Warehouse', 
      nameMM: 'ကုန်ချောဂိုဒေါင်', 
      type: 'Physical', 
      color: '🟧', 
      stock: '800 pcs', 
      status: 'in-stock', 
      capacity: '40%',
      inputMethod: 'QR/RFID',
      allowedTo: ['HQ-DISPATCH', 'PACK-WH'],
      description: 'Finished products storage'
    },
    { 
      id: 'PACK-WH', 
      name: 'Packing Warehouse', 
      nameMM: 'ထုပ်ပိုးဌာနဂိုဒေါင်', 
      type: 'Physical', 
      color: '🟫', 
      stock: '320 pcs', 
      status: 'in-stock', 
      capacity: '55%',
      inputMethod: 'Manual',
      allowedTo: ['FG-WH'],
      description: 'Packaging operations'
    },
    { 
      id: 'CUT-WH', 
      name: 'Cutting Warehouse', 
      nameMM: 'ခုတ်စက်ဂိုဒေါင်', 
      type: 'Physical', 
      color: '🟥', 
      stock: '150 kg', 
      status: 'low-stock', 
      capacity: '15%',
      inputMethod: 'Manual',
      allowedTo: ['CUTPIECE-WH', 'SCRAP'],
      description: 'Defect cutting operations'
    },
    { 
      id: 'CUTPIECE-WH', 
      name: 'Cut Piece Warehouse', 
      nameMM: 'ခုတ်ဖတ်ဂိုဒေါင်', 
      type: 'Physical', 
      color: '🟪', 
      stock: '320 pcs', 
      status: 'in-stock', 
      capacity: '55%',
      inputMethod: 'Manual',
      allowedTo: ['EXT-WH', 'PROD-WH'],
      description: 'Cut pieces for reprocessing'
    },
    { 
      id: 'EXT-WH', 
      name: 'Extrusion Warehouse', 
      nameMM: 'လွန်ချစက်ဂိုဒေါင်', 
      type: 'Physical', 
      color: '🟫', 
      stock: '90 kg', 
      status: 'out-of-stock', 
      capacity: '5%',
      inputMethod: 'Manual',
      allowedTo: ['PROD-WH', 'HQ-DISPATCH'],
      description: 'Extrusion processing'
    },
    { 
      id: 'HQ-DISPATCH', 
      name: 'HQ Dispatch Warehouse', 
      nameMM: 'ရုံးချုပ်ပို့ဆောင်ရေးဂိုဒေါင်', 
      type: 'Virtual', 
      color: '🟩', 
      stock: '500 pcs', 
      status: 'in-stock', 
      capacity: '75%',
      inputMethod: 'QR/RFID',
      allowedTo: [],
      description: 'Final dispatch point'
    }
  ];

  // Mock borrow data
  const mockBorrowRequests = [
    {
      id: 'BRW-2025-001',
      sourceJob: 'JOB20250907-001-C1',
      sourceWarehouse: 'RM-WH',
      destJob: 'JOB20250907-003-C2', 
      destWarehouse: 'PROD-WH',
      item: 'PP Granules',
      quantity: '50 kg',
      reason: 'Urgent production requirement',
      expectedReturn: '2025-09-10',
      status: 'pending',
      requestedBy: 'OP001',
      requestDate: '2025-09-07 14:30',
      approver: null
    },
    {
      id: 'BRW-2025-002',
      sourceJob: 'JOB20250907-002-C3',
      sourceWarehouse: 'FG-WH',
      destJob: 'JOB20250907-004-C1',
      destWarehouse: 'PACK-WH',
      item: 'Plastic Container 1L',
      quantity: '25 pcs',
      reason: 'Packaging line shortage',
      expectedReturn: '2025-09-08',
      status: 'approved',
      requestedBy: 'OP002',
      requestDate: '2025-09-07 13:15',
      approver: 'SUP001'
    },
    {
      id: 'BRW-2025-003',
      sourceJob: 'JOB20250907-001-C1',
      sourceWarehouse: 'PROD-WH',
      destJob: 'JOB20250907-005-C2',
      destWarehouse: 'CUT-WH',
      item: 'Defective Bottles',
      quantity: '15 pcs',
      reason: 'QC rejection for reprocessing',
      expectedReturn: '2025-09-09',
      status: 'completed',
      requestedBy: 'QC001',
      requestDate: '2025-09-06 16:45',
      approver: 'SUP002'
    }
  ];

  // Mock receiving queue
  const mockReceivingQueue = [
    {
      id: 'RQ-2025-001',
      jobId: 'JOB20250907-001-C1',
      product: '2011 - Plastic Bottle 500ml',
      quantity: '100 pcs',
      machineNo: 'INJ-001',
      operator: 'OP001 - John Doe',
      qcStatus: 'approved',
      releaseTime: '2025-09-07 14:30',
      qrCode: 'QR-2025090701-001'
    },
    {
      id: 'RQ-2025-002',
      jobId: 'JOB20250907-002-C2',
      product: '2012 - Plastic Container 1L',
      quantity: '75 pcs',
      machineNo: 'INJ-002',
      operator: 'OP002 - Jane Smith',
      qcStatus: 'pending',
      releaseTime: '2025-09-07 15:15',
      qrCode: 'QR-2025090701-002'
    }
  ];

  const mockTransactions = [
    {
      id: 'TXN-20250907-001',
      timestamp: '2025-09-07 14:30',
      type: 'Stock In',
      fromWarehouse: 'Supplier',
      toWarehouse: 'RM-WH',
      item: 'PP Granules',
      quantity: '500 kg',
      operator: 'Staff001',
      status: 'completed'
    },
    {
      id: 'TXN-20250907-002',
      timestamp: '2025-09-07 15:15',
      type: 'Transfer',
      fromWarehouse: 'PROD-WH',
      toWarehouse: 'FG-WH',
      item: 'Plastic Bottle 500ml',
      quantity: '200 pcs',
      operator: 'Staff002',
      status: 'in-progress'
    },
    {
      id: 'TXN-20250907-003',
      timestamp: '2025-09-07 16:00',
      type: 'Dispatch',
      fromWarehouse: 'HQ-DISPATCH',
      toWarehouse: 'Customer',
      item: 'Plastic Container 1L',
      quantity: '150 pcs',
      operator: 'Staff003',
      status: 'completed'
    }
  ];

  const mockUserRoles = [
    { role: 'Operator', receive: true, transfer: false, borrow: true, adjust: false, printQR: false, viewReports: true },
    { role: 'Inventory Staff', receive: true, transfer: true, borrow: true, adjust: false, printQR: true, viewReports: true },
    { role: 'Supervisor', receive: true, transfer: true, borrow: true, adjust: true, printQR: true, viewReports: true },
    { role: 'HQ Staff', receive: false, transfer: false, borrow: false, adjust: false, printQR: false, viewReports: true }
  ];

  const getStatusBadge = (status: string) => {
    const statusConfig: any = {
      'in-stock': { color: 'bg-green-100 text-green-800', label: '✅ In Stock | ရှိနေ', icon: '✅' },
      'low-stock': { color: 'bg-yellow-100 text-yellow-800', label: '⚠ Low Stock | အနည်းငယ်သာရှိ', icon: '⚠' },
      'out-of-stock': { color: 'bg-red-100 text-red-800', label: '❌ Out of Stock | ကုန်နေ', icon: '❌' },
      'in-progress': { color: 'bg-blue-100 text-blue-800', label: 'In Progress | လုပ်ဆောင်နေ', icon: null },
      'completed': { color: 'bg-green-100 text-green-800', label: 'Completed | ပြီးစီး', icon: null },
      'pending': { color: 'bg-yellow-100 text-yellow-800', label: 'Pending | စောင့်ဆိုင်းနေ', icon: null },
      'approved': { color: 'bg-green-100 text-green-800', label: 'Approved | ခွင့်ပြုထား', icon: null },
      'rejected': { color: 'bg-red-100 text-red-800', label: 'Rejected | ပယ်ချထား', icon: null },
      'overdue': { color: 'bg-red-100 text-red-800', label: 'Overdue | သတ်မှတ်ရက်လွန်', icon: null }
    };
    const config = statusConfig[status] || { color: 'bg-slate-100 text-slate-800', label: status, icon: null };
    return <Badge className={config.color}>{config.label}</Badge>;
  };

  const toggleUnit = (itemId: string) => {
    setUnitToggle(prev => ({
      ...prev,
      [itemId]: prev[itemId] === 'kg' ? 'pcs' : 'kg'
    }));
  };

  // Get allowed destination warehouses based on source warehouse
  const getAllowedDestinations = (sourceWarehouse: string) => {
    const warehouse = mockWarehouses.find(wh => wh.id === sourceWarehouse);
    return warehouse?.allowedTo || [];
  };

  // Helper function to show confirmation modal
  const showMovementConfirmation = (title: string, titleMM: string, onConfirm: (reason?: string) => void) => {
    setConfirmationModalConfig({ title, titleMM, onConfirm });
    setShowConfirmationModal(true);
  };

  // Stock In confirmation handler
  const handleStockInConfirm = (reason?: string) => {
    // Simulate stock in operation
    console.log('Stock In confirmed', reason);
    toast.success('Stock In completed successfully! | စတော့ခ်ဝင်မှု အောင်မြင်စွာ ပြီးစီးပါပြီ။');
  };

  // Stock Out confirmation handler
  const handleStockOutConfirm = (reason?: string) => {
    // Simulate stock out operation
    console.log('Stock Out confirmed', reason);
    toast.success('Stock Out completed successfully! | စတော့ခ်ထွက်မှု အောင်မြင်စွာ ပြီးစီးပါပြီ။');
  };

  // Transfer confirmation handler
  const handleTransferConfirm = (reason?: string) => {
    // Simulate transfer operation
    console.log('Transfer confirmed', reason);
    toast.success('Transfer completed successfully! | လွှဲပြောင်းမှု အောင်မြင်စွာ ပြီးစီးပါပြီ။');
    setShowTransferDialog(false);
  };

  // Finished Goods Accept confirmation handler
  const handleFGAcceptConfirm = (reason?: string) => {
    console.log('Accepting batch to FG-WH:', selectedBatch, reason);
    toast.success('Batch accepted to FG Warehouse successfully! | အုပ်စုကို FG ဂိုဒေါင်သို့ အောင်မြင်စွာ လက်ခံပါပြီ။');
    setShowAcceptDialog(false);
  };

  // Partial Accept confirmation handler
  const handlePartialAcceptConfirm = (reason?: string) => {
    console.log('Partial accepting:', partialAcceptQty, 'from batch:', selectedBatch, reason);
    toast.success(`Partially accepted ${partialAcceptQty} pcs successfully! | ${partialAcceptQty} ခု တစ်စိတ်တစ်ပိုင်း အောင်မြင်စွာ လက်ခံပါပြီ။`);
    setShowPartialAcceptDialog(false);
    setPartialAcceptQty('');
  };

  const renderInventoryDashboard = () => (
    <div className="space-y-6">
      {/* Quick Action Buttons */}
      <div className="grid grid-cols-2 md:grid-cols-5 gap-4">
        <Button size="lg" className="h-16 bg-green-600 hover:bg-green-700" onClick={() => setShowReceivingDialog(true)}>
          <Package className="h-6 w-6 mr-2" />
          <div className="text-left">
            <div>Receive</div>
            <div className="text-xs opacity-80">လက်ခံ</div>
          </div>
        </Button>
        <Button size="lg" className="h-16 bg-blue-600 hover:bg-blue-700" onClick={() => setShowTransferDialog(true)}>
          <ArrowRightLeft className="h-6 w-6 mr-2" />
          <div className="text-left">
            <div>Transfer</div>
            <div className="text-xs opacity-80">လွှဲပြောင်း</div>
          </div>
        </Button>
        <Button size="lg" className="h-16 bg-orange-600 hover:bg-orange-700" onClick={() => onPageChange('hq-dispatch')}>
          <Truck className="h-6 w-6 mr-2" />
          <div className="text-left">
            <div>Dispatch</div>
            <div className="text-xs opacity-80">ပို့ဆောင်</div>
          </div>
        </Button>
        <Button size="lg" className="h-16 bg-purple-600 hover:bg-purple-700" onClick={() => setShowBorrowDialog(true)}>
          <HandCoins className="h-6 w-6 mr-2" />
          <div className="text-left">
            <div>Borrow</div>
            <div className="text-xs opacity-80">ငှားယူ</div>
          </div>
        </Button>
        <Button size="lg" className="h-16 bg-indigo-600 hover:bg-indigo-700" onClick={() => onPageChange('inventory-reports')}>
          <FileText className="h-6 w-6 mr-2" />
          <div className="text-left">
            <div>Reports</div>
            <div className="text-xs opacity-80">အစီရင်ခံစာ</div>
          </div>
        </Button>
      </div>

      {/* Search and Filter */}
      <div className="flex gap-4">
        <div className="flex-1">
          <div className="relative">
            <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
            <Input placeholder="Search by item code, job ID, or product name... | ကုန်ပစ္စည်းကုတ်၊ အလုပ်အမှတ်၊ သို့မဟုတ် ထုတ်ကုန်အမည်ဖြင့် ရှာဖွေရန်..." className="pl-10" />
          </div>
        </div>
        <Select value={selectedWarehouse} onValueChange={setSelectedWarehouse}>
          <SelectTrigger className="w-48">
            <SelectValue placeholder="Filter warehouse" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All Warehouses | အားလုံး</SelectItem>
            {mockWarehouses.map(wh => (
              <SelectItem key={wh.id} value={wh.id}>
                {wh.color} {wh.id}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
        <Button variant="outline">
          <Filter className="h-4 w-4 mr-2" />
          More Filters | နောက်ထပ်စန်
        </Button>
      </div>

      {/* Warehouse Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {mockWarehouses.map((warehouse) => (
          <Card key={warehouse.id} className="hover:shadow-md transition-shadow cursor-pointer">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium flex items-center gap-2">
                <span className="text-lg">{warehouse.color}</span>
                {warehouse.id}
              </CardTitle>
              <Package className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-lg font-bold mb-1">{warehouse.stock}</div>
              <div className="text-sm text-muted-foreground mb-1">{warehouse.name}</div>
              <div className="text-xs text-slate-500 mb-3">{warehouse.nameMM}</div>
              <div className="flex items-center justify-between mb-2">
                {getStatusBadge(warehouse.status)}
                <span className="text-sm text-muted-foreground">{warehouse.capacity}</span>
              </div>
              <Progress value={parseInt(warehouse.capacity)} className="h-2 mb-2" />
              <div className="text-xs text-muted-foreground mb-2">
                Allowed to: {warehouse.allowedTo.length > 0 ? warehouse.allowedTo.join(', ') : 'Final destination'}
              </div>
              <div className="flex items-center justify-between text-xs text-muted-foreground">
                <span>{warehouse.type}</span>
                <span>{warehouse.inputMethod}</span>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Live Activity Feed */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Recent Transactions */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <Activity className="h-5 w-5" />
              Recent Transactions | လတ်တလောလုပ်ဆောင်ချက်များ
            </CardTitle>
            <Button variant="outline" size="sm" onClick={() => onPageChange('stock-transactions')}>
              <Eye className="h-4 w-4 mr-2" />
              View All
            </Button>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {mockTransactions.slice(0, 3).map((txn, index) => (
                <div key={index} className="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
                  <div className="flex items-center gap-3">
                    <div className={`w-2 h-2 rounded-full ${
                      txn.status === 'completed' ? 'bg-green-500' : 
                      txn.status === 'in-progress' ? 'bg-blue-500' : 'bg-yellow-500'
                    }`}></div>
                    <div>
                      <div className="font-medium">{txn.type}: {txn.item}</div>
                      <div className="text-sm text-muted-foreground">
                        {txn.fromWarehouse} → {txn.toWarehouse} | {txn.quantity}
                      </div>
                    </div>
                  </div>
                  <div className="text-right">
                    <div className="text-sm">{txn.timestamp}</div>
                    <div className="text-xs text-muted-foreground">{txn.operator}</div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Pending Receiving Queue */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <PackageOpen className="h-5 w-5" />
              Receiving Queue | လက်ခံရန်စောင့်နေသည်
            </CardTitle>
            <Button variant="outline" size="sm" onClick={() => onPageChange('finished-goods-receiving')}>
              <Eye className="h-4 w-4 mr-2" />
              View All
            </Button>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {mockReceivingQueue.map((item, index) => (
                <div key={index} className="flex items-center justify-between p-3 border rounded-lg">
                  <div>
                    <div className="font-medium">{item.product}</div>
                    <div className="text-sm text-muted-foreground">
                      {item.jobId} | {item.quantity} | {item.machineNo}
                    </div>
                    <div className="text-xs text-slate-500">{item.operator}</div>
                  </div>
                  <div className="text-right">
                    <div>{getStatusBadge(item.qcStatus)}</div>
                    <div className="text-xs text-muted-foreground mt-1">{item.releaseTime}</div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );

  const renderWarehouseControl = () => (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h2 className="text-lg font-semibold">Warehouse Control | ကုန်လှောင်ရုံထိန်းချုပ်မှု</h2>
        <div className="flex gap-2">
          <Select value={selectedWarehouse} onValueChange={setSelectedWarehouse}>
            <SelectTrigger className="w-64">
              <SelectValue placeholder="Select warehouse to manage" />
            </SelectTrigger>
            <SelectContent>
              {mockWarehouses.map(wh => (
                <SelectItem key={wh.id} value={wh.id}>
                  {wh.color} {wh.id} - {wh.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      {selectedWarehouse && selectedWarehouse !== 'all' && (
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Building2 className="h-5 w-5" />
              {selectedWarehouse} Control Panel
            </CardTitle>
          </CardHeader>
          <CardContent>
            <Tabs defaultValue="stock-in-out">
              <TabsList className="grid w-full grid-cols-3">
                <TabsTrigger value="stock-in-out">Stock In/Out | ဝင်/ထွက်</TabsTrigger>
                <TabsTrigger value="current-stock">Current Stock | လက်ရှိစတော့ခ်</TabsTrigger>
                <TabsTrigger value="transfer-rules">Transfer Rules | လွှဲပြောင်းစည်းမျဉ်း</TabsTrigger>
              </TabsList>

              <TabsContent value="stock-in-out" className="space-y-4">
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  {/* Stock In Form */}
                  <Card>
                    <CardHeader>
                      <CardTitle className="text-base flex items-center gap-2">
                        <Plus className="h-4 w-4" />
                        Stock In | စတော့ခ်ဝင်
                      </CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-4">
                      <div>
                        <Label>Item Code/Name</Label>
                        <div className="flex gap-2">
                          <Input placeholder="Enter or scan item..." />
                          <Button variant="outline" size="icon">
                            <QrCode className="h-4 w-4" />
                          </Button>
                        </div>
                      </div>
                      <div className="grid grid-cols-2 gap-4">
                        <div>
                          <Label>Quantity</Label>
                          <Input type="number" placeholder="0" />
                        </div>
                        <div>
                          <Label>Unit</Label>
                          <Select>
                            <SelectTrigger>
                              <SelectValue placeholder="Select unit" />
                            </SelectTrigger>
                            <SelectContent>
                              <SelectItem value="kg">kg</SelectItem>
                              <SelectItem value="pcs">pcs</SelectItem>
                              <SelectItem value="lb">lb</SelectItem>
                            </SelectContent>
                          </Select>
                        </div>
                      </div>
                      <div>
                        <Label>Source</Label>
                        <Input placeholder="e.g., Supplier, Production Line..." />
                      </div>
                      <div className="space-y-2">
                        <Button 
                          className="w-full bg-green-600 hover:bg-green-700"
                          onClick={() => showMovementConfirmation(
                            'Confirm Receipt',
                            'လက်ခံအတည်ပြု',
                            handleStockInConfirm
                          )}
                        >
                          <Plus className="h-4 w-4 mr-2" />
                          Add to Stock
                        </Button>
                        <p className="text-xs text-center text-slate-500">
                          Re-authentication required. | အတည်ပြုရန် စကားဝှက်လိုအပ်သည်။
                        </p>
                      </div>
                    </CardContent>
                  </Card>

                  {/* Stock Out Form */}
                  <Card>
                    <CardHeader>
                      <CardTitle className="text-base flex items-center gap-2">
                        <Minus className="h-4 w-4" />
                        Stock Out | စတော့ခ်ထွက်
                      </CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-4">
                      <div>
                        <Label>Item Code/Name</Label>
                        <div className="flex gap-2">
                          <Input placeholder="Enter or scan item..." />
                          <Button variant="outline" size="icon">
                            <QrCode className="h-4 w-4" />
                          </Button>
                        </div>
                      </div>
                      <div className="grid grid-cols-2 gap-4">
                        <div>
                          <Label>Quantity</Label>
                          <Input type="number" placeholder="0" />
                        </div>
                        <div>
                          <Label>Unit</Label>
                          <Button variant="outline" onClick={() => toggleUnit('stockout')}>
                            {unitToggle['stockout'] || 'pcs'}
                          </Button>
                        </div>
                      </div>
                      <div>
                        <Label>Destination</Label>
                        <Select>
                          <SelectTrigger>
                            <SelectValue placeholder="Select destination" />
                          </SelectTrigger>
                          <SelectContent>
                            {selectedWarehouse && getAllowedDestinations(selectedWarehouse).map(dest => (
                              <SelectItem key={dest} value={dest}>
                                {mockWarehouses.find(w => w.id === dest)?.color} {dest}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </div>
                      <div>
                        <Label>Editable Weight (Optional) | တိုးတက်နိုင်သောအလေးချိန်</Label>
                        <div className="flex gap-2">
                          <Input 
                            type="number" 
                            placeholder="Enter actual weight..." 
                            step="0.01"
                            className="bg-yellow-50 border-yellow-200"
                          />
                          <span className="flex items-center px-3 text-sm text-muted-foreground bg-slate-50 rounded border">kg</span>
                        </div>
                        <div className="text-xs text-yellow-600 mt-1">
                          ⚠ For accurate inventory tracking
                        </div>
                      </div>
                      <div className="space-y-2">
                        <Button 
                          className="w-full bg-green-600 hover:bg-green-700"
                          onClick={() => showMovementConfirmation(
                            'Confirm Issue',
                            'ထုတ်မည်အတည်ပြု',
                            handleStockOutConfirm
                          )}
                        >
                          <ArrowRightLeft className="h-4 w-4 mr-2" />
                          Confirm Issue | ထုတ်မည်အတည်ပြု
                        </Button>
                        <p className="text-xs text-center text-slate-500">
                          Re-authentication required. | အတည်ပြုရန် စကားဝှက်လိုအပ်သည်။
                        </p>
                      </div>
                    </CardContent>
                  </Card>
                </div>

                {/* Transaction Log */}
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <History className="h-5 w-5" />
                      Transaction Log for {selectedWarehouse}
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="overflow-hidden rounded-lg border">
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>Time</TableHead>
                            <TableHead>Type</TableHead>
                            <TableHead>Item</TableHead>
                            <TableHead>Quantity</TableHead>
                            <TableHead>From/To</TableHead>
                            <TableHead>Operator</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {mockTransactions.filter(t => 
                            t.fromWarehouse === selectedWarehouse || t.toWarehouse === selectedWarehouse
                          ).map((txn, index) => (
                            <TableRow key={index}>
                              <TableCell>{txn.timestamp}</TableCell>
                              <TableCell>{txn.type}</TableCell>
                              <TableCell>{txn.item}</TableCell>
                              <TableCell>{txn.quantity}</TableCell>
                              <TableCell>{txn.fromWarehouse} → {txn.toWarehouse}</TableCell>
                              <TableCell>{txn.operator}</TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </div>
                  </CardContent>
                </Card>
              </TabsContent>

              <TabsContent value="current-stock" className="space-y-4">
                <Card>
                  <CardHeader>
                    <CardTitle>Current Stock in {selectedWarehouse}</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="overflow-hidden rounded-lg border">
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>Item Code</TableHead>
                            <TableHead>Name (EN/MM)</TableHead>
                            <TableHead>Qty (pcs/kg)</TableHead>
                            <TableHead>Unit</TableHead>
                            <TableHead>Last Movement</TableHead>
                            <TableHead>Operator</TableHead>
                            <TableHead>Timestamp</TableHead>
                            <TableHead>Actions</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          <TableRow>
                            <TableCell className="font-medium">RM-001</TableCell>
                            <TableCell>
                              <div>
                                <div>PP Granules</div>
                                <div className="text-xs text-muted-foreground">PP အမွှေးများ</div>
                              </div>
                            </TableCell>
                            <TableCell>
                              <div className="flex items-center gap-2">
                                <span className="font-medium">2,500</span>
                                <Button 
                                  variant="outline" 
                                  size="sm" 
                                  onClick={() => toggleUnit('rm001')}
                                >
                                  {unitToggle['rm001'] || 'kg'}
                                </Button>
                              </div>
                            </TableCell>
                            <TableCell>kg</TableCell>
                            <TableCell>Stock In from Supplier</TableCell>
                            <TableCell>Staff001</TableCell>
                            <TableCell>2025-09-07 14:30</TableCell>
                            <TableCell>
                              <div className="flex gap-1">
                                <Button size="sm" variant="outline">
                                  <Eye className="h-3 w-3" />
                                </Button>
                                <Button size="sm" variant="outline">
                                  <ArrowRightLeft className="h-3 w-3" />
                                </Button>
                              </div>
                            </TableCell>
                          </TableRow>
                          <TableRow>
                            <TableCell className="font-medium">FG-001</TableCell>
                            <TableCell>
                              <div>
                                <div>Plastic Bottle 500ml</div>
                                <div className="text-xs text-muted-foreground">ပလတ်စတစ်ပုလင်း ၅၀၀ မီလီ</div>
                              </div>
                            </TableCell>
                            <TableCell>
                              <div className="flex items-center gap-2">
                                <span className="font-medium">800</span>
                                <Button 
                                  variant="outline" 
                                  size="sm" 
                                  onClick={() => toggleUnit('fg001')}
                                >
                                  {unitToggle['fg001'] || 'pcs'}
                                </Button>
                              </div>
                            </TableCell>
                            <TableCell>pcs</TableCell>
                            <TableCell>Transfer from PROD-WH</TableCell>
                            <TableCell>Staff002</TableCell>
                            <TableCell>2025-09-07 15:15</TableCell>
                            <TableCell>
                              <div className="flex gap-1">
                                <Button size="sm" variant="outline">
                                  <Eye className="h-3 w-3" />
                                </Button>
                                <Button size="sm" variant="outline">
                                  <ArrowRightLeft className="h-3 w-3" />
                                </Button>
                              </div>
                            </TableCell>
                          </TableRow>
                          <TableRow>
                            <TableCell className="font-medium">GL-001</TableCell>
                            <TableCell>
                              <div>
                                <div>Industrial Adhesive</div>
                                <div className="text-xs text-muted-foreground">စက်မှုလုပ်ငန်းသုံးကော်</div>
                              </div>
                            </TableCell>
                            <TableCell>
                              <div className="flex items-center gap-2">
                                <span className="font-medium">450</span>
                                <Button 
                                  variant="outline" 
                                  size="sm" 
                                  onClick={() => toggleUnit('gl001')}
                                >
                                  {unitToggle['gl001'] || 'kg'}
                                </Button>
                              </div>
                            </TableCell>
                            <TableCell>kg</TableCell>
                            <TableCell>Stock In from Supplier</TableCell>
                            <TableCell>Staff003</TableCell>
                            <TableCell>2025-09-07 13:45</TableCell>
                            <TableCell>
                              <div className="flex gap-1">
                                <Button size="sm" variant="outline">
                                  <Eye className="h-3 w-3" />
                                </Button>
                                <Button size="sm" variant="outline">
                                  <ArrowRightLeft className="h-3 w-3" />
                                </Button>
                              </div>
                            </TableCell>
                          </TableRow>
                        </TableBody>
                      </Table>
                    </div>
                  </CardContent>
                </Card>
              </TabsContent>

              <TabsContent value="transfer-rules" className="space-y-4">
                <Card>
                  <CardHeader>
                    <CardTitle>Transfer Rules for {selectedWarehouse}</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="space-y-4">
                      <div className="p-4 bg-blue-50 rounded-lg border border-blue-200">
                        <h4 className="font-medium mb-2">Allowed Destinations:</h4>
                        <div className="flex flex-wrap gap-2">
                          {getAllowedDestinations(selectedWarehouse).length > 0 ? (
                            getAllowedDestinations(selectedWarehouse).map(dest => (
                              <Badge key={dest} variant="default" className="flex items-center gap-1">
                                {mockWarehouses.find(w => w.id === dest)?.color} {dest}
                              </Badge>
                            ))
                          ) : (
                            <Badge variant="secondary">Final destination - No outbound transfers</Badge>
                          )}
                        </div>
                      </div>
                      
                      {selectedWarehouse === 'PROD-WH' && (
                        <div className="p-4 bg-yellow-50 rounded-lg border border-yellow-200">
                          <h4 className="font-medium mb-2">Special Rules:</h4>
                          <ul className="text-sm text-slate-600 space-y-1">
                            <li>• To FG-WH: Only after QC approval</li>
                            <li>• To CUT-WH: For defective items only</li>
                            <li>• To RM-WH: Return unused materials</li>
                          </ul>
                        </div>
                      )}
                      
                      {selectedWarehouse === 'FG-WH' && (
                        <div className="p-4 bg-green-50 rounded-lg border border-green-200">
                          <h4 className="font-medium mb-2">Special Rules:</h4>
                          <ul className="text-sm text-slate-600 space-y-1">
                            <li>• QR/RFID label required for all items</li>
                            <li>• FIFO principle for dispatch</li>
                            <li>• Batch tracking mandatory</li>
                          </ul>
                        </div>
                      )}
                    </div>
                  </CardContent>
                </Card>
              </TabsContent>
            </Tabs>
          </CardContent>
        </Card>
      )}
    </div>
  );

  const renderBorrowCenter = () => (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h2 className="text-lg font-semibold">Borrow Center | ငှားယူစင်တာ</h2>
        <Button onClick={() => setShowBorrowDialog(true)}>
          <Plus className="h-4 w-4 mr-2" />
          New Borrow Request | ငှားယူရန်တောင်းဆိုမှုအသစ်
        </Button>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Summary Cards */}
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-2xl font-semibold">{mockBorrowRequests.filter(r => r.status === 'pending').length}</p>
                <p className="text-sm text-muted-foreground">Pending Requests</p>
                <p className="text-xs text-slate-500">စောင့်ဆိုင်းနေသောတောင်းဆိုမှုများ</p>
              </div>
              <Clock className="h-8 w-8 text-yellow-600" />
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-2xl font-semibold">{mockBorrowRequests.filter(r => r.status === 'approved').length}</p>
                <p className="text-sm text-muted-foreground">Active Borrows</p>
                <p className="text-xs text-slate-500">လက်ရှိငှားယူမှုများ</p>
              </div>
              <HandCoins className="h-8 w-8 text-green-600" />
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-2xl font-semibold">{mockBorrowRequests.filter(r => r.status === 'completed').length}</p>
                <p className="text-sm text-muted-foreground">Completed</p>
                <p className="text-xs text-slate-500">ပြီးစီးသောငှားယူမှုများ</p>
              </div>
              <CheckCircle className="h-8 w-8 text-blue-600" />
            </div>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Clipboard className="h-5 w-5" />
            Borrow Requests | ငှားယူတောင်းဆိုမှုများ
          </CardTitle>
        </CardHeader>
        <CardContent>
          <Tabs defaultValue="all">
            <TabsList className="grid w-full grid-cols-4">
              <TabsTrigger value="all">All Requests</TabsTrigger>
              <TabsTrigger value="pending">Pending</TabsTrigger>
              <TabsTrigger value="approved">Approved</TabsTrigger>
              <TabsTrigger value="completed">Completed</TabsTrigger>
            </TabsList>

            <TabsContent value="all" className="space-y-4">
              <div className="overflow-hidden rounded-lg border">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Borrow ID</TableHead>
                      <TableHead>Item</TableHead>
                      <TableHead>From → To</TableHead>
                      <TableHead>Quantity</TableHead>
                      <TableHead>Reason</TableHead>
                      <TableHead>Expected Return</TableHead>
                      <TableHead>Status</TableHead>
                      <TableHead>Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {mockBorrowRequests.map((request) => (
                      <TableRow key={request.id}>
                        <TableCell className="font-medium">{request.id}</TableCell>
                        <TableCell>{request.item}</TableCell>
                        <TableCell>
                          <div className="text-sm">
                            <div>{request.sourceJob} ({request.sourceWarehouse})</div>
                            <div className="flex items-center gap-1 text-muted-foreground">
                              <ArrowRightLeft className="h-3 w-3" />
                              {request.destJob} ({request.destWarehouse})
                            </div>
                          </div>
                        </TableCell>
                        <TableCell>{request.quantity}</TableCell>
                        <TableCell className="max-w-40 truncate" title={request.reason}>{request.reason}</TableCell>
                        <TableCell>{request.expectedReturn}</TableCell>
                        <TableCell>{getStatusBadge(request.status)}</TableCell>
                        <TableCell>
                          <div className="flex gap-1">
                            {request.status === 'pending' && (
                              <>
                                <Button size="sm" variant="outline" className="bg-green-50 hover:bg-green-100">
                                  <Check className="h-3 w-3" />
                                </Button>
                                <Button size="sm" variant="outline" className="bg-red-50 hover:bg-red-100">
                                  <X className="h-3 w-3" />
                                </Button>
                              </>
                            )}
                            <Button size="sm" variant="outline">
                              <Eye className="h-3 w-3" />
                            </Button>
                          </div>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>
            </TabsContent>
          </Tabs>
        </CardContent>
      </Card>
    </div>
  );

  const renderStockAdjustment = () => (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-lg font-semibold">Stock Adjustment | စတော့ခ်ချိန်ညှိမှု</h2>
          <p className="text-sm text-yellow-600 flex items-center gap-1">
            <AlertTriangle className="h-4 w-4" />
            Supervisor access only | ကြီးကြပ်ရေးမှူးခွင့်ပြုချက်သာ
          </p>
        </div>
        <Button onClick={() => setShowAdjustDialog(true)}>
          <RefreshCw className="h-4 w-4 mr-2" />
          New Adjustment
        </Button>
      </div>

      {/* Adjustment Form */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Scale className="h-5 w-5" />
            Stock Adjustment Request | စတော့ခ်ချိန်ညှိတောင်းဆိုမှု
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div className="space-y-4">
              <div>
                <Label>Select Item</Label>
                <Select>
                  <SelectTrigger>
                    <SelectValue placeholder="Select item to adjust" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="item1">PP Granules - RM-WH (2,500 kg)</SelectItem>
                    <SelectItem value="item2">Plastic Bottle 500ml - FG-WH (800 pcs)</SelectItem>
                    <SelectItem value="item3">Plastic Container 1L - PROD-WH (1,200 pcs)</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label>Current Quantity</Label>
                <Input value="2,500 kg" readOnly className="bg-slate-50" />
              </div>
            </div>
            
            <div className="space-y-4">
              <div>
                <Label>Adjust Quantity</Label>
                <div className="flex gap-2">
                  <Input type="number" placeholder="Enter adjustment" />
                  <Button variant="outline" onClick={() => toggleUnit('adjust')}>
                    {unitToggle['adjust'] || 'kg'}
                  </Button>
                </div>
              </div>
              <div>
                <Label>Adjustment Type</Label>
                <Select>
                  <SelectTrigger>
                    <SelectValue placeholder="Select type" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="increase">Increase (+)</SelectItem>
                    <SelectItem value="decrease">Decrease (-)</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>

            <div className="space-y-4">
              <div>
                <Label>Reason Code</Label>
                <Select>
                  <SelectTrigger>
                    <SelectValue placeholder="Select reason" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="count-variance">Physical Count Variance</SelectItem>
                    <SelectItem value="damage">Damage/Spoilage</SelectItem>
                    <SelectItem value="system-error">System Error</SelectItem>
                    <SelectItem value="theft">Theft/Loss</SelectItem>
                    <SelectItem value="other">Other</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label>Comments</Label>
                <Textarea placeholder="Additional comments..." rows={2} />
              </div>
            </div>
          </div>

          <div className="p-4 bg-yellow-50 border border-yellow-200 rounded-lg">
            <div className="flex items-center gap-2">
              <AlertTriangle className="h-5 w-5 text-yellow-600" />
              <span className="text-sm font-medium text-yellow-800">Approval Required</span>
            </div>
            <div className="text-sm text-yellow-700 mt-1">
              This adjustment requires supervisor approval before processing.
              ဤချိန်ညှိမှုသည် လုပ်ဆောင်မီ ကြီးကြပ်ရေးမှူး၏ ခွင့်ပြုချက် လိုအပ်ပါသည်။
            </div>
          </div>

          <div className="flex gap-4">
            <Button className="bg-orange-600 hover:bg-orange-700">
              <Send className="h-4 w-4 mr-2" />
              Submit for Approval
            </Button>
            <Button variant="outline">
              <Save className="h-4 w-4 mr-2" />
              Save Draft
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Adjustment History */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <History className="h-5 w-5" />
            Adjustment History | ချိန်ညှိမှုမှတ်တမ်း
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-3">
            <div className="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
              <div className="flex items-center gap-3">
                <div className="w-2 h-2 bg-green-500 rounded-full"></div>
                <div>
                  <div className="font-medium">Stock Adjustment: PP Granules +50 kg</div>
                  <div className="text-sm text-muted-foreground">Physical Count Variance - Approved by SUP001</div>
                </div>
              </div>
              <div className="text-right">
                <div className="text-sm">2025-09-07 16:30</div>
                <div className="text-xs text-muted-foreground">Admin001</div>
              </div>
            </div>
            
            <div className="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
              <div className="flex items-center gap-3">
                <div className="w-2 h-2 bg-blue-500 rounded-full"></div>
                <div>
                  <div className="font-medium">Reallocation: JOB001 → JOB003 (25 pcs)</div>
                  <div className="text-sm text-muted-foreground">Priority change requirement - Processed</div>
                </div>
              </div>
              <div className="text-right">
                <div className="text-sm">2025-09-07 15:45</div>
                <div className="text-xs text-muted-foreground">SUP001</div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const renderInventoryReports = () => (
    <div className="space-y-6">
      {/* Report Filters */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Filter className="h-5 w-5" />
            Report Generation | အစီရင်ခံစာထုတ်လုပ်မှု
          </CardTitle>
        </CardHeader>
        <CardContent>
          <Tabs defaultValue="stock-balance" className="space-y-4">
            <TabsList className="grid w-full grid-cols-3 lg:grid-cols-6">
              <TabsTrigger value="stock-balance">Stock Balance</TabsTrigger>
              <TabsTrigger value="movement-log">Movement Log</TabsTrigger>
              <TabsTrigger value="reserved-available">Reserved vs Available</TabsTrigger>
              <TabsTrigger value="borrowing-history">Borrowing History</TabsTrigger>
              <TabsTrigger value="fifo-batch">FIFO/Batch Lot</TabsTrigger>
              <TabsTrigger value="dispatch-tracking">Dispatch Tracking</TabsTrigger>
            </TabsList>

            <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mt-4">
              <div className="space-y-2">
                <Label>Date From</Label>
                <Input type="date" defaultValue="2025-09-01" />
              </div>
              <div className="space-y-2">
                <Label>Date To</Label>
                <Input type="date" defaultValue="2025-09-07" />
              </div>
              <div className="space-y-2">
                <Label>Warehouse</Label>
                <Select>
                  <SelectTrigger>
                    <SelectValue placeholder="All Warehouses" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Warehouses</SelectItem>
                    {mockWarehouses.map(wh => (
                      <SelectItem key={wh.id} value={wh.id}>
                        {wh.color} {wh.id}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-2">
                <Label>Format</Label>
                <Select defaultValue="detailed">
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="summary">Summary</SelectItem>
                    <SelectItem value="detailed">Detailed</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>

            <div className="flex gap-2 mt-4">
              <Button>
                <BarChart3 className="h-4 w-4 mr-2" />
                Generate Report
              </Button>
              <Button variant="outline">
                <Download className="h-4 w-4 mr-2" />
                Export Excel
              </Button>
              <Button variant="outline">
                <Download className="h-4 w-4 mr-2" />
                Export PDF
              </Button>
            </div>

            <TabsContent value="stock-balance" className="space-y-4">
              <Card>
                <CardHeader>
                  <CardTitle>Stock Balance Report | စတော့ခ်လက်ကျန်အစီရင်ခံစာ</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="overflow-hidden rounded-lg border">
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Warehouse</TableHead>
                          <TableHead>Item Code</TableHead>
                          <TableHead>Item Name</TableHead>
                          <TableHead>Current Stock</TableHead>
                          <TableHead>Reserved</TableHead>
                          <TableHead>Available</TableHead>
                          <TableHead>Unit Cost</TableHead>
                          <TableHead>Total Value</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        <TableRow>
                          <TableCell className="flex items-center gap-1">
                            <span className="text-lg">🟦</span>
                            RM-WH
                          </TableCell>
                          <TableCell>RM-001</TableCell>
                          <TableCell>PP Granules</TableCell>
                          <TableCell>2,500 kg</TableCell>
                          <TableCell>500 kg</TableCell>
                          <TableCell>2,000 kg</TableCell>
                          <TableCell>$1.20/kg</TableCell>
                          <TableCell>$3,000</TableCell>
                        </TableRow>
                        <TableRow>
                          <TableCell className="flex items-center gap-1">
                            <span className="text-lg">🟧</span>
                            FG-WH
                          </TableCell>
                          <TableCell>FG-001</TableCell>
                          <TableCell>Plastic Bottle 500ml</TableCell>
                          <TableCell>800 pcs</TableCell>
                          <TableCell>200 pcs</TableCell>
                          <TableCell>600 pcs</TableCell>
                          <TableCell>$0.50/pcs</TableCell>
                          <TableCell>$400</TableCell>
                        </TableRow>
                        <TableRow>
                          <TableCell className="flex items-center gap-1">
                            <span className="text-lg">🟩</span>
                            GLUE-WH
                          </TableCell>
                          <TableCell>GL-001</TableCell>
                          <TableCell>Industrial Adhesive</TableCell>
                          <TableCell>450 kg</TableCell>
                          <TableCell>50 kg</TableCell>
                          <TableCell>400 kg</TableCell>
                          <TableCell>$2.50/kg</TableCell>
                          <TableCell>$1,125</TableCell>
                        </TableRow>
                      </TableBody>
                    </Table>
                  </div>
                </CardContent>
              </Card>
            </TabsContent>

            <TabsContent value="borrowing-history" className="space-y-4">
              <Card>
                <CardHeader>
                  <CardTitle>Borrowing History Report | ငှားယူမှုမှတ်တမ်းအစီရင်ခံစာ</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="overflow-hidden rounded-lg border">
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Borrow ID</TableHead>
                          <TableHead>Item</TableHead>
                          <TableHead>From Job/WH</TableHead>
                          <TableHead>To Job/WH</TableHead>
                          <TableHead>Quantity</TableHead>
                          <TableHead>Borrowed Date</TableHead>
                          <TableHead>Return Date</TableHead>
                          <TableHead>Status</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {mockBorrowRequests.map((request) => (
                          <TableRow key={request.id}>
                            <TableCell className="font-medium">{request.id}</TableCell>
                            <TableCell>{request.item}</TableCell>
                            <TableCell>{request.sourceJob}/{request.sourceWarehouse}</TableCell>
                            <TableCell>{request.destJob}/{request.destWarehouse}</TableCell>
                            <TableCell>{request.quantity}</TableCell>
                            <TableCell>{request.requestDate}</TableCell>
                            <TableCell>{request.expectedReturn}</TableCell>
                            <TableCell>{getStatusBadge(request.status)}</TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </div>
                </CardContent>
              </Card>
            </TabsContent>
          </Tabs>
        </CardContent>
      </Card>
    </div>
  );

  const renderUserPermissions = () => (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <UserCheck className="h-5 w-5" />
            User Permissions Matrix | အသုံးပြုသူခွင့်ပြုချက်ဇယား
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead className="w-32">Role</TableHead>
                  <TableHead>Receive</TableHead>
                  <TableHead>Transfer</TableHead>
                  <TableHead>Borrow</TableHead>
                  <TableHead>Adjust</TableHead>
                  <TableHead>Print QR</TableHead>
                  <TableHead>View Reports</TableHead>
                  <TableHead>Delete</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockUserRoles.map((role, index) => (
                  <TableRow key={index}>
                    <TableCell className="font-medium">{role.role}</TableCell>
                    <TableCell>
                      <div className="flex items-center">
                        {role.receive ? <Check className="h-4 w-4 text-green-600" /> : <X className="h-4 w-4 text-red-600" />}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center">
                        {role.transfer ? <Check className="h-4 w-4 text-green-600" /> : <X className="h-4 w-4 text-red-600" />}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center">
                        {role.borrow ? <Check className="h-4 w-4 text-green-600" /> : <X className="h-4 w-4 text-red-600" />}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center">
                        {role.adjust ? <Check className="h-4 w-4 text-green-600" /> : <X className="h-4 w-4 text-red-600" />}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center">
                        {role.printQR ? <Check className="h-4 w-4 text-green-600" /> : <X className="h-4 w-4 text-red-600" />}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center">
                        {role.viewReports ? <Check className="h-4 w-4 text-green-600" /> : <X className="h-4 w-4 text-red-600" />}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center">
                        <X className="h-4 w-4 text-red-600" />
                        <span className="text-xs text-muted-foreground ml-1">(Disabled for traceability)</span>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
          
          <div className="mt-6">
            <Button onClick={() => setShowPermissionsDialog(true)}>
              <Edit className="h-4 w-4 mr-2" />
              Edit Permissions
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const renderFinishedGoodsReceiving = () => {
    // Updated mock data for the new intake queue design
    const mockIntakeQueue = [
      {
        releaseId: 'REL-20250907-001',
        jobId: 'JOB20250907-001-C1',
        product: '2011 - Plastic Bottle 500ml | ပလတ်စတစ်ပုလင်း ၅၀၀ml',
        qtyReleased: 1000,
        qcStatus: 'approved',
        releasedBy: 'Production Staff - Ko Thant (OP001)',
        releasedTime: '2025-09-07 14:30'
      },
      {
        releaseId: 'REL-20250907-002',
        jobId: 'JOB20250907-002-C2',
        product: '2012 - Plastic Container 1L | ပလတ်စတစ်ပုံး ၁L',
        qtyReleased: 750,
        qcStatus: 'pending',
        releasedBy: 'Production Staff - Ma Khin (OP002)',
        releasedTime: '2025-09-07 15:15'
      },
      {
        releaseId: 'REL-20250907-003',
        jobId: 'JOB20250907-003-C1',
        product: '2013 - Plastic Cup 250ml | ပလတ်စတစ်ခွက် ၂၅၀ml',
        qtyReleased: 1200,
        qcStatus: 'rejected',
        releasedBy: 'Production Staff - Ko Aung (OP003)',
        releasedTime: '2025-09-07 16:00'
      },
      {
        releaseId: 'REL-20250906-004',
        jobId: 'JOB20250906-001-C2',
        product: '2014 - Plastic Plate Round | ပလတ်စတစ်ပန်းကန်',
        qtyReleased: 800,
        qcStatus: 'approved',
        releasedBy: 'Production Staff - Ma Thida (OP004)',
        releasedTime: '2025-09-06 18:45'
      }
    ];

    // Mock FG-WH stock history data
    const mockFGWHStock = [
      {
        product: '2011 - Plastic Bottle 500ml | ပလတ်စတစ်ပုလင်း ၅၀၀ml',
        qty: 2000,
        labelType: 'QR',
        labelId: 'QR-FG-20250907-001',
        acceptedBy: 'Inventory Staff - Ko Min (INV001)',
        acceptedDateTime: '2025-09-07 14:45'
      },
      {
        product: '2012 - Plastic Container 1L | ပလတ်စတစ်ပုံး ၁L',
        qty: 1500,
        labelType: 'RFID',
        labelId: 'RFID-FG-20250907-002',
        acceptedBy: 'Inventory Staff - Ma Aye (INV002)',
        acceptedDateTime: '2025-09-07 16:30'
      },
      {
        product: '2013 - Plastic Cup 250ml | ပလတ်စတစ်ခွက် ၂၅၀ml',
        qty: 3000,
        labelType: 'QR',
        labelId: 'QR-FG-20250906-003',
        acceptedBy: 'Inventory Staff - Ko Zaw (INV003)',
        acceptedDateTime: '2025-09-06 19:15'
      },
      {
        product: '2014 - Plastic Plate Round | ပလတ်စတစ်ပန်းကန်',
        qty: 800,
        labelType: 'RFID',
        labelId: 'RFID-FG-20250906-004',
        acceptedBy: 'Inventory Staff - Ma Mya (INV004)',
        acceptedDateTime: '2025-09-06 20:00'
      }
    ];

    const handleAcceptToFGWH = (batch: any) => {
      setSelectedBatch(batch);
      setShowAcceptDialog(true);
    };

    const handleGenerateLabel = (batch: any) => {
      setSelectedBatch(batch);
      setShowLabelDialog(true);
    };

    const handlePartialAccept = (batch: any) => {
      setSelectedBatch(batch);
      setShowPartialAcceptDialog(true);
    };

    const handleRejectReturn = (batch: any) => {
      setSelectedBatch(batch);
      setShowRejectDialog(true);
    };

    const filteredIntakeQueue = mockIntakeQueue.filter(item => {
      const matchesRelease = !searchFilters.releaseId || item.releaseId.toLowerCase().includes(searchFilters.releaseId.toLowerCase());
      const matchesJob = !searchFilters.jobId || item.jobId.toLowerCase().includes(searchFilters.jobId.toLowerCase());
      const matchesProduct = !searchFilters.productName || item.product.toLowerCase().includes(searchFilters.productName.toLowerCase());
      const matchesQC = searchFilters.qcStatus === 'all' || item.qcStatus === searchFilters.qcStatus;
      
      return matchesRelease && matchesJob && matchesProduct && matchesQC;
    });

    return (
      <div className="space-y-6">
        {/* Header */}
        <div className="flex justify-between items-center">
          <div>
            <h2 className="text-lg font-semibold">Finished Goods Intake Queue / ကုန်ချောလက်ခံ စာရင်း</h2>
            <p className="text-sm text-blue-600 flex items-center gap-1">
              <Info className="h-4 w-4" />
              Production Control releases batches → Inventory accepts into FG Warehouse
            </p>
            <p className="text-xs text-slate-500">ထုတ်လုပ်မှုထိန်းချုပ်မှ အုပ်စုများလွှတ်ပေး → စတော့ခ်မှ FG ဂိုဒေါင်သို့လက်ခံ</p>
          </div>
        </div>

        {/* Search Filters */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Filter className="h-5 w-5" />
              Search Filters | ရှာဖွေမှုစစ်ထုတ်
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-6 gap-4">
              <div>
                <Label htmlFor="release-id">Release ID</Label>
                <Input
                  id="release-id"
                  placeholder="REL-XXXXXXX"
                  value={searchFilters.releaseId}
                  onChange={(e) => setSearchFilters(prev => ({ ...prev, releaseId: e.target.value }))}
                />
              </div>
              <div>
                <Label htmlFor="job-id">Job ID</Label>
                <Input
                  id="job-id"
                  placeholder="JOB-XXXXXXX"
                  value={searchFilters.jobId}
                  onChange={(e) => setSearchFilters(prev => ({ ...prev, jobId: e.target.value }))}
                />
              </div>
              <div>
                <Label htmlFor="product-name">Product Name / Code</Label>
                <Input
                  id="product-name"
                  placeholder="Product name or code"
                  value={searchFilters.productName}
                  onChange={(e) => setSearchFilters(prev => ({ ...prev, productName: e.target.value }))}
                />
              </div>
              <div>
                <Label htmlFor="qc-status">QC Status</Label>
                <Select value={searchFilters.qcStatus} onValueChange={(value) => setSearchFilters(prev => ({ ...prev, qcStatus: value }))}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Status</SelectItem>
                    <SelectItem value="approved">Approved</SelectItem>
                    <SelectItem value="pending">Pending</SelectItem>
                    <SelectItem value="rejected">Rejected</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label htmlFor="date-from">Date From</Label>
                <Input
                  id="date-from"
                  type="date"
                  value={searchFilters.dateFrom}
                  onChange={(e) => setSearchFilters(prev => ({ ...prev, dateFrom: e.target.value }))}
                />
              </div>
              <div>
                <Label htmlFor="date-to">Date To</Label>
                <Input
                  id="date-to"
                  type="date"
                  value={searchFilters.dateTo}
                  onChange={(e) => setSearchFilters(prev => ({ ...prev, dateTo: e.target.value }))}
                />
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Intake Queue Table */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <PackageOpen className="h-5 w-5" />
              Intake Queue | လက်ခံရန်စောင့်နေသည်
            </CardTitle>
            <div className="text-sm text-muted-foreground">
              Batches released by Production Control awaiting acceptance into FG Warehouse
            </div>
          </CardHeader>
          <CardContent>
            <div className="overflow-hidden rounded-lg border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Release ID</TableHead>
                    <TableHead>Job ID</TableHead>
                    <TableHead>Product (EN/MM)</TableHead>
                    <TableHead>Qty Released</TableHead>
                    <TableHead>QC Status</TableHead>
                    <TableHead>Released By (Production)</TableHead>
                    <TableHead>Released Time</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredIntakeQueue.map((item, index) => (
                    <TableRow key={index}>
                      <TableCell className="font-medium">{item.releaseId}</TableCell>
                      <TableCell>{item.jobId}</TableCell>
                      <TableCell>{item.product}</TableCell>
                      <TableCell>{item.qtyReleased.toLocaleString()} pcs</TableCell>
                      <TableCell>{getStatusBadge(item.qcStatus)}</TableCell>
                      <TableCell>{item.releasedBy}</TableCell>
                      <TableCell>{item.releasedTime}</TableCell>
                      <TableCell>
                        <div className="flex gap-1 flex-wrap">
                          <Button 
                            size="sm" 
                            className="bg-green-600 hover:bg-green-700"
                            onClick={() => handleAcceptToFGWH(item)}
                            disabled={item.qcStatus !== 'approved'}
                            title="Accept batch into FG Warehouse"
                          >
                            <CheckCircle className="h-3 w-3 mr-1" />
                            Accept
                          </Button>
                          <Button 
                            size="sm" 
                            variant="outline"
                            onClick={() => handleGenerateLabel(item)}
                            disabled={item.qcStatus !== 'approved'}
                            title="Generate QR/RFID label"
                          >
                            <QrCode className="h-3 w-3 mr-1" />
                            Label
                          </Button>
                          <Button 
                            size="sm" 
                            className="bg-yellow-600 hover:bg-yellow-700"
                            onClick={() => handlePartialAccept(item)}
                            disabled={item.qcStatus !== 'approved'}
                            title="Accept partial quantity"
                          >
                            <PackageOpen className="h-3 w-3 mr-1" />
                            Partial
                          </Button>
                          <Button 
                            size="sm" 
                            variant="outline" 
                            className="bg-red-50 hover:bg-red-100"
                            onClick={() => handleRejectReturn(item)}
                            title="Reject and return to production"
                          >
                            <X className="h-3 w-3 mr-1" />
                            Reject
                          </Button>
                        </div>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>

        {/* FG-WH Stock History */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5" />
              FG-WH Stock History | FG-WH စတော့ခ်မှတ်တမ်း
            </CardTitle>
            <div className="text-sm text-muted-foreground">
              Previously accepted batches now in FG Warehouse stock
            </div>
          </CardHeader>
          <CardContent>
            <div className="overflow-hidden rounded-lg border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Product</TableHead>
                    <TableHead>Qty</TableHead>
                    <TableHead>Label Type (QR/RFID)</TableHead>
                    <TableHead>Label ID</TableHead>
                    <TableHead>Accepted By</TableHead>
                    <TableHead>Accepted Date & Time</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {mockFGWHStock.map((item, index) => (
                    <TableRow key={index}>
                      <TableCell>{item.product}</TableCell>
                      <TableCell>{item.qty.toLocaleString()} pcs</TableCell>
                      <TableCell>
                        <Badge className={item.labelType === 'QR' ? 'bg-blue-100 text-blue-800' : 'bg-purple-100 text-purple-800'}>
                          {item.labelType}
                        </Badge>
                      </TableCell>
                      <TableCell className="font-mono text-sm">{item.labelId}</TableCell>
                      <TableCell>{item.acceptedBy}</TableCell>
                      <TableCell>{item.acceptedDateTime}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>

        {/* Accept Dialog */}
        <Dialog open={showAcceptDialog} onOpenChange={setShowAcceptDialog}>
          <DialogContent className="max-w-md">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <CheckCircle className="h-5 w-5 text-green-600" />
                Accept to FG-WH | FG-WH သို့လက်ခံ
              </DialogTitle>
            </DialogHeader>
            {selectedBatch && (
              <div className="space-y-4">
                <div className="p-4 bg-green-50 rounded-lg">
                  <p className="text-sm"><strong>Release ID:</strong> {selectedBatch.releaseId}</p>
                  <p className="text-sm"><strong>Product:</strong> {selectedBatch.product}</p>
                  <p className="text-sm"><strong>Quantity:</strong> {selectedBatch.qtyReleased} pcs</p>
                </div>
                <p className="text-sm text-muted-foreground">
                  This will confirm the batch into FG warehouse stock and generate automatic labels.
                </p>
                <div className="flex gap-2">
                  <Button 
                    className="flex-1 bg-green-600 hover:bg-green-700"
                    onClick={() => showMovementConfirmation(
                      'Confirm Receipt',
                      'လက်ခံအတည်ပြု',
                      handleFGAcceptConfirm
                    )}
                  >
                    Confirm Accept
                  </Button>
                  <Button variant="outline" onClick={() => setShowAcceptDialog(false)}>
                    Cancel
                  </Button>
                </div>
              </div>
            )}
          </DialogContent>
        </Dialog>

        {/* Generate Label Dialog */}
        <Dialog open={showLabelDialog} onOpenChange={setShowLabelDialog}>
          <DialogContent className="max-w-md">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <QrCode className="h-5 w-5 text-blue-600" />
                Generate Label | လေဘယ်ထုတ်လုပ်
              </DialogTitle>
            </DialogHeader>
            {selectedBatch && (
              <div className="space-y-4">
                <div className="p-4 bg-blue-50 rounded-lg">
                  <p className="text-sm"><strong>Release ID:</strong> {selectedBatch.releaseId}</p>
                  <p className="text-sm"><strong>Product:</strong> {selectedBatch.product}</p>
                  <p className="text-sm"><strong>Quantity:</strong> {selectedBatch.qtyReleased} pcs</p>
                </div>
                <div>
                  <Label>Choose Label Type</Label>
                  <div className="grid grid-cols-2 gap-4 mt-2">
                    <div 
                      className={`p-4 border-2 rounded-lg cursor-pointer text-center ${
                        selectedLabelType === 'QR' ? 'border-blue-500 bg-blue-50' : 'border-gray-200 bg-gray-50'
                      }`}
                      onClick={() => setSelectedLabelType('QR')}
                    >
                      <QrCode className="h-8 w-8 mx-auto mb-2 text-blue-600" />
                      <div className="font-medium">QR Code</div>
                      <div className="text-xs text-muted-foreground">Recommended for bottles</div>
                    </div>
                    <div 
                      className={`p-4 border-2 rounded-lg cursor-pointer text-center ${
                        selectedLabelType === 'RFID' ? 'border-purple-500 bg-purple-50' : 'border-gray-200 bg-gray-50'
                      }`}
                      onClick={() => setSelectedLabelType('RFID')}
                    >
                      <Camera className="h-8 w-8 mx-auto mb-2 text-purple-600" />
                      <div className="font-medium">RFID</div>
                      <div className="text-xs text-muted-foreground">Recommended for containers</div>
                    </div>
                  </div>
                </div>
                <div className="flex gap-2">
                  <Button 
                    className="flex-1 bg-blue-600 hover:bg-blue-700"
                    onClick={() => {
                      console.log('Generating label:', selectedLabelType, 'for batch:', selectedBatch);
                      setShowLabelDialog(false);
                    }}
                  >
                    <Printer className="h-4 w-4 mr-2" />
                    Generate {selectedLabelType}
                  </Button>
                  <Button variant="outline" onClick={() => setShowLabelDialog(false)}>
                    Cancel
                  </Button>
                </div>
              </div>
            )}
          </DialogContent>
        </Dialog>

        {/* Partial Accept Dialog */}
        <Dialog open={showPartialAcceptDialog} onOpenChange={setShowPartialAcceptDialog}>
          <DialogContent className="max-w-md">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <PackageOpen className="h-5 w-5 text-yellow-600" />
                Partial Accept | တစ်စိတ်တစ်ပိုင်းလက်ခံ
              </DialogTitle>
            </DialogHeader>
            {selectedBatch && (
              <div className="space-y-4">
                <div className="p-4 bg-yellow-50 rounded-lg">
                  <p className="text-sm"><strong>Release ID:</strong> {selectedBatch.releaseId}</p>
                  <p className="text-sm"><strong>Product:</strong> {selectedBatch.product}</p>
                  <p className="text-sm"><strong>Total Released:</strong> {selectedBatch.qtyReleased} pcs</p>
                </div>
                <div>
                  <Label htmlFor="partial-qty">Enter Accepted Quantity</Label>
                  <Input
                    id="partial-qty"
                    type="number"
                    placeholder="Enter quantity to accept"
                    value={partialAcceptQty}
                    onChange={(e) => setPartialAcceptQty(e.target.value)}
                    max={selectedBatch.qtyReleased}
                  />
                  <p className="text-xs text-muted-foreground mt-1">
                    Remaining quantity will stay in the queue
                  </p>
                </div>
                <div className="flex gap-2">
                  <Button 
                    className="flex-1 bg-yellow-600 hover:bg-yellow-700"
                    disabled={!partialAcceptQty || parseInt(partialAcceptQty) <= 0}
                    onClick={() => {
                      console.log('Partial accepting:', partialAcceptQty, 'from batch:', selectedBatch);
                      setShowPartialAcceptDialog(false);
                      setPartialAcceptQty('');
                    }}
                  >
                    Accept {partialAcceptQty} pcs
                  </Button>
                  <Button variant="outline" onClick={() => {
                    setShowPartialAcceptDialog(false);
                    setPartialAcceptQty('');
                  }}>
                    Cancel
                  </Button>
                </div>
              </div>
            )}
          </DialogContent>
        </Dialog>

        {/* Reject/Return Dialog */}
        <Dialog open={showRejectDialog} onOpenChange={setShowRejectDialog}>
          <DialogContent className="max-w-md">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <X className="h-5 w-5 text-red-600" />
                Reject / Return to Production | ငြင်းပယ် / ထုတ်လုပ်မှုသို့ပြန်ပို့
              </DialogTitle>
            </DialogHeader>
            {selectedBatch && (
              <div className="space-y-4">
                <div className="p-4 bg-red-50 rounded-lg">
                  <p className="text-sm"><strong>Release ID:</strong> {selectedBatch.releaseId}</p>
                  <p className="text-sm"><strong>Product:</strong> {selectedBatch.product}</p>
                  <p className="text-sm"><strong>Quantity:</strong> {selectedBatch.qtyReleased} pcs</p>
                </div>
                <div>
                  <Label htmlFor="reject-reason">Reason for Rejection</Label>
                  <Select value={rejectReason} onValueChange={setRejectReason}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select reason" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="qc-fail">QC Fail</SelectItem>
                      <SelectItem value="packaging-damage">Packaging Damage</SelectItem>
                      <SelectItem value="quality-issues">Quality Issues</SelectItem>
                      <SelectItem value="contamination">Contamination</SelectItem>
                      <SelectItem value="wrong-specification">Wrong Specification</SelectItem>
                      <SelectItem value="other">Other</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
                <div className="flex gap-2">
                  <Button 
                    className="flex-1 bg-red-600 hover:bg-red-700"
                    disabled={!rejectReason}
                    onClick={() => {
                      console.log('Rejecting batch:', selectedBatch, 'reason:', rejectReason);
                      setShowRejectDialog(false);
                      setRejectReason('');
                    }}
                  >
                    Confirm Rejection
                  </Button>
                  <Button variant="outline" onClick={() => {
                    setShowRejectDialog(false);
                    setRejectReason('');
                  }}>
                    Cancel
                  </Button>
                </div>
              </div>
            )}
          </DialogContent>
        </Dialog>
      </div>
    );
  };



  const renderHQDispatch = () => (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-lg font-semibold">HQ Dispatch | ရုံးချုပ်ပို့ဆောင်မှု</h2>
          <p className="text-sm text-muted-foreground">Final dispatch operations to customers and headquarters</p>
        </div>
        <Button>
          <Plus className="h-4 w-4 mr-2" />
          Create Dispatch Order
        </Button>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-2xl font-semibold">12</p>
                <p className="text-sm text-muted-foreground">Ready for Dispatch</p>
                <p className="text-xs text-slate-500">ပို့ဆောင်ရန်အသင့်</p>
              </div>
              <Package className="h-8 w-8 text-green-600" />
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-2xl font-semibold">8</p>
                <p className="text-sm text-muted-foreground">In Transit</p>
                <p className="text-xs text-slate-500">ပို့ဆောင်နေသည်</p>
              </div>
              <Truck className="h-8 w-8 text-blue-600" />
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-2xl font-semibold">45</p>
                <p className="text-sm text-muted-foreground">Delivered Today</p>
                <p className="text-xs text-slate-500">ယနေ့ပေးပို့ပြီး</p>
              </div>
              <CheckCircle className="h-8 w-8 text-green-600" />
            </div>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Dispatch Queue | ပို့ဆောင်ရန်တ���်း</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Order ID</TableHead>
                  <TableHead>Customer</TableHead>
                  <TableHead>Product</TableHead>
                  <TableHead>Quantity</TableHead>
                  <TableHead>Priority</TableHead>
                  <TableHead>Scheduled</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                <TableRow>
                  <TableCell className="font-medium">ORD-2025-001</TableCell>
                  <TableCell>ABC Company</TableCell>
                  <TableCell>Plastic Bottles 500ml</TableCell>
                  <TableCell>1,000 pcs</TableCell>
                  <TableCell><Badge className="bg-red-100 text-red-800">High</Badge></TableCell>
                  <TableCell>2025-09-08 09:00</TableCell>
                  <TableCell>{getStatusBadge('pending')}</TableCell>
                  <TableCell>
                    <div className="flex gap-1">
                      <Button size="sm" variant="outline">
                        <Truck className="h-3 w-3" />
                      </Button>
                      <Button size="sm" variant="outline">
                        <Eye className="h-3 w-3" />
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
                <TableRow>
                  <TableCell className="font-medium">ORD-2025-002</TableCell>
                  <TableCell>XYZ Corporation</TableCell>
                  <TableCell>Plastic Containers 1L</TableCell>
                  <TableCell>500 pcs</TableCell>
                  <TableCell><Badge className="bg-yellow-100 text-yellow-800">Medium</Badge></TableCell>
                  <TableCell>2025-09-08 14:00</TableCell>
                  <TableCell>{getStatusBadge('in-progress')}</TableCell>
                  <TableCell>
                    <div className="flex gap-1">
                      <Button size="sm" variant="outline">
                        <MapPin className="h-3 w-3" />
                      </Button>
                      <Button size="sm" variant="outline">
                        <Eye className="h-3 w-3" />
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const getPageTitle = () => {
    switch (currentPage) {
      case 'inventory-dashboard':
        return {
          title: 'Inventory Dashboard | စတော့ခ်ဒက်ရှ်ဘုတ်',
          subtitle: 'Real-time warehouse monitoring with live stock counts, quick actions, and comprehensive overview.',
          subtitleMM: 'တိုက်ရိုက်စတော့ခ်အရေအတွက်များ၊ မြန်ဆန်သောလုပ်ဆောင်ချက်များနှင့် ပြီးပြည့်စုံသောခြုံငုံမြင်ကွင်းဖြင့် ကုန်လှောင်ရုံများကို အချိန်နှင့်တပြေးညီ စောင့်ကြည့်မှု။'
        };
      case 'warehouse-management':
        return {
          title: 'Warehouse Control | ကုန်လှောင်ရုံထိန်းချုပ်မှု',
          subtitle: 'Per-warehouse control with transfer rules, stock in/out operations, and transaction logging.',
          subtitleMM: 'လွှဲပြောင်းစည်းမျဉ်းများ၊ စတော့ခ်ဝင်/ထွက်လုပ်ဆောင်ချက်များနှင့် ငွေကြေးလွှဲပြောင်းမှုမှတ်တမ်းများဖြင့် ကုန်လှောင်ရုံခွဲများထိန်းချုပ်မှု။'
        };
      case 'stock-transactions':
        return {
          title: 'Stock Transactions | စတော့ခ်ငွေကြေးလွှဲပြောင်းမှုများ',
          subtitle: 'Process stock movements between warehouses with QR scanning and comprehensive tracking.',
          subtitleMM: 'QR စကင်န်နှင့် ပြီးပြည့်စုံသောခြေရာခံမှုဖြင့် ကုန်လှောင်ရုံများအကြား စတော့ခ်လှုပ်ရှားမှုများ လုပ်ဆောင်ခြင်း။'
        };
      case 'finished-goods-receiving':
        return {
          title: 'Finished Goods Intake Queue / ကုန်ချောလက်ခံ စာရင်း',
          subtitle: 'Accept batches released by Production Control into FG Warehouse with QR/RFID labeling and tracking.',
          subtitleMM: 'QR/RFID လေဘယ်တပ်ခြင်းနှင့် ခြေရာခံမှုဖြင့် ထုတ်လုပ်မှုထိန်းချုပ်မှ လွှတ်ပေးသော အုပ်စုများကို FG ဂိုဒေါင်သို့လက်ခံခြင်း။'
        };
      case 'defect-scrap-handling':
        return {
          title: 'Defect & Scrap Handling | ချို့ယွင်းမှုနှင့်အမှိုက်ကိုင်တွယ်မှု',
          subtitle: 'Manage defective products and scrap materials with sorting and disposal processes.',
          subtitleMM: 'ချို့ယွင်းသောထုတ်ကုန်များနှင့် အမှိုက်ပစ္စည်းများကို ခွဲခြ���းခြင်းနှင့် စွန့်ပစ်ခြင်းလုပ်ငန်းစဉ်များဖြင့် စီမံခန့်ခွဲခြင်း။'
        };
      case 'borrow-center':
        return {
          title: 'Borrow Center | ငှားယူစင်တာ',
          subtitle: 'Material borrowing and lending management with approval workflow and tracking.',
          subtitleMM: 'ခွင့်ပြုချက်လုပ်ငန်းစဉ်နှင့် ခြေရာခံမှုဖြင့် ပစ္စည်းငှားယူခြင်းနှင့် ငှားပေးခြင်းစီမံခန့်ခွဲမှု။'
        };
      case 'stock-adjustment':
        return {
          title: 'Stock Adjustment | စတော့ခ်ချိန်ညှိမှု',
          subtitle: 'Supervisor-only access for stock adjustments with approval workflow and audit trail.',
          subtitleMM: 'ခွင့်ပြုချက်လုပ်ငန်းစဉ်နှင့် စာရင်းစစ်ခြေရာခံမှုဖြင့် စတော့ခ်ချိန်ညှိမှုများအတွက် ကြီးကြပ်ရေးမှူးမျှသာ ဝင်ရောက်ခွင့်။'
        };
      case 'inventory-reports':
        return {
          title: 'Inventory Reports | စတော့ခ်အစီရင်ခံစာများ',
          subtitle: 'Comprehensive reporting suite with multiple report types and export capabilities.',
          subtitleMM: 'အစီရင်ခံစာအမျိုးအစားများစွာနှင့် ပို့ကုန်စွမ်းရည်များပါရှိသော ပြီးပြည့်စုံသောအစီရင်ခံစာအစုံ။'
        };
      case 'user-permissions':
        return {
          title: 'User Permissions Matrix | အသုံးပြုသူခွင့်ပြုချက်ဇယား',
          subtitle: 'Configure role-based access permissions for all inventory operations and functions.',
          subtitleMM: 'စတော့ခ်လုပ်ဆောင်ချက်များနှင့် လုပ်ငန်း���ျားအားလုံးအတွက် အခန်းကဏ္ဍအခြေခံ ဝင်ရောက်ခွင့်ပြုချက်များကို ပြင်ဆင်ခြင်း။'
        };
      case 'hq-dispatch':
        return {
          title: 'HQ Dispatch | ရုံးချုပ်ပို့ဆောင်မှု',
          subtitle: 'Final dispatch operations and tracking for customer orders and headquarters delivery.',
          subtitleMM: 'ဖောက်သည်တွင်းအမှာစာများနှင့် ရုံးချုပ်ပေးပို့မှုအတွက် နောက်ဆုံးပို့ဆောင်မှုလုပ်ငန်းများနှင့် ခြေရာခံမှု။'
        };
      default:
        return {
          title: 'Inventory Management | စတော့ခ်စီမံခန့်ခွဲမှု',
          subtitle: 'Comprehensive inventory management system for smart plastic factory operations.',
          subtitleMM: 'စမတ်ပလတ်စတစ်စက်ရုံလုပ်ငန်းများအတွက် ပြီးပြည့်စုံသော စတော့ခ်စီမံခန့်ခွဲမှုစနစ်။'
        };
    }
  };

  const renderCurrentPage = () => {
    switch (currentPage) {
      case 'inventory-dashboard':
        return renderInventoryDashboard();
      case 'warehouse-management':
        return renderWarehouseControl();
      case 'stock-transactions':
        return renderWarehouseControl(); // Same as warehouse control for now
      case 'finished-goods-receiving':
        return renderFinishedGoodsReceiving();
      case 'defect-scrap-handling':
        return renderBorrowCenter(); // Use borrow center for defect-scrap-handling for now
      case 'stock-adjustment':
        return renderStockAdjustment();
      case 'inventory-reports':
        return renderInventoryReports();
      case 'user-permissions':
        return renderUserPermissions();
      case 'hq-dispatch':
        return renderHQDispatch();
      default:
        return renderInventoryDashboard();
    }
  };

  const pageInfo = getPageTitle();

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-6">
          <h1 className="text-2xl font-semibold text-slate-900 mb-2">
            {pageInfo.title}
          </h1>
          <p className="text-slate-600">
            {pageInfo.subtitle}
          </p>
          <p className="text-sm text-slate-500 mt-1">
            {pageInfo.subtitleMM}
          </p>
        </div>

        {/* Current Page Content */}
        {renderCurrentPage()}

        {/* QR Scanner Dialog */}
        <Dialog open={showQRDialog} onOpenChange={setShowQRDialog}>
          <DialogContent className="max-w-md">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <QrCode className="h-5 w-5" />
                QR Code Scanner
              </DialogTitle>
            </DialogHeader>
            <div className="text-center p-8">
              <div className="w-48 h-48 bg-slate-100 rounded-lg mx-auto mb-4 flex items-center justify-center">
                <QrCode className="h-16 w-16 text-slate-400" />
              </div>
              <p className="text-sm text-muted-foreground mb-4">
                Position QR code within the frame to scan
              </p>
              <Button onClick={() => setShowQRDialog(false)}>
                <X className="h-4 w-4 mr-2" />
                Close Scanner
              </Button>
            </div>
          </DialogContent>
        </Dialog>

        {/* Borrow Request Dialog */}
        <Dialog open={showBorrowDialog} onOpenChange={setShowBorrowDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <HandCoins className="h-5 w-5" />
                New Borrow Request | ငှားယူရန်တောင်းဆိုမှုအသစ်
              </DialogTitle>
            </DialogHeader>
            <div className="space-y-6 p-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-4">
                  <div>
                    <Label>Source Job/Warehouse</Label>
                    <Select>
                      <SelectTrigger>
                        <SelectValue placeholder="Select source" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="job1">JOB20250907-001-C1 (RM-WH)</SelectItem>
                        <SelectItem value="job2">JOB20250907-002-C2 (FG-WH)</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                  <div>
                    <Label>Destination Job/Warehouse</Label>
                    <Select>
                      <SelectTrigger>
                        <SelectValue placeholder="Select destination" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="job3">JOB20250907-003-C3 (PROD-WH)</SelectItem>
                        <SelectItem value="job4">JOB20250907-004-C1 (PACK-WH)</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                </div>
                <div className="space-y-4">
                  <div>
                    <Label>Item</Label>
                    <Select>
                      <SelectTrigger>
                        <SelectValue placeholder="Select item" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="pp">PP Granules</SelectItem>
                        <SelectItem value="bottle">Plastic Bottle 500ml</SelectItem>
                        <SelectItem value="container">Plastic Container 1L</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                  <div className="grid grid-cols-2 gap-2">
                    <div>
                      <Label>Quantity</Label>
                      <Input type="number" placeholder="0" />
                    </div>
                    <div>
                      <Label>Unit</Label>
                      <Select>
                        <SelectTrigger>
                          <SelectValue placeholder="Unit" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="kg">kg</SelectItem>
                          <SelectItem value="pcs">pcs</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                  </div>
                </div>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <Label>Expected Return Date</Label>
                  <Input type="date" />
                </div>
                <div>
                  <Label>Reason</Label>
                  <Select>
                    <SelectTrigger>
                      <SelectValue placeholder="Select reason" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="urgent">Urgent production requirement</SelectItem>
                      <SelectItem value="shortage">Material shortage</SelectItem>
                      <SelectItem value="reprocess">Reprocessing needed</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>
              <div>
                <Label>Additional Notes</Label>
                <Textarea placeholder="Any additional information..." rows={3} />
              </div>
              <div className="flex gap-4">
                <Button className="flex-1" onClick={() => setShowBorrowDialog(false)}>
                  <Send className="h-4 w-4 mr-2" />
                  Submit Request
                </Button>
                <Button variant="outline" onClick={() => setShowBorrowDialog(false)}>
                  Cancel
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>

        {/* Transfer Dialog */}
        <Dialog open={showTransferDialog} onOpenChange={setShowTransferDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <ArrowRightLeft className="h-5 w-5" />
                Stock Transfer | စတော့ခ်လွှဲပြောင်းမှု
              </DialogTitle>
            </DialogHeader>
            <div className="space-y-6 p-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-4">
                  <div>
                    <Label>From Warehouse</Label>
                    <Select value={selectedTransferFrom} onValueChange={setSelectedTransferFrom}>
                      <SelectTrigger>
                        <SelectValue placeholder="Select source warehouse" />
                      </SelectTrigger>
                      <SelectContent>
                        {mockWarehouses.map(wh => (
                          <SelectItem key={wh.id} value={wh.id}>
                            {wh.color} {wh.id} - {wh.name}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                  <div>
                    <Label>To Warehouse</Label>
                    <Select disabled={!selectedTransferFrom}>
                      <SelectTrigger>
                        <SelectValue placeholder={selectedTransferFrom ? "Select destination warehouse" : "Select source first"} />
                      </SelectTrigger>
                      <SelectContent>
                        {selectedTransferFrom && getAllowedDestinations(selectedTransferFrom).map(dest => (
                          <SelectItem key={dest} value={dest}>
                            {mockWarehouses.find(w => w.id === dest)?.color} {dest} - {mockWarehouses.find(w => w.id === dest)?.name}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    {selectedTransferFrom && getAllowedDestinations(selectedTransferFrom).length === 0 && (
                      <p className="text-xs text-red-600 mt-1">This warehouse is a final destination point.</p>
                    )}
                  </div>
                </div>
                <div className="space-y-4">
                  <div>
                    <Label>Item Code/Name</Label>
                    <div className="flex gap-2">
                      <Input placeholder="Enter or scan item..." />
                      <Button variant="outline" size="icon">
                        <QrCode className="h-4 w-4" />
                      </Button>
                    </div>
                  </div>
                  <div className="grid grid-cols-2 gap-2">
                    <div>
                      <Label>Quantity</Label>
                      <Input type="number" placeholder="0" />
                    </div>
                    <div>
                      <Label>Unit</Label>
                      <Select>
                        <SelectTrigger>
                          <SelectValue placeholder="Unit" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="kg">kg</SelectItem>
                          <SelectItem value="pcs">pcs</SelectItem>
                          <SelectItem value="lb">lb</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                  </div>
                </div>
              </div>
              <div>
                <Label>Transfer Notes</Label>
                <Textarea placeholder="Additional notes about this transfer..." rows={3} />
              </div>
              <div className="space-y-3">
                <div className="flex gap-4">
                  <Button 
                    className="flex-1 bg-green-600 hover:bg-green-700"
                    onClick={() => showMovementConfirmation(
                      'Confirm Transfer',
                      'လွှဲပြောင်းအတည်ပြု',
                      handleTransferConfirm
                    )}
                  >
                    <ArrowRightLeft className="h-4 w-4 mr-2" />
                    Confirm Transfer
                  </Button>
                  <Button variant="outline" onClick={() => setShowTransferDialog(false)}>
                    Cancel
                  </Button>
                </div>
                <p className="text-xs text-center text-slate-500">
                  Re-authentication required. | အတည်ပြုရန် စကားဝှက်လိုအပ်သည်။
                </p>
              </div>
            </div>
          </DialogContent>
        </Dialog>

        {/* Permissions Edit Dialog */}
        <Dialog open={showPermissionsDialog} onOpenChange={setShowPermissionsDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>Edit User Permissions</DialogTitle>
            </DialogHeader>
            <div className="space-y-4">
              <p className="text-sm text-muted-foreground">
                Configure role-based permissions for inventory operations.
              </p>
              {/* Permission editing interface would go here */}
              <div className="flex justify-end gap-2">
                <Button variant="outline" onClick={() => setShowPermissionsDialog(false)}>
                  Cancel
                </Button>
                <Button onClick={() => setShowPermissionsDialog(false)}>
                  Save Changes
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
        
        {/* Movement Confirmation Modal */}
        <MovementConfirmationModal
          isOpen={showConfirmationModal}
          onClose={() => setShowConfirmationModal(false)}
          onConfirm={confirmationModalConfig.onConfirm}
          title={confirmationModalConfig.title}
          titleMM={confirmationModalConfig.titleMM}
        />
      </div>
    </div>
  );
}