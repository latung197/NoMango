import React, { useState } from 'react';
import { Truck, Plus, Printer, Download, Edit, Trash2, Package, FileText, Search, Filter, Calendar, User, Hash, Tag } from 'lucide-react';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Badge } from './ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogTrigger } from './ui/dialog';
import { Label } from './ui/label';
import { Textarea } from './ui/textarea';
import { Separator } from './ui/separator';
import { toast } from 'sonner@2.0.3';

// Mock data
const mockGroups = [
  { id: 'GRP-A', name: 'Group A', color: '#3B82F6' },
  { id: 'GRP-B', name: 'Group B', color: '#10B981' },
  { id: 'GRP-C', name: 'Group C', color: '#F59E0B' }
];

const mockJobs = [
  { id: 'JOB-001', productName: 'Plastic Container 500ml', groupId: 'GRP-A', groupName: 'Group A' },
  { id: 'JOB-002', productName: 'Bottle Cap Standard', groupId: 'GRP-B', groupName: 'Group B' },
  { id: 'JOB-003', productName: 'Storage Box Large', groupId: 'GRP-A', groupName: 'Group A' },
  { id: 'JOB-004', productName: 'Food Container 1L', groupId: 'GRP-C', groupName: 'Group C' }
];

const mockTransactions = [
  {
    id: 1,
    date: '2024-01-15',
    time: '08:30',
    type: 'In',
    category: 'raw-material',
    jobId: 'JOB-001',
    productName: 'Plastic Container 500ml',
    groupId: 'GRP-A',
    groupName: 'Group A',
    qty: 1000,
    driverName: 'Mg Thura',
    vehicleNo: 'YGN-1234',
    remarks: 'Regular delivery',
    status: 'Confirmed'
  },
  {
    id: 2,
    date: '2024-01-15',
    time: '10:15',
    type: 'Out',
    category: 'finished-goods',
    jobId: 'JOB-002',
    productName: 'Bottle Cap Standard',
    groupId: 'GRP-B',
    groupName: 'Group B',
    qty: 5000,
    driverName: 'Ko Aung',
    vehicleNo: 'MDY-5678',
    remarks: 'Urgent delivery',
    status: 'Pending'
  },
  {
    id: 3,
    date: '2024-01-15',
    time: '14:45',
    type: 'Out',
    category: 'finished-goods',
    jobId: 'JOB-003',
    productName: 'Storage Box Large',
    groupId: 'GRP-A',
    groupName: 'Group A',
    qty: 200,
    driverName: 'Ma Htwe',
    vehicleNo: 'NPT-9012',
    remarks: 'Customer pickup',
    status: 'Delivered'
  }
];

interface Transaction {
  id: number;
  date: string;
  time: string;
  type: 'In' | 'Out';
  category: 'raw-material' | 'finished-goods' | 'others';
  jobId: string;
  productName: string;
  groupId: string;
  groupName: string;
  qty: number;
  driverName?: string;
  vehicleNo?: string;
  remarks: string;
  status: 'Pending' | 'Confirmed' | 'Delivered';
}

export function LogisticControl() {
  const [activeTab, setActiveTab] = useState('raw-material');
  const [transactions, setTransactions] = useState<Transaction[]>(mockTransactions);
  const [selectedTransactions, setSelectedTransactions] = useState<number[]>([]);
  
  // Filters
  const [dateFilter, setDateFilter] = useState('');
  const [jobIdFilter, setJobIdFilter] = useState('');
  const [productFilter, setProductFilter] = useState('');
  const [groupFilter, setGroupFilter] = useState('all');
  const [statusFilter, setStatusFilter] = useState('all');
  
  // Add/Edit Transaction
  const [showAddDialog, setShowAddDialog] = useState(false);
  const [editingTransaction, setEditingTransaction] = useState<Transaction | null>(null);
  const [formData, setFormData] = useState({
    type: 'In' as 'In' | 'Out',
    jobId: undefined as string | undefined,
    qty: '',
    driverName: '',
    vehicleNo: '',
    remarks: ''
  });
  
  // Print DO
  const [showPrintDialog, setShowPrintDialog] = useState(false);
  const [printSelection, setPrintSelection] = useState<'job' | 'group'>('job');
  const [selectedJobId, setSelectedJobId] = useState<string | undefined>(undefined);
  const [selectedGroupId, setSelectedGroupId] = useState<string | undefined>(undefined);

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Pending': return 'bg-yellow-100 text-yellow-800';
      case 'Confirmed': return 'bg-blue-100 text-blue-800';
      case 'Delivered': return 'bg-green-100 text-green-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getTransactionIcon = (type: string) => {
    return type === 'In' ? 
      <Package className="h-4 w-4 text-green-600" /> : 
      <Truck className="h-4 w-4 text-blue-600" />;
  };

  const filteredTransactions = transactions.filter(transaction => {
    if (transaction.category !== activeTab) return false;
    if (dateFilter && !transaction.date.includes(dateFilter)) return false;
    if (jobIdFilter && !transaction.jobId.toLowerCase().includes(jobIdFilter.toLowerCase())) return false;
    if (productFilter && !transaction.productName.toLowerCase().includes(productFilter.toLowerCase())) return false;
    if (groupFilter && groupFilter !== 'all' && transaction.groupId !== groupFilter) return false;
    if (statusFilter && statusFilter !== 'all' && transaction.status !== statusFilter) return false;
    return true;
  });

  const handleAddTransaction = () => {
    const selectedJob = mockJobs.find(job => job.id === formData.jobId);
    if (!selectedJob || !formData.jobId) {
      toast.error('Please select a valid job');
      return;
    }

    const newTransaction: Transaction = {
      id: Date.now(),
      date: new Date().toISOString().split('T')[0],
      time: new Date().toLocaleTimeString('en-US', { hour12: false, hour: '2-digit', minute: '2-digit' }),
      type: formData.type,
      category: activeTab as any,
      jobId: formData.jobId!,
      productName: selectedJob.productName,
      groupId: selectedJob.groupId,
      groupName: selectedJob.groupName,
      qty: parseInt(formData.qty),
      driverName: formData.driverName,
      vehicleNo: formData.vehicleNo,
      remarks: formData.remarks,
      status: 'Pending'
    };

    setTransactions(prev => [newTransaction, ...prev]);
    setShowAddDialog(false);
    setFormData({
      type: 'In',
      jobId: undefined,
      qty: '',
      driverName: '',
      vehicleNo: '',
      remarks: ''
    });
    toast.success('Transaction added successfully');
  };

  const handlePrintDO = () => {
    if (printSelection === 'job' && !selectedJobId) {
      toast.error('Please select a Job ID');
      return;
    }
    if (printSelection === 'group' && !selectedGroupId) {
      toast.error('Please select a Group');
      return;
    }

    // Mock print functionality
    let printData: any[] = [];
    
    if (printSelection === 'job') {
      printData = filteredTransactions.filter(t => t.jobId === selectedJobId && t.type === 'Out');
    } else {
      // Group all transactions by group for separate DOs
      const groupTransactions = filteredTransactions.filter(t => t.groupId === selectedGroupId && t.type === 'Out');
      printData = groupTransactions;
    }

    if (printData.length === 0) {
      toast.error('No outbound transactions found for the selected criteria');
      return;
    }

    // Group by group for separate DOs if printing by group
    if (printSelection === 'group') {
      const groupedByGroup = printData.reduce((acc, transaction) => {
        if (!acc[transaction.groupId]) {
          acc[transaction.groupId] = [];
        }
        acc[transaction.groupId].push(transaction);
        return acc;
      }, {} as Record<string, Transaction[]>);

      Object.keys(groupedByGroup).forEach(groupId => {
        printDeliveryOrder(groupedByGroup[groupId], groupId);
      });
    } else {
      printDeliveryOrder(printData, printData[0]?.groupId);
    }

    setShowPrintDialog(false);
    toast.success('Delivery Order(s) sent to printer');
  };

  const printDeliveryOrder = (transactions: Transaction[], groupId: string) => {
    const group = mockGroups.find(g => g.id === groupId);
    const doNumber = `DO-${groupId}-${Date.now()}`;
    
    console.log(`=== DELIVERY ORDER ===`);
    console.log(`DO No: ${doNumber}`);
    console.log(`Date: ${new Date().toLocaleDateString()}`);
    console.log(`Group: ${group?.name || 'Unknown'}`);
    console.log(`========================`);
    
    transactions.forEach(transaction => {
      console.log(`Job ID: ${transaction.jobId}`);
      console.log(`Product: ${transaction.productName}`);
      console.log(`Qty: ${transaction.qty}`);
      console.log(`Driver: ${transaction.driverName}`);
      console.log(`Vehicle: ${transaction.vehicleNo}`);
      console.log(`------------------------`);
    });
    
    console.log(`Checked by: Logistics Staff`);
    console.log(`Approved by: Manager`);
    console.log(`========================`);
  };

  const getTabLabel = (tab: string) => {
    switch (tab) {
      case 'raw-material': return { en: 'Raw Material In/Out', mm: 'ကုန်ကြမ်း ဝင်/ထွက်' };
      case 'finished-goods': return { en: 'Finished Goods In/Out', mm: 'ပြီးစီးထုတ်ကုန် ဝင်/ထွက်' };
      case 'others': return { en: 'Others In/Out', mm: 'အခြား ဝင်/ထွက်' };
      default: return { en: 'Unknown', mm: 'မသိရ' };
    }
  };

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-full mx-auto">
        {/* Header */}
        <div className="bg-white rounded-lg shadow-sm p-6 mb-6">
          <div className="flex items-center justify-between mb-4">
            <div>
              <h1 className="text-2xl font-semibold text-slate-900 flex items-center gap-2">
                <Truck className="h-6 w-6 text-blue-600" />
                Logistic Control | လိုဂျစ်တစ် စီမံခန့်ခွဲမှု
              </h1>
              <p className="text-slate-600">Manage raw materials, finished goods, and other logistics</p>
            </div>
            <div className="flex items-center gap-2">
              <Button onClick={() => setShowAddDialog(true)} className="bg-blue-600 hover:bg-blue-700">
                <Plus className="h-4 w-4 mr-2" />
                Add Transaction | ကုန်သွယ်မှုထည့်ရန်
              </Button>
            </div>
          </div>

          {/* Global Filters */}
          <div className="grid grid-cols-1 md:grid-cols-6 gap-4">
            <div>
              <Label>Date | ရက်စွဲ</Label>
              <Input
                type="date"
                value={dateFilter}
                onChange={(e) => setDateFilter(e.target.value)}
              />
            </div>
            <div>
              <Label>Job ID | အလုပ်အမှတ်</Label>
              <div className="relative">
                <Hash className="h-4 w-4 absolute left-3 top-3 text-slate-400" />
                <Input
                  placeholder="JOB-001..."
                  value={jobIdFilter}
                  onChange={(e) => setJobIdFilter(e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>
            <div>
              <Label>Product | ထုတ်ကုန်</Label>
              <div className="relative">
                <Search className="h-4 w-4 absolute left-3 top-3 text-slate-400" />
                <Input
                  placeholder="Product name..."
                  value={productFilter}
                  onChange={(e) => setProductFilter(e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>
            <div>
              <Label>Group | အုပ်စု</Label>
              <Select value={groupFilter} onValueChange={setGroupFilter}>
                <SelectTrigger>
                  <SelectValue placeholder="All Groups" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Groups</SelectItem>
                  {mockGroups.map(group => (
                    <SelectItem key={group.id} value={group.id}>
                      <div className="flex items-center gap-2">
                        <div 
                          className="w-3 h-3 rounded-full" 
                          style={{ backgroundColor: group.color }}
                        />
                        {group.name}
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label>Status | အခြေအနေ</Label>
              <Select value={statusFilter} onValueChange={setStatusFilter}>
                <SelectTrigger>
                  <SelectValue placeholder="All Status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Status</SelectItem>
                  <SelectItem value="Pending">Pending | စောင့်ဆိုင်းနေသည်</SelectItem>
                  <SelectItem value="Confirmed">Confirmed | အတည်ပြုပြီး</SelectItem>
                  <SelectItem value="Delivered">Delivered | ပေးပို့ပြီး</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="flex items-end gap-2">
              <Button variant="outline" onClick={() => setShowPrintDialog(true)}>
                <Printer className="h-4 w-4 mr-2" />
                Print DO
              </Button>
              <Button variant="outline">
                <Download className="h-4 w-4 mr-2" />
                Export
              </Button>
            </div>
          </div>
        </div>

        {/* Main Content */}
        <div className="bg-white rounded-lg shadow-sm">
          <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
            <div className="border-b border-slate-200 px-6 pt-4">
              <TabsList className="grid w-full grid-cols-3 max-w-2xl">
                <TabsTrigger value="raw-material" className="text-center">
                  <div>
                    <div className="flex items-center justify-center gap-2">
                      <Package className="h-4 w-4" />
                      {getTabLabel('raw-material').en}
                    </div>
                    <div className="text-xs text-slate-500 mt-1">
                      {getTabLabel('raw-material').mm}
                    </div>
                  </div>
                </TabsTrigger>
                <TabsTrigger value="finished-goods" className="text-center">
                  <div>
                    <div className="flex items-center justify-center gap-2">
                      <FileText className="h-4 w-4" />
                      {getTabLabel('finished-goods').en}
                    </div>
                    <div className="text-xs text-slate-500 mt-1">
                      {getTabLabel('finished-goods').mm}
                    </div>
                  </div>
                </TabsTrigger>
                <TabsTrigger value="others" className="text-center">
                  <div>
                    <div className="flex items-center justify-center gap-2">
                      <Tag className="h-4 w-4" />
                      {getTabLabel('others').en}
                    </div>
                    <div className="text-xs text-slate-500 mt-1">
                      {getTabLabel('others').mm}
                    </div>
                  </div>
                </TabsTrigger>
              </TabsList>
            </div>

            {['raw-material', 'finished-goods', 'others'].map(category => (
              <TabsContent key={category} value={category} className="p-6">
                {filteredTransactions.length === 0 ? (
                  <div className="text-center py-12">
                    <Truck className="h-12 w-12 text-slate-300 mx-auto mb-4" />
                    <h3 className="text-lg font-medium text-slate-900 mb-2">
                      No records found for today. Add a transaction.
                    </h3>
                    <p className="text-slate-500 mb-4">
                      မြန်မာ: ယနေ့အတွက် မှတ်တမ်းမရှိပါ။ လိုအပ်သောစာရင်းကို ထည့်သွင်းပါ။
                    </p>
                    <Button onClick={() => setShowAddDialog(true)} className="bg-blue-600 hover:bg-blue-700">
                      <Plus className="h-4 w-4 mr-2" />
                      Add Transaction
                    </Button>
                  </div>
                ) : (
                  <div className="overflow-x-auto">
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Date & Time | ရက်စွဲနှင့်အချိန်</TableHead>
                          <TableHead>Type | အမျိုးအစား</TableHead>
                          <TableHead>Job ID | အလုပ်အမှတ်</TableHead>
                          <TableHead>Product | ထုတ်ကုန်</TableHead>
                          <TableHead>Group | အုပ်စု</TableHead>
                          <TableHead>Qty | အရေအတွက်</TableHead>
                          <TableHead>Driver | ယာဉ်မောင်း</TableHead>
                          <TableHead>Vehicle | ယာဉ်အမှတ်</TableHead>
                          <TableHead>Status | အခြေအနေ</TableHead>
                          <TableHead>Actions | လုပ်ဆောင်ချက်များ</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {filteredTransactions.map(transaction => (
                          <TableRow key={transaction.id}>
                            <TableCell>
                              <div>
                                <div className="font-medium">{transaction.date}</div>
                                <div className="text-sm text-slate-500">{transaction.time}</div>
                              </div>
                            </TableCell>
                            <TableCell>
                              <div className="flex items-center gap-2">
                                {getTransactionIcon(transaction.type)}
                                <span className={`font-medium ${
                                  transaction.type === 'In' ? 'text-green-600' : 'text-blue-600'
                                }`}>
                                  {transaction.type}
                                </span>
                              </div>
                            </TableCell>
                            <TableCell>
                              <Badge variant="outline">{transaction.jobId}</Badge>
                            </TableCell>
                            <TableCell>
                              <div className="max-w-[200px] truncate">
                                {transaction.productName}
                              </div>
                            </TableCell>
                            <TableCell>
                              <div className="flex items-center gap-2">
                                <div 
                                  className="w-3 h-3 rounded-full" 
                                  style={{ 
                                    backgroundColor: mockGroups.find(g => g.id === transaction.groupId)?.color || '#6B7280' 
                                  }}
                                />
                                <span className="font-medium">{transaction.groupName}</span>
                              </div>
                            </TableCell>
                            <TableCell>
                              <span className="font-medium">{transaction.qty.toLocaleString()}</span>
                            </TableCell>
                            <TableCell>{transaction.driverName || '-'}</TableCell>
                            <TableCell>{transaction.vehicleNo || '-'}</TableCell>
                            <TableCell>
                              <Badge className={getStatusColor(transaction.status)}>
                                {transaction.status}
                              </Badge>
                            </TableCell>
                            <TableCell>
                              <div className="flex items-center gap-1">
                                <Button variant="ghost" size="sm">
                                  <Edit className="h-4 w-4" />
                                </Button>
                                <Button variant="ghost" size="sm" className="text-red-600 hover:text-red-700">
                                  <Trash2 className="h-4 w-4" />
                                </Button>
                              </div>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </div>
                )}
              </TabsContent>
            ))}
          </Tabs>
        </div>

        {/* Add Transaction Dialog */}
        <Dialog open={showAddDialog} onOpenChange={setShowAddDialog}>
          <DialogContent className="max-w-md">
            <DialogHeader>
              <DialogTitle>Add Transaction | ကုန်သွယ်မှုထည့်ရန်</DialogTitle>
              <DialogDescription>
                Fill in the transaction details for raw materials, finished goods, or other logistics items.
                | ကုန်ကြမ်း၊ ပြီးစီးထုတ်ကုန် သို့မဟုတ် အခြားလိုဂျစ်တစ်ပစ္စည်းများအတွက် ကုန်သွယ်မှုအသေးစိတ်များကို ဖြည့်စွက်ပါ။
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4">
              <div>
                <Label>Transaction Type | ကုန်သွယ်မှုအမျိုးအစား</Label>
                <Select value={formData.type} onValueChange={(value: 'In' | 'Out') => setFormData(prev => ({ ...prev, type: value }))}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="In">In | ဝင်</SelectItem>
                    <SelectItem value="Out">Out | ထွက်</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label>Job ID | အလုပ်အမှတ်</Label>
                <Select value={formData.jobId || ''} onValueChange={(value) => setFormData(prev => ({ ...prev, jobId: value }))}>
                  <SelectTrigger>
                    <SelectValue placeholder="Select Job..." />
                  </SelectTrigger>
                  <SelectContent>
                    {mockJobs.map(job => (
                      <SelectItem key={job.id} value={job.id}>
                        <div>
                          <div className="font-medium">{job.id}</div>
                          <div className="text-xs text-slate-500">{job.productName}</div>
                          <div className="text-xs text-blue-600">Group: {job.groupName}</div>
                        </div>
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label>Quantity | အရေအတွက်</Label>
                <Input
                  type="number"
                  placeholder="Enter quantity..."
                  value={formData.qty}
                  onChange={(e) => setFormData(prev => ({ ...prev, qty: e.target.value }))}
                />
              </div>

              {formData.type === 'Out' && (
                <>
                  <div>
                    <Label>Driver Name | ယাဉ်မောင်းအမည်</Label>
                    <Input
                      placeholder="Enter driver name..."
                      value={formData.driverName}
                      onChange={(e) => setFormData(prev => ({ ...prev, driverName: e.target.value }))}
                    />
                  </div>

                  <div>
                    <Label>Vehicle Number | ယာဉ်အမှတ်</Label>
                    <Input
                      placeholder="Enter vehicle number..."
                      value={formData.vehicleNo}
                      onChange={(e) => setFormData(prev => ({ ...prev, vehicleNo: e.target.value }))}
                    />
                  </div>
                </>
              )}

              <div>
                <Label>Remarks | မှတ်ချက်</Label>
                <Textarea
                  placeholder="Enter remarks..."
                  value={formData.remarks}
                  onChange={(e) => setFormData(prev => ({ ...prev, remarks: e.target.value }))}
                />
              </div>

              <div className="flex gap-2 pt-4">
                <Button onClick={handleAddTransaction} className="flex-1">
                  Add Transaction
                </Button>
                <Button variant="outline" onClick={() => setShowAddDialog(false)}>
                  Cancel
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>

        {/* Print DO Dialog */}
        <Dialog open={showPrintDialog} onOpenChange={setShowPrintDialog}>
          <DialogContent className="max-w-md">
            <DialogHeader>
              <DialogTitle>Print Delivery Order | ကုန်ပစ္စည်းပေးပို့စာရွက်ထုတ်ရန်</DialogTitle>
              <DialogDescription>
                Generate and print delivery orders for your transactions by job or group.
                | သင့်ကုန်သွယ်မှုများအတွက် အလုပ် သို့မဟုတ် အုပ်စုအလိုက် ကုန်ပစ္စည်းပေးပို့စာရွက်များကို ပြုလုပ်ပြီး ထုတ်ပါ။
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4">
              <div>
                <Label>Select Job ID or Group to print Delivery Order.</Label>
                <p className="text-sm text-slate-500 mb-4">
                  မြန်မာ: ကုန်ပစ္စည်းပေးပို့စာရွက် ထုတ်ရန် အလုပ် ID သို့မဟုတ် အုပ်စုကို ရွေးပါ။
                </p>
              </div>

              <div>
                <Label>Print Option | ထုတ်ရန်ရွေးချယ်မှု</Label>
                <Select value={printSelection} onValueChange={(value: 'job' | 'group') => setPrintSelection(value)}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="job">By Job ID | အလုပ်အမှတ်အလိုက်</SelectItem>
                    <SelectItem value="group">By Group | အုပ်စုအလိုက်</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              {printSelection === 'job' ? (
                <div>
                  <Label>Job ID | အလုပ်အမှတ်</Label>
                  <Select value={selectedJobId || ''} onValueChange={setSelectedJobId}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select Job..." />
                    </SelectTrigger>
                    <SelectContent>
                      {mockJobs.map(job => (
                        <SelectItem key={job.id} value={job.id}>
                          <div>
                            <div className="font-medium">{job.id}</div>
                            <div className="text-xs text-slate-500">{job.productName}</div>
                            <div className="text-xs text-blue-600">Group: {job.groupName}</div>
                          </div>
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              ) : (
                <div>
                  <Label>Group | အုပ်စု</Label>
                  <Select value={selectedGroupId || ''} onValueChange={setSelectedGroupId}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select Group..." />
                    </SelectTrigger>
                    <SelectContent>
                      {mockGroups.map(group => (
                        <SelectItem key={group.id} value={group.id}>
                          <div className="flex items-center gap-2">
                            <div 
                              className="w-3 h-3 rounded-full" 
                              style={{ backgroundColor: group.color }}
                            />
                            {group.name}
                          </div>
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              )}

              <div className="bg-blue-50 p-3 rounded-lg">
                <p className="text-sm text-blue-800">
                  <strong>Note:</strong> If printing by Group, system will automatically create separate DOs for each group to prevent product mixing.
                </p>
                <p className="text-xs text-blue-600 mt-1">
                  မြန်မာ: အုပ်စုအလိုက် ထုတ်လျှင် ထုတ်ကုန်များ မရောမှားရန် စနစ်က အုပ်စုတိုင်းအတွက် ပဲ့ခြင်းစာရွက်များ အလိုအလျောက် ခွဲထုတ်ပေးမည်။
                </p>
              </div>

              <div className="flex gap-2 pt-4">
                <Button onClick={handlePrintDO} className="flex-1">
                  <Printer className="h-4 w-4 mr-2" />
                  Print DO
                </Button>
                <Button variant="outline" onClick={() => setShowPrintDialog(false)}>
                  Cancel
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}