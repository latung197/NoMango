import React, { useState, useEffect } from 'react';
import { Card } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Textarea } from './ui/textarea';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Badge } from './ui/badge';
import { Separator } from './ui/separator';
import { 
  CheckCircle, 
  User, 
  Factory, 
  Clock, 
  Droplets,
  Weight,
  FileDown,
  Filter,
  Calendar,
  Users,
  Settings,
  AlertCircle,
  LogOut,
  RefreshCw
} from 'lucide-react';
import { format } from 'date-fns@4.1.0';

interface GlueRecord {
  id: string;
  operatorName: string;
  operatorId: string;
  machineNo: string;
  glueType: string;
  glueTypeMyanmar: string;
  quantity: number;
  unit: string;
  notes?: string;
  timestamp: Date;
  shift: string;
}

interface Operator {
  id: string;
  name: string;
  nameMyanmar: string;
  photo: string;
  assignedMachines: string[];
  shift: string;
  department: string;
}

interface GlueFillOperatorProps {
  currentView: 'operator' | 'supervisor';
  onViewChange: (view: 'operator' | 'supervisor') => void;
}

export default function GlueFillOperator({ currentView, onViewChange }: GlueFillOperatorProps) {
  const [operatorLoginStep, setOperatorLoginStep] = useState<'login' | 'form' | 'success'>('login');
  const [currentOperator, setCurrentOperator] = useState<Operator | null>(null);
  const [selectedMachine, setSelectedMachine] = useState('');
  const [glueType, setGlueType] = useState('');
  const [quantity, setQuantity] = useState('');
  const [notes, setNotes] = useState('');
  const [glueRecords, setGlueRecords] = useState<GlueRecord[]>([]);
  const [lastRecord, setLastRecord] = useState<GlueRecord | null>(null);
  const [currentTime, setCurrentTime] = useState(new Date());
  const [filterDate, setFilterDate] = useState(format(new Date(), 'yyyy-MM-dd'));
  const [filterMachine, setFilterMachine] = useState('');
  const [filterOperator, setFilterOperator] = useState('');

  // Mock data
  const mockOperators: Operator[] = [
    {
      id: 'OP001',
      name: 'Ko Aung',
      nameMyanmar: 'ကိုအောင်',
      photo: '👨‍🏭',
      assignedMachines: ['MCH-01', 'MCH-02'],
      shift: 'Morning',
      department: 'Production'
    },
    {
      id: 'OP002', 
      name: 'Ma Thida',
      nameMyanmar: 'မသီတာ',
      photo: '👩‍🏭',
      assignedMachines: ['MCH-03'],
      shift: 'Evening',
      department: 'Production'
    },
    {
      id: 'OP003',
      name: 'Ko Zaw',
      nameMyanmar: 'ကိုဇော်',
      photo: '👨‍🏭',
      assignedMachines: ['MCH-04', 'MCH-05'],
      shift: 'Night',
      department: 'Production'
    }
  ];

  const glueTypes = [
    { id: 'PP_BAG_25', name: 'PP Bag – 25kg', nameMyanmar: 'PP အိတ် – ၂၅ကီလို', unit: 'kg' },
    { id: 'PE_PELLET_50', name: 'PE Pellet – 50kg', nameMyanmar: 'PE ပယ်လက် – ၅၀ကီလို', unit: 'kg' },
    { id: 'PVC_POWDER_30', name: 'PVC Powder – 30kg', nameMyanmar: 'PVC အမှုန့် – ၃၀ကီလို', unit: 'kg' },
    { id: 'ADHESIVE_10L', name: 'Adhesive – 10L', nameMyanmar: 'ကော် – ၁၀လီတာ', unit: 'L' },
    { id: 'COLORANT_5L', name: 'Colorant – 5L', nameMyanmar: 'အရောင်ဆေး – ၅လီတာ', unit: 'L' }
  ];

  const machines = ['MCH-01', 'MCH-02', 'MCH-03', 'MCH-04', 'MCH-05', 'MCH-06'];

  // Update current time every second
  useEffect(() => {
    const timer = setInterval(() => setCurrentTime(new Date()), 1000);
    return () => clearInterval(timer);
  }, []);

  // Simulate RFID card scan
  const handleRFIDScan = (operatorId: string) => {
    const operator = mockOperators.find(op => op.id === operatorId);
    if (operator) {
      setCurrentOperator(operator);
      setOperatorLoginStep('form');
      // Auto-select machine if operator has only one assigned
      if (operator.assignedMachines.length === 1) {
        setSelectedMachine(operator.assignedMachines[0]);
      }
    }
  };

  const handleGlueFillSubmit = () => {
    if (!currentOperator || !selectedMachine || !glueType || !quantity) {
      return;
    }

    const selectedGlueType = glueTypes.find(g => g.id === glueType);
    if (!selectedGlueType) return;

    const newRecord: GlueRecord = {
      id: `GF-${Date.now()}`,
      operatorName: currentOperator.name,
      operatorId: currentOperator.id,
      machineNo: selectedMachine,
      glueType: selectedGlueType.name,
      glueTypeMyanmar: selectedGlueType.nameMyanmar,
      quantity: parseFloat(quantity),
      unit: selectedGlueType.unit,
      notes: notes.trim() || undefined,
      timestamp: new Date(),
      shift: currentOperator.shift
    };

    setGlueRecords(prev => [newRecord, ...prev]);
    setLastRecord(newRecord);
    setOperatorLoginStep('success');

    // Reset form
    setGlueType('');
    setQuantity('');
    setNotes('');
  };

  const handleNewEntry = () => {
    setOperatorLoginStep('form');
    setLastRecord(null);
  };

  const handleLogout = () => {
    setCurrentOperator(null);
    setSelectedMachine('');
    setOperatorLoginStep('login');
    setLastRecord(null);
    setGlueType('');
    setQuantity('');
    setNotes('');
  };

  const filteredRecords = glueRecords.filter(record => {
    const dateMatch = !filterDate || format(record.timestamp, 'yyyy-MM-dd') === filterDate;
    const machineMatch = !filterMachine || record.machineNo === filterMachine;
    const operatorMatch = !filterOperator || record.operatorId === filterOperator;
    return dateMatch && machineMatch && operatorMatch;
  });

  const exportToExcel = () => {
    const csvContent = [
      ['Date', 'Time', 'Operator', 'Machine', 'Glue Type', 'Quantity', 'Unit', 'Notes', 'Shift'],
      ...filteredRecords.map(record => [
        format(record.timestamp, 'yyyy-MM-dd'),
        format(record.timestamp, 'HH:mm:ss'),
        record.operatorName,
        record.machineNo,
        record.glueType,
        record.quantity.toString(),
        record.unit,
        record.notes || '',
        record.shift
      ])
    ].map(row => row.join(',')).join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `glue-fill-records-${format(new Date(), 'yyyy-MM-dd')}.csv`;
    a.click();
    URL.revokeObjectURL(url);
  };

  if (currentView === 'supervisor') {
    return (
      <div className="min-h-screen bg-slate-50 p-4">
        <div className="max-w-7xl mx-auto space-y-6">
          {/* Header */}
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-2xl font-semibold text-slate-900">Glue Filling Records</h1>
              <p className="text-slate-600">ကော်ဖြည့်မှတ်တမ်းများ</p>
            </div>
            <div className="flex items-center gap-4">
              <Button onClick={() => onViewChange('operator')} variant="outline">
                <Users className="w-4 h-4 mr-2" />
                Operator View
              </Button>
              <Button onClick={exportToExcel} className="bg-green-600 hover:bg-green-700 text-white">
                <FileDown className="w-4 h-4 mr-2" />
                Export Excel
              </Button>
            </div>
          </div>

          {/* Filters */}
          <Card className="p-6">
            <div className="flex items-center gap-4 mb-4">
              <Filter className="w-5 h-5 text-slate-600" />
              <h3 className="font-medium">Filters / စစ်ထုတ်ရန်</h3>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
              <div>
                <Label>Date / နေ့စွဲ</Label>
                <Input
                  type="date"
                  value={filterDate}
                  onChange={(e) => setFilterDate(e.target.value)}
                />
              </div>
              <div>
                <Label>Machine / စက်</Label>
                <Select value={filterMachine} onValueChange={setFilterMachine}>
                  <SelectTrigger>
                    <SelectValue placeholder="All Machines" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="">All Machines</SelectItem>
                    {machines.map(machine => (
                      <SelectItem key={machine} value={machine}>{machine}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label>Operator / အလုပ်သမား</Label>
                <Select value={filterOperator} onValueChange={setFilterOperator}>
                  <SelectTrigger>
                    <SelectValue placeholder="All Operators" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="">All Operators</SelectItem>
                    {mockOperators.map(operator => (
                      <SelectItem key={operator.id} value={operator.id}>
                        {operator.name} ({operator.nameMyanmar})
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="flex items-end">
                <Button 
                  onClick={() => {
                    setFilterDate(format(new Date(), 'yyyy-MM-dd'));
                    setFilterMachine('');
                    setFilterOperator('');
                  }}
                  variant="outline"
                  className="w-full"
                >
                  <RefreshCw className="w-4 h-4 mr-2" />
                  Reset
                </Button>
              </div>
            </div>
          </Card>

          {/* Records Table */}
          <Card className="p-6">
            <div className="flex items-center justify-between mb-4">
              <h3 className="font-medium">
                Records ({filteredRecords.length}) / မှတ်တမ်းများ ({filteredRecords.length})
              </h3>
              <div className="text-sm text-slate-600">
                Last updated: {format(currentTime, 'HH:mm:ss')}
              </div>
            </div>
            
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Date/Time</TableHead>
                    <TableHead>Operator</TableHead>
                    <TableHead>Machine</TableHead>
                    <TableHead>Glue Type</TableHead>
                    <TableHead>Quantity</TableHead>
                    <TableHead>Shift</TableHead>
                    <TableHead>Notes</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredRecords.length === 0 ? (
                    <TableRow>
                      <TableCell colSpan={7} className="text-center py-8 text-slate-500">
                        No records found / မှတ်တမ်းမတွေ့ပါ
                      </TableCell>
                    </TableRow>
                  ) : (
                    filteredRecords.map((record) => (
                      <TableRow key={record.id}>
                        <TableCell>
                          <div>
                            <div className="font-medium">{format(record.timestamp, 'MMM dd, yyyy')}</div>
                            <div className="text-sm text-slate-500">{format(record.timestamp, 'HH:mm:ss')}</div>
                          </div>
                        </TableCell>
                        <TableCell>
                          <div>
                            <div className="font-medium">{record.operatorName}</div>
                            <div className="text-sm text-slate-500">{record.operatorId}</div>
                          </div>
                        </TableCell>
                        <TableCell>
                          <Badge variant="outline">{record.machineNo}</Badge>
                        </TableCell>
                        <TableCell>
                          <div>
                            <div className="font-medium">{record.glueType}</div>
                            <div className="text-sm text-slate-500">{record.glueTypeMyanmar}</div>
                          </div>
                        </TableCell>
                        <TableCell>
                          <span className="font-medium">{record.quantity} {record.unit}</span>
                        </TableCell>
                        <TableCell>
                          <Badge 
                            variant={
                              record.shift === 'Morning' ? 'default' : 
                              record.shift === 'Evening' ? 'secondary' : 'destructive'
                            }
                          >
                            {record.shift}
                          </Badge>
                        </TableCell>
                        <TableCell className="max-w-xs">
                          {record.notes ? (
                            <span className="text-sm">{record.notes}</span>
                          ) : (
                            <span className="text-slate-400 text-sm">No notes</span>
                          )}
                        </TableCell>
                      </TableRow>
                    ))
                  )}
                </TableBody>
              </Table>
            </div>
          </Card>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-50 p-4">
      <div className="max-w-4xl mx-auto space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-slate-900">Glue Filling Operator</h1>
            <p className="text-slate-600">ကော်ဖြည့်အလုပ်သမား</p>
          </div>
          <div className="flex items-center gap-4">
            <div className="text-right">
              <div className="font-medium">{format(currentTime, 'MMM dd, yyyy')}</div>
              <div className="text-sm text-slate-600">{format(currentTime, 'HH:mm:ss')}</div>
            </div>
            <Button onClick={() => onViewChange('supervisor')} variant="outline">
              <Settings className="w-4 h-4 mr-2" />
              Supervisor View
            </Button>
          </div>
        </div>

        {/* Step 1: RFID Login */}
        {operatorLoginStep === 'login' && (
          <Card className="p-8 text-center">
            <div className="space-y-6">
              <div className="w-24 h-24 mx-auto bg-blue-100 rounded-full flex items-center justify-center">
                <User className="w-12 h-12 text-blue-600" />
              </div>
              <div>
                <h2 className="text-xl font-semibold mb-2">Swipe Your RFID Card</h2>
                <p className="text-slate-600 mb-1">RFID ကတ်ကို ဖတ်ပါ</p>
                <p className="text-sm text-slate-500">Please swipe your RFID card to login</p>
              </div>
              
              <Separator />
              
              <div>
                <p className="text-sm text-slate-600 mb-4">Demo: Click an operator to simulate RFID scan</p>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                  {mockOperators.map((operator) => (
                    <Button
                      key={operator.id}
                      onClick={() => handleRFIDScan(operator.id)}
                      variant="outline"
                      className="h-20 flex flex-col items-center justify-center space-y-2 hover:bg-blue-50"
                    >
                      <div className="text-2xl">{operator.photo}</div>
                      <div className="text-center">
                        <div className="font-medium">{operator.name}</div>
                        <div className="text-xs text-slate-500">{operator.nameMyanmar}</div>
                      </div>
                    </Button>
                  ))}
                </div>
              </div>
            </div>
          </Card>
        )}

        {/* Step 2: Operator Info + Glue Fill Form */}
        {operatorLoginStep === 'form' && currentOperator && (
          <div className="space-y-6">
            {/* Operator Info */}
            <Card className="p-6">
              <div className="flex items-center space-x-4">
                <div className="text-4xl">{currentOperator.photo}</div>
                <div className="flex-1">
                  <h3 className="font-semibold text-lg">
                    {currentOperator.name} ({currentOperator.nameMyanmar})
                  </h3>
                  <div className="flex items-center space-x-6 text-sm text-slate-600 mt-1">
                    <div className="flex items-center space-x-1">
                      <Factory className="w-4 h-4" />
                      <span>Machines: {currentOperator.assignedMachines.join(', ')}</span>
                    </div>
                    <div className="flex items-center space-x-1">
                      <Clock className="w-4 h-4" />
                      <span>Shift: {currentOperator.shift}</span>
                    </div>
                  </div>
                </div>
                <Button onClick={handleLogout} variant="outline" size="sm">
                  <LogOut className="w-4 h-4 mr-2" />
                  Logout
                </Button>
              </div>
            </Card>

            {/* Machine Selection */}
            <Card className="p-6">
              <h3 className="font-medium mb-4 flex items-center">
                <Factory className="w-5 h-5 mr-2 text-blue-600" />
                Select Machine / စက်ရွေးပါ
              </h3>
              
              {currentOperator.assignedMachines.length === 1 ? (
                <div className="p-4 bg-blue-50 rounded-lg border border-blue-200">
                  <Badge variant="outline" className="text-lg px-4 py-2">
                    {currentOperator.assignedMachines[0]} (Auto-selected)
                  </Badge>
                </div>
              ) : (
                <div className="grid grid-cols-2 gap-4">
                  {currentOperator.assignedMachines.map((machine) => (
                    <Button
                      key={machine}
                      onClick={() => setSelectedMachine(machine)}
                      variant={selectedMachine === machine ? 'default' : 'outline'}
                      className="h-16 text-lg font-medium"
                    >
                      {machine}
                    </Button>
                  ))}
                </div>
              )}
            </Card>

            {/* Glue Fill Form */}
            <Card className="p-6">
              <h3 className="font-medium mb-6 flex items-center">
                <Droplets className="w-5 h-5 mr-2 text-blue-600" />
                Glue Fill Information / ကော်ဖြည့်ခြင်းအချက်အလက်
              </h3>

              <div className="space-y-6">
                {/* Glue Type Selection */}
                <div>
                  <Label className="text-base font-medium">
                    Glue Type / ကော်အမျိုးအစား *
                  </Label>
                  <Select value={glueType} onValueChange={setGlueType}>
                    <SelectTrigger className={`h-12 mt-2 ${!glueType ? 'border-red-300 bg-red-50' : ''}`}>
                      <SelectValue placeholder="Select glue type / ကော်အမျိုးအစားရွေးပါ" />
                    </SelectTrigger>
                    <SelectContent>
                      {glueTypes.map((type) => (
                        <SelectItem key={type.id} value={type.id}>
                          <div>
                            <div className="font-medium">{type.name}</div>
                            <div className="text-sm text-slate-500">{type.nameMyanmar}</div>
                          </div>
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  {!glueType && (
                    <p className="text-red-600 text-sm mt-1 flex items-center">
                      <AlertCircle className="w-4 h-4 mr-1" />
                      Please select glue type (အမျိုးအစားရွေးပါ)
                    </p>
                  )}
                </div>

                {/* Quantity Input */}
                <div>
                  <Label className="text-base font-medium flex items-center">
                    <Weight className="w-4 h-4 mr-2" />
                    Quantity / ပမာဏ *
                  </Label>
                  <div className="flex mt-2">
                    <Input
                      type="number"
                      value={quantity}
                      onChange={(e) => setQuantity(e.target.value)}
                      placeholder="Enter quantity / ပမာဏထည့်ပါ"
                      className={`h-12 text-lg ${!quantity ? 'border-red-300 bg-red-50' : ''}`}
                      min="0"
                      step="0.1"
                    />
                    <div className="ml-3 h-12 px-4 bg-slate-100 border border-l-0 rounded-r-md flex items-center">
                      <span className="font-medium text-slate-700">
                        {glueType ? glueTypes.find(g => g.id === glueType)?.unit || 'Unit' : 'Unit'}
                      </span>
                    </div>
                  </div>
                  {!quantity && (
                    <p className="text-red-600 text-sm mt-1 flex items-center">
                      <AlertCircle className="w-4 h-4 mr-1" />
                      Please enter quantity (ปမာဏထည့်ပါ)
                    </p>
                  )}
                </div>

                {/* Optional Notes */}
                <div>
                  <Label className="text-base font-medium">
                    Notes (Optional) / မှတ်ချက် (ရွေးချယ်စရာ)
                  </Label>
                  <Textarea
                    value={notes}
                    onChange={(e) => setNotes(e.target.value)}
                    placeholder="Add notes if needed / လိုအပ်ပါက မှတ်ချက်ထည့်ပါ"
                    className="mt-2 h-20"
                    maxLength={200}
                  />
                  <p className="text-xs text-slate-500 mt-1">{notes.length}/200 characters</p>
                </div>

                {/* Submit Button */}
                <Button
                  onClick={handleGlueFillSubmit}
                  disabled={!selectedMachine || !glueType || !quantity || parseFloat(quantity) <= 0}
                  className="w-full h-16 text-lg font-semibold bg-green-600 hover:bg-green-700 disabled:bg-slate-300"
                >
                  <CheckCircle className="w-6 h-6 mr-3" />
                  ✅ Confirm Glue Fill / ကော်ဖြည့်ခြင်းအတည်ပြုပါ
                </Button>
              </div>
            </Card>
          </div>
        )}

        {/* Step 3: Success Confirmation */}
        {operatorLoginStep === 'success' && lastRecord && (
          <Card className="p-8 text-center">
            <div className="space-y-6">
              <div className="w-24 h-24 mx-auto bg-green-100 rounded-full flex items-center justify-center">
                <CheckCircle className="w-12 h-12 text-green-600" />
              </div>
              
              <div>
                <h2 className="text-xl font-semibold text-green-800 mb-2">
                  Glue Fill Recorded Successfully!
                </h2>
                <p className="text-green-700">ကော်ဖြည့်မှတ်တမ်း အောင်မြင်စွာ သိမ်းဆည်းပြီးပါပြီ!</p>
              </div>

              <div className="bg-green-50 border border-green-200 rounded-lg p-6 text-left">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <p className="text-sm text-slate-600 mb-1">Operator / အလုပ်သမား</p>
                    <p className="font-medium">{lastRecord.operatorName}</p>
                  </div>
                  <div>
                    <p className="text-sm text-slate-600 mb-1">Machine / စက်</p>
                    <p className="font-medium">{lastRecord.machineNo}</p>
                  </div>
                  <div>
                    <p className="text-sm text-slate-600 mb-1">Glue Type / ကော်အမျိုးအစား</p>
                    <p className="font-medium">{lastRecord.glueType}</p>
                    <p className="text-sm text-slate-500">{lastRecord.glueTypeMyanmar}</p>
                  </div>
                  <div>
                    <p className="text-sm text-slate-600 mb-1">Quantity / ပမာဏ</p>
                    <p className="font-medium">{lastRecord.quantity} {lastRecord.unit}</p>
                  </div>
                  <div className="col-span-2">
                    <p className="text-sm text-slate-600 mb-1">Time / အချိန်</p>
                    <p className="font-medium">{format(lastRecord.timestamp, 'MMM dd, yyyy HH:mm:ss')}</p>
                  </div>
                  {lastRecord.notes && (
                    <div className="col-span-2">
                      <p className="text-sm text-slate-600 mb-1">Notes / မှတ်ချက်</p>
                      <p className="font-medium">{lastRecord.notes}</p>
                    </div>
                  )}
                </div>
              </div>

              <div className="flex space-x-4">
                <Button
                  onClick={handleNewEntry}
                  className="flex-1 h-12 bg-blue-600 hover:bg-blue-700"
                >
                  Add Another Entry / ထပ်ထည့်ရန်
                </Button>
                <Button
                  onClick={handleLogout}
                  variant="outline"
                  className="flex-1 h-12"
                >
                  <LogOut className="w-5 h-5 mr-2" />
                  Logout / ထွက်ရန်
                </Button>
              </div>
            </div>
          </Card>
        )}
      </div>
    </div>
  );
}