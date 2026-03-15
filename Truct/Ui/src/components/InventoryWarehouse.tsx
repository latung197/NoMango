import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Progress } from './ui/progress';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { 
  Package, 
  QrCode, 
  Search, 
  Filter, 
  TrendingUp, 
  TrendingDown, 
  AlertTriangle,
  CheckCircle,
  Clock,
  Warehouse,
  BarChart3,
  ArrowUpDown
} from 'lucide-react';

interface WarehouseItem {
  id: string;
  name: string;
  category: string;
  currentStock: number;
  maxCapacity: number;
  minThreshold: number;
  unit: string;
  status: 'normal' | 'low' | 'critical' | 'excess';
  lastUpdated: string;
  location: string;
  qrCode: string;
}

interface VirtualWarehouse {
  id: string;
  name: string;
  nameMM: string;
  type: 'raw-material' | 'production' | 'finished-goods' | 'packing' | 'cutting';
  items: WarehouseItem[];
  totalCapacity: number;
  currentUtilization: number;
  status: 'operational' | 'maintenance' | 'full';
}

const warehouses: VirtualWarehouse[] = [
  {
    id: 'WH001',
    name: 'Raw Material Warehouse',
    nameMM: 'ကုန်ကြမ်းကုန်လှောင်ရုံ',
    type: 'raw-material',
    totalCapacity: 10000,
    currentUtilization: 7500,
    status: 'operational',
    items: [
      {
        id: 'RM001',
        name: 'Plastic Pellets Type A',
        category: 'Raw Material',
        currentStock: 2500,
        maxCapacity: 3000,
        minThreshold: 500,
        unit: 'kg',
        status: 'normal',
        lastUpdated: '2024-12-27 10:30',
        location: 'A-01-001',
        qrCode: 'QR-RM001'
      },
      {
        id: 'RM002',
        name: 'Plastic Pellets Type B',
        category: 'Raw Material',
        currentStock: 1800,
        maxCapacity: 2000,
        minThreshold: 400,
        unit: 'kg',
        status: 'normal',
        lastUpdated: '2024-12-27 09:15',
        location: 'A-01-002',
        qrCode: 'QR-RM002'
      },
      {
        id: 'RM003',
        name: 'Color Additives',
        category: 'Additives',
        currentStock: 150,
        maxCapacity: 500,
        minThreshold: 100,
        unit: 'kg',
        status: 'low',
        lastUpdated: '2024-12-27 08:45',
        location: 'A-02-001',
        qrCode: 'QR-RM003'
      }
    ]
  },
  {
    id: 'WH002',
    name: 'Production Warehouse',
    nameMM: 'ထုတ်လုပ်မှုကုန်လှောင်ရုံ',
    type: 'production',
    totalCapacity: 5000,
    currentUtilization: 3200,
    status: 'operational',
    items: [
      {
        id: 'PR001',
        name: 'Semi-finished Containers',
        category: 'Work in Progress',
        currentStock: 1500,
        maxCapacity: 2000,
        minThreshold: 200,
        unit: 'pieces',
        status: 'normal',
        lastUpdated: '2024-12-27 11:00',
        location: 'B-01-001',
        qrCode: 'QR-PR001'
      },
      {
        id: 'PR002',
        name: 'Molded Parts Batch A',
        category: 'Work in Progress',
        currentStock: 800,
        maxCapacity: 1000,
        minThreshold: 100,
        unit: 'pieces',
        status: 'normal',
        lastUpdated: '2024-12-27 10:15',
        location: 'B-01-002',
        qrCode: 'QR-PR002'
      }
    ]
  },
  {
    id: 'WH003',
    name: 'Finished Goods Warehouse',
    nameMM: 'ပြီးသားထုတ်ကုန်ကုန်လှောင်ရုံ',
    type: 'finished-goods',
    totalCapacity: 8000,
    currentUtilization: 6400,
    status: 'operational',
    items: [
      {
        id: 'FG001',
        name: 'Plastic Containers Type A',
        category: 'Finished Goods',
        currentStock: 2000,
        maxCapacity: 2500,
        minThreshold: 300,
        unit: 'pieces',
        status: 'normal',
        lastUpdated: '2024-12-27 12:00',
        location: 'C-01-001',
        qrCode: 'QR-FG001'
      },
      {
        id: 'FG002',
        name: 'Plastic Cups Type B',
        category: 'Finished Goods',
        currentStock: 1800,
        maxCapacity: 2000,
        minThreshold: 250,
        unit: 'pieces',
        status: 'normal',
        lastUpdated: '2024-12-27 11:45',
        location: 'C-01-002',
        qrCode: 'QR-FG002'
      },
      {
        id: 'FG003',
        name: 'Plastic Bottles Type C',
        category: 'Finished Goods',
        currentStock: 50,
        maxCapacity: 1000,
        minThreshold: 150,
        unit: 'pieces',
        status: 'critical',
        lastUpdated: '2024-12-27 13:30',
        location: 'C-02-001',
        qrCode: 'QR-FG003'
      }
    ]
  },
  {
    id: 'WH004',
    name: 'Packing Warehouse',
    nameMM: 'ပုံးထည့်မှုကုန်လှောင်ရုံ',
    type: 'packing',
    totalCapacity: 3000,
    currentUtilization: 2100,
    status: 'operational',
    items: [
      {
        id: 'PK001',
        name: 'Cardboard Boxes',
        category: 'Packing Materials',
        currentStock: 500,
        maxCapacity: 1000,
        minThreshold: 100,
        unit: 'pieces',
        status: 'normal',
        lastUpdated: '2024-12-27 09:30',
        location: 'D-01-001',
        qrCode: 'QR-PK001'
      },
      {
        id: 'PK002',
        name: 'Plastic Wrap',
        category: 'Packing Materials',
        currentStock: 80,
        maxCapacity: 200,
        minThreshold: 50,
        unit: 'rolls',
        status: 'low',
        lastUpdated: '2024-12-27 08:15',
        location: 'D-01-002',
        qrCode: 'QR-PK002'
      }
    ]
  }
];

export function InventoryWarehouse() {
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedWarehouse, setSelectedWarehouse] = useState('all');
  const [statusFilter, setStatusFilter] = useState('all');
  const [selectedWarehouseDetail, setSelectedWarehouseDetail] = useState<VirtualWarehouse | null>(null);

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'normal': return 'bg-green-100 border-green-300';
      case 'low': return 'bg-yellow-100 border-yellow-300';
      case 'critical': return 'bg-red-100 border-red-300';
      case 'excess': return 'bg-blue-100 border-blue-300';
      default: return 'bg-gray-100 border-gray-300';
    }
  };

  const getStatusBadge = (status: string) => {
    switch (status) {
      case 'normal':
        return <Badge className="bg-green-100 text-green-800">Normal / ပုံမှန်</Badge>;
      case 'low':
        return <Badge className="bg-yellow-100 text-yellow-800">Low Stock / စတော့ခ်နည်း</Badge>;
      case 'critical':
        return <Badge className="bg-red-100 text-red-800">Critical / အရေးကြီး</Badge>;
      case 'excess':
        return <Badge className="bg-blue-100 text-blue-800">Excess / ပိုလွန်</Badge>;
      default:
        return <Badge variant="secondary">Unknown</Badge>;
    }
  };

  const getWarehouseStatusColor = (status: string) => {
    switch (status) {
      case 'operational': return 'border-green-500';
      case 'maintenance': return 'border-yellow-500';
      case 'full': return 'border-red-500';
      default: return 'border-gray-500';
    }
  };

  const getUtilizationColor = (utilization: number, capacity: number) => {
    const percentage = (utilization / capacity) * 100;
    if (percentage >= 90) return 'text-red-600';
    if (percentage >= 70) return 'text-yellow-600';
    return 'text-green-600';
  };

  const filteredWarehouses = warehouses.filter(warehouse => {
    const matchesSearch = warehouse.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         warehouse.nameMM.includes(searchTerm);
    const matchesWarehouse = selectedWarehouse === 'all' || warehouse.id === selectedWarehouse;
    
    return matchesSearch && matchesWarehouse;
  });

  if (selectedWarehouseDetail) {
    return (
      <div className="p-6 space-y-6 bg-slate-50 min-h-screen">
        {/* Header with Back Button */}
        <div className="flex items-center gap-4 mb-6">
          <Button 
            variant="outline" 
            onClick={() => setSelectedWarehouseDetail(null)}
          >
            ← Back to Warehouses
          </Button>
          <div>
            <h1 className="text-2xl font-semibold text-slate-900">{selectedWarehouseDetail.name}</h1>
            <p className="text-sm text-slate-600">{selectedWarehouseDetail.nameMM}</p>
          </div>
        </div>

        {/* Warehouse Details */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <Card className="border-0 shadow-sm">
            <CardContent className="p-6">
              <div className="flex items-center gap-3">
                <div className="p-3 bg-blue-50 rounded-lg">
                  <Package className="h-6 w-6 text-blue-600" />
                </div>
                <div>
                  <div className="text-2xl font-semibold text-slate-900">
                    {selectedWarehouseDetail.currentUtilization}
                  </div>
                  <div className="text-sm text-slate-600">Current Stock / လက်ရှိစတော့ခ်</div>
                </div>
              </div>
            </CardContent>
          </Card>
          
          <Card className="border-0 shadow-sm">
            <CardContent className="p-6">
              <div className="flex items-center gap-3">
                <div className="p-3 bg-green-50 rounded-lg">
                  <Warehouse className="h-6 w-6 text-green-600" />
                </div>
                <div>
                  <div className="text-2xl font-semibold text-slate-900">
                    {selectedWarehouseDetail.totalCapacity}
                  </div>
                  <div className="text-sm text-slate-600">Total Capacity / စုစုပေါင်းစွမ်းရည်</div>
                </div>
              </div>
            </CardContent>
          </Card>
          
          <Card className="border-0 shadow-sm">
            <CardContent className="p-6">
              <div className="flex items-center gap-3">
                <div className="p-3 bg-purple-50 rounded-lg">
                  <BarChart3 className="h-6 w-6 text-purple-600" />
                </div>
                <div>
                  <div className={`text-2xl font-semibold ${getUtilizationColor(selectedWarehouseDetail.currentUtilization, selectedWarehouseDetail.totalCapacity)}`}>
                    {Math.round((selectedWarehouseDetail.currentUtilization / selectedWarehouseDetail.totalCapacity) * 100)}%
                  </div>
                  <div className="text-sm text-slate-600">Utilization / အသုံးပြုမှု</div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Items List */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle>Inventory Items / စတော့ခ်ပစ္စည်းများ</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {selectedWarehouseDetail.items.map((item) => (
                <div key={item.id} className={`p-4 border rounded-lg ${getStatusColor(item.status)}`}>
                  <div className="flex items-center justify-between">
                    <div className="flex-1">
                      <div className="flex items-center gap-3 mb-2">
                        <div className="font-medium text-slate-900">{item.name}</div>
                        {getStatusBadge(item.status)}
                      </div>
                      <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
                        <div>
                          <div className="text-slate-500">Current Stock</div>
                          <div className="font-medium">{item.currentStock} {item.unit}</div>
                        </div>
                        <div>
                          <div className="text-slate-500">Max Capacity</div>
                          <div className="font-medium">{item.maxCapacity} {item.unit}</div>
                        </div>
                        <div>
                          <div className="text-slate-500">Location</div>
                          <div className="font-medium">{item.location}</div>
                        </div>
                        <div>
                          <div className="text-slate-500">Last Updated</div>
                          <div className="font-medium">{item.lastUpdated}</div>
                        </div>
                      </div>
                      <div className="mt-3">
                        <div className="flex justify-between text-sm mb-1">
                          <span>Stock Level</span>
                          <span>{Math.round((item.currentStock / item.maxCapacity) * 100)}%</span>
                        </div>
                        <Progress 
                          value={(item.currentStock / item.maxCapacity) * 100} 
                          className="h-2"
                        />
                      </div>
                    </div>
                    <div className="flex flex-col gap-2 ml-4">
                      <Button size="sm" variant="outline">
                        <QrCode className="h-4 w-4 mr-1" />
                        Scan QR
                      </Button>
                      <Button size="sm" className="bg-blue-600 hover:bg-blue-700">
                        <ArrowUpDown className="h-4 w-4 mr-1" />
                        Transfer
                      </Button>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="p-6 space-y-6 bg-slate-50 min-h-screen">
      {/* Header */}
      <div className="mb-6">
        <h1 className="text-2xl font-semibold text-slate-900">Inventory Management</h1>
        <p className="text-sm text-slate-600 mt-1">စတော့ခ်စီမံခန့်ခွဲမှု</p>
      </div>

      {/* Filters */}
      <Card className="border-0 shadow-sm">
        <CardContent className="p-6">
          <div className="flex flex-col md:flex-row gap-4">
            <div className="flex-1">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-slate-400" />
                <Input
                  placeholder="Search warehouses... / ကုန်လှောင်ရုံများရှာဖွေပါ..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>
            <div className="flex gap-4">
              <Select value={selectedWarehouse} onValueChange={setSelectedWarehouse}>
                <SelectTrigger className="w-48">
                  <SelectValue placeholder="Select Warehouse" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Warehouses / အားလုံး</SelectItem>
                  {warehouses.map((warehouse) => (
                    <SelectItem key={warehouse.id} value={warehouse.id}>
                      {warehouse.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <Card className="border-0 shadow-sm">
          <CardContent className="p-6">
            <div className="flex items-center gap-3">
              <div className="p-3 bg-blue-50 rounded-lg">
                <Warehouse className="h-6 w-6 text-blue-600" />
              </div>
              <div>
                <div className="text-2xl font-semibold text-slate-900">{warehouses.length}</div>
                <div className="text-sm text-slate-600">Active Warehouses / တက်ကြွကုန်လှောင်ရုံများ</div>
              </div>
            </div>
          </CardContent>
        </Card>
        
        <Card className="border-0 shadow-sm">
          <CardContent className="p-6">
            <div className="flex items-center gap-3">
              <div className="p-3 bg-green-50 rounded-lg">
                <CheckCircle className="h-6 w-6 text-green-600" />
              </div>
              <div>
                <div className="text-2xl font-semibold text-green-700">
                  {warehouses.reduce((acc, wh) => acc + wh.items.filter(item => item.status === 'normal').length, 0)}
                </div>
                <div className="text-sm text-slate-600">Normal Stock / ပုံမှန်စတော့ခ်</div>
              </div>
            </div>
          </CardContent>
        </Card>
        
        <Card className="border-0 shadow-sm">
          <CardContent className="p-6">
            <div className="flex items-center gap-3">
              <div className="p-3 bg-yellow-50 rounded-lg">
                <AlertTriangle className="h-6 w-6 text-yellow-600" />
              </div>
              <div>
                <div className="text-2xl font-semibold text-yellow-700">
                  {warehouses.reduce((acc, wh) => acc + wh.items.filter(item => item.status === 'low').length, 0)}
                </div>
                <div className="text-sm text-slate-600">Low Stock / စတော့ခ်နည်း</div>
              </div>
            </div>
          </CardContent>
        </Card>
        
        <Card className="border-0 shadow-sm">
          <CardContent className="p-6">
            <div className="flex items-center gap-3">
              <div className="p-3 bg-red-50 rounded-lg">
                <AlertTriangle className="h-6 w-6 text-red-600" />
              </div>
              <div>
                <div className="text-2xl font-semibold text-red-700">
                  {warehouses.reduce((acc, wh) => acc + wh.items.filter(item => item.status === 'critical').length, 0)}
                </div>
                <div className="text-sm text-slate-600">Critical Stock / အရေးကြီးစတော့ခ်</div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Warehouse Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {filteredWarehouses.map((warehouse) => (
          <Card 
            key={warehouse.id} 
            className={`border-0 shadow-sm border-l-4 ${getWarehouseStatusColor(warehouse.status)} cursor-pointer hover:shadow-md transition-shadow`}
            onClick={() => setSelectedWarehouseDetail(warehouse)}
          >
            <CardHeader className="pb-4">
              <CardTitle className="flex items-center justify-between">
                <div>
                  <div className="text-base">{warehouse.name}</div>
                  <div className="text-sm text-slate-500">{warehouse.nameMM}</div>
                </div>
                <Badge className={`${
                  warehouse.status === 'operational' ? 'bg-green-100 text-green-800' :
                  warehouse.status === 'maintenance' ? 'bg-yellow-100 text-yellow-800' :
                  'bg-red-100 text-red-800'
                }`}>
                  {warehouse.status}
                </Badge>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {/* Utilization */}
                <div>
                  <div className="flex justify-between text-sm mb-2">
                    <span>Utilization / အသုံးပြုမှု</span>
                    <span className={getUtilizationColor(warehouse.currentUtilization, warehouse.totalCapacity)}>
                      {Math.round((warehouse.currentUtilization / warehouse.totalCapacity) * 100)}%
                    </span>
                  </div>
                  <Progress 
                    value={(warehouse.currentUtilization / warehouse.totalCapacity) * 100} 
                    className="h-2"
                  />
                </div>

                {/* Stats */}
                <div className="grid grid-cols-2 gap-4 text-sm">
                  <div>
                    <div className="text-slate-500">Current Stock</div>
                    <div className="font-medium">{warehouse.currentUtilization.toLocaleString()}</div>
                  </div>
                  <div>
                    <div className="text-slate-500">Capacity</div>
                    <div className="font-medium">{warehouse.totalCapacity.toLocaleString()}</div>
                  </div>
                </div>

                {/* Item Status Summary */}
                <div>
                  <div className="text-sm text-slate-500 mb-2">Item Status Summary</div>
                  <div className="flex gap-2 flex-wrap">
                    <Badge className="bg-green-100 text-green-800 text-xs">
                      Normal: {warehouse.items.filter(item => item.status === 'normal').length}
                    </Badge>
                    {warehouse.items.filter(item => item.status === 'low').length > 0 && (
                      <Badge className="bg-yellow-100 text-yellow-800 text-xs">
                        Low: {warehouse.items.filter(item => item.status === 'low').length}
                      </Badge>
                    )}
                    {warehouse.items.filter(item => item.status === 'critical').length > 0 && (
                      <Badge className="bg-red-100 text-red-800 text-xs">
                        Critical: {warehouse.items.filter(item => item.status === 'critical').length}
                      </Badge>
                    )}
                  </div>
                </div>

                {/* Actions */}
                <div className="flex gap-2 pt-2">
                  <Button size="sm" variant="outline" className="flex-1">
                    <QrCode className="h-4 w-4 mr-1" />
                    QR Scan
                  </Button>
                  <Button size="sm" className="flex-1 bg-blue-600 hover:bg-blue-700">
                    View Details
                  </Button>
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}