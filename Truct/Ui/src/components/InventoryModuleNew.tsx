import React, { useState } from 'react';
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
  Scan
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

  // Navigation is now handled by the main Layout component

  const renderDashboard = () => (
    <div className="space-y-6">
      {/* Quick Actions */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Package className="h-5 w-5 text-blue-600" />
            <span>Quick Actions | လျင်မြန်သောလုပ်ဆောင်ချက်များ</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-2 md:grid-cols-5 gap-4">
            <Button 
              onClick={() => onPageChange('stock-in-out')}
              className="h-20 flex-col gap-2 bg-blue-50 hover:bg-blue-100 text-blue-700 border-blue-200"
              variant="outline"
            >
              <ArrowLeftRight className="h-6 w-6" />
              <span className="text-xs text-center">Receive | လက်ခံ</span>
            </Button>
            <Button 
              onClick={() => onPageChange('stock-in-out')}
              className="h-20 flex-col gap-2 bg-green-50 hover:bg-green-100 text-green-700 border-green-200"
              variant="outline"
            >
              <Send className="h-6 w-6" />
              <span className="text-xs text-center">Transfer | ပြောင်းရွှေ့</span>
            </Button>
            <Button 
              onClick={() => onPageChange('hq-dispatch')}
              className="h-20 flex-col gap-2 bg-purple-50 hover:bg-purple-100 text-purple-700 border-purple-200"
              variant="outline"
            >
              <Truck className="h-6 w-6" />
              <span className="text-xs text-center">Dispatch | ပို့ဆောင်</span>
            </Button>
            <Button 
              onClick={() => onPageChange('borrow-center')}
              className="h-20 flex-col gap-2 bg-orange-50 hover:bg-orange-100 text-orange-700 border-orange-200"
              variant="outline"
            >
              <RotateCcw className="h-6 w-6" />
              <span className="text-xs text-center">Borrow | ချေးယူ</span>
            </Button>
            <Button 
              onClick={() => onPageChange('inventory-reports-main')}
              className="h-20 flex-col gap-2 bg-slate-50 hover:bg-slate-100 text-slate-700 border-slate-200"
              variant="outline"
            >
              <BarChart3 className="h-6 w-6" />
              <span className="text-xs text-center">Reports | အစီရင်ခံစာ</span>
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Warehouse Status Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {mockWarehouses.map((warehouse) => (
          <Card key={warehouse.id} className="border-0 shadow-sm hover:shadow-md transition-shadow">
            <CardHeader className="pb-2">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <Warehouse className="h-5 w-5 text-blue-600" />
                  <div>
                    <CardTitle className="text-lg">{warehouse.id}</CardTitle>
                    <p className="text-sm text-slate-600">{warehouse.name} | {warehouse.nameMM}</p>
                  </div>
                </div>
                <Badge className={
                  warehouse.status === 'normal' ? 'bg-green-100 text-green-800' :
                  warehouse.status === 'low' ? 'bg-yellow-100 text-yellow-800' :
                  'bg-red-100 text-red-800'
                }>
                  {warehouse.status === 'normal' ? (
                    <>
                      <CheckCircle className="h-3 w-3 mr-1" />
                      In Stock | လုံလောက်
                    </>
                  ) : warehouse.status === 'low' ? (
                    <>
                      <AlertTriangle className="h-3 w-3 mr-1" />
                      Low | နည်း
                    </>
                  ) : (
                    <>
                      <XCircle className="h-3 w-3 mr-1" />
                      Critical | အရေးပေါ်
                    </>
                  )}
                </Badge>
              </div>
            </CardHeader>
            <CardContent>
              <div className="space-y-2">
                <div className="flex justify-between text-sm">
                  <span>Stock | စတော့</span>
                  <span className="font-medium">{warehouse.stock.toLocaleString()} {warehouse.unit}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span>Type | အမျိုးအစား</span>
                  <span className="font-medium">{warehouse.type}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span>Input | ထည့်သွင်းနည်း</span>
                  <span className="font-medium">{warehouse.inputMethod}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span>Last Update | နောက်ဆုံးပြင်</span>
                  <span className="text-slate-500">{warehouse.lastUpdate}</span>
                </div>
              </div>
              <div className="flex gap-2 mt-4">
                <Button variant="outline" size="sm" className="flex-1">
                  <Eye className="h-3 w-3 mr-1" />
                  View | ကြည့်ရှု
                </Button>
                <Button variant="outline" size="sm" className="flex-1">
                  <Edit className="h-3 w-3 mr-1" />
                  Edit | ပြင်ဆင်
                </Button>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );

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
                    <span>Filters | စစ်ထုတ်မှု</span>
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

  const renderContent = () => {
    switch (currentPage) {
      case 'inventory-dashboard':
        return renderDashboard();
      case 'finished-goods-intake':
        return renderFinishedGoodsIntake();
      case 'warehouse-registration':
        return <div className="p-6 text-center text-slate-600">Warehouse Registration module will be implemented here</div>;
      case 'warehouse-routes':
        return <div className="p-6 text-center text-slate-600">Warehouse Routes module will be implemented here</div>;
      case 'stock-in-out':
        return <div className="p-6 text-center text-slate-600">Stock In/Out module will be implemented here</div>;
      case 'borrow-center':
        return <div className="p-6 text-center text-slate-600">Borrow Center module will be implemented here</div>;
      case 'stock-adjustment':
        return <div className="p-6 text-center text-slate-600">Stock Adjustment module will be implemented here</div>;
      case 'stock-count-audit':
        return <div className="p-6 text-center text-slate-600">Stock Count & Audit module will be implemented here</div>;
      case 'inventory-reports-main':
        return <div className="p-6 text-center text-slate-600">Inventory Reports module will be implemented here</div>;
      case 'user-permissions-matrix':
        return <div className="p-6 text-center text-slate-600">User Permissions Matrix module will be implemented here</div>;
      case 'hq-dispatch':
        return <div className="p-6 text-center text-slate-600">HQ Dispatch module will be implemented here</div>;
      default:
        return renderDashboard();
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
    </div>
  );
}