import React, { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Alert, AlertDescription } from './ui/alert';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';
import { 
  Monitor,
  Factory,
  User,
  AlertTriangle,
  TrendingUp,
  TrendingDown,
  RefreshCw,
  Wifi,
  WifiOff,
  Clock,
  Activity,
  Zap,
  ArrowLeft,
  Eye
} from 'lucide-react';

// Mock data for machines
const mockMachines = [
  {
    id: 'INJ-M001',
    name: 'Injection Machine 01',
    nameLocal: 'ဖိအားစက် ၀၁',
    jobId: 'JOB20250907-001',
    product: '2011 - Plastic Bottle 500ml',
    productLocal: 'ပလတ်စတစ်ပုလင်း ၅၀၀မီလီ',
    operator: { name: 'Ko Thant', id: 'OP001' },
    status: 'running' as const,
    targetQty: 10000,
    currentQty: 8750,
    efficiency: 87.5,
    temperature: 180,
    pressure: 125,
    cycleTime: 45,
    downtime: 0,
    lastUpdate: '2025-09-07 15:32:15'
  },
  {
    id: 'INJ-M002',
    name: 'Injection Machine 02',
    nameLocal: 'ဖိအားစက် ၀၂',
    jobId: 'JOB20250907-002',
    product: '2013 - Plastic Cup 250ml',
    productLocal: 'ပလတ်စတစ်ခွက် ၂၅၀မီလီ',
    operator: { name: 'Ma Hla', id: 'OP002' },
    status: 'running' as const,
    targetQty: 8000,
    currentQty: 7200,
    efficiency: 90.0,
    temperature: 175,
    pressure: 120,
    cycleTime: 42,
    downtime: 0,
    lastUpdate: '2025-09-07 15:31:45'
  },
  {
    id: 'INJ-M003',
    name: 'Injection Machine 03',
    nameLocal: 'ဖိအားစက် ၀၃',
    jobId: 'JOB20250907-003',
    product: '2015 - Plastic Container 1L',
    productLocal: 'ပလတ်စတစ်ပုံး ၁ လီတာ',
    operator: { name: 'Ko Aung', id: 'OP003' },
    status: 'idle' as const,
    targetQty: 5000,
    currentQty: 5000,
    efficiency: 100.0,
    temperature: 160,
    pressure: 0,
    cycleTime: 0,
    downtime: 15,
    lastUpdate: '2025-09-07 15:15:00'
  },
  {
    id: 'INJ-M004',
    name: 'Injection Machine 04',
    nameLocal: 'ဖိအားစက် ၀၄',
    jobId: 'JOB20250907-004',
    product: '2017 - Plastic Tray 300x200mm',
    productLocal: 'ပလတ်စတစ်ပန်းကန် ၃၀၀x၂၀၀မီမီ',
    operator: { name: 'Ma Su', id: 'OP004' },
    status: 'running' as const,
    targetQty: 3000,
    currentQty: 2100,
    efficiency: 70.0,
    temperature: 185,
    pressure: 130,
    cycleTime: 52,
    downtime: 0,
    lastUpdate: '2025-09-07 15:30:22'
  },
  {
    id: 'INJ-M005',
    name: 'Injection Machine 05',
    nameLocal: 'ဖိအားစက် ၀၅',
    jobId: null,
    product: null,
    productLocal: null,
    operator: null,
    status: 'down' as const,
    targetQty: 0,
    currentQty: 0,
    efficiency: 0,
    temperature: 0,
    pressure: 0,
    cycleTime: 0,
    downtime: 45,
    lastUpdate: '2025-09-07 14:47:00'
  }
];

// Mock data for charts
const outputChartData = [
  { machine: 'INJ-M001', target: 10000, actual: 8750, efficiency: 87.5 },
  { machine: 'INJ-M002', target: 8000, actual: 7200, efficiency: 90.0 },
  { machine: 'INJ-M003', target: 5000, actual: 5000, efficiency: 100.0 },
  { machine: 'INJ-M004', target: 3000, actual: 2100, efficiency: 70.0 },
  { machine: 'INJ-M005', target: 0, actual: 0, efficiency: 0 }
];

const downtimeReasons = [
  { name: 'Mold Change', nameLocal: 'မှိုပြောင်းခြင်း', value: 35, color: '#ef4444' },
  { name: 'Material Issue', nameLocal: 'ပစ္စည်းပြဿနာ', value: 25, color: '#f97316' },
  { name: 'Maintenance', nameLocal: 'ပြုပြင်ထိန်းသိမ်းမှု', value: 20, color: '#eab308' },
  { name: 'Quality Check', nameLocal: 'အရည်အသွေးစစ်ဆေးမှု', value: 15, color: '#3b82f6' },
  { name: 'Other', nameLocal: 'အခြား', value: 5, color: '#6b7280' }
];

export function LiveMonitoring() {
  const [machines, setMachines] = useState(mockMachines);
  const [currentTime, setCurrentTime] = useState(new Date());
  const [alerts, setAlerts] = useState([
    {
      id: 1,
      type: 'critical' as const,
      machine: 'INJ-M005',
      message: 'Machine INJ-M005 down for 45 minutes',
      messageLocal: 'စက် INJ-M005 သည် ၄၅ မိနစ်ကြာ ပျက်နေပြီ',
      timestamp: '2025-09-07 14:47:00'
    },
    {
      id: 2,
      type: 'warning' as const,
      machine: 'INJ-M004',
      message: 'Machine INJ-M004 efficiency below 75% (70%)',
      messageLocal: 'စက် INJ-M004 ၏ ထိရောက်မှု ၇၅% အောက်သို့ကျ (၇၀%)',
      timestamp: '2025-09-07 15:20:00'
    },
    {
      id: 3,
      type: 'info' as const,
      machine: 'INJ-M001',
      message: 'Target reached 87.5% for Job JOB20250907-001',
      messageLocal: 'အလုပ် JOB20250907-001 အတွက် ပစ်မှတ် ၈၇.၅% မှီပြီ',
      timestamp: '2025-09-07 15:30:00'
    }
  ]);

  // Update time every second
  useEffect(() => {
    const interval = setInterval(() => {
      setCurrentTime(new Date());
    }, 1000);
    return () => clearInterval(interval);
  }, []);

  // Auto-refresh data every 30 seconds
  useEffect(() => {
    const interval = setInterval(() => {
      // Simulate data updates
      setMachines(prev => prev.map(machine => ({
        ...machine,
        currentQty: machine.status === 'running' ? 
          Math.min(machine.targetQty, machine.currentQty + Math.floor(Math.random() * 10)) : 
          machine.currentQty,
        lastUpdate: new Date().toISOString().replace('T', ' ').substring(0, 19)
      })));
    }, 30000);
    return () => clearInterval(interval);
  }, []);

  const getStatusConfig = (status: string) => {
    switch (status) {
      case 'running':
        return { 
          color: 'bg-green-500', 
          textColor: 'text-green-800',
          bgColor: 'bg-green-100',
          icon: '🟢',
          text: 'RUNNING',
          textLocal: 'လုပ်ငန်းလည်ပတ်နေသည်'
        };
      case 'idle':
        return { 
          color: 'bg-yellow-500', 
          textColor: 'text-yellow-800',
          bgColor: 'bg-yellow-100',
          icon: '🟡',
          text: 'IDLE',
          textLocal: 'အလုပ်မရှိ'
        };
      case 'down':
        return { 
          color: 'bg-red-500', 
          textColor: 'text-red-800',
          bgColor: 'bg-red-100',
          icon: '🔴',
          text: 'DOWN',
          textLocal: 'စက်ပျက်နေသည်'
        };
      default:
        return { 
          color: 'bg-gray-500', 
          textColor: 'text-gray-800',
          bgColor: 'bg-gray-100',
          icon: '⚪',
          text: 'UNKNOWN',
          textLocal: 'မသိ'
        };
    }
  };

  const getAlertConfig = (type: string) => {
    switch (type) {
      case 'critical':
        return { color: 'border-red-200 bg-red-50', textColor: 'text-red-800', icon: '🔴' };
      case 'warning':
        return { color: 'border-yellow-200 bg-yellow-50', textColor: 'text-yellow-800', icon: '🟡' };
      case 'info':
        return { color: 'border-blue-200 bg-blue-50', textColor: 'text-blue-800', icon: '🔵' };
      default:
        return { color: 'border-gray-200 bg-gray-50', textColor: 'text-gray-800', icon: '⚪' };
    }
  };

  const runningMachines = machines.filter(m => m.status === 'running').length;
  const idleMachines = machines.filter(m => m.status === 'idle').length;
  const downMachines = machines.filter(m => m.status === 'down').length;
  const totalOutput = machines.reduce((sum, m) => sum + m.currentQty, 0);
  const totalTarget = machines.reduce((sum, m) => sum + m.targetQty, 0);
  const overallEfficiency = totalTarget > 0 ? (totalOutput / totalTarget) * 100 : 0;

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-purple-100 rounded-lg">
              <Eye className="h-6 w-6 text-purple-600" />
            </div>
            <div>
              <h1 className="text-3xl font-semibold text-slate-900">
                Live Monitoring | တိုက်ရိုက်စောင့်ကြည့်ခြင်း
              </h1>
              <p className="text-slate-600 mt-1">
                Real-time machine status and production monitoring
              </p>
              <p className="text-sm text-slate-500">
                စက်အခြေအနေနှင့် ထုတ်လုပ်မှု တိုက်ရိုက်စောင့်ကြည့်မှု
              </p>
            </div>
          </div>
          
          <div className="text-right">
            <div className="text-sm text-slate-500">Current Time | လက်ရှိအချိန်</div>
            <div className="text-xl font-mono font-bold text-slate-900">
              {currentTime.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', second: '2-digit' })}
            </div>
            <div className="text-base text-slate-600">
              {currentTime.toLocaleDateString('en-US', { weekday: 'long', month: 'short', day: 'numeric' })}
            </div>
          </div>
        </div>

        {/* Overall Stats */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-green-100 rounded-lg">
                  <div className="w-3 h-3 bg-green-500 rounded-full"></div>
                </div>
                <div>
                  <div className="text-2xl font-bold text-green-700">{runningMachines}</div>
                  <div className="text-sm text-slate-600">Running Machines</div>
                  <div className="text-xs text-slate-500">လည်ပတ်နေသောစက်များ</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-yellow-100 rounded-lg">
                  <div className="w-3 h-3 bg-yellow-500 rounded-full"></div>
                </div>
                <div>
                  <div className="text-2xl font-bold text-yellow-700">{idleMachines}</div>
                  <div className="text-sm text-slate-600">Idle Machines</div>
                  <div className="text-xs text-slate-500">အလုပ်မရှိသောစက်များ</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-red-100 rounded-lg">
                  <div className="w-3 h-3 bg-red-500 rounded-full"></div>
                </div>
                <div>
                  <div className="text-2xl font-bold text-red-700">{downMachines}</div>
                  <div className="text-sm text-slate-600">Down Machines</div>
                  <div className="text-xs text-slate-500">ပျက်နေသောစက်များ</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-blue-100 rounded-lg">
                  <TrendingUp className="w-4 h-4 text-blue-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-blue-700">{overallEfficiency.toFixed(1)}%</div>
                  <div className="text-sm text-slate-600">Overall OEE</div>
                  <div className="text-xs text-slate-500">စုစုပေါင်းထိရောက်မှု</div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Alerts Section */}
        {alerts.length > 0 && (
          <Card className="border-0 shadow-sm">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <AlertTriangle className="h-5 w-5 text-orange-600" />
                <div>
                  <div>Active Alerts | တက်ကြွသောသတိပေးချက်များ</div>
                  <div className="text-sm text-slate-500">Real-time system alerts and notifications</div>
                </div>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                {alerts.map(alert => {
                  const config = getAlertConfig(alert.type);
                  return (
                    <Alert key={alert.id} className={config.color}>
                      <AlertDescription className={config.textColor}>
                        <div className="flex items-start gap-3">
                          <span className="text-lg">{config.icon}</span>
                          <div className="flex-1">
                            <div className="flex items-center gap-2 mb-1">
                              <span className="font-medium">{alert.machine}</span>
                              <span className="text-xs opacity-70">{alert.timestamp}</span>
                            </div>
                            <div className="mb-1">{alert.message}</div>
                            <div className="text-sm opacity-80">{alert.messageLocal}</div>
                          </div>
                        </div>
                      </AlertDescription>
                    </Alert>
                  );
                })}
              </div>
            </CardContent>
          </Card>
        )}

        {/* Machine Status Table */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <Factory className="h-5 w-5 text-blue-600" />
                <div>
                  <div>Machine Status Overview | စက်အခြေအနေခြုံငုံကြည့်ရှုမှု</div>
                  <div className="text-sm text-slate-500">Real-time status of all production machines</div>
                </div>
              </div>
              <Button variant="outline" size="sm" className="gap-2">
                <RefreshCw className="h-4 w-4" />
                Refresh | ပြန်လည်ရယူ
              </Button>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Machine | စက်</TableHead>
                    <TableHead>Job ID</TableHead>
                    <TableHead>Product | ထုတ်ကုန်</TableHead>
                    <TableHead>Target vs Done</TableHead>
                    <TableHead>Operator | အော်ပရေတာ</TableHead>
                    <TableHead>Status | အခြေအနေ</TableHead>
                    <TableHead>Efficiency | ထိရောက်မှု</TableHead>
                    <TableHead>Last Update</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {machines.map(machine => {
                    const statusConfig = getStatusConfig(machine.status);
                    const progress = machine.targetQty > 0 ? (machine.currentQty / machine.targetQty) * 100 : 0;
                    
                    return (
                      <TableRow key={machine.id}>
                        <TableCell>
                          <div>
                            <div className="font-medium">{machine.id}</div>
                            <div className="text-sm text-slate-500">{machine.nameLocal}</div>
                          </div>
                        </TableCell>
                        <TableCell>
                          <div className="font-mono text-sm">
                            {machine.jobId || <span className="text-slate-400">No Job</span>}
                          </div>
                        </TableCell>
                        <TableCell>
                          {machine.product ? (
                            <div>
                              <div className="font-medium text-sm">{machine.product}</div>
                              <div className="text-xs text-slate-500">{machine.productLocal}</div>
                            </div>
                          ) : (
                            <span className="text-slate-400">No Product</span>
                          )}
                        </TableCell>
                        <TableCell>
                          {machine.targetQty > 0 ? (
                            <div className="space-y-1">
                              <div className="flex items-center justify-between text-sm">
                                <span>{machine.currentQty.toLocaleString()}</span>
                                <span className="text-slate-500">/ {machine.targetQty.toLocaleString()}</span>
                              </div>
                              <Progress value={progress} className="h-2" />
                              <div className="text-xs text-slate-500 text-center">
                                {progress.toFixed(1)}%
                              </div>
                            </div>
                          ) : (
                            <span className="text-slate-400">No Target</span>
                          )}
                        </TableCell>
                        <TableCell>
                          {machine.operator ? (
                            <div className="flex items-center gap-2">
                              <User className="h-3 w-3 text-slate-400" />
                              <div>
                                <div className="text-sm font-medium">{machine.operator.name}</div>
                                <div className="text-xs text-slate-500">{machine.operator.id}</div>
                              </div>
                            </div>
                          ) : (
                            <span className="text-slate-400">No Operator</span>
                          )}
                        </TableCell>
                        <TableCell>
                          <Badge className={`${statusConfig.bgColor} ${statusConfig.textColor}`}>
                            <span className="mr-1">{statusConfig.icon}</span>
                            <div>
                              <div>{statusConfig.text}</div>
                              <div className="text-xs">{statusConfig.textLocal}</div>
                            </div>
                          </Badge>
                          {machine.status === 'down' && machine.downtime > 0 && (
                            <div className="text-xs text-red-600 mt-1">
                              Down {machine.downtime}m
                            </div>
                          )}
                        </TableCell>
                        <TableCell>
                          <div className="flex items-center gap-2">
                            <div className="text-lg font-bold text-slate-900">
                              {machine.efficiency.toFixed(1)}%
                            </div>
                            {machine.efficiency >= 85 ? (
                              <TrendingUp className="h-4 w-4 text-green-600" />
                            ) : machine.efficiency >= 70 ? (
                              <Activity className="h-4 w-4 text-yellow-600" />
                            ) : (
                              <TrendingDown className="h-4 w-4 text-red-600" />
                            )}
                          </div>
                        </TableCell>
                        <TableCell>
                          <div className="text-xs text-slate-500 font-mono">
                            {machine.lastUpdate}
                          </div>
                        </TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>

        {/* Charts Section */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Output vs Target Chart */}
          <Card className="border-0 shadow-sm">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <BarChart className="h-5 w-5 text-blue-600" />
                <div>
                  <div>Machine Output vs Target</div>
                  <div className="text-sm text-slate-500">စက်ထုတ်လုပ်မှု နှိုင်းယှဉ် ပစ်မှတ်</div>
                </div>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart data={outputChartData}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="machine" />
                    <YAxis />
                    <Tooltip />
                    <Bar dataKey="target" fill="#e2e8f0" name="Target" />
                    <Bar dataKey="actual" fill="#3b82f6" name="Actual" />
                  </BarChart>
                </ResponsiveContainer>
              </div>
            </CardContent>
          </Card>

          {/* Downtime Reasons Pie Chart */}
          <Card className="border-0 shadow-sm">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <PieChart className="h-5 w-5 text-orange-600" />
                <div>
                  <div>Top 5 Downtime Reasons</div>
                  <div className="text-sm text-slate-500">စက်ရပ်တန့်ရသည့် အကြောင်းရင်း ၅ ခု</div>
                </div>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={downtimeReasons}
                      cx="50%"
                      cy="50%"
                      outerRadius={80}
                      fill="#8884d8"
                      dataKey="value"
                      label={({ name, value }) => `${name}: ${value}%`}
                    >
                      {downtimeReasons.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={entry.color} />
                      ))}
                    </Pie>
                    <Tooltip />
                  </PieChart>
                </ResponsiveContainer>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}