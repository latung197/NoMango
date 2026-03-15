import React, { useState } from 'react';
import { MovementConfirmationModal } from './MovementConfirmationModal';
import { WarehouseDashboard } from './WarehouseDashboard';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Textarea } from './ui/textarea';
import { Checkbox } from './ui/checkbox';
import { Switch } from './ui/switch';
import { Separator } from './ui/separator';
import { Alert, AlertDescription } from './ui/alert';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Progress } from './ui/progress';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { Tooltip, TooltipContent, TooltipTrigger, TooltipProvider } from './ui/tooltip';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from './ui/dialog';
import { toast } from 'sonner';
import { 
  Package,
  Warehouse,
  ArrowLeftRight,
  Plus,
  Search,
  Filter,
  Download,
  Upload,
  QrCode,
  Tag,
  TrendingUp,
  TrendingDown,
  Clock,
  CheckCircle,
  XCircle,
  AlertTriangle,
  BarChart3,
  Users,
  Truck,
  Edit,
  Eye,
  MapPin,
  Factory,
  User,
  Calendar,
  FileText,
  Settings,
  Shield,
  Route,
  Archive,
  RotateCcw,
  Send,
  ShoppingCart,
  Calculator,
  RefreshCw,
  Scan,
  Play,
  Building2,
  Minus,
  Search,
  Warehouse,
  X,
  Calculator,
  Play
} from 'lucide-react';

interface InventoryModuleProps {
  currentPage: string;
  onPageChange: (page: string) => void;
}

// Mock data for warehouses
const mockWarehouses = [
  {
    id: 'RM-WH',
    name: 'Raw Material',
    nameMM: 'ကုန်ကြမ်း',
    type: 'Physical',
    inputMethod: 'QR',
    active: true,
    stock: 2500,
    unit: 'kg',
    status: 'normal',
    lastUpdate: '2025-09-07 15:30'
  },
  {
    id: 'GLUE-WH',
    name: 'Glue & Adhesive',
    nameMM: 'ကော်စာ·ကော်ခဲ',
    type: 'Physical',
    inputMethod: 'Manual',
    active: true,
    stock: 850,
    unit: 'kg',
    status: 'low',
    lastUpdate: '2025-09-07 14:45'
  },
  {
    id: 'PROD-WH',
    name: 'Production Floor',
    nameMM: 'ထုတ်လုပ်ရေးလမ်းကြောင်း',
    type: 'Virtual',
    inputMethod: 'RFID',
    active: true,
    stock: 0,
    unit: 'pcs',
    status: 'normal',
    lastUpdate: '2025-09-07 16:00'
  },
  {
    id: 'FG-WH',
    name: 'Finished Goods',
    nameMM: 'ကုန်ချော',
    type: 'Physical',
    inputMethod: 'QR',
    active: true,
    stock: 12400,
    unit: 'pcs',
    status: 'normal',
    lastUpdate: '2025-09-07 15:45'
  },
  {
    id: 'SCRAP-WH',
    name: 'Scrap & Reject',
    nameMM: 'အပိုင်းအစနှင့်ငြင်းပယ်',
    type: 'Physical',
    inputMethod: 'Manual',
    active: true,
    stock: 45,
    unit: 'kg',
    status: 'normal',
    lastUpdate: '2025-09-07 13:20'
  }
];

export function InventoryModule({ currentPage, onPageChange }: InventoryModuleProps) {
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedWarehouse, setSelectedWarehouse] = useState<string>('all');
  const [showAddForm, setShowAddForm] = useState(false);
  
  // Finished Goods Intake states
  const [selectedReleaseId, setSelectedReleaseId] = useState('');
  const [acceptedQty, setAcceptedQty] = useState('');
  const [selectedRack, setSelectedRack] = useState('');
  const [selectedBin, setSelectedBin] = useState('');
  const [labelMethod, setLabelMethod] = useState('');
  const [rejectReason, setRejectReason] = useState('');
  
  // Stock In/Out states
  const [transferMode, setTransferMode] = useState('transfer'); // 'receive' or 'transfer'
  const [fromWarehouse, setFromWarehouse] = useState('');
  const [toWarehouse, setToWarehouse] = useState('');
  const [selectedItem, setSelectedItem] = useState('');
  const [quantity, setQuantity] = useState('');
  const [unit, setUnit] = useState('');
  const [notes, setNotes] = useState('');
  
  // Scanner input states
  const [scanInput, setScanInput] = useState('');
  const [scannedProduct, setScannedProduct] = useState<any>(null);
  const [runningTotal, setRunningTotal] = useState(0);
  
  // Warehouse Registration states
  const [showAddEditModal, setShowAddEditModal] = useState(false);
  const [editingWarehouse, setEditingWarehouse] = useState<any>(null);
  const [warehouseSearchTerm, setWarehouseSearchTerm] = useState('');
  const [typeFilter, setTypeFilter] = useState('all');
  const [statusFilter, setStatusFilter] = useState('all');
  const [inputMethodFilter, setInputMethodFilter] = useState('all');
  
  // Movement confirmation modal states
  const [showConfirmationModal, setShowConfirmationModal] = useState(false);
  const [confirmationModalConfig, setConfirmationModalConfig] = useState({
    title: '',
    titleMM: '',
    onConfirm: () => {}
  });

  // Warehouse Routes states (moved to top level to avoid conditional hook calls)
  const [showNewRouteModal, setShowNewRouteModal] = useState(false);
  const [editingRoute, setEditingRoute] = useState<any>(null);
  const [routeFormData, setRouteFormData] = useState({
    fromWarehouse: '',
    toWarehouse: '',
    active: true,
    allowedCategories: [],
    maxDailyQty: '',
    autoApprove: false,
    approverRole: '',
    leadTimeMinutes: '',
    crossCompany: false,
    transportMethod: '',
    notes: ''
  });
  const [filters, setFilters] = useState({
    company: 'all',
    fromWarehouse: 'all',
    toWarehouse: 'all',
    activeStatus: 'all',
    category: 'all'
  });

  // Initialize form data with guaranteed string values
  const getInitialFormData = () => ({
    code: '',
    nameEN: '',
    nameMM: '',
    type: 'Physical' as const,
    inputMethod: 'Manual' as const,
    enableLocations: false,
    status: true,
    notes: '',
    locations: [{ rack: '', bin: '', capacity: '' }]
  });
  
  const [formData, setFormData] = useState(getInitialFormData());

  // Helper function to show confirmation modal
  const showMovementConfirmation = (title: string, titleMM: string, onConfirm: (reason?: string) => void) => {
    setConfirmationModalConfig({ title, titleMM, onConfirm });
    setShowConfirmationModal(true);
  };

  // Stock In confirmation handler
  const handleStockInConfirm = (reason?: string) => {
    console.log('Stock In confirmed', reason);
    toast.success('Stock In completed successfully! | စတော့ခ်ဝင်မှု အောင်မြင်စွာ ပြီးစီးပါပြီ။');
  };

  // Stock Out confirmation handler
  const handleStockOutConfirm = (reason?: string) => {
    console.log('Stock Out confirmed', reason);
    toast.success('Stock Out completed successfully! | စတော့ခ်ထွက်မှု အောင်မြင်စွာ ပြီးစီးပါပြီ။');
  };

  // Transfer confirmation handler
  const handleTransferConfirm = (reason?: string) => {
    console.log('Transfer confirmed', reason);
    toast.success('Transfer completed successfully! | လွှဲပြောင်းမှု အောင်မြင်စွာ ပြီးစီးပါပြီ။');
  };



  const renderFinishedGoodsIntake = () => {
    const mockReleasedBatches = [
      {
        releaseId: 'REL-20250907-001',
        jobId: 'JOB-20250907-001',
        product: 'Plastic Bottle 500ml',
        productMM: 'ပလတ်စတစ်ပုလင်း ၅၀၀မမ်လီ',
        qtyReleased: 10000,
        pendingQty: 10000,
        qcStatus: 'approved',
        releasedBy: 'Ko Zaw (SUP01)',
        releasedTime: '2025-09-07 15:30',
        status: 'pending-intake'
      },
      {
        releaseId: 'REL-20250907-002', 
        jobId: 'JOB-20250907-002',
        product: 'Plastic Cup 250ml',
        productMM: 'ပလတ်စတစ်ခွက် ၂၅၀မမ်လီ',
        qtyReleased: 8000,
        pendingQty: 8000,
        qcStatus: 'approved',
        releasedBy: 'Ma Thin (SUP02)',
        releasedTime: '2025-09-07 14:20',
        status: 'pending-intake'
      },
      {
        releaseId: 'REL-20250907-003',
        jobId: 'JOB-20250907-003',
        product: 'Plastic Container 1L',
        productMM: 'ပလတ်စတစ်ဘူး ၁လီတာ',
        qtyReleased: 5000,
        pendingQty: 2000,
        qcStatus: 'approved',
        releasedBy: 'Ko Min (SUP03)',
        releasedTime: '2025-09-07 13:45',
        status: 'partially-accepted'
      }
    ];

    const mockFGHistory = [
      {
        id: 'FG-001',
        product: 'Plastic Bottle 500ml',
        productMM: 'ပလတ်စတစ်ပုလင်း ၅၀၀မမ်လီ',
        acceptedQty: 9850,
        labelId: 'QR-JOB-20250907-001-001',
        jobId: 'JOB-20250907-001',
        acceptedBy: 'Ma Hnin (INV01)',
        dateTime: '2025-09-06 16:30'
      },
      {
        id: 'FG-002',
        product: 'Plastic Cup 250ml',
        productMM: 'ပလတ်စတစ်ခွက် ၂၅၀မမ်လီ',
        acceptedQty: 7800,
        labelId: 'UID-JOB-20250906-002-A1B2C3',
        jobId: 'JOB-20250906-002',
        acceptedBy: 'Ko Thant (INV02)',
        dateTime: '2025-09-06 15:20'
      }
    ];

    const mockRacks = ['A-01', 'A-02', 'B-01', 'B-02', 'C-01', 'C-02'];
    const mockBins = ['B-01', 'B-02', 'B-03', 'B-04', 'B-05', 'B-06'];

    const handleScan = () => {
      if (!scanInput.trim()) {
        toast.error("Please enter a label ID to scan | စကင်လုပ်ရန် လိပ်စာ ID ရိုက်ထည့်ပါ");
        return;
      }

      // Mock scan validation
      const foundBatch = mockReleasedBatches.find(batch => 
        scanInput.includes(batch.jobId) || scanInput.includes(batch.releaseId)
      );

      if (foundBatch) {
        setScannedProduct(foundBatch);
        setAcceptedQty(foundBatch.pendingQty.toString());
        toast.success(`Scanned successfully! | စကင်လုပ်ခြင်း အောင်မြင်ပါသည်!`);
        setScanInput('');
      } else {
        toast.error("Not in Intake Queue | လက်ခံစာရင်းတွင် မရှိပါ");
        setScanInput('');
      }
    };

    const handleAccept = () => {
      if (!scannedProduct) return;
      
      const qty = parseInt(acceptedQty) || scannedProduct.pendingQty;
      setRunningTotal(prev => prev + qty);
      
      toast.success(`Accepted to FG-WH | ကုန်ချော ဂိုဒေါင်သို့ လက်ခံပြီး: ${qty.toLocaleString()} pcs`);
      
      // Reset form
      setScannedProduct(null);
      setAcceptedQty('');
      setSelectedRack('');
      setSelectedBin('');
    };

    return (
      <div className="space-y-6">
        {/* Header */}
        <div className="bg-gradient-to-r from-green-50 to-blue-50 rounded-lg p-6 border border-green-200">
          <h1 className="text-2xl font-semibold text-slate-900">
            Finished Goods Intake (Scan to Accept) | ကုန်ချောလက်ခံ (စကင်ဖြင့် လက်ခံ)
          </h1>
          <p className="text-slate-600 mt-2">
            FG Warehouse Manager scans labels created at Production Control after QC approval
          </p>
          <p className="text-sm text-slate-500 mt-1">
            ⚠️ စကင်သာလုပ်ရန် - လိပ်စာများကို Production Control တွင် QC အတည်ပြုပြီးနောက် ဖန်တီးထားပြီးဖြစ်သည်
          </p>
        </div>

        <div className="grid grid-cols-1 xl:grid-cols-3 gap-6">
          {/* Left Column - Scan Panel */}
          <div className="xl:col-span-1">
            {/* Scanner Input */}
            <Card className="border-2 border-green-200 shadow-md mb-6">
              <CardHeader className="bg-green-50">
                <CardTitle className="flex items-center gap-2 text-green-800">
                  <Scan className="h-6 w-6" />
                  <span>Scan & Accept Panel | စကင်လုပ်ပြီး လက်ခံရန်</span>
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div>
                  <Label className="text-lg font-medium">
                    Scan Label to Accept | လက်ခံရန် လိပ်စာကို စကင်လုပ်ပါ
                  </Label>
                  <div className="flex gap-2 mt-2">
                    <Input
                      value={scanInput}
                      onChange={(e) => setScanInput(e.target.value)}
                      placeholder="Focus here & scan QR/RFID | စကင်ရန် ဤနေရာတွင် အာရုံစိုက်ပါ"
                      className="bg-yellow-50 border-2 border-yellow-300 text-lg h-16 font-mono"
                      onKeyPress={(e) => e.key === 'Enter' && handleScan()}
                      autoFocus
                    />
                    <Button 
                      onClick={handleScan}
                      className="bg-green-600 hover:bg-green-700 h-16 px-6"
                      size="lg"
                    >
                      <Scan className="h-5 w-5 mr-2" />
                      Scan
                    </Button>
                  </div>
                  <p className="text-sm text-slate-500 mt-2">
                    Input auto-focused for scanner | စကင်နာအတွက် အလိုအလျောက် အာရုံစိုက်ထားသည်
                  </p>
                </div>

                {/* Running Total */}
                {runningTotal > 0 && (
                  <div className="bg-blue-50 rounded-lg p-4 border border-blue-200">
                    <div className="flex items-center justify-between">
                      <span className="font-medium text-blue-800">Running Total | စုစုပေါင်း</span>
                      <div className="text-2xl font-bold text-blue-600">
                        {runningTotal.toLocaleString()} pcs
                      </div>
                    </div>
                    <p className="text-sm text-blue-600 mt-1">Multiple scans building pallet | များစွာစကင်လုပ်၍ ပိုင်တင်ဖွဲ့စည်းခြင်း</p>
                  </div>
                )}

                {/* Scanned Product Card */}
                {scannedProduct && (
                  <div className="bg-gradient-to-r from-green-50 to-green-100 rounded-lg p-4 border-2 border-green-300">
                    <h3 className="font-semibold text-green-800 mb-3">Scanned Product | စကင်လုပ်ထားသော ထုတ်ကုန်</h3>
                    <div className="space-y-3">
                      <div>
                        <div className="font-medium text-lg">{scannedProduct.product}</div>
                        <div className="text-sm text-slate-600">{scannedProduct.productMM}</div>
                      </div>
                      <div className="grid grid-cols-2 gap-3 text-sm">
                        <div>
                          <span className="text-slate-500">Job ID:</span>
                          <div className="font-medium">{scannedProduct.jobId}</div>
                        </div>
                        <div>
                          <span className="text-slate-500">Released Qty:</span>
                          <div className="font-medium text-blue-600">{scannedProduct.qtyReleased.toLocaleString()}</div>
                        </div>
                        <div>
                          <span className="text-slate-500">Pending Qty:</span>
                          <div className="font-medium text-orange-600">{scannedProduct.pendingQty.toLocaleString()}</div>
                        </div>
                      </div>

                      {/* Accept Form */}
                      <div className="space-y-3 pt-3 border-t border-green-300">
                        <div>
                          <Label>Accept Qty | လက်ခံအရေအတွက်</Label>
                          <Input
                            type="number"
                            value={acceptedQty}
                            onChange={(e) => setAcceptedQty(e.target.value)}
                            placeholder="Default = remaining"
                            className="bg-white h-12 text-lg"
                          />
                          <p className="text-xs text-slate-500 mt-1">Default = remaining | မူလအတိုင်း = ကျန်ရှိ</p>
                        </div>

                        <div className="grid grid-cols-2 gap-2">
                          <div>
                            <Label>Rack | စင်</Label>
                            <Select value={selectedRack} onValueChange={setSelectedRack}>
                              <SelectTrigger className="bg-white">
                                <SelectValue placeholder="Optional" />
                              </SelectTrigger>
                              <SelectContent>
                                {mockRacks.map((rack) => (
                                  <SelectItem key={rack} value={rack}>{rack}</SelectItem>
                                ))}
                              </SelectContent>
                            </Select>
                          </div>
                          <div>
                            <Label>Bin | ပုံး</Label>
                            <Select value={selectedBin} onValueChange={setSelectedBin}>
                              <SelectTrigger className="bg-white">
                                <SelectValue placeholder="Optional" />
                              </SelectTrigger>
                              <SelectContent>
                                {mockBins.map((bin) => (
                                  <SelectItem key={bin} value={bin}>{bin}</SelectItem>
                                ))}
                              </SelectContent>
                            </Select>
                          </div>
                        </div>

                        <div>
                          <Label>Note | မှတ်ချက်</Label>
                          <Input 
                            value={rejectReason}
                            onChange={(e) => setRejectReason(e.target.value)}
                            placeholder="Optional note"
                            className="bg-white"
                          />
                        </div>

                        <div className="flex gap-2">
                          <Button 
                            onClick={handleAccept}
                            className="bg-green-600 hover:bg-green-700 flex-1 h-12"
                          >
                            <CheckCircle className="h-4 w-4 mr-2" />
                            Accept | လက်ခံမည်
                          </Button>
                          <Button 
                            variant="outline"
                            onClick={() => {
                              setScannedProduct(null);
                              setAcceptedQty('');
                              setSelectedRack('');
                              setSelectedBin('');
                              setRejectReason('');
                            }}
                            className="h-12"
                          >
                            Cancel | ပယ်ဖျက်မည်
                          </Button>
                        </div>
                      </div>
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>
          </div>

          {/* Right Column - Queue & History */}
          <div className="xl:col-span-2 space-y-6">
            {/* Top Filters */}
            <Card className="border-0 shadow-sm">
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle className="flex items-center gap-2">
                    <Filter className="h-5 w-5 text-slate-600" />
                    <span>Filters | စစ်���ုတ်မှု</span>
                  </CardTitle>
                  <Button variant="outline" size="sm">
                    <RefreshCw className="h-4 w-4 mr-2" />
                    Refresh | ပြန်လည်ဖွင့်ရန်
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                  <div>
                    <Label>Release ID / Job ID</Label>
                    <Input placeholder="Search..." className="bg-white" />
                  </div>
                  <div>
                    <Label>Product | ထုတ်ကုန်</Label>
                    <Input placeholder="Search product" className="bg-white" />
                  </div>
                  <div>
                    <Label>QC Status | QC အခြေအနေ</Label>
                    <Select defaultValue="approved">
                      <SelectTrigger className="bg-white">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="all">All Statuses</SelectItem>
                        <SelectItem value="approved">Approved | အတည်ပြုပြီး</SelectItem>
                        <SelectItem value="pending">Pending | ဆိုင်းငံ့</SelectItem>
                        <SelectItem value="rejected">Rejected | ငြင်းပယ်</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                  <div>
                    <Label>Date Range | ရက်စွဲအပိုင်းအခြား</Label>
                    <Input type="date" className="bg-white" defaultValue="2025-09-07" />
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Intake Queue Table */}
            <Card className="border-0 shadow-sm">
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Package className="h-5 w-5 text-green-600" />
                  <span>Intake Queue Table | လက်ခံစောင့်ရန်စာရင်း</span>
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="overflow-x-auto">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Release ID | ထုတ်ပေးကုဒ်</TableHead>
                        <TableHead>Job ID | အလုပ်ကုဒ်</TableHead>
                        <TableHead>Product (EN/MM)</TableHead>
                        <TableHead>Qty Released</TableHead>
                        <TableHead>QC Status</TableHead>
                        <TableHead>Released By</TableHead>
                        <TableHead>Released Time</TableHead>
                        <TableHead>Pending Qty</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {mockReleasedBatches.map((batch) => (
                        <TableRow key={batch.releaseId} className="hover:bg-slate-50">
                          <TableCell className="font-medium font-mono">{batch.releaseId}</TableCell>
                          <TableCell className="font-medium font-mono">{batch.jobId}</TableCell>
                          <TableCell>
                            <div>
                              <div className="font-medium">{batch.product}</div>
                              <div className="text-sm text-slate-500">{batch.productMM}</div>
                            </div>
                          </TableCell>
                          <TableCell className="font-medium text-blue-600">
                            {batch.qtyReleased.toLocaleString()} pcs
                          </TableCell>
                          <TableCell>
                            <Badge className="bg-green-100 text-green-800">
                              <CheckCircle className="h-3 w-3 mr-1" />
                              Approved | အတည်ပြုပြီး
                            </Badge>
                          </TableCell>
                          <TableCell>
                            <div className="flex items-center gap-1">
                              <User className="h-3 w-3 text-slate-400" />
                              {batch.releasedBy}
                            </div>
                          </TableCell>
                          <TableCell className="text-sm text-slate-600">{batch.releasedTime}</TableCell>
                          <TableCell className="font-medium text-orange-600">
                            {batch.pendingQty.toLocaleString()} pcs
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              </CardContent>
            </Card>

            {/* FG-WH Stock History */}
            <Card className="border-0 shadow-sm">
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Archive className="h-5 w-5 text-slate-600" />
                  <span>FG-WH Stock History | FG-WH စတော့မှတ်တမ်း</span>
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="overflow-x-auto">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Product | ထုတ်ကုန်</TableHead>
                        <TableHead>Accepted Qty</TableHead>
                        <TableHead>Label ID</TableHead>
                        <TableHead>Job ID</TableHead>
                        <TableHead>Accepted By</TableHead>
                        <TableHead>Date & Time</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {mockFGHistory.map((item) => (
                        <TableRow key={item.id}>
                          <TableCell>
                            <div>
                              <div className="font-medium">{item.product}</div>
                              <div className="text-sm text-slate-500">{item.productMM}</div>
                            </div>
                          </TableCell>
                          <TableCell className="font-medium text-green-600">
                            {item.acceptedQty.toLocaleString()} pcs
                          </TableCell>
                          <TableCell className="font-mono text-sm font-medium">
                            {item.labelId}
                          </TableCell>
                          <TableCell className="font-mono font-medium">
                            {item.jobId}
                          </TableCell>
                          <TableCell>
                            <div className="flex items-center gap-1">
                              <User className="h-3 w-3 text-slate-400" />
                              {item.acceptedBy}
                            </div>
                          </TableCell>
                          <TableCell className="text-sm text-slate-600">{item.dateTime}</TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
                <div className="mt-4 text-center">
                  <p className="text-sm text-slate-500">
                    <span className="text-green-600 font-medium">✓ Accepted to FG-WH | ကုန်ချော ဂိုဒေါင်သို့ လက်ခံပြီး</span>
                    {' • '}
                    <span className="text-orange-600 font-medium">◐ Partial Accept | အပိုင်းလိုက် လက်ခံ</span>
                  </p>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    );
  };

  const renderWarehouseRegistration = () => {

    // Mock warehouses data for registration
    const mockWarehouses = [
      { 
        code: 'RM-WH', 
        nameEN: 'Raw Material Warehouse', 
        nameMM: 'ကုန်ကြမ်းဂိုဒေါင်', 
        type: 'Physical',
        inputMethod: 'Manual',
        hasLocations: true,
        status: 'Active',
        lastUpdated: '2025-09-07',
        lastUpdatedBy: 'Admin User'
      },
      { 
        code: 'GLUE-WH', 
        nameEN: 'Glue Warehouse', 
        nameMM: 'ကော်စာ/ကော်ခဲဂိုဒေါင်', 
        type: 'Physical',
        inputMethod: 'Manual',
        hasLocations: false,
        status: 'Active',
        lastUpdated: '2025-09-06',
        lastUpdatedBy: 'Admin User'
      },
      { 
        code: 'PROD-WH', 
        nameEN: 'Production Warehouse', 
        nameMM: 'ထုတ်လုပ်မှုဂိုဒေါင်', 
        type: 'Virtual',
        inputMethod: 'QR',
        hasLocations: false,
        status: 'Active',
        lastUpdated: '2025-09-05',
        lastUpdatedBy: 'Supervisor'
      },
      { 
        code: 'FG-WH', 
        nameEN: 'Finished Goods Warehouse', 
        nameMM: 'ကုန်ချောဂိုဒေါင်', 
        type: 'Physical',
        inputMethod: 'RFID',
        hasLocations: true,
        status: 'Active',
        lastUpdated: '2025-09-04',
        lastUpdatedBy: 'Admin User'
      },
      { 
        code: 'OLD-WH', 
        nameEN: 'Old Storage', 
        nameMM: 'ဟောင်းကျသောဂိုဒေါင်', 
        type: 'Physical',
        inputMethod: 'Manual',
        hasLocations: false,
        status: 'Inactive',
        lastUpdated: '2025-08-15',
        lastUpdatedBy: 'Admin User'
      }
    ];

    const handleAddWarehouse = () => {
      setEditingWarehouse(null);
      setFormData(getInitialFormData());
      setShowAddEditModal(true);
    };

    const handleEditWarehouse = (warehouse: any) => {
      setEditingWarehouse(warehouse);
      setFormData({
        code: String(warehouse.code || ''),
        nameEN: String(warehouse.nameEN || ''),
        nameMM: String(warehouse.nameMM || ''),
        type: (warehouse.type || 'Physical') as 'Physical' | 'Virtual',
        inputMethod: (warehouse.inputMethod || 'Manual') as 'QR' | 'RFID' | 'Manual',
        enableLocations: Boolean(warehouse.hasLocations),
        status: warehouse.status === 'Active',
        notes: String(warehouse.notes || ''),
        locations: [{ rack: '', bin: '', capacity: '' }]
      });
      setShowAddEditModal(true);
    };

    const handleSaveWarehouse = () => {
      // Validation
      if (!formData.code || !formData.nameEN || !formData.nameMM) {
        toast.error('Please fill in all required fields');
        return;
      }
      
      // Check for duplicate code (only for new warehouses)
      if (!editingWarehouse && mockWarehouses.some(w => w.code === formData.code.toUpperCase())) {
        toast.error('Warehouse code already exists');
        return;
      }

      // Save logic would go here
      toast.success(editingWarehouse ? 'Warehouse updated successfully' : 'Warehouse created successfully');
      setShowAddEditModal(false);
      // Reset form data to initial state
      setFormData(getInitialFormData());
      setEditingWarehouse(null);
    };

    const handleToggleStatus = (warehouse: any) => {
      const action = warehouse.status === 'Active' ? 'deactivated' : 'activated';
      toast.success(`Warehouse ${warehouse.code} ${action} successfully`);
    };

    const handleCloseModal = (open: boolean) => {
      setShowAddEditModal(open);
      if (!open) {
        // Reset form when closing modal
        setFormData(getInitialFormData());
        setEditingWarehouse(null);
      }
    };

    const addLocationRow = () => {
      setFormData(prev => ({
        ...prev,
        locations: [...(prev.locations || []), { rack: '', bin: '', capacity: '' }]
      }));
    };

    const removeLocationRow = (index: number) => {
      setFormData(prev => ({
        ...prev,
        locations: (prev.locations || []).filter((_, i) => i !== index)
      }));
    };

    const updateLocationRow = (index: number, field: string, value: string) => {
      setFormData(prev => ({
        ...prev,
        locations: (prev.locations || []).map((loc, i) => 
          i === index ? { 
            ...loc, 
            [field]: String(value || '') 
          } : {
            rack: String(loc.rack || ''),
            bin: String(loc.bin || ''),
            capacity: String(loc.capacity || '')
          }
        )
      }));
    };

    // Filter warehouses
    const filteredWarehouses = mockWarehouses.filter(warehouse => {
      const matchesSearch = 
        warehouse.code.toLowerCase().includes(warehouseSearchTerm.toLowerCase()) ||
        warehouse.nameEN.toLowerCase().includes(warehouseSearchTerm.toLowerCase()) ||
        warehouse.nameMM.toLowerCase().includes(warehouseSearchTerm.toLowerCase());
      
      const matchesType = typeFilter === 'all' || warehouse.type === typeFilter;
      const matchesStatus = statusFilter === 'all' || warehouse.status === statusFilter;
      const matchesInputMethod = inputMethodFilter === 'all' || warehouse.inputMethod === inputMethodFilter;
      
      return matchesSearch && matchesType && matchesStatus && matchesInputMethod;
    });

    return (
      <div className="space-y-6">
        {/* Page Title */}
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-semibold text-slate-900">
              Warehouse Registration / ဂိုဒေါင်မှတ်ပုံတင်
            </h1>
            <p className="text-slate-600 mt-1">Register and manage warehouse information and configurations</p>
          </div>
          <Button 
            onClick={handleAddWarehouse}
            className="bg-blue-600 hover:bg-blue-700"
          >
            <Plus className="h-4 w-4 mr-2" />
            Add Warehouse / ဂိုဒေါင်အသစ်ထည့်ရန်
          </Button>
        </div>

        {/* Search and Filters */}
        <Card>
          <CardContent className="p-6">
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
              <div className="relative">
                <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
                <Input
                  placeholder="Search by code or name..."
                  value={warehouseSearchTerm}
                  onChange={(e) => setWarehouseSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>
              <Select value={typeFilter} onValueChange={setTypeFilter}>
                <SelectTrigger>
                  <SelectValue placeholder="Filter by type" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Types</SelectItem>
                  <SelectItem value="Physical">Physical / အကောင်အထည်ရှိ</SelectItem>
                  <SelectItem value="Virtual">Virtual / သဘောတရား</SelectItem>
                </SelectContent>
              </Select>
              <Select value={statusFilter} onValueChange={setStatusFilter}>
                <SelectTrigger>
                  <SelectValue placeholder="Filter by status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Status</SelectItem>
                  <SelectItem value="Active">Active / အသုံးပြု</SelectItem>
                  <SelectItem value="Inactive">Inactive / ပိတ်ထား</SelectItem>
                </SelectContent>
              </Select>
              <Select value={inputMethodFilter} onValueChange={setInputMethodFilter}>
                <SelectTrigger>
                  <SelectValue placeholder="Filter by input method" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Methods</SelectItem>
                  <SelectItem value="QR">QR</SelectItem>
                  <SelectItem value="RFID">RFID</SelectItem>
                  <SelectItem value="Manual">Manual</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </CardContent>
        </Card>

        {/* Warehouse List */}
        {filteredWarehouses.length > 0 ? (
          <Card>
            <CardContent className="p-0">
              <div className="overflow-hidden rounded-lg border">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Code / ကုဒ်</TableHead>
                      <TableHead>Name (EN / MM) / အမည်</TableHead>
                      <TableHead>Type / အမျိုးအစား</TableHead>
                      <TableHead>Input Method / ထည့်သွင်းနည်း</TableHead>
                      <TableHead>Locations/Bins / တည်နေရာ</TableHead>
                      <TableHead>Status / အခြေအနေ</TableHead>
                      <TableHead>Last Updated / နောက်ဆုံးပြင်ဆင်</TableHead>
                      <TableHead>Actions / လုပ်ဆောင်ချက်များ</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {filteredWarehouses.map((warehouse) => (
                      <TableRow key={warehouse.code}>
                        <TableCell className="font-medium">{warehouse.code}</TableCell>
                        <TableCell>
                          <div>
                            <div className="font-medium">{warehouse.nameEN}</div>
                            <div className="text-sm text-muted-foreground">{warehouse.nameMM}</div>
                          </div>
                        </TableCell>
                        <TableCell>
                          <Badge variant={warehouse.type === 'Physical' ? 'default' : 'secondary'}>
                            {warehouse.type} / {warehouse.type === 'Physical' ? 'အကောင်အထည်ရှိ' : 'သဘောတရား'}
                          </Badge>
                        </TableCell>
                        <TableCell>{warehouse.inputMethod}</TableCell>
                        <TableCell>
                          {warehouse.hasLocations ? (
                            <Button variant="link" size="sm" className="p-0 h-auto">
                              Yes / ရှိ
                            </Button>
                          ) : (
                            <span className="text-muted-foreground">No / မရှိ</span>
                          )}
                        </TableCell>
                        <TableCell>
                          <Badge 
                            className={warehouse.status === 'Active' 
                              ? 'bg-green-100 text-green-800' 
                              : 'bg-red-100 text-red-800'
                            }
                          >
                            {warehouse.status} / {warehouse.status === 'Active' ? 'အသုံးပြု' : 'ပိတ်ထား'}
                          </Badge>
                        </TableCell>
                        <TableCell>
                          <div>
                            <div className="text-sm">{warehouse.lastUpdated}</div>
                            <div className="text-xs text-muted-foreground">{warehouse.lastUpdatedBy}</div>
                          </div>
                        </TableCell>
                        <TableCell>
                          <div className="flex gap-2">
                            <Button 
                              variant="outline" 
                              size="sm"
                              onClick={() => handleEditWarehouse(warehouse)}
                            >
                              <Edit className="h-3 w-3 mr-1" />
                              Edit
                            </Button>
                            <Button 
                              variant="outline" 
                              size="sm"
                              onClick={() => handleToggleStatus(warehouse)}
                              className={warehouse.status === 'Active' ? 'text-red-600' : 'text-green-600'}
                            >
                              {warehouse.status === 'Active' ? 'Deactivate' : 'Activate'}
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
        ) : (
          // Empty State
          <Card>
            <CardContent className="p-12 text-center">
              <div className="flex flex-col items-center space-y-4">
                <div className="w-16 h-16 bg-slate-100 rounded-full flex items-center justify-center">
                  <Warehouse className="h-8 w-8 text-slate-400" />
                </div>
                <div>
                  <h3 className="text-lg font-medium text-slate-900 mb-2">
                    No warehouses found
                  </h3>
                  <p className="text-slate-600 mb-4">
                    {warehouseSearchTerm || typeFilter !== 'all' || statusFilter !== 'all' || inputMethodFilter !== 'all'
                      ? 'No warehouses match your current filters. Try adjusting your search criteria.'
                      : 'Add your first warehouse to start using inventory.'
                    }
                  </p>
                  <p className="text-sm text-slate-500 mb-6">
                    {warehouseSearchTerm || typeFilter !== 'all' || statusFilter !== 'all' || inputMethodFilter !== 'all'
                      ? 'သင်၏လက်ရှိစစ်ထုတ်မှုနှင့် ကိုက်ညီသောဂိုဒေါင်မရှိပါ။ ရှာဖွေမှုနည်းလမ်းများကို ပြင်ဆင်ကြည့်ပါ။'
                      : 'စတော့ခ်ဝန်ဆောင်မှုကို စတင်အသုံးပြုရန် သင်၏ပထမဆုံးဂိုဒေါင်ကို ထည့်ပါ။'
                    }
                  </p>
                  {!warehouseSearchTerm && typeFilter === 'all' && statusFilter === 'all' && inputMethodFilter === 'all' && (
                    <Button onClick={handleAddWarehouse} className="bg-blue-600 hover:bg-blue-700">
                      <Plus className="h-4 w-4 mr-2" />
                      Add Warehouse / ဂိုဒေါင်ထည့်ရန်
                    </Button>
                  )}
                </div>
              </div>
            </CardContent>
          </Card>
        )}

        {/* Add/Edit Modal */}
        <Dialog open={showAddEditModal} onOpenChange={handleCloseModal}>
          <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle>
                {editingWarehouse ? 'Edit Warehouse / ဂိုဒေါင်ပြင်ဆင်ရန်' : 'Add Warehouse / ဂိုဒေါင်အသစ်ထည့်ရန်'}
              </DialogTitle>
              <DialogDescription>
                {editingWarehouse 
                  ? 'Update warehouse information and settings / ဂိုဒေါင်အချက်အလက်များနှင့် ဆက်တင်များကို ပြင်ဆင်ပါ'
                  : 'Register a new warehouse with all required information / လိုအပ်သောအချက်အလက်အားလုံးဖြင့် ဂိုဒေါင်အသစ်တစ်ခုကို မှတ်ပုံတင်ပါ'
                }
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-6 py-4">
              {/* Basic Information */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="code">Warehouse Code / ကုဒ် *</Label>
                  <Input
                    id="code"
                    placeholder="e.g., RM-WH"
                    value={formData.code}
                    onChange={(e) => setFormData(prev => ({ ...prev, code: e.target.value.toUpperCase() }))}
                    disabled={!!editingWarehouse}
                  />
                  <p className="text-xs text-muted-foreground mt-1">Unique identifier, uppercase</p>
                </div>
                <div>
                  <Label htmlFor="status-toggle">Status / အခြေအနေ</Label>
                  <div className="flex items-center space-x-2 mt-2">
                    <Switch
                      id="status-toggle"
                      checked={formData.status}
                      onCheckedChange={(checked) => setFormData(prev => ({ ...prev, status: checked }))}
                    />
                    <Label htmlFor="status-toggle">
                      {formData.status ? 'Active / အသုံးပြု' : 'Inactive / ပိတ်ထား'}
                    </Label>
                  </div>
                </div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="nameEN">English Name / အင်္ဂလိပ်အမည် *</Label>
                  <Input
                    id="nameEN"
                    placeholder="e.g., Raw Material Warehouse"
                    value={formData.nameEN}
                    onChange={(e) => setFormData(prev => ({ ...prev, nameEN: e.target.value }))}
                  />
                </div>
                <div>
                  <Label htmlFor="nameMM">Myanmar Name / မြန်မာအမည် *</Label>
                  <Input
                    id="nameMM"
                    placeholder="e.g., ကုန်ကြမ်းဂိုဒေါင်"
                    value={formData.nameMM}
                    onChange={(e) => setFormData(prev => ({ ...prev, nameMM: e.target.value }))}
                  />
                </div>
              </div>

              {/* Type Selection */}
              <div>
                <Label>Type / အမျိုးအစား *</Label>
                <div className="flex space-x-4 mt-2">
                  <div className="flex items-center space-x-2">
                    <input
                      type="radio"
                      id="physical"
                      name="type"
                      value="Physical"
                      checked={formData.type === 'Physical'}
                      onChange={(e) => setFormData(prev => ({ ...prev, type: e.target.value }))}
                    />
                    <Label htmlFor="physical">Physical / အကောင်အထည်ရှိ</Label>
                  </div>
                  <div className="flex items-center space-x-2">
                    <input
                      type="radio"
                      id="virtual"
                      name="type"
                      value="Virtual"
                      checked={formData.type === 'Virtual'}
                      onChange={(e) => setFormData(prev => ({ ...prev, type: e.target.value }))}
                    />
                    <Label htmlFor="virtual">Virtual / သဘောတရား</Label>
                  </div>
                </div>
              </div>

              {/* Input Method */}
              <div>
                <Label htmlFor="inputMethod">Default Input Method / ထည့်သွင်းနည်း *</Label>
                <Select value={formData.inputMethod} onValueChange={(value) => setFormData(prev => ({ ...prev, inputMethod: value }))}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="QR">QR</SelectItem>
                    <SelectItem value="RFID">RFID</SelectItem>
                    <SelectItem value="Manual">Manual</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              {/* Locations/Bins Toggle */}
              {formData.type === 'Physical' && (
                <div>
                  <div className="flex items-center space-x-2">
                    <Switch
                      id="locations-toggle"
                      checked={formData.enableLocations}
                      onCheckedChange={(checked) => setFormData(prev => ({ ...prev, enableLocations: checked }))}
                    />
                    <Label htmlFor="locations-toggle">
                      Enable Locations/Bins? / တည်နေရာ (Rack/Bin) အသုံးပြုမလား
                    </Label>
                  </div>

                  {/* Locations Sub-table */}
                  {formData.enableLocations && (
                    <div className="mt-4 p-4 border rounded-lg bg-slate-50">
                      <div className="flex justify-between items-center mb-4">
                        <Label>Location Configuration</Label>
                        <Button type="button" variant="outline" size="sm" onClick={addLocationRow}>
                          <Plus className="h-3 w-3 mr-1" />
                          Add Row
                        </Button>
                      </div>
                      <div className="space-y-2">
                        {(formData.locations || []).map((location, index) => (
                          <div key={index} className="grid grid-cols-4 gap-2 items-center">
                            <Input
                              placeholder="Rack"
                              value={location.rack || ''}
                              onChange={(e) => updateLocationRow(index, 'rack', e.target.value)}
                            />
                            <Input
                              placeholder="Bin"
                              value={location.bin || ''}
                              onChange={(e) => updateLocationRow(index, 'bin', e.target.value)}
                            />
                            <Input
                              placeholder="Capacity (optional)"
                              value={location.capacity || ''}
                              onChange={(e) => updateLocationRow(index, 'capacity', e.target.value)}
                            />
                            {(formData.locations || []).length > 1 && (
                              <Button
                                type="button"
                                variant="outline"
                                size="sm"
                                onClick={() => removeLocationRow(index)}
                              >
                                <X className="h-3 w-3" />
                              </Button>
                            )}
                          </div>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              )}

              {/* Notes */}
              <div>
                <Label htmlFor="notes">Notes / မှတ်ချက် (optional)</Label>
                <Textarea
                  id="notes"
                  placeholder="Additional notes about this warehouse..."
                  value={formData.notes}
                  onChange={(e) => setFormData(prev => ({ ...prev, notes: e.target.value }))}
                  rows={3}
                />
              </div>

              {/* Modal Actions */}
              <div className="flex justify-end gap-2 pt-4">
                <Button variant="outline" onClick={() => handleCloseModal(false)}>
                  Cancel / ပယ်ဖျက်မည်
                </Button>
                <Button onClick={handleSaveWarehouse} className="bg-blue-600 hover:bg-blue-700">
                  Save / သိမ်းမည်
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
      </div>
    );
  };

  const renderWarehouseRoutes = () => {
    // Mock data for warehouse routes
    const mockRoutes = [
      {
        id: 'RT-001',
        fromWarehouse: 'RM-WH',
        toWarehouse: 'PROD-WH',
        active: true,
        autoApprove: false,
        approverRole: 'Production Supervisor',
        allowedCategories: ['Raw Materials', 'Pellets'],
        maxDailyQty: 5000,
        leadTimeMinutes: 15,
        crossCompany: false,
        transportMethod: 'Trolley',
        notes: 'Standard raw material transfer route'
      },
      {
        id: 'RT-002',
        fromWarehouse: 'PROD-WH',
        toWarehouse: 'FG-WH',
        active: true,
        autoApprove: true,
        approverRole: null,
        allowedCategories: ['Finished Goods'],
        maxDailyQty: null,
        leadTimeMinutes: 10,
        crossCompany: false,
        transportMethod: 'Manual',
        notes: 'Automated transfer for QC approved products'
      },
      {
        id: 'RT-003',
        fromWarehouse: 'FG-WH',
        toWarehouse: 'HQ-YANGON',
        active: true,
        autoApprove: false,
        approverRole: 'Dispatch Manager',
        allowedCategories: ['Finished Goods'],
        maxDailyQty: 10000,
        leadTimeMinutes: 120,
        crossCompany: true,
        transportMethod: 'Truck',
        notes: 'Cross-company dispatch to Yangon HQ'
      },
      {
        id: 'RT-004',
        fromWarehouse: 'GLUE-WH',
        toWarehouse: 'PROD-WH',
        active: false,
        autoApprove: false,
        approverRole: 'Chemical Handler',
        allowedCategories: ['Glue & Adhesive'],
        maxDailyQty: 200,
        leadTimeMinutes: 30,
        crossCompany: false,
        transportMethod: 'Other',
        notes: 'Temporary route - under maintenance'
      }
    ];



    const categoryOptions = [
      'Raw Materials',
      'Pellets', 
      'Cut Piece',
      'Finished Goods',
      'Spare Parts',
      'Glue & Adhesive',
      'Packaging Materials',
      'Quality Control Samples'
    ];

    const transportMethods = [
      'Truck',
      'Trolley', 
      'Manual',
      'Other'
    ];

    const approverRoles = [
      'Inventory Supervisor',
      'Production Manager',
      'Dispatch Manager',
      'Chemical Handler',
      'Quality Control Lead',
      'Factory Manager'
    ];

    const handleNewRoute = () => {
      setEditingRoute(null);
      setRouteFormData({
        fromWarehouse: '',
        toWarehouse: '',
        active: true,
        allowedCategories: [],
        maxDailyQty: '',
        autoApprove: false,
        approverRole: '',
        leadTimeMinutes: '',
        crossCompany: false,
        transportMethod: '',
        notes: ''
      });
      setShowNewRouteModal(true);
    };

    const handleEditRoute = (route: any) => {
      setEditingRoute(route);
      setRouteFormData({
        fromWarehouse: route.fromWarehouse,
        toWarehouse: route.toWarehouse,
        active: route.active,
        allowedCategories: route.allowedCategories,
        maxDailyQty: route.maxDailyQty?.toString() || '',
        autoApprove: route.autoApprove,
        approverRole: route.approverRole || '',
        leadTimeMinutes: route.leadTimeMinutes?.toString() || '',
        crossCompany: route.crossCompany,
        transportMethod: route.transportMethod,
        notes: route.notes
      });
      setShowNewRouteModal(true);
    };

    const handleSaveRoute = () => {
      // Validation
      if (!routeFormData.fromWarehouse || !routeFormData.toWarehouse) {
        toast.error('Please select both From and To warehouses');
        return;
      }
      
      if (routeFormData.fromWarehouse === routeFormData.toWarehouse) {
        toast.error('From and To warehouses cannot be the same');
        return;
      }

      // Check for duplicates (only for new routes)
      if (!editingRoute) {
        const duplicate = mockRoutes.find(route => 
          route.fromWarehouse === routeFormData.fromWarehouse && 
          route.toWarehouse === routeFormData.toWarehouse
        );
        if (duplicate) {
          toast.error('A route between these warehouses already exists');
          return;
        }
      }

      toast.success(editingRoute ? 'Route updated successfully' : 'Route created successfully');
      setShowNewRouteModal(false);
    };

    const handleToggleActive = (route: any) => {
      const action = route.active ? 'deactivated' : 'activated';
      toast.success(`Route ${route.id} ${action} successfully`);
    };

    const handleToggleAutoApprove = (route: any) => {
      const action = route.autoApprove ? 'disabled' : 'enabled';
      toast.success(`Auto-approve ${action} for route ${route.id}`);
    };

    // Filter routes
    const filteredRoutes = mockRoutes.filter(route => {
      const matchesFromWarehouse = filters.fromWarehouse === 'all' || route.fromWarehouse === filters.fromWarehouse;
      const matchesToWarehouse = filters.toWarehouse === 'all' || route.toWarehouse === filters.toWarehouse;
      const matchesActiveStatus = filters.activeStatus === 'all' || 
        (filters.activeStatus === 'active' && route.active) ||
        (filters.activeStatus === 'inactive' && !route.active);
      const matchesCategory = filters.category === 'all' || route.allowedCategories.includes(filters.category);
      
      return matchesFromWarehouse && matchesToWarehouse && matchesActiveStatus && matchesCategory;
    });

    return (
      <div className="space-y-6">
        {/* Header */}
        <div className="bg-gradient-to-r from-purple-50 to-indigo-50 rounded-lg p-6 border border-purple-200">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-2xl font-semibold text-slate-900">
                Warehouse Routes | ဂိုဒေါင်လမ်းကြောင်းများ
              </h1>
              <p className="text-slate-600 mt-2">
                Configure transfer routes and approval workflows between warehouses
              </p>
            </div>
            <div className="flex gap-3">
              <Button variant="outline" className="bg-white">
                <Download className="h-4 w-4 mr-2" />
                Export | ထုတ်ယူ
              </Button>
              <Button variant="outline" className="bg-white">
                <RefreshCw className="h-4 w-4 mr-2" />
                Refresh | ပြန်လည်ဖွင့်ရန်
              </Button>
              <Button onClick={handleNewRoute} className="bg-purple-600 hover:bg-purple-700">
                <Plus className="h-4 w-4 mr-2" />
                New Route | လမ်းကြောင်းအသစ်
              </Button>
            </div>
          </div>
        </div>

        {/* Filters */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Filter className="h-5 w-5 text-slate-600" />
              <span>Filters | စစ်ထုတ်မှု</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
              <div>
                <Label>Company | ကုမ္ပဏီ</Label>
                <Select value={filters.company} onValueChange={(value) => setFilters({...filters, company: value})}>
                  <SelectTrigger className="bg-white">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Companies</SelectItem>
                    <SelectItem value="internal">Internal Only</SelectItem>
                    <SelectItem value="cross-company">Cross Company</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label>From Warehouse | မှ ဂိုဒေါင်</Label>
                <Select value={filters.fromWarehouse} onValueChange={(value) => setFilters({...filters, fromWarehouse: value})}>
                  <SelectTrigger className="bg-white">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Warehouses</SelectItem>
                    {mockWarehouses.map((wh) => (
                      <SelectItem key={wh.id} value={wh.id}>{wh.id}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label>To Warehouse | သို့ ဂိုဒေါင���</Label>
                <Select value={filters.toWarehouse} onValueChange={(value) => setFilters({...filters, toWarehouse: value})}>
                  <SelectTrigger className="bg-white">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Warehouses</SelectItem>
                    {mockWarehouses.map((wh) => (
                      <SelectItem key={wh.id} value={wh.id}>{wh.id}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label>Status | အခြေအနေ</Label>
                <Select value={filters.activeStatus} onValueChange={(value) => setFilters({...filters, activeStatus: value})}>
                  <SelectTrigger className="bg-white">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Status</SelectItem>
                    <SelectItem value="active">Active | လုပ်ဆောင��နေ</SelectItem>
                    <SelectItem value="inactive">Inactive | ပိတ်ထား</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label>Category | အမျိုးအစား</Label>
                <Select value={filters.category} onValueChange={(value) => setFilters({...filters, category: value})}>
                  <SelectTrigger className="bg-white">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Categories</SelectItem>
                    {categoryOptions.map((cat) => (
                      <SelectItem key={cat} value={cat}>{cat}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Routes Table */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Route className="h-5 w-5 text-purple-600" />
              <span>Warehouse Routes | ဂိုဒေါင်လမ်းကြောင်းများ</span>
              <Badge className="ml-2">{filteredRoutes.length} routes</Badge>
            </CardTitle>
          </CardHeader>
          <CardContent>
            {filteredRoutes.length === 0 ? (
              <div className="text-center py-12">
                <Route className="h-12 w-12 text-slate-300 mx-auto mb-4" />
                <h3 className="text-lg font-medium text-slate-900 mb-2">No routes yet</h3>
                <p className="text-slate-500 mb-4">Create your first route to get started</p>
                <Button onClick={handleNewRoute} className="bg-purple-600 hover:bg-purple-700">
                  <Plus className="h-4 w-4 mr-2" />
                  Create First Route
                </Button>
              </div>
            ) : (
              <div className="overflow-x-auto">
                <TooltipProvider>
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>From Warehouse</TableHead>
                      <TableHead>→</TableHead>
                      <TableHead>To Warehouse</TableHead>
                      <TableHead>Active</TableHead>
                      <TableHead>Auto-Approve</TableHead>
                      <TableHead>Approver Role</TableHead>
                      <TableHead>Allowed Categories</TableHead>
                      <TableHead>Max Daily Qty</TableHead>
                      <TableHead>Lead Time</TableHead>
                      <TableHead>Cross-Company</TableHead>
                      <TableHead>Transport Method</TableHead>
                      <TableHead>Notes</TableHead>
                      <TableHead>Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {filteredRoutes.map((route) => (
                      <TableRow key={route.id} className="hover:bg-slate-50">
                        <TableCell className="font-medium">{route.fromWarehouse}</TableCell>
                        <TableCell>
                          <ArrowLeftRight className="h-4 w-4 text-slate-400" />
                        </TableCell>
                        <TableCell className="font-medium">{route.toWarehouse}</TableCell>
                        <TableCell>
                          <div className="flex items-center gap-2">
                            <Switch 
                              checked={route.active}
                              onCheckedChange={() => handleToggleActive(route)}
                            />
                            <Badge className={route.active ? "bg-green-100 text-green-800" : "bg-gray-100 text-gray-800"}>
                              {route.active ? 'Active' : 'Inactive'}
                            </Badge>
                          </div>
                        </TableCell>
                        <TableCell>
                          <div className="flex items-center gap-2">
                            <Tooltip>
                              <TooltipTrigger asChild>
                                <div>
                                  <Switch 
                                    checked={route.autoApprove}
                                    onCheckedChange={() => handleToggleAutoApprove(route)}
                                  />
                                </div>
                              </TooltipTrigger>
                              <TooltipContent>
                                {route.autoApprove 
                                  ? "Transfers are automatically approved without manual intervention"
                                  : "Transfers require manual approval from designated role"
                                }
                              </TooltipContent>
                            </Tooltip>
                            {!route.autoApprove && (
                              <span className="text-xs text-amber-600">Approval Required</span>
                            )}
                          </div>
                        </TableCell>
                        <TableCell>
                          {route.approverRole ? (
                            <span className="text-sm">{route.approverRole}</span>
                          ) : (
                            <span className="text-slate-400 text-sm">Auto-approved</span>
                          )}
                        </TableCell>
                        <TableCell>
                          <div className="flex flex-wrap gap-1">
                            {route.allowedCategories.map((cat) => (
                              <Badge key={cat} variant="outline" className="text-xs">
                                {cat}
                              </Badge>
                            ))}
                          </div>
                        </TableCell>
                        <TableCell>
                          {route.maxDailyQty ? (
                            <span className="text-sm font-medium">{route.maxDailyQty.toLocaleString()}</span>
                          ) : (
                            <span className="text-slate-400 text-sm">Unlimited</span>
                          )}
                        </TableCell>
                        <TableCell>
                          <span className="text-sm">{route.leadTimeMinutes} mins</span>
                        </TableCell>
                        <TableCell>
                          {route.crossCompany ? (
                            <Badge className="bg-orange-100 text-orange-800">Cross-Company</Badge>
                          ) : (
                            <Badge className="bg-blue-100 text-blue-800">Internal</Badge>
                          )}
                        </TableCell>
                        <TableCell>
                          <Badge variant="outline">{route.transportMethod}</Badge>
                        </TableCell>
                        <TableCell>
                          <span className="text-sm text-slate-600 max-w-32 truncate block">
                            {route.notes}
                          </span>
                        </TableCell>
                        <TableCell>
                          <Button 
                            variant="outline" 
                            size="sm"
                            onClick={() => handleEditRoute(route)}
                          >
                            <Edit className="h-3 w-3" />
                          </Button>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
                </TooltipProvider>
              </div>
            )}
          </CardContent>
        </Card>

        {/* New/Edit Route Modal */}
        <Dialog open={showNewRouteModal} onOpenChange={setShowNewRouteModal}>
          <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle>
                {editingRoute ? 'Edit Route | လမ်းကြောင်းပြင်ဆင်' : 'New Route | လမ်းကြောင်းအသစ်'}
              </DialogTitle>
              <DialogDescription>
                Configure warehouse transfer route and approval settings
              </DialogDescription>
            </DialogHeader>
            
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>From Warehouse | မှ ဂိုဒေါင် *</Label>
                  <Select 
                    value={routeFormData.fromWarehouse} 
                    onValueChange={(value) => setRouteFormData({...routeFormData, fromWarehouse: value})}
                  >
                    <SelectTrigger className="bg-white">
                      <SelectValue placeholder="Select source warehouse" />
                    </SelectTrigger>
                    <SelectContent>
                      {mockWarehouses.map((wh) => (
                        <SelectItem key={wh.id} value={wh.id}>{wh.id} - {wh.name}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <div>
                  <Label>To Warehouse | သို့ ဂိုဒေါင် *</Label>
                  <Select 
                    value={routeFormData.toWarehouse} 
                    onValueChange={(value) => setRouteFormData({...routeFormData, toWarehouse: value})}
                  >
                    <SelectTrigger className="bg-white">
                      <SelectValue placeholder="Select destination warehouse" />
                    </SelectTrigger>
                    <SelectContent>
                      {mockWarehouses.map((wh) => (
                        <SelectItem key={wh.id} value={wh.id}>{wh.id} - {wh.name}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div className="flex items-center gap-6">
                <div className="flex items-center space-x-2">
                  <Switch 
                    checked={routeFormData.active}
                    onCheckedChange={(checked) => setRouteFormData({...routeFormData, active: checked})}
                  />
                  <Label>Active | လုပ်ဆောင်နေ</Label>
                </div>
                <div className="flex items-center space-x-2">
                  <Switch 
                    checked={routeFormData.crossCompany}
                    onCheckedChange={(checked) => setRouteFormData({...routeFormData, crossCompany: checked})}
                  />
                  <Label>Cross-Company Transfer | ကုမ္ပဏီအချင်းချင်းလွှဲပြောင်း</Label>
                </div>
              </div>

              {routeFormData.crossCompany && (
                <Alert>
                  <AlertTriangle className="h-4 w-4" />
                  <AlertDescription>
                    Cross-company transfers require additional compliance documentation and approvals.
                  </AlertDescription>
                </Alert>
              )}

              <div>
                <Label>Allowed Categories | ခွင့်ပြုထားသောအမျိုးအစားများ *</Label>
                <div className="grid grid-cols-2 gap-2 mt-2 p-3 border rounded-lg bg-slate-50">
                  {categoryOptions.map((cat) => (
                    <div key={cat} className="flex items-center space-x-2">
                      <Checkbox 
                        checked={routeFormData.allowedCategories.includes(cat)}
                        onCheckedChange={(checked) => {
                          if (checked) {
                            setRouteFormData({
                              ...routeFormData, 
                              allowedCategories: [...routeFormData.allowedCategories, cat]
                            });
                          } else {
                            setRouteFormData({
                              ...routeFormData,
                              allowedCategories: routeFormData.allowedCategories.filter(c => c !== cat)
                            });
                          }
                        }}
                      />
                      <Label className="text-sm">{cat}</Label>
                    </div>
                  ))}
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Max Daily Qty | နေ့စဉ်အများဆုံးပမာဏ</Label>
                  <Input 
                    type="number"
                    value={routeFormData.maxDailyQty}
                    onChange={(e) => setRouteFormData({...routeFormData, maxDailyQty: e.target.value})}
                    placeholder="Optional (leave empty for unlimited)"
                    className="bg-white"
                  />
                </div>
                <div>
                  <Label>Lead Time (minutes) | ကြိုတင်အချိန်</Label>
                  <Input 
                    type="number"
                    value={routeFormData.leadTimeMinutes}
                    onChange={(e) => setRouteFormData({...routeFormData, leadTimeMinutes: e.target.value})}
                    placeholder="Time required for transfer"
                    className="bg-white"
                  />
                  <p className="text-xs text-slate-500 mt-1">SLA: Time required to complete transfer</p>
                </div>
              </div>

              <div className="flex items-center space-x-2">
                <Switch 
                  checked={routeFormData.autoApprove}
                  onCheckedChange={(checked) => setRouteFormData({...routeFormData, autoApprove: checked})}
                />
                <Label>Auto-Approve Transfers | အလိုအလျောက်အတည်ပြုမှု</Label>
              </div>

              {!routeFormData.autoApprove && (
                <div>
                  <Label>Approver Role | အတည်ပြုသူ Role *</Label>
                  <Select 
                    value={routeFormData.approverRole} 
                    onValueChange={(value) => setRouteFormData({...routeFormData, approverRole: value})}
                  >
                    <SelectTrigger className="bg-white">
                      <SelectValue placeholder="Select approver role" />
                    </SelectTrigger>
                    <SelectContent>
                      {approverRoles.map((role) => (
                        <SelectItem key={role} value={role}>{role}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <p className="text-xs text-slate-500 mt-1">
                    Transfers on this route will require approval from users with this role
                  </p>
                </div>
              )}

              <div>
                <Label>Transport Method | ပို့ဆောင်နည်းလမ်း</Label>
                <Select 
                  value={routeFormData.transportMethod} 
                  onValueChange={(value) => setRouteFormData({...routeFormData, transportMethod: value})}
                >
                  <SelectTrigger className="bg-white">
                    <SelectValue placeholder="Select transport method" />
                  </SelectTrigger>
                  <SelectContent>
                    {transportMethods.map((method) => (
                      <SelectItem key={method} value={method}>{method}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label>Notes | မှတ်ချက်များ</Label>
                <Textarea 
                  value={routeFormData.notes}
                  onChange={(e) => setRouteFormData({...routeFormData, notes: e.target.value})}
                  placeholder="Optional notes about this route..."
                  className="bg-white"
                />
              </div>

              <div className="flex justify-end gap-2 pt-4">
                <Button variant="outline" onClick={() => setShowNewRouteModal(false)}>
                  Cancel | ပယ်ဖျက်မည်
                </Button>
                <Button onClick={handleSaveRoute} className="bg-purple-600 hover:bg-purple-700">
                  Save Route | လမ်းကြောင်းသိမ်းမည်
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
      </div>
    );
  };

  const renderStockInOut = () => (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-green-50 to-emerald-50 rounded-lg p-6 border border-green-200">
        <h1 className="text-2xl font-semibold text-slate-900">
          Stock In / Stock Out | စတော့ဝင် / စတော့ထွက်
        </h1>
        <p className="text-slate-600 mt-2">
          Manage inventory transfers, receipts, and movements between warehouses
        </p>
      </div>

      {/* Transfer Type Selection */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <ArrowLeftRight className="h-5 w-5 text-green-600" />
            <span>Transfer Operations | လွှဲပြောင်းမှုလုပ်ငန်းများ</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <Button 
              variant={transferMode === 'receive' ? 'default' : 'outline'}
              onClick={() => setTransferMode('receive')}
              className="h-20 flex-col gap-2"
            >
              <Download className="h-6 w-6" />
              <span>Stock In | စတော့ဝင်</span>
            </Button>
            <Button 
              variant={transferMode === 'transfer' ? 'default' : 'outline'}
              onClick={() => setTransferMode('transfer')}
              className="h-20 flex-col gap-2"
            >
              <ArrowLeftRight className="h-6 w-6" />
              <span>Transfer | လွှဲပြောင်း</span>
            </Button>
            <Button 
              variant={transferMode === 'issue' ? 'default' : 'outline'}
              onClick={() => setTransferMode('issue')}
              className="h-20 flex-col gap-2"
            >
              <Upload className="h-6 w-6" />
              <span>Stock Out | စတော့ထွက်</span>
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Transfer Form */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Edit className="h-5 w-5 text-blue-600" />
              <span>
                {transferMode === 'receive' ? 'Stock In Details | စတော့ဝင်အသေးစိတ်' :
                 transferMode === 'transfer' ? 'Transfer Details | လွှဲပြောင်းအသေးစိတ်' :
                 'Stock Out Details | စတော့ထွက်အသေးစိတ်'}
              </span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {transferMode !== 'receive' && (
                <div>
                  <Label>From Warehouse | မှ ဂိုဒေါင်</Label>
                  <Select value={fromWarehouse} onValueChange={setFromWarehouse}>
                    <SelectTrigger className="bg-white">
                      <SelectValue placeholder="Select source warehouse" />
                    </SelectTrigger>
                    <SelectContent>
                      {mockWarehouses.map((wh) => (
                        <SelectItem key={wh.id} value={wh.id}>{wh.id} - {wh.name}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              )}
              
              {transferMode !== 'issue' && (
                <div>
                  <Label>To Warehouse | သို့ ဂိုဒေါင်</Label>
                  <Select value={toWarehouse} onValueChange={setToWarehouse}>
                    <SelectTrigger className="bg-white">
                      <SelectValue placeholder="Select destination warehouse" />
                    </SelectTrigger>
                    <SelectContent>
                      {mockWarehouses.map((wh) => (
                        <SelectItem key={wh.id} value={wh.id}>{wh.id} - {wh.name}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              )}

              <div>
                <Label>Item / Product | ပစ္စည်း / ထုတ်ကုန်</Label>
                <Input 
                  value={selectedItem}
                  onChange={(e) => setSelectedItem(e.target.value)}
                  placeholder="Enter item name or scan barcode"
                  className="bg-white"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Quantity | အရေအတွက်</Label>
                  <Input 
                    type="number"
                    value={quantity}
                    onChange={(e) => setQuantity(e.target.value)}
                    placeholder="0"
                    className="bg-white"
                  />
                </div>
                <div>
                  <Label>Unit | ယူနစ်</Label>
                  <Select value={unit} onValueChange={setUnit}>
                    <SelectTrigger className="bg-white">
                      <SelectValue placeholder="Unit" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="pcs">PCS</SelectItem>
                      <SelectItem value="kg">KG</SelectItem>
                      <SelectItem value="liter">Liter</SelectItem>
                      <SelectItem value="box">Box</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div>
                <Label>Notes | မှတ်ချက်များ</Label>
                <Textarea 
                  value={notes}
                  onChange={(e) => setNotes(e.target.value)}
                  placeholder="Optional notes..."
                  className="bg-white"
                />
              </div>

              <div className="space-y-2">
                <Button 
                  className="w-full bg-green-600 hover:bg-green-700"
                  onClick={() => {
                    const confirmTitle = transferMode === 'receive' ? 'Confirm Receipt' : 
                                        transferMode === 'transfer' ? 'Confirm Transfer' : 
                                        'Confirm Issue';
                    const confirmTitleMM = transferMode === 'receive' ? 'လက်ခံအတည်ပြု' : 
                                          transferMode === 'transfer' ? 'လွှဲပြောင်းအတည်ပြု' : 
                                          'ထုတ်မည်အတည်ပြု';
                    const handler = transferMode === 'receive' ? handleStockInConfirm : 
                                   transferMode === 'transfer' ? handleTransferConfirm : 
                                   handleStockOutConfirm;
                    showMovementConfirmation(confirmTitle, confirmTitleMM, handler);
                  }}
                >
                  <CheckCircle className="h-4 w-4 mr-2" />
                  Confirm {transferMode === 'receive' ? 'Receipt' : transferMode === 'transfer' ? 'Transfer' : 'Issue'} | 
                  {transferMode === 'receive' ? ' လက်ခံမည်' : transferMode === 'transfer' ? ' လွှဲပြောင်းမည်' : ' ထုတ်ပေးမည်'}
                </Button>
                <p className="text-xs text-center text-slate-500">
                  Re-authentication required. | အတည်ပြုရန် စကားဝှက်လိုအပ်သည်။
                </p>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Recent Transactions */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Clock className="h-5 w-5 text-slate-600" />
              <span>Recent Transactions | လတ်တလောငွေလွှဲမှုများ</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {[
                { type: 'IN', item: 'PET Resin', qty: '500 kg', from: 'Supplier', to: 'RM-WH', time: '14:30' },
                { type: 'TRANSFER', item: 'Plastic Bottle', qty: '1000 pcs', from: 'PROD-WH', to: 'FG-WH', time: '13:45' },
                { type: 'OUT', item: 'Finished Goods', qty: '2000 pcs', from: 'FG-WH', to: 'HQ', time: '12:20' },
                { type: 'TRANSFER', item: 'Glue Adhesive', qty: '50 kg', from: 'GLUE-WH', to: 'PROD-WH', time: '11:15' }
              ].map((transaction, index) => (
                <div key={index} className="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
                  <div className="flex items-center gap-3">
                    <Badge className={
                      transaction.type === 'IN' ? 'bg-green-100 text-green-800' :
                      transaction.type === 'OUT' ? 'bg-red-100 text-red-800' :
                      'bg-blue-100 text-blue-800'
                    }>
                      {transaction.type}
                    </Badge>
                    <div>
                      <div className="font-medium text-sm">{transaction.item}</div>
                      <div className="text-xs text-slate-500">{transaction.qty}</div>
                    </div>
                  </div>
                  <div className="text-right">
                    <div className="text-xs text-slate-500">{transaction.from} → {transaction.to}</div>
                    <div className="text-xs text-slate-400">{transaction.time}</div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );

  const renderBorrowCenter = () => (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-orange-50 to-amber-50 rounded-lg p-6 border border-orange-200">
        <h1 className="text-2xl font-semibold text-slate-900">
          Borrow Center | ချေးယူရေးစင်တာ
        </h1>
        <p className="text-slate-600 mt-2">
          Manage material borrowing, lending, and return operations between departments and warehouses
        </p>
      </div>

      {/* Borrow Operations */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        {/* Quick Actions */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <RotateCcw className="h-5 w-5 text-orange-600" />
              <span>Quick Actions | လျင်မြန်သောလုပ်ဆောင်ချက်များ</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              <Button className="w-full bg-orange-600 hover:bg-orange-700 h-12">
                <Plus className="h-4 w-4 mr-2" />
                New Borrow Request | ချေးယူတောင်းခံချက်အသစ်
              </Button>
              <Button variant="outline" className="w-full h-12">
                <RotateCcw className="h-4 w-4 mr-2" />
                Return Items | ပစ္စည်းပြန်အပ်
              </Button>
              <Button variant="outline" className="w-full h-12">
                <Eye className="h-4 w-4 mr-2" />
                View Borrowings | ချေးယူမှုများကြည့်ရှု
              </Button>
            </div>
          </CardContent>
        </Card>

        {/* Active Borrowings */}
        <Card className="border-0 shadow-sm md:col-span-2">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Clock className="h-5 w-5 text-blue-600" />
              <span>Active Borrowings | လက်ရှိချေးယူမှုများ</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {[
                { id: 'BOR-001', item: 'PET Resin', qty: '100 kg', borrower: 'Production Line A', due: '2025-09-08', status: 'active' },
                { id: 'BOR-002', item: 'Cleaning Tools', qty: '5 sets', borrower: 'Maintenance Dept', due: '2025-09-09', status: 'overdue' },
                { id: 'BOR-003', item: 'Safety Equipment', qty: '10 pcs', borrower: 'QC Department', due: '2025-09-10', status: 'active' }
              ].map((borrow) => (
                <div key={borrow.id} className="flex items-center justify-between p-4 bg-slate-50 rounded-lg">
                  <div className="flex items-center gap-3">
                    <div>
                      <div className="font-medium">{borrow.item}</div>
                      <div className="text-sm text-slate-500">{borrow.id} • {borrow.qty}</div>
                    </div>
                  </div>
                  <div className="text-right">
                    <div className="text-sm">{borrow.borrower}</div>
                    <div className="flex items-center gap-2">
                      <span className="text-xs text-slate-500">Due: {borrow.due}</span>
                      <Badge className={borrow.status === 'active' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}>
                        {borrow.status === 'active' ? 'Active' : 'Overdue'}
                      </Badge>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Borrow Request Form */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <FileText className="h-5 w-5 text-green-600" />
            <span>New Borrow Request | ချေးယူတောင်းခံချက်အသစ်</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="space-y-4">
              <div>
                <Label>Borrower Department | ချေးယူသောဌာန</Label>
                <Select>
                  <SelectTrigger className="bg-white">
                    <SelectValue placeholder="Select department" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="production-a">Production Line A | ထုတ်လုပ်ရေးလိုင်း A</SelectItem>
                    <SelectItem value="production-b">Production Line B | ထုတ်လုပ်ရေးလိုင်း B</SelectItem>
                    <SelectItem value="qc">QC Department | QC ဌာန</SelectItem>
                    <SelectItem value="maintenance">Maintenance | ပြုပြင်ထိန်းသိမ်းရေး</SelectItem>
                    <SelectItem value="engineering">Engineering | အင်ဂျင်နီယာ</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label>Item to Borrow | ချေးယူမည့်ပစ္စည်း</Label>
                <Input placeholder="Enter item name" className="bg-white" />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Quantity | အရေအတွက်</Label>
                  <Input type="number" placeholder="0" className="bg-white" />
                </div>
                <div>
                  <Label>Unit | ယူနစ်</Label>
                  <Select>
                    <SelectTrigger className="bg-white">
                      <SelectValue placeholder="Unit" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="pcs">PCS</SelectItem>
                      <SelectItem value="kg">KG</SelectItem>
                      <SelectItem value="sets">Sets</SelectItem>
                      <SelectItem value="boxes">Boxes</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>
            </div>
            <div className="space-y-4">
              <div>
                <Label>Expected Return Date | ပြန်အပ်မည့်ရက်စွဲ</Label>
                <Input type="date" className="bg-white" />
              </div>
              <div>
                <Label>Purpose | ရည်ရွယ်ချက်</Label>
                <Textarea placeholder="Reason for borrowing..." className="bg-white" />
              </div>
              <Button className="w-full bg-orange-600 hover:bg-orange-700">
                <Send className="h-4 w-4 mr-2" />
                Submit Request | တောင်းခံချက်တင်သွင်းမည်
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const renderStockAdjustment = () => (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-red-50 to-rose-50 rounded-lg p-6 border border-red-200">
        <h1 className="text-2xl font-semibold text-slate-900">
          Stock Adjustment & Reallocation | စတော့ပြင်ဆင်မှုနှင့် ပြန်လည်ခွဲဝေမှု
        </h1>
        <p className="text-slate-600 mt-2">
          Adjust inventory levels, handle discrepancies, and reallocate stock between locations
        </p>
      </div>

      {/* Adjustment Types */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Calculator className="h-5 w-5 text-red-600" />
            <span>Adjustment Types | ပြင်ဆင်မှုအမျိုးအစားများ</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <Button variant="outline" className="h-20 flex-col gap-2">
              <TrendingUp className="h-5 w-5 text-green-600" />
              <span className="text-xs text-center">Stock Increase | စတော့တိုးမြှင့်</span>
            </Button>
            <Button variant="outline" className="h-20 flex-col gap-2">
              <TrendingDown className="h-5 w-5 text-red-600" />
              <span className="text-xs text-center">Stock Decrease | စတော့လျှော့ချ</span>
            </Button>
            <Button variant="outline" className="h-20 flex-col gap-2">
              <ArrowLeftRight className="h-5 w-5 text-blue-600" />
              <span className="text-xs text-center">Reallocation | ပြန်လည်ခွဲဝေ</span>
            </Button>
            <Button variant="outline" className="h-20 flex-col gap-2">
              <AlertTriangle className="h-5 w-5 text-orange-600" />
              <span className="text-xs text-center">Discrepancy | ကွာခြားမှု</span>
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Adjustment Form */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Edit className="h-5 w-5 text-blue-600" />
              <span>Stock Adjustment Form | စတော့ပြင်ဆင်မှုပုံစံ</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div>
                <Label>Adjustment Type | ပြင်ဆင်မှုအမျိုးအစား</Label>
                <Select>
                  <SelectTrigger className="bg-white">
                    <SelectValue placeholder="Select adjustment type" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="increase">Stock Increase | စတော့တိုးမြှင့်</SelectItem>
                    <SelectItem value="decrease">Stock Decrease | စတော့လျှော့ချ</SelectItem>
                    <SelectItem value="reallocation">Reallocation | ပြန်လည်ခွဲဝေ</SelectItem>
                    <SelectItem value="correction">Correction | မှန်ကန်စေရန်ပြင်ဆင်</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              
              <div>
                <Label>Warehouse | ဂိုဒေါင်</Label>
                <Select>
                  <SelectTrigger className="bg-white">
                    <SelectValue placeholder="Select warehouse" />
                  </SelectTrigger>
                  <SelectContent>
                    {mockWarehouses.map((wh) => (
                      <SelectItem key={wh.id} value={wh.id}>{wh.id} - {wh.name}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label>Item / Product | ပစ္စည်း / ထုတ်ကုန်</Label>
                <Input placeholder="Enter item name or scan barcode" className="bg-white" />
              </div>

              <div className="grid grid-cols-3 gap-4">
                <div>
                  <Label>Current Stock | လက်ရှိစတော့</Label>
                  <Input value="1,500" className="bg-slate-50" disabled />
                </div>
                <div>
                  <Label>Adjustment | ပြင်ဆင်မှု</Label>
                  <Input type="number" placeholder="±0" className="bg-white" />
                </div>
                <div>
                  <Label>New Stock | စတော့အသစ်</Label>
                  <Input value="1,500" className="bg-slate-50" disabled />
                </div>
              </div>

              <div>
                <Label>Reason Code | အကြောင်းရင်းကုဒ်</Label>
                <Select>
                  <SelectTrigger className="bg-white">
                    <SelectValue placeholder="Select reason" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="damaged">Damaged Stock | ပျက်စီးသောစတော့</SelectItem>
                    <SelectItem value="expired">Expired Material | သက်တမ်းကုန်ကြမ်း</SelectItem>
                    <SelectItem value="found">Found Stock | တွေ့ရှိသောစတော့</SelectItem>
                    <SelectItem value="count-error">Count Error | ရေတွက်မှားယွင်းမှု</SelectItem>
                    <SelectItem value="transfer">Transfer Adjustment | လွှဲပြောင်းမှုပြင်ဆင်</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label>Notes | မှတ်ချက်များ</Label>
                <Textarea placeholder="Explanation for adjustment..." className="bg-white" />
              </div>

              <div className="space-y-2">
                <Button 
                  className="w-full bg-red-600 hover:bg-red-700"
                  onClick={() => showMovementConfirmation(
                    'Confirm Adjustment',
                    'ပြင်ဆင်မှုအတည်ပြု',
                    (reason) => {
                      console.log('Stock Adjustment confirmed', reason);
                      toast.success('Stock adjustment completed successfully! | စတော့ပြင်ဆင်မှု အောင်မြင်စွာ ပြီးစီးပါပြီ။');
                    }
                  )}
                >
                  <CheckCircle className="h-4 w-4 mr-2" />
                  Submit Adjustment | ပြင်ဆင်မှုတင်သွင်းမည်
                </Button>
                <p className="text-xs text-center text-slate-500">
                  Re-authentication required. | အတည်ပြုရန် စကားဝှက်လိုအပ်သည်။
                </p>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Recent Adjustments */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Clock className="h-5 w-5 text-slate-600" />
              <span>Recent Adjustments | လတ်တလောပြင်ဆင်မှုများ</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {[
                { type: 'decrease', item: 'PET Resin', qty: '-50 kg', reason: 'Damaged', user: 'Ma Hnin', time: '14:30' },
                { type: 'increase', item: 'Safety Gloves', qty: '+100 pcs', reason: 'Found Stock', user: 'Ko Zaw', time: '13:45' },
                { type: 'reallocation', item: 'Plastic Bottles', qty: '500 pcs', reason: 'Transfer', user: 'Ma Thin', time: '12:20' },
                { type: 'decrease', item: 'Cleaning Solution', qty: '-5 L', reason: 'Expired', user: 'Ko Min', time: '11:15' }
              ].map((adj, index) => (
                <div key={index} className="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
                  <div className="flex items-center gap-3">
                    <Badge className={
                      adj.type === 'increase' ? 'bg-green-100 text-green-800' :
                      adj.type === 'decrease' ? 'bg-red-100 text-red-800' :
                      'bg-blue-100 text-blue-800'
                    }>
                      {adj.type === 'increase' ? '+' : adj.type === 'decrease' ? '-' : '↔'}
                    </Badge>
                    <div>
                      <div className="font-medium text-sm">{adj.item}</div>
                      <div className="text-xs text-slate-500">{adj.qty} • {adj.reason}</div>
                    </div>
                  </div>
                  <div className="text-right">
                    <div className="text-xs text-slate-500">{adj.user}</div>
                    <div className="text-xs text-slate-400">{adj.time}</div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );

  const renderStockCountAudit = () => (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-indigo-50 to-purple-50 rounded-lg p-6 border border-indigo-200">
        <h1 className="text-2xl font-semibold text-slate-900">
          Stock Count & Audit | စတော့ရေတွက်မှုနှင့် စစ်ဆေးမှု
        </h1>
        <p className="text-slate-600 mt-2">
          Conduct physical inventory counts, cycle counts, and audit inventory accuracy
        </p>
      </div>

      {/* Count Operations */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Calculator className="h-5 w-5 text-indigo-600" />
            <span>Count Operations | ရေတွက်မှုလုပ်ငန်းများ</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <Button className="h-20 flex-col gap-2 bg-indigo-600 hover:bg-indigo-700">
              <Package className="h-5 w-5" />
              <span className="text-xs text-center">Start Count | ရေတွက်မှုစတင်</span>
            </Button>
            <Button variant="outline" className="h-20 flex-col gap-2">
              <Search className="h-5 w-5" />
              <span className="text-xs text-center">Cycle Count | အစဉ်ရေတွက်</span>
            </Button>
            <Button variant="outline" className="h-20 flex-col gap-2">
              <Shield className="h-5 w-5" />
              <span className="text-xs text-center">Audit | စစ်ဆေးမှု</span>
            </Button>
            <Button variant="outline" className="h-20 flex-col gap-2">
              <BarChart3 className="h-5 w-5" />
              <span className="text-xs text-center">Reports | အစီရင်ခံစာ</span>
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Active Counts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Clock className="h-5 w-5 text-blue-600" />
              <span>Active Counts | လက်ရှိရေတွက်မှုများ</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {[
                { id: 'CNT-001', warehouse: 'RM-WH', type: 'Full Count', progress: 75, items: 45, counted: 34, status: 'in-progress' },
                { id: 'CNT-002', warehouse: 'FG-WH', type: 'Cycle Count', progress: 100, items: 20, counted: 20, status: 'completed' },
                { id: 'CNT-003', warehouse: 'PROD-WH', type: 'Audit Count', progress: 30, items: 15, counted: 4, status: 'in-progress' }
              ].map((count) => (
                <div key={count.id} className="p-4 bg-slate-50 rounded-lg">
                  <div className="flex items-center justify-between mb-2">
                    <div>
                      <div className="font-medium">{count.id}</div>
                      <div className="text-sm text-slate-500">{count.warehouse} • {count.type}</div>
                    </div>
                    <Badge className={count.status === 'completed' ? 'bg-green-100 text-green-800' : 'bg-blue-100 text-blue-800'}>
                      {count.status === 'completed' ? 'Completed' : 'In Progress'}
                    </Badge>
                  </div>
                  <div className="space-y-2">
                    <div className="flex justify-between text-sm">
                      <span>Progress:</span>
                      <span>{count.progress}%</span>
                    </div>
                    <div className="w-full bg-slate-200 rounded-full h-2">
                      <div className="bg-blue-600 h-2 rounded-full" style={{ width: `${count.progress}%` }}></div>
                    </div>
                    <div className="text-xs text-slate-500">
                      {count.counted} of {count.items} items counted
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Start New Count */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Plus className="h-5 w-5 text-green-600" />
              <span>Start New Count | ရေတွက်မှုအသစ်စတင်ရန်</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div>
                <Label>Count Type | ရေတွက်မှုအမျိုးအစား</Label>
                <Select>
                  <SelectTrigger className="bg-white">
                    <SelectValue placeholder="Select count type" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="full">Full Physical Count | အပြည့်အဝရုပ်ပိုင်းရေတွက်မှု</SelectItem>
                    <SelectItem value="cycle">Cycle Count | အစဉ်ရေတွက်မှု</SelectItem>
                    <SelectItem value="audit">Audit Count | စစ်ဆေးမှုရေတွက်မှု</SelectItem>
                    <SelectItem value="spot">Spot Check | ချက်ချင်းစစ်ဆေးမှု</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label>Warehouse | ဂိုဒေါင်</Label>
                <Select>
                  <SelectTrigger className="bg-white">
                    <SelectValue placeholder="Select warehouse" />
                  </SelectTrigger>
                  <SelectContent>
                    {mockWarehouses.map((wh) => (
                      <SelectItem key={wh.id} value={wh.id}>{wh.id} - {wh.name}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label>Count Date | ရေတွက်မည့်ရက်စွဲ</Label>
                <Input type="date" className="bg-white" />
              </div>

              <div>
                <Label>Assigned Counter | တာဝန်ပေးအပ်သောရေတွက်သူ</Label>
                <Select>
                  <SelectTrigger className="bg-white">
                    <SelectValue placeholder="Select counter" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="ma-hnin">Ma Hnin (INV01)</SelectItem>
                    <SelectItem value="ko-zaw">Ko Zaw (INV02)</SelectItem>
                    <SelectItem value="ma-thin">Ma Thin (INV03)</SelectItem>
                    <SelectItem value="ko-min">Ko Min (INV04)</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label>Instructions | ညွှန်ကြားချက်များ</Label>
                <Textarea placeholder="Special instructions for counting..." className="bg-white" />
              </div>

              <Button className="w-full bg-indigo-600 hover:bg-indigo-700">
                <Play className="h-4 w-4 mr-2" />
                Start Count Session | ရေတွက်မှုစတင်မည်
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Count Results */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <BarChart3 className="h-5 w-5 text-slate-600" />
            <span>Recent Count Results | လတ်တလောရေတွက်မှုရလဒ်များ</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Count ID | ရေတွက်မှုအမှတ်</TableHead>
                  <TableHead>Warehouse</TableHead>
                  <TableHead>Type</TableHead>
                  <TableHead>System Qty</TableHead>
                  <TableHead>Physical Qty</TableHead>
                  <TableHead>Variance</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Date</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {[
                  { id: 'CNT-001', warehouse: 'RM-WH', type: 'Full', system: 1500, physical: 1485, variance: -15, status: 'Adjusted', date: '2025-09-06' },
                  { id: 'CNT-002', warehouse: 'FG-WH', type: 'Cycle', system: 2000, physical: 2000, variance: 0, status: 'Matched', date: '2025-09-06' },
                  { id: 'CNT-003', warehouse: 'GLUE-WH', type: 'Audit', system: 850, physical: 860, variance: 10, status: 'Pending', date: '2025-09-05' }
                ].map((result) => (
                  <TableRow key={result.id}>
                    <TableCell className="font-medium font-mono">{result.id}</TableCell>
                    <TableCell>{result.warehouse}</TableCell>
                    <TableCell>{result.type}</TableCell>
                    <TableCell className="text-right">{result.system.toLocaleString()}</TableCell>
                    <TableCell className="text-right">{result.physical.toLocaleString()}</TableCell>
                    <TableCell className={`text-right font-medium ${result.variance > 0 ? 'text-green-600' : result.variance < 0 ? 'text-red-600' : 'text-slate-600'}`}>
                      {result.variance > 0 ? '+' : ''}{result.variance}
                    </TableCell>
                    <TableCell>
                      <Badge className={
                        result.status === 'Matched' ? 'bg-green-100 text-green-800' :
                        result.status === 'Adjusted' ? 'bg-blue-100 text-blue-800' :
                        'bg-yellow-100 text-yellow-800'
                      }>
                        {result.status}
                      </Badge>
                    </TableCell>
                    <TableCell className="text-sm text-slate-600">{result.date}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const renderInventoryReports = () => (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-slate-50 to-gray-50 rounded-lg p-6 border border-slate-200">
        <h1 className="text-2xl font-semibold text-slate-900">
          Inventory Reports | စတော့အစီရင်ခံစာများ
        </h1>
        <p className="text-slate-600 mt-2">
          Comprehensive inventory reporting and analytics for all warehouse operations
        </p>
      </div>

      {/* Report Categories */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {[
          { title: 'Stock Status', titleMM: 'စတော့အခြေအနေ', icon: Package, color: 'blue', count: 8 },
          { title: 'Movement Reports', titleMM: 'လှုပ်ရှားမှုအစီရင်ခံစာ', icon: ArrowLeftRight, color: 'green', count: 6 },
          { title: 'Audit Reports', titleMM: 'စစ်ဆေးမှုအစီရင်ခံစာ', icon: Shield, color: 'purple', count: 4 },
          { title: 'Analytics', titleMM: 'ခွဲခြမ်းစိတ်ဖြာမှု', icon: BarChart3, color: 'orange', count: 5 }
        ].map((category) => (
          <Card key={category.title} className="border-0 shadow-sm hover:shadow-md transition-shadow cursor-pointer">
            <CardHeader>
              <div className="flex items-center gap-3">
                <div className={`p-2 rounded-lg bg-${category.color}-100`}>
                  <category.icon className={`h-6 w-6 text-${category.color}-600`} />
                </div>
                <div>
                  <h3 className="font-semibold">{category.title}</h3>
                  <p className="text-sm text-slate-600">{category.titleMM}</p>
                </div>
              </div>
            </CardHeader>
            <CardContent>
              <div className="flex items-center justify-between">
                <span className="text-2xl font-bold text-slate-700">{category.count}</span>
                <span className="text-sm text-slate-500">Reports</span>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Quick Reports */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <BarChart3 className="h-5 w-5 text-blue-600" />
            <span>Quick Reports | လျင်မြန်သောအစီရင်ခံစာများ</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            {[
              'Current Stock Summary | လက်ရှိစတော့အချုပ်အချာ',
              'Low Stock Alert | နည်းစတော့သတိပ���းချက်',
              'Stock Movement | စ��ော့လှုပ်ရှားမှု',
              'Warehouse Utilization | ဂိုဒေါင်အသုံးပြုမှု'
            ].map((report) => (
              <Button key={report} variant="outline" className="h-16 text-xs p-2">
                {report}
              </Button>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Report Builder */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Settings className="h-5 w-5 text-green-600" />
            <span>Custom Report Builder | စိတ်ကြိုက်အစီရင်ခံစာပြုလုပ်ရန်</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="space-y-4">
              <div>
                <Label>Report Type | အစီရင်ခံစာအမျိုးအစား</Label>
                <Select>
                  <SelectTrigger className="bg-white">
                    <SelectValue placeholder="Select report type" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="stock-status">Stock Status Report | စတော့အခြေအနေအစီရင်ခံစာ</SelectItem>
                    <SelectItem value="movement">Stock Movement Report | စတော့လှုပ်ရှားမှုအစီရင်ခံစာ</SelectItem>
                    <SelectItem value="audit">Audit Report | စစ်ဆေးမှုအစီရင်ခံစာ</SelectItem>
                    <SelectItem value="aging">Stock Aging Report | စတော့ကြေးမုံအစီရင်ခံစာ</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label>Warehouse Filter | ဂိုဒေါင်စစ်ထုတ်ရန်</Label>
                <Select>
                  <SelectTrigger className="bg-white">
                    <SelectValue placeholder="All warehouses" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Warehouses | ဂိုဒေါင်အားလုံး</SelectItem>
                    {mockWarehouses.map((wh) => (
                      <SelectItem key={wh.id} value={wh.id}>{wh.id} - {wh.name}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>From Date | မှရက်စွဲ</Label>
                  <Input type="date" className="bg-white" />
                </div>
                <div>
                  <Label>To Date | သို့ရက်စွဲ</Label>
                  <Input type="date" className="bg-white" />
                </div>
              </div>
            </div>
            <div className="space-y-4">
              <div>
                <Label>Output Format | ထွက်ရှိမှုပုံစံ</Label>
                <div className="flex gap-2">
                  <Button variant="outline" className="flex-1">
                    <Download className="h-4 w-4 mr-2" />
                    PDF
                  </Button>
                  <Button variant="outline" className="flex-1">
                    <Download className="h-4 w-4 mr-2" />
                    Excel
                  </Button>
                  <Button variant="outline" className="flex-1">
                    <Eye className="h-4 w-4 mr-2" />
                    View
                  </Button>
                </div>
              </div>
              <Button className="w-full bg-blue-600 hover:bg-blue-700">
                <BarChart3 className="h-4 w-4 mr-2" />
                Generate Report | အစီရင်ခံစာထုတ်ပြန်မည်
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const renderUserPermissionsMatrix = () => (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-amber-50 to-yellow-50 rounded-lg p-6 border border-amber-200">
        <h1 className="text-2xl font-semibold text-slate-900">
          User Permissions Matrix | အသုံးပြုသူခွင့်ပြုချက်ဇယား
        </h1>
        <p className="text-slate-600 mt-2">
          Manage user access rights and permissions for inventory operations and warehouse functions
        </p>
      </div>

      {/* Permission Overview */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        {[
          { role: 'Warehouse Manager', roleMM: 'ဂိုဒေါင်မန်နေဂျာ', users: 3, permissions: 28, level: 'Full Access' },
          { role: 'Inventory Clerk', roleMM: 'စတော့စာရေး', users: 8, permissions: 15, level: 'Standard' },
          { role: 'Stock Counter', roleMM: 'စတော့ရေတွက်သူ', users: 5, permissions: 8, level: 'Limited' },
          { role: 'Viewer', roleMM: 'ကြည့်ရှုသူ', users: 12, permissions: 3, level: 'Read Only' }
        ].map((role) => (
          <Card key={role.role} className="border-0 shadow-sm">
            <CardHeader>
              <div className="flex items-center gap-2">
                <Shield className="h-5 w-5 text-amber-600" />
                <div>
                  <h3 className="font-semibold">{role.role}</h3>
                  <p className="text-sm text-slate-600">{role.roleMM}</p>
                </div>
              </div>
            </CardHeader>
            <CardContent>
              <div className="space-y-2 text-sm">
                <div className="flex justify-between">
                  <span>Users:</span>
                  <span className="font-medium">{role.users}</span>
                </div>
                <div className="flex justify-between">
                  <span>Permissions:</span>
                  <span className="font-medium">{role.permissions}</span>
                </div>
                <div className="flex justify-between">
                  <span>Level:</span>
                  <Badge className="text-xs">{role.level}</Badge>
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Permissions Matrix */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Users className="h-5 w-5 text-blue-600" />
            <span>Permissions Matrix | ခွင့်ပြုချက်ဇယား</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead className="min-w-[200px]">Function | လုပ်ငန်းစဉ်</TableHead>
                  <TableHead className="text-center">Manager</TableHead>
                  <TableHead className="text-center">Clerk</TableHead>
                  <TableHead className="text-center">Counter</TableHead>
                  <TableHead className="text-center">Viewer</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {[
                  { function: 'Warehouse Registration | ဂိုဒေါင်မှတ်ပုံတင်', manager: 'full', clerk: 'none', counter: 'none', viewer: 'none' },
                  { function: 'Stock In/Out | စတော့ဝင်/ထွက်', manager: 'full', clerk: 'full', counter: 'read', viewer: 'read' },
                  { function: 'Stock Adjustment | စတော့ပြင်ဆင်မှု', manager: 'full', clerk: 'limited', counter: 'none', viewer: 'read' },
                  { function: 'Stock Count | စတော့ရေတွက်မှု', manager: 'full', clerk: 'full', counter: 'full', viewer: 'read' },
                  { function: 'Finished Goods Intake | ကုန်ချောလက်ခံ', manager: 'full', clerk: 'full', counter: 'none', viewer: 'read' },
                  { function: 'Borrow Center | ချေးယူရေးစင်တာ', manager: 'full', clerk: 'limited', counter: 'none', viewer: 'read' },
                  { function: 'Inventory Reports | စတော့အစီရင်ခံစာ', manager: 'full', clerk: 'standard', counter: 'limited', viewer: 'read' },
                  { function: 'User Management | အသုံးပြုသူစီမံခန့်ခွဲမှု', manager: 'full', clerk: 'none', counter: 'none', viewer: 'none' }
                ].map((permission, index) => (
                  <TableRow key={index}>
                    <TableCell className="font-medium">{permission.function}</TableCell>
                    <TableCell className="text-center">
                      <Badge className={
                        permission.manager === 'full' ? 'bg-green-100 text-green-800' :
                        permission.manager === 'limited' ? 'bg-yellow-100 text-yellow-800' :
                        permission.manager === 'read' ? 'bg-blue-100 text-blue-800' :
                        'bg-gray-100 text-gray-800'
                      }>
                        {permission.manager === 'full' ? 'Full' :
                         permission.manager === 'limited' ? 'Limited' :
                         permission.manager === 'read' ? 'Read' : 'None'}
                      </Badge>
                    </TableCell>
                    <TableCell className="text-center">
                      <Badge className={
                        permission.clerk === 'full' ? 'bg-green-100 text-green-800' :
                        permission.clerk === 'limited' ? 'bg-yellow-100 text-yellow-800' :
                        permission.clerk === 'read' ? 'bg-blue-100 text-blue-800' :
                        'bg-gray-100 text-gray-800'
                      }>
                        {permission.clerk === 'full' ? 'Full' :
                         permission.clerk === 'limited' ? 'Limited' :
                         permission.clerk === 'read' ? 'Read' : 'None'}
                      </Badge>
                    </TableCell>
                    <TableCell className="text-center">
                      <Badge className={
                        permission.counter === 'full' ? 'bg-green-100 text-green-800' :
                        permission.counter === 'limited' ? 'bg-yellow-100 text-yellow-800' :
                        permission.counter === 'read' ? 'bg-blue-100 text-blue-800' :
                        'bg-gray-100 text-gray-800'
                      }>
                        {permission.counter === 'full' ? 'Full' :
                         permission.counter === 'limited' ? 'Limited' :
                         permission.counter === 'read' ? 'Read' : 'None'}
                      </Badge>
                    </TableCell>
                    <TableCell className="text-center">
                      <Badge className={
                        permission.viewer === 'full' ? 'bg-green-100 text-green-800' :
                        permission.viewer === 'limited' ? 'bg-yellow-100 text-yellow-800' :
                        permission.viewer === 'read' ? 'bg-blue-100 text-blue-800' :
                        'bg-gray-100 text-gray-800'
                      }>
                        {permission.viewer === 'full' ? 'Full' :
                         permission.viewer === 'limited' ? 'Limited' :
                         permission.viewer === 'read' ? 'Read' : 'None'}
                      </Badge>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>

      {/* User Management */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <User className="h-5 w-5 text-green-600" />
              <span>User Management | အသုံးပြုသူစီ��ံခန့်ခွဲမှု</span>
            </CardTitle>
            <Button className="bg-green-600 hover:bg-green-700">
              <Plus className="h-4 w-4 mr-2" />
              Add User | အသုံးပြုသူထည့်ရန်
            </Button>
          </div>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>User | အသုံးပြုသူ</TableHead>
                  <TableHead>Role | အခန်းကဏ္ဍ</TableHead>
                  <TableHead>Department | ဌာန</TableHead>
                  <TableHead>Last Access | နောက်ဆုံးဝင်ရောက်</TableHead>
                  <TableHead>Status | အခ��ေအနေ</TableHead>
                  <TableHead>Actions | လုပ်ဆောင်ချက်</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {[
                  { name: 'Ma Hnin Wai', userId: 'INV001', role: 'Warehouse Manager', dept: 'Inventory', lastAccess: '2025-09-07 16:30', status: 'active' },
                  { name: 'Ko Zaw Htet', userId: 'INV002', role: 'Inventory Clerk', dept: 'Inventory', lastAccess: '2025-09-07 15:45', status: 'active' },
                  { name: 'Ma Thin Thin', userId: 'INV003', role: 'Stock Counter', dept: 'Inventory', lastAccess: '2025-09-07 14:20', status: 'active' },
                  { name: 'Ko Min Khant', userId: 'SUP001', role: 'Viewer', dept: 'Supervisor', lastAccess: '2025-09-07 13:10', status: 'inactive' }
                ].map((user) => (
                  <TableRow key={user.userId}>
                    <TableCell>
                      <div>
                        <div className="font-medium">{user.name}</div>
                        <div className="text-sm text-slate-500">{user.userId}</div>
                      </div>
                    </TableCell>
                    <TableCell>{user.role}</TableCell>
                    <TableCell>{user.dept}</TableCell>
                    <TableCell className="text-sm text-slate-600">{user.lastAccess}</TableCell>
                    <TableCell>
                      <Badge className={user.status === 'active' ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}>
                        {user.status === 'active' ? 'Active' : 'Inactive'}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <Button variant="outline" size="sm">
                          <Edit className="h-3 w-3" />
                        </Button>
                        <Button variant="outline" size="sm">
                          <Shield className="h-3 w-3" />
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
    </div>
  );

  const renderHQDispatch = () => (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-teal-50 to-cyan-50 rounded-lg p-6 border border-teal-200">
        <h1 className="text-2xl font-semibold text-slate-900">
          HQ Logistic | ရုံးချုပ်ပို့ဆောင်မှု
        </h1>
        <p className="text-slate-600 mt-2">
          Manage finished goods dispatch to headquarters and track delivery status
        </p>
      </div>

      {/* Dispatch Overview */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        {[
          { title: 'Pending Dispatch', titleMM: 'ပေးပို့ရန်ကျန်ရှိ', count: 12, icon: Clock, color: 'orange' },
          { title: 'In Transit', titleMM: 'ပို့ဆောင်နေဆဲ', count: 8, icon: Truck, color: 'blue' },
          { title: 'Delivered', titleMM: 'ရောက်ရှိပြီး', count: 45, icon: CheckCircle, color: 'green' },
          { title: 'Total Shipments', titleMM: 'စုစုပေါင်းပေးပို့မှု', count: 65, icon: Package, color: 'slate' }
        ].map((stat) => (
          <Card key={stat.title} className="border-0 shadow-sm">
            <CardContent className="p-6">
              <div className="flex items-center gap-3">
                <div className={`p-3 rounded-lg bg-${stat.color}-100`}>
                  <stat.icon className={`h-6 w-6 text-${stat.color}-600`} />
                </div>
                <div>
                  <p className="text-2xl font-bold text-slate-900">{stat.count}</p>
                  <p className="text-sm text-slate-600">{stat.title}</p>
                  <p className="text-xs text-slate-500">{stat.titleMM}</p>
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Create Dispatch */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <Plus className="h-5 w-5 text-teal-600" />
              <span>Create New Dispatch | ပို့ဆောင်မှုအသစ်ဖန်တီးရန်</span>
            </CardTitle>
            <Button className="bg-teal-600 hover:bg-teal-700">
              <Truck className="h-4 w-4 mr-2" />
              Quick Dispatch | လျင်မြန်ပို့ဆောင်မှု
            </Button>
          </div>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div className="space-y-4">
              <div>
                <Label>Destination HQ | သွားရောက်မည့်ရုံးချုပ်</Label>
                <Select>
                  <SelectTrigger className="bg-white">
                    <SelectValue placeholder="Select HQ location" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="yangon-hq">Yangon HQ | ရန်ကုန်ရုံးချုပ်</SelectItem>
                    <SelectItem value="mandalay-hq">Mandalay HQ | မန္တလေးရုံးချုပ်</SelectItem>
                    <SelectItem value="naypyitaw-hq">Naypyitaw HQ | နေပြည်တော်ရုံးချုပ်</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label>Transport Method | ပို့ဆောင်နည်းလမ်း</Label>
                <Select>
                  <SelectTrigger className="bg-white">
                    <SelectValue placeholder="Select transport method" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="company-truck">Company Truck | ကုမ္ပဏီယာဉ်</SelectItem>
                    <SelectItem value="third-party">Third Party Logistics | ပြင်ပ ပို့ဆောင်ရေး</SelectItem>
                    <SelectItem value="courier">Courier Service | စာပို့ဝန်ဆောင်မှု</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Dispatch Date | ပို့ဆောင်ရက်စွဲ</Label>
                  <Input type="date" className="bg-white" />
                </div>
                <div>
                  <Label>Expected Delivery | ရောက်ရှိမည့်ရက်စွဲ</Label>
                  <Input type="date" className="bg-white" />
                </div>
              </div>
            </div>
            <div className="space-y-4">
              <div>
                <Label>Driver Information | ယာဉ်မောင်းအချက်အလက်</Label>
                <Input placeholder="Driver name and contact" className="bg-white" />
              </div>
              <div>
                <Label>Vehicle Number | ယာဉ်အမှတ်</Label>
                <Input placeholder="License plate number" className="bg-white" />
              </div>
              <div>
                <Label>Special Instructions | အထူးညွှန်ကြားချက်</Label>
                <Textarea placeholder="Delivery instructions..." className="bg-white" />
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Active Dispatches */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Truck className="h-5 w-5 text-blue-600" />
            <span>Active Dispatches | လက်ရှိပို့ဆောင်မှုများ</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Dispatch ID | ပို့ဆောင်မှုအမှတ်</TableHead>
                  <TableHead>Destination | သွားရောက်ရာ</TableHead>
                  <TableHead>Items | ပစ္စည်းများ</TableHead>
                  <TableHead>Dispatch Date | ပို့ဆောင်ရက်စွဲ</TableHead>
                  <TableHead>Status | အခြေအနေ</TableHead>
                  <TableHead>Driver | ယာဉ်မောင်း</TableHead>
                  <TableHead>Actions | လုပ်ဆောင်ချက်</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {[
                  { id: 'DIS-001', destination: 'Yangon HQ', items: '15 items', date: '2025-09-07', status: 'In Transit', driver: 'Ko Aung', vehicle: 'AA-1234' },
                  { id: 'DIS-002', destination: 'Mandalay HQ', items: '8 items', date: '2025-09-06', status: 'Pending', driver: 'Ma Su', vehicle: 'BB-5678' },
                  { id: 'DIS-003', destination: 'Naypyitaw HQ', items: '22 items', date: '2025-09-05', status: 'Delivered', driver: 'Ko Win', vehicle: 'CC-9012' }
                ].map((dispatch) => (
                  <TableRow key={dispatch.id}>
                    <TableCell className="font-medium font-mono">{dispatch.id}</TableCell>
                    <TableCell>{dispatch.destination}</TableCell>
                    <TableCell>{dispatch.items}</TableCell>
                    <TableCell>{dispatch.date}</TableCell>
                    <TableCell>
                      <Badge className={
                        dispatch.status === 'In Transit' ? 'bg-blue-100 text-blue-800' :
                        dispatch.status === 'Pending' ? 'bg-orange-100 text-orange-800' :
                        'bg-green-100 text-green-800'
                      }>
                        {dispatch.status}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div>
                        <div className="font-medium">{dispatch.driver}</div>
                        <div className="text-sm text-slate-500">{dispatch.vehicle}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <Button variant="outline" size="sm">
                          <Eye className="h-3 w-3" />
                        </Button>
                        <Button variant="outline" size="sm">
                          <Edit className="h-3 w-3" />
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
    </div>
  );

  const renderContent = () => {
    switch (currentPage) {
      case 'finished-goods-intake':
        return renderFinishedGoodsIntake();
      case 'warehouse-registration':
        return renderWarehouseRegistration();
      case 'warehouse-routes':
        return renderWarehouseRoutes();
      case 'warehouse-dashboard':
        return <WarehouseDashboard />;
      case 'stock-in-out':
        return renderStockInOut();
      case 'borrow-center':
        return renderBorrowCenter();
      case 'stock-adjustment':
        return renderStockAdjustment();
      case 'stock-count-audit':
        return renderStockCountAudit();
      case 'inventory-reports-main':
        return renderInventoryReports();
      case 'user-permissions-matrix':
        return renderUserPermissionsMatrix();
      case 'hq-dispatch':
        return renderHQDispatch();
      default:
        return (
          <div className="space-y-6">
            <div className="bg-gradient-to-r from-blue-50 to-indigo-50 rounded-lg p-6 border border-blue-200">
              <h1 className="text-2xl font-semibold text-slate-900">
                Inventory Module | ကုန်စာရင်းပိုင်းခြေ
              </h1>
              <p className="text-slate-600 mt-2">
                Select a function from the navigation to get started
              </p>
              <p className="text-sm text-slate-500 mt-1">
                စတင်ရန် ညွှန်ကြားမှုမှ လုပ်ဆောင်ချက်တစ်ခု ရွေးချယ်ပါ
              </p>
            </div>
          </div>
        );
    }
  };

  return (
    <div className="h-full bg-slate-50">
      {/* Main Content Area - Direct Content Rendering */}
      <div className="h-full overflow-y-auto bg-white">
        <div className="p-6">
          {renderContent()}
        </div>
      </div>
      
      {/* Movement Confirmation Modal */}
      <MovementConfirmationModal
        isOpen={showConfirmationModal}
        onClose={() => setShowConfirmationModal(false)}
        onConfirm={confirmationModalConfig.onConfirm}
        title={confirmationModalConfig.title}
        titleMM={confirmationModalConfig.titleMM}
      />
    </div>
  );
}