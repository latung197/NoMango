import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { WarehouseManagement } from './WarehouseManagement';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from './ui/dialog';
import { Avatar, AvatarFallback, AvatarImage } from './ui/avatar';
import { Alert, AlertDescription } from './ui/alert';
import { toast } from 'sonner@2.0.3';
import { 
  Search,
  Filter,
  RefreshCw,
  Plus,
  FileText,
  LogIn,
  Info,
  Bell,
  Package,
  Clock,
  ArrowRight,
  ArrowLeft,
  Warehouse,
  Settings,
  AlertCircle,
  CheckCircle,
  XCircle
} from 'lucide-react';

interface StockItem {
  id: string;
  name: string;
  code: string;
  category: string;
  currentStock: number;
  reservedQty: number;
  availableQty: number;
  reorderLevel: number;
  maxLevel: number;
  unit: string;
  lastMovement: string;
  status: 'normal' | 'low' | 'critical' | 'overstock';
}

interface WarehouseKPIs {
  inStock: number;
  reserved: number;
  pendingIn: number;
  pendingOut: number;
  pendingRequests: number;
  stockItems: StockItem[];
  lowStockCount: number;
  criticalStockCount: number;
}

interface WarehouseData {
  id: string;
  name: string;
  code: string;
  type: 'RM' | 'PROD' | 'CUT' | 'CUTPIECE' | 'EXT' | 'FG' | 'PACK' | 'HQ-DISPATCH';
  company: string;
  manager: {
    name: string;
    avatar?: string;
    initials: string;
  };
  kpis: WarehouseKPIs;
  status: 'active' | 'maintenance' | 'blocked';
}

export function WarehouseDashboard() {
  const [searchTerm, setSearchTerm] = useState('');
  const [typeFilter, setTypeFilter] = useState<string>('all');
  const [companyFilter, setCompanyFilter] = useState<string>('all');
  const [showLoginModal, setShowLoginModal] = useState(false);
  const [selectedWarehouse, setSelectedWarehouse] = useState<WarehouseData | null>(null);
  const [loginCredentials, setLoginCredentials] = useState({ username: '', password: '' });
  const [loginError, setLoginError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [currentWarehouse, setCurrentWarehouse] = useState<WarehouseData | null>(null);
  const [showStockDetails, setShowStockDetails] = useState(false);
  const [selectedWarehouseForDetails, setSelectedWarehouseForDetails] = useState<WarehouseData | null>(null);

  // Mock warehouse data with detailed stock information
  const warehouses: WarehouseData[] = [
    {
      id: 'WH-RM-001',
      name: 'Raw Materials Central',
      code: 'RM-CENTRAL',
      type: 'RM',
      company: 'Smart Plastic Co.',
      manager: { name: 'Ma Thin Thin', initials: 'TT' },
      kpis: { 
        inStock: 15420, 
        reserved: 2800, 
        pendingIn: 850, 
        pendingOut: 320, 
        pendingRequests: 5,
        lowStockCount: 3,
        criticalStockCount: 1,
        stockItems: [
          {
            id: 'PE-001',
            name: 'Polyethylene Resin Grade A',
            code: 'PE-001',
            category: 'Primary Resin',
            currentStock: 8500,
            reservedQty: 1200,
            availableQty: 7300,
            reorderLevel: 2000,
            maxLevel: 15000,
            unit: 'kg',
            lastMovement: '2024-01-15 14:30',
            status: 'normal'
          },
          {
            id: 'PP-002',
            name: 'Polypropylene Red Masterbatch',
            code: 'PP-002',
            category: 'Masterbatch',
            currentStock: 450,
            reservedQty: 200,
            availableQty: 250,
            reorderLevel: 500,
            maxLevel: 2000,
            unit: 'kg',
            lastMovement: '2024-01-15 13:45',
            status: 'low'
          },
          {
            id: 'ADD-001',
            name: 'UV Stabilizer',
            code: 'ADD-001',
            category: 'Additives',
            currentStock: 85,
            reservedQty: 50,
            availableQty: 35,
            reorderLevel: 100,
            maxLevel: 500,
            unit: 'kg',
            lastMovement: '2024-01-15 12:20',
            status: 'critical'
          },
          {
            id: 'HDPE-003',
            name: 'HDPE Natural Grade',
            code: 'HDPE-003',
            category: 'Primary Resin',
            currentStock: 6385,
            reservedQty: 1350,
            availableQty: 5035,
            reorderLevel: 1500,
            maxLevel: 10000,
            unit: 'kg',
            lastMovement: '2024-01-15 15:10',
            status: 'normal'
          }
        ]
      },
      status: 'active'
    },
    {
      id: 'WH-PROD-001',
      name: 'Production Floor A',
      code: 'PROD-A',
      type: 'PROD',
      company: 'Smart Plastic Co.',
      manager: { name: 'Ko Aung Kyaw', initials: 'AK' },
      kpis: { 
        inStock: 8750, 
        reserved: 1200, 
        pendingIn: 450, 
        pendingOut: 680, 
        pendingRequests: 3,
        lowStockCount: 2,
        criticalStockCount: 0,
        stockItems: [
          {
            id: 'WIP-001',
            name: 'Bottle 500ml - In Progress',
            code: 'WIP-001',
            category: 'Work in Progress',
            currentStock: 3200,
            reservedQty: 500,
            availableQty: 2700,
            reorderLevel: 1000,
            maxLevel: 8000,
            unit: 'pcs',
            lastMovement: '2024-01-15 15:45',
            status: 'normal'
          },
          {
            id: 'WIP-002',
            name: 'Food Container 1L - Semi-Finished',
            code: 'WIP-002',
            category: 'Work in Progress',
            currentStock: 2150,
            reservedQty: 400,
            availableQty: 1750,
            reorderLevel: 2000,
            maxLevel: 5000,
            unit: 'pcs',
            lastMovement: '2024-01-15 14:20',
            status: 'low'
          },
          {
            id: 'MOLD-001',
            name: 'Injection Mold Set A',
            code: 'MOLD-001',
            category: 'Tooling',
            currentStock: 1,
            reservedQty: 0,
            availableQty: 1,
            reorderLevel: 1,
            maxLevel: 2,
            unit: 'set',
            lastMovement: '2024-01-15 08:00',
            status: 'normal'
          }
        ]
      },
      status: 'active'
    },
    {
      id: 'WH-FG-001',
      name: 'Finished Goods Storage',
      code: 'FG-MAIN',
      type: 'FG',
      company: 'Smart Plastic Co.',
      manager: { name: 'Ma Su Su', initials: 'SS' },
      kpis: { 
        inStock: 25630, 
        reserved: 8900, 
        pendingIn: 1250, 
        pendingOut: 2100, 
        pendingRequests: 8,
        lowStockCount: 1,
        criticalStockCount: 0,
        stockItems: [
          {
            id: 'FG-BTL-500',
            name: 'Plastic Bottle 500ml Clear',
            code: 'FG-BTL-500',
            category: 'Bottles',
            currentStock: 15400,
            reservedQty: 4500,
            availableQty: 10900,
            reorderLevel: 5000,
            maxLevel: 30000,
            unit: 'pcs',
            lastMovement: '2024-01-15 16:00',
            status: 'normal'
          },
          {
            id: 'FG-CONT-1L',
            name: 'Food Container 1L White',
            code: 'FG-CONT-1L',
            category: 'Containers',
            currentStock: 8930,
            reservedQty: 3200,
            availableQty: 5730,
            reorderLevel: 3000,
            maxLevel: 20000,
            unit: 'pcs',
            lastMovement: '2024-01-15 15:30',
            status: 'normal'
          },
          {
            id: 'FG-CUP-200',
            name: 'Disposable Cup 200ml',
            code: 'FG-CUP-200',
            category: 'Cups',
            currentStock: 1300,
            reservedQty: 1200,
            availableQty: 100,
            reorderLevel: 2000,
            maxLevel: 15000,
            unit: 'pcs',
            lastMovement: '2024-01-15 13:15',
            status: 'low'
          }
        ]
      },
      status: 'active'
    },
    {
      id: 'WH-CUT-001',
      name: 'Cut Piece Storage',
      code: 'CUT-STG',
      type: 'CUT',
      company: 'Smart Plastic Co.',
      manager: { name: 'Ko Zaw Min', initials: 'ZM' },
      kpis: { 
        inStock: 3420, 
        reserved: 890, 
        pendingIn: 120, 
        pendingOut: 250, 
        pendingRequests: 2,
        lowStockCount: 1,
        criticalStockCount: 1,
        stockItems: [
          {
            id: 'CUT-BTL-TRIM',
            name: 'Bottle Trimming Waste',
            code: 'CUT-BTL-TRIM',
            category: 'Recyclable Waste',
            currentStock: 2100,
            reservedQty: 500,
            availableQty: 1600,
            reorderLevel: 1000,
            maxLevel: 5000,
            unit: 'kg',
            lastMovement: '2024-01-15 14:00',
            status: 'normal'
          },
          {
            id: 'CUT-CONT-OFF',
            name: 'Container Cut-offs',
            code: 'CUT-CONT-OFF',
            category: 'Recyclable Waste',
            currentStock: 820,
            reservedQty: 200,
            availableQty: 620,
            reorderLevel: 800,
            maxLevel: 3000,
            unit: 'kg',
            lastMovement: '2024-01-15 13:30',
            status: 'normal'
          }
        ]
      },
      status: 'maintenance'
    },
    {
      id: 'WH-PACK-001',
      name: 'Packaging Materials',
      code: 'PACK-01',
      type: 'PACK',
      company: 'Smart Plastic Co.',
      manager: { name: 'Ma Khin Khin', initials: 'KK' },
      kpis: { 
        inStock: 12450, 
        reserved: 3200, 
        pendingIn: 780, 
        pendingOut: 450, 
        pendingRequests: 4,
        lowStockCount: 2,
        criticalStockCount: 0,
        stockItems: [
          {
            id: 'PACK-BOX-CARD',
            name: 'Cardboard Boxes Large',
            code: 'PACK-BOX-CARD',
            category: 'Primary Packaging',
            currentStock: 5200,
            reservedQty: 1500,
            availableQty: 3700,
            reorderLevel: 2000,
            maxLevel: 10000,
            unit: 'pcs',
            lastMovement: '2024-01-15 15:20',
            status: 'normal'
          },
          {
            id: 'PACK-LABEL-BTL',
            name: 'Bottle Labels Assorted',
            code: 'PACK-LABEL-BTL',
            category: 'Labels',
            currentStock: 4850,
            reservedQty: 1200,
            availableQty: 3650,
            reorderLevel: 3000,
            maxLevel: 15000,
            unit: 'pcs',
            lastMovement: '2024-01-15 14:45',
            status: 'normal'
          },
          {
            id: 'PACK-WRAP-PLAST',
            name: 'Plastic Wrap Roll',
            code: 'PACK-WRAP-PLAST',
            category: 'Wrapping Materials',
            currentStock: 890,
            reservedQty: 300,
            availableQty: 590,
            reorderLevel: 1000,
            maxLevel: 3000,
            unit: 'rolls',
            lastMovement: '2024-01-15 12:30',
            status: 'low'
          }
        ]
      },
      status: 'active'
    },
    {
      id: 'WH-HQ-001',
      name: 'HQ Dispatch Center',
      code: 'HQ-DISP',
      type: 'HQ-DISPATCH',
      company: 'Smart Plastic Co.',
      manager: { name: 'Ko Min Thu', initials: 'MT' },
      kpis: { 
        inStock: 18900, 
        reserved: 12500, 
        pendingIn: 2800, 
        pendingOut: 5600, 
        pendingRequests: 12,
        totalValue: 8950000,
        lowStockCount: 0,
        criticalStockCount: 0,
        stockItems: [
          {
            id: 'DISP-BTL-MIX',
            name: 'Mixed Bottles for Dispatch',
            code: 'DISP-BTL-MIX',
            category: 'Ready for Shipment',
            currentStock: 12300,
            reservedQty: 8500,
            availableQty: 3800,
            reorderLevel: 5000,
            maxLevel: 25000,
            unit: 'pcs',
            lastMovement: '2024-01-15 16:30',
            status: 'normal'
          },
          {
            id: 'DISP-CONT-BULK',
            name: 'Bulk Containers for Export',
            code: 'DISP-CONT-BULK',
            category: 'Export Items',
            currentStock: 6600,
            reservedQty: 4000,
            availableQty: 2600,
            reorderLevel: 2000,
            maxLevel: 15000,
            unit: 'pcs',
            lastMovement: '2024-01-15 15:45',
            status: 'normal'
          }
        ]
      },
      status: 'active'
    }
  ];

  const companies = ['Smart Plastic Co.', 'Sister Company A', 'Sister Company B'];

  const getTypeColor = (type: string) => {
    const colors = {
      'RM': 'bg-blue-100 text-blue-800',
      'PROD': 'bg-purple-100 text-purple-800', 
      'CUT': 'bg-orange-100 text-orange-800',
      'CUTPIECE': 'bg-amber-100 text-amber-800',
      'EXT': 'bg-gray-100 text-gray-800',
      'FG': 'bg-green-100 text-green-800',
      'PACK': 'bg-pink-100 text-pink-800',
      'HQ-DISPATCH': 'bg-indigo-100 text-indigo-800'
    };
    return colors[type as keyof typeof colors] || 'bg-gray-100 text-gray-800';
  };

  const getStatusColor = (status: string) => {
    const colors = {
      'active': 'bg-green-100 text-green-800',
      'maintenance': 'bg-yellow-100 text-yellow-800',
      'blocked': 'bg-red-100 text-red-800'
    };
    return colors[status as keyof typeof colors] || 'bg-gray-100 text-gray-800';
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'active': return <CheckCircle className="h-3 w-3" />;
      case 'maintenance': return <Settings className="h-3 w-3" />;
      case 'blocked': return <XCircle className="h-3 w-3" />;
      default: return <AlertCircle className="h-3 w-3" />;
    }
  };

  const getStockStatusColor = (status: string) => {
    switch (status) {
      case 'normal': return 'bg-green-100 text-green-800 border-green-200';
      case 'low': return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'critical': return 'bg-red-100 text-red-800 border-red-200';
      case 'overstock': return 'bg-blue-100 text-blue-800 border-blue-200';
      default: return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const getStockStatusIcon = (status: string) => {
    switch (status) {
      case 'normal': return '✅';
      case 'low': return '⚠️';
      case 'critical': return '🚨';
      case 'overstock': return '📦';
      default: return '❓';
    }
  };

  const handleViewStockDetails = (warehouse: WarehouseData) => {
    setSelectedWarehouseForDetails(warehouse);
    setShowStockDetails(true);
  };

  const formatCurrency = (value: number) => {
    return `${(value / 1000).toFixed(0)}K MMK`;
  };

  const filteredWarehouses = warehouses.filter(warehouse => {
    const matchesSearch = warehouse.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         warehouse.code.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesType = typeFilter === 'all' || warehouse.type === typeFilter;
    const matchesCompany = companyFilter === 'all' || warehouse.company === companyFilter;
    
    return matchesSearch && matchesType && matchesCompany;
  });

  const handleWarehouseLogin = (warehouse: WarehouseData) => {
    setSelectedWarehouse(warehouse);
    setShowLoginModal(true);
    setLoginCredentials({ username: '', password: '' });
    setLoginError('');
  };

  const handleLogin = async () => {
    setIsLoading(true);
    setLoginError('');

    // Simulate login process
    await new Promise(resolve => setTimeout(resolve, 1000));

    if (!loginCredentials.username || !loginCredentials.password) {
      setLoginError('Please enter both username and password');
      setIsLoading(false);
      return;
    }

    // Simple mock authentication
    if (loginCredentials.username === 'admin' && loginCredentials.password === 'password') {
      toast.success(`Successfully logged into ${selectedWarehouse?.name}`);
      setShowLoginModal(false);
      setCurrentWarehouse(selectedWarehouse);
    } else {
      setLoginError('Invalid username or password. Try admin/password for demo.');
    }
    
    setIsLoading(false);
  };

  const handleRefresh = () => {
    toast.success('Warehouse data refreshed');
  };

  const EmptyState = () => (
    <div className="col-span-full flex flex-col items-center justify-center py-16 px-4">
      <div className="w-24 h-24 bg-slate-100 rounded-full flex items-center justify-center mb-4">
        <Warehouse className="h-12 w-12 text-slate-400" />
      </div>
      <h3 className="text-lg font-medium text-slate-900 mb-2">No warehouses registered</h3>
      <p className="text-slate-600 text-center mb-4">
        Get started by registering your first warehouse in Master Data.
      </p>
      <Button variant="outline">
        <Plus className="h-4 w-4 mr-2" />
        Add Warehouse
      </Button>
    </div>
  );

  // If logged into a warehouse, show the warehouse management interface
  if (currentWarehouse) {
    return (
      <WarehouseManagement 
        warehouse={currentWarehouse} 
        onBack={() => setCurrentWarehouse(null)}
      />
    );
  }

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-6">
          <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-4 mb-4">
            <div>
              <h1 className="text-2xl font-semibold text-slate-900">Warehouse Dashboard</h1>
              <p className="text-slate-600">ဂိုဒေါင်ဒက်ရှ်ဘုတ်</p>
            </div>
            
            <div className="flex flex-wrap gap-2">
              <Button variant="outline" size="sm">
                <Plus className="h-4 w-4 mr-2" />
                Add Adjustment
                <span className="text-xs text-slate-500 ml-1">(Supervisor)</span>
              </Button>
              <Button variant="outline" size="sm">
                <FileText className="h-4 w-4 mr-2" />
                Reports
              </Button>
              <Button variant="outline" size="sm" onClick={handleRefresh}>
                <RefreshCw className="h-4 w-4 mr-2" />
                Refresh
              </Button>
            </div>
          </div>

          {/* Search and Filters */}
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-slate-400" />
              <Input
                placeholder="Search warehouse name or code..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-10"
              />
            </div>
            
            <Select value={typeFilter} onValueChange={setTypeFilter}>
              <SelectTrigger>
                <SelectValue placeholder="Filter by type" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Types</SelectItem>
                <SelectItem value="RM">Raw Materials</SelectItem>
                <SelectItem value="PROD">Production</SelectItem>
                <SelectItem value="CUT">Cut Piece</SelectItem>
                <SelectItem value="CUTPIECE">Cut Piece Storage</SelectItem>
                <SelectItem value="EXT">External</SelectItem>
                <SelectItem value="FG">Finished Goods</SelectItem>
                <SelectItem value="PACK">Packaging</SelectItem>
                <SelectItem value="HQ-DISPATCH">HQ Dispatch</SelectItem>
              </SelectContent>
            </Select>

            <Select value={companyFilter} onValueChange={setCompanyFilter}>
              <SelectTrigger>
                <SelectValue placeholder="Select company" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Companies</SelectItem>
                {companies.map(company => (
                  <SelectItem key={company} value={company}>{company}</SelectItem>
                ))}
              </SelectContent>
            </Select>

            <div className="flex items-center gap-2">
              <Filter className="h-4 w-4 text-slate-500" />
              <span className="text-sm text-slate-600">
                {filteredWarehouses.length} warehouses
              </span>
            </div>
          </div>
        </div>

        {/* Warehouse Grid */}
        {filteredWarehouses.length === 0 ? (
          <EmptyState />
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {filteredWarehouses.map((warehouse) => (
              <Card key={warehouse.id} className="hover:shadow-lg transition-shadow">
                <CardHeader className="pb-3">
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <CardTitle className="text-lg mb-1">{warehouse.name}</CardTitle>
                      <p className="text-sm text-slate-500 mb-2">{warehouse.code}</p>
                      <div className="flex items-center gap-2 mb-2">
                        <Badge className={getTypeColor(warehouse.type)}>
                          {warehouse.type}
                        </Badge>
                        <Badge className={getStatusColor(warehouse.status)} variant="outline">
                          {getStatusIcon(warehouse.status)}
                          <span className="ml-1 capitalize">{warehouse.status}</span>
                        </Badge>
                      </div>
                    </div>
                  </div>

                  {/* Manager */}
                  <div className="flex items-center gap-2">
                    <Avatar className="h-8 w-8">
                      <AvatarImage src={warehouse.manager.avatar} />
                      <AvatarFallback className="text-xs">{warehouse.manager.initials}</AvatarFallback>
                    </Avatar>
                    <div>
                      <p className="text-sm font-medium text-slate-900">{warehouse.manager.name}</p>
                      <p className="text-xs text-slate-500">Manager</p>
                    </div>
                  </div>
                </CardHeader>

                <CardContent className="pt-0">
                  {/* Simplified Summary - No detailed stock information */}
                  <div className="space-y-4">
                    {/* Basic Warehouse Info */}
                    <div className="bg-gradient-to-r from-slate-50 to-slate-100 rounded-lg p-3">
                      <div className="text-center">
                        <div className="text-sm text-slate-600 mb-1">Company</div>
                        <div className="font-medium text-slate-900">{warehouse.company}</div>
                      </div>
                    </div>

                    {/* Basic Status Info Only */}
                    <div className="bg-slate-50 rounded-lg p-3">
                      <div className="text-center">
                        <div className="text-sm text-slate-600 mb-2">Warehouse Status</div>
                        <Badge className={`${getStatusColor(warehouse.status)} text-sm px-3 py-1`}>
                          {getStatusIcon(warehouse.status)}
                          <span className="ml-1 capitalize">{warehouse.status}</span>
                        </Badge>
                        <div className="text-xs text-slate-500 mt-2">
                          Login required to view stock details
                        </div>
                        <div className="text-xs text-slate-400">
                          စတော့အခြေအနေကြည့်ရန် အကောင့်ဝင်ရန်လိုအပ်သည်
                        </div>
                      </div>
                    </div>

                    {/* Login Alert */}
                    <div className="bg-blue-50 border border-blue-200 rounded-lg p-3">
                      <div className="flex items-center justify-center gap-2">
                        <LogIn className="h-4 w-4 text-blue-600" />
                        <span className="text-sm text-blue-800 font-medium">Access Required</span>
                      </div>
                      <div className="text-xs text-blue-600 text-center mt-1">
                        Enter with username & password to view stock balance details
                      </div>
                    </div>

                    {/* Single Action Button */}
                    <Button 
                      className="w-full bg-blue-600 hover:bg-blue-700" 
                      onClick={() => handleWarehouseLogin(warehouse)}
                      disabled={warehouse.status === 'blocked'}
                    >
                      <LogIn className="h-4 w-4 mr-2" />
                      Enter Warehouse
                      <span className="ml-2 text-xs opacity-75">(Login Required)</span>
                    </Button>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        )}

        {/* Login Modal */}
        <Dialog open={showLoginModal} onOpenChange={setShowLoginModal}>
          <DialogContent className="sm:max-w-md">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <LogIn className="h-5 w-5" />
                Access {selectedWarehouse?.name}
              </DialogTitle>
              <DialogDescription>
                Enter your credentials to access the warehouse management system for {selectedWarehouse?.name}.
              </DialogDescription>
            </DialogHeader>
            
            <div className="space-y-4">
              {loginError && (
                <Alert className="border-red-200 bg-red-50">
                  <AlertCircle className="h-4 w-4 text-red-600" />
                  <AlertDescription className="text-red-800">
                    {loginError}
                  </AlertDescription>
                </Alert>
              )}

              <div className="space-y-2">
                <Label htmlFor="username">Username</Label>
                <Input
                  id="username"
                  value={loginCredentials.username}
                  onChange={(e) => setLoginCredentials(prev => ({ ...prev, username: e.target.value }))}
                  placeholder="Enter your username"
                  className="text-base"
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="password">Password</Label>
                <Input
                  id="password"
                  type="password"
                  value={loginCredentials.password}
                  onChange={(e) => setLoginCredentials(prev => ({ ...prev, password: e.target.value }))}
                  placeholder="Enter your password"
                  className="text-base"
                  onKeyDown={(e) => e.key === 'Enter' && handleLogin()}
                />
              </div>

              <div className="flex gap-3 pt-4">
                <Button 
                  onClick={handleLogin} 
                  disabled={isLoading}
                  className="flex-1"
                  size="lg"
                >
                  {isLoading ? (
                    <>
                      <RefreshCw className="h-4 w-4 mr-2 animate-spin" />
                      Logging in...
                    </>
                  ) : (
                    <>
                      <LogIn className="h-4 w-4 mr-2" />
                      Login
                    </>
                  )}
                </Button>
                <Button 
                  variant="outline" 
                  onClick={() => setShowLoginModal(false)}
                  disabled={isLoading}
                  size="lg"
                >
                  Cancel
                </Button>
              </div>

              <div className="text-xs text-slate-500 text-center">
                Demo credentials: admin / password
              </div>
            </div>
          </DialogContent>
        </Dialog>

        {/* Stock Details Modal */}
        <Dialog open={showStockDetails} onOpenChange={setShowStockDetails}>
          <DialogContent className="max-w-6xl max-h-[90vh] overflow-hidden">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <Package className="h-5 w-5" />
                Stock Balance Details - {selectedWarehouseForDetails?.name}
              </DialogTitle>
              <DialogDescription>
                Comprehensive stock balance report for {selectedWarehouseForDetails?.code} | 
                Manager: {selectedWarehouseForDetails?.manager.name}
              </DialogDescription>
            </DialogHeader>
            
            {selectedWarehouseForDetails && (
              <div className="space-y-6 overflow-y-auto max-h-[70vh] pr-2">
                {/* Summary Cards */}
                <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                  <Card className="bg-green-50 border-green-200">
                    <CardContent className="p-4 text-center">
                      <div className="text-2xl font-bold text-green-800">
                        {selectedWarehouseForDetails.kpis.inStock.toLocaleString()}
                      </div>
                      <div className="text-sm font-medium text-green-600">📦 Total In Stock</div>
                      <div className="text-xs text-green-500">စုစုပေါင်းကုန်ပစ္စည်း</div>
                    </CardContent>
                  </Card>
                  
                  <Card className="bg-blue-50 border-blue-200">
                    <CardContent className="p-4 text-center">
                      <div className="text-2xl font-bold text-blue-800">
                        {formatCurrency(selectedWarehouseForDetails.kpis.totalValue)}
                      </div>
                      <div className="text-sm font-medium text-blue-600">💰 Total Value</div>
                      <div className="text-xs text-blue-500">စုစုပေါင်းတန်ဖိုး</div>
                    </CardContent>
                  </Card>

                  <Card className="bg-yellow-50 border-yellow-200">
                    <CardContent className="p-4 text-center">
                      <div className="text-2xl font-bold text-yellow-800">
                        {selectedWarehouseForDetails.kpis.lowStockCount + selectedWarehouseForDetails.kpis.criticalStockCount}
                      </div>
                      <div className="text-sm font-medium text-yellow-600">⚠️ Stock Alerts</div>
                      <div className="text-xs text-yellow-500">သတိပေးချက်များ</div>
                    </CardContent>
                  </Card>

                  <Card className="bg-purple-50 border-purple-200">
                    <CardContent className="p-4 text-center">
                      <div className="text-2xl font-bold text-purple-800">
                        {selectedWarehouseForDetails.kpis.stockItems.length}
                      </div>
                      <div className="text-sm font-medium text-purple-600">📋 Stock Items</div>
                      <div className="text-xs text-purple-500">ကုန်ပစ္စည်းအမျိုးအစား</div>
                    </CardContent>
                  </Card>
                </div>

                {/* Stock Items Table */}
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <Package className="h-5 w-5" />
                      Detailed Stock Breakdown | အသေးစိတ်ကုန်ပစ္စည်းခွဲခြမ်းချက်
                    </CardTitle>
                  </CardHeader>
                  <CardContent className="p-0">
                    <div className="overflow-x-auto">
                      <table className="w-full">
                        <thead className="bg-slate-50 border-b">
                          <tr>
                            <th className="text-left p-4 font-medium text-slate-700">Item Details</th>
                            <th className="text-center p-4 font-medium text-slate-700">Category</th>
                            <th className="text-center p-4 font-medium text-slate-700">Current Stock</th>
                            <th className="text-center p-4 font-medium text-slate-700">Reserved</th>
                            <th className="text-center p-4 font-medium text-slate-700">Available</th>
                            <th className="text-center p-4 font-medium text-slate-700">Levels</th>
                            <th className="text-center p-4 font-medium text-slate-700">Status</th>
                            <th className="text-center p-4 font-medium text-slate-700">Last Movement</th>
                          </tr>
                        </thead>
                        <tbody>
                          {selectedWarehouseForDetails.kpis.stockItems.map((item, index) => (
                            <tr key={item.id} className={`border-b hover:bg-slate-50 ${index % 2 === 0 ? 'bg-white' : 'bg-slate-25'}`}>
                              <td className="p-4">
                                <div>
                                  <div className="font-medium text-slate-900">{item.name}</div>
                                  <div className="text-sm text-slate-500 font-mono">{item.code}</div>
                                </div>
                              </td>
                              <td className="p-4 text-center">
                                <Badge variant="outline" className="text-xs">
                                  {item.category}
                                </Badge>
                              </td>
                              <td className="p-4 text-center">
                                <div className="font-bold text-lg">{item.currentStock.toLocaleString()}</div>
                                <div className="text-xs text-slate-500">{item.unit}</div>
                              </td>
                              <td className="p-4 text-center">
                                <div className="font-medium text-amber-700">{item.reservedQty.toLocaleString()}</div>
                                <div className="text-xs text-slate-500">{item.unit}</div>
                              </td>
                              <td className="p-4 text-center">
                                <div className="font-medium text-green-700">{item.availableQty.toLocaleString()}</div>
                                <div className="text-xs text-slate-500">{item.unit}</div>
                              </td>
                              <td className="p-4 text-center">
                                <div className="text-xs space-y-1">
                                  <div className="text-red-600">Min: {item.reorderLevel.toLocaleString()}</div>
                                  <div className="text-blue-600">Max: {item.maxLevel.toLocaleString()}</div>
                                </div>
                              </td>
                              <td className="p-4 text-center">
                                <Badge className={getStockStatusColor(item.status)}>
                                  {getStockStatusIcon(item.status)} {item.status.toUpperCase()}
                                </Badge>
                              </td>
                              <td className="p-4 text-center">
                                <div className="text-xs text-slate-600 font-mono">{item.lastMovement}</div>
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  </CardContent>
                </Card>

                {/* Stock Level Analysis */}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <Card>
                    <CardHeader>
                      <CardTitle className="text-lg">🚨 Critical & Low Stock Items</CardTitle>
                    </CardHeader>
                    <CardContent>
                      <div className="space-y-3">
                        {selectedWarehouseForDetails.kpis.stockItems
                          .filter(item => item.status === 'critical' || item.status === 'low')
                          .map((item) => (
                            <div key={item.id} className={`p-3 rounded-lg border ${
                              item.status === 'critical' ? 'bg-red-50 border-red-200' : 'bg-yellow-50 border-yellow-200'
                            }`}>
                              <div className="flex items-center justify-between">
                                <div>
                                  <div className="font-medium">{item.name}</div>
                                  <div className="text-sm text-slate-500">{item.code}</div>
                                </div>
                                <div className="text-right">
                                  <div className={`font-bold ${
                                    item.status === 'critical' ? 'text-red-700' : 'text-yellow-700'
                                  }`}>
                                    {item.currentStock.toLocaleString()} {item.unit}
                                  </div>
                                  <div className="text-xs text-slate-500">
                                    Reorder at: {item.reorderLevel.toLocaleString()} {item.unit}
                                  </div>
                                </div>
                              </div>
                            </div>
                          ))}
                        {selectedWarehouseForDetails.kpis.stockItems
                          .filter(item => item.status === 'critical' || item.status === 'low').length === 0 && (
                          <div className="text-center text-slate-500 py-4">
                            ✅ All stock levels are normal
                          </div>
                        )}
                      </div>
                    </CardContent>
                  </Card>

                  <Card>
                    <CardHeader>
                      <CardTitle className="text-lg">📊 Stock Summary by Category</CardTitle>
                    </CardHeader>
                    <CardContent>
                      <div className="space-y-2">
                        {Object.entries(
                          selectedWarehouseForDetails.kpis.stockItems.reduce((acc, item) => {
                            acc[item.category] = (acc[item.category] || 0) + item.currentStock;
                            return acc;
                          }, {} as Record<string, number>)
                        ).map(([category, total]) => (
                          <div key={category} className="flex items-center justify-between p-2 bg-slate-50 rounded">
                            <span className="font-medium">{category}</span>
                            <Badge variant="outline" className="font-mono">
                              {total.toLocaleString()}
                            </Badge>
                          </div>
                        ))}
                      </div>
                    </CardContent>
                  </Card>
                </div>

                {/* Action Buttons */}
                <div className="flex gap-3 pt-4 border-t">
                  <Button 
                    className="bg-blue-600 hover:bg-blue-700"
                    onClick={() => {
                      setShowStockDetails(false);
                      handleWarehouseLogin(selectedWarehouseForDetails);
                    }}
                  >
                    <LogIn className="h-4 w-4 mr-2" />
                    Enter Warehouse
                  </Button>
                  <Button variant="outline">
                    <FileText className="h-4 w-4 mr-2" />
                    Export Report
                  </Button>
                  <Button variant="outline">
                    <RefreshCw className="h-4 w-4 mr-2" />
                    Refresh Data
                  </Button>
                  <Button 
                    variant="outline" 
                    onClick={() => setShowStockDetails(false)}
                    className="ml-auto"
                  >
                    Close
                  </Button>
                </div>
              </div>
            )}
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}