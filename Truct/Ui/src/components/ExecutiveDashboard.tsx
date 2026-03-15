import React, { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Avatar, AvatarImage, AvatarFallback } from './ui/avatar';
import { 
  Factory,
  Users,
  Package,
  Activity,
  AlertTriangle,
  CheckCircle,
  Clock,
  TrendingUp,
  TrendingDown,
  Target,
  Download,
  RefreshCw,
  MessageCircle,
  X,
  Send,
  Mic,
  FileText,
  Image,
  Zap,
  Settings,
  Calendar,
  User,
  ChevronRight,
  ChevronDown
} from 'lucide-react';
import { 
  BarChart, 
  Bar, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  ResponsiveContainer, 
  PieChart, 
  Pie, 
  Cell,
  LineChart,
  Line
} from 'recharts';

export function ExecutiveDashboard() {
  const [currentTime, setCurrentTime] = useState(new Date());
  const [languageToggle, setLanguageToggle] = useState<'en' | 'mm'>('en');
  const [chatOpen, setChatOpen] = useState(false);
  const [chatMessage, setChatMessage] = useState('');
  const [alerts, setAlerts] = useState([
    { id: 1, message: 'Machine MCH-05 is down for 45 min', type: 'error', time: '10:30 AM' },
    { id: 2, message: 'Production target missed by 20% today', type: 'warning', time: '09:15 AM' }
  ]);

  // Auto-refresh time every second
  useEffect(() => {
    const timer = setInterval(() => setCurrentTime(new Date()), 1000);
    return () => clearInterval(timer);
  }, []);

  // Sample data
  const todayKPIs = {
    totalUnits: 9400,
    defectiveProducts: 320,
    qrPrintedGoods: 8800
  };

  const machineData = [
    {
      id: 'MCH-01',
      status: 'running',
      product: 'Plastic Bottle 500ml',
      target: 1200,
      done: 1150,
      progress: 95.8,
      downtime: null,
      operator: 'Ko Zaw Min'
    },
    {
      id: 'MCH-02',
      status: 'idle',
      product: 'Food Container Large',
      target: 800,
      done: 680,
      progress: 85.0,
      downtime: 'Material Shortage',
      operator: 'Ma Thandar'
    },
    {
      id: 'MCH-03',
      status: 'running',
      product: 'Plastic Cup 300ml',
      target: 1000,
      done: 1080,
      progress: 108.0,
      downtime: null,
      operator: 'Ko Aung Aung'
    },
    {
      id: 'MCH-04',
      status: 'down',
      product: 'Storage Box Medium',
      target: 600,
      done: 250,
      progress: 41.7,
      downtime: 'Mold Breakage',
      operator: 'Ma Phyu Phyu'
    },
    {
      id: 'MCH-05',
      status: 'running',
      product: 'Bottle Cap Standard',
      target: 2000,
      done: 1950,
      progress: 97.5,
      downtime: null,
      operator: 'Ko Min Thu'
    }
  ];

  const operatorData = [
    {
      name: 'Ko Zaw Min',
      machine: 'MCH-01',
      shift: 'Morning',
      output: 1150,
      status: 'Active',
      photo: 'KZ'
    },
    {
      name: 'Ma Thandar',
      machine: 'MCH-02', 
      shift: 'Morning',
      output: 680,
      status: 'Active',
      photo: 'MT'
    },
    {
      name: 'Ko Aung Aung',
      machine: 'MCH-03',
      shift: 'Morning', 
      output: 1080,
      status: 'Active',
      photo: 'KA'
    },
    {
      name: 'Ma Phyu Phyu',
      machine: 'MCH-04',
      shift: 'Evening',
      output: 250,
      status: 'Inactive',
      photo: 'MP'
    },
    {
      name: 'Ko Min Thu',
      machine: 'MCH-05',
      shift: 'Evening',
      output: 1950,
      status: 'Active',
      photo: 'KM'
    }
  ];

  const chartData = [
    { machine: 'MCH-01', target: 1200, actual: 1150 },
    { machine: 'MCH-02', target: 800, actual: 680 },
    { machine: 'MCH-03', target: 1000, actual: 1080 },
    { machine: 'MCH-04', target: 600, actual: 250 },
    { machine: 'MCH-05', target: 2000, actual: 1950 }
  ];

  const downtimeData = [
    { name: 'Mold Issue', value: 35, color: '#ef4444' },
    { name: 'Material Shortage', value: 25, color: '#f59e0b' },
    { name: 'Power Cut', value: 20, color: '#06b6d4' },
    { name: 'Operator Delay', value: 15, color: '#8b5cf6' },
    { name: 'Others', value: 5, color: '#64748b' }
  ];

  const operatorRanking = [
    { name: 'Ko Min Thu', efficiency: 118, badge: '🥇' },
    { name: 'Ko Zaw Min', efficiency: 115, badge: '🥈' },
    { name: 'Ko Aung Aung', efficiency: 108, badge: '🥉' },
    { name: 'Ma Thandar', efficiency: 102, badge: '4th' },
    { name: 'Ma Phyu Phyu', efficiency: 85, badge: '5th' }
  ];

  const chatSuggestions = [
    "Today's Output",
    "Top Downtime",
    "Who's on MCH-02?"
  ];

  const getStatusEmoji = (status: string) => {
    switch (status) {
      case 'running': return '🟢';
      case 'idle': return '🟡';
      case 'down': return '🔴';
      default: return '⚪';
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'running': return 'bg-green-100 text-green-800';
      case 'idle': return 'bg-yellow-100 text-yellow-800';
      case 'down': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const dismissAlert = (alertId: number) => {
    setAlerts(alerts.filter(alert => alert.id !== alertId));
  };

  const handleChatSubmit = () => {
    if (chatMessage.trim()) {
      // Handle chat message submission
      console.log('Chat message:', chatMessage);
      setChatMessage('');
    }
  };

  const formatTime = (date: Date) => {
    return date.toLocaleTimeString('en-US', { 
      hour: '2-digit', 
      minute: '2-digit',
      second: '2-digit',
      hour12: true 
    });
  };

  const formatDate = (date: Date) => {
    return date.toLocaleDateString('en-US', { 
      weekday: 'long',
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    });
  };

  return (
    <div className="min-h-screen bg-slate-50">
      {/* Header Section */}
      <div className="bg-slate-900 text-white p-6">
        <div className="max-w-7xl mx-auto">
          <div className="flex justify-between items-start">
            <div className="flex items-center gap-4">
              <div className="bg-blue-600 p-3 rounded-lg">
                <Factory className="h-8 w-8" />
              </div>
              <div>
                <h1 className="text-3xl font-semibold">Smart Plastic Factory</h1>
                <h2 className="text-xl text-slate-300">Executive Dashboard</h2>
                <p className="text-sm text-slate-400 mt-1">
                  {languageToggle === 'en' ? 'Executive Command Center' : 'အမှုဆောင်ဒက်ရှ်ဘုတ်'}
                </p>
              </div>
            </div>
            
            <div className="flex items-center gap-4">
              <div className="text-right">
                <div className="text-2xl font-semibold font-mono">{formatTime(currentTime)}</div>
                <div className="text-sm text-slate-300">{formatDate(currentTime)}</div>
              </div>
              
              <div className="flex gap-2">
                <Button variant="outline" size="sm" className="bg-white/10 border-white/20 text-white hover:bg-white/20">
                  <Download className="h-4 w-4 mr-2" />
                  {languageToggle === 'en' ? 'Export PDF' : 'PDF ထုတ်'}
                </Button>
                <Button variant="outline" size="sm" className="bg-white/10 border-white/20 text-white hover:bg-white/20">
                  <RefreshCw className="h-4 w-4 mr-2" />
                  {languageToggle === 'en' ? 'Refresh' : 'ပြန်လည်စတင်'}
                </Button>
                <Button 
                  onClick={() => setChatOpen(!chatOpen)}
                  variant="outline" 
                  size="sm" 
                  className="bg-blue-600/20 border-blue-400/50 text-blue-200 hover:bg-blue-600/30"
                >
                  <MessageCircle className="h-4 w-4 mr-2" />
                  {languageToggle === 'en' ? 'AI Chat' : 'AI ချတ်'}
                </Button>
                <Button
                  onClick={() => setLanguageToggle(languageToggle === 'en' ? 'mm' : 'en')}
                  variant="outline"
                  size="sm"
                  className="bg-white/10 border-white/20 text-white hover:bg-white/20"
                >
                  {languageToggle === 'en' ? 'မြန်မာ' : 'EN'}
                </Button>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Alert Banner */}
      {alerts.length > 0 && (
        <div className="bg-red-600 text-white">
          {alerts.map((alert) => (
            <div key={alert.id} className="flex items-center justify-between px-6 py-3 border-b border-red-500 last:border-b-0">
              <div className="flex items-center gap-3">
                <AlertTriangle className="h-5 w-5" />
                <span className="font-medium">{alert.message}</span>
                <span className="text-sm opacity-80">{alert.time}</span>
              </div>
              <Button
                onClick={() => dismissAlert(alert.id)}
                variant="ghost"
                size="sm"
                className="text-white hover:bg-white/20 h-8 w-8 p-0"
              >
                <X className="h-4 w-4" />
              </Button>
            </div>
          ))}
        </div>
      )}

      <div className="max-w-7xl mx-auto p-6 space-y-6">
        {/* Output Summary Panel */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <Card className="bg-gradient-to-r from-blue-600 to-blue-700 text-white border-0">
            <CardContent className="p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-blue-100 mb-1">
                    {languageToggle === 'en' ? 'Total Units Produced' : 'စုစုပေါင်းထုတ်လုပ်မှု'}
                  </p>
                  <p className="text-3xl font-bold">{todayKPIs.totalUnits.toLocaleString()}</p>
                  <p className="text-sm text-blue-100">pcs (Today)</p>
                </div>
                <Package className="h-12 w-12 text-blue-200" />
              </div>
            </CardContent>
          </Card>

          <Card className="bg-gradient-to-r from-orange-500 to-orange-600 text-white border-0">
            <CardContent className="p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-orange-100 mb-1">
                    {languageToggle === 'en' ? 'Defective Products' : 'ချွတ်ယွင်းထုတ်ကုန်များ'}
                  </p>
                  <p className="text-3xl font-bold">{todayKPIs.defectiveProducts}</p>
                  <p className="text-sm text-orange-100">pcs (3.4%)</p>
                </div>
                <AlertTriangle className="h-12 w-12 text-orange-200" />
              </div>
            </CardContent>
          </Card>

          <Card className="bg-gradient-to-r from-green-600 to-green-700 text-white border-0">
            <CardContent className="p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-green-100 mb-1">
                    {languageToggle === 'en' ? 'QR-Printed Finished Goods' : 'QR ကုဒ်ပါထုတ်ကုန်များ'}
                  </p>
                  <p className="text-3xl font-bold">{todayKPIs.qrPrintedGoods.toLocaleString()}</p>
                  <p className="text-sm text-green-100">pcs (93.6%)</p>
                </div>
                <CheckCircle className="h-12 w-12 text-green-200" />
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Machine View Panel */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Activity className="h-5 w-5 text-blue-600" />
              {languageToggle === 'en' ? 'Live Machine Status Monitor' : 'တိုက်ရိုက်စက်အခြေအနေစစ်ကြည့်မှု'}
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b">
                    <th className="text-left p-3 font-medium">
                      {languageToggle === 'en' ? 'Machine ID' : 'စက်နံပါတ်'}
                    </th>
                    <th className="text-left p-3 font-medium">
                      {languageToggle === 'en' ? 'Status' : 'အခြေအနေ'}
                    </th>
                    <th className="text-left p-3 font-medium">
                      {languageToggle === 'en' ? 'Product Name' : 'ထုတ်ကုန်အမည်'}
                    </th>
                    <th className="text-left p-3 font-medium">
                      {languageToggle === 'en' ? 'Target vs Done' : 'ပစ်မှတ်နှင့်ပြီးစီး'}
                    </th>
                    <th className="text-left p-3 font-medium">
                      {languageToggle === 'en' ? 'Progress' : 'တိုးတက်မှု'}
                    </th>
                    <th className="text-left p-3 font-medium">
                      {languageToggle === 'en' ? 'Downtime Reason' : 'ရပ်နားချိန်အကြောင်းရင်း'}
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {machineData.map((machine) => (
                    <tr key={machine.id} className="border-b hover:bg-slate-50">
                      <td className="p-3 font-semibold">{machine.id}</td>
                      <td className="p-3">
                        <div className="flex items-center gap-2">
                          <span className="text-lg">{getStatusEmoji(machine.status)}</span>
                          <Badge className={getStatusColor(machine.status)}>
                            {machine.status.charAt(0).toUpperCase() + machine.status.slice(1)}
                          </Badge>
                        </div>
                      </td>
                      <td className="p-3">{machine.product}</td>
                      <td className="p-3">
                        <div className="text-sm">
                          <div className="font-medium">{machine.done} / {machine.target}</div>
                          <div className="text-slate-500">units</div>
                        </div>
                      </td>
                      <td className="p-3">
                        <div className="flex items-center gap-2">
                          <Progress value={machine.progress} className="w-20" />
                          <span className="text-sm font-medium">{machine.progress}%</span>
                        </div>
                      </td>
                      <td className="p-3">
                        {machine.downtime ? (
                          <Badge variant="destructive" className="text-xs">
                            {machine.downtime}
                          </Badge>
                        ) : (
                          <span className="text-slate-400 text-sm">-</span>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </CardContent>
        </Card>

        {/* Operator Assignment Panel */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Users className="h-5 w-5 text-purple-600" />
              {languageToggle === 'en' ? 'Operator Assignment' : 'အလုပ်သမားတာဝန်ခွဲဝေမှု'}
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4">
              {operatorData.map((operator) => (
                <div key={operator.name} className="bg-white border border-slate-200 rounded-lg p-4">
                  <div className="flex items-center gap-3 mb-3">
                    <Avatar className="h-10 w-10">
                      <AvatarFallback className="bg-blue-100 text-blue-700">
                        {operator.photo}
                      </AvatarFallback>
                    </Avatar>
                    <div className="flex-1 min-w-0">
                      <p className="font-medium text-sm truncate">{operator.name}</p>
                      <p className="text-xs text-slate-500">{operator.machine}</p>
                    </div>
                  </div>
                  <div className="space-y-2 text-sm">
                    <div className="flex justify-between">
                      <span className="text-slate-500">Shift:</span>
                      <span className="font-medium">{operator.shift}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-slate-500">Output:</span>
                      <span className="font-medium">{operator.output}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-slate-500">Status:</span>
                      <Badge 
                        className={operator.status === 'Active' ? 
                          'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}
                      >
                        {operator.status}
                      </Badge>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Analytics & Charts Section */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Production Output Chart */}
          <Card className="lg:col-span-2">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <TrendingUp className="h-5 w-5 text-blue-600" />
                {languageToggle === 'en' ? 'Production Output (Today)' : 'ထုတ်လုပ်မှုပမာဏ (ဒီနေ့)'}
              </CardTitle>
            </CardHeader>
            <CardContent>
              <ResponsiveContainer width="100%" height={300}>
                <BarChart data={chartData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="machine" />
                  <YAxis />
                  <Tooltip />
                  <Bar dataKey="target" fill="#e2e8f0" name="Target" />
                  <Bar dataKey="actual" fill="#3b82f6" name="Actual" />
                </BarChart>
              </ResponsiveContainer>
            </CardContent>
          </Card>

          {/* Operator Performance Ranking */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Target className="h-5 w-5 text-green-600" />
                {languageToggle === 'en' ? 'Top Performers' : 'အကောင်းဆုံးစွမ်းဆောင်သူများ'}
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                {operatorRanking.map((operator, index) => (
                  <div key={operator.name} className="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
                    <div className="flex items-center gap-3">
                      <span className="text-lg">{operator.badge}</span>
                      <div>
                        <p className="font-medium text-sm">{operator.name}</p>
                        <p className="text-xs text-slate-500">{operator.efficiency}% efficiency</p>
                      </div>
                    </div>
                    <Badge className="bg-green-100 text-green-800">
                      #{index + 1}
                    </Badge>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Machine Downtime Chart */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Clock className="h-5 w-5 text-orange-600" />
              {languageToggle === 'en' ? 'Machine Downtime Analysis (Top 5 Reasons)' : 'စက်ရပ်နားချိန်ခွဲခြမ်းစိတ်ဖြာမှု'}
            </CardTitle>
          </CardHeader>
          <CardContent>
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={downtimeData}
                  cx="50%"
                  cy="50%"
                  labelLine={false}
                  label={({ name, value }) => `${name}: ${value}%`}
                  outerRadius={100}
                  fill="#8884d8"
                  dataKey="value"
                >
                  {downtimeData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.color} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
      </div>

      {/* AI Chat Box (Collapsible) */}
      {chatOpen && (
        <div className="fixed right-6 bottom-6 w-80 h-96 bg-white border border-slate-200 rounded-lg shadow-xl z-50">
          <div className="bg-gradient-to-r from-blue-600 to-blue-700 text-white p-4 rounded-t-lg">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <MessageCircle className="h-5 w-5" />
                <h3 className="font-medium">AI Assistant</h3>
              </div>
              <Button
                onClick={() => setChatOpen(false)}
                variant="ghost"
                size="sm"
                className="text-white hover:bg-white/20 h-6 w-6 p-0"
              >
                <X className="h-4 w-4" />
              </Button>
            </div>
          </div>
          
          <div className="p-4 h-64 overflow-y-auto">
            <div className="space-y-3">
              <div className="bg-blue-50 p-3 rounded-lg">
                <p className="text-sm">Hello! I'm your AI assistant. How can I help you with factory operations today?</p>
              </div>
              
              <div className="text-sm text-slate-600">
                <p className="mb-2">Quick suggestions:</p>
                <div className="flex flex-wrap gap-2">
                  {chatSuggestions.map((suggestion) => (
                    <Button
                      key={suggestion}
                      onClick={() => setChatMessage(suggestion)}
                      variant="outline"
                      size="sm"
                      className="text-xs h-6"
                    >
                      {suggestion}
                    </Button>
                  ))}
                </div>
              </div>
            </div>
          </div>
          
          <div className="p-4 border-t">
            <div className="flex gap-2 mb-2">
              <Button variant="outline" size="sm" className="h-6 text-xs">
                <FileText className="h-3 w-3 mr-1" />
                PDF
              </Button>
              <Button variant="outline" size="sm" className="h-6 text-xs">
                <Image className="h-3 w-3 mr-1" />
                PNG
              </Button>
            </div>
            <div className="flex gap-2">
              <Input
                value={chatMessage}
                onChange={(e) => setChatMessage(e.target.value)}
                placeholder="Type your question..."
                className="flex-1 h-8 text-sm"
                onKeyPress={(e) => e.key === 'Enter' && handleChatSubmit()}
              />
              <Button onClick={handleChatSubmit} size="sm" className="h-8 w-8 p-0">
                <Send className="h-3 w-3" />
              </Button>
              <Button variant="outline" size="sm" className="h-8 w-8 p-0">
                <Mic className="h-3 w-3" />
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default ExecutiveDashboard;