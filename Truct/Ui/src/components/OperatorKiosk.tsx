import React, { useState, useEffect } from 'react';
import { Card, CardContent } from './ui/card';
import { Button } from './ui/button';
import { Progress } from './ui/progress';
import { Badge } from './ui/badge';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { 
  User, 
  Scan, 
  Plus, 
  Minus, 
  Check, 
  AlertTriangle,
  Clock,
  Package,
  Coffee,
  PlayCircle,
  LogOut,
  UserCheck,
  Activity,
  Settings,
  CheckCircle,
  Wifi,
  Timer
} from 'lucide-react';

// Types
type MachineStatus = 'running' | 'idle' | 'down';
type OperatorStatus = 'logged-out' | 'logged-in' | 'on-break';

interface Operator {
  id: string;
  name: string;
  rfidId: string;
  photo?: string;
  shift: string;
}

interface ActivityLog {
  id: string;
  time: string;
  action: string;
  actionMm: string;
  icon: string;
}

interface ProductInfo {
  code: string;
  name: string;
  nameLocal: string;
  target: number;
  completed: number;
  defects: number;
  leftoverMaterial: number;
}

interface BreakdownReason {
  code: string;
  name: string;
  nameLocal: string;
  icon: string;
}

export function OperatorKiosk() {
  // Core state management
  const [machineStatus, setMachineStatus] = useState<MachineStatus>('running');
  const [operatorStatus, setOperatorStatus] = useState<OperatorStatus>('logged-in');
  const [currentTime, setCurrentTime] = useState(new Date());
  const [showRFIDLogin, setShowRFIDLogin] = useState(false);
  const [showMachineDownDialog, setShowMachineDownDialog] = useState(false);
  const [showQCApproval, setShowQCApproval] = useState(false);

  // Session data
  const [currentOperator, setCurrentOperator] = useState<Operator>({
    id: 'EMP001',
    name: 'John Doe',
    rfidId: 'RF001',
    shift: 'Day Shift'
  });
  
  const [sessionStartTime] = useState(new Date(Date.now() - 4 * 60 * 60 * 1000)); // 4 hours ago
  const [breakStartTime, setBreakStartTime] = useState<Date | null>(null);
  const [machineDownTime, setMachineDownTime] = useState<Date | null>(null);
  const [sessionTime, setSessionTime] = useState(14400); // 4 hours in seconds
  const [breakTime, setBreakTime] = useState(0);
  const [downTime, setDownTime] = useState(0);

  // Production data
  const [productInfo, setProductInfo] = useState<ProductInfo>({
    code: 'PC-001',
    name: 'Plastic Container 500ml',
    nameLocal: 'ပလတ်စတစ်ပုလင်း ၅၀၀မီလီ',
    target: 500,
    completed: 347,
    defects: 3,
    leftoverMaterial: 25
  });

  // Quantity entry
  const [entryQuantity, setEntryQuantity] = useState(0);
  const [selectedBreakdownReason, setSelectedBreakdownReason] = useState('');

  // Activity log
  const [activityLog] = useState<ActivityLog[]>([
    { id: '1', time: '08:15', action: 'Shift Started', actionMm: 'အလုပ်ပတ်စတင်', icon: '🟢' },
    { id: '2', time: '10:30', action: 'Submitted +25 pieces', actionMm: '+25 ခု တင်သွင်းပြီး', icon: '📝' },
    { id: '3', time: '12:20', action: 'Break Started', actionMm: 'အနားယူစတင်', icon: '⏸' },
    { id: '4', time: '12:35', action: 'Resumed Work', actionMm: 'အလုပ်ပြန်စတင်', icon: '▶️' },
    { id: '5', time: '14:10', action: 'Submitted +18 pieces', actionMm: '+18 ခု တင်သွင်းပြီး', icon: '📝' },
    { id: '6', time: '15:45', action: 'QC Check Passed', actionMm: 'QC စစ်ဆေးမှုအောင်မြင်', icon: '✅' },
    { id: '7', time: '16:20', action: 'Material Refilled', actionMm: 'ပစ္စည်းအဖြည့်', icon: '📦' },
    { id: '8', time: '16:45', action: 'Temperature Adjusted', actionMm: 'အပူချိန်ပြင်ဆင်', icon: '🌡️' }
  ]);

  // Breakdown reasons
  const breakdownReasons: BreakdownReason[] = [
    { code: 'MOLD_JAM', name: 'Mold Jam', nameLocal: 'မှိုပိတ်ခြင်း', icon: '🔧' },
    { code: 'POWER_FAILURE', name: 'Power Failure', nameLocal: 'လျှပ်စစ်ပျက်ခြင်း', icon: '⚡' },
    { code: 'MATERIAL_SHORTAGE', name: 'Material Shortage', nameLocal: 'ပစ္စည်းမလုံလောက်ခြင်း', icon: '📦' },
    { code: 'TEMPERATURE_ISSUE', name: 'Temperature Issue', nameLocal: 'အပူချိန်ပြဿနာ', icon: '🌡️' },
    { code: 'MECHANICAL_FAULT', name: 'Mechanical Fault', nameLocal: 'စက်ပိုင်းဆိုင်ရာချို့ယွင်းမှု', icon: '⚙️' },
    { code: 'OTHER', name: 'Other Issue', nameLocal: 'အခြားပြဿနာ', icon: '❓' }
  ];

  // Timer effect
  useEffect(() => {
    const interval = setInterval(() => {
      setCurrentTime(new Date());
      
      if (operatorStatus === 'logged-in' && !breakStartTime) {
        const now = new Date();
        setSessionTime(Math.floor((now.getTime() - sessionStartTime.getTime()) / 1000));
      }
      
      if (operatorStatus === 'on-break' && breakStartTime) {
        const now = new Date();
        setBreakTime(Math.floor((now.getTime() - breakStartTime.getTime()) / 1000));
      }
      
      if (machineStatus === 'down' && machineDownTime) {
        const now = new Date();
        setDownTime(Math.floor((now.getTime() - machineDownTime.getTime()) / 1000));
      }
    }, 1000);

    return () => clearInterval(interval);
  }, [operatorStatus, sessionStartTime, breakStartTime, machineStatus, machineDownTime]);

  // Utility functions
  const formatTime = (seconds: number) => {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const secs = seconds % 60;
    return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  };

  const getMachineStatusConfig = () => {
    switch (machineStatus) {
      case 'running':
        return { color: 'bg-green-500', text: '🟢 RUNNING', textLocal: 'လုপ်ငန်းလည်ပတ်နေသည်' };
      case 'idle':
        return { color: 'bg-yellow-500', text: '🟡 IDLE', textLocal: 'အလုပ်မရှိ' };
      case 'down':
        return { color: 'bg-red-500', text: '🔴 DOWN', textLocal: 'စက်ပျက်နေသည်' };
      default:
        return { color: 'bg-gray-500', text: 'UNKNOWN', textLocal: 'မသိ' };
    }
  };

  // Event handlers
  const handleQuantityChange = (change: number) => {
    setEntryQuantity(prev => Math.max(0, prev + change));
  };

  const handleSubmitQuantity = () => {
    if (entryQuantity > 0 && machineStatus === 'running') {
      setProductInfo(prev => ({
        ...prev,
        completed: prev.completed + entryQuantity
      }));
      setEntryQuantity(0);
      
      // Check if target is reached
      if (productInfo.completed + entryQuantity >= productInfo.target) {
        setShowQCApproval(true);
      }
    }
  };

  const handleStartBreak = () => {
    setOperatorStatus('on-break');
    setBreakStartTime(new Date());
    setMachineStatus('idle');
  };

  const handleResumeFromBreak = () => {
    setOperatorStatus('logged-in');
    setBreakStartTime(null);
    setBreakTime(0);
    setMachineStatus('running');
  };

  const handleMachineDown = () => {
    setMachineStatus('down');
    setMachineDownTime(new Date());
    setShowMachineDownDialog(false);
    setSelectedBreakdownReason('');
  };

  const statusConfig = getMachineStatusConfig();
  const progress = (productInfo.completed / productInfo.target) * 100;
  const isTargetReached = productInfo.completed >= productInfo.target;

  return (
    <div className="min-h-screen w-full bg-slate-100 flex flex-col">
      {/* Top Bar */}
      <div className="bg-white shadow-sm border-b-2 border-slate-200 px-6 py-4 flex justify-between items-center shrink-0">
        <div className="flex items-center gap-3">
          <div className="p-2 bg-blue-100 rounded-lg">
            <Settings className="h-6 w-6 text-blue-600" />
          </div>
          <div>
            <h1 className="text-2xl font-bold text-slate-900">Smart Factory Operator Kiosk</h1>
            <p className="text-lg text-slate-600">စမတ်စက်မှုလုပ်ငန်းအော်ပရေတာကိုင်က်</p>
          </div>
        </div>
        <div className="text-right">
          <div className="text-sm text-slate-500">Current Time / လက်ရှိအချိန်</div>
          <div className="text-2xl font-mono font-bold text-slate-900">
            {currentTime.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', second: '2-digit' })}
          </div>
          <div className="text-lg text-slate-600">
            {currentTime.toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric' })}
          </div>
        </div>
      </div>

      {/* Main Content with Vertical Scroll */}
      <div className="flex-1 overflow-y-auto overflow-x-auto">
        <div className="p-4 space-y-4 min-w-[1200px]">
          {/* 1. Operator Info Section */}
          <Card className="border-0 shadow-lg">
            <CardContent className="p-4">
              <div className="flex items-center gap-3 mb-3">
                <div className="p-2 bg-green-100 rounded-full">
                  <UserCheck className="h-5 w-5 text-green-600" />
                </div>
                <div>
                  <h3 className="text-lg font-bold text-slate-900">Operator Info</h3>
                  <p className="text-sm text-slate-600">အော်ပရေတာအချက်အလက်</p>
                </div>
              </div>
              
              {operatorStatus === 'logged-out' ? (
                <div className="bg-blue-50 border-2 border-blue-200 rounded-lg p-4 text-center max-w-md">
                  <div className="w-12 h-12 bg-blue-100 rounded-full mx-auto mb-2 flex items-center justify-center animate-pulse">
                    <Scan className="h-6 w-6 text-blue-600" />
                  </div>
                  <div className="text-lg font-bold text-blue-800 mb-1">Tap RFID Card</div>
                  <div className="text-sm text-blue-700">RFID ကဒ်ထည့်ပါ</div>
                  <Button 
                    className="mt-2 w-full h-8 text-sm bg-blue-600 hover:bg-blue-700"
                    onClick={() => setOperatorStatus('logged-in')}
                  >
                    Demo Login
                  </Button>
                </div>
              ) : (
                <div className="bg-green-50 border-2 border-green-200 rounded-lg p-4 max-w-md">
                  <div className="flex items-center gap-3">
                    <div className="w-12 h-12 bg-green-500 rounded-full flex items-center justify-center">
                      <User className="h-6 w-6 text-white" />
                    </div>
                    <div>
                      <div className="text-lg font-bold text-green-800">{currentOperator.name}</div>
                      <div className="text-sm text-green-700">ID: {currentOperator.id} • Shift: {currentOperator.shift}</div>
                      <Badge className="bg-green-100 text-green-800 border-green-300 mt-1 text-xs">
                        🟢 RFID Connected
                      </Badge>
                    </div>
                  </div>
                </div>
              )}
            </CardContent>
          </Card>

          {/* 2. Current Product Section */}
          <Card className="border-0 shadow-lg">
            <CardContent className="p-4">
              <div className="flex items-center gap-3 mb-3">
                <div className="p-2 bg-purple-100 rounded-full">
                  <Package className="h-5 w-5 text-purple-600" />
                </div>
                <div>
                  <h3 className="text-lg font-bold text-slate-900">Current Product</h3>
                  <p className="text-sm text-slate-600">လက်ရှိထုတ်ကုန်</p>
                </div>
              </div>

              <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
                <div className="bg-slate-50 rounded-lg p-4">
                  <div className="text-lg font-bold text-slate-900 mb-1">{productInfo.name}</div>
                  <div className="text-base text-slate-600 mb-2">{productInfo.nameLocal}</div>
                  <div className="text-sm text-slate-500">Code: {productInfo.code}</div>
                </div>

                <div>
                  <div className="flex justify-between items-center mb-2">
                    <span className="text-sm font-medium text-slate-700">Progress / တိုးတက်မှု</span>
                    <span className="text-xl font-bold text-slate-900">{progress.toFixed(1)}%</span>
                  </div>
                  <Progress value={progress} className="h-3 mb-3" />
                  
                  <div className="grid grid-cols-2 gap-3">
                    <div className="text-center p-3 bg-blue-50 rounded-lg">
                      <div className="text-2xl font-bold text-blue-700">{productInfo.target}</div>
                      <div className="text-sm text-blue-600">Target / ပစ်မှတ်</div>
                    </div>
                    <div className="text-center p-3 bg-green-50 rounded-lg">
                      <div className="text-2xl font-bold text-green-700">{productInfo.completed}</div>
                      <div className="text-sm text-green-600">Completed / ပြီးစီး</div>
                    </div>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* 3. Pieces Completed Counter + Buttons Section */}
          <Card className="border-0 shadow-lg">
            <CardContent className="p-6">
              <div className="flex items-center gap-3 mb-4">
                <div className="p-2 bg-orange-100 rounded-full">
                  <Plus className="h-6 w-6 text-orange-600" />
                </div>
                <div>
                  <h3 className="text-xl font-bold text-slate-900">Pieces Completed</h3>
                  <p className="text-lg text-slate-600">ပြီးစီးပြီးအရေအတွက်</p>
                </div>
              </div>

              {/* QC Approval State */}
              {isTargetReached || showQCApproval ? (
                <div className="text-center py-6">
                  <div className="w-20 h-20 mx-auto mb-4 p-4 bg-green-100 rounded-full animate-bounce">
                    <CheckCircle className="w-full h-full text-green-600" />
                  </div>
                  <h2 className="text-3xl font-bold text-green-800 mb-2">🎉 TARGET REACHED!</h2>
                  <p className="text-2xl text-green-700 mb-4">ပစ်မှတ်မှီပြီ!</p>
                  <div className="bg-yellow-50 border-2 border-yellow-200 rounded-lg p-4 mb-4 max-w-md mx-auto">
                    <h3 className="text-lg font-bold text-yellow-800 mb-1">🔒 QC Approval Required</h3>
                    <p className="text-sm text-yellow-700">Quality Control approval needed</p>
                    <p className="text-xs text-yellow-600">အရည်အသွေးထိန်းချုပ်မှုအတည်ပြုချက်လိုအပ်</p>
                  </div>
                  <Button 
                    size="lg" 
                    className="h-12 text-lg bg-green-600 hover:bg-green-700 px-8"
                    onClick={() => setShowQCApproval(false)}
                  >
                    QC Approved - Continue
                  </Button>
                </div>
              ) : (
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  {/* Big Quantity Display */}
                  <div className="text-center">
                    <div className="text-6xl font-bold text-slate-900 mb-4">{entryQuantity}</div>
                    
                    {/* Large + and - Buttons */}
                    <div className="grid grid-cols-2 gap-4 mb-4">
                      <Button 
                        size="lg" 
                        variant="outline"
                        className="h-16 text-2xl border-4 border-red-300 text-red-700 hover:bg-red-50 font-bold"
                        onClick={() => handleQuantityChange(-1)}
                        disabled={entryQuantity <= 0 || machineStatus !== 'running'}
                      >
                        <Minus className="h-8 w-8" />
                      </Button>
                      <Button 
                        size="lg" 
                        className="h-16 text-2xl bg-green-600 hover:bg-green-700 font-bold"
                        onClick={() => handleQuantityChange(1)}
                        disabled={machineStatus !== 'running'}
                      >
                        <Plus className="h-8 w-8" />
                      </Button>
                    </div>
                  </div>

                  <div className="space-y-4">
                    {/* Quick Add Buttons */}
                    <div className="grid grid-cols-3 gap-2">
                      {[5, 10, 25].map((num) => (
                        <Button 
                          key={num}
                          variant="outline" 
                          className="h-12 text-lg font-bold border-2 border-slate-300 hover:bg-slate-50"
                          onClick={() => handleQuantityChange(num)}
                          disabled={machineStatus !== 'running'}
                        >
                          +{num}
                        </Button>
                      ))}
                    </div>

                    {/* Submit Button */}
                    <Button 
                      size="lg" 
                      className="w-full h-16 text-xl bg-blue-600 hover:bg-blue-700 font-bold"
                      disabled={entryQuantity <= 0 || machineStatus !== 'running'}
                      onClick={handleSubmitQuantity}
                    >
                      <Check className="h-6 w-6 mr-2" />
                      <div>
                        <div>🔵 Submit Count</div>
                        <div className="text-sm opacity-80">အရေအတွက်တင်သွင်းပါ</div>
                      </div>
                    </Button>

                    {machineStatus !== 'running' && (
                      <div className="text-center text-sm text-amber-600 bg-amber-50 p-3 rounded">
                        {machineStatus === 'idle' 
                          ? '⏸ Production paused - Resume to continue'
                          : '🔴 Machine down - Contact maintenance'
                        }
                      </div>
                    )}
                  </div>
                </div>
              )}
            </CardContent>
          </Card>

          {/* 4. Machine Status + Timer Section */}
          <Card className="border-0 shadow-lg">
            <CardContent className="p-4">
              <div className="flex items-center gap-3 mb-3">
                <div className="p-2 bg-blue-100 rounded-full">
                  <Activity className="h-5 w-5 text-blue-600" />
                </div>
                <div>
                  <h3 className="text-lg font-bold text-slate-900">Machine Status + Timer</h3>
                  <p className="text-sm text-slate-600">စက်အခြေအနေနှင့်အချိန်</p>
                </div>
              </div>

              <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
                <div>
                  <div className={`${statusConfig.color} text-white rounded-lg p-4 text-center mb-3`}>
                    <div className="text-2xl font-bold">{statusConfig.text}</div>
                    <div className="text-base">{statusConfig.textLocal}</div>
                  </div>

                  <div className="bg-slate-50 rounded-lg p-3">
                    <div className="flex justify-between items-center">
                      <span className="text-base font-medium text-slate-700">Runtime / အချိန်</span>
                      <span className="text-2xl font-mono font-bold text-slate-900">
                        {formatTime(machineStatus === 'down' ? downTime : operatorStatus === 'on-break' ? breakTime : sessionTime)}
                      </span>
                    </div>
                  </div>
                </div>

                <div className="space-y-3">
                  {/* Machine Down Reason Dropdown */}
                  {machineStatus === 'down' && (
                    <div>
                      <label className="text-sm font-medium text-slate-700 block mb-2">Breakdown Reason / ပျက်စီးရသည့်အကြောင်းရင်း</label>
                      <Select value={selectedBreakdownReason} onValueChange={setSelectedBreakdownReason}>
                        <SelectTrigger className="h-10">
                          <SelectValue placeholder="Select reason..." />
                        </SelectTrigger>
                        <SelectContent>
                          {breakdownReasons.map((reason) => (
                            <SelectItem key={reason.code} value={reason.code}>
                              <div className="flex items-center gap-2">
                                <span>{reason.icon}</span>
                                <div>
                                  <div>{reason.name}</div>
                                  <div className="text-xs text-slate-500">{reason.nameLocal}</div>
                                </div>
                              </div>
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                  )}
                  
                  <div className="grid grid-cols-2 gap-3">
                    {operatorStatus === 'on-break' ? (
                      <Button 
                        className="h-12 text-base bg-blue-600 hover:bg-blue-700 col-span-2"
                        onClick={handleResumeFromBreak}
                      >
                        <PlayCircle className="h-5 w-5 mr-2" />
                        Resume / ဆက်လက်လုပ်ဆောင်ရန်
                      </Button>
                    ) : (
                      <Button 
                        className="h-12 text-base bg-yellow-600 hover:bg-yellow-700"
                        onClick={handleStartBreak}
                        disabled={machineStatus === 'down'}
                      >
                        <Coffee className="h-5 w-5 mr-2" />
                        Break / အနားယူ
                      </Button>
                    )}
                    
                    <Dialog open={showMachineDownDialog} onOpenChange={setShowMachineDownDialog}>
                      <DialogTrigger asChild>
                        <Button 
                          className="h-12 text-base bg-red-600 hover:bg-red-700"
                          disabled={machineStatus === 'down'}
                        >
                          <AlertTriangle className="h-5 w-5 mr-2" />
                          Down / ပျက်
                        </Button>
                      </DialogTrigger>
                      <DialogContent className="max-w-lg">
                        <DialogHeader>
                          <DialogTitle>🔴 Report Machine Down</DialogTitle>
                          <DialogDescription>Select the breakdown reason</DialogDescription>
                        </DialogHeader>
                        <div className="space-y-4">
                          <Select value={selectedBreakdownReason} onValueChange={setSelectedBreakdownReason}>
                            <SelectTrigger>
                              <SelectValue placeholder="Select reason..." />
                            </SelectTrigger>
                            <SelectContent>
                              {breakdownReasons.map((reason) => (
                                <SelectItem key={reason.code} value={reason.code}>
                                  <div className="flex items-center gap-2">
                                    <span>{reason.icon}</span>
                                    <div>
                                      <div>{reason.name}</div>
                                      <div className="text-xs text-slate-500">{reason.nameLocal}</div>
                                    </div>
                                  </div>
                                </SelectItem>
                              ))}
                            </SelectContent>
                          </Select>
                          
                          <div className="flex gap-3">
                            <Button 
                              variant="outline" 
                              onClick={() => setShowMachineDownDialog(false)}
                              className="flex-1"
                            >
                              Cancel
                            </Button>
                            <Button 
                              onClick={handleMachineDown}
                              className="flex-1 bg-red-600 hover:bg-red-700"
                              disabled={!selectedBreakdownReason}
                            >
                              Report Down
                            </Button>
                          </div>
                        </div>
                      </DialogContent>
                    </Dialog>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* 5. End Shift Summary + Logout Section */}
          <Card className="border-0 shadow-lg">
            <CardContent className="p-4">
              <div className="flex items-center gap-3 mb-4">
                <div className="p-2 bg-rose-100 rounded-full">
                  <LogOut className="h-5 w-5 text-rose-600" />
                </div>
                <div>
                  <h3 className="text-lg font-bold text-slate-900">End of Shift Summary</h3>
                  <p className="text-sm text-slate-600">အလုပ်ပတ်အကျဉ်းချုပ်</p>
                </div>
              </div>
              
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
                <div className="grid grid-cols-2 gap-3">
                  <div className="text-center p-3 bg-green-50 rounded-lg">
                    <div className="text-2xl font-bold text-green-700">{productInfo.completed}</div>
                    <div className="text-sm text-green-600">Completed / ပြီးစီး</div>
                  </div>
                  
                  <div className="text-center p-3 bg-red-50 rounded-lg">
                    <div className="text-2xl font-bold text-red-700">{productInfo.defects}</div>
                    <div className="text-sm text-red-600">Defects / ချွတ်ယွင်း</div>
                  </div>
                  
                  <div className="text-center p-3 bg-yellow-50 rounded-lg">
                    <div className="text-2xl font-bold text-yellow-700">{productInfo.leftoverMaterial}kg</div>
                    <div className="text-sm text-yellow-600">Leftover / ကျန်ရှိပစ္စည်း</div>
                  </div>
                  
                  <div className="text-center p-3 bg-blue-50 rounded-lg">
                    <div className="text-xl font-bold text-blue-700">{formatTime(sessionTime)}</div>
                    <div className="text-sm text-blue-600">Work Time / အလုပ်ချိန်</div>
                  </div>
                </div>
                
                <div className="flex items-center justify-center">
                  <Button 
                    size="lg" 
                    className="h-16 text-xl bg-rose-600 hover:bg-rose-700 px-12"
                    onClick={() => setOperatorStatus('logged-out')}
                  >
                    <LogOut className="h-6 w-6 mr-3" />
                    <div>
                      <div>✅ Confirm & Logout</div>
                      <div className="text-base opacity-80">အတည်ပြုပြီးထွက်ရန်</div>
                    </div>
                  </Button>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}